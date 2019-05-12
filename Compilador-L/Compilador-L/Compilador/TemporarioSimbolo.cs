using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador_L.Compilador
{
	class TemporarioSimbolo
	{
		private byte tipo;
		private byte classe;

		public TemporarioSimbolo()
		{
			this.tipo = 0;
			this.classe = 0;
		}

		public TemporarioSimbolo(byte tipo, byte classe)
		{
			this.tipo = tipo;
			this.classe = classe;

		}

		public byte getTipo()
		{
			return tipo;
		}

		public void setTipo(byte tipo)
		{
			this.tipo = tipo;
		}

		public byte getClasse()
		{
			return classe;
		}

		public void setClasse(byte classe)
		{
			this.classe = classe;
		}



	}
}
