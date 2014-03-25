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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GPSTracker.FakeDeviceGateway
{
    class Program
    {
        static int counter = 0;
        static Random rand = new Random();

        static void Main(string[] args)
        {
            OrleansClient.Initialize("LocalConfiguration.xml");

            var devices = new List<Model>();
            for (var i = 0; i < 20; i++)
            {
                devices.Add(new Model()
                {
                    DeviceId = i,
                    Lat = rand.NextDouble(30, 47),
                    Lon = rand.NextDouble(-122, -81),
                    Direction = rand.NextDouble(-Math.PI, Math.PI),
                    Speed = rand.NextDouble(0, 0.05)
                });
            }


            var timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.Elapsed += (s, e) =>
            {
                Console.WriteLine(counter);
                Interlocked.Exchange(ref counter, 0);
            };
            timer.Start();

            var tasks = new List<Task>();

            foreach (var model in devices)
            {
                var ts = new ThreadStart(() =>
                {
                    while (true)
                    {
                        try
                        {
                            SendMessage(model).Wait();
                            Thread.Sleep(1000);
                        }
                        catch (Exception ex)
                        {
                            ex.ToString().Log();
                        }

                    }
                });
                new Thread(ts).Start();
            }
            tasks.WhenAll().Wait();
            Thread.Sleep(20);

        }

        private static async Task SendMessage(Model model)
        {
            model.Direction += rand.NextDouble(-0.001, 0.001);
            model.Speed += rand.NextDouble(-0.001, 0.001);
            model.Lat += Math.Cos(model.Direction) * model.Speed;
            model.Lon += Math.Sin(model.Direction) * model.Speed;
            model.Speed = model.Speed.Cap(0, 0.2);
            model.Lat = model.Lat.Cap(30, 47);
            model.Lon = model.Lon.Cap(-120, -81);
            var device = DeviceGrainFactory.GetGrain(model.DeviceId);
            Interlocked.Increment(ref counter);
            await device.ProcessMessage(new DeviceMessage(model.Lat, model.Lon, counter, model.DeviceId, DateTime.UtcNow));
        }

        class Model
        {
            public int DeviceId { get; set; }
            public double Lat { get; set; }
            public double Lon { get; set; }
            public double Direction { get; set; }
            public double Speed { get; set; }
        }

    }
}
