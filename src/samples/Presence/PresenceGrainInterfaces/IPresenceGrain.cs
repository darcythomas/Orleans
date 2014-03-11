using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Orleans;

namespace Orleans.Samples.Presence.GrainInterfaces
{
    /// <summary>
    /// Defines an interface for sending binary updates without knowing the specific game ID.
    /// Simulates what game consoles do when they send data to the cloud.
    /// </summary>
    [StatelessWorker]
    public interface IPresenceGrain : IGrain
    {
        Task Heartbeat(byte[] data);
    }
}
