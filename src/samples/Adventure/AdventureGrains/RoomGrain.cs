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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureGrains
{
    /// <summary>
    /// Orleans grain implementation class Grain1.
    /// </summary>
    public class RoomGrain : Orleans.GrainBase, IRoomGrain
    {
        // TODO: replace placeholder grain interface with actual grain
        // communication interface(s).

        string description;

        List<PlayerInfo> players = new List<PlayerInfo>();
        List<MonsterInfo> monsters = new List<MonsterInfo>();
        List<Thing> things = new List<Thing>();

        IRoomGrain northGrain = null;
        IRoomGrain southGrain = null;
        IRoomGrain eastGrain = null;
        IRoomGrain westGrain = null;

        Task IRoomGrain.Enter(PlayerInfo player)
        {
            players.Add(player);
            return TaskDone.Done;
        }

        Task IRoomGrain.Enter(MonsterInfo monster)
        {
            monsters.Add(monster);
            return TaskDone.Done;
        }

        Task IRoomGrain.Exit(MonsterInfo monster)
        {
            monsters.RemoveAll(x => x.Key == monster.Key);
            return TaskDone.Done;
        }

        Task IRoomGrain.Exit(PlayerInfo player)
        {
            players.RemoveAll(x => x.Key == player.Key);
            return TaskDone.Done;
        }

        Task IRoomGrain.Drop(Thing thing)
        {
            things.Add(thing);
            return TaskDone.Done;
        }

        Task IRoomGrain.Take(Thing thing)
        {
            things.RemoveAll(x => x.Name == thing.Name);
            return TaskDone.Done;
        }

        Task IRoomGrain.SetDescription(string description)
        {
            this.description = description;
            return TaskDone.Done;
        }

        Task IRoomGrain.SetExits(IRoomGrain north, IRoomGrain south, IRoomGrain east, IRoomGrain west)
        {
            this.northGrain = north;
            this.southGrain = south;
            this.eastGrain = east;
            this.westGrain = west;
            return TaskDone.Done;
        }


        Task<Thing> IRoomGrain.FindThing(string name)
        {
            return Task.FromResult(things.Where(x => x.Name == name).FirstOrDefault());
        }

        Task<string> IRoomGrain.Description()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(this.description);

            sb.Append(" Exits to the ");

            if (this.northGrain != null) sb.Append("north ");
            if (this.southGrain != null) sb.Append("south ");
            if (this.eastGrain != null) sb.Append("east ");
            if (this.westGrain != null) sb.Append("west ");

            sb.AppendLine();

            if (things.Count > 0)
            {
                sb.AppendLine(" The following items are present:");
                foreach (var thing in things)
                {
                    sb.AppendLine(thing.Name);
                }
            }

            if (players.Count > 1)
            {
                sb.AppendLine("The following players are present:");
                foreach (var player in players)
                {
                    sb.AppendLine(player.Name);
                }
            }

            if (monsters.Count > 1)
            {
                sb.AppendLine("The following monsters are present:");
                foreach (var monster in monsters)
                {
                    sb.AppendLine(monster.Name);
                }
            }

            return Task.FromResult(sb.ToString());
        }


        Task<IRoomGrain> IRoomGrain.NorthGrain()
        {
            return Task.FromResult(northGrain);
        }

        Task<IRoomGrain> IRoomGrain.SouthGrain()
        {
            return Task.FromResult(southGrain);
        }

        Task<IRoomGrain> IRoomGrain.EastGrain()
        {
            return Task.FromResult(eastGrain);
        }

        Task<IRoomGrain> IRoomGrain.WestGrain()
        {
            return Task.FromResult(westGrain);
        }
    }
}
