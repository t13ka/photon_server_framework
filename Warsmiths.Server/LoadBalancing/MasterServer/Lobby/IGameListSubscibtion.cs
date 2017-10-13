using System;
using System.Collections;

namespace Warsmiths.Server.MasterServer.Lobby
{
    public interface IGameListSubscibtion : IDisposable
    {
        Hashtable GetGameList();
    }
}
