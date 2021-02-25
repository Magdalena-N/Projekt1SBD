using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Projekt1
{
    class Tape
    {
        private const int PAGESIZE = 300;
        private const int RECORDSIZE = 20; // 5 * int(4bytes) = 20 bytes
        private const int BUFFERSIZE = PAGESIZE / RECORDSIZE;

        private string filePath;
        private int outIndex;
        private int inIndex;
        private int filePosition;
        public bool endOfFile;
        public bool emptyBuffer;
        private int recordsInInputBuffer;
        public int numberOfReadings;
        public int numberOfWritings;

        private Record[] inputBuffer = new Record[BUFFERSIZE];
        private Record[] outputBuffer = new Record[BUFFERSIZE];

        private byte[] binaryBuffer = new byte[PAGESIZE];

        public Tape(string _filePath, string mode = "read")
        {

            filePath = _filePath;
            outIndex = 0;
            inIndex = BUFFERSIZE;
            filePosition = 0;
            endOfFile = false;
            recordsInInputBuffer = 0;
            emptyBuffer = false;
            numberOfReadings = 0;
            numberOfWritings = 0;
            if (mode.Equals("write"))
            {
                File.WriteAllText(filePath, string.Empty);  // czyszczenie pliku wyjsciowego
            }

        }
        public void Close()
        {
            //jesli jest cos jeszcze w buforze trzeba to zapisac do pliku
            if (outIndex > 0)
            {
                WriteBlockOfBytes();
            }
            filePosition = 0;
        }
        
        public bool ReadBlockOfBytes()
        {
            /* Odczytuje blok bajtow z pliku wejsciowego do bufora
             * jest wywolywana w momencie, gdy chcemy odczytać rekord
             * z bufora, a bufor jest pusty
             * zwraca true jesli bufor pozostal pusty
             * zwraca false jesli bufor nie jest juz pusty
             */

            numberOfReadings++; //zwiększamy liczbe odczytów z dysku 
            recordsInInputBuffer = 0;
            
            
            using (FileStream aFile = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                if (filePosition == aFile.Length)
                {
                    endOfFile = true;
                    return true;
                }

                using (BinaryReader reader = new BinaryReader(aFile))
                {
                    reader.BaseStream.Seek(filePosition, SeekOrigin.Begin); // ustawiamy sie na odpowiednim miejscu w pliku
                    int readBytes = reader.Read(binaryBuffer);  // w readBytes mamy liczbe odczytanych bajtow
                    // sprawdzamy czy udalo sie odczytac cala strone dyskowa, czy plik zostal caly odczytany
                    if(readBytes != PAGESIZE)
                    {
                        endOfFile = true;
                    }
                    recordsInInputBuffer = readBytes / RECORDSIZE;
                    filePosition += readBytes;
                    // konwersja odczytanych bajtów na bufor rekordów
                    for (int j = 0, k = 0; j < readBytes; j += RECORDSIZE, k++) 
                    {
                        int[] values = new int[Record.SIZE];
                        for (int i = 0; i < Record.SIZE; i++)
                        {
                            values[i] = BitConverter.ToInt32(binaryBuffer, 4 * i + j);
                        }
                        Record record = new Record(values);
                        inputBuffer[k] = record;
                    }
                    
                    if (recordsInInputBuffer != 0)
                    {
                        return false;   // bufor nie jest juz pusty
                    }
                }

            }
            return false;   // bufor nie jest juz pusty

        }
        public Record ReadRecord()
        {
            /* Pozwala odczytac rekord z bufora */

            if (inIndex == BUFFERSIZE)   // bufor odczytu caly odczytany, trzeba pobrac z pliku dyskowego strone dyskowa
            {
                emptyBuffer = ReadBlockOfBytes();
                inIndex = 0;
            }
            if (emptyBuffer && endOfFile)
            {
                return null;
            }

            Record record = inputBuffer[inIndex++];
            if (recordsInInputBuffer - inIndex == 0)
            {
                emptyBuffer = true;
            }
            return record;
        }
        public void WriteRecord(Record record)
        {
            /* Zapisuje rekord do bufora zapisu, 
             * gdy bufor jest pełny to zapisuje do pliku dyskowego 
             */
            if (outIndex == BUFFERSIZE)
            {
                WriteBlockOfBytes();    //zapisuje strone dyskowa
                outIndex = 0;   //wpisywanie do bufora zaczynamy od poczatku
            }

            outputBuffer[outIndex++] = record;
        }
        
        public void WriteBlockOfBytes()
        {
            
            /* Zapisuje strone dyskowa w pliku dyskowym */

            numberOfWritings++;

            using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.OpenOrCreate)))
            {
                writer.Seek(filePosition, SeekOrigin.Begin);    // ustawiamy sie na odpowiednim miejscu w pliku
                byte[] number = new byte[sizeof(int)];
                int value;
                // konwersja rekordow na bajty
                for (int j = 0; j < outIndex; j++)
                {
                    for (int i = 0, k = 0; i < Record.SIZE; i++, k++) 
                    {
                        value = outputBuffer[j].numbers[i];
                        number = BitConverter.GetBytes(value);
                        binaryBuffer[4 * k + RECORDSIZE * j] = number[0];
                        binaryBuffer[4 * k + RECORDSIZE * j + 1] = number[1];
                        binaryBuffer[4 * k + RECORDSIZE * j + 2] = number[2];
                        binaryBuffer[4 * k + RECORDSIZE * j + 3] = number[3];
                    }
                    
                }
                writer.Write(binaryBuffer, 0, outIndex * RECORDSIZE);   // zapisanie strony dyskowej do pliku
                filePosition += outIndex * RECORDSIZE;
            }
        }
    }
}
