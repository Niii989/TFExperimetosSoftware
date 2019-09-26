using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {            
            try
            {//Pass the filepath and filename to the StreamWriter Constructor
                //Nombre del archivo
                string filename = "C:\\downloads";      //Tiene que existir la ruta
                string fullPath = Path.Combine(filename, "Prueba.txt");

                StreamWriter sw = File.CreateText(fullPath);
                                
                //Write a line of text
                sw.WriteLine("Hello World!!");
                //Write a second line of text
                sw.WriteLine("From the StreamWriter class");
                //Close the file
                sw.Close();

            } catch(Exception e){
                Console.WriteLine("Exception: " + e.Message);
            } finally {
                Console.WriteLine("Executing finally block.");
            }






            }
    }
}
