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
using System.Globalization;

namespace PCSX2Emul.Tools.Savestate
{
    class SStates
    {
        public static byte[] Screenshot { get; set; } = null;

        private SStates() { }
        
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
                        
        private static byte[] takeScreenshot()
        {            
            return Screenshot;
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
    }
}
