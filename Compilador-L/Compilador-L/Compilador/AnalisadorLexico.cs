using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador_L.Compilador
{
    public class AnalisadorLexico
    {


        TabelaSimbolos tbSimbolos;
        Simbolos simbolo;

        private string lexema;
        private char _char;
        public int linha;
        public bool devolve;
        public bool EOF;
		public int getLinha()
		{
			return this.linha;
		}
        public  AnalisadorLexico(TabelaSimbolos tabelaSimbolos)
        {
            linha = 1;
            tbSimbolos = tabelaSimbolos;
            devolve = false;
            EOF = false;
        }

        public Simbolos buscarProximoLexema(LerArquivo arquivo)
        {


            int estadoAtual = 0;
            int estadoFinal = 16;

            while(estadoAtual != estadoFinal)
            {

                switch (estadoAtual) {

                    case 0:
                        // Ler proximo caractere caso devolver for false.
                        if (!devolve)
                        {
                            _char = (char)arquivo.Read();   
                        }

                        devolve = false;
                        lexema = "";

                        if (eCaractere(_char))
                        {   
                            if (_char ==  10 || _char == 11)
                            {// Próxima linha 0Ah ou Tab vertical (\v).
                                estadoAtual = 0;
                                linha++;
                            }

                            else if (_char == 8 || _char == 9 || _char == 13 || _char == 32)
                            {// Apagar (\b) ou Tab horizontal (\t) ou Início da linha 0Dh (\r) ou Espaço.
                                estadoAtual = 0;
                            }
                            // Confere se char e um dos caracteres abaixo e aceita como token.
                            else if (_char == 10 || _char == '[' || _char == ']' || _char == '%' || _char == ')' || _char == '(' || _char == '='
                                 || _char == ';' || _char == ',' || _char == '+' || _char == '-' || _char == '*' || _char == '{' || _char == '}')
                            {
                                lexema += _char;
                                estadoAtual = estadoFinal;
                            }

                            else if (eLetra(_char))
                            {// É letra então vai pro estado 1.
                                lexema += _char;
                                estadoAtual = 1;
                            }

                            else if (_char == '_')
                            {// É sublinhado então vai pro estado 2.
                                lexema += _char;
                                estadoAtual = 2;
                            }

                            else if (_char == '.')
                            {// É ponto então vai pro estado 3.
                                lexema += _char;
                                estadoAtual = 3;
                            }

                            else if (_char == '<')
                            {// É menor então vai pro estado 4.
                                lexema += _char;
                                estadoAtual = 4;
                            }

                            else if (_char == '>')
                            {// É maior então vai pro estado 5.
                                lexema += _char;
                                estadoAtual = 5;
                            }

                            else if (_char == '0')
                            {// É zero então vai pro estado 6.
                                lexema += _char;
                                estadoAtual = 6;
                            }

                            else if (eDigito(_char) && _char != '0')
                            {// É digito e diferente de zero então vai pro estado 10.
                                lexema += _char;
                                estadoAtual = 9;
                            }

                            else if (_char == '/')
                            {// É barra então vai pro estado 11.
                                lexema += _char;
                                estadoAtual = 10;
                            }

                            else if (_char == '"')
                            {// É aspas então vai pro estado 14.
                                lexema += _char;
                                estadoAtual = 13;
                            }

                            else if (_char == 39)
                            {// É apóstofro então vai pro estado 15.
                                lexema += _char;
                                estadoAtual = 14;
                            }

                            else if (_char == -1 || _char == 65535)
                            {// É final de arquivo então vai pro estado 16(final).
                                lexema += _char;
                                estadoAtual = estadoFinal;
                                EOF = true;
                                arquivo.Close();
                            }

                            else
                            {// Lexema não identificado.
                                lexema += _char;
                                Erro.ErroLexico.Lexema(linha,lexema);
                            }
                        }
                        else
                        {// Caractere invalido.
                            Erro.ErroLexico.Char(linha);
                        }

                        break;// Fim estado 0.

                    case 1:

                        _char = (char)arquivo.Read();

                        // Verifica se caractere valido.
                        if (eCaractere(_char))
                        {
                            if (eLetra(_char) | eDigito(_char) | _char == '_' | _char == '.')
                            {// Verifica se obedece as condições para montagem do token identificador.

                                lexema += _char;
                                estadoAtual = 1;
                            }
                            else
                            {// Reconhece ID
                                estadoAtual = estadoFinal;
                                devolve = true;
                            }
                        }
                        else
                        {// Caractere Invalido.
                            Erro.ErroLexico.Char(linha);
                        }

                        break;

                    case 2:

                        _char = (char)arquivo.Read();

                        // Verifica se caractere valido.
                        if (eCaractere(_char))
                        {
                            // "Loop" _ 
                            if (_char == '_')
                            {
                                lexema += _char;
                                estadoAtual = 2;
                            }
                            else if (eLetra(_char) | eDigito(_char) | _char == '.')
                            {// Verifica se obedece as condições para montagem do token identificador

                                lexema += _char;
                                estadoAtual = 1;//Volta para estado 1 garantindo não ter somente _
                            }
                            else if (_char == -1 || _char == 65535)
                            {// Fim do arquivo não esperado.
                                Erro.ErroLexico.Arquivo(linha);
                            }
                            else
                            {// Caso seja outro caractere não permitido, logo não consegue identificar o lexema.
                                Erro.ErroLexico.Lexema(linha, lexema);
                            }
                        }
                        else
                        {// Caractere Invalido.
                            Erro.ErroLexico.Char(linha);
                        }

                        break;

                    case 3:

                        _char = (char)arquivo.Read();

                        // Verifica se caractere valido.
                        if (eCaractere(_char))
                        {
                            // "Loop" _ 
                            if (_char == '.')
                            {
                                lexema += _char;
                                estadoAtual = 3;
                            }
                            else if (eLetra(_char) | eDigito(_char) | _char == '_')
                            {// Verifica se obedece as condições para montagem do token identificador

                                lexema += _char;
                                estadoAtual = 1;//Volta para estado 1 garantindo não ter somente ponto
                            }
                            else if (_char == -1 || _char == 65535)
                            {// Fim do arquivo não esperado.
                                Erro.ErroLexico.Arquivo(linha);
                            }
                            else
                            {// Caso seja outro caractere não permitido, logo não consegue identificar o lexema.
                                Erro.ErroLexico.Lexema(linha, lexema);
                            }
                        }
                        else
                        {// Caractere Invalido.
                            Erro.ErroLexico.Char(linha);
                        }

                        break;

                    case 4:

                        _char = (char)arquivo.Read();

                        // Verifica se caractere valido.
                        if (eCaractere(_char))
                        {
                            // token <> ou <=
                            if (_char == '>' | _char == '=')
                            {
                                lexema += _char;
                                estadoAtual = estadoFinal;
                            }
                            else 
                            {// token < e devolve o proximo.
                                devolve = true;
                                estadoAtual = estadoFinal;
                            }
                        }
                        else
                        {// Caractere Invalido.
                            Erro.ErroLexico.Char(linha);
                        }

                        break;

                    case 5:

                        _char = (char)arquivo.Read();

                        // Verifica se caractere valido.
                        if (eCaractere(_char))
                        {
                            // token >= 
                            if (_char == '=')
                            {
                                lexema += _char;
                                estadoAtual = estadoFinal;
                            }
                            else
                            {// token > e devolve o proximo.
                                devolve = true;
                                estadoAtual = estadoFinal;
                            }
                        }
                        else
                        {// Caractere Invalido.
                            Erro.ErroLexico.Char(linha);
                        }

                        break;

                    case 6:

                        _char = (char)arquivo.Read();

                        // Verifica se caractere valido.
                        if (eCaractere(_char))
                        {
                            // Formando token hexadecimal 0x
                            if (_char == 'x')
                            {
                                lexema += _char;
                                estadoAtual = 7;
                            }
                            else if(eDigito(_char))
                            {// Formação de numero.
                                lexema += _char;
                                estadoAtual = 9;
                            }
                            else
                            {// valor Zero.
                                devolve = true;
                                estadoAtual = estadoFinal;
                            }
                        }
                        else
                        {// Caractere Invalido.
                            Erro.ErroLexico.Char(linha);
                        }

                        break;

                    case 7:

                        _char = (char)arquivo.Read();

                        // Verifica se caractere valido.
                        if (eCaractere(_char))
                        {
                            // Formação token hexadecimal 0xD
                            if (eHexadecimal(_char))
                            {
                                lexema += _char;
                                estadoAtual = 8;
                            }
                            else if (_char == -1 || _char == 65535)
                            {// Fim do arquivo não esperado.
                                Erro.ErroLexico.Arquivo(linha);
                            }
                            else
                            {// Caso seja outro caractere não permitido, logo não consegue identificar o lexema.
                                Erro.ErroLexico.Lexema(linha, lexema);
                            }
                        }
                        else
                        {// Caractere Invalido.
                            Erro.ErroLexico.Char(linha);
                        }

                        break;

                    case 8:

                        _char = (char)arquivo.Read();

                        // Verifica se caractere valido.
                        if (eCaractere(_char))
                        {
                            // token hexadecimal 0xDD
                            if (eHexadecimal(_char))
                            {
                                lexema += _char;
                                estadoAtual = estadoFinal;
                            }
                            else if (_char == -1 || _char == 65535)
                            {// Fim do arquivo não esperado.
                                Erro.ErroLexico.Arquivo(linha);
                            }
                            else
                            {// Caso seja outro caractere não permitido, logo não consegue identificar o lexema.
                                Erro.ErroLexico.Lexema(linha, lexema);
                            }
                        }
                        else
                        {// Caractere Invalido.
                            Erro.ErroLexico.Char(linha);
                        }

                        break;

                    case 9:

                        _char = (char)arquivo.Read();

                        // Verifica se caractere valido.
                        if (eCaractere(_char))
                        {
                            // "Loop" formação valor
                            if (eDigito(_char))
                            {
                                lexema += _char;
                                estadoAtual = 9;
                            }
                            else
                            {// const valor
                                devolve = true;
                                estadoAtual = estadoFinal;
                            }
                        }
                        else
                        {// Caractere Invalido.
                            Erro.ErroLexico.Char(linha);
                        }

                        break;

                    case 10:

                        _char = (char)arquivo.Read();

                        // Verifica se caractere valido.
                        if (eCaractere(_char))
                        {
                            // inicio comentario
                            if (_char == '*')
                            {
                                lexema += _char;
                                estadoAtual = 11;                          
                            }
                            else
                            {// token /
                                devolve = true;
                                estadoAtual = estadoFinal;
                            }
                        }
                        else
                        {// Caractere Invalido.
                            Erro.ErroLexico.Char(linha);
                        }

                        break;

                    case 11:

                        _char = (char)arquivo.Read();

                        // Verifica se caractere valido.
                        if (eCaractere(_char))
                        {
                            // Caminho finalizar comentario
                            if (_char == '*')
                            {
                                lexema += _char;
                                estadoAtual = 12;
                            }
                            else if (_char == -1 || _char == 65535)
                            {// Fim do arquivo não esperado.
                                Erro.ErroLexico.Arquivo(linha);
                            }
                            else
                            {// "Loop" ignorando carcteres
                                lexema += _char;
                                estadoAtual = 11;
                            }
                        }
                        else
                        {// Caractere Invalido.
                            Erro.ErroLexico.Char(linha);
                        }

                        break;

                    case 12:

                        _char = (char)arquivo.Read();

                        // Verifica se caractere valido.
                        if (eCaractere(_char))
                        {
                            // Caminho finalizar comentario
                            if (_char == '*')
                            {
                                lexema += _char;
                                estadoAtual = 12;
                            }
                            else if (_char == '/')
                            {// Finaliza comentario
                                lexema += _char;
                                estadoAtual = 0;
                            }
                            else if (_char == -1 || _char == 65535)
                            {// Fim do arquivo não esperado.
                                Erro.ErroLexico.Arquivo(linha);
                            }
                            else
                            {// Voltando pro estado 11 para continuação comentario
                                lexema += _char;
                                estadoAtual = 11;
                            }
                        }
                        else
                        {// Caractere Invalido.
                            Erro.ErroLexico.Char(linha);
                        }

                        break;

                    case 13:

                        _char = (char)arquivo.Read();

                        // Verifica se caractere valido.
                        if (eCaractere(_char))
                        {
                            // String 
                            if (_char == '"')
                            {
                                lexema += _char;
                                estadoAtual = estadoFinal;
                            }else if(_char == 10 || _char == '$')
                            {// não pode conter quebra de linha e $
                                Erro.ErroLexico.Lexema(linha, lexema);
                            }
                            else if (_char == -1 || _char == 65535)
                            {// Fim do arquivo não esperado.
                                Erro.ErroLexico.Arquivo(linha);
                            }
                            else
                            {
                                lexema += _char;
                                estadoAtual = 13;
                            }
                        }
                        else
                        {// Caractere Invalido.
                            Erro.ErroLexico.Char(linha);
                        }

                        break;

                    case 14:

                        _char = (char)arquivo.Read();

                        // Verifica se caractere valido.
                        if (eCaractereImprimivel(_char))
                        {
                            lexema += _char;
                            estadoAtual = 15;
                        }
                        else if (_char == -1 || _char == 65535)
                        {// Fim do arquivo não esperado.
                            Erro.ErroLexico.Arquivo(linha);
                        }
                        else
                        {// Caractere Invalido(não pertence a linguagem) ou não imprivel.
                            Erro.ErroLexico.Char(linha);
                        }

                        break;

                    case 15:

                        _char = (char)arquivo.Read();

                        // Verifica se caractere valido.
                        if (eCaractere(_char))
                        {
                            //token char
                            if (_char == 39)
                            {
                                lexema += _char;
                                estadoAtual = estadoFinal;
                            }               
                            else if (_char == -1 || _char == 65535)
                            {// Fim do arquivo não esperado.
                                Erro.ErroLexico.Arquivo(linha);
                            }
                            else
                            {// Erro ao formar o  char 'c ?(alguma coisa)
                                Erro.ErroLexico.Lexema(linha,lexema);
                            }
                        }
                        else
                        {// Caractere Invalido(não pertence a linguagem) ou não imprivel.
                            Erro.ErroLexico.Char(linha);
                        }

                        break;
                }

            }

            if (!EOF)// não for final do arquivo.
            {
                if (tbSimbolos.buscarSimbolo(lexema.ToLower()) != null)
                {// Caso lexema já exista na tabela de simbolos
                    simbolo = tbSimbolos.buscarSimbolo(lexema.ToLower());
                }
                else if (lexema[0] == '_' || lexema[0] == '.' || eLetra(lexema[0]))
                {// adicionar identificador na tabela
                    if(lexema.Length >= 255)
                    {// validando tamanho identificador
                        Erro.ErroLexico.Lexema(linha,lexema);
                    }
                    else
                    {//inserindo na tabela
                        simbolo = tbSimbolos.inserirIdentificador(lexema.ToLower());
                    }
                }
                else if (lexema[0] == '"')
                {// identificando string 
                    if(lexema.Length >= 256)
                    {// validando tamanho string
                        Erro.ErroLexico.Lexema(linha,lexema);
                    }
                    else
                    {
                        lexema = '"'+lexema.Substring(1, lexema.Length - 2) + "$"+'"';
                    }

                    simbolo = new Simbolos(lexema, TabelaSimbolos.CONSTANTE);
                }
                else
                {   //const hexadecimal ou alfanumérico('c') ou inteiro
                    if (lexema.Length > 1 && lexema[1] == 'x' || lexema[0] == 39  || (-32768 <= int.Parse(lexema) && int.Parse(lexema) <= 32767))
                    {
                        simbolo = new Simbolos(lexema, TabelaSimbolos.CONSTANTE);
                    }
                    else
                    { // ACHO QUE FALTA A VERIFICAÇÃO DO VETOR 4BYTES
                        simbolo = new Simbolos(lexema, 111);
                    }
                }
                
            }
            else
            {// retonar eof para futuras validações. 
                simbolo = new Simbolos("EOF", TabelaSimbolos.EOF);
            }

            return simbolo;

        }


        /// <summary>
        /// Testa se o caractere é um dígito decimal.
        /// </summary> 
        /// <param name="c">Caractere a ser testado</param>
        /// <returns>true se for um dígito decimal e falso caso contrário.</returns>
        private static bool eDigito(char c)
        {
            return '0' <= c && c <= '9';
        }

        /// <summary>
        /// Testa se o caractere é uma Letra
        /// </summary>
        /// <param name="c">Caractere a ser testado</param>
        /// <returns>true se for uma letra e false caso contrário</returns>
        private static bool eLetra(char c)
        {
            return ('a' <= c && c <= 'z') || ('A' <= c && c <= 'Z');
        }

        /// <summary>
        /// Testa se o caractere é um dígito hexadecimal.
        /// </summary>
        /// <param name="c">Caractere a ser testado</param>
        /// <returns>true se for um dígito hexadecimal e false caso contrário</returns>
        private static bool eHexadecimal(char c)
        {
            return ('0' <= c && c <= '9') || ('A' <= c && c <= 'F');
        }

        /// <summary>
        /// Testa se o caractere é um caractere permitido no arquivo.
        /// </summary>
        /// <param name="c">Caractere a ser testado</param>
        /// <returns>true se for um caractere permitido e false caso contrário.</returns>
        private static bool eCaractere(char c)
        {
            // 8 - apagar (\b)
            // 9 - tab horizontal (\t)
            // 10 - Move o cursor para a próxima linha da tela 0Ah (\n)
            // 11 - tab vertical (\v)
            // 13 - Move o cursor para o início da linha atual 0Dh (\r)
            // 32 - spaço 
            // 33 - !   |   34 - "  |   37 - %  |   38 - &  |   39 - '  |   40 - (  |   41 - )  |   42 - *  |   43 - +  |   44 - ,  |   45 - -  |   46 - .   |   47 - /   | -1 e 65535 - EOF
            // 58 - :   |   59 - ;  |   60 - <  |   61 - =  |   62 - >  |   63 - ?  |   64 - @  |   91 - [  |   93 - ]  |   94 - ^  |   95 - _  |   123 - {  |   124 - |  | 125 - }
            return eLetra(c) || eDigito(c) || (8 <= c && c <= 11) || c == 13 || (32 <= c && c <= 34) || (37 <= c && c <= 47) || (58 <= c && c <= 64) || c == 91 || (93 <= c && c <= 95) || (123 <= c && c <= 125) || c == 65535 || c == -1;
        }

        private static bool eCaractereImprimivel(char c)
        {
            return eLetra(c) || eDigito(c) || c == 33 || c == 34 || (37 <= c && c <= 47) || (58 <= c && c <= 64) || c == 91 || (93 <= c && c <= 95) || (123 <= c && c <= 125);
        }

    }
}
