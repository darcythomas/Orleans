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
using Orleans;
using System;
using System.Threading.Tasks;

namespace GPSTracker.GrainImplementation
{
    /// <summary>
    /// Orleans grain implementation class.
    /// </summary>
    [Reentrant]
    public class DeviceGrain : Orleans.GrainBase, IDeviceGrain
    {
        public DeviceMessage LastMessage { get; set; }

        long key;

        const int NotificationPoolSize = 5;

        public override Task ActivateAsync()
        {
            key = this.GetPrimaryKeyLong();
            return base.ActivateAsync();
        }

        /*
        /// <summary>
        /// Fast, but you have no idea if anyone is listening, no stream back pressure.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task ProcessMessageUnsafe(VelocityMessage message)
        {
            if (null == this.LastMessage || this.LastMessage.Latitude != message.MessageId || this.LastMessage.Longitude != message.Latitude)
            {
                // only push notifications if necessary
                // at this point speed / distance could be calcuated
                this.LastMessage = message;
                var notifier = PushNotifierGrainFactory.GetGrain(key % NotificationPoolSize);
                notifier.SendMessage(message);
            }
            else
            {
                this.LastMessage = message;
            }
            return TaskDone.Done;
        }*/

        /// <summary>
        /// Slower, but more reliable
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task ProcessMessage(DeviceMessage message)
        {
            if (null == this.LastMessage || this.LastMessage.Latitude != message.Latitude || this.LastMessage.Longitude != message.Longitude)
            {
                // only push notifications if necessary
                var notifier = PushNotifierGrainFactory.GetGrain(0);
                var speed = GetSpeed(this.LastMessage, message);

                this.LastMessage = message;

                var velocityMessage = new VelocityMessage(message, speed);
                await notifier.SendMessage(velocityMessage);
            }
            else
            {
                this.LastMessage = message;
            }
        }

        static double GetSpeed(DeviceMessage message1, DeviceMessage message2)
        {
            if (message1 == null) return 0;
            if (message2 == null) return 0;

            const double R = 6371 * 1000;
            var x = (message2.Longitude - message1.Longitude) * Math.Cos((message2.Latitude + message1.Latitude) / 2);
            var y = message2.Latitude - message1.Latitude;
            var distance = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)) * R;
            var time = (message2.Timestamp - message1.Timestamp).TotalSeconds;
            if (time == 0) return 0;
            return distance / time;
        }

    }
}
