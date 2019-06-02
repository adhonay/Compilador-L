/*
 * Pontifícia Universidade Católica de Minas Gerais
 * Compilador
 * Autores: Adhonay Júnior, Izabela Costa
 * Matricula: 504656, 498535
 **/

namespace Compilador_L.Compilador
{
	class TemporarioSimbolo
	{		
        public byte classe { get; set; }
        public byte tipo { get; set; }
        public int tamanho { get; set; }

        public TemporarioSimbolo()
		{
            this.tipo = 0;
            this.classe = 0;
            this.tamanho = -1;
		}

		public TemporarioSimbolo(byte tipo, byte classe)
		{
			this.tipo = tipo;
			this.classe = classe;

		}
	}
}
