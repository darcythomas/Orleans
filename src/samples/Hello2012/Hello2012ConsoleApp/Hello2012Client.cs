using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans.Samples.Hello2012.GrainInterfaces;

namespace Orleans.Samples.Hello2012.Client
{
    class Hello2012Client
    {
        private string name;

        public Hello2012Client(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                this.name = args[0];
            }
            else
            {
                this.name = "New Orleans User";
            }
        }

        public async Task<int> Run()
        {
            try
            {
                OrleansClient.Initialize();

                IHello2012TaskGrain grainRef = Hello2012TaskGrainFactory.GetGrain(0);

                string reply = await grainRef.SayHelloAsync(name);

                Console.WriteLine("Hello2012 grain said: " + reply);

                return 0;
            }
            catch (Exception exc)
            {
                Console.WriteLine("Error calling HelloOrleans grain: " + exc.ToString());

                return 1;
            }
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            var prog = new Hello2012Client(args);
            int rc = prog.Run().Result;

            Console.WriteLine("--> Press any key to exit program <--");
            Console.ReadKey();

            Environment.Exit(rc);
        }
    }
}
