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

using System.Threading.Tasks;

namespace Orleans.Samples.Chirper2012.GrainInterfaces
{
    /// <summary>
    /// Orleans observer interface IChirperSubscriber
    /// </summary>
    public interface IChirperSubscriber : IGrain
    {
        /// <summary>Notification that a new Chirp has been received</summary>
        /// <param name="chirp">Chirp message entry</param>
        Task NewChirp(ChirperMessage chirp);
    }
}
