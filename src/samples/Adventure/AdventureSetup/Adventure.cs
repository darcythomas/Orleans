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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AdventureSetup
{
    public class Adventure
    {
        private async Task<IRoomGrain> MakeRoom(string id, string description, string north, string south, string east, string west)
        {
            IRoomGrain roomGrain = RoomGrainFactory.GetGrain(0, id);

            await roomGrain.SetDescription(description);

            IRoomGrain northRoomGrain = null;
            IRoomGrain southRoomGrain = null;
            IRoomGrain eastRoomGrain = null;
            IRoomGrain westRoomGrain = null;

            if (north != "")
                northRoomGrain = RoomGrainFactory.GetGrain(0, north);

            if (south != "")
                southRoomGrain = RoomGrainFactory.GetGrain(0, south);

            if (east != "")
                eastRoomGrain = RoomGrainFactory.GetGrain(0, east);

            if (west != "")
                westRoomGrain = RoomGrainFactory.GetGrain(0, west);

            await roomGrain.SetExits(northRoomGrain, southRoomGrain, eastRoomGrain, westRoomGrain);
            return roomGrain;
        }

        private async Task MakeThing(string id, string name, string room)
        {
            Thing thing = new Thing();
            thing.Name = name;
            IRoomGrain roomGrain = RoomGrainFactory.GetGrain(0, room);
            await roomGrain.Drop(thing);
        }

        private async Task MakeMonster(int id, string name, IRoomGrain room)
        {
            var monsterGrain = MonsterGrainFactory.GetGrain(id);
            monsterGrain.SetName(name);
            monsterGrain.SetRoomGrain(room);
        }


        public async Task Configure(string filename)
        {
            var rand = new Random();
            string line;
            int counter = 0;
            using (var file = new StreamReader(filename))
            {
                var rooms = new List<IRoomGrain>();
                while ((line = file.ReadLine()) != null)
                {
                    if (line.StartsWith(";")) continue;// Skip comment lines
                    var fields = line.Split('|').Select(s => s.Trim().ToLower()).ToArray();

                    switch (fields[0])
                    {
                        case "room":
                            rooms.Add(await MakeRoom(fields[1], fields[2], fields[3], fields[4], fields[5], fields[6]));
                            break;
                        case "thing":
                            await MakeThing(fields[1], fields[2], fields[3]);
                            break;
                        case "monster":
                            await MakeMonster(counter++, fields[1], rooms[rand.Next(0, rooms.Count)]);
                            break;
                    }
                }
            }
        }
    }
}
