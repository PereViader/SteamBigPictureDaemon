using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace SteamBigPictureDaemon
{
    public static class User32
    {
        public static class Misc
        {
            internal static void ThrowWin32ExceptionsIfError(int errorCode)
            {
                switch (errorCode)
                {
                    case 0: //    0 ERROR_SUCCESS                   The operation completed successfully.
                        // The error code indicates that there is no error, so do not throw an exception.
                        break;

                    case 6:    //    6 ERROR_INVALID_HANDLE            The handle is invalid.
                    case 1400: // 1400 ERROR_INVALID_WINDOW_HANDLE     Invalid window handle.
                    case 1401: // 1401 ERROR_INVALID_MENU_HANDLE       Invalid menu handle.
                    case 1402: // 1402 ERROR_INVALID_CURSOR_HANDLE     Invalid cursor handle.
                    case 1403: // 1403 ERROR_INVALID_ACCEL_HANDLE      Invalid accelerator table handle.
                    case 1404: // 1404 ERROR_INVALID_HOOK_HANDLE       Invalid hook handle.
                    case 1405: // 1405 ERROR_INVALID_DWP_HANDLE        Invalid handle to a multiple-window position structure.
                    case 1406: // 1406 ERROR_TLW_WITH_WSCHILD          Cannot create a top-level child window.
                    case 1407: // 1407 ERROR_CANNOT_FIND_WND_CLASS     Cannot find window class.
                    case 1408: // 1408 ERROR_WINDOW_OF_OTHER_THREAD    Invalid window; it belongs to other thread.
                        throw new ElementNotAvailableException();

                    // We're getting this in AMD64 when calling RealGetWindowClass; adding this code
                    // to allow the DRTs to pass while we continue investigation.
                    case 87: //   87 ERROR_INVALID_PARAMETER
                        throw new ElementNotAvailableException();


                    case 8:  //    8 ERROR_NOT_ENOUGH_MEMORY         Not enough storage is available to process this command.
                    case 14: //   14 ERROR_OUTOFMEMORY               Not enough storage is available to complete this operation.
                        throw new OutOfMemoryException();

                    case 998: //  998 ERROR_NOACCESS                  Invalid access to memory location.
                        throw new InvalidOperationException();

                    default:
                        // Not sure how to map the reset of the error codes so throw generic Win32Exception.
                        throw new Win32Exception(errorCode);
                }
            }

            public class ElementNotAvailableException : Exception
            {
            }
        }

        public static class NativeMethods
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct POINTL
            {
                [MarshalAs(UnmanagedType.I4)]
                public int x;
                [MarshalAs(UnmanagedType.I4)]
                public int y;
            }

            [StructLayout(LayoutKind.Sequential,
    CharSet = CharSet.Ansi)]
            public struct DEVMODE
            {
                // You can define the following constant
                // but OUTSIDE the structure because you know
                // that size and layout of the structure
                // is very important
                // CCHDEVICENAME = 32 = 0x50
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
                public string dmDeviceName;
                // In addition you can define the last character array
                // as following:
                //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
                //public Char[] dmDeviceName;

                // After the 32-bytes array
                [MarshalAs(UnmanagedType.U2)]
                public UInt16 dmSpecVersion;

                [MarshalAs(UnmanagedType.U2)]
                public UInt16 dmDriverVersion;

                [MarshalAs(UnmanagedType.U2)]
                public UInt16 dmSize;

                [MarshalAs(UnmanagedType.U2)]
                public UInt16 dmDriverExtra;

                [MarshalAs(UnmanagedType.U4)]
                public UInt32 dmFields;

                public POINTL dmPosition;

                [MarshalAs(UnmanagedType.U4)]
                public UInt32 dmDisplayOrientation;

                [MarshalAs(UnmanagedType.U4)]
                public UInt32 dmDisplayFixedOutput;

                [MarshalAs(UnmanagedType.I2)]
                public Int16 dmColor;

                [MarshalAs(UnmanagedType.I2)]
                public Int16 dmDuplex;

                [MarshalAs(UnmanagedType.I2)]
                public Int16 dmYResolution;

                [MarshalAs(UnmanagedType.I2)]
                public Int16 dmTTOption;

                [MarshalAs(UnmanagedType.I2)]
                public Int16 dmCollate;

                // CCHDEVICENAME = 32 = 0x50
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
                public string dmFormName;
                // Also can be defined as
                //[MarshalAs(UnmanagedType.ByValArray,
                //    SizeConst = 32, ArraySubType = UnmanagedType.U1)]
                //public Byte[] dmFormName;

                [MarshalAs(UnmanagedType.U2)]
                public UInt16 dmLogPixels;

                [MarshalAs(UnmanagedType.U4)]
                public UInt32 dmBitsPerPel;

                [MarshalAs(UnmanagedType.U4)]
                public UInt32 dmPelsWidth;

                [MarshalAs(UnmanagedType.U4)]
                public UInt32 dmPelsHeight;

                [MarshalAs(UnmanagedType.U4)]
                public UInt32 dmDisplayFlags;

                [MarshalAs(UnmanagedType.U4)]
                public UInt32 dmDisplayFrequency;

                [MarshalAs(UnmanagedType.U4)]
                public UInt32 dmICMMethod;

                [MarshalAs(UnmanagedType.U4)]
                public UInt32 dmICMIntent;

                [MarshalAs(UnmanagedType.U4)]
                public UInt32 dmMediaType;

                [MarshalAs(UnmanagedType.U4)]
                public UInt32 dmDitherType;

                [MarshalAs(UnmanagedType.U4)]
                public UInt32 dmReserved1;

                [MarshalAs(UnmanagedType.U4)]
                public UInt32 dmReserved2;

                [MarshalAs(UnmanagedType.U4)]
                public UInt32 dmPanningWidth;

                [MarshalAs(UnmanagedType.U4)]
                public UInt32 dmPanningHeight;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct HWND : IEquatable<HWND>
            {
                public IntPtr h;

                public static HWND Cast(IntPtr h)
                {
                    HWND hTemp = new HWND();
                    hTemp.h = h;
                    return hTemp;
                }

                public static implicit operator IntPtr(HWND h)
                {
                    return h.h;
                }

                public static implicit operator HWND(IntPtr intPtr)
                {
                    return Cast(intPtr);
                }

                public static HWND NULL
                {
                    get
                    {
                        HWND hTemp = new HWND();
                        hTemp.h = IntPtr.Zero;
                        return hTemp;
                    }
                }

                public static bool operator ==(HWND hl, HWND hr)
                {
                    return hl.h == hr.h;
                }

                public static bool operator !=(HWND hl, HWND hr)
                {
                    return hl.h != hr.h;
                }

                override public bool Equals(object oCompare)
                {
                    return oCompare is HWND other && Equals(other);
                }

                public override int GetHashCode()
                {
                    return h.GetHashCode();
                }

                public bool Equals(HWND other)
                {
                    return h.Equals(other.h);
                }
            }


            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
            public static extern IntPtr GetWindowThreadProcessId([In] HWND hWnd, [Out] out uint ProcessId);

            [DllImport("user32.dll", CharSet = CharSet.Ansi)]
            public static extern HWND GetForegroundWindow();

            [DllImport("user32", EntryPoint = "GetWindowTextA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
            public static extern int GetWindowText(HWND hwnd, StringBuilder lpString, int cch);
            [DllImport("user32", EntryPoint = "GetWindowTextLengthA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
            public static extern int GetWindowTextLength(HWND hwnd);

            [DllImport("user32", EntryPoint = "GetClassNameA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
            public static extern int GetClassName(HWND hWnd, StringBuilder text, int count);

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern bool IsWindow(HWND hwnd);

            public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, HWND hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

            [DllImport("user32.dll")]
            public static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

            [DllImport("User32.dll")]
            [return: MarshalAs(UnmanagedType.I4)]
            public static extern int ChangeDisplaySettings(
                [In, Out]
                ref DEVMODE lpDevMode,
                [param: MarshalAs(UnmanagedType.U4)]
                uint dwflags
                );

            [DllImport("User32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern Boolean EnumDisplaySettings(
                [param: MarshalAs(UnmanagedType.LPTStr)]
                string lpszDeviceName,
                [param: MarshalAs(UnmanagedType.U4)]
                int iModeNum,
                [In, Out]
                ref DEVMODE lpDevMode);

            internal const uint WINEVENT_OUTOFCONTEXT = 0;
            internal const int EVENT_OBJECT_DESTROY = 0x8001;
            internal const uint EVENT_SYSTEM_FOREGROUND = 0x0003;
            internal const uint EVENT_SYSTEM_MINIMIZEEND = 0x0017;
            internal const int MAX_PATH = 260;


            //
            // IAccessible / OLEACC / WinEvents
            //

            public const int CHILDID_SELF = 0;
            public const int STATE_SYSTEM_UNAVAILABLE = 0x00000001;
            public const int STATE_SYSTEM_FOCUSED = 0x00000004;
            public const int OBJID_CARET = -8;
            public const int OBJID_CLIENT = -4;
            public const int OBJID_MENU = -3;
            public const int OBJID_SYSMENU = -1;
            public const int OBJID_WINDOW = 0;
        }

        public static uint ForegroundProcessId
        {
            get
            {
                var activeWindowHandle = NativeMethods.GetForegroundWindow();
                NativeMethods.GetWindowThreadProcessId(activeWindowHandle, out var processId);
                return processId;
            }
        }

        /// <summary>
        /// Get text of the window
        /// </summary>
        public static bool TryGetWindowText(NativeMethods.HWND window, out string text)
        {
            var length = NativeMethods.GetWindowTextLength(window) + 1;
            var sb = new StringBuilder(length);
            var result = NativeMethods.GetWindowText(window, sb, NativeMethods.MAX_PATH);
            var lastWin32Error = Marshal.GetLastWin32Error();

            if (result == 0)
            {
                text = default;
                return false;
            }

            text = sb.ToString();
            return true;
        }

        /// <summary>
        /// Get class of the window
        /// </summary>
        public static bool TryGetWindowClass(NativeMethods.HWND window, out string text)
        {
            var sb = new StringBuilder(NativeMethods.MAX_PATH);
            var result = NativeMethods.GetClassName(window, sb, NativeMethods.MAX_PATH);
            var lastWin32Error = Marshal.GetLastWin32Error();

            if (result == 0)
            {
                text = default;
                return false;
            }

            text = sb.ToString();
            return true;
        }
    }
}
