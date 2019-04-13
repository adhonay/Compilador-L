using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador_L
{
    /*
     * classe para armazenamento dos simbolos.Os simbolos tem as seguintes informações: 
     * token
     * endereco
     * lexemas (são retirados do código fonte)
    */
    class Simbolos
    {
        private byte token { get; set; }
        private string lexema { get; set; }
        private int endereco { get; set; }

        public Simbolos(string lexema, byte token)
        {
            this.lexema = lexema;
            this.token = token;
            this.endereco = 0;

        }

        public String toString()
        {
            return "Simbolo inserido {" + "lexema: " + lexema + ", token: " + token + ", endereco: " + endereco +  '}';
        }

    }



}
