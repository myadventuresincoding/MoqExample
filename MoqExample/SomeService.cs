using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoqExample
{
    public interface ISomeService
    {
        SomeStuff GetNextStuff();
        void DoStuff();
    }

    public class SomeService : ISomeService
    {
        public SomeStuff GetNextStuff()
        {
            return new SomeStuff();
        }

        public void DoStuff()
        {
        }
    }
}
