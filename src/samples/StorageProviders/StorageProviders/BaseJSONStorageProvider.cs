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
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

using Orleans;
using Orleans.Storage;
using Orleans.Providers;

namespace Samples.StorageProviders
{
    /// <summary>
    /// Base class for JSON-based grain storage providers.
    /// </summary>
    public abstract class BaseJSONStorageProvider : IStorageProvider
    {
        /// <summary>
        /// Logger object
        /// </summary>
        public OrleansLogger Log
        {
            get; protected set;
        }

        /// <summary>
        /// Storage provider name
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Data manager instance
        /// </summary>
        /// <remarks>The data manager is responsible for reading and writing JSON strings.</remarks>
        protected IJSONStateDataManager DataManager { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public BaseJSONStorageProvider()
        {
        }

        /// <summary>
        /// Initializes the storage provider.
        /// </summary>
        /// <param name="name">The storage provider name.</param>
        /// <param name="storageProviderManager">A Orleans runtime object managing all storage providers.</param>
        public virtual Task Init(string name, IProviderRuntime providerRuntime, IProviderConfiguration config)
        {
            Log = providerRuntime.GetLogger(this.GetType().FullName, Logger.LoggerType.Application);
            return TaskDone.Done;
        }

        /// <summary>
        /// Closes the storage provider during silo shutdown.
        /// </summary>
        /// <returns></returns>
        public Task Close()
        {
            if (DataManager != null)
                DataManager.Dispose();
            DataManager = null;
            return TaskDone.Done;
        }

        /// <summary>
        /// Reads persisted state from the backing store and deserializes it into the the target
        /// grain state object.
        /// </summary>
        /// <param name="grainType">A string holding the name of the grain class.</param>
        /// <param name="grainId">Represents the long-lived identity of the grain.</param>
        /// <param name="grainState">A reference to an object to hold the persisted state of the grain.</param>
        /// <returns></returns>
        public async Task ReadStateAsync(string grainType, GrainId grainId, IGrainState grainState)
        {
            if (DataManager == null) throw new ArgumentException("DataManager property not initialized");
            var entityData = await DataManager.Read(grainState.GetType().Name, grainId.ToKeyString());
            if (entityData != null)
            {
                ConvertFromStorageFormat(grainState, entityData);
            }
        }

        /// <summary>
        /// Writes the persisted state from a grain state object into its backing store.
        /// </summary>
        /// <param name="grainType">A string holding the name of the grain class.</param>
        /// <param name="grainId">Represents the long-lived identity of the grain.</param>
        /// <param name="grainState">A reference to an object holding the persisted state of the grain.</param>
        /// <returns></returns>
        public Task WriteStateAsync(string grainType, GrainId grainId, IGrainState grainState)
        {
            if (DataManager == null) throw new ArgumentException("DataManager property not initialized");
            var entityData = ConvertToStorageFormat(grainState);
            return DataManager.Write(grainState.GetType().Name, grainId.ToKeyString(), entityData);
        }

        /// <summary>
        /// Removes grain state from its backing store, if found.
        /// </summary>
        /// <param name="grainType">A string holding the name of the grain class.</param>
        /// <param name="grainId">Represents the long-lived identity of the grain.</param>
        /// <param name="grainState">An object holding the persisted state of the grain.</param>
        /// <returns></returns>
        public Task ClearStateAsync(string grainType, GrainId grainId, GrainState grainState)
        {
            if (DataManager == null) throw new ArgumentException("DataManager property not initialized");
            DataManager.Delete(grainState.GetType().Name, grainId.ToKeyString());
            return TaskDone.Done;
        }

        /// <summary>
        /// Serializes from a grain instance to a JSON document.
        /// </summary>
        /// <param name="grainState"></param>
        /// <param name="entity"></param>
        /// <remarks>
        /// See:
        /// http://msdn.microsoft.com/en-us/library/system.web.script.serialization.javascriptserializer.aspx
        /// for more on the JSON serializer.
        /// </remarks>
        protected static string ConvertToStorageFormat(IGrainState grainState)
        {
            Dictionary<string, object> dataValues = grainState.AsDictionary();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(dataValues);
        }

        /// <summary>
        /// Constructs a grain state instance by deserializing a JSON document.
        /// </summary>
        /// <param name="grainState"></param>
        /// <param name="entity"></param>
        protected static void ConvertFromStorageFormat(IGrainState grainState, string entityData)
        {
            JavaScriptSerializer deserializer = new JavaScriptSerializer();
            object data = deserializer.Deserialize(entityData, grainState.GetType());
            var dict = ((IGrainState)data).AsDictionary();
            grainState.SetAll(dict);
        }
    }

}
