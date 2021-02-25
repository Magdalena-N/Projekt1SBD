using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Projekt1
{
    class Sorter
    {
        public int readings = 0;
        public int writings = 0;

        public void Sorting(string fileName, bool showFileAfterPhase)
        {
            
            // tasma z danymi wejsciowymi
            Tape input = new Tape("./../../../data/" + fileName);

            // rozdzielenie danych seriami na dwie tasmy
            Tape outputA = new Tape("./../../../data/A.bin","write");
            Tape outputB = new Tape("./../../../data/B.bin","write");

            RewriteInput(input, outputA, outputB); // A i B tasmy ze wstepnie podzielonym wejsciem

            Tape inputA = new Tape("./../../../data/A.bin");
            Tape inputB = new Tape("./../../../data/B.bin");
            Console.WriteLine("\nFile after distribute to tape A and B:\n");
            Console.WriteLine("A:");
            Reader.ShowFile(inputA);
            Console.WriteLine("B:");
            Reader.ShowFile(inputB);

            int phasesCounter = 0;
            int change = 0;
            string inputFileA, inputFileB, outputFileC, outputFileD;
            inputFileA = "A.bin";
            inputFileB = "B.bin";
            outputFileC = "C.bin";
            outputFileD = "D.bin";
            while(true)
            {
                
                if(!DistributingMerging(inputFileA, inputFileB, outputFileC, outputFileD))
                {
                    break;
                }
                phasesCounter++;
                if (change == 0)
                {
                    inputFileA = "C.bin";
                    inputFileB = "D.bin";
                    outputFileC = "A.bin";
                    outputFileD = "B.bin";
                    change = 1;
                }
                else
                {
                    inputFileA = "A.bin";
                    inputFileB = "B.bin";
                    outputFileC = "C.bin";
                    outputFileD = "D.bin";
                    change = 0;
                }
                if (showFileAfterPhase)
                {
                    Tape fileC = new Tape("./../../../data/" + inputFileA);
                    Tape fileD = new Tape("./../../../data/" + inputFileB);

                    Console.WriteLine($"\nTapes after {phasesCounter} phase\n");
                    Console.WriteLine("Tape 1:");
                    Reader.ShowFile(fileC);
                    Console.WriteLine("Tape 2:");
                    Reader.ShowFile(fileD);
                }
                
            }

            RewriteTape(inputFileA);

            Tape inputC = new Tape("./../../../data/sorted_" + inputFileA);

            Console.WriteLine("\nFile after sorting\n");
            Reader.ShowFile(inputC);

            Console.WriteLine($"\nNumber of phases: {phasesCounter}");
            Console.WriteLine($"\nNumber of readings: {readings}");
            Console.WriteLine($"Number of writings: {writings}");

            
        }

        private void RewriteTape(string inputFileA)
        {
            Tape input = new Tape("./../../../data/" + inputFileA);
            Tape output = new Tape("./../../../data/sorted_" + inputFileA, "write");
            Record record = input.ReadRecord();
            while (record != null)
            {
                output.WriteRecord(record);
                record = input.ReadRecord();
            }
            output.Close();

        }

        private void RewriteInput(Tape input, Tape outputA, Tape outputB)
        {
            int whichTape = 0;  //0 dla tasmy A 1 dla tasmy B
            Console.WriteLine();

            int previous = -1;
            Record record = input.ReadRecord();
            // przepisywanie calego pliku input
            while (record != null)
            {
                //przepisywanie na jedna tasme dopóki uklad jest rosnacy
                
                if (previous == -1)
                {
                    previous = record.divisors;
                }
                if (record.divisors < previous)
                {
                    if (whichTape == 0)
                    {
                        whichTape = 1;
                    }
                    else
                    {
                        whichTape = 0;
                    }
                }

                if (whichTape == 0)
                {
                    outputA.WriteRecord(record);
                }
                else
                {
                    outputB.WriteRecord(record);
                }
                previous = record.divisors;
                record = input.ReadRecord();

            }
            outputA.Close();
            outputB.Close();

            readings += input.numberOfReadings;
            writings += outputA.numberOfWritings;
            writings += outputB.numberOfWritings;
        }

        private bool DistributingMerging(string inputFileA, string inputFileB,string outputFileC,string outputFileD)
        {
            /* Funkcja scala serie z dwóch taśm i dystrybuuje je naprzemiennie na jedną z taśm wyjściowych
             * zwraca true, jesli na drugiej taśmie wyjsciowej pozostaly serie
             * zwraca false, jesli druga taśma jest pusta
             */
            //tworzenie czterech tasm dla sortowania 2+2
            Tape inputA = new Tape("./../../../data/" + inputFileA);
            Tape inputB = new Tape("./../../../data/" + inputFileB);
            Tape outputC = new Tape("./../../../data/" + outputFileC, "write");
            Tape outputD = new Tape("./../../../data/" + outputFileD, "write");


           
            Record recordB = inputB.ReadRecord();
            if(recordB == null)
            {
                readings += inputB.numberOfReadings;
                return false;
            }
            Record recordA = inputA.ReadRecord();
            int output = 0; //0 dla C i 1 dla D

            int previousA = recordA.divisors;
            int previousB = recordB.divisors;

            while(true)
            {
                //jezeli tasmy nie sa puste
                if(recordA != null && recordB != null)
                {
                    //sprawdzenie czy seria A sie skonczyla
                    if (recordA.divisors < previousA)
                    {
                        //przepisujemy reszte serii B
                        while (recordB != null && recordB.divisors >= previousB) 
                        {
                            if(output == 0)
                            {
                                outputC.WriteRecord(recordB);
                            }
                            else
                            {
                                outputD.WriteRecord(recordB);
                            }
                            previousB = recordB.divisors;
                            recordB = inputB.ReadRecord();
                        }
                        previousA = -1;
                        previousB = -1;
                        output = output == 0 ? 1 : 0;
                    }
                    //sprawdzamy czy seria B sie skonczyla
                    else if(recordB.divisors< previousB)
                    {
                        //przepisujemy reszte serii A
                        while (recordA != null && recordA.divisors >= previousA)
                        {
                            if (output == 0)
                            {
                                outputC.WriteRecord(recordA);
                            }
                            else
                            {
                                outputD.WriteRecord(recordA);
                            }
                            previousA = recordA.divisors;
                            recordA = inputA.ReadRecord();
                        }
                        previousA = -1;
                        previousB = -1;
                        output = output == 0 ? 1 : 0;
                    }
                    // zadna seria sie nie skonczyla
                    else
                    {
                        //porownujemy ktory rekord mniejszy
                        if (recordA.divisors < recordB.divisors)
                        {
                            if(output == 0)
                            {
                                outputC.WriteRecord(recordA);
                            }
                            else
                            {
                                outputD.WriteRecord(recordA);
                            }
                            previousA = recordA.divisors;
                            recordA = inputA.ReadRecord();
                        }
                        else
                        {
                            if (output == 0)
                            {
                                outputC.WriteRecord(recordB);
                            }
                            else
                            {
                                outputD.WriteRecord(recordB);
                            }
                            previousB = recordB.divisors;
                            recordB = inputB.ReadRecord();
                        }
                    }
                }
                else
                {
                    //jezeli A pusta to przepisz B naprzemiennie na C i D
                    if(recordA == null)
                    {
                        //jak seria sie skonczy to zmieniamy tasme wyjsciowa
                        while(recordB != null)
                        {
                            if (recordB.divisors < previousB)
                            {
                                output = output == 0 ? 1 : 0;
                            }
                            if (output == 0)
                            {
                                outputC.WriteRecord(recordB);
                            }
                            else
                            {
                                outputD.WriteRecord(recordB);
                            }
                            previousB = recordB.divisors;
                            recordB = inputB.ReadRecord();
                        }
                        break;
                    }

                    //jezeli B pusta to przepisz A naprzemiennie na C i D
                    else
                    {
                        while(recordA != null)
                        {
                            if(recordA.divisors < previousA)
                            {
                                output = output == 0 ? 1 : 0;
                            }
                            if (output == 0)
                            {
                                outputC.WriteRecord(recordA);
                            }
                            else
                            {
                                outputD.WriteRecord(recordA);
                            }
                            previousA = recordA.divisors;
                            recordA = inputA.ReadRecord();
                        }
                        break;
                    }
                }



            }
            outputC.Close();
            outputD.Close();
            readings += inputA.numberOfReadings;
            readings += inputB.numberOfReadings;
            writings += outputC.numberOfWritings;
            writings += outputD.numberOfWritings;
            return true;
        }
       
    }
}
