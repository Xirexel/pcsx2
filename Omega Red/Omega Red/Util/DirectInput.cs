//
// DirectInput.cs
//
// Glue code to just enough of DirectInput so that we can successfully
// read the joystick controllers. Not exactly pretty.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Omega_Red.Util
{


    //------------------------------------------------------------------------------------------------
    // Interfaces and DLL imports
    //------------------------------------------------------------------------------------------------

    [ComImport,Guid("BF798031-483A-4DA2-AA99-5D64ED369700"),InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDirectInput8W
        {
        void CreateDevice(
            [In] ref Guid rguid,
            [Out, MarshalAs(UnmanagedType.Interface)] out IDirectInputDevice8W ppDevice,
            [In, MarshalAs(UnmanagedType.IUnknown)] object punkOuter
            );
        void EnumDevices(
            [In,MarshalAs(UnmanagedType.U4)] DI8DEVTYPE dwDevType,
            [In] IntPtr callback,
            [In] IntPtr pvRef,
            [In,MarshalAs(UnmanagedType.U4)] DIEDFL dwFlags
            );
        void _GetDeviceStatus();
        void _RunControlPanel();
        void Initialize(IntPtr hInstance, int dwVersion);
        void _FindDevice();
        void _EnumDevicesBySemantics();
        void _ConfigureDevices();
        }

    [ComImport,Guid("54D41081-DC15-4833-A41B-748F73A38179"),InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDirectInputDevice8W 
        {
        void _GetCapabilities();
        void EnumObjects(
            [In] IntPtr callback,
            [In] IntPtr pvRef,
            [In,MarshalAs(UnmanagedType.U4)] DIDFT dwFlags
            );
        void _GetProperty();
        void SetProperty(
            [In] IntPtr rguidProp,
            [In] ref DIPROPHEADER diph
            );
        [PreserveSig] int Acquire();
        [PreserveSig] int Unacquire();
        int GetDeviceState(
            [In, MarshalAs(UnmanagedType.U4)] int cbData,
            [In] IntPtr pData
            );
        void _GetDeviceData();
        void SetDataFormat(
            [In] ref DIDATAFORMAT pdf
            );
        void _SetEventNotification();
        void _SetCooperativeLevel();
        void _GetObjectInfo();
        void _GetDeviceInfo();
        void _RunControlPanel();
        void _Initialize();
        void _CreateEffect();
        void _EnumEffects();
        void _GetEffectInfo();
        void _GetForceFeedbackState();
        void _SendForceFeedbackCommand();
        void _EnumCreatedEffectObjects();
        void _Escape();
        [PreserveSig] int Poll();
        void _SendDeviceData();
        void _EnumEffectsInFile();
        void _WriteEffectToFile();
        void _BuildActionMap();
        void _SetActionMap();
        void _GetImageInfo();
        }

    //------------------------------------------------------------------------------------------------
    // Delegates
    //------------------------------------------------------------------------------------------------

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate int LPDIENUMDEVICESCALLBACK(ref DIDEVICEINSTANCEW pddi, IntPtr pvRef);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate int DIEnumDeviceObjectsCallback(ref DIDEVICEOBJECTINSTANCE pddoi, IntPtr pvRef);

    //------------------------------------------------------------------------------------------------
    // Structs
    //------------------------------------------------------------------------------------------------

    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
    public struct DIDEVICEINSTANCEW
        {
        public int  dwSize;
        public Guid guidInstance;
        public Guid guidProduct;
        public int  dwDevType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=Win32NativeMethods.MAX_PATH)] public string tszInstanceName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=Win32NativeMethods.MAX_PATH)] public string tszProductName;
        public Guid guidFFDriver;
        public short wUsagePage;
        public short wUsage;

        public DIDEVICEINSTANCEW Clone()
            {
            return (DIDEVICEINSTANCEW)(this.MemberwiseClone());
            }
        }

    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
    public struct DIDEVICEOBJECTINSTANCE
        {
        public int   dwSize;
        public Guid  guidType;
        public int   dwOfs;
        public int   dwType;
        public int   dwFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=Win32NativeMethods.MAX_PATH)] public string tszName;
        public int   dwFFMaxForce;
        public int   dwFFForceResolution;
        public short wCollectionNumber;
        public short wDesignatorIndex;
        public short wUsagePage;
        public short wUsage;
        public int   dwDimension;
        public short wExponent;
        public short wReportId;

        public DIDEVICEOBJECTINSTANCE Clone()
            {
            return (DIDEVICEOBJECTINSTANCE)(this.MemberwiseClone());
            }
        }

    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
    public struct DIPROPRANGE
        {
        public DIPROPHEADER diph;
        public int lMin;
        public int lMax;
        }

    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
    public struct DIPROPHEADER
        {
        public int dwSize;
        public int dwHeaderSize;
        public int dwObj;
        public int dwHow;
        }

    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
    public struct DIDATAFORMAT
        {
        public int    dwSize;
        public int    dwObjSize;
        public int    dwFlags;     // use DIDF_ABSAXIS
        public int    dwDataSize;
        public int    dwNumObjs;
        public IntPtr rgodf;

        public void Init(int dwNumObjs)
            {
            this.dwSize     = Marshal.SizeOf(this);
            this.dwObjSize  = Marshal.SizeOf(typeof(DIOBJECTDATAFORMAT));
            this.dwFlags    = 0;
            this.dwDataSize = 0;
            this.rgodf      = IntPtr.Zero;
            this.dwNumObjs  = 0;
            Alloc(dwNumObjs);
            }

        public void Alloc(int dwNumObjs)
            {
            Free();
            //
            int cb         = this.dwObjSize * dwNumObjs;
            this.rgodf     = Marshal.AllocCoTaskMem(cb);
            this.dwNumObjs = dwNumObjs;
            for (int i = 0; i < this.dwNumObjs; i++)
                {
                //this.rgodf[i].Init();
                }
            }

        public void Free()
            {
            if (this.rgodf != null)
                {
                for (int i = 0; i < this.dwNumObjs; i++)
                    {
                    //this.rgodf[i].Free();
                    }
                Marshal.FreeCoTaskMem((IntPtr)this.rgodf);
                this.rgodf = IntPtr.Zero;
                this.dwNumObjs = 0;
                }
            }
        }

    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
    public struct DIOBJECTDATAFORMAT 
    {
        public IntPtr  pguid;
        public int    dwOfs;
        public int    dwType;
        public int    dwFlags;

        public DIOBJECTDATAFORMAT(Guid guid, DIJOYSTATE.DIJOFS dib, DIDFT didft, DIDOI flags)
        {
            var l_bytesGuid = guid.ToByteArray();

            this.pguid = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(Guid)));

            Marshal.Copy(l_bytesGuid, 0, this.pguid, l_bytesGuid.Length);

            this.dwOfs = (int)dib;
            this.dwType = (int)didft;
            this.dwFlags = (int)flags;
        }

        public void Init()
        {
            this.pguid = IntPtr.Zero;
        //Set(null, IntPtr.Zero, DIDFT.ALL, DIDOI.NONE);
        }

        public void Free()
        {
            FreeGuid();
        }

        public void Set(ref Guid guid, IntPtr dib, DIDFT didft, DIDOI flags)
        {
            //fixed (Guid* pguid = &guid)
            //    {
            //    Set(pguid, dib, didft, flags);
            //    }
        }

        //public void Set(ref Guid pguid, IntPtr dib, DIDFT didft, DIDOI flags)
        //{
            //this.dwOfs    = (int)dib;
            //this.dwType   = (int)didft;
            //this.dwFlags  = (int)flags;
            //if (null == pguid)
            //    {
            //    FreeGuid();
            //    }
            //else
            //    {
            //    AllocGuid();
            //    *(this.pguid) = *pguid;
            //    }
        //}

        private void AllocGuid()
        {
            if (null == this.pguid)
                {
                this.pguid = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(Guid)));
                }
        }

        private void FreeGuid()
        {
            if (null != this.pguid)
                {
                Marshal.FreeCoTaskMem((IntPtr)this.pguid);
                this.pguid = IntPtr.Zero;
                }
        }
    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DIJOYSTATE
    {
        public int lX;
        public int lY;
        public int lZ;
        public int lRx;
        public int lRy;
        public int lRz;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] rglSlider;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public UInt32[] rgdwPOV;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public int[] rgbButtons;



        public enum DIJOFS : uint
        {
            X = 0,
            Y = X + sizeof(int),
            Z = Y + sizeof(int),
            RX = Z + sizeof(int),
            RY = RX + sizeof(int),
            RZ = RY + sizeof(int),
            SLIDER0 = RZ + sizeof(int),
            SLIDER1= SLIDER0 + sizeof(int),
            POV0 = SLIDER1 + sizeof(int),
            POV1 = POV0 + sizeof(int),
            POV2 = POV1 + sizeof(int),
            POV3 = POV2 + sizeof(int),
            BUTTON0 = POV3 + sizeof(int),
            BUTTON1 = BUTTON0 + sizeof(int),
            BUTTON2 = BUTTON1 + sizeof(int),
            BUTTON3 = BUTTON2 + sizeof(int),
            BUTTON4 = BUTTON3 + sizeof(int),
            BUTTON5 = BUTTON4 + sizeof(int),
            BUTTON6 = BUTTON5 + sizeof(int),
            BUTTON7 = BUTTON6 + sizeof(int),
            BUTTON8 = BUTTON7 + sizeof(int),
            BUTTON9 = BUTTON8 + sizeof(int),
            BUTTON10 = BUTTON9 + sizeof(int),
            BUTTON11 = BUTTON10 + sizeof(int),
            BUTTON12 = BUTTON11 + sizeof(int),
            BUTTON13 = BUTTON12 + sizeof(int),
            BUTTON14 = BUTTON13 + sizeof(int),
            BUTTON15 = BUTTON14 + sizeof(int),
            BUTTON16 = BUTTON15 + sizeof(int),
            BUTTON17 = BUTTON16 + sizeof(int),
            BUTTON18 = BUTTON17 + sizeof(int),
            BUTTON19 = BUTTON18 + sizeof(int),
            BUTTON20 = BUTTON19 + sizeof(int),
            BUTTON21 = BUTTON20 + sizeof(int),
            BUTTON22 = BUTTON21 + sizeof(int),
            BUTTON23 = BUTTON22 + sizeof(int),
            BUTTON24 = BUTTON23 + sizeof(int),
            BUTTON25 = BUTTON24 + sizeof(int),
            BUTTON26 = BUTTON25 + sizeof(int),
            BUTTON27 = BUTTON26 + sizeof(int),
            BUTTON28 = BUTTON27 + sizeof(int),
            BUTTON29 = BUTTON28 + sizeof(int),
            BUTTON30 = BUTTON29 + sizeof(int),
            BUTTON31 = BUTTON30 + sizeof(int)
        }
    }

    public enum DIDFT : uint
    {
        ALL = 0x00000000,
        RELAXIS = 0x00000001,
        ABSAXIS = 0x00000002,
        AXIS = 0x00000003,
        PSHBUTTON = 0x00000004,
        TGLBUTTON = 0x00000008,
        BUTTON = 0x0000000C,
        POV = 0x00000010,
        COLLECTION = 0x00000040,
        NODATA = 0x00000080,
        ANYINSTANCE = 0x00FFFF00,
        INSTANCEMASK = ANYINSTANCE,
        FFACTUATOR = 0x01000000,
        FFEFFECTTRIGGER = 0x02000000,
        OUTPUT = 0x10000000,
        VENDORDEFINED = 0x04000000,
        ALIAS = 0x08000000,
        NOCOLLECTION = 0x00FFFF00,
        MYSTERY = 0x80000000,   // We seem to need this (and it's in c_dfDIJoystick2), but we don't know what it does
        OPTIONAL = 0x80000000
    }

    //------------------------------------------------------------------------------------------------
    // Enums and other constants
    //------------------------------------------------------------------------------------------------

    public static class Guids
    {
        public static Guid XAxis  = new Guid(0xA36D02E0, 0xC9F3, 0x11CF, 0xBF, 0xC7, 0x44, 0x45, 0x53, 0x54, 0x00, 0x00);
        public static Guid YAxis  = new Guid(0xA36D02E1, 0xC9F3, 0x11CF, 0xBF, 0xC7, 0x44, 0x45, 0x53, 0x54, 0x00, 0x00);
        public static Guid ZAxis  = new Guid(0xA36D02E2, 0xC9F3, 0x11CF, 0xBF, 0xC7, 0x44, 0x45, 0x53, 0x54, 0x00, 0x00);
        public static Guid RzAxis = new Guid(0xA36D02E3, 0xC9F3, 0x11CF, 0xBF, 0xC7, 0x44, 0x45, 0x53, 0x54, 0x00, 0x00);
        public static Guid Slider = new Guid(0xA36D02E4, 0xC9F3, 0x11CF, 0xBF, 0xC7, 0x44, 0x45, 0x53, 0x54, 0x00, 0x00);
        public static Guid Button = new Guid(0xA36D02F0, 0xC9F3, 0x11CF, 0xBF, 0xC7, 0x44, 0x45, 0x53, 0x54, 0x00, 0x00);
        public static Guid POV    = new Guid(0xA36D02F2, 0xC9F3, 0x11CF, 0xBF, 0xC7, 0x44, 0x45, 0x53, 0x54, 0x00, 0x00);
        public static Guid RxAxis = new Guid(0xA36D02F4, 0xC9F3, 0x11CF, 0xBF, 0xC7, 0x44, 0x45, 0x53, 0x54, 0x00, 0x00);
        public static Guid RyAxis = new Guid(0xA36D02F5, 0xC9F3, 0x11CF, 0xBF, 0xC7, 0x44, 0x45, 0x53, 0x54, 0x00, 0x00);
    };
    
    public enum DIDF
        {
        ABSAXIS = 1,
        RELAXIS = 2,
        }

    public enum DI8DEVTYPE
        {
        DEVICE           = 0x11,
        MOUSE            = 0x12,
        KEYBOARD         = 0x13,
        JOYSTICK         = 0x14,
        GAMEPAD          = 0x15,
        DRIVING          = 0x16,
        FLIGHT           = 0x17,
        FIRSTPERSON      = 0x18,
        DEVICECTRL       = 0x19,
        SCREENPOINTER    = 0x1A,
        REMOTE           = 0x1B,
        SUPPLEMENTAL     = 0x1C,
        CLASS_ALL        = 0,
        CLASS_DEVICE     = 1,
        CLASS_POINTER    = 2,
        CLASS_KEYBOARD   = 3,
        CLASS_GAMECTRL   = 4,
        };

    public enum DIEDFL
        {
        ALLDEVICES       = 0x00000000,
        ATTACHEDONLY     = 0x00000001,
        FORCEFEEDBACK    = 0x00000100,
        INCLUDEALIASES   = 0x00010000,
        INCLUDEPHANTOMS  = 0x00020000,
        INCLUDEHIDDEN    = 0x00040000,
        }

    public enum DIJOFS : uint
    {
        ALL             = 0x00000000,
        RELAXIS         = 0x00000001,
        ABSAXIS         = 0x00000002,
        AXIS            = 0x00000003,
        PSHBUTTON       = 0x00000004,
        TGLBUTTON       = 0x00000008,
        BUTTON          = 0x0000000C,
        POV             = 0x00000010,
        COLLECTION      = 0x00000040,
        NODATA          = 0x00000080,
        ANYINSTANCE     = 0x00FFFF00,
        INSTANCEMASK    = ANYINSTANCE,
        FFACTUATOR      = 0x01000000,
        FFEFFECTTRIGGER = 0x02000000,
        OUTPUT          = 0x10000000,
        VENDORDEFINED   = 0x04000000,
        ALIAS           = 0x08000000,
        NOCOLLECTION    = 0x00FFFF00,
        MYSTERY         = 0x80000000,   // We seem to need this (and it's in c_dfDIJoystick2), but we don't know what it does
        OPTIONAL        = 0x80000000
    }

    public enum DIDOI
        {
        NONE              = 0x00000000,
        FFACTUATOR        = 0x00000001,
        FFEFFECTTRIGGER   = 0x00000002,
        POLLED            = 0x00008000,
        ASPECTPOSITION    = 0x00000100,
        ASPECTVELOCITY    = 0x00000200,
        ASPECTACCEL       = 0x00000300,
        ASPECTFORCE       = 0x00000400,
        ASPECTMASK        = 0x00000F00,
        GUIDISUSAGE       = 0x00010000,
        }

    public enum DIPROP
        {
        BUFFERSIZE       = 1,
        AXISMODE         = 2,
        GRANULARITY      = 3,
        RANGE            = 4,
        DEADZONE         = 5,
        SATURATION       = 6,
        FFGAIN           = 7,
        FFLOAD           = 8,
        AUTOCENTER       = 9,
        CALIBRATIONMODE  = 10,
        CALIBRATION      = 11,
        GUIDANDPATH      = 12,
        INSTANCENAME     = 13,
        PRODUCTNAME      = 14,
        JOYSTICKID       = 15,
        GETPORTDISPLAYNAME  = 16,
        PHYSICALRANGE       = 18,
        LOGICALRANGE        = 19,
        KEYNAME             = 20,
        CPOINTS             = 21,
        APPDATA          = 22,
        SCANCODE         = 23,
        VIDPID           = 24,
        USERNAME         = 25,
        TYPENAME         = 26,
        }

    public enum DIPH
        {
        DEVICE      = 0,
        BYOFFSET    = 1,
        BYID        = 2,
        BYUSAGE     = 3,
        }





    //const DIDATAFORMAT c_dfDIJoystick = {
    //    sizeof(DIDATAFORMAT),
    //    sizeof(DIOBJECTDATAFORMAT),
    //    DIDF_ABSAXIS,
    //    sizeof(DIJOYSTATE),
    //    ARRAY_SIZE(dfDIJoystick),
    //    (LPDIOBJECTDATAFORMAT)dfDIJoystick
    //};


    //------------------------------------------------------------------------------------------------
    // Classes
    //------------------------------------------------------------------------------------------------

    public class DirectInputDevice
    {
        public DIOBJECTDATAFORMAT[] dfDIJoystick = {

          new DIOBJECTDATAFORMAT(Guids.XAxis,DIJOYSTATE.DIJOFS.X, DIDFT.AXIS,0),
          new DIOBJECTDATAFORMAT(Guids.YAxis,DIJOYSTATE.DIJOFS.Y,DIDFT.OPTIONAL|DIDFT.AXIS|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guids.ZAxis,DIJOYSTATE.DIJOFS.Z,DIDFT.OPTIONAL|DIDFT.AXIS|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guids.RxAxis,DIJOYSTATE.DIJOFS.RX,DIDFT.OPTIONAL|DIDFT.AXIS|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guids.RyAxis,DIJOYSTATE.DIJOFS.RY,DIDFT.OPTIONAL|DIDFT.AXIS|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guids.RzAxis,DIJOYSTATE.DIJOFS.RZ,DIDFT.OPTIONAL|DIDFT.AXIS|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guids.Slider,DIJOYSTATE.DIJOFS.SLIDER0,DIDFT.OPTIONAL|DIDFT.AXIS|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guids.Slider,DIJOYSTATE.DIJOFS.SLIDER1,DIDFT.OPTIONAL|DIDFT.AXIS|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guids.POV,DIJOYSTATE.DIJOFS.POV0,DIDFT.OPTIONAL|DIDFT.POV|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guids.POV,DIJOYSTATE.DIJOFS.POV1,DIDFT.OPTIONAL|DIDFT.POV|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guids.POV,DIJOYSTATE.DIJOFS.POV2,DIDFT.OPTIONAL|DIDFT.POV|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guids.POV,DIJOYSTATE.DIJOFS.POV3,DIDFT.OPTIONAL|DIDFT.POV|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guids.Button,DIJOYSTATE.DIJOFS.BUTTON0,DIDFT.OPTIONAL|DIDFT.BUTTON|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guids.Button,DIJOYSTATE.DIJOFS.BUTTON1,DIDFT.OPTIONAL|DIDFT.BUTTON|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guids.Button,DIJOYSTATE.DIJOFS.BUTTON2,DIDFT.OPTIONAL|DIDFT.BUTTON|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guids.Button,DIJOYSTATE.DIJOFS.BUTTON3,DIDFT.OPTIONAL|DIDFT.BUTTON|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guids.Button,DIJOYSTATE.DIJOFS.BUTTON4,DIDFT.OPTIONAL|DIDFT.BUTTON|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guids.Button,DIJOYSTATE.DIJOFS.BUTTON5,DIDFT.OPTIONAL|DIDFT.BUTTON|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guids.Button,DIJOYSTATE.DIJOFS.BUTTON6,DIDFT.OPTIONAL|DIDFT.BUTTON|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guids.Button,DIJOYSTATE.DIJOFS.BUTTON7,DIDFT.OPTIONAL|DIDFT.BUTTON|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guids.Button,DIJOYSTATE.DIJOFS.BUTTON8,DIDFT.OPTIONAL|DIDFT.BUTTON|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guids.Button,DIJOYSTATE.DIJOFS.BUTTON9,DIDFT.OPTIONAL|DIDFT.BUTTON|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guids.Button,DIJOYSTATE.DIJOFS.BUTTON10,DIDFT.OPTIONAL|DIDFT.BUTTON|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guids.Button,DIJOYSTATE.DIJOFS.BUTTON11,DIDFT.OPTIONAL|DIDFT.PSHBUTTON|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guids.Button,DIJOYSTATE.DIJOFS.BUTTON12,DIDFT.OPTIONAL|DIDFT.TGLBUTTON|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guid.Empty,DIJOYSTATE.DIJOFS.BUTTON13,DIDFT.OPTIONAL|DIDFT.BUTTON|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guid.Empty,DIJOYSTATE.DIJOFS.BUTTON14,DIDFT.OPTIONAL|DIDFT.BUTTON|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guid.Empty,DIJOYSTATE.DIJOFS.BUTTON15,DIDFT.OPTIONAL|DIDFT.BUTTON|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guid.Empty,DIJOYSTATE.DIJOFS.BUTTON16,DIDFT.OPTIONAL|DIDFT.BUTTON|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guid.Empty,DIJOYSTATE.DIJOFS.BUTTON17,DIDFT.OPTIONAL|DIDFT.BUTTON|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guid.Empty,DIJOYSTATE.DIJOFS.BUTTON18,DIDFT.OPTIONAL|DIDFT.BUTTON|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guid.Empty,DIJOYSTATE.DIJOFS.BUTTON19,DIDFT.OPTIONAL|DIDFT.BUTTON|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guid.Empty,DIJOYSTATE.DIJOFS.BUTTON20,DIDFT.OPTIONAL|DIDFT.BUTTON|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guid.Empty,DIJOYSTATE.DIJOFS.BUTTON21,DIDFT.OPTIONAL|DIDFT.BUTTON|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guid.Empty,DIJOYSTATE.DIJOFS.BUTTON22,DIDFT.OPTIONAL|DIDFT.BUTTON|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guid.Empty,DIJOYSTATE.DIJOFS.BUTTON23,DIDFT.OPTIONAL|DIDFT.BUTTON|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guid.Empty,DIJOYSTATE.DIJOFS.BUTTON24,DIDFT.OPTIONAL|DIDFT.BUTTON|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guid.Empty,DIJOYSTATE.DIJOFS.BUTTON25,DIDFT.OPTIONAL|DIDFT.BUTTON|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guid.Empty,DIJOYSTATE.DIJOFS.BUTTON26,DIDFT.OPTIONAL|DIDFT.BUTTON|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guid.Empty,DIJOYSTATE.DIJOFS.BUTTON27,DIDFT.OPTIONAL|DIDFT.BUTTON|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guid.Empty,DIJOYSTATE.DIJOFS.BUTTON28,DIDFT.OPTIONAL|DIDFT.BUTTON|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guid.Empty,DIJOYSTATE.DIJOFS.BUTTON29,DIDFT.OPTIONAL|DIDFT.BUTTON|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guid.Empty,DIJOYSTATE.DIJOFS.BUTTON30,DIDFT.OPTIONAL|DIDFT.BUTTON|DIDFT.ANYINSTANCE,0),
          new DIOBJECTDATAFORMAT(Guid.Empty,DIJOYSTATE.DIJOFS.BUTTON31,DIDFT.OPTIONAL|DIDFT.BUTTON|DIDFT.ANYINSTANCE,0)
        };

        DIDATAFORMAT c_dfDIJoystick = new DIDATAFORMAT();

        //--------------------------------------------------------------------------------------------
        // State
        //--------------------------------------------------------------------------------------------

        IDirectInputDevice8W pInputDevice;

        //--------------------------------------------------------------------------------------------
        // Construction
        //--------------------------------------------------------------------------------------------

        public DirectInputDevice(IDirectInputDevice8W pInputDevice)
            {
            
            this.pInputDevice = pInputDevice;

            c_dfDIJoystick.dwSize = Marshal.SizeOf(typeof(DIDATAFORMAT));
            c_dfDIJoystick.dwObjSize = Marshal.SizeOf(typeof(DIOBJECTDATAFORMAT));
            c_dfDIJoystick.dwFlags = (int)DIDF.ABSAXIS;     // use DIDF_ABSAXIS
            c_dfDIJoystick.dwDataSize = Marshal.SizeOf(typeof(DIJOYSTATE));
            c_dfDIJoystick.dwNumObjs = dfDIJoystick.Length;
            
            IntPtr l_ptrDIJoystick = Marshal.AllocCoTaskMem(c_dfDIJoystick.dwObjSize * c_dfDIJoystick.dwNumObjs);
             
            c_dfDIJoystick.rgodf = l_ptrDIJoystick;

            for (int i = 0; i < c_dfDIJoystick.dwNumObjs; i++)
            {
                Marshal.StructureToPtr(dfDIJoystick[i], l_ptrDIJoystick, false);

                l_ptrDIJoystick += c_dfDIJoystick.dwObjSize;
            }
        }

        static int EnumObjectsCallback(ref DIDEVICEOBJECTINSTANCE pddoi, IntPtr pvRef)
            {
            GCHandle hResult = GCHandle.FromIntPtr(pvRef);
            List<DIDEVICEOBJECTINSTANCE> result = (List<DIDEVICEOBJECTINSTANCE>)(hResult.Target);
            result.Add(pddoi.Clone());
            return 1;
            }

        public List<DIDEVICEOBJECTINSTANCE> EnumObjects(DIDFT flags)
            {
            List<DIDEVICEOBJECTINSTANCE> result = new List<DIDEVICEOBJECTINSTANCE>();
            //
            DIEnumDeviceObjectsCallback callback = new DIEnumDeviceObjectsCallback(EnumObjectsCallback);
            GCHandle hCallback = GCHandle.Alloc(callback);
            GCHandle hResult   = GCHandle.Alloc(result);
            try {
                IntPtr pfn = Marshal.GetFunctionPointerForDelegate(callback);
                this.pInputDevice.EnumObjects(pfn, GCHandle.ToIntPtr(hResult), flags);
                }
            finally
                {
                hCallback.Free();
                hResult.Free();
                }
            //
            return result;
            }

        public void SetRange(DIDEVICEOBJECTINSTANCE o, int lowerRange, int upperRange)
            {
            DIPROPRANGE diproprange = new DIPROPRANGE();
            //
            diproprange.diph.dwSize       = Marshal.SizeOf(diproprange);
            diproprange.diph.dwHeaderSize = Marshal.SizeOf(diproprange.diph);
            diproprange.diph.dwObj        = o.dwType;
            diproprange.diph.dwHow        = (int)DIPH.BYID;
            diproprange.lMin              = lowerRange;
            diproprange.lMax              = upperRange;
            //
            this.pInputDevice.SetProperty((IntPtr)(DIPROP.RANGE), ref diproprange.diph);
            }

        public void setData()
        {
            SetDataFormat(ref c_dfDIJoystick);

            foreach (var lDIJoystick in dfDIJoystick)
            {
                if ((lDIJoystick.dwType & (int)DIDFT.AXIS) != 0)
                {
                    DIPROPRANGE diproprange = new DIPROPRANGE();
                    //
                    diproprange.diph.dwSize = Marshal.SizeOf(diproprange);
                    diproprange.diph.dwHeaderSize = Marshal.SizeOf(diproprange.diph);
                    diproprange.diph.dwObj = lDIJoystick.dwOfs;
                    diproprange.diph.dwHow = (int)DIPH.BYOFFSET;
                    diproprange.lMin = -((1 << 15) - 1);
                    diproprange.lMax = ((1 << 15) - 1);

                    try
                    {

                        this.pInputDevice.SetProperty((IntPtr)(DIPROP.RANGE), ref diproprange.diph);
                    }
                    catch (Exception)
                    {
                    }

                    //
                }
            }
        }

        public void SetDataFormat(ref DIDATAFORMAT pdf)
            {
            this.pInputDevice.SetDataFormat(pdf);
            }

        public void Acquire()   { Marshal.ThrowExceptionForHR(this.pInputDevice.Acquire());   }
        public void Unacquire() { Marshal.ThrowExceptionForHR(this.pInputDevice.Unacquire()); }
        public void Poll()      { Marshal.ThrowExceptionForHR(this.pInputDevice.Poll());      }
        public int GetDeviceState(int cbData, IntPtr data) {
            return this.pInputDevice.GetDeviceState(cbData, data);
        }
        }

    public class DirectInput
        {
        //--------------------------------------------------------------------------------------------
        // State
        //--------------------------------------------------------------------------------------------

        IDirectInput8W pDirectInput8;

        //--------------------------------------------------------------------------------------------
        // Construction
        //--------------------------------------------------------------------------------------------

        public DirectInput()
            {
            // Instantiate access to the main IDirectInput8 interface
            Guid   clsid = new Guid("25E609E4-B259-11CF-BFC7-444553540000");  // CLSID_DirectInput8
            Guid   iid   = (typeof(IDirectInput8W).GUID);
            object punk = null;
            int hr = Win32NativeMethods.CoCreateInstance(ref clsid, null, Win32NativeMethods.CLSCTX.INPROC_SERVER, ref iid, out punk);
            if (0==hr)
                {
                pDirectInput8 = (IDirectInput8W)punk;
                IntPtr hInstance = Win32NativeMethods.GetModuleHandleW(null);
                pDirectInput8.Initialize(hInstance, 0x800);
                }
            else
                Marshal.ThrowExceptionForHR(hr);
            }

        static int EnumDevicesCallback(ref DIDEVICEINSTANCEW pddi, IntPtr pvRef)
            {
            GCHandle hResult = GCHandle.FromIntPtr(pvRef);
            List<DIDEVICEINSTANCEW> result = (List<DIDEVICEINSTANCEW>)(hResult.Target);
            result.Add(pddi.Clone());
            return 1;
            }

        public List<DIDEVICEINSTANCEW> EnumDevices(DI8DEVTYPE deviceType, DIEDFL flags)
            {
            List<DIDEVICEINSTANCEW> result = new List<DIDEVICEINSTANCEW>();
            //
            LPDIENUMDEVICESCALLBACK callback = new LPDIENUMDEVICESCALLBACK(DirectInput.EnumDevicesCallback);
            GCHandle hCallback = GCHandle.Alloc(callback);
            GCHandle hResult   = GCHandle.Alloc(result);
            try {
                IntPtr pfn = Marshal.GetFunctionPointerForDelegate(callback);
                this.pDirectInput8.EnumDevices(deviceType, pfn, GCHandle.ToIntPtr(hResult), flags);
                }
            finally
                {
                hCallback.Free();
                hResult.Free();
                }
            //
            return result;
            }

        public DirectInputDevice CreateDevice(Guid guid)
            {
            IDirectInputDevice8W pInputDevice;
            this.pDirectInput8.CreateDevice(ref guid, out pInputDevice, null);
            return new DirectInputDevice(pInputDevice);
            }
        }
    }
