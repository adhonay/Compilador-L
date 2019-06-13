/*
 * Pontifícia Universidade Católica de Minas Gerais
 * Compilador
 * Autores: Adhonay Júnior, Izabela Costa
 * Matricula: 504656, 498535
 **/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Otimizador
{
    class OtimizadorLC
    {

        public List<String> buffer;
        public String[] linha;
        public void print(String file)
        {
            try
            {
                StreamWriter _file = new StreamWriter(file);

                foreach (String s in buffer)
                {
                    _file.Write(s);
                    _file.Write("\n");
                }

                _file.Close();

            }
            catch (Exception e) { }
        }

        public void add(String str)
        {
            buffer.Add(str);
        }

        public static void peephole()
        {
            OtimizadorLC peephole = new OtimizadorLC();
            peephole.buffer = new List<string>();
            peephole.linha = System.IO.File.ReadAllLines("c:/8086/arquivo.asm");
            var tratarLinha = peephole.linha;
            String[] linha = new String[tratarLinha.Length];
            int contador = 0;
            for (int i = 0; i < tratarLinha.Length; i++)
            {
                if (!(tratarLinha[i] == "" || tratarLinha[i].FirstOrDefault() == ';'))
                {
                    linha[contador] = tratarLinha[i];
                    contador++;
                }
            }

            for (int i = 0; i < linha.Length; i++)
            {
                if (i + 1 == linha.Length) { break; }

                if (linha[i] == null || linha[i + 1] == null)
                {
                    if (linha[i] != null)
                    {
                        peephole.add(linha[i]);
                    }
                    break;
                }

                if (linha[i].Contains("mov") && linha[i + 1].Contains("mov"))
                {
                    String[] mov1 = linha[i].Replace(",", "").Split(' ');
                    String[] mov2 = linha[i + 1].Replace(",", "").Split(' ');

                    if (mov1[1].Contains("DS") && mov2[2].Contains("DS") && mov1[1].Equals(mov2[2]))
                    {
                        peephole.add("mov " + mov2[1] + ", " + mov1[2] + "               ;LINHA OTIMIZADA PEEPHOLE");
                        i++;
                    }
                    else
                    {
                        peephole.add(linha[i]);
                    }

                }
                else
                {
                    peephole.add(linha[i]);
                }
            }
            peephole.print("c:/8086/arquivoOtimizado.asm");
        }

        public static void reducaoCusto()
        {
            OtimizadorLC peephole = new OtimizadorLC();
            peephole.buffer = new List<string>();
            peephole.linha = System.IO.File.ReadAllLines("c:/8086/arquivo.asm");
            var linha = peephole.linha;

            for (int i = 0; i < linha.Length; i++)
            {
                String[] imul = linha[i].Replace(",", "").Split(' ');

                if (imul[0].Contains("imul") && imul[1].Contains("AX"))
                {
                    peephole.add("add AX, AX ;LINHA OTIMIZADA REDUÇAO DE CUSTO");
                    i++;
                }
                else
                {
                    peephole.add(linha[i]);
                }
            }
            peephole.print("c:/8086/arquivoOtimizado.asm");
        }

        static void Main(string[] args)
        {   //executar 1 por vez.
            peephole();
            //reducaoCusto();
            Console.WriteLine("Otimizado com sucesso.");
            Console.ReadKey();
        }
    }
}
