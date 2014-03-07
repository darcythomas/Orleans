using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Orleans;

namespace HelloWorldInterfaces
{
    /// <summary>
    /// Orleans grain communication interface IGrain1
    /// </summary>
    public interface IHello : Orleans.IGrain
    {
        Task<string> SayHello(string greeting);
    }
}
