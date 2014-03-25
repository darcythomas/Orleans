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

using Orleans;
using System.Threading.Tasks;

namespace AdventureGrainInterfaces
{
    /// <summary>
    /// A room is any location in a game.
    /// </summary>
    [ExtendedPrimaryKey]
    public interface IRoomGrain : Orleans.IGrain
    {
        // Rooms have a textual description
        Task<string> Description();
        Task SetDescription(string description);

        // Rooms have exits to other rooms
        Task SetExits(IRoomGrain northRoomGrain, IRoomGrain southRoomGrain, IRoomGrain eastRoomGrain, IRoomGrain westRoomGrain);
        Task<IRoomGrain> NorthGrain();
        Task<IRoomGrain> SouthGrain();
        Task<IRoomGrain> EastGrain();
        Task<IRoomGrain> WestGrain();

        // Players can enter or exit a room
        Task Enter(PlayerInfo player);
        Task Exit(PlayerInfo player);

        // Players can enter or exit a room
        Task Enter(MonsterInfo monster);
        Task Exit(MonsterInfo monster);

        // Things can be dropped or taken from a room
        Task Drop(Thing thing);
        Task Take(Thing thing);
        Task<Thing> FindThing(string name);
    }



}
