using System;
using System.Dynamic;
using System.IO;

namespace Projekt1
{
    class Program
    {
        static void Main(string[] args)
        {
            int option;
            string fileName = "input.bin";
            bool showFileAfterPhase = false;

            Console.WriteLine("Choose option:");
            Console.WriteLine("1. Generate records");
            Console.WriteLine("2. Write records");
            Console.WriteLine("3. Test file");
            option = Convert.ToInt32(Console.ReadLine());

            switch (option)
            {
                case 1:
                    // wygenerowanie pliku z losowymi rekordami
                    Generator generator = new Generator("./../../../data/" + fileName);
                    generator.Generate();
                    Console.WriteLine("Do you want see file after each phase? [yes/no]");
                    showFileAfterPhase = Console.ReadLine().Equals("yes") ? true : false;
                    Sort(fileName, showFileAfterPhase);
                    break;
                case 2:
                    // wprowadzanie rekordow z klawiatury
                    Console.WriteLine("Enter file name you want to create");
                    fileName = Console.ReadLine();
                    fileName = fileName + ".bin";
                    Maker maker = new Maker("./../../../data/" + fileName);
                    maker.Make();
                    Console.WriteLine("Do you want see file after each phase? [yes/no]");
                    showFileAfterPhase = Console.ReadLine().Equals("yes") ? true : false;
                    Sort(fileName, showFileAfterPhase);
                    break;
                case 3:
                    Console.WriteLine("Enter file name you want to load");
                    fileName = Console.ReadLine();
                    fileName = fileName + ".bin";
                    Console.WriteLine("Do you want see file after each phase? [yes/no]");
                    showFileAfterPhase = Console.ReadLine().Equals("yes") ? true : false;
                    Sort(fileName, showFileAfterPhase);
                    break;
                default:
                    Console.WriteLine("Wrong option!");
                    break;
            }
            
        }
        private static void Sort(string fileName, bool showFileAfterPhase)
        {
            Tape tape = new Tape("./../../../data/" + fileName);
            Console.WriteLine("\nFile before sorting\n");
            Reader.ShowFile(tape);

            // sortowanie
            Sorter sorter = new Sorter();
            sorter.Sorting(fileName, showFileAfterPhase);
        }
       
    }

}
