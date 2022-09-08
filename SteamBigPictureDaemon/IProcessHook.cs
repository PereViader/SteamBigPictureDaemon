using System;

namespace SteamBigPictureDaemon
{
    public interface IProcessHook
    {
        event Action OnStart;
        event Action OnEnd;
    }
}
