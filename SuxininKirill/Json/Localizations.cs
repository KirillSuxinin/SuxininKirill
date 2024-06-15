using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuxininKirill.Json
{
    public class Localizations
    {

        public class Rootobject
        {
            public Rus[] Rus { get; set; }
        }

        public class Rus
        {
            public string Row { get; set; }
            public string Text { get; set; }
        }

    }
}
