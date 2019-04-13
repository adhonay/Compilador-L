using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador_L
{
    public class TabelaSimbolos
    {
        Dictionary<string, Simbolos> tabela;

        public TabelaSimbolos()
        {
            tabela = new Dictionary<string, Simbolos>();

            tabela.Add("if", new Simbolos("if", 0));
            tabela.Add("for", new Simbolos("for", 0));
        }
        
    }
}
