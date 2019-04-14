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
        /*
         * Dictionary corresponde a tupla: palavra chave - simbolo
         * Instancias da classe simbolo recebe lexema e token
         * Tokens palavras reservadas da linguagem:
        */
        public static readonly byte CONST = 0;
        public static readonly byte VAR = 1;
        public static readonly byte INTEGER = 2;
        public static readonly byte CHAR = 3;
        public static readonly byte FOR = 4;
        public static readonly byte IF = 5;
        public static readonly byte ELSE = 6;
        public static readonly byte AND = 7;
        public static readonly byte OR = 8;
        public static readonly byte NOT = 9;
        public static readonly byte TO = 10;
        public static readonly byte DO = 11;
        public static readonly byte THEN = 12;
        public static readonly byte READLN = 13;
        public static readonly byte STEP = 14;
        public static readonly byte WRITE = 15;
        public static readonly byte WRITELN = 16;
        public static readonly byte IGUAL = 17;
        public static readonly byte ABPARENTESES = 18;
        public static readonly byte FEPARENTESES = 19;
        public static readonly byte MENOR = 20;
        public static readonly byte MAIOR = 21;
        public static readonly byte DIFERENTE = 22;
        public static readonly byte MAIORIGUAL = 23;
        public static readonly byte MENORIGUAL = 24;
        public static readonly byte VIRGULA = 25;
        public static readonly byte MAIS = 26;
        public static readonly byte MENOS = 27;
        public static readonly byte MULTIPLICACAO = 28;
        public static readonly byte DIVISAO = 29;
        public static readonly byte PONTOVIRGULA = 30;
        public static readonly byte ABCHAVE = 31;
        public static readonly byte FECHAVE = 32;
        public static readonly byte PORCENTAGEM = 33;
        public static readonly byte ABCOLCHETE = 34;
        public static readonly byte FECOLCHETE = 35;
        /*
         * Identificador é inserido na tabela sob demanda,
         */
        public static readonly byte ID = 36;
        public static readonly byte EOF = Byte.MaxValue;

        Dictionary<string, Simbolos> tabela;

        public TabelaSimbolos()
        {
            tabela = new Dictionary<string, Simbolos>();

            tabela.Add("const", new Simbolos("const", CONST));
            tabela.Add("var", new Simbolos("var", VAR));
            tabela.Add("integer", new Simbolos("integer", INTEGER));
            tabela.Add("char", new Simbolos("char", CHAR));
            tabela.Add("for", new Simbolos("for", FOR));
            tabela.Add("if", new Simbolos("if", IF));
            tabela.Add("else", new Simbolos("else", ELSE));
            tabela.Add("and", new Simbolos("and", AND));
            tabela.Add("or", new Simbolos("or", OR));
            tabela.Add("not", new Simbolos("not", NOT));
            tabela.Add("to", new Simbolos("to", TO));
            tabela.Add("do", new Simbolos("do", DO));
            tabela.Add("then", new Simbolos("then", THEN));
            tabela.Add("readln", new Simbolos("readln", READLN));
            tabela.Add("step", new Simbolos("step", STEP));
            tabela.Add("write", new Simbolos("write", WRITE));
            tabela.Add("writeln", new Simbolos("writeln", WRITELN));
            tabela.Add("=", new Simbolos("=", IGUAL));
            tabela.Add("(", new Simbolos("(", ABPARENTESES));
            tabela.Add(")", new Simbolos(")", FEPARENTESES));
            tabela.Add("<", new Simbolos("<", MENOR));
            tabela.Add(">", new Simbolos(">", MAIOR));
            tabela.Add("<>", new Simbolos("<>", DIFERENTE));
            tabela.Add(">=", new Simbolos(">=", MAIORIGUAL));
            tabela.Add("<=", new Simbolos("<=", MENORIGUAL));
            tabela.Add(",", new Simbolos(",", VIRGULA));
            tabela.Add("+", new Simbolos("+", MAIS));
            tabela.Add("-", new Simbolos("-", MENOS));
            tabela.Add("*", new Simbolos("*", MULTIPLICACAO));
            tabela.Add("/", new Simbolos("/", DIVISAO));
            tabela.Add(";", new Simbolos(";", PONTOVIRGULA));
            tabela.Add("{", new Simbolos("{", ABCHAVE));
            tabela.Add("}", new Simbolos("}", FECHAVE));
            tabela.Add("%", new Simbolos("%", PORCENTAGEM));
            tabela.Add("[", new Simbolos("[", ABCOLCHETE));
            tabela.Add("]", new Simbolos("]", FECOLCHETE));

        }

        public void Listar()
        {
            foreach (var teste in tabela)
            {
                Console.WriteLine(teste.Value.toString());
            }
        }

        public int? buscarEndereco(string lexema)
        {
            if (tabela.TryGetValue(lexema, out Simbolos endereco))
                return endereco.endereco;
            return null;
        }
        public Simbolos buscarSimbolo(string lexema)
        {
            return tabela.Where(o => o.Key == lexema).FirstOrDefault().Value;
        }
        public Simbolos inserirIdentificador(string lexema)
        {
            try
            {
                tabela.Add(lexema, new Simbolos(lexema, ID));
            }
            catch (Exception e)
            {
                Console.Write("Já foi adicionado um item com a mesma chave.");
            }
            return tabela.Where(o => o.Key == lexema).FirstOrDefault().Value;
        }


    }
}
