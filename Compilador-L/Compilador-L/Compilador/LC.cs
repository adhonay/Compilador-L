using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Pontifícia Universidade Católica de Minas Gerais
 * Compilador
 * Autores: Adhonay Júnior, Izabela Costa
 * Matricula: 504656, 498535
 **/
namespace Compilador_L.Compilador
{
    class LC
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Digite o nome e extensão do arquivo.");
            var arquivo = "exemplo.l";//Console.ReadLine();

            if (File.Exists(arquivo))
            {

                if (arquivo.Substring(arquivo.Length - 2, 1) == "." && arquivo.Substring(arquivo.Length - 1) == "l" || arquivo.Substring(arquivo.Length - 1) == "L")
                {
                    Stream entrada = File.Open(arquivo, FileMode.Open);
                    LerArquivo ler = new LerArquivo(entrada);

                    AnalisadorLexico analisador = new AnalisadorLexico(new TabelaSimbolos());

                    //Simbolos simbolos = analisador.buscarProximoLexema(ler);

                    var teste = analisador.buscarProximoLexema(ler).lexema;

                    while(teste!= "EOF")
                    {
                        Console.WriteLine(teste);                       
                        teste = analisador.buscarProximoLexema(ler).lexema;
                    }
                    
                    Console.ReadKey();

                    //string linha = ler.ReadLine();
                    //while (linha != null)
                    //{

                        

                    //    if(linha == "  readln(nome);")
                    //    {
                    //        Erro.ErroLexico.Char(ler.numeroLinha);
                    //        break;
                    //    }

                    //    Console.WriteLine(linha);
                    //   // Console.WriteLine(ler.numeroLinha);
                    //    linha = ler.ReadLine();
                    //    //Console.WriteLine("compilado com sucesso.");
                    //}
                    //ler.Close();
                    //entrada.Close();
                }
                else
                {
                    Console.WriteLine(arquivo + " não compativel.");
                }
            }
            else
            {
                Console.WriteLine(arquivo + " não encontrado.");
            }
            Console.WriteLine("Precione ENTER para sair.");
            Console.ReadKey();

        }
    }
}
