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

        //Проверка соединения с сервером.
        public static Func<int, bool> ConnectedFunc;

        private const uint CleanPoint = 1;

        private Action<bool> m_isConnected;

        private int m_handler = -1;

        public int Handler { get { return m_handler; }}

        private RtmpClient() { }

        ~RtmpClient()
        {
            if (m_handler != -1)
                disconnect();
        }

        public static RtmpClient createInstance(string a_streamsXml, string a_url, Action<bool> a_isConnected)
        {
            RtmpClient l_instance = null;

            try
            {
                do
                {
                    l_instance = new RtmpClient();

                    l_instance.m_handler = Connect(a_streamsXml, a_url);

                    l_instance.m_isConnected = a_isConnected;

                } while (false);

            }
            catch (Exception)
            {
            }

            return l_instance;
        }

        public void disconnect()
        {
            var l_handler = m_handler;

            m_handler = -1;

            Disconnect(l_handler);
        }

        public void sendVideoData(int sampleTime, IntPtr buf, int size, uint sampleflags, int streamIdx)
        {
            if (m_handler == -1)
                return;

            Write(m_handler, sampleTime, buf, size, sampleflags, streamIdx, 1);

            if (sampleflags == CleanPoint)
                checkConnection();
        }

        public void sendAudioData(int sampleTime, IntPtr buf, int size, uint sampleflags, int streamIdx)
        {
            if (m_handler == -1)
                return;

            Write(m_handler, sampleTime, buf, size, sampleflags, streamIdx, 0);
        }

        private void checkConnection()
        {
            if (m_handler == -1)
                return;

            if (ConnectedFunc != null)
            {
                if(m_handler != -1 && m_isConnected != null)
                    m_isConnected(ConnectedFunc(m_handler));
            }
        }
    }
}
