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

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Orleans;
using Orleans.Samples.Chirper2012.GrainInterfaces;

namespace Orleans.Samples.Chirper2012.Client
{
    internal class ChirperClient : IChirperViewer
    {
        private IChirperViewer viewer;
        private IChirperAccount publisher;

        public bool IsPublisher { get; private set; }
        public long UserId { get; private set; }
        public bool Snapshot { get; private set; }

        public async Task<bool> Run()
        {
            if (UserId == 0) throw new ArgumentNullException("UserId", "No user UserId provided");

            ConsoleText.WriteStatus(string.Format("ChirperClient UserId={0}", UserId));
            bool ok = true;

            try 
            {
                OrleansClient.Initialize();

                IChirperAccount account = ChirperAccountFactory.GetGrain(UserId);
                publisher = account;

                List<ChirperMessage> chirps = await account.GetReceivedMessages(10);

                // Process the most recent chirps received
                foreach (ChirperMessage c in chirps)
                {
                    this.NewChirpArrived(c);
                }

                if (Snapshot)
                {
                    Console.WriteLine("--- Press any key to exit ---");
                    Console.ReadKey();
                }
                else
                {
                    // ... and then subscribe to receive any new chirps
                    viewer = await ChirperViewerFactory.CreateObjectReference(this);
                    ConsoleText.WriteStatus("Listening for new chirps...");
                    await account.ViewerConnect(viewer);
                    // Ctrl-C to exit
                    Thread.Sleep(-1);
                }
            }
            catch (Exception exc)
            {
                ConsoleText.WriteError("Error connecting Chirper client for user=" + UserId, exc);
                ok = false;
            }

            return ok;
        }

        public Task PublishMessage(string message)
        {
            return publisher.PublishMessage(message);
        }

        #region IChirperViewer interface methods

        public void NewChirpArrived(ChirperMessage chirp)
        {
            Console.WriteLine(
                @"New chirp from @{0} at {1} on {2}: {3}",
                chirp.PublisherAlias,
                chirp.Timestamp.ToShortTimeString(),
                chirp.Timestamp.ToShortDateString(),
                chirp.Message);
        }

        public void SubscriptionAdded(ChirperUserInfo following)
        {
            Console.WriteLine(
                @"Added subscription to {0}",
                following);
        }

        public void SubscriptionRemoved(ChirperUserInfo notFollowing)
        {
            Console.WriteLine(
                @"Removed subscription to {0}",
                notFollowing);
        }

        #endregion

        public bool ParseArgs(string[] args)
        {
            IsPublisher = false;

            if (args.Length <= 0) return false;
            bool ok = true;
            int argPos = 0;
            for (int i = 0; i < args.Length; i++)
            {
                string a = args[i];
                if (a.StartsWith("-") || a.StartsWith("/"))
                {
                    a = a.ToLowerInvariant().Substring(1);
                    switch (a)
                    {
                        case "pub":
                            IsPublisher = true;
                            break;
                        case "snap":
                        case "snapshot":
                            this.Snapshot = true;
                            break;
                        case "?":
                        case "help":
                        default:
                            ok = false;
                            break;
                    }
                }
                // unqualified arguments below
                else if (argPos == 0)
                {
                    long id = 0;
                    ok = !string.IsNullOrWhiteSpace(a) && long.TryParse(a, out id);
                    this.UserId = id;
                    argPos++;
                }
                else
                {
                    ConsoleText.WriteError("Unknown command line argument: " + a);
                    ok = false;
                }

                if (!ok) break;
            }
            return ok;
        }

        public void PrintUsage()
        {
            ConsoleText.WriteUsage(Assembly.GetExecutingAssembly().GetName().Name + ".exe [/snapshot] <user ID> ");
        }
    }
}