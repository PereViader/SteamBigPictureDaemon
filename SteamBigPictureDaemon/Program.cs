using System.Runtime.InteropServices;

namespace SteamBigPictureDaemon
{
    class Program
    {
        static void Main(string[] args)
        {
            User32.NativeMethods.DEVMODE originalScreenMode = new();
            originalScreenMode.dmSize = (ushort)Marshal.SizeOf(originalScreenMode);

            if (!User32.NativeMethods.EnumDisplaySettings(null, -1, ref originalScreenMode))
            {
                return;
            }

            User32.NativeMethods.DEVMODE changedScreenMode = originalScreenMode;
            changedScreenMode.dmPelsHeight = 1080;
            changedScreenMode.dmPelsWidth = 1920;

            User32.NativeMethods.ChangeDisplaySettings(ref changedScreenMode, 0);

            System.Console.ReadKey();

            User32.NativeMethods.ChangeDisplaySettings(ref originalScreenMode, 0);
        }

        //static void Main(string[] args)
        //{
        //    User32.NativeMethods.DEVMODE originalScreenMode = new();
        //    originalScreenMode.dmSize = (ushort)Marshal.SizeOf(originalScreenMode);

        //    if (!User32.NativeMethods.EnumDisplaySettings(null, -1, ref originalScreenMode))
        //    {
        //        return;
        //    }

        //    User32.NativeMethods.DEVMODE changedScreenMode = originalScreenMode;
        //    changedScreenMode.dmPelsHeight = 1080;
        //    changedScreenMode.dmPelsWidth = 1920;

        //    using BigPictureProcessHook processHook = new();

        //    processHook.OnStart += () =>
        //    {
        //        System.Console.WriteLine("Enter");
        //        User32.NativeMethods.ChangeDisplaySettings(ref changedScreenMode, 0);
        //    };

        //    processHook.OnEnd += () =>
        //    {
        //        System.Console.WriteLine("Exit");
        //        User32.NativeMethods.ChangeDisplaySettings(ref originalScreenMode, 0);
        //    };
        //}
    }
}
