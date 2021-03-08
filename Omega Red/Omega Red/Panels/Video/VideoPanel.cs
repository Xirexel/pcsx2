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

using Omega_Red.Panels.Video.Interop;
using Omega_Red.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Omega_Red.Panels
{
    internal class VideoPanel : System.Windows.Controls.ContentControl, IDisposable
    {
        enum D3DFMT
        {
            D3DFMT_A8R8G8B8 = 21,
            D3DFMT_X8R8G8B8 = 22
        }

        private const int Format = (int)D3DFMT.D3DFMT_A8R8G8B8;

        public uint VideoWidth { get; private set; }
        public uint VideoHeight { get; private set; }

        public static uint WIDTH = 1280;// 800; 

        public static uint HEIGHT = 720;// 600;

        private class D3D9Image : D3DImage, IDisposable
        {
            private Direct3DTexture9 texture;

            private static Direct3DDevice9Ex direct3DDevice9Ex = null;

            private D3D9Image() { }

            static public System.Tuple<D3DImage, IntPtr> createD3D9Image(uint aWidth, uint aHeight)
            {
                D3D9Image lImageSource = new D3D9Image();

                return lImageSource.init(aWidth, aHeight) ?
                    System.Tuple.Create<D3DImage, IntPtr>(lImageSource, lImageSource.texture.SharedHandle)
                    :
                    null;
            }

            public BitmapSource getBackBuffer()
            {
                return this.CopyBackBuffer();
            }

            private bool init(uint aWidth, uint aHeight)
            {
                bool lresult = false;

                do
                {

                    // Free the old texture
                    if (this.texture != null)
                    {
                        this.texture.Dispose();
                        this.texture = null;
                    }

                    var device = CreateDevice(NativeMethods.GetDesktopWindow());

                    if (device != null)
                    {
                        texture = GetSharedSurface(device, aWidth, aHeight);

                        Lock();

                        this.SetBackBuffer(D3DResourceType.IDirect3DSurface9, this.texture.GetSurfaceLevel(0).NativeInterface);

                        Unlock();

                        lresult = true;
                    }

                } while (false);

                return lresult;
            }

            private Direct3DTexture9 GetSharedSurface(Direct3DDevice9Ex device, uint aWidth, uint aHeight)
            {

                return device.CreateTexture(
                    aWidth,
                    aHeight,
                    1,
                    1,  //D3DUSAGE_RENDERTARGET
                    Format,
                    0  //D3DPOOL_DEFAULT
                    );
            }

            private static Direct3DDevice9Ex CreateDevice(IntPtr handle)
            {
                if (direct3DDevice9Ex == null)
                    using (var d3d9 = Direct3D9Ex.Create(32))
                    {
                        var present = new NativeStructs.D3DPRESENT_PARAMETERS();

                        try
                        {
                            var wih = new System.Windows.Interop.WindowInteropHelper(App.Current.MainWindow);

                            if (wih.Handle != IntPtr.Zero)
                                handle = wih.Handle;
                        }
                        catch (Exception)
                        {
                        }

                        present.BackBufferFormat = Format;
                        present.BackBufferHeight = 1;
                        present.BackBufferWidth = 1;
                        present.Windowed = 1; // TRUE
                        present.SwapEffect = 1; // D3DSWAPEFFECT_DISCARD
                        present.hDeviceWindow = handle;
                        present.PresentationInterval = unchecked((int)0x80000000); // D3DPRESENT_INTERVAL_IMMEDIATE;

                        direct3DDevice9Ex = d3d9.CreateDeviceEx(
                            0, // D3DADAPTER_DEFAULT
                            1, // D3DDEVTYPE_HAL
                            handle,
                            0x40 | 0x10,// D3DCREATE_HARDWARE_VERTEXPROCESSING | D3DCREATE_PUREDEVICE
                            present,
                            null);
                    }

                return direct3DDevice9Ex;
            }

            public void Dispose()
            {
                Lock();

                this.SetBackBuffer(D3DResourceType.IDirect3DSurface9, IntPtr.Zero);

                Unlock();

                if (texture != null)
                    texture.Dispose();

                texture = null;
            }
        }

        private System.Windows.Interop.D3DImage imageSource = null;

        private IntPtr sharedHandle = IntPtr.Zero;

        public IntPtr SharedHandle { get { return sharedHandle; } }

        public string SymbolicLink { get; private set; }
        
        public VideoPanel()
        {
            SymbolicLink = "";

            Managers.ConfigManager.Instance.DisplayFrameEvent += (a_displayFrame) => { m_displayFrame = a_displayFrame; };
                       
            var image = new System.Windows.Controls.Image();
            image.Stretch = System.Windows.Media.Stretch.Uniform;

            Grid l_grid = new Grid();
            l_grid.Children.Add(image);

            m_frameRateCalcTimer = new Timer((state) => {

                m_lock.EnterReadLock();

                try
                {
                    var tempFrameCount = m_frameCount;

                    if(Application.Current != null)
                        Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, (ThreadStart)delegate ()
                        {
                            if (Settings.Default.ShowFrameRate)
                                Managers.ConfigManager.Instance.setFrameRate(tempFrameCount.ToString());
                            else
                                Managers.ConfigManager.Instance.setFrameRate("");
                        });

                    m_frameCount = 0;
                }
                finally
                {
                    m_lock.ExitReadLock();
                }

            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

            this.AddChild(l_grid);


            // To greatly reduce flickering we're only going to AddDirtyRect
            // when WPF is rendering.
            System.Windows.Media.CompositionTarget.Rendering += this.CompositionTargetRendering;

            
            Managers.ConfigManager.Instance.ResolutionEvent += (a_resolution) => {

                VideoWidth = (a_resolution * 16)/9;

                VideoHeight = a_resolution;

                var lTuple = D3D9Image.createD3D9Image(VideoWidth, VideoHeight);

                if (lTuple != null)
                {
                    this.imageSource = lTuple.Item1;

                    this.sharedHandle = lTuple.Item2;
                }

                image.Source = this.imageSource;
            };

            App.Current.Exit += (sender, e)=>{
                m_frameRateCalcTimer.Dispose();
            };
        }
        
        public VideoPanel(uint aWidth, uint aHeight, string a_SymbolicLink)
        {
            VideoWidth = aWidth;

            VideoHeight = aHeight;

            SymbolicLink = a_SymbolicLink;
            
            var lTuple = D3D9Image.createD3D9Image(aWidth, aHeight);

            if (lTuple != null)
            {
                this.imageSource = lTuple.Item1;

                this.sharedHandle = lTuple.Item2;
            }

            if (this.imageSource != null)
            {
                var image = new System.Windows.Controls.Image();
                image.Stretch = System.Windows.Media.Stretch.Uniform;
                image.Source = this.imageSource;

                this.AddChild(image);


                // To greatly reduce flickering we're only going to AddDirtyRect
                // when WPF is rendering.
                System.Windows.Media.CompositionTarget.Rendering += this.CompositionTargetRendering;
            }
        }

        ~VideoPanel()
        {

        }

        public byte[] takeScreenshot()
        {
            byte[] l_result = null;

            var l_D3D9Image = imageSource as D3D9Image;

            if (l_D3D9Image != null)
            {
                var l_bitmap = l_D3D9Image.getBackBuffer();

                if(l_bitmap != null)
                {
                    JpegBitmapEncoder l_encoder = new JpegBitmapEncoder();

                    l_encoder.QualityLevel = 75;

                    Array lPixels = Array.CreateInstance(typeof(Byte), l_bitmap.PixelWidth * 4 * l_bitmap.PixelHeight);

                    l_bitmap.CopyPixels(lPixels, l_bitmap.PixelWidth * 4, 0);

                    l_bitmap = BitmapSource.Create(
                        l_bitmap.PixelWidth,
                        l_bitmap.PixelHeight,
                        l_bitmap.DpiX,
                        l_bitmap.DpiY,
                        PixelFormats.Bgr32,
                        null,
                        lPixels,
                        l_bitmap.PixelWidth * 4
                        );

                    l_encoder.Frames.Add(BitmapFrame.Create(l_bitmap));

                    using (var outputStream = new MemoryStream())
                    {
                        l_encoder.Save(outputStream);

                        l_result = outputStream.ToArray();
                    }
                }
            }

            return l_result;
        }
        
        private readonly ReaderWriterLockSlim m_lock = new ReaderWriterLockSlim();
        private int m_frameCount;
        private readonly Timer m_frameRateCalcTimer;
        
        private int m_displayFrameCount = 0;

        private int m_displayFrame = 1;

        private void CompositionTargetRendering(object sender, EventArgs e)
        {
            ++m_displayFrameCount;

            m_displayFrameCount = m_displayFrameCount % m_displayFrame;

            if (m_displayFrameCount == 0)
            {
                if(Settings.Default.ShowFrameRate)
                {
                    m_lock.EnterWriteLock();
                    try
                    {
                        m_frameCount++;
                    }
                    finally
                    {
                        m_lock.ExitWriteLock();
                    }
                }

                if (this.imageSource != null && this.imageSource.IsFrontBufferAvailable)
                {
                    this.imageSource.Lock();
                    this.imageSource.AddDirtyRect(new Int32Rect(0, 0, this.imageSource.PixelWidth, this.imageSource.PixelHeight));
                    this.imageSource.Unlock();
                }
            }
        }

        public void Dispose()
        {
            var lIDisposable = imageSource as IDisposable;

            this.imageSource = null;

            if (lIDisposable != null)
                lIDisposable.Dispose();
        }
    }
}
