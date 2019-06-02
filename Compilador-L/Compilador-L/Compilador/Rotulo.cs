/*
 * Pontifícia Universidade Católica de Minas Gerais
 * Compilador
 * Autores: Adhonay Júnior, Izabela Costa
 * Matricula: 504656, 498535
 **/
using System;


namespace Compilador_L.Compilador
{
    class Rotulo
    {
        private static int rotulo;

        public Rotulo()
        {
            rotulo = 0;
        }

        public String novoRotulo()
        {
            return "R" + rotulo++;
        }

    }
}
