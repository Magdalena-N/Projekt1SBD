using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Projekt1
{
    class Generator
    {
         
        Random random = new Random();
        Tape tape;
        public Generator(string filePath)
        {
            tape = new Tape(filePath,"write");
            
        }
        public void Generate()
        {
            Console.WriteLine("How many records would you like to randomly generate?");
            int numberOfRecords = Convert.ToInt32(Console.ReadLine());

            
            for (int i = 0; i < numberOfRecords; i++) 
            {
                int[] values = new int[Record.SIZE];
                for (int j = 0; j < Record.SIZE; j++) 
                {
                    values[j] = random.Next() % 73;
                }
                
                Record record = new Record(values);
                
                tape.WriteRecord(record);
            }
            tape.Close();
            

        }

    }
}
