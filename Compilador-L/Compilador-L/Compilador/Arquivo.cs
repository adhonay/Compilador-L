/*
 * Pontifícia Universidade Católica de Minas Gerais
 * Compilador
 * Autores: Adhonay Júnior, Izabela Costa
 * Matricula: 504656, 498535
 **/
using System.IO;
using System;
using System.Collections.Generic;

public class Arquivo
{

    public List<String> buffer;

    public Arquivo()
    {
        buffer = new List<String>();
    }

    public void print(String file)
    {
        try
        {
            StreamWriter _file = new StreamWriter(file);

            foreach (String s in buffer)
            {
                _file.Write(s);
                _file.Write("\n");
            }

            _file.Close();

        }
        catch (Exception e) { }
    }

    public void add(String str)
    {
        buffer.Add(str);
    }
}
