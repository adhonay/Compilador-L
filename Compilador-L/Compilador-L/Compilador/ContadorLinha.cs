using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador_L.Compilador
{

    public class LerArquivo : StreamReader
    {
        private int _numeroLinha = 0;
        public int numeroLinha { get { return _numeroLinha; } }

        public LerArquivo(Stream stream) : base(stream) { }

        public override string ReadLine()
        {
            _numeroLinha++;
            return base.ReadLine();
        }
    } 
}
