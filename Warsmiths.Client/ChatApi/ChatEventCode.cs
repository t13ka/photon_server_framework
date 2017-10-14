// ----------------------------------------------------------------------------------------------------------------------
// <summary>The Photon Chat Api enables clients to connect to a chat server and communicate with other clients.</summary>
// <remarks>ChatClient is the main class of this api.</remarks>
// ----------------------------------------------------------------------------------------------------------------------

namespace YourGame.Client.ChatApi
{
    /// <summary>
    /// Wraps up internally used constants in Photon Chat events. You don't have to use them directly usually.
    /// </summary>
    public class ChatEventCode
    {
        public const byte ChatMessages = 0;
        public const byte Users = 1; // List of users or List of changes for List of users
        public const byte PrivateMessage = 2;
        public const byte FriendsList = 3;
        public const byte StatusUpdate = 4;
        public const byte Subscribe = 5;
        public const byte Unsubscribe = 6;
    }
}