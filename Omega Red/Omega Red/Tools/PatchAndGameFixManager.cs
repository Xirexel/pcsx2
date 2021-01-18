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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Omega_Red.Util;
using System.Reflection;
using System.IO.Compression;
using Omega_Red.Properties;

namespace Omega_Red.Tools
{
        
    class PatchAndGameFixManager
    {
        private PatchAndGameFixManager() { }

        private static PatchAndGameFixManager m_Instance = null;

        public static PatchAndGameFixManager Instance { get { if (m_Instance == null) m_Instance = new PatchAndGameFixManager(); return m_Instance; } }
        
        public const string _UNKNOWN_GAME_KEY = "_UNKNOWN_GAME_KEY";

        public string curGameKey = _UNKNOWN_GAME_KEY;

        private void PatchesVerboseReset()
        {
	        curGameKey = _UNKNOWN_GAME_KEY;
        }

        public void LoadAllPatchesAndStuff()
        {
            PatchesVerboseReset();

            _ApplySettings();
        }
                
        private void sioSetGameSerial(string a_GameSerial)
        {
        }
                    

        // This routine loads patches from the game database (but not the config/game fixes/hacks)
        // Returns number of patches loaded
        private uint loadPatchesFromGamesDB(string crc, GameIndex.GameData game)
        {
	        bool patchFound = false;

	        string patch = "";

            if (game != null)
            {
                if (game.sectionExists("patches", crc)) {
                    patch = game.getSection("patches", crc);
                    patchFound = true;
                }
                else if (game.sectionExists("patches")) {
                    patch = game.getSection("patches");
                    patchFound = true;
                }
            }

            if (patchFound) TrimPatches(patch);

	        return 0;
        }

        private void TrimPatches(string s)
        {
            if(string.IsNullOrEmpty(s))
                return;

            var l_splits = s.Split(new char[]{'\n'});

            if(l_splits == null ||
                l_splits.Length <= 0)
                return;

            foreach (var patchString in l_splits)
            {
                if (patchString.StartsWith("/"))
                    continue;

            }
        }

        private void _ApplySettings()
        {
            //Threading::ScopedLock lock(mtx__ApplySettings);
	        // 'fixup' is the EmuConfig we're going to upload to the emulator, which very well may
	        // differ from the user-configured EmuConfig settings.  So we make a copy here and then
	        // we apply the commandline overrides and database gamefixes, and then upload 'fixup'
	        // to the global EmuConfig.
	        //
	        // Note: It's important that we apply the commandline overrides *before* database fixes.
	        // The database takes precedence (if enabled).

            //fixup = src;

            //const CommandlineOverrides& overrides( wxGetApp().Overrides );
            //if( overrides.DisableSpeedhacks || !g_Conf->EnableSpeedHacks )
            //    fixup.Speedhacks.DisableAll();

            //if( overrides.ApplyCustomGamefixes )
            //{
            //    for (GamefixId id=GamefixId_FIRST; id < pxEnumEnd; ++id)
            //        fixup.Gamefixes.Set( id, overrides.Gamefixes.Get(id) );
            //}
            //else if( !g_Conf->EnableGameFixes )
            //    fixup.Gamefixes.DisableAll();

            //if( overrides.ProfilingMode )
            //{
            //    fixup.GS.FrameLimitEnable = false;
            //    fixup.GS.VsyncEnable = VsyncMode::Off;
            //}
            
            string gameCRC = null;
            string gameSerial = null;
            string gamePatch = null;
            string gameFixes = null;
            string gameCheats = null;
            string gameWsHacks = null;

	        string gameName = null;
            string gameCompat = null;
            string gameMemCardFilter = null;
        }

        public void LoadPatches(string gameCRC)
        {
            if (!Settings.Default.DisableWideScreen && !string.IsNullOrEmpty(gameCRC))
            {
                var f = LoadPatchesFromZip(gameCRC);

                //if (int numberLoadedWideScreenPatches = LoadPatchesFromDir(gameCRC, GetCheatsWsFolder(), L"Widescreen hacks"))
                //{
                //    gameWsHacks.Printf(L" [%d widescreen hacks]", numberLoadedWideScreenPatches);
                //    Console.WriteLn(Color_Gray, "Found widescreen patches in the cheats_ws folder --> skipping cheats_ws.zip");
                //}
                //else
                //{
                //    // No ws cheat files found at the cheats_ws folder, try the ws cheats zip file.
                //    string cheats_ws_archive = Path::Combine(PathDefs::GetProgramDataDir(), wxFileName(L"cheats_ws.zip"));
                //    int numberDbfCheatsLoaded = LoadPatchesFromZip(gameCRC, cheats_ws_archive);
                //    PatchesCon->WriteLn(Color_Green, "(Wide Screen Cheats DB) Patches Loaded: %d", numberDbfCheatsLoaded);
                //    gameWsHacks.Printf(L" [%d widescreen hacks]", numberDbfCheatsLoaded);
                //}
            }
        }
        
        int LoadPatchesFromZip(string gameCRC)
        {
            int lresult = 0;
            
            Assembly l_assembly = Assembly.GetExecutingAssembly();

            Stream l_Cheats_WSStream = l_assembly.GetManifestResourceStream("Omega_Red.Assests.cheats_ws.zip");

            if (l_Cheats_WSStream != null && l_Cheats_WSStream.CanRead)
            {
                using (ZipArchive archive = new ZipArchive(l_Cheats_WSStream, ZipArchiveMode.Read))
                {

                    foreach (var item in archive.Entries)
                    {
                        if (item.Name.ToLower().Contains(gameCRC.ToLower()) && item.Name.ToLower().Contains(".pnach"))
                        {
                            StreamReader lg = new StreamReader(item.Open());

                            var lpatchesString = lg.ReadToEnd();

                            TrimPatches(lpatchesString);
                        }
                    }

                }

            }

            return lresult;
        }
    }
}
