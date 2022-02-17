using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Gmad_Extractor
{
    class Program
    {
        struct FileEntry
        {
            public string strName;
            public long iSize;
            public uint iCRC;
            public long iOffset;
            public int iFileNumber;
            public string Description;
        }

        static List<FileEntry> entries = new List<FileEntry>();
        static uint m_fileblock = 0;
        static List<string> GMAFiles = new List<string>();

        static void AddFiles(string path)
        {
            FileAttributes attr = File.GetAttributes(path);
            if (attr.HasFlag(FileAttributes.Directory))
                foreach (var item in Directory.GetFiles(path))
                    GMAFiles.Add(item);
            else
                GMAFiles.Add(path);
        }
        static bool IOCheck(string[] args)
        {
            if (args.Count() > 0)
                AddFiles(args[0]);
            else
            {
                Console.Write("Path: ");
                AddFiles(Console.ReadLine());
            }
            return true;
        }
        static void Main(string[] args)
        {
            if (IOCheck(args))
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                foreach (var item in GMAFiles)
                {
                    string GMAFileName = Path.GetFileNameWithoutExtension(item);
                    if (File.Exists(item))
                    {
                        Console.WriteLine($"\nExtracting: {GMAFileName}");
                        Directory.CreateDirectory($@"{GMAFileName}");

                        byte[] file = File.ReadAllBytes($@"{item}");
                        ByteParser parser = new ByteParser(file);
                        string GMAD = string.Join("", new[] { parser.GetChar(), parser.GetChar(), parser.GetChar(), parser.GetChar() });
                        if (GMAD == "GMAD")
                        {
                            if (Parse(parser))
                            {
                                Console.WriteLine(entries[0].Description);
                                File.WriteAllText($@"{GMAFileName}\addon.json", entries[0].Description);
                                foreach (var entry in entries)
                                {
                                    string FileName = Path.GetFileName(entry.strName);
                                    string FilePath = Path.GetDirectoryName(entry.strName);
                                    Directory.CreateDirectory($@"{GMAFileName}\{FilePath}");
                                    Console.WriteLine($@"{GMAFileName}\{FilePath}\{FileName} - Size: {entry.iSize}");

                                    string fullpath = $@"{GMAFileName}\{FilePath}\{FileName}";
                                    using (var cancer = File.OpenWrite(fullpath))
                                        cancer.Write(file, (int)entry.iOffset + (int)m_fileblock, (int)entry.iSize);
                                }
                            }
                        }
                    }
                    else
                        Console.WriteLine("Item doesnt exist");
                }
                Console.WriteLine($"\n{stopwatch.Elapsed}");
                Console.WriteLine("Done.");
                stopwatch.Stop();
            }
        }
        static bool Parse(ByteParser parser)
        {
            try
            {
                entries.Clear();
                char fmt_version = parser.GetChar();

                if (fmt_version > 1)
                {
                    string strContent = parser.GetStringToTermination();
                    while (!string.IsNullOrEmpty(strContent))
                        strContent = parser.GetStringToTermination();
                }

                ulong steamid = parser.GetULong();
                ulong timestamp = parser.GetULong();

                string m_name = parser.GetStringToTermination();
                string m_desc = parser.GetStringToTermination();
                string m_author = parser.GetStringToTermination();

                long addon_version = parser.GetInt();

                int iFileNumber = 1;
                long iOffset = 0;

                while (parser.GetUInt() != 0)
                {
                    entries.Add(new FileEntry
                    {
                        strName = parser.GetStringToTermination(),
                        iSize = parser.GetLong(),
                        iCRC = parser.GetUInt(),
                        iOffset = iOffset,
                        iFileNumber = iFileNumber,
                        Description = m_desc
                    });
                    iOffset += entries[entries.Count - 1].iSize;
                    iFileNumber++;
                }
                m_fileblock = (uint)parser.CurrentPosition;
                return true;
            }
            catch { return false; }
        }
    }
}
