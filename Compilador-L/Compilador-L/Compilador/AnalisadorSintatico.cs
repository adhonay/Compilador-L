using System;
using System.IO;

/*
 * Pontifícia Universidade Católica de Minas Gerais
 * Compilador
 * Autores: Adhonay Júnior, Izabela Costa
 * Matricula: 504656, 498535
 **/

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
        TabelaSimbolos tabela;
		
		public AnalisadorSintatico(Stream arquivoEntrada)
		{
            tabela = new TabelaSimbolos();
            aLexico = new AnalisadorLexico(tabela);
            ler = new LerArquivo(arquivoEntrada);
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
                Erro.ErroSintatico.Arquivo(aLexico.linha);
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
				Erro.ErroSintatico.Lexema(aLexico.linha, tokenE.lexema);
			}

		}
		// D -> ( VAR {  (INT|CHAR) ID [ATR] {VIRGULA ID [ATR]}*  }+    |     const id= [+|-] constante )
		public void D()
		{
			TemporarioSimbolo _Daux = new TemporarioSimbolo();
            TemporarioSimbolo _D = new TemporarioSimbolo(); // auxilia na construção dos atributos do ATR
            Simbolos auxID = new Simbolos("", Byte.MaxValue);
            Simbolos auxCONST = new Simbolos("", Byte.MaxValue);
            Boolean sinal = false ;

            if (tokenE.token == TabelaSimbolos.VAR)
			{
                //ação semantica 1
                _Daux.classe = Simbolos.CLASSE_VAR;
				casaToken(TabelaSimbolos.VAR);

				do
				{
					if (tokenE.token == TabelaSimbolos.INTEGER)
					{
                        //ação semantica 3
                        _Daux.tipo= Simbolos.TIPO_INTEIRO;
						casaToken(TabelaSimbolos.INTEGER);
					}
					else
					{
                        //ação semantica 4
                        _Daux.tipo = Simbolos.TIPO_CARACTERE;
						casaToken(TabelaSimbolos.CHAR);
					}
                         //recebo o ID
                    auxID = tokenE;
					casaToken(TabelaSimbolos.ID);
                    //ação semantica 5
                    if(auxID.classe == Simbolos.SEM_CLASSE)
                    {
                        tabela.buscarSimbolo(auxID.lexema).classe = _Daux.classe;
                        if(auxID.classe != Simbolos.CLASSE_CONST)
                        {
                            tabela.buscarSimbolo(auxID.lexema).tipo = _Daux.tipo;
                        }
                    }
                    else
                    {
                        Erro.ErroSemantico.Declarado(aLexico.linha, auxID.lexema);
                    }
					if (tokenE.token == TabelaSimbolos.IGUAL || tokenE.token == TabelaSimbolos.ABCOLCHETE)
					{
                        Atr(_D,auxID.tipo);
					}
                    //inicio ação semantica 9
                    if(_D.tipo != auxID.tipo) //atr.tipo != id.tipo erro
                    {
                        Erro.ErroSemantico.Tipos(aLexico.linha);
                    }
                    else
                    {
                        auxID.tamanho = _D.tamanho;
                    }
                    //fim ação semantica 9
					while (tokenE.token == TabelaSimbolos.VIRGULA)
					{
						casaToken(TabelaSimbolos.VIRGULA);
                        auxID = tokenE;
						casaToken(TabelaSimbolos.ID);
                        //inicio ação semantica 5
                        if (auxID.classe == Simbolos.SEM_CLASSE)
                        {
                            tabela.buscarSimbolo(auxID.lexema).classe = _Daux.classe;
                            if (auxID.classe != Simbolos.CLASSE_CONST)
                            {
                                tabela.buscarSimbolo(auxID.lexema).tipo = _Daux.tipo;
                            }
                        }
                        else
                        {
                            Erro.ErroSemantico.Declarado(aLexico.linha, auxID.lexema);
                        }
                        // fim ação semantica 5
                        if (tokenE.token == TabelaSimbolos.IGUAL || tokenE.token == TabelaSimbolos.ABCOLCHETE)
						{
                            Atr(_D, auxID.tipo);
                        }
                        //inicio ação semantica 9
                        if (_D.tipo != auxID.tipo) //atr.tipo != id.tipo erro
                        {
                            Erro.ErroSemantico.Tipos(aLexico.linha);
                        }
                        else
                        {
                            auxID.tamanho = _D.tamanho;
                        }
                        //fim ação semantica 9

                    }
                    casaToken(TabelaSimbolos.PONTOVIRGULA);

				} while (tokenE.token == TabelaSimbolos.CHAR || tokenE.token == TabelaSimbolos.INTEGER);
				
			}// D -> CONST ID = [-] CONSTANTE;
			else
			{
                //inicio ação semantica 2
                _Daux.classe = Simbolos.CLASSE_CONST;
                //fim ação semantica 2
                casaToken(TabelaSimbolos.CONST);
                auxID = tokenE;
				casaToken(TabelaSimbolos.ID);
                //inicio ação semantica 5
                if (auxID.classe == Simbolos.SEM_CLASSE)
                {
                    tabela.buscarSimbolo(auxID.lexema).classe = _Daux.classe;
                    if (auxID.classe != Simbolos.CLASSE_CONST)
                    {
                        tabela.buscarSimbolo(auxID.lexema).tipo = _Daux.tipo;
                    }
                }
                else
                {
                    Erro.ErroSemantico.Declarado(aLexico.linha, auxID.lexema);
                }
                //fim ação semantica 5
                casaToken(TabelaSimbolos.IGUAL);
				if(tokenE.token == TabelaSimbolos.MENOS)
				{
					casaToken(TabelaSimbolos.MENOS);
                    //ação semantica 6
                    sinal = true;
                }
                auxCONST = tokenE;
				casaToken(TabelaSimbolos.CONSTANTE);
                //inicio ação semantica 10
                if(sinal == true)
                {
                    if(auxCONST.tipo != Simbolos.TIPO_INTEIRO)
                    {
                        Erro.ErroSemantico.Tipos(aLexico.linha);
                    }
                    else
                    {
                        auxID.tipo = auxCONST.tipo;
                        auxID.tamanho = auxCONST.tamanho;
                    }
                }
                else if(auxCONST.tipo == Simbolos.TIPO_HEXADECIMAL || auxCONST.tipo == Simbolos.TIPO_CARACTERE)
                {
                    auxID.tipo = Simbolos.TIPO_CARACTERE;
                    auxID.tamanho = auxCONST.tamanho;
                }
                else if(auxCONST.tipo == Simbolos.TIPO_INTEIRO)
                {
                    auxID.tipo = auxCONST.tipo;
                    auxID.tamanho = auxCONST.tamanho;
                }
                else
                {
                    //erro tipo inesperado (nao é int, char ou hexa) sendo atribuido a constante
                    Erro.ErroSemantico.Tipos(aLexico.linha);
                }
                //fim ação semantica 10

                casaToken(TabelaSimbolos.PONTOVIRGULA);
			}
		}
		//Atr -> = [-] constante | [constante]
		public void Atr(TemporarioSimbolo _Atr,Byte _IDtipo)
		{
            Boolean sinal = false;
            Simbolos auxCONST = new Simbolos("", Byte.MaxValue);

            if (tokenE.token == TabelaSimbolos.ABCOLCHETE)
			{
				casaToken(TabelaSimbolos.ABCOLCHETE);
                auxCONST = tokenE;
				casaToken(TabelaSimbolos.CONSTANTE);
                //inicio ação semantica 8
                if (auxCONST.tipo != Simbolos.TIPO_INTEIRO)
                {
                    //erro tipo
                }
                else
                {
                    _Atr.tipo = _IDtipo;
                    _Atr.tamanho = int.Parse(auxCONST.lexema);
                }
                // fim ação semantica 8
				casaToken(TabelaSimbolos.FECOLCHETE);
			} else
			{
				casaToken(TabelaSimbolos.IGUAL);
				if (tokenE.token == TabelaSimbolos.MENOS) 
				{                 
					casaToken(TabelaSimbolos.MENOS);
                    sinal = true;
                }
                auxCONST = tokenE; 
				casaToken(TabelaSimbolos.CONSTANTE);
                //inicio ação semantica 7
                if(sinal == true)
                {
                    if(auxCONST.tipo != Simbolos.TIPO_INTEIRO)
                    {
                        //ERRO TIPOS 
                    }
                    else
                    {
                        _Atr.tipo = auxCONST.tipo;
                        _Atr.tamanho= auxCONST.tamanho;
                    }

                }
                else if(auxCONST.tipo == Simbolos.TIPO_HEXADECIMAL|| auxCONST.tipo == Simbolos.TIPO_CARACTERE)
                {
                    _Atr.tipo = Simbolos.TIPO_CARACTERE;
                    _Atr.tamanho = auxCONST.tamanho;
                }
                else
                {
                    _Atr.tipo = auxCONST.tipo; // se vier int sem sinal ou string cai aqui
                    _Atr.tamanho = auxCONST.tamanho;
                } 
                // fim ação semantica 7

			}

		}
        //C-> ID [ ABCOLCHETE E FECOLCHETE ] IGUAL E PONTOVIRGULA |
        //FOR ID IGUAL E TO E [STEP CONSTANTE] DO ( ABCHAVE {C}* FECHAVE | C)  | 
        //(write|writln) ABPARENTESES (E | CONSTANTE )  {, ( E | CONSTANTE)  }*  FEPARENTESES ;
        //READLN  ABPARENTESES ID [ [ CONSTANTE | ID ]  ] FEPARENTESES ; | 
        //IF E THEN (  {  {C}* } [ELSE ( { {C}* } | C ) ]    C [ ELSE ( {  {C}* } | C) ] )  
        public void C()
		{
            TemporarioSimbolo _C = new TemporarioSimbolo(); //a ser preenchido por E();
            Simbolos _auxCONSTc = new Simbolos("", Byte.MaxValue);
            Simbolos _auxIDc = new Simbolos("", Byte.MaxValue);
            Boolean isVetor =false, vetorUtilizado =false;

            //C->ID [ ABCOLCHETE E FECOLCHETE ] IGUAL E PONTOVIRGULA
            if (tokenE.token == TabelaSimbolos.ID)
			{
                _auxIDc = tokenE;
				casaToken(tokenE.token);
                //inicio ação semantica 0
                if(_auxIDc.classe == Simbolos.SEM_CLASSE)
                {
                    //erro id não declarado
                }
                //inicio ação semantica 1
                else if(_auxIDc.classe == Simbolos.CLASSE_CONST)
                {
                    //erro tipos incompativeis (um identificador constante n pode ter atribuiçao)
                }
                //fim ação semantica 2
                else if (_auxIDc.tamanho > 0)
                {
                    isVetor = true;
                }

                if (tokenE.token == TabelaSimbolos.ABCOLCHETE)
				{
					casaToken(TabelaSimbolos.ABCOLCHETE);
					E(_C);
                    //inicio açao semantica 3
                    if (_C.tipo != Simbolos.TIPO_INTEIRO)
                    {
                        //erro dentro do colchete so pode ter numero
                    }                 
                    else if (_auxIDc.tamanho == 0) 
                    {
                        //erro 
                    }//fim ação 3, inicio ação semantica 12
                    else
                    {
                        vetorUtilizado = true;
                    }
                    //fim ação semantica 12
                    casaToken(TabelaSimbolos.FECOLCHETE);
				}
				casaToken(TabelaSimbolos.IGUAL);
				E(_C);
                //inicio ação semantica 4
                if(_C.tipo != _auxIDc.tipo || isVetor != vetorUtilizado)
                {
                    //ERRO 
                }
                //fim ação semantica 4
				casaToken(TabelaSimbolos.PONTOVIRGULA);
			} 
			//C -> FOR ID IGUAL E TO E [STEP CONSTANTE] DO ( ABCHAVE {C}* FECHAVE | C) 
			else if(tokenE.token == TabelaSimbolos.FOR)
			{
				casaToken(TabelaSimbolos.FOR);
                _auxIDc = tokenE;
				casaToken(TabelaSimbolos.ID);

                //inicio ação semantica 0
                if (_auxIDc.classe == Simbolos.SEM_CLASSE)
                {
                    //erro id não declarado
                }
                //inicio ação semantica 1
                if (_auxIDc.classe == Simbolos.CLASSE_CONST)
                {
                    //ERRO IDENTIFICADOR É UMA CONSTANTE NÃO PODE RECEBER NADA
                }
                //incio ação semantica 5
                if(_auxIDc.tipo != Simbolos.TIPO_INTEIRO || _auxIDc.tamanho > 0)
                {
                    // erro id no for tem q ser inteiro para fazer for id = numero
                }
                //fim ação semantica 5
				casaToken(TabelaSimbolos.IGUAL);
				E(_C);

                //inicio ação semantica 6
                if(_C.tipo != Simbolos.TIPO_INTEIRO)
                {
                    //erro , E tem qe retornar um int para o for
                }
                //fim ação semantica 6

                casaToken(TabelaSimbolos.TO);
				E(_C);

                //inicio ação semantica 6
                if (_C.tipo != Simbolos.TIPO_INTEIRO)
                {
                    //erro , E tem qe retornar um int para o for
                }
                //fim ação semantica 6


                // STEP OPCIONAL
                if (tokenE.token == TabelaSimbolos.STEP)
				{
					casaToken(TabelaSimbolos.STEP);
                    _auxCONSTc = tokenE;
					casaToken(TabelaSimbolos.CONSTANTE);	

                    //ação semantica 7
                    if(_auxCONSTc.tipo != Simbolos.TIPO_INTEIRO)
                    {
                        //erro a constante no step tem q ser int
                    }
                    //fim ação semantica 7
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
				E(_C);

                //inicio ação semantica 8
                if(true/*_C.tipo != Simbolos.TIPO_LOGICO*/)
                {
                    //ERRO E TEM QUE RETORNAR LOGICO
                }
                //fim ação semantica 8

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
                _auxIDc = tokenE;
				casaToken(TabelaSimbolos.ID);
                isVetor = false; vetorUtilizado = false;

                //inicio ação semantica 0
                if (_auxIDc.classe == Simbolos.SEM_CLASSE)
                {
                  
                }
                // fim ação 0 , inicio ação 01
                if (_auxIDc.classe == Simbolos.CLASSE_CONST) 
                {
                   
                }// fim ação semantica 1, Inicio ação semantica 14
                if(true/*_auxIDc.tipo == Simbolos.TIPO_LOGICO*/)
                {
                    
                }
                // fim ação semantica 14, inicio ação semantica 2
                if(_auxIDc.tamanho > 0)
                {
                    isVetor = true;
                }//fim ação semantica 2

            
				if(tokenE.token == TabelaSimbolos.ABCOLCHETE)
				{
                    //inicio ação semantica 9
                    if(_auxIDc.tamanho <= 0)
                    {
                       //erro 
                    }//fim ação 9
					casaToken(TabelaSimbolos.ABCOLCHETE);
					if(tokenE.token == TabelaSimbolos.CONSTANTE)
					{
                        _auxCONSTc = tokenE;
						casaToken(TabelaSimbolos.CONSTANTE);
                        //inicio ação semantica 7
                        if(_auxCONSTc.tipo != Simbolos.TIPO_INTEIRO)
                        {
                            //erro
                        }//fim ação semantica 7

                        //inicio ação sementica 12
                        vetorUtilizado = true;
                        //fim ação 12
					}else
					{
                        _auxIDc = tokenE;
						casaToken(TabelaSimbolos.ID);
                        //inicio ação semantica 0
                        if(_auxIDc.classe == Simbolos.SEM_CLASSE)
                        {
                            //erro
                        }
                        //fim ação semantica 0, inicio ação 10
                        if(_auxIDc.tipo != Simbolos.TIPO_INTEIRO)
                        {
                            //erro
                        }//fim ação semantica 10

                        //inicio ação semantica 12
                        vetorUtilizado = true;
                        //fim ação 12
					}
					
					casaToken(TabelaSimbolos.FECOLCHETE);
				}
                //inicio ação semantica 13
                if(vetorUtilizado != isVetor)
                {
                    //erro
                }
                //fim ação semtnica 13
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
					E(_C);
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
						E(_C);
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
		public void E(TemporarioSimbolo _E)
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
            TemporarioSimbolo _F = new TemporarioSimbolo(); 

            //F -> '(' E ')'
            if (tokenE.token == TabelaSimbolos.ABPARENTESES)
			{
				casaToken(TabelaSimbolos.ABPARENTESES);
				E(_F);
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
					E(_F);
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
                        tokenE = aLexico.buscarProximoLexema(ler);
                    }
                    else if (tokenE == null)
                    {
                 
                        Erro.ErroSintatico.Arquivo(aLexico.linha);
                    }
                    else if (tokenE.token == TabelaSimbolos.EOF)
                    {

                        Erro.ErroSintatico.Arquivo(aLexico.linha);
                    }
                    else
                    {
                        Erro.ErroSintatico.Lexema(aLexico.linha, tokenE.lexema);
                    }
             }
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}

	}

}

