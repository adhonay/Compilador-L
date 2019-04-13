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

        public static readonly byte WHILE = 2;
        public static readonly byte ENDWHILE = 3;
        public static readonly byte IF = 4;
        public static readonly byte ENDIF = 5;
        public static readonly byte ELSE = 6;
        public static readonly byte ENDELSE = 7;
        public static readonly byte READLN = 8;
        public static readonly byte WRITE = 9;
        public static readonly byte WRITELN = 10;
        public static readonly byte TRUE = 11;
        public static readonly byte FALSE = 12;
        public static readonly byte BYTE = 13;
        public static readonly byte BOOLEAN = 14;
        public static readonly byte INT = 15;
        public static readonly byte STRING = 16;
        public static readonly byte AND = 17;
        public static readonly byte OR = 18;
        public static readonly byte NOT = 19;
        public static readonly byte RECEIVE = 20;
        public static readonly byte EQUALS = 21;
        public static readonly byte OPPAR = 22;
        public static readonly byte CLPAR = 23;
        public static readonly byte LESSTHAN = 24;
        public static readonly byte MORETHAN = 25;
        public static readonly byte DIFFERENT = 26;
        public static readonly byte LESSEQUAL = 27;
        public static readonly byte MOREEQUAL = 28;
        public static readonly byte PLUS = 29;
        public static readonly byte MINUS = 30;
        public static readonly byte TIMES = 31;
        public static readonly byte DIVIDE = 32;
        public static readonly byte COMMA = 33;
        public static readonly byte SEMICOLON = 34;

        public static readonly byte ID = 35;
        public static readonly byte CONST = 36;

        public static readonly byte EOF = Byte.MaxValue;

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
