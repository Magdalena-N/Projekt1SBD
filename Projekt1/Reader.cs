using System;
using System.Collections.Generic;
using System.Text;

namespace Projekt1
{
    class Reader
    {
        public static void ShowFile(Tape tape)
        {
            Console.WriteLine(String.Format("{0,3}|{1,3}|{2,3}|{3,3}|{4,3}|{5,8}", "1", "2", "3", "4", "5", "Divisors"));
            Console.WriteLine("----------------------------");
            //endOfFile jest true kiedy wszystkie rekordy zostaly przeczytane
            //while (tape.endOfFile == false || tape.emptyBuffer == false)
            Record record = tape.ReadRecord();
            while (record != null)
            {

                Console.WriteLine(String.Format("{0,3}|{1,3}|{2,3}|{3,3}|{4,3}|{5,8}", record.numbers[0], record.numbers[1], record.numbers[2],
                    record.numbers[3], record.numbers[4], record.divisors));

                record = tape.ReadRecord();
            }


        }
    }
}
