//*********************************************************
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
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Data.Services.Common;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

using Orleans;
using Orleans.Providers;

namespace Samples.StorageProviders
{
    /// <summary>
    /// Orleans storage provider implementation for file-backed stores.
    /// </summary>
    /// <remarks>
    /// The storage provider should be included in a deployment by adding this line to the Orleans server configuration file:
    /// 
    ///     <Provider Type="Samples.StorageProviders.OrleansFileStorage" Name="FileStore" RooDirectory="SOME FILE PATH" />
    ///
    /// and this line to any grain that uses it:
    /// 
    ///     [StorageProvider(ProviderName = "FileStore")]
    /// 
    /// The name 'FileStore' is an arbitrary choice.
    /// 
    /// Note that unless the root directory path is a network path available to all silos in a deployment, grain state
    /// will not transport from one silo to another.
    /// </remarks>
    public class OrleansFileStorage : BaseJSONStorageProvider
    {
        /// <summary>
        /// The directory path, relative to the host of the silo. Set from
        /// configuration data during initialization.
        /// </summary>
        public string RootDirectory { get; set; }

        /// <summary>
        /// Initializes the provider during silo startup.
        /// </summary>
        /// <param name="name">The name of the provider</param>
        /// <param name="storageProviderManager">A provider manager reference.</param>
        /// <returns></returns>
        public override Task Init(string name, IProviderRuntime providerRuntime, IProviderConfiguration config)
        {
            this.Name = name;
            this.RootDirectory = config.Properties["RootDirectory"];
            if (string.IsNullOrWhiteSpace(RootDirectory)) throw new ArgumentException("RootDirectory property not set");
            DataManager = new GrainStateFileDataManager(RootDirectory);
            return base.Init(name, providerRuntime, config);
        }
    }

    /// <summary>
    /// Interfaces with the file system.
    /// </summary>
    internal class GrainStateFileDataManager : IJSONStateDataManager 
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="storageDirectory">A path relative to the silo host process' working directory.</param>
        public GrainStateFileDataManager(string storageDirectory)
        {
            directory = new DirectoryInfo(storageDirectory);
            if (!directory.Exists)
                directory.Create();
        }

        /// <summary>
        /// Deletes a file representing a grain state object.
        /// </summary>
        /// <param name="typename">The type of the grain state object.</param>
        /// <param name="fileName">The grain id string.</param>
        public Task Delete(string collectionName, string key)
        {
            var fName = key + "." + collectionName;
            var path = Path.Combine(directory.FullName, fName);

            var fileInfo = new FileInfo(path);
            if (fileInfo.Exists)
                fileInfo.Delete();

            return TaskDone.Done;
        }

        /// <summary>
        /// Reads a file representing a grain state object.
        /// </summary>
        /// <param name="fileName">The grain id string.</param>
        /// <param name="typename">The type of the grain state object.</param>
        public async Task<string> Read(string collectionName, string key)
        {
            var fName = key + "." + collectionName;
            var path = Path.Combine(directory.FullName, fName);

            var fileInfo = new FileInfo(path);
            if (!fileInfo.Exists)
                return null;

            using (var stream = fileInfo.OpenText())
            {
                return await stream.ReadToEndAsync();
            }
        }

        /// <summary>
        /// Writes a file representing a grain state object.
        /// </summary>
        /// <param name="fileName">The grain id string.</param>
        /// <param name="typename">The type of the grain state object.</param>
        public async Task Write(string collectionName, string key, string entityData)
        {
            string fileName = key + "." + collectionName;
            var path = Path.Combine(directory.FullName, fileName);

            var fileInfo = new FileInfo(path);

            using (var stream = new StreamWriter(fileInfo.Open(FileMode.Create, FileAccess.Write)))
            {
                await stream.WriteAsync(entityData);
            }
        }

        public void Dispose()
        {
        }

        private DirectoryInfo directory;
    }
}
