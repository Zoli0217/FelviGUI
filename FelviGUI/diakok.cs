using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FelviGUI
{
    class diakok
    {
        public diakok(string oktazon, string nev, int hozott, int kpmagy, int kpmat)
        {
            Oktazon = oktazon;
            Nev = nev;
            Hozott = hozott;
            Kpmagy = kpmagy;
            Kpmat = kpmat;
        }

        public string Oktazon { get; set; }
        public string Nev { get; set; }
        public int Hozott { get; set; }
        public int Kpmagy { get; set; }
        public int Kpmat { get; set; }
    }
}
