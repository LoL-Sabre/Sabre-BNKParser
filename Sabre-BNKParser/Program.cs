using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabre_BNKParser
{
    class Program
    {
        static void Main(string[] args)
        {
            bool extract = false;
            bool isChampionBank = false;
            bool isEventBank = false;
            bool isAudioBank = false;
            Console.WriteLine("Example: Lucian_Skin06_SFX_audio.bnk -E -CHAMPBANK -AUDIOBANK      -E - extract, -CHAMPBANK - Champion bank, -EVENTBANK");
            string input = Console.ReadLine();
            string[] inp = input.Split(' ');
            if(inp.Length == 1)
            {
                inp = new string[] {inp[0], "", "", "" };
            }
            if(inp.Length == 2)
            {
                inp = new string[] {inp[0], inp[1], "", "" };
            }
            if(inp.Length == 3)
            {
                inp = new string[] {inp[0], inp[1], inp[2], "" };
            }
            if(inp[1] == "-E" || inp[2] == "-E" || inp[3] == "-E")
            {
                extract = true;
            }
            if(inp[1] == "-CHAMPBANK" || inp[2] == "-CHAMPBANK" || inp[3] == "-CHAMPBANK")
            {
                isChampionBank = true;
            }
            if(inp[1] == "-AUDIOBANK" || inp[2] == "-AUDIOBANK" || inp[3] == "-AUDIOBANK")
            {
                isAudioBank = true;
            }
            if(inp[1] == "-EVENTBANK" || inp[2] == "-EVENTBANK" || inp[3] == "-EVENTBANK")
            {
                isEventBank = true;
            }
            if(isEventBank == true)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Event banks not supported");
                Console.ResetColor();
                Console.ReadLine();
            }
            else
            {
                BNKFile b = new BNKFile(inp[0], isAudioBank, isEventBank, isChampionBank, extract);
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("------BKHD------");
                Console.ResetColor();
                Console.WriteLine("Magic = " + b.bkhd.Magic);
                Console.WriteLine("Length = " + b.bkhd.LengthOfSection);
                Console.WriteLine("ID = " + b.bkhd.ID);

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("------DIDX------");
                Console.ResetColor();
                Console.WriteLine("Magic = " + b.didx.Magic);
                Console.WriteLine("Length = " + b.didx.Length);
                Console.WriteLine("WEMCount = " + b.didx.WEMCount);
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("------EMBEDDED WEM FILES------");
                Console.ResetColor();
                foreach(var f in b.didx.Files)
                {
                    Console.WriteLine();
                    Console.WriteLine("ID = " + f.ID);
                    Console.WriteLine("Offset from DATA section = " + f.OffsetDataSection);
                    Console.WriteLine("Data Length = " + f.DataLength);
                }

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("------DATA------");
                Console.ResetColor();
                Console.WriteLine("Magic = " + b.data.Magic);
                Console.WriteLine("Length = " + b.data.Length);
                Console.WriteLine();
                Console.ReadLine();
            }
        }
        public static long CalculateOffsetBNK(uint NumberOfEmbeddedSoundbanks)
        {
            long offset = 56;
            offset += NumberOfEmbeddedSoundbanks * 12;
            return offset;
        }   
    }
}
