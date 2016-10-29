using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Sabre_BNKParser
{
    class BNKFile
    {
        public BinaryReader br;
        public BKHD bkhd;
        public DIDX didx;
        public DATA data;
        public string fileLoc;
        public BNKFile(string fileLocation, bool isAudioBank = false, bool isEventBank = false, bool isChampionBank = false, bool extractAudio = false)
        {
            fileLoc = fileLocation;
            br = new BinaryReader(File.Open(fileLocation, FileMode.Open));
            if(isAudioBank == true)
            {
                bkhd = new BKHD(br);
                if(isChampionBank == true)
                {
                    br.ReadBytes(12);
                }
                didx = new DIDX(br);
                data = new DATA(br, didx, bkhd);
                if(extractAudio == true)
                {
                    foreach(var a in didx.Files)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("Extracting " + a.ID.ToString() + ".wem");
                        DIDXEmbeddedWEMFile.ExportAudio(a.ID.ToString() + ".wem", a.Data, this);
                    }
                }
            }
            else if(isEventBank == true)
            {
                bkhd = new BKHD(br);
            }
        }
        public class BKHD //28
        {
            public string Magic;
            public UInt32 LengthOfSection;
            public UInt32 Version;
            public UInt32 ID;
            public UInt32[] Zero = new UInt32[3];
            public BKHD(BinaryReader br)
            {
                Magic = Encoding.ASCII.GetString(br.ReadBytes(4));
                LengthOfSection = br.ReadUInt32();
                Version = br.ReadUInt32();
                ID = br.ReadUInt32();
                for(int i = 0; i < 3; i++)
                {
                    Zero[i] = br.ReadUInt32();
                }
            }
        }
        public class DIDX // 12 + 12 * WEM
        {
            public string Magic;
            public UInt32 Length;
            public UInt32 WEMCount;
            public List<DIDXEmbeddedWEMFile> Files = new List<DIDXEmbeddedWEMFile>();
            public DIDX(BinaryReader br)
            {
                Magic = Encoding.ASCII.GetString(br.ReadBytes(4));
                Length = br.ReadUInt32();
                WEMCount = Length / 12;
                for(int i = 0; i < WEMCount; i++)
                {
                    Files.Add(new DIDXEmbeddedWEMFile(br));
                }
            }
        }
        public class DATA
        {
            public string Magic;
            public UInt32 Length;
            public DATA(BinaryReader br, DIDX didx, BKHD bkhd)
            {
                Magic = Encoding.ASCII.GetString(br.ReadBytes(4));
                Length = br.ReadUInt32();
                foreach(var f in didx.Files)
                {
                    br.BaseStream.Seek(CalculateOffsetBNK(didx.WEMCount) + f.OffsetDataSection, SeekOrigin.Begin);
                    f.Data = br.ReadBytes((int)f.DataLength);
                }
            }
        }
        public class HIRC
        {
            public string Magic;
            public UInt32 Length;
            public UInt32 NumberOfEvents;
            public HIRC(BinaryReader br)
            {

            }
        }
        public class STID
        {
            public string Magic;
            public UInt32 Length;
            public UInt32 ZeroOne;
            public UInt32 SoundbankCount;
            public List<STIDSoundbank> Soundbanks = new List<STIDSoundbank>();
            public STID(BinaryReader br)
            {
                Magic = Encoding.ASCII.GetString(br.ReadBytes(4));
                Length = br.ReadUInt32();
                ZeroOne = br.ReadUInt32();
                SoundbankCount = br.ReadUInt32();
                for(int i = 0; i < SoundbankCount; i++)
                {
                    Soundbanks.Add(new STIDSoundbank(br));
                }
            }
        }
        public class STMG
        {
            public STMG(BinaryReader br)
            {

            }
        }
        public class STIDSoundbank
        {
            public UInt32 ID;
            public byte LengthOfBank;
            public string Bank;
            public STIDSoundbank(BinaryReader br)
            {
                ID = br.ReadUInt32();
                LengthOfBank = br.ReadByte();
                Bank = Encoding.ASCII.GetString(br.ReadBytes(LengthOfBank));
            }
        }
        public class DIDXEmbeddedWEMFile
        {
            public UInt32 ID;
            public UInt32 OffsetDataSection;
            public UInt32 DataLength;
            public byte[] Data;
            public DIDXEmbeddedWEMFile(BinaryReader br)
            {
                ID = br.ReadUInt32();
                OffsetDataSection = br.ReadUInt32();
                DataLength = br.ReadUInt32();
            }
            public static void ExportAudio(string fileName, byte[] data, BNKFile bnk)
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + "/OUTPUT/" + "/" + bnk.fileLoc + "/");
                File.WriteAllBytes(Environment.CurrentDirectory + "/OUTPUT/" + "/" + bnk.fileLoc + "/" + fileName, data);
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
