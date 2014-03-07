using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Orleans;

namespace HelloWordGrains
{
    /// <summary>
    /// Orleans grain implementation class HelloGrain.
    /// </summary>
    public class HelloGrain : Orleans.GrainBase, HelloWorldInterfaces.IHello
    {
        Task<string> HelloWorldInterfaces.IHello.SayHello(string greeting)
        {
            return Task.FromResult("You said: '" + greeting + "', I say: Hello!");
        }
    }
}
