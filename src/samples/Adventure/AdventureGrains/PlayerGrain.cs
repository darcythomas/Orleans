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
    public class PlayerGrain : Orleans.GrainBase, IPlayerGrain
    {
        string name = "nobody";
        IRoomGrain roomGrain; // Current room
        List<Thing> things = new List<Thing>(); // Things that the player is carrying

        Task<string> IPlayerGrain.Name()
        {
            return Task.FromResult(name);
        }

        Task<IRoomGrain> IPlayerGrain.RoomGrain()
        {
            return Task.FromResult(roomGrain);
        }


        async Task IPlayerGrain.Die()
        {
            // Drop everything
            var tasks = new List<Task<string>>();
            foreach (var thing in things)
            {
                tasks.Add(this.Drop(thing));
            }
            await Task.WhenAll(tasks);

            // TODO 
            string myKeyExt;
            this.GetPrimaryKey(out myKeyExt);
            var myInfo = new PlayerInfo { Key = myKeyExt, Name = this.name };

            // Exit the game
            await this.roomGrain.Exit(myInfo);
        }

        async Task<string> Drop(Thing thing)
        {
            if (thing != null) 
            {
                this.things.Remove(thing);
                await this.roomGrain.Drop(thing);
                return "Okay.";
            }
            else
                return "I don't understand.";
        }

        async Task<string> Take(Thing thing)
        {
            if (thing != null) 
            {
                this.things.Add(thing);
                await this.roomGrain.Take(thing);
                return "Okay.";
            }
            else
                return "I don't understand.";
        }


        Task IPlayerGrain.SetName(string name)
        {
            this.name = name;
            return TaskDone.Done;
        }

        Task IPlayerGrain.SetRoomGrain(IRoomGrain room)
        {
            this.roomGrain = room;
            // TODO 
            string myKeyExt;
            this.GetPrimaryKey(out myKeyExt);
            return room.Enter(new PlayerInfo { Key = myKeyExt, Name = this.name });
        }

        async Task<string> Go(string direction)
        {
            IRoomGrain destination = null;

            switch (direction)
            {
                case "north":
                    destination = await this.roomGrain.NorthGrain();
                    break;
                case "south":
                    destination = await this.roomGrain.SouthGrain();
                    break;
                case "east":
                    destination = await this.roomGrain.EastGrain();
                    break;
                case "west":
                    destination = await this.roomGrain.WestGrain();
                    break;
                default:
                    return "I don't understand.";
            }

            string myKeyExt;
            this.GetPrimaryKey(out myKeyExt);
            var myInfo = new PlayerInfo { Key = myKeyExt, Name = this.name };

            var description = "You cannot go in that direction.";

            if (destination != null)
            {
                await this.roomGrain.Exit(myInfo);
                await destination.Enter(myInfo);

                this.roomGrain = destination;
                description = await destination.Description();
            }
            return description;
        }

        private string RemoveStopWords(string s)
        {
            string[] stopwords = new string[] { " on ", " the ", " a " };

            StringBuilder sb = new StringBuilder(s);
            foreach (string word in stopwords)
            {
                sb.Replace(word, " ");
            }

            return sb.ToString();
        }

        private Thing FindMyThing(string name)
        {
            return things.Where(x => x.Name == name).FirstOrDefault();
        }

        private string Rest(string[] words)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 1; i < words.Length; i++)
                sb.Append(words[i] + " ");

            return sb.ToString().Trim().ToLower();
        }

        async Task<string> IPlayerGrain.Play(string command)
        {
            Thing thing;
            command = RemoveStopWords(command);

            string[] words = command.Split(' ');

            string verb = words[0];

            switch (verb)
            {
                case "look":
                    return await this.roomGrain.Description();

                case "go":
                    return await Go(words[1]);

                case "north":
                case "south":
                case "east":
                case "west":
                    return await Go(verb);

                case "drop":
                    thing = FindMyThing(Rest(words));
                    return await Drop(thing);

                case "take":
                    thing = await roomGrain.FindThing(Rest(words));
                    return await Take(thing);

                case "inv":
                case "inventory": 
                    return "You are carrying: " + string.Join(" ", things.Select( x => x.Name));

                case "end":
                    return "";
            }
            return "I don't understand.";
        }
    }
}
