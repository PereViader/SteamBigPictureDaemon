using System;
using WinEventHook;

namespace SteamBigPictureDaemon
{
    public class BigPictureProcessHook : IProcessHook, IDisposable
    {
        public event Action OnStart;
        public event Action OnEnd;

        private readonly WindowEventHook windowCreatedEventHook = new WindowEventHook(WindowEvent.EVENT_OBJECT_SHOW);
        private readonly WindowEventHook windowDestroyedEventHook = new(WindowEvent.EVENT_OBJECT_DESTROY);

        private readonly ReentrancySafeEventProcessor<WinEventHookEventArgs> windowCreatedSafe;
        private readonly ReentrancySafeEventProcessor<WinEventHookEventArgs> windowDestroyedSafe;

        private bool isHooked;

        private bool disposedValue;

        public BigPictureProcessHook()
        {
            windowCreatedSafe = new(HandleWindowCreated);
            windowDestroyedSafe = new(HandleWindowDestroyed);

            windowCreatedEventHook.EventReceived += WindowCreated;
            windowDestroyedEventHook.EventReceived += WindowDestroyed;

            windowCreatedEventHook.HookGlobal();
        }

        private void WindowCreated(object sender, WinEventHookEventArgs e)
        {
            if (isHooked)
            {
                return;
            }

            if (!IsWindowEvent(e))
            {
                return;
            }

            windowCreatedSafe.EnqueueAndProcess(e);
        }

        private void WindowDestroyed(object sender, WinEventHookEventArgs e)
        {
            if (!isHooked)
            {
                return;
            }

            if (!IsWindowEvent(e))
            {
                return;
            }

            windowDestroyedSafe.EnqueueAndProcess(e);
        }

        private static bool IsWindowEvent(WinEventHookEventArgs eventArgs)
        {
            return eventArgs.ObjectId == AccessibleObjectID.OBJID_WINDOW && eventArgs.IsOwnEvent;
        }

        public void HandleWindowCreated(WinEventHookEventArgs winEventHookEventArgs)
        {
            if (!IsBigPicture(winEventHookEventArgs))
            {
                return;
            }

            windowCreatedSafe.FlushQueue();

            isHooked = true;
            OnStart?.Invoke();

            windowCreatedEventHook.Unhook();

            User32.NativeMethods.GetWindowThreadProcessId(winEventHookEventArgs.WindowHandle, out uint processId);
            windowDestroyedEventHook.HookToProcess(processId);
        }

        public void HandleWindowDestroyed(WinEventHookEventArgs winEventHookEventArgs)
        {
            if (!isHooked)
            {
                return;
            }

            if (!IsBigPicture(winEventHookEventArgs))
            {
                return;
            }

            windowDestroyedSafe.FlushQueue();

            windowDestroyedEventHook.Unhook();
            windowCreatedEventHook.HookGlobal();

            isHooked = false;
            OnEnd?.Invoke();
        }

        private bool IsBigPicture(WinEventHookEventArgs winEventHookEventArgs)
        {
            if (!User32.TryGetWindowText(winEventHookEventArgs.WindowHandle, out var title))
            {
                return false;
            }

            if (!User32.TryGetWindowClass(winEventHookEventArgs.WindowHandle, out var windowClass))
            {
                return false;
            }

            return title.Equals("Steam") && windowClass.Equals("CUIEngineWin32");
        }

        public void Dispose()
        {
            if (!disposedValue)
            {
                disposedValue = true;

                windowCreatedEventHook.Dispose();
                windowDestroyedEventHook.Dispose();
            }
        }
    }
}
