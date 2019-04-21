
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

        public Simbolos(string lexema, byte token)
        {
            this.lexema = lexema;
            this.token = token;
            this.endereco = -1;

        }

        public string toString()
        {
            return "Simbolo inserido {" + "lexema: " + lexema + ", token: " + token + ", endereco: " + endereco +  '}';
        }



    }



}
