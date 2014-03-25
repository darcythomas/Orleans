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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GPSTracker.FakeDeviceGateway
{
    public static class ExtensionMethods
    {

        public static double NextDouble(this Random rand, double min, double max)
        {
            return (rand.NextDouble() * (max - min)) + min;
        }


        public static double Cap(this double value, double min, double max)
        {
            return Math.Min(max, Math.Max(min, value));
        }

        public static int Cap(this int value, int min, int max)
        {
            return Math.Min(max, Math.Max(min, value));
        }

        public static float Cap(this float value, float min, float max)
        {
            return Math.Min(max, Math.Max(min, value));
        }

        public static string Log(this string value)
        {
            Console.WriteLine(value);
            return value;
        }

        public static Exception Log(this Exception value)
        {
            Console.WriteLine(value.ToString());
            return value;
        }

        public static async Task WhenAll(this IEnumerable<Task> tasks)
        {
            await Task.WhenAll(tasks);
        }

    }
}
