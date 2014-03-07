using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans;
using Orleans.Samples.Hello2012.GrainInterfaces;

namespace Orleans.Samples.Hello2012.Grains
{
    /// <summary>
    /// Orleans grain implementation class for IHello2012TaskGrain
    /// </summary>
    public class Hello2012Grain : Orleans.GrainBase, IHello2012TaskGrain
    {
        IHelloEchoTaskGrain grainRef;

        public override Task ActivateAsync()
        {
            // Handle initialization of this activation of this grain
            grainRef = HelloEchoTaskGrainFactory.GetGrain(0);
            return TaskDone.Done;
        }

        public override Task DeactivateAsync()
        {
            // Handle cleanup of this activation of this grain
            grainRef = null;
            return TaskDone.Done;
        }

        public async Task<string> SayHelloAsync(string name)
        {
            string reply = await grainRef.EchoHelloAsync(name);
            Console.WriteLine("Hello2012Grain: '{0}'", reply);
            return reply;
        }
    }

    /// <summary>
    /// Orleans grain implementation class for IHelloEchoTaskGrain
    /// </summary>
    public class HelloEchoGrain : GrainBase, IHelloEchoTaskGrain
    {
        private string lastEcho;

        public Task<string> LastEchoAsync { 
            get { return Task.FromResult(lastEcho); } 
        }

        public Task<string> EchoHelloAsync(string name)
        {
            string reply = "Hello " + name;
            Console.WriteLine("HelloEchoGrain saying: '{0}'", reply);
            lastEcho = reply;
            return LastEchoAsync;
        }
    }
}
