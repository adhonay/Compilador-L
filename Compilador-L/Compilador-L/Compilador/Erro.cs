/*
 * Pontifícia Universidade Católica de Minas Gerais
 * Compilador
 * Autores: Adhonay Júnior, Izabela Costa
 * Matricula: 504656, 498535
 **/
using System;

namespace Compilador_L.Compilador
{
    public class Erro
    {
       public struct ErroLexico
       {
            public static void Char(int linha) {  Console.WriteLine (linha + ":caractere invalido."); Console.ReadKey(); Environment.Exit(0); }
            public static void Lexema(int linha, string lex) { Console.WriteLine(linha + ":lexema não identificado["+lex+"]."); Console.ReadKey(); Environment.Exit(0); }
            public static void Arquivo(int linha) { Console.WriteLine(linha + ":fim de arquivo não esperado."); Console.ReadKey(); Environment.Exit(0); }
       }
		public struct ErroSintatico
		{
			public static void Lexema(int linha, string lex) { Console.WriteLine(linha + ":Token não esperado[" + lex + "]."); Console.ReadKey(); Environment.Exit(0); }
			public static void Arquivo(int linha) { Console.WriteLine(linha + ":fim de arquivo não esperado."); Console.ReadKey(); Environment.Exit(0); }

		}
        public struct ErroSemantico
        {
            public static void NaoDeclarado(int linha, string lex) { Console.WriteLine(linha + ":identificador nao declarado[" + lex + "]."); Console.ReadKey(); Environment.Exit(0); }
            public static void Declarado (int linha, string lex) { Console.WriteLine(linha + ":identificador ja declarado[" + lex + "]."); Console.ReadKey(); Environment.Exit(0); }
            public static void Classe(int linha, string lex) { Console.WriteLine(linha + ":classe de identificador incompatível[" + lex + "]."); Console.ReadKey(); Environment.Exit(0); }
            public static void Tipos(int linha) { Console.WriteLine(linha + ":tipos incompatíveis."); Console.ReadKey(); Environment.Exit(0); }
            public static void Tamanho(int linha) { Console.WriteLine(linha + ":tamanho do vetor excede o máximo permitido."); Console.ReadKey(); Environment.Exit(0); }


        }

    }
}
