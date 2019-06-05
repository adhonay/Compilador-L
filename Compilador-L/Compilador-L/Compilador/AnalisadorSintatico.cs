/*
 * Pontifícia Universidade Católica de Minas Gerais
 * Compilador
 * Autores: Adhonay Júnior, Izabela Costa
 * Matricula: 504656, 498535
 **/

using System;
using System.IO;
using System.Collections.Generic;



// Arrumei a questão do E / constante 
//arrumei verdadeiro = 1 nao pode ja tinhamos entendido isso com o not que tnha o mesmo sentido defazer verdadeiro
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
        Memoria m;
        Rotulo r;
        Arquivo a;
        Stack<String> aux = new Stack<String>();

        public AnalisadorSintatico(Stream arquivoEntrada)
		{
            tabela = new TabelaSimbolos();
            aLexico = new AnalisadorLexico(tabela);
            ler = new LerArquivo(arquivoEntrada);
            m = new Memoria();
            r = new Rotulo();
            a = new Arquivo();
			tokenE = aLexico.buscarProximoLexema(ler);                 
		}

		//S-> {D}+ | {C}+
		public void S()
		{
            // acao semantica  GC
                                            
            a.add("sseg SEGMENT STACK       ;início seg. pilha");
            a.add("byte 16384 DUP(?)        ;dimensiona pilha");
            a.add("sseg ENDS                ;fim seg. pilha");                                                                           
            a.add("dseg SEGMENT PUBLIC 	 ;início seg. Dados");
            a.add("byte 16384 DUP(?)        ;temporários");
            a.add("");

            do
            {
				D();
			} while (tokenE.token == TabelaSimbolos.VAR || tokenE.token == TabelaSimbolos.CONST);
			if (tokenE.token == TabelaSimbolos.EOF)
			{
                //erro ocorre pois o programa deve ter ao menos um comando n pode finalizar após declarações
                Erro.ErroSintatico.Arquivo(aLexico.linha);
			}

            // açaõ semantica GC
            a.add("");
            a.add("dseg ENDS                ;fim seg. dados");
            a.add("");
            a.add("cseg SEGMENT PUBLIC 	 ;início seg. Código");
            a.add("ASSUME CS:cseg, DS:dseg");
            a.add("_strt:                   ;início do programa");
            a.add("mov AX, dseg");
            a.add("mov DS, AX");

            

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
            a.add("mov AH, 76               ;mov ah,4Ch ");
            a.add("int 33                   ;int 21h ");
            a.add("cseg ENDS                ;fim seg. código");
            a.add("END _strt                ;fim programa");

            a.print("c:/8086/arquivo.asm");

        }
		// D -> ( VAR {  (INT|CHAR) ID [ATR] {VIRGULA ID [ATR]}*  }+    |     const id= [+|-] constante )
		public void D()
		{
			TemporarioSimbolo _Daux = new TemporarioSimbolo();
            TemporarioSimbolo _D = new TemporarioSimbolo(); // auxilia na construção dos atributos do ATR
            Simbolos auxID = new Simbolos("", Byte.MaxValue);
            Simbolos auxCONST = new Simbolos("", Byte.MaxValue);
            Boolean sinal = false, inicializado = false;

            if (tokenE.token == TabelaSimbolos.VAR)
			{
                //ação semantica 1
                _Daux.classe = Simbolos.CLASSE_VAR;
				casaToken(TabelaSimbolos.VAR);

				do
				{
                    inicializado = false;

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
                        
                        Atr(_D,auxID);
                        inicializado = true;

                        //inicio ação semantica 9
                        if (_D.tipo != auxID.tipo) //atr.tipo != id.tipo erro
                        {
                            if (!(_D.tipo == Simbolos.TIPO_HEXADECIMAL && auxID.tipo == Simbolos.TIPO_CARACTERE)) {
                                Erro.ErroSemantico.Tipos(aLexico.linha);
                            }
                        }
                        else
                        {
                            auxID.tamanho = _D.tamanho;
                        }
                        //fim ação semantica 9

                    }
                    //GERAÇÃO DE CODIGO DELCARAÇÃO VAR INICIO

                    if (inicializado)
                    {
                        if (_D.tipo == Simbolos.TIPO_INTEIRO)
                        {
                            if (_D.tamanho == Simbolos.ESCALAR)
                            {
                                tabela.inserirEndereco(auxID.lexema, m.alocarInteiro());

                                var sinalConstante =  _D.valor.Substring(0, 1);
                                if(sinalConstante == "-")
                                {
                                    int valor = (int.Parse(_D.valor.Substring(1))*-1);
                                    a.add("sword " + valor + "                ;declaração var int neg");
                                }
                                else
                                {
                                    a.add("sword " + int.Parse(_D.valor) + "                 ;declaração var int pos");
                                }
                            }
                            else {// se for vetor.
                                tabela.inserirEndereco(auxID.lexema, m.alocarVetorInterio(_D.tamanho));
                                a.add("byte " + _D.tamanho + " DUP(?)           ;declaração var int vetor");
                            } 

                        }
                        else if (_D.tipo == Simbolos.TIPO_CARACTERE)
                        {
                            if (_D.tamanho == Simbolos.ESCALAR)
                            {
                                tabela.inserirEndereco(auxID.lexema, m.alocarCaractere());
                                char caractere = _D.valor[1];
                                a.add("byte " + (int)caractere + "                   ;declaração var char");
                            }
                            else
                            {// se for vetor.
                                tabela.inserirEndereco(auxID.lexema, m.alocarVetorCaractere(_D.tamanho));
                                a.add("byte " + _D.tamanho + " DUP(?)           ;declaração var char vetor");
                            }
                        }
                        else if (_D.tipo == Simbolos.TIPO_HEXADECIMAL)
                        {
                            tabela.inserirEndereco(auxID.lexema, m.alocarCaractere());
                            var hex = Convert.ToInt64(_D.valor, 16);
                            a.add("byte " + hex + "                 ;declaração var char hex");
                        }
                    }
                    else
                    {
                        if (_Daux.tipo == Simbolos.TIPO_INTEIRO)
                        {
                            tabela.inserirEndereco(auxID.lexema, m.alocarInteiro());
                            a.add("sword ?                  ;declaração var int não inicializada");
                        }
                        else if (_Daux.tipo == Simbolos.TIPO_CARACTERE)
                        {
                            tabela.inserirEndereco(auxID.lexema, m.alocarCaractere());
                            a.add("byte ?                   ;declaração var char não inicializada");
                        }
                    }
                    //GERAÇÃO DE CODIGO DELCARAÇÃO VAR FIM

                    while (tokenE.token == TabelaSimbolos.VIRGULA)
                    {
                        inicializado = false;
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
                            Atr(_D, auxID);
                            inicializado = true;
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
                        //GERAÇÃO DE CODIGO DELCARAÇÃO VAR INICIO

                        if (inicializado)
                        {
                            if (_D.tipo == Simbolos.TIPO_INTEIRO)
                            {
                                if (_D.tamanho == Simbolos.ESCALAR)
                                {
                                    tabela.inserirEndereco(auxID.lexema, m.alocarInteiro());

                                    var sinalConstante = _D.valor.Substring(0, 1);
                                    if (sinalConstante == "-")
                                    {
                                        int valor = (int.Parse(_D.valor.Substring(1)) * -1);
                                        a.add("sword " + valor + "                ;declaração var int neg");
                                    }
                                    else
                                    {
                                        a.add("sword " + int.Parse(_D.valor) + "                 ;declaração var int pos");
                                    }
                                }
                                else
                                {// se for vetor.
                                    tabela.inserirEndereco(auxID.lexema, m.alocarVetorInterio(_D.tamanho));
                                    a.add("sword " + _D.tamanho + " DUP(?)           ;declaração var int vetor");
                                }

                            }
                            else if (_D.tipo == Simbolos.TIPO_CARACTERE)
                            {
                                if (_D.tamanho == Simbolos.ESCALAR)
                                {
                                    tabela.inserirEndereco(auxID.lexema, m.alocarCaractere());
                                    char caractere = _D.valor[1];
                                    a.add("byte " + (int)caractere + "                   ;declaração var char");
                                }
                                else
                                {// se for vetor.
                                    tabela.inserirEndereco(auxID.lexema, m.alocarVetorCaractere(_D.tamanho));
                                    a.add("byte " + _D.tamanho + " DUP(?)           ;declaração var char vetor");
                                }
                            }
                            else if (_D.tipo == Simbolos.TIPO_HEXADECIMAL)
                            {
                                tabela.inserirEndereco(auxID.lexema, m.alocarCaractere());
                                var hex = Convert.ToInt64(_D.valor, 16);
                                a.add("byte " + hex + "                 ;declaração var char hex");
                            }
                        }
                        else
                        {
                            if (_Daux.tipo == Simbolos.TIPO_INTEIRO)
                            {
                                tabela.inserirEndereco(auxID.lexema, m.alocarInteiro());
                                a.add("sword ?                  ;declaração var int não inicializada");
                            }
                            else if (_Daux.tipo == Simbolos.TIPO_CARACTERE)
                            {
                                tabela.inserirEndereco(auxID.lexema, m.alocarCaractere());
                                a.add("byte ?                   ;declaração var char não inicializada");
                            }
                        }
                    }
                    //GERAÇÃO DE CODIGO DELCARAÇÃO VAR FIM

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
                else if(auxCONST.tipo == Simbolos.TIPO_HEXADECIMAL || auxCONST.tipo == Simbolos.TIPO_CARACTERE )
                {
                    auxID.tipo = Simbolos.TIPO_CARACTERE;
                    auxID.tamanho = auxCONST.tamanho;
                }
                else if(auxCONST.tipo == Simbolos.TIPO_INTEIRO )
                {
                    auxID.tipo = auxCONST.tipo;
                    auxID.tamanho = auxCONST.tamanho;
                }
                else
                {
                    //erro tipo inesperado (nao é int, char, hexa ou string) sendo atribuido a constante
                    Erro.ErroSemantico.Tipos(aLexico.linha);
                }
                //fim ação semantica 10


                //GERAÇÃO DE CODIGO DELCARAÇÃO CONST INICIO

                if (auxCONST.tipo == Simbolos.TIPO_INTEIRO)
                {
                    tabela.inserirEndereco(auxID.lexema, m.alocarInteiro());

                    if (sinal == true)
                    {
                        a.add("sword -" + int.Parse(auxCONST.lexema) + "                 ;declaração const int negativa");
                    }
                    else
                    {
                        a.add("sword " + int.Parse(auxCONST.lexema) + "                  ;declaração const int");
                    }
                }
                else if (auxCONST.tipo == Simbolos.TIPO_CARACTERE)
                {
                    tabela.inserirEndereco(auxID.lexema, m.alocarCaractere());
                    char caractere = auxCONST.lexema[1];
                    a.add("byte " + (int)caractere + "                  ;declaração const char");

                }
                else if (auxCONST.tipo == Simbolos.TIPO_HEXADECIMAL)
                {
                    tabela.inserirEndereco(auxID.lexema, m.alocarCaractere());
                    var hex = Convert.ToInt64(auxCONST.lexema, 16);
                    a.add("byte " + hex + "                 ;declaração const char hex");

                }
                //GERAÇÃO DE CODIGO DELCARAÇÃO CONST FIM

                casaToken(TabelaSimbolos.PONTOVIRGULA);
			}
                      
		}
		//Atr -> = [-] constante | [constante]
		public void Atr(TemporarioSimbolo _Atr,Simbolos _ID)
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
                    Erro.ErroSemantico.Tipos(aLexico.linha);
                }
                else if (long.Parse(auxCONST.lexema) > int.MaxValue)
                {
                    Erro.ErroSemantico.Tamanho(aLexico.linha);
                }
                else
                {
                    _Atr.tipo = _ID.tipo;
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
                        Erro.ErroSemantico.Tipos(aLexico.linha);
                    }
                    else
                    {
                        _Atr.tipo = auxCONST.tipo;
                        _Atr.tamanho= auxCONST.tamanho;
                        _Atr.valor = "-"+auxCONST.lexema;
                    }

                }
                else if (auxCONST.tipo == Simbolos.TIPO_INTEIRO)
                {
                    _Atr.tipo = Simbolos.TIPO_INTEIRO;
                    _Atr.tamanho = auxCONST.tamanho;
                    _Atr.valor = auxCONST.lexema;
                }
                else if(auxCONST.tipo == Simbolos.TIPO_CARACTERE)
                {
                    _Atr.tipo = Simbolos.TIPO_CARACTERE;
                    _Atr.tamanho = auxCONST.tamanho;
                    _Atr.valor = auxCONST.lexema;
                }
                else if (auxCONST.tipo == Simbolos.TIPO_HEXADECIMAL)
                {
                    _Atr.tipo = Simbolos.TIPO_HEXADECIMAL;
                    _Atr.tamanho = auxCONST.tamanho;
                    _Atr.valor = auxCONST.lexema;
                }
                else
                {   //string cai aqui
                    _Atr.tipo = Simbolos.TIPO_STRING;
                    _Atr.tamanho = auxCONST.tamanho;
                    _Atr.valor = auxCONST.lexema;
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
            Simbolos _auxIDcTemp = new Simbolos("", Byte.MaxValue);
            TemporarioSimbolo _Ctemp = new TemporarioSimbolo(); // copia de _C para caso seja vetor copiar tamanho.
            Boolean isVetor = false, vetorUtilizado = false, entrouStep, _Celse, _CquebraLinha ;

            //C->ID [ ABCOLCHETE E FECOLCHETE ] IGUAL E PONTOVIRGULA
            if (tokenE.token == TabelaSimbolos.ID)
            {
                
                _auxIDc = tokenE;
                casaToken(tokenE.token);
                

                //inicio ação semantica 0
                if (_auxIDc.classe == Simbolos.SEM_CLASSE)
                {
                    Erro.ErroSemantico.NaoDeclarado(aLexico.linha, _auxIDc.lexema);
                    //erro id não declarado
                }
                //inicio ação semantica 1
                else if (_auxIDc.classe == Simbolos.CLASSE_CONST)
                {
                    Erro.ErroSemantico.Classe(aLexico.linha, _auxIDc.lexema);
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
                    m.resetarTemporario();
                    E(_C);
                    _Ctemp = _C;
                    //inicio açao semantica 3
                    if (_C.tipo != Simbolos.TIPO_INTEIRO || _C.tamanho != Simbolos.ESCALAR )
                    {
                        Erro.ErroSemantico.Tipos(aLexico.linha);
                        //erro dentro do colchete so pode ter numero
                    }
                    else if (_auxIDc.tamanho == 0)
                    {
                        Erro.ErroSemantico.Tipos(aLexico.linha);
                    }//fim ação 3, inicio ação semantica 12
                    else
                    {
                       vetorUtilizado = true;
                    }
                    //fim ação semantica 12
                    casaToken(TabelaSimbolos.FECOLCHETE);
                }
                casaToken(TabelaSimbolos.IGUAL);
                m.resetarTemporario();
                E(_C);
                //inicio ação semantica 4
                if (_C.tipo != _auxIDc.tipo)
                {
                    //permito atribuir string a vetor de caracter
                    if (_C.tipo == Simbolos.TIPO_STRING && _auxIDc.tipo == Simbolos.TIPO_CARACTERE && _auxIDc.tamanho != Simbolos.ESCALAR)
                    {
                        if (_C.tamanho > _auxIDc.tamanho)
                        {
                            Erro.ErroSemantico.Tamanho(aLexico.linha);
                        }
                    }
                    else if (!(_auxIDc.tipo== Simbolos.TIPO_CARACTERE && _C.tipo == Simbolos.TIPO_HEXADECIMAL && vetorUtilizado==true))
                    {
                        Erro.ErroSemantico.Tipos(aLexico.linha);
                    }
                    

                    //ERRO 
                }
                else if (_C.tamanho > 0 && _auxIDc.tamanho > 0 && vetorUtilizado == false)
                {
                    if (_C.tipo == Simbolos.TIPO_INTEIRO || _auxIDc.tipo == Simbolos.TIPO_INTEIRO)
                    {
                        Erro.ErroSemantico.Tipos(aLexico.linha);
                    }

                    if (_C.tamanho > _auxIDc.tamanho)
                    {
                        Erro.ErroSemantico.Tamanho(aLexico.linha);
                    }                  
                }
                else if (_C.tamanho > 0 && _auxIDc.tamanho > 0 && vetorUtilizado == true)
                {
                        Erro.ErroSemantico.Tipos(aLexico.linha);
                }
                else if (_C.tamanho == Simbolos.ESCALAR && _auxIDc.tamanho > 0 && vetorUtilizado == false)
                {
                    Erro.ErroSemantico.Tipos(aLexico.linha);
                }

                //GERAÇÃO DE CODIGO ATRIBUIÇÃO ID INICIO

                if (_auxIDc.tipo == Simbolos.TIPO_INTEIRO)
                {
                    if (_auxIDc.tamanho != Simbolos.ESCALAR && vetorUtilizado == true)//integer id[x]
                    {
                        a.add("");
                        a.add(";atribuição a vetor de inteiro");
                        a.add("mov AX, DS:[" + _Ctemp.endereco + "]          ;carrega valor indexado do vetor");
                        a.add("mov BX, DS:[" + _auxIDc.endereco + "]       ;carrega end inicio vet identificador");
                        a.add("imul 2                   ;multiplica por 2 valor indexado");
                        a.add("add AX, BX               ;soma os dois valores, ax tem o end pos vet para salvar");
                        a.add("mov BX, DS:[" + _C.endereco + "]          ;carrega valor do end exp de igualdade");
                        a.add("mov DS:[AX], BX          ;armazena na pos vet o valor da exp recebida");
                    }
                    else if (_auxIDc.tamanho == Simbolos.ESCALAR && vetorUtilizado == false)//integer id
                    {
                        a.add("");
                        a.add(";atribuição a inteiro escalar");
                        a.add("mov AX, DS:[" + _C.endereco + "]          ;carrega valor do end exp de igualdade");
                        a.add("mov DS:[" + _auxIDc.endereco + "], AX       ;armazena no end id o ax");
                    }

                }
                else if (_auxIDc.tipo == Simbolos.TIPO_CARACTERE) {

                    if (_auxIDc.tamanho != Simbolos.ESCALAR && vetorUtilizado == true)//char id[x]
                    {
                        a.add("");
                        a.add(";atribuição a pos vetor de caractere");
                        a.add("mov AX, DS:[" + _Ctemp.endereco + "]          ;carrega valor indexado do vetor");
                        a.add("mov BX, DS:[" + _auxIDc.endereco + "]       ;carrega end inicio vet identificador");
                        a.add("add AX, BX               ;soma os dois valores, ax tem o end pos vet para salvar");
                        a.add("mov BX, DS:[" + _C.endereco + "]          ;carrega valor do end exp de igualdade");
                        a.add("mov DS:[AX], BX          ;armazena na pos vet o valor da exp recebida");
                    }
                    else if (_auxIDc.tamanho != Simbolos.ESCALAR && vetorUtilizado == false)// char id(vet)
                    {
                        a.add("");
                        a.add(";atribuição de string/vet char a vet caractere");
                        String labelInicio = r.novoRotulo();
                        String labelFim = r.novoRotulo();
                        a.add("mov SI, DS:[" + _C.endereco + "]          ;SI contem string ou vetor");
                        a.add("mov DI, " + _auxIDc.endereco + "            ;DI contem nova string ou vet");
                        a.add("mov AX, DS:[SI]");

                        a.add(labelInicio + ":");
                        a.add("cmp AX, 36");
                        a.add("je " + labelFim);
                        a.add("mov DS:[DI], AX");
                        a.add("add SI, 1");
                        a.add("add DI, 1");
                        a.add("mov AX, DS:[SI]");
                        a.add("jmp " + labelInicio);

                        a.add(labelFim + ":");
                        a.add("mov AX, DS:[SI]");
                    }
                    else if (_auxIDc.tamanho == Simbolos.ESCALAR && vetorUtilizado == false)//char id
                    {
                        a.add("");
                        a.add(";atribuição a caractere escalar");
                        a.add("mov AL, DS:[" + _C.endereco + "]          ;carrega valor do end exp de igualdade");
                        a.add("cbw                      ;extender char. ah = 0");
                        a.add("mov DS:[" + _auxIDc.endereco + "], AX       ;armazena no end id o ax");
                    }

                }
                //GERAÇÃO DE CODIGO ATRIBUIÇÃO ID FIM
                //fim ação semantica 4
                casaToken(TabelaSimbolos.PONTOVIRGULA);
            }
            //C -> FOR ID IGUAL E TO E [STEP CONSTANTE] DO ( ABCHAVE {C}* FECHAVE | C) 
            else if (tokenE.token == TabelaSimbolos.FOR)
            {
                casaToken(TabelaSimbolos.FOR);
                _auxIDc = tokenE;
                casaToken(TabelaSimbolos.ID);

                //inicio ação semantica 0
                if (_auxIDc.classe == Simbolos.SEM_CLASSE)
                {
                    Erro.ErroSemantico.NaoDeclarado(aLexico.linha, _auxIDc.lexema);
                    //erro id não declarado
                }
                //inicio ação semantica 1
                if (_auxIDc.classe == Simbolos.CLASSE_CONST)
                {
                    Erro.ErroSemantico.Classe(aLexico.linha, _auxIDc.lexema);
                    //ERRO IDENTIFICADOR É UMA CONSTANTE NÃO PODE RECEBER NADA
                }
                //incio ação semantica 5
                if (_auxIDc.tipo != Simbolos.TIPO_INTEIRO || _auxIDc.tamanho > 0)
                {
                    Erro.ErroSemantico.Tipos(aLexico.linha);
                    // erro id no for tem q ser inteiro para fazer for id = numero
                }
                //fim ação semantica 5
                casaToken(TabelaSimbolos.IGUAL);
                m.resetarTemporario();
                E(_C);
                _Ctemp = _C;
                //inicio ação semantica 6
                if (_C.tipo != Simbolos.TIPO_INTEIRO || _C.tamanho > 0)
                {
                    Erro.ErroSemantico.Tipos(aLexico.linha);
                    //erro , E tem qe retornar um int para o for
                }
                //fim ação semantica 6

                casaToken(TabelaSimbolos.TO);
                m.resetarTemporario();
                E(_C);

                //inicio ação semantica 6
                if (_C.tipo != Simbolos.TIPO_INTEIRO || _C.tamanho > 0)
                {
                    Erro.ErroSemantico.Tipos(aLexico.linha);
                    //erro , E tem qe retornar um int para o for
                }
                //fim ação semantica 6


                // STEP OPCIONAL
                entrouStep = false;
                if (tokenE.token == TabelaSimbolos.STEP)
                {
                    casaToken(TabelaSimbolos.STEP);
                    _auxCONSTc = tokenE;
                    casaToken(TabelaSimbolos.CONSTANTE);

                    //ação semantica 7
                    if (_auxCONSTc.tipo != Simbolos.TIPO_INTEIRO)
                    {
                        Erro.ErroSemantico.Tipos(aLexico.linha);
                        //erro a constante no step tem q ser int
                    }
                    entrouStep = true;
                    //fim ação semantica 7
                }

                // GERAÇÃO DE CODIGO COMANDO FOR INICIO

                a.add("");
                a.add(";Repetição For");

                a.add("mov AX, DS:[" + _Ctemp.endereco + "]          ;copia valor do E dps da igualdade ");
                a.add("mov DS:[" + _auxIDc.endereco + "], AX       ;armazena o valor no id corespondente");

                String labelInicio = r.novoRotulo();
                String labelFim = r.novoRotulo();

                
                a.add(labelInicio + ":");
                a.add("mov AX, DS:[" + _C.endereco + "]          ;copia valor do E dps do 'TO' ");
                a.add("cmp DS:[" + _auxIDc.endereco + "], AX       ;compara E1 com E2");
                a.add("jg " + labelFim);       
                a.add("mov AX, 1                ;inicializa ");

                if (entrouStep)
                {
                    a.add("mov BX, " + int.Parse(_auxCONSTc.lexema) + "                ;entrou no step, copia valor do pulo");
                    a.add("imul BX                  ;multiplica pulo por ax e salva em ax");
                }

                a.add("push AX                  ;adiciona valor de comparação do for na pilha");
              

                casaToken(TabelaSimbolos.DO);
                if (tokenE.token == TabelaSimbolos.ABCHAVE)
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
                a.add("");
                a.add("pop AX                   ;remove valor de comparação do for da pilha");
                a.add("add DS:[" + _auxIDc.endereco + "], AX       ;soma valor atual da repetição com anterior do id");
                a.add("jmp " + labelInicio);
                a.add(labelFim + ":");
                a.add(";Fim repetição For");
                // GERAÇÃO DE CODIGO COMANDO FOR FIM
            }
            //C -> IF E THEN (ABCHAVE {C}* FECHACHEVE [ELSE ( ABCHAVE {C}* FECHAVE | C )]  |  C[ELSE ( ABCHAVE {C}* FECHAVE | C) ]  )
            else if (tokenE.token == TabelaSimbolos.IF)
            {
                casaToken(TabelaSimbolos.IF);
                a.add("");
                a.add(";comparação if");

                m.resetarTemporario();
                E(_C);

                //inicio ação semantica 8
                if (_C.tipo != Simbolos.TIPO_LOGICO)
                {
                    Erro.ErroSemantico.Tipos(aLexico.linha);
                    //ERRO E TEM QUE RETORNAR LOGICO
                }
                //fim ação semantica 8

                _Celse = false;

                String labelFalso = r.novoRotulo();
                String labelFim = r.novoRotulo();

                
                a.add("mov AL, DS:[" + _C.endereco + "]          ;carrega em al resultado da exp");
                a.add("cmp AL, 255              ;compara com verdadeiro");
                a.add("jne " + labelFalso);
                

                casaToken(TabelaSimbolos.THEN);

                //C -> ABCHAVE {C}* FECHACHEVE [ELSE ( ABCHAVE {C}* FECHAVE | C )] 
                if (tokenE.token == TabelaSimbolos.ABCHAVE)
                {
                    casaToken(TabelaSimbolos.ABCHAVE);
                    //LISTA DE 0 OU VARIOS COMANDOS.
                    while (tokenE.token == TabelaSimbolos.ID || tokenE.token == TabelaSimbolos.FOR ||
                       tokenE.token == TabelaSimbolos.IF || tokenE.token == TabelaSimbolos.WRITE ||
                       tokenE.token == TabelaSimbolos.WRITELN || tokenE.token == TabelaSimbolos.READLN ||
                       tokenE.token == TabelaSimbolos.PONTOVIRGULA)
                    {
                        C();
                    }
                    casaToken(TabelaSimbolos.FECHAVE);
                    if (tokenE.token == TabelaSimbolos.ELSE)
                    {
                        
                        casaToken(TabelaSimbolos.ELSE);
                        _Celse = true;
                        a.add(labelFalso + ":");

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
                    if (tokenE.token == TabelaSimbolos.ELSE)
                    {
                        casaToken(TabelaSimbolos.ELSE);

                        _Celse = true;
                        a.add("jmp " + labelFim);
                        a.add(labelFalso + ":");

                        if (tokenE.token == TabelaSimbolos.ABCHAVE)
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
                if (!_Celse)
                {
                    a.add(labelFalso + ":");
                }
                else
                {
                    a.add(labelFim + ":");
                }
            }
            // C -> READLN  ABPARENTESES ID FEPARENTESES PONTOVIRGULA;
            // id é um identificador de variável inteira, caractere alfanumérico ou stringg
            // esse comando lê e armazena o valor lido em um id. De acordo com o exemplo esse id pode ser de um vetor
            //caso não possa ser id de vetor comentar o if abaixo.
            else if (tokenE.token == TabelaSimbolos.READLN)
            {
                casaToken(TabelaSimbolos.READLN);
                a.add("");
                a.add(";readln");

                casaToken(TabelaSimbolos.ABPARENTESES);
                _auxIDc = tokenE;
                _auxIDcTemp = _auxIDc;
                casaToken(TabelaSimbolos.ID);
                isVetor = false; vetorUtilizado = false;

                //inicio ação semantica 0
                if (_auxIDc.classe == Simbolos.SEM_CLASSE)
                {
                    Erro.ErroSemantico.NaoDeclarado(aLexico.linha, _auxIDc.lexema);

                }
                // fim ação 0 , inicio ação 01
                if (_auxIDc.classe == Simbolos.CLASSE_CONST)
                {
                    Erro.ErroSemantico.Classe(aLexico.linha, _auxIDc.lexema);

                }// fim ação semantica 1, Inicio ação semantica 14
                if (_auxIDc.tipo == Simbolos.TIPO_LOGICO)
                {
                    Erro.ErroSemantico.Tipos(aLexico.linha);
                }
                // fim ação semantica 14, inicio ação semantica 2
                if (_auxIDc.tamanho > 0)
                {
                    isVetor = true;
                }//fim ação semantica 2


                if (tokenE.token == TabelaSimbolos.ABCOLCHETE)
                {
                    //inicio ação semantica 9
                    if (_auxIDc.tamanho == 0)
                    {
                        Erro.ErroSemantico.Tipos(aLexico.linha);
                        //erro 
                    }//fim ação 9
                    casaToken(TabelaSimbolos.ABCOLCHETE);
                    if (tokenE.token == TabelaSimbolos.CONSTANTE)
                    {
                        _auxCONSTc = tokenE;
                        casaToken(TabelaSimbolos.CONSTANTE);
                        //inicio ação semantica 7
                        if (_auxCONSTc.tipo != Simbolos.TIPO_INTEIRO)
                        {
                            Erro.ErroSemantico.Tipos(aLexico.linha);
                            //erro
                        }//fim ação semantica 7

                        a.add(";caso id[int]");
                        a.add("mov AX, " + int.Parse(_auxCONSTc.lexema) + "                ;carrega valor indexado do vetor");
                        //a.add("mov BX, DS:[" + _auxIDc.endereco + "]       ;carrega end inicio vet identificador");
                        if (_auxIDc.tipo == Simbolos.TIPO_INTEIRO)
                        {
                            a.add("imul 2                   ;multiplica por 2 valor indexado");
                        }
                        a.add("push AX");


                        //inicio ação sementica 12
                        vetorUtilizado = true;
                        //fim ação 12
                    }
                    else
                    {
                        _auxIDc = tokenE;
                        casaToken(TabelaSimbolos.ID);
                        //inicio ação semantica 0
                        if (_auxIDc.classe == Simbolos.SEM_CLASSE)
                        {
                            Erro.ErroSemantico.NaoDeclarado(aLexico.linha, _auxIDc.lexema);
                        }
                        if (_auxIDc.tamanho > 0)
                        {
                            Erro.ErroSemantico.Tipos(aLexico.linha);
                        }
                        //fim ação semantica 0, inicio ação 10
                        if (_auxIDc.tipo != Simbolos.TIPO_INTEIRO)
                        {
                            Erro.ErroSemantico.Tipos(aLexico.linha);

                        }//fim ação semantica 10


                        a.add(";caso id[id]");
                        a.add("mov AX, DS:[" + _auxIDc.endereco + "]       ;carrega valor indexado do vetor");
                        a.add("imul 2                   ;multiplica por 2 valor indexado");
                        a.add("push AX");

                        //inicio ação semantica 12
                        vetorUtilizado = true;
                        //fim ação 12
                    }

                    casaToken(TabelaSimbolos.FECOLCHETE);
                }

                //inicio ação semantica 13
                if (vetorUtilizado != isVetor && _auxIDc.tipo == Simbolos.TIPO_INTEIRO)
                {
                    Erro.ErroSemantico.Tipos(aLexico.linha);
                }

                a.add("mov SI, " + _auxIDcTemp.endereco);


                if (isVetor == true && vetorUtilizado == false && _auxIDcTemp.tipo == Simbolos.TIPO_CARACTERE)
                {// atribuir string a vetor de char.

                    int enderecoBufferTemp = m.allocateTemporaryBuffer();

                    String labelInicio = r.novoRotulo();
                    String labelFim = r.novoRotulo();

                    a.add("");
                    a.add(";atribuir string a vetor de char.");
                    a.add("mov DX, " + enderecoBufferTemp);
                    a.add("mov AL, 255");
                    a.add("mov DS:[" + enderecoBufferTemp + "], AL");
                    a.add("mov AH, 10");
                    a.add("int 33");
                    a.add("mov AH, 2");
                    a.add("mov DL, 13");
                    a.add("int 33");
                    a.add("mov DL, 10");
                    a.add("int 33");

                    a.add("mov DI, " + enderecoBufferTemp + 2);
                    a.add(labelInicio + ":");
                    a.add("mov AX, DS:[DI] ");
                    a.add("cmp AX, 13");
                    a.add("je " + labelFim + "");
                    a.add("mov AH, 0");
                    a.add("mov AL, DS:[DI]");
                    a.add("mov DS:[SI], AX");
                    a.add("add SI, 1 ");
                    a.add("add DI, 1 ");
                    a.add("jmp " + labelInicio + "");
                    a.add(labelFim + ":");
                    a.add("mov AH, 0");
                    a.add("mov AL, 36");

                    a.add("mov DS:[SI], AL");
                }
                else 
                {
                    int enderecoBufferTemp = m.allocateTemporaryBuffer();

                    if (_auxIDcTemp.tipo == Simbolos.TIPO_INTEIRO)
                    {
                        String labelR0 = r.novoRotulo();
                        String labelR1 = r.novoRotulo();
                        String labelR2 = r.novoRotulo();

                        a.add("");
                        a.add(";atribui inteiro à variável.");
                        a.add("mov DX, " + enderecoBufferTemp);
                        a.add("mov AL, 255");
                        a.add("mov DS:[" + enderecoBufferTemp + "], AL");
                        a.add("mov AH, 10");
                        a.add("int 33");
                        a.add("mov AH, 2");
                        a.add("mov DL, 13");
                        a.add("int 33");
                        a.add("mov DL, 10");
                        a.add("int 33");


                        a.add("mov DI, " + (enderecoBufferTemp + 2) + "              ;posição do string");
                        a.add("mov AX, 0                ;acumulador");
                        a.add("mov CX, 10               ;base decimal");
                        a.add("mov DX, 1                ;valor sinal +");
                        a.add("mov BH, 0");
                        a.add("mov BL, DS:[DI]          ;caractere");
                        a.add("cmp BX, 45               ;verifica sinal");
                        a.add("jne " + labelR0 + "                   ;se não negativo");
                        a.add("mov DX, -1               ;valor sinal -");
                        a.add("add DI, 1                ;incrementa base");
                        a.add("mov BL, DS:[DI]          ;próximo caractere");

                        a.add(labelR0 + ":");
                        a.add("push DX                  ;empilha sinal");
                        a.add("mov dx, 0                ;reg. multiplicacao");

                        a.add(labelR1 + ":");
                        a.add("cmp BX, 13               ;verifica fim string");
                        a.add("je " + labelR2 + "                    ;salta se fim string");
                        a.add("imul CX                  ;mult. 10");
                        a.add("add BX, -48              ;converte caractere");
                        a.add("add AX, BX               ;soma valor caractere");
                        a.add("add DI, 1                ;incrementa base");
                        a.add("mov BH, 0");
                        a.add("mov BL, DS:[DI]          ;próximo caractere");
                        a.add("jmp " + labelR1 + "                   ;loop");

                        a.add(labelR2 + ":");
                        a.add("pop CX                   ;desempilha sinal");
                        a.add("imul CX                  ;mult. sinal");

                        if (vetorUtilizado)
                        {// indexar no vetor
                            a.add("pop BX                   ;pega posição indexada do vetor");
                            a.add("add SI, BX               ;soma posicao com valor indexado");
                        }

                        a.add("mov DS:[SI], AX");
                    }
                    else
                    {//então e caractere

                        a.add("");
                        a.add(";atribui caractere à variável.");

                        //Leitura do DOS
                        a.add("mov DX, " + enderecoBufferTemp);
                        a.add("mov AL, 255");
                        a.add("mov DS:[" + enderecoBufferTemp + "], AL");
                        a.add("mov AH, 10");
                        a.add("int 33");

                        //Quebra de linha
                        a.add("mov AH, 2");
                        a.add("mov DL, 13");
                        a.add("int 33");
                        a.add("mov DL, 10");
                        a.add("int 33");

                        if (vetorUtilizado)
                        {// indexar no vetor
                            a.add("pop BX                   ;pega posição indexada do vetor");
                            a.add("add SI, BX               ;soma posicao com valor indexado");
                        }

                        a.add("mov DI, " + (enderecoBufferTemp + 2) + "             ;posição do string");
                        a.add("mov AH, 0                ;acumulador");
                        a.add("mov AL, DS:[DI]          ;caractere");
                        a.add("mov DS:[SI], AL");
                    }

                }
           
                //fim ação semtnica 13
                casaToken(TabelaSimbolos.FEPARENTESES);
                casaToken(TabelaSimbolos.PONTOVIRGULA);

            }
            // C-> (WRITELN | WRITE) ABPARENTESES (E ) {VIRGULA ( E )  }*  FEPARENTESES PONTOVIRGULA
            else if (tokenE.token == TabelaSimbolos.WRITE || tokenE.token == TabelaSimbolos.WRITELN) //---- QUEBRA DE LINHA EM LN !?
            {
                if (tokenE.token == TabelaSimbolos.WRITE)
                {
                    casaToken(TabelaSimbolos.WRITE);
                    a.add("");
                    a.add(";write");
                    _CquebraLinha = false;
                }
                else
                {
                    casaToken(TabelaSimbolos.WRITELN);
                    a.add("");
                    a.add(";writeln");
                    _CquebraLinha = true;
                }

                casaToken(TabelaSimbolos.ABPARENTESES);
                m.resetarTemporario();
                E(_C);

                if (_C.tipo == Simbolos.TIPO_LOGICO)
                {
                    Erro.ErroSemantico.Tipos(aLexico.linha);
                }

                if (_C.tipo == Simbolos.TIPO_STRING || _C.tamanho != Simbolos.ESCALAR)
                {
                    a.add("");
                    a.add(";printar string/vet ");
                    a.add("mov DX, " + _C.endereco);
                    a.add("mov AH, 9");
                    a.add("int 033");
                }
                else
                {
                    String labelR0 = r.novoRotulo();
                    String labelR1 = r.novoRotulo();
                    String labelR2 = r.novoRotulo();
                    int stringEndereco = m.alocarTemporarioString();

                    a.add("mov DI, " + stringEndereco + " ;end. string temp");

                    if (_C.tipo == Simbolos.TIPO_INTEIRO)
                    {
                        a.add("");
                        a.add(";printar interio");
                        a.add("mov AX, DS:[" + _C.endereco + "]");
                    }
                    else
                    {
                        a.add("");
                        a.add(";printar caractere");
                        a.add("mov AL, DS:[" + _C.endereco + "]");
                        a.add("cbw");
                    }

                    a.add("mov CX, 0 ;contador");
                    a.add("cmp AX,0 ;verifica sinal");
                    a.add("jge " + labelR0 + " ;salta se número positivo");
                    a.add("mov BL, 45 ;senão, escreve sinal –");
                    a.add("mov DS:[DI], BL");
                    a.add("add DI, 1 ;incrementa índice");
                    a.add("neg AX ;toma módulo do número");

                    a.add(labelR0 + ":");
                    a.add("mov BX, 10 ;divisor");

                    a.add(labelR1 + ":");
                    a.add("add CX, 1 ;incrementa contador");
                    a.add("mov DX, 0 ;estende 32bits p/ div.");
                    a.add("idiv BX ;divide DXAX por BX");
                    a.add("push DX ;empilha valor do resto");
                    a.add("cmp AX, 0 ;verifica se quoc. é 0");
                    a.add("jne " + labelR1 + " ;se não é 0, continua");

                    a.add(";agora, desemp. os valores e escreve o string");
                    a.add(labelR2 + ":");
                    a.add("pop DX ;desempilha valor");
                    a.add("add DX, 48 ;transforma em caractere");
                    a.add("mov DS:[DI],DL ;escreve caractere");
                    a.add("add DI, 1 ;incrementa base");
                    a.add("add CX, -1 ;decrementa contador");
                    a.add("cmp CX, 0 ;verifica pilha vazia");
                    a.add("jne " + labelR2 + " ;se não pilha vazia, loop");

                    a.add(";grava fim de string");
                    a.add("mov DL, 36 ;fim de string");
                    a.add("mov DS:[DI], DL ;grava '$'");

                    a.add(";exibe string");
                    a.add("mov DX, " + stringEndereco);
                    a.add("mov AH, 9");
                    a.add("int 33");

                    if (_CquebraLinha)
                    {
                        a.add("");
                        a.add(";quebra de linha");
                        a.add("mov DX, 13");
                        a.add("mov AH, 2");
                        a.add("int 33");
                        a.add("mov DX, 10");
                        a.add("int 33");
                    }
                }

                while (tokenE.token == TabelaSimbolos.VIRGULA)
                {
                    casaToken(TabelaSimbolos.VIRGULA);
                    m.resetarTemporario();
                    E(_C);

                    if (_C.tipo == Simbolos.TIPO_LOGICO)
                    {
                        Erro.ErroSemantico.Tipos(aLexico.linha);
                    }

                    if (_C.tipo == Simbolos.TIPO_STRING || _C.tamanho != Simbolos.ESCALAR)
                    {
                        a.add("");
                        a.add(";printar string/vet");
                        a.add("mov DX, " + _C.endereco);
                        a.add("mov AH, 9");
                        a.add("int 033");
                    }
                    else
                    {
                        String labelR0 = r.novoRotulo();
                        String labelR1 = r.novoRotulo();
                        String labelR2 = r.novoRotulo();
                        int stringEndereco = m.alocarTemporarioString();

                        a.add("mov DI, " + stringEndereco + " ;end. string temp");

                        if (_C.tipo == Simbolos.TIPO_INTEIRO)
                        {
                            a.add("");
                            a.add(";printar interio");
                            a.add("mov AX, DS:[" + _C.endereco + "]");
                        }
                        else
                        {
                            a.add("");
                            a.add(";printar caractere");
                            a.add("mov AL, DS:[" + _C.endereco + "]");
                            a.add("cbw");
                        }

                        a.add("mov CX, 0 ;contador");
                        a.add("cmp AX,0 ;verifica sinal");
                        a.add("jge " + labelR0 + " ;salta se número positivo");
                        a.add("mov BL, 45 ;senão, escreve sinal –");
                        a.add("mov DS:[DI], BL");
                        a.add("add DI, 1 ;incrementa índice");
                        a.add("neg AX ;toma módulo do número");

                        a.add(labelR0 + ":");
                        a.add("mov BX, 10 ;divisor");

                        a.add(labelR1 + ":");
                        a.add("add CX, 1 ;incrementa contador");
                        a.add("mov DX, 0 ;estende 32bits p/ div.");
                        a.add("idiv BX ;divide DXAX por BX");
                        a.add("push DX ;empilha valor do resto");
                        a.add("cmp AX, 0 ;verifica se quoc. é 0");
                        a.add("jne " + labelR1 + " ;se não é 0, continua");

                        a.add(";agora, desemp. os valores e escreve o string");
                        a.add(labelR2 + ":");
                        a.add("pop DX ;desempilha valor");
                        a.add("add DX, 48 ;transforma em caractere");
                        a.add("mov DS:[DI],DL ;escreve caractere");
                        a.add("add DI, 1 ;incrementa base");
                        a.add("add CX, -1 ;decrementa contador");
                        a.add("cmp CX, 0 ;verifica pilha vazia");
                        a.add("jne " + labelR2 + " ;se não pilha vazia, loop");

                        a.add(";grava fim de string");
                        a.add("mov DL, 36 ;fim de string");
                        a.add("mov DS:[DI], DL ;grava '$'");

                        a.add(";exibe string");
                        a.add("mov DX, " + stringEndereco);
                        a.add("mov AH, 9");
                        a.add("int 33");

                        if (_CquebraLinha)
                        {
                            a.add("");
                            a.add(";quebra de linha");
                            a.add("mov DX, 13");
                            a.add("mov AH, 2");
                            a.add("int 33");
                            a.add("mov DX, 10");
                            a.add("int 33");
                        }
                    }
                }
                casaToken(TabelaSimbolos.FEPARENTESES);
                casaToken(TabelaSimbolos.PONTOVIRGULA);

            }
            // C -> ;
            else
            {
                casaToken(TabelaSimbolos.PONTOVIRGULA);
                a.add("");
                a.add("; Ponto virgula");
            }
        }
        // E -> ES [ ( IGUAL(aceita vetor, aceita string ) | MENOR | MAIOR | MENORIGUAL | MAIORIGUAL | DIFERENTE ) ES ]
        public void E(TemporarioSimbolo _E)
        {
            TemporarioSimbolo _ES1 = new TemporarioSimbolo();
            byte temp;
            ES(_E);
            temp = 0;

            if (tokenE.token == TabelaSimbolos.IGUAL || tokenE.token == TabelaSimbolos.MENOR ||
                tokenE.token == TabelaSimbolos.MAIOR || tokenE.token == TabelaSimbolos.MENORIGUAL ||
                tokenE.token == TabelaSimbolos.MAIORIGUAL || tokenE.token == TabelaSimbolos.DIFERENTE)
            {
                temp = tokenE.token;

                casaToken(tokenE.token);
                ES(_ES1);

                //inicio ação semantica
                if (temp != TabelaSimbolos.IGUAL)
                {
                    //permito relacionais inteiro com inteiro e caracter com caracter mas n vetores
                    if (_E.tipo != Simbolos.TIPO_INTEIRO && _E.tipo !=Simbolos.TIPO_CARACTERE && _E.tipo != Simbolos.TIPO_HEXADECIMAL ||
                        _ES1.tipo != Simbolos.TIPO_INTEIRO && _ES1.tipo != Simbolos.TIPO_CARACTERE && _ES1.tipo != Simbolos.TIPO_HEXADECIMAL||
                        _ES1.tipo != _E.tipo && !(_ES1.tipo == Simbolos.TIPO_HEXADECIMAL && _E.tipo == Simbolos.TIPO_CARACTERE || _ES1.tipo == Simbolos.TIPO_CARACTERE && _E.tipo == Simbolos.TIPO_HEXADECIMAL)
                        || _E.tamanho != Simbolos.ESCALAR || _ES1.tamanho != Simbolos.ESCALAR) 
                    {
                        Erro.ErroSemantico.Tipos(aLexico.linha);
                    }
                }
                else
                {

                    if (!((_E.tipo == Simbolos.TIPO_INTEIRO && _ES1.tipo == Simbolos.TIPO_INTEIRO && _E.tamanho == Simbolos.ESCALAR && _ES1.tamanho == Simbolos.ESCALAR)
                       || (_E.tipo == Simbolos.TIPO_STRING && _ES1.tipo == Simbolos.TIPO_STRING)
                       || (_E.tipo == Simbolos.TIPO_STRING && _ES1.tipo == Simbolos.TIPO_CARACTERE && _ES1.tamanho > 0)// n permite string ser comparadda com logico
                       || (_E.tipo == Simbolos.TIPO_CARACTERE && _E.tamanho > 0 && _ES1.tipo == Simbolos.TIPO_STRING)
                       || (_E.tipo == Simbolos.TIPO_CARACTERE && _ES1.tipo == Simbolos.TIPO_CARACTERE && !(_E.tamanho > 0 && _ES1.tamanho == 0 || _E.tamanho == 0 && _ES1.tamanho > 0) )
                       || (_E.tipo == Simbolos.TIPO_HEXADECIMAL && _ES1.tipo == Simbolos.TIPO_CARACTERE && !(_E.tamanho > 0 && _ES1.tamanho == 0 || _E.tamanho == 0 && _ES1.tamanho > 0))
                       || ( _E.tipo == Simbolos.TIPO_CARACTERE && _ES1.tipo == Simbolos.TIPO_HEXADECIMAL && !(_E.tamanho > 0 && _ES1.tamanho == 0 || _E.tamanho == 0 && _ES1.tamanho > 0))
                       || (_E.tipo == Simbolos.TIPO_HEXADECIMAL && _ES1.tipo == Simbolos.TIPO_HEXADECIMAL && !(_E.tamanho > 0 && _ES1.tamanho == 0 || _E.tamanho == 0 && _ES1.tamanho > 0))
                       || (_E.tipo == Simbolos.TIPO_INTEIRO && _E.tamanho == Simbolos.ESCALAR &&_ES1.tipo == Simbolos.TIPO_LOGICO) // permite int ser comparado com logico
                       || (_E.tipo == Simbolos.TIPO_LOGICO && _ES1.tipo == Simbolos.TIPO_INTEIRO && _ES1.tamanho ==Simbolos.ESCALAR)))
                    {
                        Erro.ErroSemantico.Tipos(aLexico.linha);
                    }
                }

                _E.tipo = Simbolos.TIPO_LOGICO;
            }
        }
        //ES -> [ + | - ] T { ( + | - | or ) T }*
        public void ES(TemporarioSimbolo _ES)
        {
            TemporarioSimbolo _T1 = new TemporarioSimbolo();
            Boolean opcao = false, opMenos = false, opMais = false, opLogico = false;

            if (tokenE.token == TabelaSimbolos.MAIS || tokenE.token == TabelaSimbolos.MENOS)
            {
                if (tokenE.token == TabelaSimbolos.MAIS)
                {
                    opMais = true;
                }
                else
                {
                    opMenos = true;
                }
                casaToken(tokenE.token);
                opcao = true;

            }
            T(_ES);
            //inicio ação semantica
            //não permito um vetor receber um sinal negativo na frente 
            if (opcao == true && (_ES.tipo != Simbolos.TIPO_INTEIRO || _ES.tamanho != Simbolos.ESCALAR))
                
            {
                Erro.ErroSemantico.Tipos(aLexico.linha);
            }
            //fim ação semantica
            while (tokenE.token == TabelaSimbolos.MAIS || tokenE.token == TabelaSimbolos.MENOS || tokenE.token == TabelaSimbolos.OR)
            {
                //inicio ação semantica 
                if (tokenE.token == TabelaSimbolos.OR)
                {
                    opLogico = true;
                    opMais = false; opMenos = false;
                }
                else
                {
                    opLogico = false;
                    if (tokenE.token == TabelaSimbolos.MAIS)
                    {
                        opMais = true;
                        opMenos = false;
                    }
                    else
                    {
                        opMenos = true;
                        opMais = false;

                    }
                }
                //fim ação semantica 
                casaToken(tokenE.token);
                T(_T1);
                //inicio ação smenatica
                if (opLogico == true)
                {
                    if (_ES.tipo != Simbolos.TIPO_LOGICO || _T1.tipo != Simbolos.TIPO_LOGICO)

                    {
                        Erro.ErroSemantico.Tipos(aLexico.linha);
                    }
                }
                else
                {
                    // permito somar int com int ou caracter com caracter mas não vetores
                    if (_ES.tipo != Simbolos.TIPO_INTEIRO && _ES.tipo != Simbolos.TIPO_CARACTERE && _ES.tipo!= Simbolos.TIPO_HEXADECIMAL ||
                        _T1.tipo != Simbolos.TIPO_INTEIRO && _T1.tipo != Simbolos.TIPO_CARACTERE && _T1.tipo != Simbolos.TIPO_HEXADECIMAL ||
                        _T1.tipo != _ES.tipo && !(_T1.tipo == Simbolos.TIPO_HEXADECIMAL && _ES.tipo == Simbolos.TIPO_CARACTERE || _T1.tipo == Simbolos.TIPO_CARACTERE && _ES.tipo == Simbolos.TIPO_HEXADECIMAL)
                        || _T1.tamanho > 0 || _ES.tamanho > 0) 

                    {
                        Erro.ErroSemantico.Tipos(aLexico.linha);
                    }
                }
                //fim ação semantica

            }

        }

        // T-> F { ( * | / | % | and ) F }
        public void T(TemporarioSimbolo _T)
        {
            TemporarioSimbolo _F1 = new TemporarioSimbolo();
            Boolean and = false, mult = false, div = false, mod = false;

            F(_T); // inicio fim ação semantica 9
            while (tokenE.token == TabelaSimbolos.MULTIPLICACAO || tokenE.token == TabelaSimbolos.DIVISAO ||
                tokenE.token == TabelaSimbolos.PORCENTAGEM || tokenE.token == TabelaSimbolos.AND)
            {
                if (tokenE.token == TabelaSimbolos.MULTIPLICACAO)
                {
                    mult = true;
                }
                else if (tokenE.token == TabelaSimbolos.DIVISAO)
                {
                    div = true;
                }
                else if (tokenE.token == TabelaSimbolos.PORCENTAGEM)
                {
                    mod = true;
                }
                else
                {
                    and = true;
                }

                casaToken(tokenE.token);
                F(_F1);

                //inicio ação semantica 10
                //permito soma de int com int ou caracter com caracter mas n de vetores
                if ((mult == true || div == true || mod == true) &&
                       (_T.tipo != Simbolos.TIPO_INTEIRO && _T.tipo !=Simbolos.TIPO_CARACTERE && _T.tipo != Simbolos.TIPO_HEXADECIMAL   ||
                         _F1.tipo != Simbolos.TIPO_INTEIRO && _F1.tipo != Simbolos.TIPO_CARACTERE && _F1.tipo != Simbolos.TIPO_HEXADECIMAL  ||
                         _F1.tipo != _T.tipo && !(_F1.tipo == Simbolos.TIPO_HEXADECIMAL && _T.tipo == Simbolos.TIPO_CARACTERE || _F1.tipo == Simbolos.TIPO_CARACTERE && _T.tipo == Simbolos.TIPO_HEXADECIMAL)
                         || _T.tamanho > 0 || _F1.tamanho > 0))
                  
                {
                    Erro.ErroSemantico.Tipos(aLexico.linha);

                }
                else if (and == true && (_T.tipo != Simbolos.TIPO_LOGICO || _F1.tipo != Simbolos.TIPO_LOGICO ))
                {
                    Erro.ErroSemantico.Tipos(aLexico.linha);
                }
                //fim ação semantica 10
                and = false; mult = false; div = false; mod = false;

            }
        }
        // F -> ABPARENTESES E FEPARENTESES |CONSTANTE| ID[ ABCOLCHETE E FECOLHETE ] | NOT F
        public void F(TemporarioSimbolo _F)
        {
            TemporarioSimbolo _F1 = new TemporarioSimbolo();
            Simbolos _auxCONSTf = new Simbolos("", Byte.MaxValue);
            Simbolos _auxIDf = new Simbolos("", Byte.MaxValue);
            Boolean isVetor = false, isVetorUtilizado = false;

            //F -> '(' E ')'
            if (tokenE.token == TabelaSimbolos.ABPARENTESES)
            {
                casaToken(TabelaSimbolos.ABPARENTESES);
                E(_F);
                //inicio e fim da ação semantica 1 ocorre na linha acima
                casaToken(TabelaSimbolos.FEPARENTESES);

            }// F -> CONSTANTE
            else if (tokenE.token == TabelaSimbolos.CONSTANTE)
            {
                _auxCONSTf = tokenE;
                casaToken(TabelaSimbolos.CONSTANTE);

                //inicio ação semantica 2
                _F.tipo = _auxCONSTf.tipo;
                _F.tamanho = _auxCONSTf.tamanho;
                //fim ação semantica 2

                if (_auxCONSTf.tipo == Simbolos.TIPO_STRING)
                {
                    _F.tamanho = _auxCONSTf.lexema.Length;
                }


            }// F-> ID[ "[" E "]" ] 
            else if (tokenE.token == TabelaSimbolos.ID)
            {
                _auxIDf = tokenE;
                casaToken(TabelaSimbolos.ID);

                //inicio ação semantica 3
                if (_auxIDf.classe == Simbolos.SEM_CLASSE)
                {
                    Erro.ErroSemantico.NaoDeclarado(aLexico.linha, _auxIDf.lexema);
                }
                //fim ação semantica 3, inicio ação 4
                if (_auxIDf.tamanho > 0)
                {
                    isVetor = true;
                }
                //fim ação 4 

                if (tokenE.token == TabelaSimbolos.ABCOLCHETE)
                {
                    //inicio ação semantica 5
                    if (_auxIDf.classe == Simbolos.CLASSE_CONST)
                    {
                        Erro.ErroSemantico.Classe(aLexico.linha, _auxIDf.lexema);
                    }
                    else if (_auxIDf.tamanho <= 0)
                    {
                        Erro.ErroSemantico.Tipos(aLexico.linha);
                    }
                    // fim ação semantica 5

                    casaToken(TabelaSimbolos.ABCOLCHETE);
                    E(_F);

                    //inicio ação semantica 6
                    if (_F.tipo != Simbolos.TIPO_INTEIRO)
                    {
                        Erro.ErroSemantico.Tipos(aLexico.linha);
                    }
                    else
                    {
                        isVetorUtilizado = true;
                        _F.tipo = _auxIDf.tipo;
                        _F.tamanho = Simbolos.ESCALAR;

                    }
                    //fim ação semantica 6

                    casaToken(TabelaSimbolos.FECOLCHETE);
                }
                else
                {
                    _F.tipo = _auxIDf.tipo;
                    _F.tamanho = _auxIDf.tamanho;                 
                }
                isVetor = false; isVetorUtilizado = false;

                //fim ação semantica 7

            }// F-> NOT F
            else
            {
                casaToken(TabelaSimbolos.NOT);
                F(_F1);

                //inicio ação semantica 8
                if (_F1.tipo != Simbolos.TIPO_LOGICO)
                {
                    Erro.ErroSemantico.Tipos(aLexico.linha);
                }
                else
                {
                    _F.tipo = Simbolos.TIPO_LOGICO;
                }
                //fim ação semantica 8
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


