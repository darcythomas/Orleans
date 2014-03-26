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
using System.Threading.Tasks;
using System.Web.Mvc;

namespace TwitterWebApplication.Controllers
{
    public class GrainController : Controller
    {

        public async Task<string> ReadInputStreamAsync()
        {
            var data = new byte[1024];
            var output = "";
            while (true)
            {
                int numBytesRead = await this.Request.InputStream.ReadAsync(data, 0, 1024);
                if (numBytesRead <= 0) return output;
                output += System.Text.Encoding.UTF8.GetString(data, 0, numBytesRead);
            }
        }

        [HttpPost]
        public async Task<ActionResult> SetScore(string hashtags, int score)
        {
            var tweet = await ReadInputStreamAsync();
            var grain = TweetDispatcherGrainFactory.GetGrain(0);
            await grain.AddScore(score, hashtags.ToLower().Split(','), tweet);
            return this.Content("");
        }

        public async Task<ActionResult> GetScores(string hashtags)
        {
            var tweetGrain = TweetDispatcherGrainFactory.GetGrain(0);
            var tweetGrainTask = tweetGrain.GetTotals(hashtags.ToLower().Split(','));

            var counterGrain = CounterFactory.GetGrain(0);
            var counterGrainTask = counterGrain.GetTotalCounter();

            await Task.WhenAll(tweetGrainTask, counterGrainTask);

            return Json(new object[] { tweetGrainTask.Result, counterGrainTask.Result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return View();
        }

    }

}