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

using TwitterGrainInterfaces;
using Orleans;
using System.Threading.Tasks;


namespace TwitterGrains
{

    public interface ICounterState : IGrainState
    {
        int Counter { get; set; }
    }

    [StorageProvider(ProviderName = "store1")]
    [Reentrant]
    public class CounterGrain : Orleans.GrainBase<ICounterState>, ICounter
    {
        public async Task IncrementCounter()
        {
            this.State.Counter += 1;
            if (this.State.Counter % 100 == 0) await State.WriteStateAsync();
        }

        public async Task ResetCounter()
        {
            this.State.Counter = 0;
            await this.State.WriteStateAsync();
        }

        public Task<int> GetTotalCounter()
        {
            return Task.FromResult(this.State.Counter);
        }

    }
}
