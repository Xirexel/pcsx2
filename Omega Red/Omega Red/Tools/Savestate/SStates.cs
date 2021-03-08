using Omega_Red.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Omega_Red.Tools.Savestate
{
    class SStates
    {
        private SStates() { }
        
        private static SStates m_Instance = null;

        public static SStates Instance { get { if (m_Instance == null) m_Instance = new SStates(); return m_Instance; } }


        public SaveStateInfo readData(string a_file_path, int a_value, bool a_isAutosave = false, bool a_isIsQuicksave = false, int a_DecodePixelWidth = 100)
        {
            SaveStateInfo l_result = new SaveStateInfo();

            l_result.IsAutosave = a_isAutosave;

            l_result.IsQuicksave = a_isIsQuicksave;

            l_result.FilePath = a_file_path;

            l_result.Index = a_value;

            l_result.Visibility = System.Windows.Visibility.Visible;

            l_result.Item = null;
            
            using (FileStream zipToOpen = new FileStream(a_file_path, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
                {
                    var l_SavestateEntry = new SavestateEntry_InternalStructures();

                    ZipArchiveEntry l_StructuresEntry = archive.GetEntry(l_SavestateEntry.GetFilename());

                    if (l_StructuresEntry != null)
                    {
                        using (BinaryReader reader = new BinaryReader(l_StructuresEntry.Open()))
                        {
                            l_result.CheckSum = l_SavestateEntry.Parser(new MemLoadingState(reader));
                        }
                    }

                    var l_SavestateEntry_TimeSession = new SavestateEntry_TimeSession();

                    l_StructuresEntry = archive.GetEntry(l_SavestateEntry_TimeSession.GetFilename());

                    if (l_StructuresEntry != null)
                    {
                        using (BinaryReader reader = new BinaryReader(l_StructuresEntry.Open()))
                        {
                            var lTimeTulpe = l_SavestateEntry_TimeSession.Parser(new MemLoadingState(reader));

                            l_result.Date = lTimeTulpe.Item1.Replace(' ', '\n');

                            DateTime lDateTime = DateTime.Now;

                            if (DateTime.TryParseExact(lTimeTulpe.Item1, "dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out lDateTime))
                                l_result.DateTime = lDateTime;
                            else if (DateTime.TryParseExact(lTimeTulpe.Item1, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out lDateTime))
                                l_result.DateTime = lDateTime;
                            else
                                l_result.DateTime = DateTime.Now;

                            l_result.DurationNative = TimeSpan.FromSeconds(lTimeTulpe.Item2);

                            l_result.Duration = l_result.DurationNative.ToString(@"dd\.hh\:mm\:ss");
                        }
                    }

                    SavestateEntry_Screenshot l_SavestateEntry_Screenshot = new SavestateEntry_Screenshot();

                    l_StructuresEntry = archive.GetEntry(l_SavestateEntry_Screenshot.GetFilename());

                    if (l_StructuresEntry != null)
                    {
                        using (BinaryReader reader = new BinaryReader(l_StructuresEntry.Open()))
                        {
                            var l_bytes = l_SavestateEntry_Screenshot.Parser(new MemLoadingState(reader));

                            var bitmap = new BitmapImage();

                            using (var stream = new MemoryStream(l_bytes))
                            {
                                stream.Position = 0; // here

                                bitmap.BeginInit();

                                if(a_DecodePixelWidth > 0)
                                    bitmap.DecodePixelWidth = a_DecodePixelWidth;

                                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                bitmap.StreamSource = stream;
                                bitmap.EndInit();
                                bitmap.Freeze();
                            }

                            l_result.ImageSource = bitmap;
                        }
                    }
                }
            }

            return l_result;
        }
    }
}
