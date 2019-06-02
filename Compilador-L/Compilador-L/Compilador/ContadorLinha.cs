/*
 * Pontifícia Universidade Católica de Minas Gerais
 * Compilador
 * Autores: Adhonay Júnior, Izabela Costa
 * Matricula: 504656, 498535
 **/
using System.IO;

namespace Compilador_L.Compilador
{

    public class LerArquivo : StreamReader
    {
        private int _numeroLinha = 0;
        public int numeroLinha { get { return _numeroLinha; } }

        public LerArquivo(Stream stream) : base(stream) { }

        public override string ReadLine()
        {
            _numeroLinha++;
            return base.ReadLine();
        }
    } 
}
