/*
 * Pontifícia Universidade Católica de Minas Gerais
 * Compilador
 * Autores: Adhonay Júnior, Izabela Costa
 * Matricula: 504656, 498535
 **/
using System.IO;
using System;
using System.Collections.Generic;

public class Memoria
{

    public static int memoria {get; set;}
    public static int temporario {get; set;}

    public Memoria()
    {
        memoria = 16384;//4000H
        temporario = 0;
    }

    public void resetarTemporario()
    {
        temporario = 0;
    }

    public int alocarInteiro()
    {
        int address = memoria;
        memoria += 2;
        return address;
    }

    public int alocarCaractere()
    {
        int address = memoria;
        memoria += 1;
        return address;
    }

    public int alocarString()
    {
        int address = memoria;
        memoria += 256;
        return address;
    }


    public int alocarLogico()
    {
        int address = memoria;
        memoria += 1;
        return address;
    }

   
    public int alocarString(int size)
    {
        int address = memoria;
        memoria += size;
        return address;
    }

    public int alocarTemporarioyCaractere()
    {
        int address = temporario;
        temporario += 1;
        return address;
    }

    public int alocarTemporarioInteiro()
    {
        int address = temporario;
        temporario += 2;
        return address;
    }

    public int alocarTemporarioLogico()
    {
        int address = temporario;
        temporario += 1;
        return address;
    }

    public int alocarTemporarioString()
    {
        int address = temporario;
        temporario += 256;
        return address;
    }

    public int alocarTemporarioString(int tamanho)
    {
        int address = temporario;
        temporario += tamanho;
        return address;
    }

    public int allocateTemporaryBuffer()
    {
        int address = temporario;
        temporario += 259;
        return address;
    }
}
