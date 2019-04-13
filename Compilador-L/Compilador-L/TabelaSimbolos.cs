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

        public void Listar()
        {
            foreach (var teste in tabela)
            {
                Console.WriteLine(teste.Value.toString());
            }
        }

        public int buscarEndereco(string lexema)
        {
            return tabela.Where(o => o.Key == lexema).FirstOrDefault().Value.endereco;
        }
        public Simbolos buscarSimbolo(string lexema)
        {
            return tabela.Where(o => o.Key == lexema).FirstOrDefault().Value;
        }
        //public Simbolos inserirEndereco(string lexema)
        //{
        //    return tabela.Add(lexema, new Simbolos(lexema));
        //}

        
    }
}
