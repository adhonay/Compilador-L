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
			tokenE = aLexico.buscarProximoLexema(ler);
		}

		//S-> {D}+ | {C}+
		public void S()
		{		
			
			do{
				D();
			} while (tokenE.token == TabelaSimbolos.VAR || tokenE.token == TabelaSimbolos.CONST);
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
		// D -> ( VAR {  (INT|CHAR) ID [ATR] {VIRGULA ID [ATR]}*  }+    |     const id= [+|-] constante )
		public void D()
		{
			if (tokenE.token == TabelaSimbolos.VAR)
			{
				casaToken(TabelaSimbolos.VAR);
				do
				{
					if (tokenE.token == TabelaSimbolos.INTEGER)
					{
						casaToken(TabelaSimbolos.INTEGER);
					}
					else
					{
						casaToken(TabelaSimbolos.CHAR);
					}
					casaToken(TabelaSimbolos.ID);
					if (tokenE.token == TabelaSimbolos.IGUAL || tokenE.token == TabelaSimbolos.ABCOLCHETE)
					{
						Atr();
					}
					while (tokenE.token == TabelaSimbolos.VIRGULA)
					{
						casaToken(TabelaSimbolos.VIRGULA);
						casaToken(TabelaSimbolos.ID);
						if (tokenE.token == TabelaSimbolos.IGUAL || tokenE.token == TabelaSimbolos.ABCOLCHETE)
						{
							Atr();
						}
					}
					casaToken(TabelaSimbolos.PONTOVIRGULA);

				} while (tokenE.token == TabelaSimbolos.CHAR || tokenE.token == TabelaSimbolos.INTEGER);
				
			}// D -> CONST ID = [-] CONSTANTE;
			else
			{
				casaToken(TabelaSimbolos.CONST);
				casaToken(TabelaSimbolos.ID);
				casaToken(TabelaSimbolos.IGUAL);
				if(tokenE.token == TabelaSimbolos.MENOS) // CASO POSSA TER +3 ENTAO CRIAR OUTRO IF AAQUI
				{
					casaToken(TabelaSimbolos.MENOS);
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
				if (tokenE.token == TabelaSimbolos.MENOS) // CASO POSSA TER +3 CRIAR OUTRO IF
				{
					casaToken(TabelaSimbolos.MENOS);
				}
				
				casaToken(TabelaSimbolos.CONSTANTE);
			}

		}
		//C-> ID [ ABCOLCHETE E FECOLCHETE ] IGUAL E PONTOVIRGULA | FOR ID IGUAL E TO E [STEP CONSTANTE] DO ( ABCHAVE {C}* FECHAVE | C)  | (write|writln) op
		//ELSE -> else (C | {C})
		public void C()
		{
			//C->ID [ ABCOLCHETE E FECOLCHETE ] IGUAL E PONTOVIRGULA
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
			//C -> FOR ID IGUAL E TO E [STEP CONSTANTE] DO ( ABCHAVE {C}* FECHAVE | C) 
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
			//C -> IF E THEN (ABCHAVE {C}* FECHACHEVE [ELSE ( ABCHAVE {C}* FECHAVE | C )]  |  C[ELSE ( ABCHAVE {C}* FECHAVE | C) ]  )
			else if (tokenE.token == TabelaSimbolos.IF)
			{
				casaToken(TabelaSimbolos.IF);
				E();
				casaToken(TabelaSimbolos.THEN);
				//C -> ABCHAVE {C}* FECHACHEVE [ELSE ( ABCHAVE {C}* FECHAVE | C )] 
				if (tokenE.token == TabelaSimbolos.ABCHAVE) 
				{
					casaToken(TabelaSimbolos.ABCHAVE);
					//LISTA DE 0 OU VARIOS COMANDOS. CASO SEJA OBRIGATORIO UM COMANDO TROCAR POR DO WHILE
					while (tokenE.token == TabelaSimbolos.ID || tokenE.token == TabelaSimbolos.FOR ||
					   tokenE.token == TabelaSimbolos.IF || tokenE.token == TabelaSimbolos.WRITE ||
					   tokenE.token == TabelaSimbolos.WRITELN || tokenE.token == TabelaSimbolos.READLN ||
					   tokenE.token == TabelaSimbolos.PONTOVIRGULA)
					{
						C();
					}
					casaToken(TabelaSimbolos.FECHAVE);
					if(tokenE.token == TabelaSimbolos.ELSE)
					{
						casaToken(TabelaSimbolos.ELSE);
						if(tokenE.token == TabelaSimbolos.ABCHAVE)
						{
							casaToken(TabelaSimbolos.ABCHAVE);
							//LISTA DE 0 OU VARIOS COMANDOS. CASO SEJA OBRIGATORIO UM COMANDO TROCAR POR DO WHILE
							while (tokenE.token == TabelaSimbolos.ID || tokenE.token == TabelaSimbolos.FOR ||
								tokenE.token == TabelaSimbolos.IF || tokenE.token == TabelaSimbolos.WRITE ||
								tokenE.token == TabelaSimbolos.WRITELN || tokenE.token == TabelaSimbolos.READLN ||
							    tokenE.token == TabelaSimbolos.PONTOVIRGULA)
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
							casaToken(TabelaSimbolos.ABCHAVE);
							while (tokenE.token == TabelaSimbolos.ID || tokenE.token == TabelaSimbolos.FOR ||
								tokenE.token == TabelaSimbolos.IF || tokenE.token == TabelaSimbolos.WRITE ||
							  tokenE.token == TabelaSimbolos.WRITELN || tokenE.token == TabelaSimbolos.READLN
							  || tokenE.token == TabelaSimbolos.PONTOVIRGULA)
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
			// id é um identificador de variável inteira, caractere alfanumérico ou stringg
			// esse comando lê e armazena o valor lido em um id. De acordo com o exemplo esse id pode ser de um vetor
			//caso não possa ser id de vetor comentar o if abaixo.
			else if (tokenE.token == TabelaSimbolos.READLN)
			{
				casaToken(TabelaSimbolos.READLN);
				casaToken(TabelaSimbolos.ABPARENTESES);
				casaToken(TabelaSimbolos.ID);
				if(tokenE.token == TabelaSimbolos.ABCOLCHETE)
				{
					casaToken(TabelaSimbolos.ABCOLCHETE);
					if(tokenE.token == TabelaSimbolos.CONSTANTE)
					{
						casaToken(TabelaSimbolos.CONSTANTE);
					}else
					{
						casaToken(TabelaSimbolos.ID);
					}
					
					casaToken(TabelaSimbolos.FECOLCHETE);
				}
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
		// E -> ES [ ( IGUAL | MENOR | MAIOR | MENORIGUAL | MAIORIGUAL | DIFERENTE ) ES ]
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
		//ES -> [ + | - ] T { ( + | - | or ) T }*
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
		// F -> ABPARENTESES E FEPARENTESES |CONSTANTE| ID[ ABCOLCHETE E FECOLHETE ] | NOT F
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
               
                    if (tokenE.token == tokenEsperado)
                    {
                        //Console.WriteLine("entrou "+ tokenE.lexema);
                        tokenE = aLexico.buscarProximoLexema(ler);
                        //Console.WriteLine("saiu " + tokenE.lexema);
                    }
                    else if (tokenE == null)
                    {

                        Erro.ErroSintatico.Arquivo(aLexico.getLinha());
                    }
                    else if (tokenE.token == TabelaSimbolos.EOF)
                    {
                        Erro.ErroSintatico.Arquivo(aLexico.getLinha());
                    }
                    else
                    {

                        Erro.ErroSintatico.Lexema(aLexico.getLinha(), tokenE.lexema);
                    }
                }
				
				
			
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}

	}

}

