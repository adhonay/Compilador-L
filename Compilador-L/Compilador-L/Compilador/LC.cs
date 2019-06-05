/*
 * Pontifícia Universidade Católica de Minas Gerais
 * Compilador
 * Autores: Adhonay Júnior, Izabela Costa
 * Matricula: 504656, 498535
 **/

using System;
using System.IO;

namespace Compilador_L.Compilador
{
    class LC
    {
        static void Main(string[] args)
        {
            var arquivo = Console.ReadLine();

            if (File.Exists(arquivo))
            {

                if (arquivo.Substring(arquivo.Length - 2, 1) == "." && arquivo.Substring(arquivo.Length - 1) == "l" || arquivo.Substring(arquivo.Length - 1) == "L")
                {
                    Stream entrada = File.Open(arquivo, FileMode.Open);
					AnalisadorSintatico aSintatico = new AnalisadorSintatico(entrada);

					aSintatico.S();
                    Console.WriteLine("Compilado com sucesso.");
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

            Console.ReadKey();

        }
    }
}
