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

using AdventureGrainInterfaces;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureGrains
{
    public class MonsterGrain : Orleans.GrainBase, IMonsterGrain
    {
        MonsterInfo monsterInfo = new MonsterInfo();
        IRoomGrain roomGrain; // Current room

        public override Task ActivateAsync()
        {
            this.monsterInfo.Key = this.GetPrimaryKeyLong();

            RegisterTimer((_) => Move(), null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
            return base.ActivateAsync();
        }

        Task IMonsterGrain.SetName(string name)
        {
            this.monsterInfo.Name = name;
            return TaskDone.Done;
        }

        Task<string> IMonsterGrain.Name()
        {
            return Task.FromResult(this.monsterInfo.Name);
        }

        async Task IMonsterGrain.SetRoomGrain(IRoomGrain room)
        {
            this.roomGrain = room;
            await this.roomGrain.Enter(this.monsterInfo);
        }

        Task<IRoomGrain> IMonsterGrain.RoomGrain()
        {
            return Task.FromResult(roomGrain);
        }

        async Task Move()
        {
            var rand = new Random().Next(0, 4);
            IRoomGrain nextRoom = null;
            switch (rand)
            {
                case 0:
                    nextRoom = await this.roomGrain.NorthGrain();
                    break;
                case 1:
                    nextRoom = await this.roomGrain.SouthGrain();
                    break;
                case 2:
                    nextRoom = await this.roomGrain.EastGrain();
                    break;
                case 3:
                    nextRoom = await this.roomGrain.WestGrain();
                    break;
            }
            if (null == nextRoom) return;

            await Task.WhenAll(
                this.roomGrain.Exit(this.monsterInfo),
                nextRoom.Enter(this.monsterInfo));

            this.roomGrain = nextRoom;
        }

    }
}
