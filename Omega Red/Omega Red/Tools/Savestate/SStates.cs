/*  Omega Red - Client PS2 Emulator for PCs
*
*  Omega Red is free software: you can redistribute it and/or modify it under the terms
*  of the GNU Lesser General Public License as published by the Free Software Found-
*  ation, either version 3 of the License, or (at your option) any later version.
*
*  Omega Red is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
*  without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
*  PURPOSE.  See the GNU General Public License for more details.
*
*  You should have received a copy of the GNU General Public License along with Omega Red.
*  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omega_Red.Properties;
using Omega_Red.Util;
using Omega_Red.Models;
using Omega_Red.Panels;
using System.Windows.Media.Imaging;
using System.Globalization;

namespace Omega_Red.Tools.Savestate
{
    class SStates
    {
        private SStates() { }

        private static VideoPanel m_VideoPanel = null;

        private static SStates m_Instance = null;

        public static SStates Instance { get { if (m_Instance == null) m_Instance = new SStates(); return m_Instance; } }

        private List<IBaseSavestateEntry> m_SavestateEntries = new List<IBaseSavestateEntry>()
        {            
            new SavestateEntry_StateVersion(),
            new SavestateEntry_InternalStructures(),
            new SavestateEntry_Screenshot(takeScreenshot),
            new SavestateEntry_EmotionMemory(),
            new SavestateEntry_IopMemory(),
	        new SavestateEntry_HwRegs(),
	        new SavestateEntry_IopHwRegs(),
	        new SavestateEntry_Scratchpad(),
	        new SavestateEntry_VU0mem(),
	        new SavestateEntry_VU1mem(),
	        new SavestateEntry_VU0prog(),
	        new SavestateEntry_VU1prog(),
            new PluginSavestateEntry(PCSX2ModuleManager.ModuleType.VideoRenderer),
            new PluginSavestateEntry(PCSX2ModuleManager.ModuleType.AudioRenderer)
        };

        private List<IBaseSavestateEntry> m_PPSSPPSavestateEntries = new List<IBaseSavestateEntry>()
        {
            new SavestateEntry_StateVersion(),
            new SavestateEntry_PPSSPPInternalStructures(),
            new SavestateEntry_Screenshot(takeScreenshot)
        };

        private List<IBaseSavestateEntry> m_PCSXSavestateEntries = new List<IBaseSavestateEntry>()
        {
            new SavestateEntry_StateVersion(),
            new SavestateEntry_PCSXInternalStructures(),
            new SavestateEntry_Screenshot(takeScreenshot)
        };

        public void setVideoPanel(VideoPanel a_VideoPanel)
        {
            m_VideoPanel = a_VideoPanel;
        }

        private static byte[] takeScreenshot()
        {
            byte[] l_result = null;

            if (m_VideoPanel != null)
                l_result = m_VideoPanel.takeScreenshot();

            return l_result;
        }

        public SaveStateInfo readData(string a_file_path, int a_value, bool a_isAutosave = false, bool a_isIsQuicksave = false)
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
                                bitmap.DecodePixelWidth = 100;
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

        public void Save(string a_FilePath, string aDate, double aDurationInSeconds)
        {                                    
            using (FileStream zipToOpen = new FileStream(a_FilePath, FileMode.Create))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    var lSavestateEntry_TimeSession = new SavestateEntry_TimeSession(aDate, aDurationInSeconds);

                    m_SavestateEntries.Add(lSavestateEntry_TimeSession);

                    foreach (var l_SavestateEntry in m_SavestateEntries)
                    {
                        ZipArchiveEntry l_InternalStructuresEntry = archive.CreateEntry(l_SavestateEntry.GetFilename());

                        using (BinaryWriter writer = new BinaryWriter(l_InternalStructuresEntry.Open()))
                        {
                            l_SavestateEntry.FreezeOut(new MemSavingState(writer));
                        }  
                    }

                    m_SavestateEntries.Remove(lSavestateEntry_TimeSession);
                }
            }
        }

        public void SavePPSSPP(string a_FilePath, string a_TempFilePath, string aDate, double aDurationInSeconds)
        {
            using (FileStream zipToOpen = new FileStream(a_FilePath, FileMode.Create))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    var lSavestateEntry_TimeSession = new SavestateEntry_TimeSession(aDate, aDurationInSeconds);

                    m_PPSSPPSavestateEntries.Add(lSavestateEntry_TimeSession);

                    var lSavestateEntry_PPSSPPSState = new SavestateEntry_PPSSPPSState(a_TempFilePath);

                    m_PPSSPPSavestateEntries.Add(lSavestateEntry_PPSSPPSState);                    

                    foreach (var l_SavestateEntry in m_PPSSPPSavestateEntries)
                    {
                        ZipArchiveEntry l_InternalStructuresEntry = archive.CreateEntry(l_SavestateEntry.GetFilename());

                        using (BinaryWriter writer = new BinaryWriter(l_InternalStructuresEntry.Open()))
                        {
                            l_SavestateEntry.FreezeOut(new MemSavingState(writer));
                        }
                    }

                    m_PPSSPPSavestateEntries.Remove(lSavestateEntry_TimeSession);

                    m_PPSSPPSavestateEntries.Remove(lSavestateEntry_PPSSPPSState);
                }
            }
        }

        public void SavePCSX(string a_FilePath, string a_TempFilePath, string aDate, double aDurationInSeconds)
        {
            using (FileStream zipToOpen = new FileStream(a_FilePath, FileMode.Create))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    var lSavestateEntry_TimeSession = new SavestateEntry_TimeSession(aDate, aDurationInSeconds);

                    m_PCSXSavestateEntries.Add(lSavestateEntry_TimeSession);

                    var lSavestateEntry_PCSXState = new SavestateEntry_PCSXState(a_TempFilePath);

                    m_PCSXSavestateEntries.Add(lSavestateEntry_PCSXState);

                    foreach (var l_SavestateEntry in m_PCSXSavestateEntries)
                    {
                        ZipArchiveEntry l_InternalStructuresEntry = archive.CreateEntry(l_SavestateEntry.GetFilename());

                        using (BinaryWriter writer = new BinaryWriter(l_InternalStructuresEntry.Open()))
                        {
                            l_SavestateEntry.FreezeOut(new MemSavingState(writer));
                        }
                    }

                    m_PCSXSavestateEntries.Remove(lSavestateEntry_TimeSession);

                    m_PCSXSavestateEntries.Remove(lSavestateEntry_PCSXState);
                }
            }
        }

        public void Load(string a_FilePath)
        {

            using (FileStream zipToOpen = new FileStream(a_FilePath, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
                {
                    foreach (var l_SavestateEntry in m_SavestateEntries)
                    {
                        ZipArchiveEntry l_InternalStructuresEntry = archive.GetEntry(l_SavestateEntry.GetFilename());

                        if (l_InternalStructuresEntry != null)
                        {
                            using (BinaryReader reader = new BinaryReader(l_InternalStructuresEntry.Open()))
                            {
                                l_SavestateEntry.FreezeIn(new MemLoadingState(reader));
                            }
                        }
                    }
                }
            }
        }

        public void LoadPPSSPP(string a_FilePath, string a_tempFilePath)
        {

            using (FileStream zipToOpen = new FileStream(a_FilePath, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
                {
                    var lSavestateEntry_PPSSPPSState = new SavestateEntry_PPSSPPSState(a_tempFilePath);
                    
                    ZipArchiveEntry l_InternalStructuresEntry = archive.GetEntry(lSavestateEntry_PPSSPPSState.GetFilename());

                    if (l_InternalStructuresEntry != null)
                    {
                        using (BinaryReader reader = new BinaryReader(l_InternalStructuresEntry.Open()))
                        {
                            lSavestateEntry_PPSSPPSState.FreezeIn(new MemLoadingState(reader));
                        }
                    }
                }
            }
        }

        public void LoadPCSX(string a_FilePath, string a_tempFilePath)
        {

            using (FileStream zipToOpen = new FileStream(a_FilePath, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
                {
                    var lSavestateEntry_PCSXState = new SavestateEntry_PCSXState(a_tempFilePath);

                    ZipArchiveEntry l_InternalStructuresEntry = archive.GetEntry(lSavestateEntry_PCSXState.GetFilename());

                    if (l_InternalStructuresEntry != null)
                    {
                        using (BinaryReader reader = new BinaryReader(l_InternalStructuresEntry.Open()))
                        {
                            lSavestateEntry_PCSXState.FreezeIn(new MemLoadingState(reader));
                        }
                    }
                }
            }
        }
    }
}
