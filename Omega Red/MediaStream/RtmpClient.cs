using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MediaStream
{
    class RtmpClient
    {
        //Соединение с RTMP сервером.
        public static Func<string, string, int> Connect;

        //Отключение от RTMP сервера.
        public static Action<int> Disconnect;

        //Запись на RTMP сервер.
        public static Action<int, int, IntPtr, int, uint, int, int> Write;



        private int m_handler = -1;

        private RtmpClient() { }

        ~RtmpClient()
        {
            if (m_handler != -1)
                Disconnect(m_handler);
        }

        public static RtmpClient createInstance(string a_streamsXml, string a_url)
        {
            RtmpClient l_instance = null;

            try
            {
                do
                {
                    l_instance = new RtmpClient();

                    l_instance.m_handler = Connect(a_streamsXml, a_url);


                } while (false);

            }
            catch (Exception)
            {
            }

            return l_instance;
        }

        public void disconnect()
        {
            Disconnect(m_handler);
        }

        public void sendVideoData(int sampleTime, IntPtr buf, int size, uint sampleflags, int streamIdx)
        {
            Write(m_handler, sampleTime, buf, size, sampleflags, streamIdx, 1);
        }

        public void sendAudioData(int sampleTime, IntPtr buf, int size, uint sampleflags, int streamIdx)
        {
            Write(m_handler, sampleTime, buf, size, sampleflags, streamIdx, 0);
        }
    }
}
