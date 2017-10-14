// ----------------------------------------------------------------------------------------------------------------------
// <summary>The Photon Chat Api enables clients to connect to a chat server and communicate with other clients.</summary>
// <remarks>ChatClient is the main class of this api.</remarks>
// ----------------------------------------------------------------------------------------------------------------------

namespace YourGame.Client.ChatApi
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using ExitGames.Client.Photon;

    /// <summary>Central class of the Photon Chat API to connect, handle channels and messages.</summary>
    /// <remarks>
    ///     This class must be instantiated with a IChatClientListener instance to get the callbacks.
    ///     Integrate it into your game loop by calling Service regularly.
    ///     Call Connect with an AppId that is setup as Photon Chat application. Note: Connect covers multiple
    ///     messages between this client and the servers. A short workflow will connect you to a chat server.
    ///     Each ChatClient resembles a user in chat (set in Connect). Each user automatically subscribes a channel
    ///     for incoming private messages and can message any other user privately.
    ///     Before you publish messages in any non-private channel, that channel must be subscribed.
    ///     PublicChannels is a list of subscribed channels, containing messages and senders.
    ///     PrivateChannels contains all incoming and sent private messages.
    /// </remarks>
    public class ChatClient : IPhotonPeerListener
    {
        /// <summary>The address of last connected Name Server.</summary>
        public string NameServerAddress { get; private set; }

        /// <summary>The address of the actual chat server assigned from NameServer. Public for read only.</summary>
        public string FrontendAddress { get; private set; }

        /// <summary>
        ///     Region used to connect to. Currently all chat is done in EU. It can make sense to use only one region for the
        ///     whole game.
        /// </summary>
        private string chatRegion = "EU";

        /// <summary>Settable only before you connect! Defaults to "EU".</summary>
        public string ChatRegion
        {
            get => chatRegion;
            set => chatRegion = value;
        }

        /// <summary>Current state of the ChatClient. Also use CanChat.</summary>
        public ChatState State { get; private set; }

        public ChatDisconnectCause DisconnectedCause { get; private set; }

        public bool CanChat => State == ChatState.ConnectedToFrontEnd && HasPeer;

        private bool HasPeer => chatPeer != null;

        /// <summary>
        ///     The version of your client. A new version also creates a new "virtual app" to separate players from older
        ///     client versions.
        /// </summary>
        public string AppVersion { get; private set; }

        /// <summary>
        ///     The AppID as assigned from the Photon Cloud. If you host yourself, this is the "regular" Photon Server
        ///     Application Name (most likely: "LoadBalancing").
        /// </summary>
        public string AppId { get; private set; }

        /// <summary>Settable only before you connect!</summary>
        public AuthenticationValues AuthValues;

        /// <summary>The unique ID of a user/person, stored in AuthValues.UserId. Set it before you connect.</summary>
        /// <remarks>
        ///     This value wraps AuthValues.UserId.
        ///     It's not a nickname and we assume users with the same userID are the same person.
        /// </remarks>
        public string UserId
        {
            get => AuthValues != null ? AuthValues.UserId : null;
            private set
            {
                if (AuthValues == null) AuthValues = new AuthenticationValues();
                AuthValues.UserId = value;
            }
        }

        public readonly Dictionary<string, ChatChannel> PublicChannels;

        public readonly Dictionary<string, ChatChannel> PrivateChannels;

        private readonly IChatClientListener listener;

        private readonly ChatPeer chatPeer;

        private bool didAuthenticate;

        private int msDeltaForServiceCalls = 50;

        private int msTimestampOfLastServiceCall;

        private const string ChatApppName = "chat";

        public ChatClient(
            IChatClientListener listener,
            ConnectionProtocol protocol =
#if UNITY_WEBGL
    ConnectionProtocol.WebSocketSecure
#else
                ConnectionProtocol.Udp
#endif
        )
        {
#if UNITY_WEBGL
	        if (protocol != ConnectionProtocol.WebSocket && protocol != ConnectionProtocol.WebSocketSecure) {
				UnityEngine.Debug.Log("WebGL only supports WebSocket protocol. Overriding ChatClient.Connect() 'protocol' parameter");
				protocol = ConnectionProtocol.WebSocketSecure;
			}
#endif

            this.listener = listener;
            State = ChatState.Uninitialized;

            chatPeer = new ChatPeer(this, protocol);

            PublicChannels = new Dictionary<string, ChatChannel>();
            PrivateChannels = new Dictionary<string, ChatChannel>();
        }

        public bool Connect(string appId, string appVersion, AuthenticationValues authValues)
        {
            chatPeer.TimePingInterval = 3000;
            DisconnectedCause = ChatDisconnectCause.None;

            AuthValues = authValues;
            AppId = appId;
            AppVersion = appVersion;
            didAuthenticate = false;
            msDeltaForServiceCalls = 100;

            // clean all channels
            PublicChannels.Clear();
            PrivateChannels.Clear();

            NameServerAddress = chatPeer.NameServerAddress;
            var isConnecting = chatPeer.Connect();
            if (isConnecting) State = ChatState.ConnectingToNameServer;
            return isConnecting;
        }

        /// <summary>
        ///     Must be called regularly to keep connection between client and server alive and to process incoming messages.
        /// </summary>
        /// <remarks>
        ///     This method limits the effort it does automatically using the private variable msDeltaForServiceCalls.
        ///     That value is lower for connect and multiplied by 4 when chat-server connection is ready.
        /// </remarks>
        public void Service()
        {
            if (HasPeer && (Environment.TickCount - msTimestampOfLastServiceCall > msDeltaForServiceCalls
                            || msTimestampOfLastServiceCall == 0))
            {
                msTimestampOfLastServiceCall = Environment.TickCount;
                chatPeer.Service();

                // TODO: make sure to call service regularly. in best case it could be integrated into PhotonHandler.FallbackSendAckThread()!
            }
        }

        public void Disconnect()
        {
            if (HasPeer && chatPeer.PeerState != PeerStateValue.Disconnected) chatPeer.Disconnect();
        }

        public void StopThread()
        {
            if (HasPeer) chatPeer.StopThread();
        }

        /// <summary>Sends operation to subscribe to a list of channels by name.</summary>
        /// <param name="channels">List of channels to subscribe to. Avoid null or empty values.</param>
        /// <returns>If the operation could be sent at all (Example: Fails if not connected to Chat Server).</returns>
        public bool Subscribe(string[] channels)
        {
            return Subscribe(channels, 0);
        }

        /// <summary>
        ///     Sends operation to subscribe client to channels, optionally fetching a number of messages from the cache.
        /// </summary>
        /// <remarks>
        ///     Subscribes channels will forward new messages to this user. Use PublishMessage to do so.
        ///     The messages cache is limited but can be useful to get into ongoing conversations, if that's needed.
        /// </remarks>
        /// <param name="channels">List of channels to subscribe to. Avoid null or empty values.</param>
        /// <param name="messagesFromHistory">
        ///     0: no history. 1 and higher: number of messages in history. -1: all available
        ///     history.
        /// </param>
        /// <returns>If the operation could be sent at all (Example: Fails if not connected to Chat Server).</returns>
        public bool Subscribe(string[] channels, int messagesFromHistory)
        {
            if (!CanChat) return false;

            if (channels == null || channels.Length == 0)
            {
                listener.DebugReturn(DebugLevel.WARNING, "Subscribe can't be called for empty or null channels-list.");
                return false;
            }

            return SendChannelOperation(channels, ChatOperationCode.Subscribe, messagesFromHistory);
        }

        /// <summary>Unsubscribes from a list of channels, which stops getting messages from those.</summary>
        /// <remarks>
        ///     The client will remove these channels from the PublicChannels dictionary once the server sent a response to this
        ///     request.
        ///     The request will be sent to the server and IChatClientListener.OnUnsubscribed gets called when the server
        ///     actually removed the channel subscriptions.
        ///     Unsubscribe will fail if you include null or empty channel names.
        /// </remarks>
        /// <param name="channels">Names of channels to unsubscribe.</param>
        /// <returns>False, if not connected to a chat server.</returns>
        public bool Unsubscribe(string[] channels)
        {
            if (!CanChat) return false;

            if (channels == null || channels.Length == 0)
            {
                listener.DebugReturn(
                    DebugLevel.WARNING,
                    "Unsubscribe can't be called for empty or null channels-list.");
                return false;
            }

            return SendChannelOperation(channels, ChatOperationCode.Unsubscribe, 0);
        }

        /// <summary>Sends a message to a public channel which this client subscribed to.</summary>
        /// <remarks>
        ///     Before you publish to a channel, you have to subscribe it.
        ///     Everyone in that channel will get the message.
        /// </remarks>
        /// <param name="channelName">Name of the channel to publish to.</param>
        /// <param name="message">Your message (string or any serializable data).</param>
        /// <returns>False if the client is not yet ready to send messages.</returns>
        public bool PublishMessage(string channelName, object message)
        {
            if (!CanChat) return false;

            if (string.IsNullOrEmpty(channelName) || message == null)
            {
                listener.DebugReturn(DebugLevel.WARNING, "PublishMessage parameters must be non-null and not empty.");
                return false;
            }

            var parameters = new Dictionary<byte, object>
                                 {
                                     { ChatParameterCode.Channel, channelName },
                                     { ChatParameterCode.Message, message }
                                 };

            return chatPeer.OpCustom(ChatOperationCode.Publish, parameters, true);
        }

        /// <summary>
        ///     Sends a private message to a single target user. Calls OnPrivateMessage on the receiving client.
        /// </summary>
        /// <param name="target">Username to send this message to.</param>
        /// <param name="message">The message you want to send. Can be a simple string or anything serializable.</param>
        /// <returns>True if this clients can send the message to the server.</returns>
        public bool SendPrivateMessage(string target, object message)
        {
            return SendPrivateMessage(target, message, false);
        }

        /// <summary>
        ///     Sends a private message to a single target user. Calls OnPrivateMessage on the receiving client.
        /// </summary>
        /// <param name="target">Username to send this message to.</param>
        /// <param name="message">The message you want to send. Can be a simple string or anything serializable.</param>
        /// <param name="encrypt">
        ///     Optionally, private messages can be encrypted. Encryption is not end-to-end as the server
        ///     decrypts the message.
        /// </param>
        /// <returns>True if this clients can send the message to the server.</returns>
        public bool SendPrivateMessage(string target, object message, bool encrypt)
        {
            if (!CanChat) return false;

            if (string.IsNullOrEmpty(target) || message == null)
            {
                listener.DebugReturn(
                    DebugLevel.WARNING,
                    "SendPrivateMessage parameters must be non-null and not empty.");
                return false;
            }

            var parameters = new Dictionary<byte, object>
                                 {
                                     { ChatParameterCode.UserId, target },
                                     { ChatParameterCode.Message, message }
                                 };

            var sent = chatPeer.OpCustom(ChatOperationCode.SendPrivate, parameters, true, 0, encrypt);
            return sent;
        }

        /// <summary>Sets the user's status (pre-defined or custom) and an optional message.</summary>
        /// <remarks>
        ///     The predefined status values can be found in class ChatUserStatus.
        ///     State ChatUserStatus.Invisible will make you offline for everyone and send no message.
        ///     You can set custom values in the status integer. Aside from the pre-configured ones,
        ///     all states will be considered visible and online. Else, no one would see the custom state.
        ///     The message object can be anything that Photon can serialize, including (but not limited to)
        ///     Hashtable, object[] and string. This value is defined by your own conventions.
        /// </remarks>
        /// <param name="status">Predefined states are in class ChatUserStatus. Other values can be used at will.</param>
        /// <param name="message">Optional string message or null.</param>
        /// <param name="skipMessage">If true, the message gets ignored. It can be null but won't replace any current message.</param>
        /// <returns>True if the operation gets called on the server.</returns>
        private bool SetOnlineStatus(int status, object message, bool skipMessage)
        {
            if (!CanChat) return false;

            var parameters = new Dictionary<byte, object> { { ChatParameterCode.Status, status } };

            if (skipMessage) parameters[ChatParameterCode.SkipMessage] = true;
            else parameters[ChatParameterCode.Message] = message;
            return chatPeer.OpCustom(ChatOperationCode.UpdateStatus, parameters, true);
        }

        /// <summary>Sets the user's status without changing your status-message.</summary>
        /// <remarks>
        ///     The predefined status values can be found in class ChatUserStatus.
        ///     State ChatUserStatus.Invisible will make you offline for everyone and send no message.
        ///     You can set custom values in the status integer. Aside from the pre-configured ones,
        ///     all states will be considered visible and online. Else, no one would see the custom state.
        ///     This overload does not change the set message.
        /// </remarks>
        /// <param name="status">Predefined states are in class ChatUserStatus. Other values can be used at will.</param>
        /// <returns>True if the operation gets called on the server.</returns>
        public bool SetOnlineStatus(int status)
        {
            return SetOnlineStatus(status, null, true);
        }

        /// <summary>Sets the user's status without changing your status-message.</summary>
        /// <remarks>
        ///     The predefined status values can be found in class ChatUserStatus.
        ///     State ChatUserStatus.Invisible will make you offline for everyone and send no message.
        ///     You can set custom values in the status integer. Aside from the pre-configured ones,
        ///     all states will be considered visible and online. Else, no one would see the custom state.
        ///     The message object can be anything that Photon can serialize, including (but not limited to)
        ///     Hashtable, object[] and string. This value is defined by your own conventions.
        /// </remarks>
        /// <param name="status">Predefined states are in class ChatUserStatus. Other values can be used at will.</param>
        /// <param name="message">Also sets a status-message which your friends can get.</param>
        /// <returns>True if the operation gets called on the server.</returns>
        public bool SetOnlineStatus(int status, object message)
        {
            return SetOnlineStatus(status, message, false);
        }

        /// <summary>
        ///     Adds friends to a list on the Chat Server which will send you status updates for those.
        /// </summary>
        /// <remarks>
        ///     AddFriends and RemoveFriends enable clients to handle their friend list
        ///     in the Photon Chat server. Having users on your friends list gives you access
        ///     to their current online status (and whatever info your client sets in it).
        ///     Each user can set an online status consisting of an integer and an arbitratry
        ///     (serializable) object. The object can be null, Hashtable, object[] or anything
        ///     else Photon can serialize.
        ///     The status is published automatically to friends (anyone who set your user ID
        ///     with AddFriends).
        ///     Photon flushes friends-list when a chat client disconnects, so it has to be
        ///     set each time. If your community API gives you access to online status already,
        ///     you could filter and set online friends in AddFriends.
        ///     Actual friend relations are not persistent and have to be stored outside
        ///     of Photon.
        /// </remarks>
        /// <param name="friends">Array of friend userIds.</param>
        /// <returns>If the operation could be sent.</returns>
        public bool AddFriends(string[] friends)
        {
            if (!CanChat) return false;

            if (friends == null || friends.Length == 0)
            {
                listener.DebugReturn(DebugLevel.WARNING, "AddFriends can't be called for empty or null list.");
                return false;
            }

            var parameters = new Dictionary<byte, object> { { ChatParameterCode.Friends, friends } };
            return chatPeer.OpCustom(ChatOperationCode.AddFriends, parameters, true);
        }

        /// <summary>
        ///     Removes the provided entries from the list on the Chat Server and stops their status updates.
        /// </summary>
        /// <remarks>
        ///     Photon flushes friends-list when a chat client disconnects. Unless you want to
        ///     remove individual entries, you don't have to RemoveFriends.
        ///     AddFriends and RemoveFriends enable clients to handle their friend list
        ///     in the Photon Chat server. Having users on your friends list gives you access
        ///     to their current online status (and whatever info your client sets in it).
        ///     Each user can set an online status consisting of an integer and an arbitratry
        ///     (serializable) object. The object can be null, Hashtable, object[] or anything
        ///     else Photon can serialize.
        ///     The status is published automatically to friends (anyone who set your user ID
        ///     with AddFriends).
        ///     Photon flushes friends-list when a chat client disconnects, so it has to be
        ///     set each time. If your community API gives you access to online status already,
        ///     you could filter and set online friends in AddFriends.
        ///     Actual friend relations are not persistent and have to be stored outside
        ///     of Photon.
        ///     AddFriends and RemoveFriends enable clients to handle their friend list
        ///     in the Photon Chat server. Having users on your friends list gives you access
        ///     to their current online status (and whatever info your client sets in it).
        ///     Each user can set an online status consisting of an integer and an arbitratry
        ///     (serializable) object. The object can be null, Hashtable, object[] or anything
        ///     else Photon can serialize.
        ///     The status is published automatically to friends (anyone who set your user ID
        ///     with AddFriends).
        ///     Actual friend relations are not persistent and have to be stored outside
        ///     of Photon.
        /// </remarks>
        /// <param name="friends">Array of friend userIds.</param>
        /// <returns>If the operation could be sent.</returns>
        public bool RemoveFriends(string[] friends)
        {
            if (!CanChat) return false;

            if (friends == null || friends.Length == 0)
            {
                listener.DebugReturn(DebugLevel.WARNING, "RemoveFriends can't be called for empty or null list.");
                return false;
            }

            var parameters = new Dictionary<byte, object> { { ChatParameterCode.Friends, friends } };
            return chatPeer.OpCustom(ChatOperationCode.RemoveFriends, parameters, true);
        }

        /// <summary>
        ///     Get you the (locally used) channel name for the chat between this client and another user.
        /// </summary>
        /// <param name="userName">Remote user's name or UserId.</param>
        /// <returns>The (locally used) channel name for a private channel.</returns>
        public string GetPrivateChannelNameByUser(string userName)
        {
            return string.Format("{0}:{1}", UserId, userName);
        }

        /// <summary>
        ///     Simplified access to either private or public channels by name.
        /// </summary>
        /// <param name="channelName">
        ///     Name of the channel to get. For private channels, the channel-name is composed of both user's
        ///     names.
        /// </param>
        /// <param name="isPrivate">Define if you expect a private or public channel.</param>
        /// <param name="channel">Out parameter gives you the found channel, if any.</param>
        /// <returns>True if the channel was found.</returns>
        public bool TryGetChannel(string channelName, bool isPrivate, out ChatChannel channel)
        {
            if (!isPrivate) return PublicChannels.TryGetValue(channelName, out channel);
            return PrivateChannels.TryGetValue(channelName, out channel);
        }

        public void SendAcksOnly()
        {
            if (chatPeer != null) chatPeer.SendAcksOnly();
        }

        /// <summary>
        ///     Sets the level (and amount) of debug output provided by the library.
        /// </summary>
        /// <remarks>
        ///     This affects the callbacks to IChatClientListener.DebugReturn.
        ///     Default Level: Error.
        /// </remarks>
        public DebugLevel DebugOut
        {
            get => chatPeer.DebugOut;
            set => chatPeer.DebugOut = value;
        }

        #region Private methods area

        #region IPhotonPeerListener implementation

        void IPhotonPeerListener.DebugReturn(DebugLevel level, string message)
        {
            listener.DebugReturn(level, message);
        }

        void IPhotonPeerListener.OnEvent(EventData eventData)
        {
            switch (eventData.Code)
            {
                case ChatEventCode.ChatMessages:
                    HandleChatMessagesEvent(eventData);
                    break;
                case ChatEventCode.PrivateMessage:
                    HandlePrivateMessageEvent(eventData);
                    break;
                case ChatEventCode.StatusUpdate:
                    HandleStatusUpdate(eventData);
                    break;
                case ChatEventCode.Subscribe:
                    HandleSubscribeEvent(eventData);
                    break;
                case ChatEventCode.Unsubscribe:
                    HandleUnsubscribeEvent(eventData);
                    break;
            }
        }

        void IPhotonPeerListener.OnOperationResponse(OperationResponse operationResponse)
        {
            switch (operationResponse.OperationCode)
            {
                case ChatOperationCode.Authenticate:
                    HandleAuthResponse(operationResponse);
                    break;

                // the following operations usually don't return useful data and no error.
                case ChatOperationCode.Subscribe:
                case ChatOperationCode.Unsubscribe:
                case ChatOperationCode.Publish:
                case ChatOperationCode.SendPrivate:
                default:
                    if (operationResponse.ReturnCode != 0)
                        listener.DebugReturn(
                            DebugLevel.ERROR,
                            string.Format(
                                "Chat Operation {0} failed (Code: {1}). Debug Message: {2}",
                                operationResponse.OperationCode,
                                operationResponse.ReturnCode,
                                operationResponse.DebugMessage));
                    break;
            }
        }

        void IPhotonPeerListener.OnStatusChanged(StatusCode statusCode)
        {
            switch (statusCode)
            {
                case StatusCode.Connect:
                    if (!chatPeer.IsProtocolSecure)
                    {
#if UNITY
						UnityEngine.Debug.Log("Establishing Encryption");
#endif
                        chatPeer.EstablishEncryption();
                    }
                    else
                    {
#if UNITY
						UnityEngine.Debug.Log("Skipping Encryption");
#endif
                        if (!didAuthenticate)
                        {
                            didAuthenticate =
                                chatPeer.AuthenticateOnNameServer(AppId, AppVersion, chatRegion, AuthValues);
                            if (!didAuthenticate)
                                ((IPhotonPeerListener)this).DebugReturn(
                                    DebugLevel.ERROR,
                                    "Error calling OpAuthenticate! Did not work. Check log output, AuthValues and if you're connected. State: "
                                    + State);
                        }
                    }

                    if (State == ChatState.ConnectingToNameServer)
                    {
                        State = ChatState.ConnectedToNameServer;
                        listener.OnChatStateChange(State);
                    }
                    else if (State == ChatState.ConnectingToFrontEnd)
                    {
                        AuthenticateOnFrontEnd();
                    }

                    break;
                case StatusCode.EncryptionEstablished:
                    // once encryption is availble, the client should send one (secure) authenticate. it includes the AppId (which identifies your app on the Photon Cloud)
                    if (!didAuthenticate)
                    {
                        didAuthenticate = chatPeer.AuthenticateOnNameServer(AppId, AppVersion, chatRegion, AuthValues);
                        if (!didAuthenticate)
                            ((IPhotonPeerListener)this).DebugReturn(
                                DebugLevel.ERROR,
                                "Error calling OpAuthenticate! Did not work. Check log output, AuthValues and if you're connected. State: "
                                + State);
                    }

                    break;
                case StatusCode.EncryptionFailedToEstablish:
                    State = ChatState.Disconnecting;
                    chatPeer.Disconnect();
                    break;
                case StatusCode.Disconnect:
                    if (State == ChatState.Authenticated)
                    {
                        ConnectToFrontEnd();
                    }
                    else
                    {
                        State = ChatState.Disconnected;
                        listener.OnChatStateChange(ChatState.Disconnected);
                        listener.OnDisconnected();
                    }

                    break;
            }
        }

#if SDK_V4
        void IPhotonPeerListener.OnMessage(object msg)
        {
            // in v4 interface IPhotonPeerListener
            return;
        }
#endif

        #endregion

        private bool SendChannelOperation(string[] channels, byte operation, int historyLength)
        {
            var opParameters = new Dictionary<byte, object> { { ChatParameterCode.Channels, channels } };

            if (historyLength != 0) opParameters.Add(ChatParameterCode.HistoryLength, historyLength);

            return chatPeer.OpCustom(operation, opParameters, true);
        }

        private void HandlePrivateMessageEvent(EventData eventData)
        {
            // Console.WriteLine(SupportClass.DictionaryToString(eventData.Parameters));
            var message = eventData.Parameters[ChatParameterCode.Message];
            var sender = (string)eventData.Parameters[ChatParameterCode.Sender];

            string channelName;
            if (UserId != null && UserId.Equals(sender))
            {
                var target = (string)eventData.Parameters[ChatParameterCode.UserId];
                channelName = GetPrivateChannelNameByUser(target);
            }
            else
            {
                channelName = GetPrivateChannelNameByUser(sender);
            }

            ChatChannel channel;
            if (!PrivateChannels.TryGetValue(channelName, out channel))
            {
                channel = new ChatChannel(channelName);
                channel.IsPrivate = true;
                PrivateChannels.Add(channel.Name, channel);
            }

            channel.Add(sender, message);
            listener.OnPrivateMessage(sender, message, channelName);
        }

        private void HandleChatMessagesEvent(EventData eventData)
        {
            var messages = (object[])eventData.Parameters[ChatParameterCode.Messages];
            var senders = (string[])eventData.Parameters[ChatParameterCode.Senders];
            var channelName = (string)eventData.Parameters[ChatParameterCode.Channel];

            ChatChannel channel;
            if (!PublicChannels.TryGetValue(channelName, out channel)) return;

            channel.Add(senders, messages);
            listener.OnGetMessages(channelName, senders, messages);
        }

        private void HandleSubscribeEvent(EventData eventData)
        {
            var channelsInResponse = (string[])eventData.Parameters[ChatParameterCode.Channels];
            var results = (bool[])eventData.Parameters[ChatParameterCode.SubscribeResults];

            for (var i = 0; i < channelsInResponse.Length; i++)
                if (results[i])
                {
                    var channelName = channelsInResponse[i];
                    if (!PublicChannels.ContainsKey(channelName))
                    {
                        var channel = new ChatChannel(channelName);
                        PublicChannels.Add(channel.Name, channel);
                    }
                }

            listener.OnSubscribed(channelsInResponse, results);
        }

        private void HandleUnsubscribeEvent(EventData eventData)
        {
            var channelsInRequest = (string[])eventData[ChatParameterCode.Channels];
            for (var i = 0; i < channelsInRequest.Length; i++)
            {
                var channelName = channelsInRequest[i];
                PublicChannels.Remove(channelName);
            }

            listener.OnUnsubscribed(channelsInRequest);
        }

        private void HandleAuthResponse(OperationResponse operationResponse)
        {
            listener.DebugReturn(
                DebugLevel.INFO,
                operationResponse.ToStringFull() + " on: " + chatPeer.NameServerAddress);
            if (operationResponse.ReturnCode == 0)
            {
                if (State == ChatState.ConnectedToNameServer)
                {
                    State = ChatState.Authenticated;
                    listener.OnChatStateChange(State);

                    if (operationResponse.Parameters.ContainsKey(ParameterCode.Secret))
                    {
                        if (AuthValues == null) AuthValues = new AuthenticationValues();
                        AuthValues.Token = operationResponse[ParameterCode.Secret] as string;
                        FrontendAddress = (string)operationResponse[ParameterCode.Address];

                        // we disconnect and status handler starts to connect to front end
                        chatPeer.Disconnect();
                    }
                }
                else if (State == ChatState.ConnectingToFrontEnd)
                {
                    msDeltaForServiceCalls = msDeltaForServiceCalls * 4;

                    // when we arrived on chat server: limit Service calls some more
                    State = ChatState.ConnectedToFrontEnd;
                    listener.OnChatStateChange(State);
                    listener.OnConnected();
                }
            }
            else
            {
                // this.listener.DebugReturn(DebugLevel.INFO, operationResponse.ToStringFull() + " NS: " + this.NameServerAddress + " FrontEnd: " + this.frontEndAddress);
                switch (operationResponse.ReturnCode)
                {
                    case ErrorCode.InvalidAuthentication:
                        DisconnectedCause = ChatDisconnectCause.InvalidAuthentication;
                        break;
                    case ErrorCode.CustomAuthenticationFailed:
                        DisconnectedCause = ChatDisconnectCause.CustomAuthenticationFailed;
                        break;
                    case ErrorCode.InvalidRegion:
                        DisconnectedCause = ChatDisconnectCause.InvalidRegion;
                        break;
                    case ErrorCode.MaxCcuReached:
                        DisconnectedCause = ChatDisconnectCause.MaxCcuReached;
                        break;
                    case ErrorCode.OperationNotAllowedInCurrentState:
                        DisconnectedCause = ChatDisconnectCause.OperationNotAllowedInCurrentState;
                        break;
                }

                State = ChatState.Disconnecting;
                chatPeer.Disconnect();
            }
        }

        private void HandleStatusUpdate(EventData eventData)
        {
            var user = (string)eventData.Parameters[ChatParameterCode.Sender];
            var status = (int)eventData.Parameters[ChatParameterCode.Status];

            object message = null;
            var gotMessage = eventData.Parameters.ContainsKey(ChatParameterCode.Message);
            if (gotMessage) message = eventData.Parameters[ChatParameterCode.Message];

            listener.OnStatusUpdate(user, status, gotMessage, message);
        }

        private void ConnectToFrontEnd()
        {
            State = ChatState.ConnectingToFrontEnd;
            listener.DebugReturn(DebugLevel.INFO, "Connecting to frontend " + FrontendAddress);
            chatPeer.Connect(FrontendAddress, ChatApppName);
        }

        private bool AuthenticateOnFrontEnd()
        {
            if (AuthValues != null)
            {
                var opParameters = new Dictionary<byte, object> { { ChatParameterCode.Secret, AuthValues.Token } };
                return chatPeer.OpCustom(ChatOperationCode.Authenticate, opParameters, true);
            }

            Debug.WriteLine("Can't authenticate on front end server. CustomAuthValues is null");
            return false;
        }

        #endregion
    }
}