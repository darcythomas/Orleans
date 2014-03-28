﻿//*********************************************************//
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

        Dictionary<string, IRoomGrain> exits = new Dictionary<string, IRoomGrain>();

        Task IRoomGrain.Enter(PlayerInfo player)
        {
            players.Add(player);
            return TaskDone.Done;
        }

        Task IRoomGrain.Enter(MonsterInfo monster)
        {
            monsters.RemoveAll(x => x.Id == monster.Id);
            monsters.Add(monster);
            return TaskDone.Done;
        }

        Task IRoomGrain.Exit(MonsterInfo monster)
        {
            monsters.RemoveAll(x => x.Id == monster.Id);
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

        Task IRoomGrain.SetInfo(RoomInfo info)
        {
            this.description = info.Description;

            foreach (var kv in info.Directions)
            {
                this.exits.Add(kv.Key, RoomGrainFactory.GetGrain(kv.Value));
            }
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

            if (things.Count > 0)
            {
                sb.AppendLine("The following things are present:");
                foreach (var thing in things)
                {
                    sb.Append("  ").AppendLine(thing.Name);
                }
            }

            if (players.Count > 1 || monsters.Count > 0)
            {
                sb.AppendLine("Beware! These guys are in the room with you:");
                if (players.Count > 1)
                    foreach (var player in players)
                    {
                        sb.Append("  ").AppendLine(player.Name);
                    }
                if (monsters.Count > 0)
                    foreach (var monster in monsters)
                    {
                        sb.Append("  ").AppendLine(monster.Name);
                    }
            }

            return Task.FromResult(sb.ToString());
        }

        Task<IRoomGrain> IRoomGrain.ExitTo(string direction)
        {
            return Task.FromResult((exits.ContainsKey(direction)) ? exits[direction] : null);
        }
    }
}
