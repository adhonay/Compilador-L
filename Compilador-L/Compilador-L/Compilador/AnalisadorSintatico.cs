using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Compilador_L.Compilador
{
	class AnalisadorSintatico
	{

		/*
		* Classe verifica se o códifo fonte é gerado pela gramática 
		*/
		LerArquivo ler;
		AnalisadorLexico aLexico;
		Simbolos tokenE;
		
		
		 public AnalisadorSintatico(Stream arquivoEntrada)
		 {
			
			ler = new LerArquivo(arquivoEntrada);
			aLexico = new AnalisadorLexico(new TabelaSimbolos());
			//principal();
			tokenE = aLexico.buscarProximoLexema(ler);
			S();

		}
		public void principal()
		{
			tokenE = aLexico.buscarProximoLexema(ler);
			S();
		}
		//S-> {D} | {C}+
		public void S()
		{		
			while(tokenE.token== TabelaSimbolos.VAR || tokenE.token == TabelaSimbolos.CONST)
			{
				D();
			}
			if (tokenE.token == TabelaSimbolos.EOF)
			{
				//erro ocorre pois o programa deve ter ao menos um comando n pode finalizar após declarações
				Erro.ErroSintatico.Arquivo(aLexico.getLinha());
			}
			do
			{
				C();
			} while (tokenE.token == TabelaSimbolos.ID || tokenE.token == TabelaSimbolos.FOR ||
					tokenE.token == TabelaSimbolos.IF|| tokenE.token == TabelaSimbolos.PONTOVIRGULA ||
					tokenE.token == TabelaSimbolos.WRITE|| tokenE.token == TabelaSimbolos.WRITELN||
					tokenE.token == TabelaSimbolos.READLN);

			if (aLexico.EOF == false)
			{
				//erro ocorre pois o programa termina sua leitura nos comandos qualquer simbolo após é inadequado.
				Erro.ErroSintatico.Lexema(aLexico.getLinha(), tokenE.lexema);
			}

		}
		// D -> (var (int|char) id [atribuiçao] | const id= [+|-] constante )
		public void D()
		{
			if(tokenE.token == TabelaSimbolos.VAR)
			{
				casaToken(TabelaSimbolos.VAR);
				if(tokenE.token == TabelaSimbolos.INTEGER || tokenE.token == TabelaSimbolos.CHAR) 
				{
					casaToken(tokenE.token);
				}
				casaToken(TabelaSimbolos.ID);
				if(tokenE.token == TabelaSimbolos.IGUAL || tokenE.token == TabelaSimbolos.ABCOLCHETE)
				{
					Atr();
				}
				while(tokenE.token == TabelaSimbolos.VIRGULA)
				{
					casaToken(TabelaSimbolos.VIRGULA);
					casaToken(TabelaSimbolos.ID);
					if(tokenE.token == TabelaSimbolos.IGUAL || tokenE.token == TabelaSimbolos.ABCOLCHETE)
					{
						Atr();
					}
				}
				casaToken(TabelaSimbolos.PONTOVIRGULA);

			}// D -> const id= [+|-] constante;
			else
			{
				casaToken(TabelaSimbolos.CONST);
				casaToken(TabelaSimbolos.ID);
				casaToken(TabelaSimbolos.IGUAL);
				if(tokenE.token == TabelaSimbolos.MENOS)
				{
					casaToken(TabelaSimbolos.MENOS);
				}
				else if(tokenE.token == TabelaSimbolos.MAIS)   // CASO NÃO POSSA EXISTIR +3  APENAS 3 OU -3 APAGAR IF
				{
					casaToken(TabelaSimbolos.MAIS);
				}
				casaToken(TabelaSimbolos.CONSTANTE);

				casaToken(TabelaSimbolos.PONTOVIRGULA);
			}
		}
		//Atr -> = [+|-] constante | [constante]
		public void Atr()
		{
			if (tokenE.token == TabelaSimbolos.ABCOLCHETE)
			{
				casaToken(TabelaSimbolos.ABCOLCHETE);
				casaToken(TabelaSimbolos.CONSTANTE);
				casaToken(TabelaSimbolos.FECOLCHETE);
			} else
			{
				casaToken(TabelaSimbolos.IGUAL);
				if (tokenE.token == TabelaSimbolos.MENOS)
				{
					casaToken(TabelaSimbolos.MENOS);
				}
				else if (tokenE.token == TabelaSimbolos.MAIS)
				{
					casaToken(TabelaSimbolos.MAIS);
				}
				casaToken(TabelaSimbolos.CONST);
			}

		}
		//C-> id [ "[" E "]" ] = E | for id=E to E [step constante] do {C}| if E then ( {C}+ [_EL] ) | (write|writln) op
		//ELSE -> else (C | {C})
		public void C()
		{
			//C-> id [ "[" E "]" ] = E;
			if (tokenE.token == TabelaSimbolos.ID)
			{
				casaToken(tokenE.token);
				if(tokenE.token == TabelaSimbolos.ABCOLCHETE)
				{
					casaToken(TabelaSimbolos.ABCOLCHETE);
					E();
					casaToken(TabelaSimbolos.FECOLCHETE);
				}
				casaToken(TabelaSimbolos.IGUAL);
				E();
				casaToken(TabelaSimbolos.PONTOVIRGULA);


			} 
			//C -> FOR ID IGUAL E TO E [STEP CONSTANTE] DO ( ABCOLCHETE {C}* FECOLCHETE | C) 
			else if(tokenE.token == TabelaSimbolos.FOR)
			{
				casaToken(TabelaSimbolos.FOR);
				casaToken(TabelaSimbolos.ID);
				casaToken(TabelaSimbolos.IGUAL);
				E();
				casaToken(TabelaSimbolos.TO);
				E();
				if(tokenE.token == TabelaSimbolos.STEP)
				{
					casaToken(TabelaSimbolos.STEP);
					casaToken(TabelaSimbolos.CONSTANTE);					
				}
				casaToken(TabelaSimbolos.DO);
				if(tokenE.token == TabelaSimbolos.ABCHAVE)
				{
					casaToken(TabelaSimbolos.ABCHAVE);
					while (tokenE.token == TabelaSimbolos.ID || tokenE.token == TabelaSimbolos.FOR || tokenE.token == TabelaSimbolos.PONTOVIRGULA ||
					   tokenE.token == TabelaSimbolos.IF || tokenE.token == TabelaSimbolos.WRITE ||
					   tokenE.token == TabelaSimbolos.WRITELN || tokenE.token == TabelaSimbolos.READLN)
					{
						C();
						
					}

					casaToken(TabelaSimbolos.FECHAVE);
				}
				else
				{
					C();
				}

			}
			//C -> IF E THEN ( '{' COMANDO '}' [ELSE ( '{' COMANDO '}' | COMANDO) ] |  COMANDO [ELSE ( '{' COMANDO '}' | COMANDO) ]  )
			// '{' REPRESENTA O TOKEN ABCOLCHETE NESTE COMENTÁRIO.
			else if (tokenE.token == TabelaSimbolos.IF)
			{
				casaToken(TabelaSimbolos.IF);
				E();
				casaToken(TabelaSimbolos.THEN);
				//C -> IF E THEN ( '{' COMANDO '}' [ELSE ( '{' COMANDO '}' | COMANDO) ]
				if (tokenE.token == TabelaSimbolos.ABCHAVE) 
				{
					casaToken(TabelaSimbolos.ABCHAVE);
					while (tokenE.token == TabelaSimbolos.ID || tokenE.token == TabelaSimbolos.FOR ||
					   tokenE.token == TabelaSimbolos.IF || tokenE.token == TabelaSimbolos.WRITE ||
					   tokenE.token == TabelaSimbolos.WRITELN || tokenE.token == TabelaSimbolos.READLN)
					{
						C();
					}
					casaToken(TabelaSimbolos.FECHAVE);
					if(tokenE.token == TabelaSimbolos.ELSE)
					{
						casaToken(TabelaSimbolos.ELSE);
						if(tokenE.token == TabelaSimbolos.ABCHAVE)
						{
							while (tokenE.token == TabelaSimbolos.ID || tokenE.token == TabelaSimbolos.FOR ||
								tokenE.token == TabelaSimbolos.IF || tokenE.token == TabelaSimbolos.WRITE ||
							  tokenE.token == TabelaSimbolos.WRITELN || tokenE.token == TabelaSimbolos.READLN)
							{
								C();
							}
							casaToken(TabelaSimbolos.FECHAVE);
						}
						else
						{
							C();
						}

					}
				}//C-> COMANDO [ELSE ( '{' COMANDO '}' | COMANDO) ]  )
				else
				{
					C();
					if(tokenE.token == TabelaSimbolos.ELSE)
					{
						casaToken(TabelaSimbolos.ELSE);
						if(tokenE.token == TabelaSimbolos.ABCHAVE)
						{
							while (tokenE.token == TabelaSimbolos.ID || tokenE.token == TabelaSimbolos.FOR ||
								tokenE.token == TabelaSimbolos.IF || tokenE.token == TabelaSimbolos.WRITE ||
							  tokenE.token == TabelaSimbolos.WRITELN || tokenE.token == TabelaSimbolos.READLN)
							{
								C();
							}
							casaToken(TabelaSimbolos.FECHAVE);
						}
						else
						{
							C();
						}
					}
				}
				
		
			}
			// C -> READLN  ABPARENTESES ID FEPARENTESES PONTOVIRGULA;
			else if(tokenE.token == TabelaSimbolos.READLN)
			{
				casaToken(TabelaSimbolos.READLN);
				casaToken(TabelaSimbolos.ABPARENTESES);
				casaToken(TabelaSimbolos.ID);
				casaToken(TabelaSimbolos.FEPARENTESES);
				casaToken(TabelaSimbolos.PONTOVIRGULA);

			}
			// C-> (WRITELN | WRITE) ABPARENTESES (E | CONSTANTE ) {VIRGULA ( E | CONSTANTE)  }*  FEPARENTESES PONTOVIRGULA
			else if (tokenE.token == TabelaSimbolos.WRITE || tokenE.token == TabelaSimbolos.WRITELN) //---- QUEBRA DE LINHA EM LN !?
			{
				casaToken(tokenE.token);
				casaToken(TabelaSimbolos.ABPARENTESES);

				if(tokenE.token == TabelaSimbolos.CONSTANTE)
				{
					casaToken(TabelaSimbolos.CONSTANTE);
				}
				else
				{
					E();
				}
				
				while(tokenE.token == TabelaSimbolos.VIRGULA)
				{
					casaToken(TabelaSimbolos.VIRGULA);
					if (tokenE.token == TabelaSimbolos.CONSTANTE)
					{
						casaToken(TabelaSimbolos.CONSTANTE);
					}
					else
					{
						E();
					}

				}
				casaToken(TabelaSimbolos.FEPARENTESES);
				casaToken(TabelaSimbolos.PONTOVIRGULA);

			}
			// C -> ;
			else
			{
				casaToken(TabelaSimbolos.PONTOVIRGULA);
			}
		}
		// E -> ES [ ( = | < | > | <= | >= | <> ) ES ]
		public void E()
		{
			ES();
			if(tokenE.token == TabelaSimbolos.IGUAL || tokenE.token == TabelaSimbolos.MENOR ||
				tokenE.token == TabelaSimbolos.MAIOR||tokenE.token == TabelaSimbolos.MENORIGUAL || 
				tokenE.token == TabelaSimbolos.MAIORIGUAL || tokenE.token == TabelaSimbolos.DIFERENTE)
			{
				casaToken(tokenE.token);
				ES();

			}

		}
		//ES -> [ + | - ] T { ( + | - | or ) T }
		public void ES()
		{
			if(tokenE.token == TabelaSimbolos.MAIS || tokenE.token == TabelaSimbolos.MENOS)
			{
				casaToken(tokenE.token);
			}
			T();
			while(tokenE.token == TabelaSimbolos.MAIS || tokenE.token == TabelaSimbolos.MENOS || tokenE.token == TabelaSimbolos.OR)
			{
				casaToken(tokenE.token);
				T();
			}

		}

		// T-> F { ( * | / | % | and ) F }
		public void T()
		{
			F();
			while(tokenE.token == TabelaSimbolos.MULTIPLICACAO || tokenE.token == TabelaSimbolos.DIVISAO ||
				tokenE.token == TabelaSimbolos.PORCENTAGEM || tokenE.token == TabelaSimbolos.AND)
			{
				casaToken(tokenE.token);
				F();

			}
		}
		// F -> "(" E ")" |CONSTANTE| ID[ "[" E "]" ] | NOT F
		public void F()
		{
			//F -> '(' E ')'
			if (tokenE.token == TabelaSimbolos.ABPARENTESES)
			{
				casaToken(TabelaSimbolos.ABPARENTESES);
				E();
				casaToken(TabelaSimbolos.FEPARENTESES);

			}// F -> CONSTANTE
			else if(tokenE.token == TabelaSimbolos.CONSTANTE)
			{
				casaToken(TabelaSimbolos.CONSTANTE);
			}// F-> ID[ "[" E "]" ] 
			else if (tokenE.token == TabelaSimbolos.ID)
			{
				casaToken(TabelaSimbolos.ID);
				if(tokenE.token == TabelaSimbolos.ABCOLCHETE)
				{
					casaToken(TabelaSimbolos.ABCOLCHETE);
					E();
					casaToken(TabelaSimbolos.FECOLCHETE);
				}
			}// F-> NOT F
			else 
			{
				casaToken(TabelaSimbolos.NOT);
				F();
			}

		}
		public void casaToken(byte tokenEsperado)
		{
			try
			{
				if(aLexico.EOF == false)
				{
					if (tokenE.token == tokenEsperado)
					{
						Console.WriteLine("entrou "+ tokenE.lexema);
						tokenE = aLexico.buscarProximoLexema(ler);
						Console.WriteLine("saiu " + tokenE.lexema);
					}
					else if(tokenE == null)
					{
						Erro.ErroSintatico.Arquivo(aLexico.getLinha());
					}
					else
					{
						Console.WriteLine("caiu aki");
						Erro.ErroSintatico.Lexema(aLexico.getLinha(), tokenE.lexema);				
					}
				}				
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}

	}

}

