using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador_L.Compilador
{
    public class Erro
    {
       public struct ErroLexico
       {
            public static void Char(int linha) {  Console.WriteLine (linha + ":caractere invalido.");}
            public static void Lexema(int linha, string lex) { Console.WriteLine(linha + ":lexema nao identificado["+lex+"]"); }
            public static void Arquivo(int linha) { Console.WriteLine(linha + ":fim de arquivo nao esperado.");}
       }

    }
}
