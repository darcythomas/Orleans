using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans;

namespace Orleans.Samples.Hello2012.GrainInterfaces
{
    public interface IHello2012TaskGrain : Orleans.IGrain
    {
        Task<string> SayHelloAsync(string name);
    }

    public interface IHelloEchoTaskGrain : IGrain
    {
        Task<string> LastEchoAsync { get; }
        Task<string> EchoHelloAsync(string name);
    }
}
