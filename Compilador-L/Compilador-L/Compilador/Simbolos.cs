
/*
 * Pontifícia Universidade Católica de Minas Gerais
 * Compilador
 * Autores: Adhonay Júnior, Izabela Costa
 * Matricula: 504656, 498535
 **/

namespace Compilador_L.Compilador
{
    /*
     * classe para armazenamento dos simbolos.Os simbolos tem as seguintes informações: 
     * token
     * endereco
     * lexemas (são retirados do código fonte)
    */
    public class Simbolos
    {
        public byte token { get; set; }
        public string lexema { get; set; }
        public int endereco { get; set; }
        public byte classe { get; set; }
        public byte tipo { get; set; }
        public int tamanho { get; set; }

        public static readonly byte SEM_CLASSE = 0;
        public static readonly byte CLASSE_VAR = 1;
        public static readonly byte CLASSE_CONST = 2;

        public static readonly byte SEM_TIPO = 3;
        public static readonly byte TIPO_INTEIRO = 4;
        public static readonly byte TIPO_CARACTERE = 5;
        //falta o professor responder para colocar possiveis tipos arranjo e string pra atribuir no lexico

        public static readonly int SEM_TAMANHO = -1;
        public static readonly int SEM_ENDERECO = -1;
        public Simbolos(string lexema, byte token)
        {
            this.lexema = lexema;
            this.token = token;
            this.endereco = SEM_ENDERECO;
            this.classe = SEM_CLASSE;
            this.tipo = SEM_TIPO;
            this.tamanho = SEM_TAMANHO;

        }
  
        public Simbolos(string lexema, byte token , byte tipo)
        {
            this.lexema = lexema;
            this.token = token;
            this.endereco = SEM_ENDERECO;
            this.classe = SEM_CLASSE;
            this.tipo = tipo;
            this.tamanho = SEM_TAMANHO;

        }

        public string toString()
        {
            return "Simbolo inserido {" + "lexema: " + lexema + ", token: " + token + ", endereco: " + endereco + ", classe: " + classe + ", tipo: " + tipo + ", tamanho: " + tamanho + '}';
        }



    }



}
