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
using PPSSPPEmul.Util;
using System.Globalization;

namespace PPSSPPEmul.Tools.Savestate
{
    class SStates
    {
        public static byte[] Screenshot { get; set; } = null;

        private SStates() { }
        
        private static SStates m_Instance = null;

        public static SStates Instance { get { if (m_Instance == null) m_Instance = new SStates(); return m_Instance; } }
        
        private List<IBaseSavestateEntry> m_PPSSPPSavestateEntries = new List<IBaseSavestateEntry>()
        {
            new SavestateEntry_StateVersion(),
            new SavestateEntry_PPSSPPInternalStructures(),
            new SavestateEntry_Screenshot(takeScreenshot)
        };
        
        private static byte[] takeScreenshot()
        {
            return Screenshot;
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
    }
}
