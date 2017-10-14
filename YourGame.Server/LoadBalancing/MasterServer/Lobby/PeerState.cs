using System;

namespace YourGame.Server.MasterServer.Lobby
{
    /// <summary>
    ///     Stores properties of peers which have joined a game.
    /// </summary>
    /// <remarks>
    ///     To avoid storing a reference to a peer object this class creates a copy of the
    ///     peers UserId and BlockedUsers properties.
    ///     This properties are needed for match making even if the peer has disconnected to
    ///     join the game on the game server instance.
    ///     if a reference to the peer object would be stored the garbage collector cannot
    ///     remove the peer object from memory and references to the native socket server
    ///     objects cannot be disposed.
    /// </remarks>
    public class PeerState
    {
        public readonly string UserId;
        public readonly DateTime UtcCreated;

        public PeerState(string userId)
        {
            UtcCreated = DateTime.UtcNow;
            UserId = userId;
        }

        public PeerState(ILobbyPeer peer)
        {
            UserId = peer.UserId ?? string.Empty;
            UtcCreated = DateTime.UtcNow;
        }
    }
}