// ----------------------------------------------------------------------------------------------------------------------
// <summary>The Photon Chat Api enables clients to connect to a chat server and communicate with other clients.</summary>
// <remarks>ChatClient is the main class of this api.</remarks>
// ----------------------------------------------------------------------------------------------------------------------

namespace YourGame.Client.ChatApi
{
    using System.Collections.Generic;

    /// <summary>
    ///     A channel of communication in Photon Chat, updated by ChatClient and provided as READ ONLY.
    /// </summary>
    /// <remarks>
    ///     Contains messages and senders to use (read!) and display by your GUI.
    ///     Access these by:
    ///     ChatClient.PublicChannels
    ///     ChatClient.PrivateChannels
    /// </remarks>
    public class ChatChannel
    {
        /// <summary>
        ///     Messages in chronoligical order. Senders and Messages refer to each other by index. Senders[x] is the sender
        ///     of Messages[x].
        /// </summary>
        public readonly List<object> Messages = new List<object>();

        /// <summary>Name of the channel (used to subscribe and unsubscribe).</summary>
        public readonly string Name;

        /// <summary>
        ///     Senders of messages in chronoligical order. Senders and Messages refer to each other by index. Senders[x] is
        ///     the sender of Messages[x].
        /// </summary>
        public readonly List<string> Senders = new List<string>();

        /// <summary>
        ///     Used internally to create new channels. This does NOT create a channel on the server! Use
        ///     ChatClient.Subscribe.
        /// </summary>
        public ChatChannel(string name)
        {
            Name = name;
        }

        /// <summary>Is this a private 1:1 channel?</summary>
        public bool IsPrivate { get; protected internal set; }

        /// <summary>Count of messages this client still buffers/knows for this channel.</summary>
        public int MessageCount => Messages.Count;

        /// <summary>Used internally to add messages to this channel.</summary>
        public void Add(string sender, object message)
        {
            Senders.Add(sender);
            Messages.Add(message);
        }

        /// <summary>Used internally to add messages to this channel.</summary>
        public void Add(string[] senders, object[] messages)
        {
            Senders.AddRange(senders);
            Messages.AddRange(messages);
        }

        /// <summary>Clear the local cache of messages currently stored. This frees memory but doesn't affect the server.</summary>
        public void ClearMessages()
        {
            Senders.Clear();
            Messages.Clear();
        }
    }
}