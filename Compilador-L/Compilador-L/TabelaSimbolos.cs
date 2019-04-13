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

        public Hashtable TabelaSim()
        {
            
            Hashtable tabela = new Hashtable();
            tabela.Add("teste", "ds");
            tabela.Add("adhonay", 2);
            return tabela;
            
        }
        
        
    }
}
