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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Red.Tools
{
    class GameIndex
    {
        public class Patches
        {
            private string m_patches = "";

            public void addPatch(string a_patch)
            {
                m_patches += a_patch;
            }

            public string patches { get { return m_patches; } }
        }

        public class GameData
        {
            private Dictionary<string, string> m_propertyCollection = new Dictionary<string, string>();

            private Dictionary<string, Patches> m_patchesCollection = new Dictionary<string, Patches>();

            public string FriendlyName { get{
            
                if(m_propertyCollection.ContainsKey("name"))
                {
                    return m_propertyCollection["name"];
                }
                else
	            {
                    return "";
	            }
            
            } }
            
            public string Compatibility
            {
                get
                {

                    if (m_propertyCollection.ContainsKey("compat"))
                    {
                        int l_compat = 0;

                        if (int.TryParse(m_propertyCollection["compat"], out l_compat))
                        {
                            switch (l_compat)
                            {
                                case 6: return "Perfect";
                                case 5: return "Playable";
                                case 4: return "In-Game";
                                case 3: return "Menu";
                                case 2: return "Intro";
                                case 1: return "Nothing";
                                default: return "Unknown";
                            }
                        }

                        return "Unknown";
                    }
                    else
                    {
                        return "Unknown";
                    }

                }
            }
            
            public void addProperty(string a_key, string a_property)
            {
                if (!m_propertyCollection.ContainsKey(a_key.ToLower()))
                    m_propertyCollection.Add(a_key.ToLower(), a_property);
            }

            public void addPatches(string a_key, Patches a_patches)
            {
                if (!m_patchesCollection.ContainsKey(a_key.ToLower()))
                    m_patchesCollection.Add(a_key.ToLower(), a_patches);
            }

            private string convertSection(string a_key, string value)
            {
                return string.Format("[{0}{1}]", a_key, string.IsNullOrEmpty(value) ? null : string.Format(" = {0}", value)).ToLower();
            }

            public bool sectionExists(string a_key, string value = null)
            {
                var l_key = convertSection(a_key, value);

                return m_patchesCollection.ContainsKey(l_key);
            }

            public string getSection(string a_key, string value = "")
            {
                string l_result = "";

                var l_key = convertSection(a_key, value);

                if (m_patchesCollection.ContainsKey(l_key))
                {
                    l_result = m_patchesCollection[l_key].patches;
                }

                return l_result;
            }

            public bool keyExists(string a_key)
            {
                return m_propertyCollection.ContainsKey(a_key.ToLower());
            }

            public int getInt(string a_key)
            {
                int l_result = -1;

                if (m_propertyCollection.ContainsKey(a_key.ToLower()))
                {
                    int l_value = 0;

                    if (int.TryParse(m_propertyCollection[a_key.ToLower()], out l_value))
                    {
                        l_result = l_value;
                    }
                }

                return l_result;
            }

            public bool getBool(string a_key)
            {
                bool l_result = false;

                if (m_propertyCollection.ContainsKey(a_key.ToLower()))
                {
                    int l_value = 0;

                    if (int.TryParse(m_propertyCollection[a_key.ToLower()], out l_value))
                    {
                        l_result = l_value != 0;
                    }
                }

                return l_result;
            }

            public string getString(string a_key)
            {
                string l_result = "";

                if (m_propertyCollection.ContainsKey(a_key.ToLower()))
                {
                    l_result = m_propertyCollection[a_key.ToLower()];
                }

                return l_result;
            }

            
        }

        private Dictionary<string, GameData> m_gameCollection = new Dictionary<string,GameData>();

        private static GameIndex m_Instance = null;

        public static GameIndex Instance { get { if (m_Instance == null) m_Instance = new GameIndex(); return m_Instance; } }

        private GameIndex()
        {
            loadGameIndex();
        }
        
        private void loadGameIndex()
        {
            Assembly l_assembly = Assembly.GetExecutingAssembly();

            Stream l_GameIndexStream = l_assembly.GetManifestResourceStream("Omega_Red.Assests.GameIndex.dbf");

            if (l_GameIndexStream != null && l_GameIndexStream.CanRead)
            {
                StreamReader lf = new StreamReader(l_GameIndexStream);

                GameData l_currentGameData = null;

                Patches l_patches = null;

                while (!lf.EndOfStream)
                {
                    var l_line = lf.ReadLine();

                    if (l_line.StartsWith("Serial"))
                    {
                        var lsplit = l_line.Split(new string[] { " = " }, StringSplitOptions.RemoveEmptyEntries);

                        if(lsplit != null && lsplit.Length == 2)
                        {
                            l_currentGameData = new GameData();

                            m_gameCollection.Add(lsplit[1], l_currentGameData);
                        }
                    }
                    else
                        if (l_line.StartsWith("--") ||
                            l_line.StartsWith("//") ||
                            l_line.StartsWith("\t--") ||
                            l_line.StartsWith("\t//"))
                        {
                            continue;
                        }
                    else
                    if(!l_line.StartsWith("--") &&
                        !l_line.StartsWith("//") &&
                        !l_line.StartsWith("[patches") &&
                        !l_line.StartsWith("[/patches") &&
                        l_patches == null)
                    {
                        if (l_currentGameData == null)
                            continue;                                             

                        var lsplit = l_line.Split(new string[] { " = " }, StringSplitOptions.RemoveEmptyEntries);

                        if (lsplit != null && lsplit.Length == 2)
                        {
                            l_currentGameData.addProperty(lsplit[0].TrimEnd(new char[] { ' ' }), lsplit[1]);
                        }
                    }
                    else
                        if (l_line.StartsWith("[patches"))
                        {
                            if (l_currentGameData == null)
                                continue;

                            l_patches = new Patches();

                            l_currentGameData.addPatches(l_line, l_patches);
                        }
                        else
                            if (l_line.StartsWith("[/patches"))
                            {
                                if (l_patches == null)
                                    continue;

                                l_patches = null;
                            }
                            else
                                {
                                    if (l_patches == null)
                                        continue;

                                    l_patches.addPatch(l_line + "\n");
                                }
                }
            }
        }
        
        public GameData convert(string a_disc_serial)
        {
            GameData l_result = null;

            do
            {
                if(m_gameCollection.ContainsKey(a_disc_serial))
                {
                    l_result = m_gameCollection[a_disc_serial];
                }
                
            } while (false);

            return l_result;
        }
    }
}
