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
namespace Compilador_L
{
    class LC
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Digite o nome e extensão do arquivo");
            var arquivo = Console.ReadLine();

            if (File.Exists(arquivo))
            {

                if (arquivo.Substring(arquivo.Length - 2,1) == "." && arquivo.Substring(arquivo.Length - 1) == "l" || arquivo.Substring(arquivo.Length - 1) == "L")
                {
                    Stream entrada = File.Open(arquivo, FileMode.Open);

                    StreamReader leitor = new StreamReader(entrada);
                    string linha = leitor.ReadLine();
                    while (linha != null)
                    {
                        //Console.WriteLine(linha);
                        //linha = leitor.ReadLine();
                        Console.WriteLine("compilado com sucesso.");
                    }
                    leitor.Close();
                    entrada.Close();  
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
