using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador_L.Compilador
{
    class Rotulo
    {
        private static int rotulo;

        public Rotulo()
        {
            rotulo = 0;
        }

        public String newRotulo()
        {
            return "R" + rotulo++;
        }

    }
}
