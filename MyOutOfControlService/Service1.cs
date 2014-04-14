using DotNetNate.Integration.Wcf.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;

namespace MyOutOfControlService
{
    
    [IsolatedAppDomainBehavior]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [Serializable]
    public class Service1 : 
        MarshalByRefObject,
        IService1
    {

        [ResourceConstraintOperationBehavior(long.MaxValue, 500000, 20000)]
        public string Operation1(int value)
        {
            int i = 0;

            var t = TimeSpan.MaxValue.TotalMilliseconds;

            while (true) {
                string s = string.Format("{0}{1}{2}{3}", i, i, i, i);
                Console.WriteLine(s);
            }           
        }
        [ResourceConstraintOperationBehavior("Fast Execution Policy")]
        public string Operation2(int value)
        {
            for (int i = 0; i < 25000; i++)
            {
                Console.WriteLine(i);
            }

            return string.Empty;
        }
        [ResourceConstraintOperationBehavior]
        public string Operation3(int value)
        {
            while (true) { }
            
        }
    }
}
