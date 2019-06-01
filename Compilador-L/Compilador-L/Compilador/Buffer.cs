using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Pontifícia Universidade Católica de Minas Gerais
 * Compilador
 * Autores: Adhonay Júnior, Izabela Costa
 * Matricula: 504656, 498535
 **/
using System;
using System.Collections;

public class Buffer
{

    public ArrayList<String> buffer;

    public Buffer()
    {
        buffer = new ArrayList<>();
    }

    public void print(String file)
    {
        try
        {
            BufferedWriter _file = new BufferedWriter(new FileWriter(file));

            for (String s : buffer)
            {
                _file.write(s);
                _file.newLine();
            }

            _file.close();

        }
        catch (Exception e) { }
    }

    public void add(String str)
    {
        buffer.add(str);
    }
}
