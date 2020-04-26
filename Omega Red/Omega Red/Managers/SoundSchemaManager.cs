using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Omega_Red.Managers
{
    class SoundSchemaManager
    {
        public enum Event
        {
            Switch,
            Click
        }

        private MediaPlayer m_Sound = new MediaPlayer();

        private static SoundSchemaManager m_Instance = null;

        public static SoundSchemaManager Instance { get { if (m_Instance == null) m_Instance = new SoundSchemaManager(); return m_Instance; } }

        private SoundSchemaManager() { }

        public void playEvent(Event a_event)
        {
            var l_name = Enum.GetName(a_event.GetType(), a_event);

            var l_uri = new Uri(@"pack://application:,,,/Omega Red;Component/Assests/Sounds/BaseSounds/" + l_name + ".wav");

            SoundPlayer player = new SoundPlayer(System.Windows.Application.GetResourceStream(l_uri).Stream);

            player.Play();
        }
    }
}
