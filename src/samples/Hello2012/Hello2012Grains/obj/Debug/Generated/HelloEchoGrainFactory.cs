//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34011
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Orleans.Samples.Hello2012.Grains
{
    using System;
    using System.Collections.Generic;
    using Orleans;
    using Orleans.Runtime.Coordination;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.0.795.25827")]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute()]
    [SerializableAttribute()]
    [Orleans.GrainStateAttribute("Orleans.Samples.Hello2012.Grains.HelloEchoGrain")]
    public class HelloEchoGrainState : GrainState
    {
        

            public HelloEchoGrainState() : base("Orleans.Samples.Hello2012.Grains.HelloEchoGrain")
            { 
                _InitStateFields();
            }
            private void _InitStateFields()
            {
            }
            public override void SetAll(Dictionary<string,object> values)
            {   
                
            }
        
        [Orleans.CopierMethodAttribute()]
        public static object _Copier(object original)
        {
            HelloEchoGrainState input = ((HelloEchoGrainState)(original));
            return input.DeepCopy();
        }
        
        [Orleans.SerializerMethodAttribute()]
        public static void _Serializer(object original, Orleans.Serialization.BinaryTokenStreamWriter stream, System.Type expected)
        {
            HelloEchoGrainState input = ((HelloEchoGrainState)(original));
            input.SerializeTo(stream);
        }
        
        [Orleans.DeserializerMethodAttribute()]
        public static object _Deserializer(System.Type expected, Orleans.Serialization.BinaryTokenStreamReader stream)
        {
            HelloEchoGrainState result = new HelloEchoGrainState();
            result.DeserializeFrom(stream);
            return result;
        }
    }
}
