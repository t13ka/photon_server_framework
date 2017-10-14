namespace YourGame.Client.Loadbalancing
{
    using System;
    using System.Collections.Generic;

    using ExitGames.Client.Photon;

    using YourGame.Client.Loadbalancing.Codes;
    using YourGame.Client.Loadbalancing.Enums;

#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_WII || UNITY_IPHONE || UNITY_ANDROID || UNITY_PS3 || UNITY_XBOX360 || UNITY_NACL  || UNITY_FLASH  || UNITY_BLACKBERRY || UNITY_PSP2 || UNITY_WEBGL
#endif

    /// <summary>
    ///     A LoadbalancingPeer provides the operations and 
    ///     <see langword="enum" />definitions needed to use the loadbalancing
    ///     server application which is also used in Photon Cloud.
    /// </summary>
    /// <remarks>
    ///     The <see cref="LoadBalancingPeer" /> does not keep a state, instead
    ///     this is done by a LoadBalancingClient.
    /// </remarks>
    public class LoadBalancingPeer : PhotonPeer
    {
        /// <summary>
        ///     Creates a Peer with selected connection protocol.
        /// </summary>
        /// <remarks>
        ///     Each connection protocol has it's own default networking ports for
        ///     Photon.
        /// </remarks>
        /// <param name="protocolType">The preferred option is UDP.</param>
        public LoadBalancingPeer(ConnectionProtocol protocolType) : base(protocolType)
        {
            // this does not require a Listener, so:
            // make sure to set this.Listener before using a peer!
        }

        /// <summary>
        ///     Creates a Peer with default connection protocol (UDP).
        /// </summary>
        public LoadBalancingPeer(IPhotonPeerListener listener, ConnectionProtocol protocolType)
            : base(listener, protocolType)
        {
        }

        public virtual bool OpGetRegions(string appId)
        {
            var parameters = new Dictionary<byte, object> {[ParameterCode.ApplicationId] = appId};

            return OpCustom(OperationCode.GetRegions, parameters, true, 0, true);
        }

        /// <summary>
        ///     Calls OpJoinLobby(string name, <see cref="LobbyType" /> lobbyType).
        /// </summary>
        /// <returns>
        ///     If the operation could be sent (requires connection).
        /// </returns>
        public virtual bool OpJoinLobby()
        {
            return OpJoinLobby(TypedLobby.Default);
        }

        /// <summary>
        ///     Joins the <paramref name="lobby" /> on the Master Server, where you
        ///     get a list of RoomInfos of currently open rooms. This is an async
        ///     request which triggers a OnOperationResponse() call.
        /// </summary>
        /// <param name="lobby">The lobby join to.</param>
        /// <returns>
        ///     If the operation could be sent (has to be connected).
        /// </returns>
        public virtual bool OpJoinLobby(TypedLobby lobby)
        {
            if (lobby == null)
            {
                lobby = TypedLobby.Default;
            }
            var parameters = new Dictionary<byte, object>
            {
                [ParameterCode.LobbyName] = lobby.Name,
                [ParameterCode.LobbyType] = (byte) lobby.Type
            };

            return OpCustom(OperationCode.JoinLobby, parameters, true);
        }


        /// <summary>
        ///     Leaves the lobby on the Master Server. This is an async request
        ///     which triggers a OnOperationResponse() call.
        /// </summary>
        /// <returns>
        ///     If the operation could be sent (requires connection).
        /// </returns>
        public virtual bool OpLeaveLobby()
        {
            if (DebugOut >= DebugLevel.INFO)
            {
                Listener.DebugReturn(DebugLevel.INFO, "OpLeaveLobby()");
            }

            return OpCustom(OperationCode.LeaveLobby, null, true);
        }

        private void RoomOptionsToOpParameters(Dictionary<byte, object> op, RoomOptions roomOptions)
        {
            if (roomOptions == null)
            {
                roomOptions = new RoomOptions();
            }

            var gameProperties = new Hashtable
            {
                [GamePropertyKey.IsOpen] = roomOptions.IsOpen,
                [GamePropertyKey.IsVisible] = roomOptions.IsVisible,
                [GamePropertyKey.PropsListedInLobby] = roomOptions.CustomRoomPropertiesForLobby == null
                    ? new string[0]
                    : roomOptions.CustomRoomPropertiesForLobby
            };
            gameProperties.MergeStringKeys(roomOptions.CustomRoomProperties);
            if (roomOptions.MaxPlayers > 0)
            {
                gameProperties[GamePropertyKey.MaxPlayers] = roomOptions.MaxPlayers;
            }
            op[ParameterCode.GameProperties] = gameProperties;


            op[ParameterCode.CleanupCacheOnLeave] = roomOptions.CleanupCacheOnLeave;

            if (roomOptions.CheckUserOnJoin)
            {
                op[ParameterCode.CheckUserOnJoin] = true; //TURNBASED
            }

            if (roomOptions.PlayerTtl > 0 || roomOptions.PlayerTtl == -1)
            {
                op[ParameterCode.PlayerTTL] = roomOptions.PlayerTtl; //TURNBASED
            }
            if (roomOptions.EmptyRoomTtl > 0)
            {
                op[ParameterCode.EmptyRoomTTL] = roomOptions.EmptyRoomTtl; //TURNBASED
            }
            if (roomOptions.SuppressRoomEvents)
            {
                op[ParameterCode.SuppressRoomEvents] = true;
            }
            if (roomOptions.Plugins != null)
            {
                op[ParameterCode.Plugins] = roomOptions.Plugins;
            }
        }

        /// <summary>
        ///     Creates a room (on either Master or Game Server). The
        ///     OperationResponse depends on the server the peer is connected to:
        ///     Master will return a Game Server to connect to. Game Server will
        ///     return the joined Room's data. This is an async request which
        ///     triggers a OnOperationResponse() call.
        /// </summary>
        /// <remarks>
        ///     If the room is already existing, the OperationResponse will have a
        ///     returnCode of ErrorCode.GameAlreadyExists.
        /// </remarks>
        /// <param name="roomName">
        ///     The name of this room. Must be unique. Pass <see langword="null" />
        ///     to make the server assign a name.
        /// </param>
        /// <param name="roomOptions">
        ///     <see cref="Room" /> creation options. Pass <see langword="null" /> for
        ///     defaults.
        /// </param>
        /// <param name="lobby">
        ///     Lobby this room gets added to. If null, current lobby (or default
        ///     lobby) is used.
        /// </param>
        /// <param name="playerProperties">
        ///     This player's custom properties. Use string keys!
        /// </param>
        /// <param name="onGameServer">
        ///     This operation sends more parameters to the GameServer than to the
        ///     MasterServer to optimize traffic.
        /// </param>
        /// <returns>
        ///     If the operation could be sent (requires connection).
        /// </returns>
        public virtual bool OpCreateRoom(string roomName, RoomOptions roomOptions, TypedLobby lobby,
            Hashtable playerProperties, bool onGameServer)
        {
            if (DebugOut >= DebugLevel.INFO)
            {
                Listener.DebugReturn(DebugLevel.INFO, "OpCreateRoom()");
            }

            var op = new Dictionary<byte, object>();
            if (!string.IsNullOrEmpty(roomName))
            {
                op[ParameterCode.RoomName] = roomName;
            }
            if (lobby != null && !string.IsNullOrEmpty(lobby.Name))
            {
                op[ParameterCode.LobbyName] = lobby.Name;
                op[ParameterCode.LobbyType] = lobby.Type;
            }

            // room- and player-props are only needed by the GameServer
            if (onGameServer)
            {
                if (playerProperties != null)
                {
                    op[ParameterCode.PlayerProperties] = playerProperties;
                    op[ParameterCode.Broadcast] = true;
                }

                RoomOptionsToOpParameters(op, roomOptions);
            }

            return OpCustom(OperationCode.CreateGame, op, true);
        }


        /// <summary>
        ///     Joins a room by name or creates new room if room with given name not
        ///     exists. The OperationResponse depends on the server the peer is
        ///     connected to: Master will return a Game Server to connect to. Game
        ///     Server will return the joined Room's data. This is an async request
        ///     which triggers a OnOperationResponse() call.
        /// </summary>
        /// <remarks>
        ///     If the room is not existing (anymore), the OperationResponse will
        ///     have a returnCode of ErrorCode.GameDoesNotExist. Other possible
        ///     ErrorCodes are: GameClosed, GameFull.
        /// </remarks>
        /// <param name="roomName">The name of an existing room.</param>
        /// <param name="playerProperties">
        ///     This player's custom properties.
        /// </param>
        /// <param name="actorId">
        ///     To allow players to return to a game, they have to specify their
        ///     actorId.
        /// </param>
        /// <param name="createRoomOptions">
        ///     Options used for new room creation (only sent if
        ///     <paramref name="createIfNotExists" /> is true).
        /// </param>
        /// <param name="lobby">
        ///     Typed lobby to be used if the roomname is not in use (and room gets
        ///     created). It's never used when <paramref name="createIfNotExists" />
        ///     is false.
        /// </param>
        /// <param name="createIfNotExists">
        ///     Tells the server to create the room if it doesn't exist (if true).
        /// </param>
        /// <param name="onGameServer">
        ///     Master- and Game-Server require different parameters. This allows us
        ///     to optimize the requests based on the server type (unknown in peer).
        /// </param>
        /// <returns>
        ///     If the operation could be sent (requires connection).
        /// </returns>
        public virtual bool OpJoinRoom(string roomName, Hashtable playerProperties, int actorId,
            RoomOptions createRoomOptions, TypedLobby lobby, bool createIfNotExists, bool onGameServer)
        {
            if (DebugOut >= DebugLevel.INFO)
            {
                Listener.DebugReturn(DebugLevel.INFO, "OpJoinOrCreateRoom()");
            }

            if (string.IsNullOrEmpty(roomName))
            {
                Listener.DebugReturn(DebugLevel.ERROR, "OpJoinOrCreateRoom() failed. Please specify a roomname.");
                return false;
            }


            var op = new Dictionary<byte, object> {[ParameterCode.RoomName] = roomName};

            if (createIfNotExists)
            {
                op[ParameterCode.JoinMode] = (byte) JoinMode.CreateIfNotExists;
                if (lobby != null)
                {
                    op[ParameterCode.LobbyName] = lobby.Name;
                    op[ParameterCode.LobbyType] = (byte) lobby.Type;
                }
            }

            if (actorId != 0)
            {
                op[ParameterCode.JoinMode] = (byte) JoinMode.Rejoin;
                op[ParameterCode.ActorNr] = actorId;
            }

            if (onGameServer)
            {
                if (playerProperties != null)
                {
                    op[ParameterCode.PlayerProperties] = playerProperties;
                    op[ParameterCode.Broadcast] = true;
                }

                if (createIfNotExists)
                {
                    RoomOptionsToOpParameters(op, createRoomOptions);
                }
            }

            return OpCustom(OperationCode.JoinGame, op, true);
        }

        /// <summary>
        ///     Operation to join a random, available room. Overloads take additional player properties.
        ///     This is an async request which triggers a OnOperationResponse() call.
        ///     If all rooms are closed or full, the OperationResponse will have a returnCode of ErrorCode.NoRandomMatchFound.
        ///     If successful, the OperationResponse contains a gameserver address and the name of some room.
        /// </summary>
        /// <param name="expectedCustomRoomProperties">
        ///     Optional. A room will only be joined, if it matches these custom properties
        ///     (with string keys).
        /// </param>
        /// <param name="expectedMaxPlayers">Filters for a particular maxplayer setting. Use 0 to accept any maxPlayer value.</param>
        /// <param name="playerProperties">This player's properties (custom and well known alike).</param>
        /// <param name="matchingType">Selects one of the available matchmaking algorithms. See MatchmakingMode enum for options.</param>
        /// <param name="lobby">The lobby in which to find a room. Use null for default lobby.</param>
        /// <param name="sqlLobbyFilter">
        ///     Basically the "where" clause of a sql statement. Use null for random game. Examples: "C0 = 1 AND C2 > 50". "C5 =
        ///     \"Map2\" AND C2 > 10 AND C2 < 20"</param>
        /// <returns>If the operation could be sent currently (requires connection).</returns>
        public virtual bool OpJoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers,
            Hashtable playerProperties, MatchmakingMode matchingType, TypedLobby lobby, string sqlLobbyFilter)
        {
            if (DebugOut >= DebugLevel.INFO)
            {
                Listener.DebugReturn(DebugLevel.INFO, "OpJoinRandomRoom()");
            }
            if (lobby == null)
            {
                lobby = TypedLobby.Default;
            }
            var expectedRoomProperties = new Hashtable();
            expectedRoomProperties.MergeStringKeys(expectedCustomRoomProperties);
            if (expectedMaxPlayers > 0)
            {
                expectedRoomProperties[GamePropertyKey.MaxPlayers] = expectedMaxPlayers;
            }

            var opParameters = new Dictionary<byte, object>();
            if (expectedRoomProperties.Count > 0)
            {
                opParameters[ParameterCode.GameProperties] = expectedRoomProperties;
            }

            if (playerProperties != null && playerProperties.Count > 0)
            {
                opParameters[ParameterCode.PlayerProperties] = playerProperties;
            }

            if (matchingType != MatchmakingMode.FillRoom)
            {
                opParameters[ParameterCode.MatchMakingType] = (byte) matchingType;
            }

            if (!string.IsNullOrEmpty(lobby.Name))
            {
                opParameters[ParameterCode.LobbyName] = lobby.Name;
                opParameters[ParameterCode.LobbyType] = (byte) lobby.Type;
            }

            if (!string.IsNullOrEmpty(sqlLobbyFilter))
            {
                opParameters[ParameterCode.Data] = sqlLobbyFilter;
            }
            return OpCustom(OperationCode.JoinRandomGame, opParameters, true);
        }

        /// <summary>
        ///     Leaves a room with option to come back later or "for good".
        /// </summary>
        /// <param name="willComeBack">
        ///     Async games can be re-joined (loaded) later on. Set to false, if you
        ///     want to abandon a game entirely.
        /// </param>
        /// <returns>
        ///     If the opteration can be send currently.
        /// </returns>
        public virtual bool OpLeaveRoom(bool willComeBack)
        {
            var opParameters = new Dictionary<byte, object>();
            if (willComeBack)
            {
                opParameters[ParameterCode.IsComingBack] = willComeBack;
            }
            return OpCustom(OperationCode.Leave, opParameters, true);
        }

        /// <summary>
        ///     Request the rooms and online status for a list of friends (each
        ///     client must set a unique username via OpAuthenticate).
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Used on Master Server to find the rooms played by a selected list of
        ///         users. Users identify themselves by using
        ///         <see cref="OpAuthenticate" /> with a unique username. The list of
        ///         usernames must be fetched from some other source (not provided by
        ///         Photon).
        ///     </para>
        ///     <para>
        ///         The server response includes 2 arrays of info (each index matching a
        ///         friend from the request):
        ///         ParameterCode.FindFriendsResponseOnlineList = bool[] of online
        ///         states ParameterCode.FindFriendsResponseRoomIdList = string[] of
        ///         room names (empty string if not in a room)
        ///     </para>
        /// </remarks>
        /// <param name="friendsToFind">
        ///     Array of friend's names (make sure they are unique).
        /// </param>
        /// <returns>
        ///     If the operation could be sent (requires connection).
        /// </returns>
        public virtual bool OpFindFriends(string[] friendsToFind)
        {
            var opParameters = new Dictionary<byte, object>();
            if (friendsToFind != null && friendsToFind.Length > 0)
            {
                opParameters[ParameterCode.FindFriendsRequestList] = friendsToFind;
            }

            return OpCustom(OperationCode.FindFriends, opParameters, true);
        }


        /// <summary>
        ///     Sets properties of a player / actor. Internally this uses
        ///     OpSetProperties, which can be used to either set room or player
        ///     properties.
        /// </summary>
        /// <param name="actorNr">
        ///     The payer ID (a.k.a. actorNumber) of the player to attach these
        ///     properties to.
        /// </param>
        /// <param name="actorProperties">
        ///     The properties to add or update.
        /// </param>
        /// <param name="expectedProperties"></param>
        /// <returns>
        ///     If the operation could be sent (requires connection).
        /// </returns>
        public virtual bool OpSetPropertiesOfActor(int actorNr, Hashtable actorProperties,
            Hashtable expectedProperties = null)
        {
            if (DebugOut >= DebugLevel.INFO)
            {
                Listener.DebugReturn(DebugLevel.INFO, "OpSetPropertiesOfActor()");
            }

            var opParameters = new Dictionary<byte, object>
            {
                {ParameterCode.Properties, actorProperties},
                {ParameterCode.ActorNr, actorNr},
                {ParameterCode.Broadcast, true}
            };
            if (expectedProperties != null && expectedProperties.Count != 0)
            {
                opParameters.Add(ParameterCode.ExpectedValues, expectedProperties);
            }

            return OpCustom(OperationCode.SetProperties, opParameters, true, 0, false);
        }

        /// <summary>
        ///     Sets properties of a room. Internally this uses OpSetProperties,
        ///     which can be used to either set room or player properties.
        /// </summary>
        /// <param name="gameProperties"></param>
        /// <param name="webForward"></param>
        /// <param name="expectedProperties"></param>
        /// <returns>
        ///     If the operation could be sent (has to be connected).
        /// </returns>
        public virtual bool OpSetPropertiesOfRoom(Hashtable gameProperties, bool webForward,
            Hashtable expectedProperties = null)
        {
            if (DebugOut >= DebugLevel.INFO)
            {
                Listener.DebugReturn(DebugLevel.INFO, "OpSetPropertiesOfRoom()");
            }

            var opParameters = new Dictionary<byte, object>
            {
                {ParameterCode.Properties, gameProperties},
                {ParameterCode.Broadcast, true}
            };
            if (expectedProperties != null && expectedProperties.Count != 0)
            {
                opParameters.Add(ParameterCode.ExpectedValues, expectedProperties);
            }

            if (webForward)
            {
                opParameters[ParameterCode.EventForward] = true;
                //UnityEngine.Debug.LogWarning("Forwarding props. To player.ID: " + gameProperties["turn"]);
            }

            return OpCustom(OperationCode.SetProperties, opParameters, true, 0, false);
        }

        /// <summary>
        ///     Sends this app's <paramref name="appId" /> and
        ///     <paramref name="appVersion" /> to identify this application server
        ///     side. This is an async request which triggers a
        ///     OnOperationResponse() call.
        /// </summary>
        /// <remarks>
        ///     This operation makes use of encryption, if that is established
        ///     before. See: EstablishEncryption(). <see cref="Uri.Check" />
        ///     encryption with IsEncryptionAvailable. This operation is allowed
        ///     only once per connection (multiple calls will have
        ///     <see cref="ErrorCode" /> != Ok).
        /// </remarks>
        /// <param name="appId">
        ///     Your application's name or ID to authenticate. This is assigned by
        ///     Photon Cloud (webpage).
        /// </param>
        /// <param name="appVersion">
        ///     The client's version (clients with differing client appVersions are
        ///     separated and players don't meet).
        /// </param>
        /// <param name="authValues">
        ///     Optional authentication values. The client can set no values or a
        ///     UserId or some parameters for Custom Authentication by a server.
        /// </param>
        /// <param name="regionCode">
        ///     Optional region code, if the client should connect to a specific
        ///     Photon Cloud Region.
        /// </param>
        /// <param name="getLobbyStatistics">
        ///     Set to <see langword="true" /> on Master Server to receive "Lobby
        ///     Statistics" events.
        /// </param>
        /// <returns>
        ///     If the operation could be sent (has to be connected).
        /// </returns>
        public virtual bool OpAuthenticate(string appId, string appVersion, AuthenticationValues authValues,
            string regionCode, bool getLobbyStatistics)
        {
            if (DebugOut >= DebugLevel.INFO)
            {
                Listener.DebugReturn(DebugLevel.INFO, "OpAuthenticate()");
            }

            var opParameters = new Dictionary<byte, object>();
            if (getLobbyStatistics)
            {
                // must be sent in operation, even if a Token is available
                opParameters[ParameterCode.LobbyStats] = true;
            }

            if (authValues != null && authValues.Token != null)
            {
                opParameters[ParameterCode.Secret] = authValues.Token;
                return OpCustom(OperationCode.Authenticate, opParameters, true, 0, false);
            }


            opParameters[ParameterCode.AppVersion] = appVersion;
            opParameters[ParameterCode.ApplicationId] = appId;

            if (!string.IsNullOrEmpty(regionCode))
            {
                opParameters[ParameterCode.Region] = regionCode;
            }

            if (authValues != null)
            {
                if (!string.IsNullOrEmpty(authValues.UserId))
                {
                    opParameters[ParameterCode.UserId] = authValues.UserId;
                }

                if (authValues.AuthType != CustomAuthenticationType.None)
                {
                    opParameters[ParameterCode.ClientAuthenticationType] = (byte) authValues.AuthType;
                    if (!string.IsNullOrEmpty(authValues.Token))
                    {
                        opParameters[ParameterCode.Secret] = authValues.Token;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(authValues.AuthGetParameters))
                        {
                            opParameters[ParameterCode.ClientAuthenticationParams] = authValues.AuthGetParameters;
                        }
                        if (authValues.AuthPostData != null)
                        {
                            opParameters[ParameterCode.ClientAuthenticationData] = authValues.AuthPostData;
                        }
                    }
                }
            }

            return OpCustom(OperationCode.Authenticate, opParameters, true, 0, IsEncryptionAvailable);
        }

        /// <summary>
        ///     Operation to handle this client's interest groups (for events in
        ///     room).
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Note the difference between passing <see langword="null" /> and
        ///         byte[0]: <see langword="null" /> won't add/remove any groups. byte[0]
        ///         will add/remove all (existing) groups. First, removing groups is
        ///         executed. This way, you could leave all groups and join only the
        ///         ones provided.
        ///     </para>
        ///     <para>
        ///         Changes become active not immediately but when the server executes
        ///         this operation (approximately RTT/2).
        ///     </para>
        /// </remarks>
        /// <param name="groupsToRemove">
        ///     Groups to remove from interest. Null will not leave any. A byte[0]
        ///     will remove all.
        /// </param>
        /// <param name="groupsToAdd">
        ///     Groups to add to interest. Null will not add any. A byte[0] will add
        ///     all current.
        /// </param>
        /// <returns>
        /// </returns>
        public virtual bool OpChangeGroups(byte[] groupsToRemove, byte[] groupsToAdd)
        {
            if (DebugOut >= DebugLevel.ALL)
            {
                Listener.DebugReturn(DebugLevel.ALL, "OpChangeGroups()");
            }

            var opParameters = new Dictionary<byte, object>();
            if (groupsToRemove != null)
            {
                opParameters[ParameterCode.Remove] = groupsToRemove;
            }
            if (groupsToAdd != null)
            {
                opParameters[ParameterCode.Add] = groupsToAdd;
            }

            return OpCustom(OperationCode.ChangeGroups, opParameters, true, 0);
        }

        /// <summary>
        ///     Send an event with custom code/type and any content to the other
        ///     players in the same room.
        /// </summary>
        /// <remarks>
        ///     This <see langword="override" /> explicitly uses another parameter
        ///     order to not mix it up with the implementation for
        ///     <see cref="Hashtable" /> only.
        /// </remarks>
        /// <param name="eventCode">
        ///     Identifies this type of event (and the content). Your game's event
        ///     codes can start with 0.
        /// </param>
        /// <param name="sendReliable">
        ///     If this event has to arrive reliably (potentially repeated if it's
        ///     lost).
        /// </param>
        /// <param name="customEventContent">
        ///     Any serializable datatype (including <see cref="Hashtable" /> like
        ///     the other <see cref="OpRaiseEvent" /> overloads).
        /// </param>
        /// <returns>
        ///     If operation could be enqueued for sending. Sent when calling:
        ///     Service or SendOutgoingCommands.
        /// </returns>
        [Obsolete("Use overload with RaiseEventOptions to reduce parameter- and overload-clutter.")]
        public virtual bool OpRaiseEvent(byte eventCode, bool sendReliable, object customEventContent)
        {
            return OpRaiseEvent(eventCode, customEventContent, sendReliable, null);
        }

        /// <summary>
        ///     Send an event with custom code/type and any content to the other
        ///     players in the same room.
        /// </summary>
        /// <remarks>
        ///     This <see langword="override" /> explicitly uses another parameter
        ///     order to not mix it up with the implementation for
        ///     <see cref="Hashtable" /> only.
        /// </remarks>
        /// <param name="eventCode">
        ///     Identifies this type of event (and the content). Your game's event
        ///     codes can start with 0.
        /// </param>
        /// <param name="sendReliable">
        ///     If this event has to arrive reliably (potentially repeated if it's
        ///     lost).
        /// </param>
        /// <param name="customEventContent">
        ///     Any serializable datatype (including <see cref="Hashtable" /> like
        ///     the other <see cref="OpRaiseEvent" /> overloads).
        /// </param>
        /// <param name="channelId">
        ///     Command sequence in which this command belongs. Must be less than
        ///     value of ChannelCount property. Default: 0.
        /// </param>
        /// <param name="cache">
        ///     Affects how the server will treat the event caching-wise. Can cache
        ///     events for players joining later on or remove previously cached
        ///     events. Default: DoNotCache.
        /// </param>
        /// <param name="targetActors">
        ///     List of ActorNumbers (in this room) to send the event to. Overrides
        ///     caching. Default: null.
        /// </param>
        /// <param name="receivers">
        ///     Defines a target-player group. Default: Others.
        /// </param>
        /// <param name="interestGroup">
        ///     Defines to which interest group the event is sent. Players can
        ///     subscribe or unsibscribe to groups. Group 0 is always sent to all.
        ///     Default: 0.
        /// </param>
        /// <returns>
        ///     If operation could be enqueued for sending. Sent when calling:
        ///     Service or SendOutgoingCommands.
        /// </returns>
        [Obsolete("Use overload with RaiseEventOptions to reduce parameter- and overload-clutter.")]
        public virtual bool OpRaiseEvent(byte eventCode, bool sendReliable, object customEventContent, byte channelId,
            EventCaching cache, int[] targetActors, ReceiverGroup receivers, byte interestGroup)
        {
            return OpRaiseEvent(eventCode, sendReliable, customEventContent, channelId, cache, targetActors, receivers,
                interestGroup, false);
        }

        /// <summary>
        ///     Send an event with custom code/type and any content to the other
        ///     players in the same room.
        /// </summary>
        /// <remarks>
        ///     This <see langword="override" /> explicitly uses another parameter
        ///     order to not mix it up with the implementation for
        ///     <see cref="Hashtable" /> only.
        /// </remarks>
        /// <param name="eventCode">
        ///     Identifies this type of event (and the content). Your game's event
        ///     codes can start with 0.
        /// </param>
        /// <param name="sendReliable">
        ///     If this event has to arrive reliably (potentially repeated if it's
        ///     lost).
        /// </param>
        /// <param name="customEventContent">
        ///     Any serializable datatype (including <see cref="Hashtable" /> like
        ///     the other <see cref="OpRaiseEvent" /> overloads).
        /// </param>
        /// <param name="channelId">
        ///     Command sequence in which this command belongs. Must be less than
        ///     value of ChannelCount property. Default: 0.
        /// </param>
        /// <param name="cache">
        ///     Affects how the server will treat the event caching-wise. Can cache
        ///     events for players joining later on or remove previously cached
        ///     events. Default: DoNotCache.
        /// </param>
        /// <param name="targetActors">
        ///     List of ActorNumbers (in this room) to send the event to. Overrides
        ///     caching. Default: null.
        /// </param>
        /// <param name="receivers">
        ///     Defines a target-player group. Default: Others.
        /// </param>
        /// <param name="interestGroup">
        ///     Defines to which interest group the event is sent. Players can
        ///     subscribe or unsibscribe to groups. Group 0 is always sent to all.
        ///     Default: 0.
        /// </param>
        /// <param name="forwardToWebhook">
        ///     Tells the server to send this event to the "WebHook" configured for
        ///     your Application in the Dashboard. Should only be used in Turnbased
        ///     games.
        /// </param>
        /// <returns>
        ///     If operation could be enqueued for sending. Sent when calling:
        ///     Service or SendOutgoingCommands.
        /// </returns>
        [Obsolete("Use overload with RaiseEventOptions to reduce parameter- and overload-clutter.")]
        public virtual bool OpRaiseEvent(byte eventCode, bool sendReliable, object customEventContent, byte channelId,
            EventCaching cache, int[] targetActors, ReceiverGroup receivers, byte interestGroup, bool forwardToWebhook)
        {
            var opParameters = new Dictionary<byte, object> {[ParameterCode.Code] = eventCode};

            if (customEventContent != null)
            {
                opParameters[ParameterCode.Data] = customEventContent;
            }
            if (cache != EventCaching.DoNotCache)
            {
                opParameters[ParameterCode.Cache] = (byte) cache;
            }
            if (receivers != ReceiverGroup.Others)
            {
                opParameters[ParameterCode.ReceiverGroup] = (byte) receivers;
            }
            if (interestGroup != 0)
            {
                opParameters[ParameterCode.Group] = interestGroup;
            }
            if (targetActors != null)
            {
                opParameters[ParameterCode.ActorList] = targetActors;
            }
            if (forwardToWebhook)
            {
                opParameters[ParameterCode.EventForward] = true; //TURNBASED
            }

            return OpCustom(OperationCode.RaiseEvent, opParameters, sendReliable, channelId, false);
        }

        /// <summary>
        ///     Send your custom data as event to an "interest group" in the current
        ///     Room.
        /// </summary>
        /// <remarks>
        ///     No matter if reliable or not, when an event is sent to a interest
        ///     Group, some users won't get this data. Clients can control the
        ///     groups they are interested in by using OpChangeGroups.
        /// </remarks>
        /// <param name="eventCode">
        ///     Identifies this type of event (and the content). Your game's event
        ///     codes can start with 0.
        /// </param>
        /// <param name="interestGroup">
        ///     The ID of the interest group this event goes to (exclusively).
        /// </param>
        /// <param name="customEventContent">
        ///     Custom data you want to send along (use null, if none).
        /// </param>
        /// <param name="sendReliable">
        ///     If this event has to arrive reliably (potentially repeated if it's
        ///     lost).
        /// </param>
        /// <returns>
        ///     If operation could be enqueued for sending
        /// </returns>
        [Obsolete("Use overload with RaiseEventOptions to reduce parameter- and overload-clutter.")]
        public virtual bool OpRaiseEvent(byte eventCode, byte interestGroup, Hashtable customEventContent,
            bool sendReliable)
        {
            return OpRaiseEvent(eventCode, sendReliable, customEventContent, 0, EventCaching.DoNotCache, null,
                ReceiverGroup.Others, 0, false);
        }

        /// <summary>
        ///     Used in a room to raise (send) an event to the other players.
        ///     Multiple overloads expose different parameters to this frequently
        ///     used operation. This is an async request will trigger a
        ///     OnOperationResponse() call only in error-cases, because it's called
        ///     many times per second and can hardly fail.
        /// </summary>
        /// <param name="eventCode">
        ///     Code for this "type" of event (assign a code, "meaning" and content
        ///     at will, starting at code 1).
        /// </param>
        /// <param name="evData">
        ///     Data to send. <see cref="Hashtable" /> that contains key-values of
        ///     Photon serializable datatypes.
        /// </param>
        /// <param name="sendReliable">
        ///     Use <see langword="false" /> if the event is replaced by a newer
        ///     rapidly. Reliable events add overhead and add lag when repeated.
        /// </param>
        /// <param name="channelId">
        ///     The "channel" to which this event should belong. Per channel, the
        ///     sequence is kept in order.
        /// </param>
        /// <returns>
        ///     If the operation could be sent (has to be connected).
        /// </returns>
        [Obsolete("Use overload with RaiseEventOptions to reduce parameter- and overload-clutter.")]
        public virtual bool OpRaiseEvent(byte eventCode, Hashtable evData, bool sendReliable, byte channelId)
        {
            return OpRaiseEvent(eventCode, sendReliable, evData, channelId, EventCaching.DoNotCache, null,
                ReceiverGroup.Others, 0, false);
        }

        /// <summary>
        ///     Used in a room to raise (send) an event to the other players.
        ///     Multiple overloads expose different parameters to this frequently
        ///     used operation.
        /// </summary>
        /// <param name="eventCode">
        ///     Code for this "type" of event (use a code per "meaning" or content).
        /// </param>
        /// <param name="evData">
        ///     Data to send. <see cref="Hashtable" /> that contains key-values of
        ///     Photon serializable datatypes.
        /// </param>
        /// <param name="sendReliable">
        ///     Use <see langword="false" /> if the event is replaced by a newer
        ///     rapidly. Reliable events add overhead and add lag when repeated.
        /// </param>
        /// <param name="channelId">
        ///     The "channel" to which this event should belong. Per channel, the
        ///     sequence is kept in order.
        /// </param>
        /// <param name="targetActors">
        ///     Defines the target players who should receive the event (use only
        ///     for small target groups).
        /// </param>
        /// <returns>
        ///     If the operation could be sent (has to be connected).
        /// </returns>
        [Obsolete("Use overload with RaiseEventOptions to reduce parameter- and overload-clutter.")]
        public virtual bool OpRaiseEvent(byte eventCode, Hashtable evData, bool sendReliable, byte channelId,
            int[] targetActors)
        {
            return OpRaiseEvent(eventCode, sendReliable, evData, channelId, EventCaching.DoNotCache, targetActors,
                ReceiverGroup.Others, 0, false);
        }

        /// <summary>
        ///     Used in a room to raise (send) an event to the other players.
        ///     Multiple overloads expose different parameters to this frequently
        ///     used operation.
        /// </summary>
        /// <param name="eventCode">
        ///     Code for this "type" of event (use a code per "meaning" or content).
        /// </param>
        /// <param name="evData">
        ///     Data to send. <see cref="Hashtable" /> that contains key-values of
        ///     Photon serializable datatypes.
        /// </param>
        /// <param name="sendReliable">
        ///     Use <see langword="false" /> if the event is replaced by a newer
        ///     rapidly. Reliable events add overhead and add lag when repeated.
        /// </param>
        /// <param name="channelId">
        ///     The "channel" to which this event should belong. Per channel, the
        ///     sequence is kept in order.
        /// </param>
        /// <param name="targetActors">
        ///     Defines the target players who should receive the event (use only
        ///     for small target groups).
        /// </param>
        /// <param name="cache">
        ///     Use <see cref="EventCaching" /> options to store this event for
        ///     players who join.
        /// </param>
        /// <returns>
        ///     If the operation could be sent (has to be connected).
        /// </returns>
        [Obsolete("Use overload with RaiseEventOptions to reduce parameter- and overload-clutter.")]
        public virtual bool OpRaiseEvent(byte eventCode, Hashtable evData, bool sendReliable, byte channelId,
            int[] targetActors, EventCaching cache)
        {
            return OpRaiseEvent(eventCode, sendReliable, evData, channelId, cache, targetActors,
                ReceiverGroup.Others, 0, false);
        }

        /// <summary>
        ///     Used in a room to raise (send) an event to the other players.
        ///     Multiple overloads expose different parameters to this frequently
        ///     used operation.
        /// </summary>
        /// <param name="eventCode">
        ///     Code for this "type" of event (use a code per "meaning" or content).
        /// </param>
        /// <param name="evData">
        ///     Data to send. <see cref="Hashtable" /> that contains key-values of
        ///     Photon serializable datatypes.
        /// </param>
        /// <param name="sendReliable">
        ///     Use <see langword="false" /> if the event is replaced by a newer
        ///     rapidly. Reliable events add overhead and add lag when repeated.
        /// </param>
        /// <param name="channelId">
        ///     The "channel" to which this event should belong. Per channel, the
        ///     sequence is kept in order.
        /// </param>
        /// <param name="cache">
        ///     Use <see cref="EventCaching" /> options to store this event for
        ///     players who join.
        /// </param>
        /// <param name="receivers">
        ///     <see cref="ReceiverGroup" /> defines to which group of players the
        ///     event is passed on.
        /// </param>
        /// <returns>
        ///     If the operation could be sent (has to be connected).
        /// </returns>
        [Obsolete("Use overload with RaiseEventOptions to reduce parameter- and overload-clutter.")]
        public virtual bool OpRaiseEvent(byte eventCode, Hashtable evData, bool sendReliable, byte channelId,
            EventCaching cache, ReceiverGroup receivers)
        {
            return OpRaiseEvent(eventCode, sendReliable, evData, channelId, cache, null, receivers, 0, false);
        }

        /// <summary>
        ///     Send an event with custom code/type and any content to the other
        ///     players in the same room.
        /// </summary>
        /// <remarks>
        ///     This <see langword="override" /> explicitly uses another parameter
        ///     order to not mix it up with the implementation for
        ///     <see cref="Hashtable" /> only.
        /// </remarks>
        /// <param name="eventCode">
        ///     Identifies this type of event (and the content). Your game's event
        ///     codes can start with 0.
        /// </param>
        /// <param name="customEventContent">
        ///     Any serializable datatype (including <see cref="Hashtable" /> like
        ///     the other <see cref="OpRaiseEvent" /> overloads).
        /// </param>
        /// <param name="sendReliable">
        ///     If this event has to arrive reliably (potentially repeated if it's
        ///     lost).
        /// </param>
        /// <param name="raiseEventOptions">
        ///     Contains (slightly) less often used options. If you pass null, the
        ///     default options will be used.
        /// </param>
        /// <returns>
        ///     If operation could be enqueued for sending. Sent when calling:
        ///     Service or SendOutgoingCommands.
        /// </returns>
        public virtual bool OpRaiseEvent(byte eventCode, object customEventContent, bool sendReliable,
            RaiseEventOptions raiseEventOptions)
        {
            var opParameters = new Dictionary<byte, object> {[ParameterCode.Code] = eventCode};
            if (customEventContent != null)
            {
                opParameters[ParameterCode.Data] = customEventContent;
            }

            if (raiseEventOptions == null)
            {
                raiseEventOptions = RaiseEventOptions.Default;
            }
            else
            {
                if (raiseEventOptions.CachingOption != EventCaching.DoNotCache)
                {
                    opParameters[ParameterCode.Cache] = (byte) raiseEventOptions.CachingOption;
                }
                if (raiseEventOptions.Receivers != ReceiverGroup.Others)
                {
                    opParameters[ParameterCode.ReceiverGroup] = (byte) raiseEventOptions.Receivers;
                }
                if (raiseEventOptions.InterestGroup != 0)
                {
                    opParameters[ParameterCode.Group] = raiseEventOptions.InterestGroup;
                }
                if (raiseEventOptions.TargetActors != null)
                {
                    opParameters[ParameterCode.ActorList] = raiseEventOptions.TargetActors;
                }
                if (raiseEventOptions.ForwardToWebhook)
                {
                    opParameters[ParameterCode.EventForward] = true; //TURNBASED
                }
            }

            return OpCustom(OperationCode.RaiseEvent, opParameters, sendReliable,
                raiseEventOptions.SequenceChannel, false);
        }
    }
}