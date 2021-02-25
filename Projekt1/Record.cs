using System;
using System.Collections.Generic;
using System.Text;

namespace Projekt1
{
    class Record
    {
        public const int SIZE = 5; 
        public int[] numbers = new int[SIZE];
        public int divisors = 0;

        
        public Record(int[] num)
        {
            numbers = num;
            divisors = ComputeDivisors();
           
        }
        public int ComputeDivisors()
        {
            int divisors = 0;
            long product = 1;
            foreach (int number in numbers)
            {
                product *= number;
            }
            if (product == 0)
            {
                divisors = 0;
            }
            else
            {
                for (long i = 1; i * i <= product; i++)
                {
                    if (product % i == 0)
                    {
                        if (product / i != i)
                        {
                            divisors += 2;
                        }
                        else
                        {
                            divisors++;
                        }

                    }
                }
            }

            return divisors;
        }
    }
}
