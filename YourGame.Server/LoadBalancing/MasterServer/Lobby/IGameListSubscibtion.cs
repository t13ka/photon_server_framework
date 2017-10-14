using System;
using System.Collections;

namespace YourGame.Server.MasterServer.Lobby
{
    public interface IGameListSubscibtion : IDisposable
    {
        Hashtable GetGameList();
    }
}
