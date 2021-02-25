using System;
using System.Collections.Generic;
using System.Text;

namespace Projekt1
{
    class Maker
    {
        Tape tape;

        public Maker(string filePath)
        {
            tape = new Tape(filePath, "write");
        }
        public void Make()
        {
            Console.WriteLine("How many records would you like to enter?");
            int numberOfRecords = Convert.ToInt32(Console.ReadLine());


            for (int i = 0; i < numberOfRecords; i++)
            {
                int[] values = new int[Record.SIZE];
                for (int j = 0; j < Record.SIZE; j++)
                {
                    values[j] = Convert.ToInt32(Console.ReadLine());
                }

                Record record = new Record(values);
                
                tape.WriteRecord(record);
            }
            tape.Close();   // przepisanie reszty bufora do pliku
            

        }
    }
}
