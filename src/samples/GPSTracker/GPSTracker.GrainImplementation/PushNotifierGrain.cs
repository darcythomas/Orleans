//*********************************************************//
//    Copyright (c) Microsoft. All rights reserved.
//    
//    Apache 2.0 License
//    
//    You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
//    
//    Unless required by applicable law or agreed to in writing, software 
//    distributed under the License is distributed on an "AS IS" BASIS, 
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or 
//    implied. See the License for the specific language governing 
//    permissions and limitations under the License.
//
//*********************************************************

using GPSTracker.Common;
using GPSTracker.GrainInterface;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.WindowsAzure.ServiceRuntime;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GPSTracker.GrainImplementation
{
    /// <summary>
    /// Orleans grain implementation class.
    /// </summary>
    [Reentrant]
    public class PushNotifierGrain : Orleans.GrainBase, IPushNotifierGrain
    {
        Dictionary<string, Tuple<HubConnection, IHubProxy>> hubs;
        List<VelocityMessage> messageQueue;

        public override async Task ActivateAsync()
        {
            messageQueue = new List<VelocityMessage>();

            var key = this.GetPrimaryKeyLong();
            Console.WriteLine("activating push notification grain {0}", key);

            hubs = new Dictionary<string, Tuple<HubConnection, IHubProxy>>();

            this.RegisterTimer(FlushQueue, null, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100));

            if (RoleEnvironment.IsAvailable)
            {
                // in azure
                await RefreshHubs(null);
                this.RegisterTimer(RefreshHubs, null, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(60));
            }
            else
            {
                // not in azure
                await AddHub("http://localhost:48777/");
            }

            await base.ActivateAsync();
        }

        private async Task RefreshHubs(object _)
        {
            Console.WriteLine("Refreshing Hubs");
            var addresses = new List<string>();
            var tasks = new List<Task>();

            // discover the current infrastructure
            foreach (var instance in RoleEnvironment.Roles["GPSTracker.Web"].Instances)
            {
                var endpoint = instance.InstanceEndpoints["InternalSignalR"];
                addresses.Add(string.Format("http://{0}", endpoint.IPEndpoint.ToString()));
            }
            var newHubs = addresses.Where(x => !hubs.Keys.Contains(x)).ToArray();
            var deadHubs = hubs.Keys.Where(x => !addresses.Contains(x)).ToArray();

            // remove dead hubs
            foreach (var hub in deadHubs)
            {
                hubs.Remove(hub);
            }

            // add new hubs
            foreach (var hub in newHubs)
            {
                tasks.Add(AddHub(hub));
            }
            await Task.WhenAll(tasks);

            Console.WriteLine("There are now {0} hubs", hubs.Count);
        }

        public bool Flushed = false;

        private Task FlushQueue(object _)
        {
            if (!this.Flushed)
            {
                this.Flush();
            }
            this.Flushed = false;
            return TaskDone.Done;
        }

        private async Task AddHub(string address)
        {
            var hubConnection = new HubConnection(address);
            hubConnection.Headers.Add("ORLEANS", "GRAIN");
            var hub = hubConnection.CreateHubProxy("LocationHub");
            await hubConnection.Start();
            hubs.Add(address, new Tuple<HubConnection, IHubProxy>(hubConnection, hub));
        }

        public Task SendMessage(VelocityMessage message)
        {
            messageQueue.Add(message);
            if (messageQueue.Count < 25)
            {
                Flush();
                this.Flushed = true;
            }
            return TaskDone.Done;
        }

        private void Flush()
        {
            if (messageQueue.Count == 0) return;

            var messagesToSend = messageQueue.ToArray();
            messageQueue.Clear();

            var promises = new List<Task>();
            foreach (var hub in hubs.Values)
            {
                try
                {
                    if (hub.Item1.State == ConnectionState.Connected)
                    {
                        hub.Item2.Invoke("LocationUpdates", new VelocityBatch { Messages = messagesToSend });
                    }
                    else
                    {
                        hub.Item1.Start();
                    }
                    //promises.Add(hub.Invoke("LocationUpdates", new MessageBatch { Messages = messagesToSend }));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

        }
    }
}
