using System;
using System.Collections.Generic;
using System.Diagnostics;
using ExitGames.Client.Photon;
using Warsmiths.Client.Loadbalancing.Codes;
using Warsmiths.Client.Loadbalancing.Enums;
using Warsmiths.Common;
using ErrorCode = Warsmiths.Client.Loadbalancing.Codes.ErrorCode;
using EventCode = Warsmiths.Client.Loadbalancing.Codes.EventCode;
using OperationCode = Warsmiths.Client.Loadbalancing.Codes.OperationCode;
using ParameterCode = Warsmiths.Client.Loadbalancing.Codes.ParameterCode;

namespace Warsmiths.Client.Loadbalancing
{
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_WII || UNITY_IPHONE || UNITY_ANDROID || UNITY_PS3 || UNITY_XBOX360 || UNITY_NACL  || UNITY_FLASH  || UNITY_BLACKBERRY || UNITY_PSP2 || UNITY_WEBGLusing 
    Hashtable = ExitGames.Client.Photon.Hashtable;
#endif

    /// <summary>
    ///     This class implements the Photon LoadBalancing workflow by using a
    ///     LoadBalancingPeer. It keeps a state and will automatically execute
    ///     transitions between the Master and Game Servers.
    /// </summary>
    /// <remarks>
    ///     This class (and the <see cref="Player" /> class) should be extended to
    ///     implement your own game logic. You can <see langword="override" />
    ///     CreatePlayer as "factory" method for Players and return your own
    ///     <see cref="Player" /> instances. The State of this class is essential to
    ///     know when a client is in a lobby (or just on the master) and when in a
    ///     game where the actual gameplay should take place. Extension notes: An
    ///     extension of this class should <see langword="override" /> the methods of
    ///     the IPhotonPeerListener, as they are called when the state changes. Call
    ///     base.method first, then pick the operation or state you want to react to
    ///     and put it in a switch-case. We try to provide demo to each platform
    ///     where this api can be used, so lookout for those.
    /// </remarks>
    public class LoadBalancingClient : IPhotonPeerListener
    {
        /// <summary>
        ///     Available server (types) for internally used field: server.
        /// </summary>
        public enum ServerConnection
        {
            MasterServer,
            GameServer,
            NameServer
        }

        /// <summary>
        ///     Name <see cref="Server" /> port per protocol (the UDP port is
        ///     different than TCP, etc).
        /// </summary>
        private static readonly Dictionary<ConnectionProtocol, int> ProtocolToNameServerPort =
            new Dictionary<ConnectionProtocol, int>
            {
                {ConnectionProtocol.Udp, 5058},
                {ConnectionProtocol.Tcp, 4533},
                {ConnectionProtocol.WebSocket, 9093},
                {ConnectionProtocol.WebSocketSecure, 19093}
            }; //, { ConnectionProtocol.RHttp, 6063 } };

        /// <summary>
        ///     Backing field for property.
        /// </summary>
        private bool _autoJoinLobby = true;

        private RoomOptions _createRoomOptions; // kept for game server call with same parameters

        /// <summary>
        ///     The current room this client is connected to (null if none
        ///     available).
        /// </summary>
        public Room CurrentRoom;

        /// <summary>
        ///     Internally used to trigger OpAuthenticate when encryption was
        ///     established after a connect.
        /// </summary>
        private bool _didAuthenticate;

        /// <summary>
        ///     If set to true, the Master <see cref="Server" /> will report the list
        ///     of used lobbies to the client. This sets and updates
        ///     LobbyStatistics.
        /// </summary>
        public bool EnableLobbyStatistics;

        /// <summary>
        ///     Contains the list of names of friends to look up their
        ///     <see cref="_state" /> on the server.
        /// </summary>
        private string[] _friendListRequested;

        /// <summary>
        ///     Private timestamp (in ms) of the last friendlist update.
        /// </summary>
        private int _friendListTimestamp;

        /// <summary>
        ///     Internal flag to know if the client currently fetches a friend list.
        /// </summary>
        private bool _isFetchingFriendList;

        /// <summary>
        ///     Internally used in OpJoin to store the actorNumber this client wants
        ///     to claim in the room.
        /// </summary>
        private int _lastJoinActorNumber;

        /// <summary>
        ///     Internally used to decide if a room must be created or joined on
        ///     game server.
        /// </summary>
        private JoinType _lastJoinType;

        /// <summary>
        ///     The client uses a <see cref="LoadBalancingPeer" /> as API to
        ///     communicate with the server. This is <see langword="public" /> for
        ///     ease-of-use: Some methods like <see cref="OpRaiseEvent" /> are not
        ///     relevant for the connection <see cref="_state" /> and don't need a
        ///     override.
        /// </summary>
        public LoadBalancingPeer peer;

        /// <summary>
        ///     Private field for LocalPlayer.
        /// </summary>
        private Player _localPlayer;

        /// <summary>
        ///     Name <see cref="Server" /> Host Name for Photon Cloud. Without port
        ///     and without any prefix.
        /// </summary>
        public string NameServerHost = "ns.exitgames.com";

        /// <summary>
        ///     Name <see cref="Server" /> for HTTP connections to the Photon Cloud.
        ///     Includes prefix and port.
        /// </summary>
        public string NameServerHttp = "http://ns.exitgames.com:80/photon/n";

        /// <summary>
        ///     Register a method to be called when an event got dispatched. Gets
        ///     called at the end of OnEvent().
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This is an alternative to extending LoadBalancingClient to
        ///         <see langword="override" /> OnEvent().
        ///     </para>
        ///     <para>
        ///         Note that <see cref="OnEvent" /> is executing before your Action is
        ///         called. That means for example: Joining players will already be in
        ///         the player list but leaving players will already be removed from the
        ///         room.
        ///     </para>
        /// </remarks>
        public Action<EventData> OnEventAction = eventData => { };

        public Action OnLocalPlayerJoined = () => { };

        /// <summary>
        ///     При изменении свойств игрока
        /// </summary>
        public Action<int, Hashtable> OnPlayerChangeProperties = (actorId, props) => { };

        /// <summary>
        ///     This "list" is populated while being in the lobby of the Master. It
        ///     contains <see cref="RoomInfo" /> per roomName (keys).
        /// </summary>
        public Dictionary<string, RoomInfo> RoomInfoList = new Dictionary<string, RoomInfo>();

        /// <summary>
        ///     Backing field for property.
        /// </summary>
        private ClientState _state = ClientState.Uninitialized;

        public LoadBalancingClient(ConnectionProtocol protocol = ConnectionProtocol.Udp)
        {
#if UNITY_WEBGL
            UnityEngine.Debug.Log("LoadBalancingClient using a new WebSocket LoadBalancingPeer and SocketWebTcp.");
            protocol = ConnectionProtocol.WebSocketSecure;
#endif

            peer = new LoadBalancingPeer(this, protocol);

#if UNITY_WEBGL
            if (protocol == ConnectionProtocol.WebSocket || protocol == ConnectionProtocol.WebSocketSecure) {
                this.peer.SocketImplementation = typeof(SocketWebTcp);
            }
#endif
        }

        public LoadBalancingClient(string masterAddress, string appId, string gameVersion,
            ConnectionProtocol protocol = ConnectionProtocol.Udp) : this(protocol)
        {
            MasterServerAddress = masterAddress;
            AppId = appId;
            AppVersion = gameVersion;
        }

        /// <summary>
        ///     The version of your client. A new version also creates a new
        ///     "virtual app" to separate players from older client versions.
        /// </summary>
        public string AppVersion ;

        /// <summary>
        ///     The AppID as assigned from the Photon Cloud. If you host yourself,
        ///     this is the "regular" Photon <see cref="Server" /> Application Name
        ///     (most likely: "LoadBalancing").
        /// </summary>
        public string AppId ;

        /// <summary>
        ///     A user's authentication values for authentication in Photon.
        /// </summary>
        /// <remarks>
        ///     Set this property or pass <see cref="AuthenticationValues" /> by
        ///     Connect(..., authValues).
        /// </remarks>
        public AuthenticationValues AuthValues ;

        /// <summary>
        ///     True if this client uses a NameServer to get the Master
        ///     <see cref="Server" /> address.
        /// </summary>
        public bool IsUsingNameServer { get; private set; }

        /// <summary>
        ///     Name <see cref="Server" /> Address for Photon Cloud (based on current
        ///     protocol). You can use the default values and usually won't have to
        ///     set this value.
        /// </summary>
        public string NameServerAddress
        {
            get { return GetNameServerAddress(); }
        }

        /// <summary>
        ///     The currently used server address (if any). The type of server is
        ///     define by <see cref="Server" /> property.
        /// </summary>
        public string CurrentServerAddress
        {
            get { return peer.ServerAddress; }
        }


        /// <summary>
        ///     Your Master <see cref="Server" /> address. In PhotonCloud, call
        ///     ConnectToRegionMaster() to find your Master Server.
        /// </summary>
        /// <remarks>
        ///     In the Photon Cloud, explicit definition of a Master
        ///     <see cref="Server" /> Address is not best practice. The Photon Cloud
        ///     has a "Name Server" which redirects clients to a specific Master
        ///     <see cref="Server" /> (per Region and AppId).
        /// </remarks>
        public string MasterServerAddress { get; protected internal set; }

        /// <summary>
        ///     The game server's address for a particular room. In use temporarily,
        ///     as assigned by master.
        /// </summary>
        private string GameServerAddress ;

        /// <summary>
        ///     The server this client is currently connected or connecting to.
        /// </summary>
        /// <remarks>
        ///     Each server (NameServer, MasterServer, GameServer) allow some
        ///     operations and reject others.
        /// </remarks>
        public ServerConnection Server { get; private set; }

        /// <summary>
        ///     Current <see cref="_state" /> this client is in. Careful: several
        ///     states are "transitions" that lead to other states.
        /// </summary>
        public ClientState State
        {
            get { return _state; }

            protected internal set
            {
                _state = value;
                if (OnStateChangeAction != null) OnStateChangeAction(_state);
            }
        }

        /// <summary>
        ///     Returns if this client is currently connected or connecting to some
        ///     type of server.
        /// </summary>
        /// <remarks>
        ///     This is even <see langword="true" /> while switching servers. Use
        ///     <see cref="IsConnectedAndReady" /> to check only for those states
        ///     that enable you to send Operations.
        /// </remarks>
        public bool IsConnected
        {
            get
            {
                return peer != null && State != ClientState.Uninitialized &&
                       State != ClientState.Disconnected;
            }
        }


        /// <summary>
        ///     A refined version of <see cref="IsConnected" /> which is
        ///     <see langword="true" /> only if your connection to the server is
        ///     ready to accept operations.
        /// </summary>
        /// <remarks>
        ///     Which operations are available, depends on the Server. For example,
        ///     the NameServer allows <see cref="OpGetRegions" /> which is not
        ///     available anywhere else. The MasterServer does not allow you to send
        ///     events (OpRaiseEvent) and on the GameServer you are unable to join a
        ///     lobby (OpJoinLobby). <see cref="Uri.Check" /> which server you are on
        ///     with PhotonNetwork.Server.
        /// </remarks>
        public bool IsConnectedAndReady
        {
            get
            {
                if (peer == null)
                {
                    return false;
                }

                switch (State)
                {
                    case ClientState.Uninitialized:
                    case ClientState.Disconnected:
                    case ClientState.Disconnecting:
                    case ClientState.Authenticating:
                    case ClientState.ConnectingToGameserver:
                    case ClientState.ConnectingToMasterserver:
                    case ClientState.ConnectingToNameServer:
                    case ClientState.Joining:
                    case ClientState.Leaving:
                        return false; // we are not ready to execute any operations
                }

                return true;
            }
        }

        /// <summary>
        ///     Register a method to be called when this client's
        ///     <see cref="ClientState" /> gets set.
        /// </summary>
        /// <remarks>
        ///     This can be useful to react to being connected, joined into a room,
        ///     etc.
        /// </remarks>
        public Action<ClientState> OnStateChangeAction ;

        /// <summary>
        ///     Register a method to be called when this client's
        ///     <see cref="ClientState" /> gets set.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This is an alternative to extending LoadBalancingClient to
        ///         <see langword="override" /> OnOperationResponse().
        ///     </para>
        ///     <para>
        ///         Note that <see cref="OnOperationResponse" /> gets executed before
        ///         your Action is called. That means for example: The
        ///         <see cref="OpJoinLobby" /> response already set the
        ///         <see cref="_state" /> to "JoinedLobby" and the response to OpLeave
        ///         already triggered the <see cref="Disconnect" /> before this is
        ///         called.
        ///     </para>
        /// </remarks>
        public Action<OperationResponse> OnOpResponseAction ;

        /// <summary>
        ///     Summarizes (aggregates) the different causes for disconnects of a
        ///     client.
        /// </summary>
        /// <remarks>
        ///     A disconnect can be caused by: errors in the network connection or
        ///     some vital operation failing (which is considered "high level").
        ///     While operations always trigger a call to OnOperationResponse,
        ///     connection related changes are treated in OnStatusChanged. The
        ///     <see cref="DisconnectCause" /> is set in either case and summarizes
        ///     the causes for any disconnect in a single <see cref="_state" /> value
        ///     which can be used to display (or debug) the cause for disconnection.
        /// </remarks>
        public DisconnectCause DisconnectedCause { get; protected set; }

        /// <summary>
        ///     The lobby this client currently uses.
        /// </summary>
        public TypedLobby CurrentLobby { get; protected internal set; }

        /// <summary>
        ///     The name of the lobby this client currently uses.
        /// </summary>
        [Obsolete("Use CurrentLobby.Name")]
        public string CurrentLobbyName
        {
            get { return CurrentLobby.Name; }
        }

        /// <summary>
        ///     The type of the lobby this client currently uses. There are
        ///     "default" and "SQL" typed lobbies. See: LobbyType.
        /// </summary>
        [Obsolete("Use CurrentLobby.Type")]
        public LobbyType CurrentLobbyType
        {
            get { return CurrentLobby.Type; }
        }

        /// <summary>
        ///     If your client should join random games, you can skip joining the
        ///     lobby. Call <see cref="OpJoinRandomRoom" /> and create a room if that
        ///     fails.
        /// </summary>
        public bool AutoJoinLobby
        {
            get { return _autoJoinLobby; }

            set { _autoJoinLobby = value; }
        }

        /// <summary>
        ///     Internally used as easy check if we should request lobby statistics
        ///     from "this" particular server.
        /// </summary>
        private bool RequestLobbyStatistics
        {
            get { return EnableLobbyStatistics && Server == ServerConnection.MasterServer; }
        }

        /// <summary>
        ///     If RequestLobbyStatistics is true, this provides a list of used
        ///     lobbies (their name, type, room- and player-count) of this
        ///     application, while on the Master Server.
        /// </summary>
        public List<TypedLobbyInfo> LobbyStatistics { get; } = new List<TypedLobbyInfo>();

        /// <summary>
        ///     Same as NickName.
        /// </summary>
        [Obsolete("Use NickName instead.")]
        public string PlayerName
        {
            get { return NickName; }

            set { NickName = value; }
        }

        /// <summary>
        ///     The nickname of the player (synced with others). Same as
        ///     client.LocalPlayer.NickName.
        /// </summary>
        public string NickName
        {
            get { return LocalPlayer.NickName; }

            set
            {
                if (LocalPlayer == null)
                {
                    return;
                }

                LocalPlayer.NickName = value;
            }
        }

        /// <summary>
        ///     An ID for this user. Sent in OpAuthenticate when you connect. If not
        ///     set, the <see cref="PlayerName" /> is applied during connect.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         On connect, if the <see cref="UserId" /> is <see langword="null" /> or
        ///         empty, the client will copy the PlayName to UserId. If
        ///         <see cref="PlayerName" /> is not set either (before connect), the
        ///         server applies a temporary ID which stays unknown to this client and
        ///         other clients.
        ///     </para>
        ///     <para>
        ///         The <see cref="UserId" /> is what's used in FindFriends and for
        ///         fetching data for your account (with WebHooks e.g.).
        ///     </para>
        ///     <para>
        ///         By convention, set this ID before you connect, not while being
        ///         connected. There is no error but the ID won't change while being
        ///         connected.
        ///     </para>
        /// </remarks>
        public string UserId
        {
            get
            {
                if (AuthValues != null)
                {
                    return AuthValues.UserId;
                }
                return null;
            }
            set
            {
                if (AuthValues == null)
                {
                    AuthValues = new AuthenticationValues();
                }
                AuthValues.UserId = value;
            }
        }

        /// <summary>
        ///     The local player is never <see langword="null" /> but not valid
        ///     unless the client is in a room, too. The ID will be -1 outside of
        ///     rooms.
        /// </summary>
        public Player LocalPlayer
        {
            get
            {
                if (_localPlayer == null)
                {
                    _localPlayer = CreatePlayer(string.Empty, -1, true, null);
                }

                return _localPlayer;
            }

            set { _localPlayer = value; }
        }

        /// <summary>
        ///     Statistic value available on master server: Players on master
        ///     (looking for games).
        /// </summary>
        public int PlayersOnMasterCount ;

        /// <summary>
        ///     Statistic value available on master server: Players in rooms
        ///     (playing).
        /// </summary>
        public int PlayersInRoomsCount ;

        /// <summary>
        ///     Statistic value available on master server: Rooms currently created.
        /// </summary>
        public int RoomsCount ;

        /// <summary>
        ///     List of friends, their online status and the room they are in. Null
        ///     until initialized by <see cref="OpFindFriends" /> response.
        /// </summary>
        /// <remarks>
        ///     Do not modify this list! It's internally handled by
        ///     <see cref="OpFindFriends" /> and meant as read-only. The value of
        ///     <see cref="FriendListAge" /> gives you a hint how old the data is.
        ///     Don't get this list more often than useful (> 10 seconds). In best
        ///     case, keep the list you fetch really short. You could (e.g.) get the
        ///     full list only once, then request a few updates only for friends who
        ///     are online. After a while (e.g. 1 minute), you can get the full list
        ///     again.
        /// </remarks>
        public List<FriendInfo> FriendList { get; private set; }

        /// <summary>
        ///     Age of friend list info (in milliseconds). It's 0 until a friend
        ///     list is fetched.
        /// </summary>
        public int FriendListAge
        {
            get
            {
                return _isFetchingFriendList || _friendListTimestamp == 0
                    ? 0
                    : Environment.TickCount - _friendListTimestamp;
            }
        }

        /// <summary>
        ///     Internally used to check if a "Secret" is available to use. Sent by
        ///     Photon Cloud servers, it simplifies authentication when switching
        ///     servers.
        /// </summary>
        protected bool IsAuthorizeSecretAvailable
        {
            get { return AuthValues != null && !string.IsNullOrEmpty(AuthValues.Token); }
        }

        /// <summary>
        ///     A list of region names for the Photon Cloud. Set by the result of
        ///     OpGetRegions().
        /// </summary>
        /// <remarks>
        ///     Put a "case OperationCode.GetRegions:" into your
        ///     <see cref="OnOperationResponse" /> method to notice when the result
        ///     is available.
        /// </remarks>
        public string[] AvailableRegions { get; private set; }

        /// <summary>
        ///     A list of region server (IP addresses with port) for the Photon
        ///     Cloud. Set by the result of OpGetRegions().
        /// </summary>
        /// <remarks>
        ///     Put a "case OperationCode.GetRegions:" into your
        ///     <see cref="OnOperationResponse" /> method to notice when the result
        ///     is available.
        /// </remarks>
        public string[] AvailableRegionsServers { get; private set; }

        /// <summary>
        ///     The cloud region this client connects to. Set by
        ///     ConnectToRegionMaster(). Not set if you don't use a NameServer!
        /// </summary>
        public string CloudRegion { get; private set; }

        /// <summary>
        ///     Gets the NameServer Address (with prefix and port), based on the set
        ///     protocol (this.peer.UsedProtocol).
        /// </summary>
        /// <returns>
        ///     NameServer Address (with prefix and port).
        /// </returns>
        private string GetNameServerAddress()
        {
            var currentProtocol = peer.UsedProtocol;

#if RHTTP
            if (currentProtocol == ConnectionProtocol.RHttp)
            {
                return NameServerHttp;
            }
#endif

            var protocolPort = 0;
            ProtocolToNameServerPort.TryGetValue(currentProtocol, out protocolPort);

            var protocolPrefix = string.Empty;
            if (currentProtocol == ConnectionProtocol.WebSocket)
            {
                protocolPrefix = "ws://";
            }
            else if (currentProtocol == ConnectionProtocol.WebSocketSecure)
            {
                protocolPrefix = "wss://";
            }

            return string.Format("{0}{1}:{2}", protocolPrefix, NameServerHost, protocolPort);
        }

        #region Operations and Commands

        /// <summary>
        ///     Starts the "process" to connect to the master server. Relevant
        ///     connection-values parameters can be set via parameters.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The process to connect includes several steps: the actual
        ///         connecting, establishing encryption, authentification (of app and
        ///         optionally the user) and joining a lobby (if
        ///         <see cref="AutoJoinLobby" /> is true).
        ///     </para>
        ///     <para>
        ///         Instead of providing all these parameters, you can also set the
        ///         individual properties of a client before calling Connect().
        ///     </para>
        ///     <para>
        ///         Users can connect either anonymously or use "Custom Authentication"
        ///         to verify each individual player's login. Custom Authentication in
        ///         Photon uses external services and communities to verify users. While
        ///         the client provides a user's info, the service setup is done in the
        ///         Photon Cloud Dashboard. The parameter <paramref name="authValues" />
        ///         will set this.AuthValues and use them in the connect process.
        ///     </para>
        ///     <para>
        ///         To connect to the Photon Cloud, a valid <see cref="AppId" /> must be
        ///         provided. This is shown in the Photon Cloud Dashboard.
        ///         https://cloud.photonengine.com/dashboard Connecting to the Photon
        ///         Cloud might fail due to:
        ///     </para>
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 Network issues (OnStatusChanged() StatusCode.ExceptionOnConnect)
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 Region not available (OnOperationResponse() for OpAuthenticate with
        ///                 ReturnCode == ErrorCode.InvalidRegion)
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 Subscription CCU limit reached (OnOperationResponse() for
        ///                 OpAuthenticate with ReturnCode == ErrorCode.MaxCcuReached) More
        ///                 about the connection limitations:
        ///                 http://doc.photonengine.com/photon-cloud/SubscriptionErrorCases/#cat-references
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <param name="masterServerAddress">
        ///     Set a master server address instead of using the default. Uses
        ///     default if <see langword="null" /> or empty.
        /// </param>
        /// <param name="appId">
        ///     Your application's name or the AppID assigned by Photon Cloud (as
        ///     listed in Dashboard). Uses default if <see langword="null" /> or
        ///     empty.
        /// </param>
        /// <param name="appVersion">
        ///     Can be used to separate users by their client's version (useful to
        ///     add features without breaking older clients). Uses default if
        ///     <see langword="null" /> or empty.
        /// </param>
        /// <param name="nickName">Optional name for this player.</param>
        /// <param name="authValues">
        ///     Authentication values for this user. Optional. If you provide a
        ///     unique userID it is used for FindFriends.
        /// </param>
        /// <returns>
        ///     If the operation could be send (can be <see langword="false" /> for
        ///     bad server urls).
        /// </returns>
        public bool Connect(string masterServerAddress, string appId, string appVersion, string nickName,
            AuthenticationValues authValues)
        {
            peer.DisconnectTimeout = 60000;

            if (!string.IsNullOrEmpty(masterServerAddress))
            {
                MasterServerAddress = masterServerAddress;
            }

            if (!string.IsNullOrEmpty(appId))
            {
                AppId = appId;
            }

            if (!string.IsNullOrEmpty(appVersion))
            {
                AppVersion = appVersion;
            }

            if (!string.IsNullOrEmpty(nickName))
            {
                NickName = nickName;
            }

            AuthValues = authValues;

            return Connect();
        }

        /// <summary>
        ///     Starts the "process" to connect to a Master Server, using
        ///     <see cref="MasterServerAddress" /> and <see cref="AppId" />
        ///     properties.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         To connect to the Photon Cloud, use ConnectToRegionMaster().
        ///     </para>
        ///     <para>
        ///         The process to connect includes several steps: the actual
        ///         connecting, establishing encryption, authentification (of app and
        ///         optionally the user) and joining a lobby (if
        ///         <see cref="AutoJoinLobby" /> is true).
        ///     </para>
        ///     <para>
        ///         Users can connect either anonymously or use "Custom Authentication"
        ///         to verify each individual player's login. Custom Authentication in
        ///         Photon uses external services and communities to verify users. While
        ///         the client provides a user's info, the service setup is done in the
        ///         Photon Cloud Dashboard. The parameter authValues will set
        ///         this.AuthValues and use them in the connect process.
        ///     </para>
        ///     <para>Connecting to the Photon Cloud might fail due to:</para>
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 Network issues (OnStatusChanged() StatusCode.ExceptionOnConnect)
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 Region not available (OnOperationResponse() for OpAuthenticate with
        ///                 ReturnCode == ErrorCode.InvalidRegion)
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 Subscription CCU limit reached (OnOperationResponse() for
        ///                 OpAuthenticate with ReturnCode == ErrorCode.MaxCcuReached) More
        ///                 about the connection limitations:
        ///                 http://doc.photonengine.com/photon-cloud/SubscriptionErrorCases/#cat-references
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        public virtual bool Connect()
        {
            DisconnectedCause = DisconnectCause.None;

            if (peer.Connect(MasterServerAddress, AppId))
            {
                State = ClientState.ConnectingToMasterserver;
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Connects to the NameServer for Photon Cloud, where a region and
        ///     server list can be obtained.
        /// </summary>
        /// <returns>
        ///     If the workflow was started or failed right away.
        /// </returns>
        /// <see cref="M:Warsmiths.Client.Loadbalancing.LoadBalancingClient.OpGetRegions" />
        public bool ConnectToNameServer()
        {
            IsUsingNameServer = true;
            CloudRegion = null;
            if (!peer.Connect(NameServerAddress, "NameServer"))
            {
                return false;
            }

            State = ClientState.ConnectingToNameServer;
            return true;
        }

        /// <summary>
        ///     Connects you to a specific region's Master Server, using the Name
        ///     <see cref="Server" /> to find the IP.
        /// </summary>
        /// <returns>
        ///     If the operation could be sent. If false, no operation was sent.
        /// </returns>
        public bool ConnectToRegionMaster(string region)
        {
            IsUsingNameServer = true;

            if (State == ClientState.ConnectedToNameServer)
            {
                CloudRegion = region;
                return peer.OpAuthenticate(AppId, AppVersion, AuthValues, region,
                    RequestLobbyStatistics);
            }

            CloudRegion = region;
            if (!peer.Connect(NameServerAddress, "NameServer"))
            {
                return false;
            }

            State = ClientState.ConnectingToNameServer;
            return true;
        }

        /// <summary>
        ///     Disconnects this client from any server and sets this.State if the
        ///     connection is successfuly closed.
        /// </summary>
        public void Disconnect()
        {
            if (State != ClientState.Disconnected)
            {
                State = ClientState.Disconnecting;
                peer.Disconnect();

                // we can set this high-level state if the low-level (connection)state is "disconnected"
                if (peer.PeerState == PeerStateValue.Disconnected ||
                    peer.PeerState == PeerStateValue.InitializingApplication)
                {
                    State = ClientState.Disconnected;
                }
            }
        }

        /// <summary>
        ///     This method excutes dispatches all incoming commands and sends
        ///     anything in the outgoing queues via DispatchIncomingCommands and
        ///     SendOutgoingCommands.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The Photon client libraries are designed to fit easily into a game
        ///         or application. The application is in control of the context
        ///         (thread) in which incoming events and responses are executed and has
        ///         full control of the creation of UDP/TCP packages.
        ///     </para>
        ///     <para>
        ///         Sending packages and dispatching received messages are two separate
        ///         tasks. <see cref="Service" /> combines them into one method at the
        ///         cost of control. It calls DispatchIncomingCommands and
        ///         SendOutgoingCommands.
        ///     </para>
        ///     <para>Call this method regularly (2..20 times a second).</para>
        ///     <para>
        ///         This will Dispatch ANY received commands (unless a reliable command
        ///         in-order is still missing) and events AND will send queued outgoing
        ///         commands. Fewer calls might be more effective if a device cannot
        ///         send many packets per second, as multiple operations might be
        ///         combined into one package.
        ///     </para>
        /// </remarks>
        /// <example>
        ///     <para>You could replace <see cref="Service" /> by:</para>
        ///     <para>
        ///         while (DispatchIncomingCommands()); //Dispatch until everything is
        ///         Dispatched... SendOutgoingCommands(); //Send a UDP/TCP package with
        ///         outgoing messages
        ///     </para>
        /// </example>
        /// <seealso cref="M:ExitGames.Client.Photon.PhotonPeer.DispatchIncomingCommands" />
        /// <seealso cref="M:ExitGames.Client.Photon.PhotonPeer.SendOutgoingCommands" />
        public void Service()
        {
            if (peer != null)
            {
                peer.Service();
            }
        }

        /// <summary>
        ///     Private <see cref="Disconnect" /> variant that sets the state, too.
        /// </summary>
        private void DisconnectToReconnect()
        {
            switch (Server)
            {
                case ServerConnection.NameServer:
                    State = ClientState.DisconnectingFromNameServer;
                    break;
                case ServerConnection.MasterServer:
                    State = ClientState.DisconnectingFromMasterserver;
                    break;
                case ServerConnection.GameServer:
                    State = ClientState.DisconnectingFromGameserver;
                    break;
            }

            peer.Disconnect();
        }

        /// <summary>
        ///     Privately used only. Starts the "process" to connect to the game
        ///     server (connect before a game is joined).
        /// </summary>
        private bool ConnectToGameServer()
        {
            if (peer.Connect(GameServerAddress, AppId))
            {
                State = ClientState.ConnectingToGameserver;
                return true;
            }

            // TODO: handle error "cant connect to GS"
            return false;
        }

        /// <summary>
        ///     While on the NameServer, this gets you the list of regional servers
        ///     (short names and their IPs to ping them).
        /// </summary>
        /// <returns>
        ///     If the operation could be sent. If false, no operation was sent
        ///     (e.g. while not connected to the NameServer).
        /// </returns>
        public bool OpGetRegions()
        {
            if (Server != ServerConnection.NameServer)
            {
                return false;
            }

            var sent = peer.OpGetRegions(AppId);
            if (sent)
            {
                AvailableRegions = null;
            }

            return sent;
        }

        /// <summary>
        ///     Joins the lobby on the Master Server, where you get a list of
        ///     RoomInfos of currently open rooms.
        /// </summary>
        /// <returns>
        ///     If the operation could be sent (has to be connected).
        /// </returns>
        [Obsolete("Use alternative with TypedLobby parameter.")]
        public bool OpJoinLobby(string name, LobbyType lobbyType)
        {
            var lobby = new TypedLobby(name, lobbyType);
            var sent = peer.OpJoinLobby(lobby);
            if (sent)
            {
                CurrentLobby = lobby;
            }

            return sent;
        }

        /// <summary>
        ///     Joins the <paramref name="lobby" /> on the Master Server, where you
        ///     get a list of RoomInfos of currently open rooms. This is an async
        ///     request which triggers a OnOperationResponse() call.
        /// </summary>
        /// <param name="lobby">
        ///     The lobby join to. Use <see langword="null" /> for default lobby.
        /// </param>
        /// <returns>
        ///     If the operation could be sent (has to be connected).
        /// </returns>
        public bool OpJoinLobby(TypedLobby lobby)
        {
            if (lobby == null)
            {
                lobby = TypedLobby.Default;
            }
            var sent = peer.OpJoinLobby(lobby);
            if (sent)
            {
                CurrentLobby = lobby;
            }

            return sent;
        }

        /// <summary>
        ///     Opposite of joining a lobby. You don't have to explicitly leave a
        ///     lobby to join another (client can be in one max, at any time).
        /// </summary>
        /// <returns>
        ///     If the operation could be sent (has to be connected).
        /// </returns>
        public bool OpLeaveLobby()
        {
            return peer.OpLeaveLobby();
        }

        /// <summary>
        ///     Operation to join a random room if available. You can use room
        ///     properties to filter accepted rooms.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         You can use <paramref name="expectedCustomRoomProperties" /> and
        ///         <paramref name="expectedMaxPlayers" /> as filters for accepting
        ///         rooms. If you set expectedCustomRoomProperties, a room must have the
        ///         exact same key values set at Custom Properties. You need to define
        ///         which Custom <see cref="Room" /> Properties will be available for
        ///         matchmaking when you create a room. See: OpCreateRoom(string
        ///         roomName, <see cref="RoomOptions" /> roomOptions,
        ///         <see cref="TypedLobby" /> lobby)
        ///     </para>
        ///     <para>
        ///         This operation fails if no rooms are fitting or available (all full,
        ///         closed or not visible). Override this class and implement
        ///         OnOperationResponse(OperationResponse operationResponse).
        ///     </para>
        ///     <para>
        ///         <see cref="OpJoinRandomRoom" /> can only be called while the client
        ///         is connected to a Master Server. You should check
        ///         LoadBalancingClient.Server and
        ///         LoadBalancingClient.IsConnectedAndReady before calling this method.
        ///         Alternatively, check the returned <see langword="bool" /> value.
        ///     </para>
        ///     <para>
        ///         While the server is looking for a game, the State will be Joining.
        ///         It's set immediately when this method sent the Operation.
        ///     </para>
        ///     <para>
        ///         If successful, the LoadBalancingClient will get a Game
        ///         <see cref="Server" /> Address and use it automatically to
        ///         <see langword="switch" /> servers and join the room. When you're in
        ///         the room, this client's State will become ClientState.Joined (both,
        ///         for joining or creating it). Set a <see cref="OnStateChangeAction" />
        ///         method to check for states.
        ///     </para>
        ///     <para>
        ///         When joining a room, this client's <see cref="Player" /> Custom
        ///         Properties will be sent to the room. Use
        ///         LocalPlayer.SetCustomProperties to set them, even while not yet in
        ///         the room. Note that the player properties will be cached locally and
        ///         sent to any next room you would join, too.
        ///     </para>
        ///     <para>
        ///         More about matchmaking:
        ///         http://doc.photonengine.com/en/realtime/current/reference/matchmaking-and-lobby
        ///     </para>
        /// </remarks>
        /// <param name="expectedCustomRoomProperties">
        ///     Optional. A room will only be joined, if it matches these custom
        ///     properties (with string keys).
        /// </param>
        /// <param name="expectedMaxPlayers">
        ///     Filters for a particular maxplayer setting. Use 0 to accept any
        ///     maxPlayer value.
        /// </param>
        /// <returns>
        ///     If the operation could be sent currently (requires connection to
        ///     Master Server).
        /// </returns>
        public bool OpJoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers)
        {
            return OpJoinRandomRoom(expectedCustomRoomProperties, expectedMaxPlayers, MatchmakingMode.FillRoom,
                TypedLobby.Default, null);
        }

        /// <summary>
        ///     Operation to join a random room if available. You can use room
        ///     properties to filter accepted rooms.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         You can use <paramref name="expectedCustomRoomProperties" /> and
        ///         <paramref name="expectedMaxPlayers" /> as filters for accepting
        ///         rooms. If you set expectedCustomRoomProperties, a room must have the
        ///         exact same key values set at Custom Properties. You need to define
        ///         which Custom <see cref="Room" /> Properties will be available for
        ///         matchmaking when you create a room. See: OpCreateRoom(string
        ///         roomName, <see cref="RoomOptions" /> roomOptions,
        ///         <see cref="TypedLobby" /> lobby)
        ///     </para>
        ///     <para>
        ///         This operation fails if no rooms are fitting or available (all full,
        ///         closed or not visible). Override this class and implement
        ///         OnOperationResponse(OperationResponse operationResponse).
        ///     </para>
        ///     <para>
        ///         <see cref="OpJoinRandomRoom" /> can only be called while the client
        ///         is connected to a Master Server. You should check
        ///         LoadBalancingClient.Server and
        ///         LoadBalancingClient.IsConnectedAndReady before calling this method.
        ///         Alternatively, check the returned <see langword="bool" /> value.
        ///     </para>
        ///     <para>
        ///         While the server is looking for a game, the State will be Joining.
        ///         It's set immediately when this method sent the Operation.
        ///     </para>
        ///     <para>
        ///         If successful, the LoadBalancingClient will get a Game
        ///         <see cref="Server" /> Address and use it automatically to
        ///         <see langword="switch" /> servers and join the room. When you're in
        ///         the room, this client's State will become ClientState.Joined (both,
        ///         for joining or creating it). Set a <see cref="OnStateChangeAction" />
        ///         method to check for states.
        ///     </para>
        ///     <para>
        ///         When joining a room, this client's <see cref="Player" /> Custom
        ///         Properties will be sent to the room. Use
        ///         LocalPlayer.SetCustomProperties to set them, even while not yet in
        ///         the room. Note that the player properties will be cached locally and
        ///         sent to any next room you would join, too.
        ///     </para>
        ///     <para>
        ///         More about matchmaking:
        ///         http://doc.photonengine.com/en/realtime/current/reference/matchmaking-and-lobby
        ///     </para>
        /// </remarks>
        /// <param name="expectedCustomRoomProperties">
        ///     Optional. A room will only be joined, if it matches these custom
        ///     properties (with string keys).
        /// </param>
        /// <param name="expectedMaxPlayers">
        ///     Filters for a particular maxplayer setting. Use 0 to accept any
        ///     maxPlayer value.
        /// </param>
        /// <param name="matchmakingMode">
        ///     Selects one of the available matchmaking algorithms. See
        ///     <see cref="MatchmakingMode" /> <see langword="enum" /> for options.
        /// </param>
        /// <returns>
        ///     If the operation could be sent currently (requires connection to
        ///     Master Server).
        /// </returns>
        public bool OpJoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers,
            MatchmakingMode matchmakingMode)
        {
            return OpJoinRandomRoom(expectedCustomRoomProperties, expectedMaxPlayers, matchmakingMode,
                TypedLobby.Default, null);
        }

        /// <summary>
        ///     Operation to join a random, available room.
        /// </summary>
        /// <returns>
        ///     If the operation could be sent (has to be connected).
        /// </returns>
        [Obsolete("Use overload with TypedLobby Parameter")]
        public bool OpJoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers,
            MatchmakingMode matchmakingMode, string lobbyName, LobbyType lobbyType, string sqlLobbyFilter)
        {
            return OpJoinRandomRoom(expectedCustomRoomProperties, expectedMaxPlayers, matchmakingMode,
                new TypedLobby {Name = lobbyName, Type = lobbyType}, sqlLobbyFilter);
        }

        /// <summary>
        ///     Operation to join a random room if available. You can use room
        ///     properties to filter accepted rooms.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         You can use <paramref name="expectedCustomRoomProperties" /> and
        ///         <paramref name="expectedMaxPlayers" /> as filters for accepting
        ///         rooms. If you set expectedCustomRoomProperties, a room must have the
        ///         exact same key values set at Custom Properties. You need to define
        ///         which Custom <see cref="Room" /> Properties will be available for
        ///         matchmaking when you create a room. See: OpCreateRoom(string
        ///         roomName, <see cref="RoomOptions" /> roomOptions,
        ///         <see cref="TypedLobby" /> lobby)
        ///     </para>
        ///     <para>
        ///         This operation fails if no rooms are fitting or available (all full,
        ///         closed or not visible). Override this class and implement
        ///         OnOperationResponse(OperationResponse operationResponse).
        ///     </para>
        ///     <para>
        ///         <see cref="OpJoinRandomRoom" /> can only be called while the client
        ///         is connected to a Master Server. You should check
        ///         LoadBalancingClient.Server and
        ///         LoadBalancingClient.IsConnectedAndReady before calling this method.
        ///         Alternatively, check the returned <see langword="bool" /> value.
        ///     </para>
        ///     <para>
        ///         While the server is looking for a game, the State will be Joining.
        ///         It's set immediately when this method sent the Operation.
        ///     </para>
        ///     <para>
        ///         If successful, the LoadBalancingClient will get a Game
        ///         <see cref="Server" /> Address and use it automatically to
        ///         <see langword="switch" /> servers and join the room. When you're in
        ///         the room, this client's State will become ClientState.Joined (both,
        ///         for joining or creating it). Set a <see cref="OnStateChangeAction" />
        ///         method to check for states.
        ///     </para>
        ///     <para>
        ///         When joining a room, this client's <see cref="Player" /> Custom
        ///         Properties will be sent to the room. Use
        ///         LocalPlayer.SetCustomProperties to set them, even while not yet in
        ///         the room. Note that the player properties will be cached locally and
        ///         sent to any next room you would join, too.
        ///     </para>
        ///     <para>
        ///         The parameter <paramref name="lobby" /> can be <see langword="null" />
        ///         (using the defaul lobby) or a typed <paramref name="lobby" /> you
        ///         make up. Lobbies are created on the fly, as required by the clients.
        ///         If you organize matchmaking with lobbies, keep in mind that they
        ///         also fragment your matchmaking. Using more lobbies will put less
        ///         rooms in each.
        ///     </para>
        ///     <para>
        ///         The parameter <paramref name="sqlLobbyFilter" /> can only be combined
        ///         with the LobbyType.SqlLobby. In that case, it's used to define a
        ///         sql-like "WHERE" clause for filtering rooms. This is useful for
        ///         skill-based matchmaking e.g..
        ///     </para>
        ///     <para>
        ///         More about matchmaking:
        ///         http://doc.photonengine.com/en/realtime/current/reference/matchmaking-and-lobby
        ///     </para>
        /// </remarks>
        /// <param name="expectedCustomRoomProperties">
        ///     Optional. A room will only be joined, if it matches these custom
        ///     properties (with string keys).
        /// </param>
        /// <param name="expectedMaxPlayers">
        ///     Filters for a particular maxplayer setting. Use 0 to accept any
        ///     maxPlayer value.
        /// </param>
        /// <param name="matchmakingMode">
        ///     Selects one of the available matchmaking algorithms. See
        ///     <see cref="MatchmakingMode" /> <see langword="enum" /> for options.
        /// </param>
        /// <param name="lobby">
        ///     The lobby in which to find a room. Use <see langword="null" /> for
        ///     default lobby.
        /// </param>
        /// <param name="sqlLobbyFilter">
        ///     Can be used with LobbyType.SqlLobby only. This is a "where" clause
        ///     of a sql statement. Use <see langword="null" /> for random game.
        /// </param>
        /// <returns>
        ///     If the operation could be sent currently (requires connection to
        ///     Master Server).
        /// </returns>
        public bool OpJoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers,
            MatchmakingMode matchmakingMode, TypedLobby lobby, string sqlLobbyFilter)
        {
            if (lobby == null)
            {
                lobby = TypedLobby.Default;
            }
            State = ClientState.Joining;
            _lastJoinType = JoinType.JoinRandomRoom;
            _lastJoinActorNumber = 0;
            CurrentRoom = CreateRoom(null, new RoomOptions());

            Hashtable playerPropsToSend = null;
            if (Server == ServerConnection.GameServer)
            {
                playerPropsToSend = LocalPlayer.AllProperties;
            }

            CurrentLobby = lobby;
            return peer.OpJoinRandomRoom(expectedCustomRoomProperties, expectedMaxPlayers,
                playerPropsToSend, matchmakingMode, lobby, sqlLobbyFilter);
        }

        /// <summary>
        ///     Joins a room by roomName. Useful when using room lists in lobbies or
        ///     when you know the name otherwise.
        /// </summary>
        /// <param name="roomName">
        ///     The name of the room to join. Must be existing already, open and
        ///     non-full or can't be joined.
        /// </param>
        /// <param name="createIfNotExists">
        ///     If true, the server will attempt to create a room.
        /// </param>
        /// <returns>
        ///     If the operation could be sent (has to be connected).
        /// </returns>
        [Obsolete(
            "We introduced OpJoinOrCreateRoom to make the parameter createIfNotExists obsolete. Use either OpJoinRoom or OpJoinOrCreateRoom."
            )]
        public bool OpJoinRoom(string roomName, bool createIfNotExists)
        {
            return OpJoinRoom(roomName, createIfNotExists, 0);
        }

        /// <summary>
        ///     Joins a room by roomName. Useful when using room lists in lobbies or
        ///     when you know the name otherwise.
        /// </summary>
        [Obsolete(
            "We introduced OpJoinOrCreateRoom to make the parameter createIfNotExists obsolete. Use either OpJoinRoom or OpJoinOrCreateRoom."
            )]
        public bool OpJoinRoom(string roomName, bool createIfNotExists, int actorNumber)
        {
            if (createIfNotExists)
            {
                return OpJoinOrCreateRoom(roomName, actorNumber, null);
            }
            return OpJoinRoom(roomName, actorNumber);
        }

        /// <summary>
        ///     Joins a room by roomName. Useful when using room lists in lobbies or
        ///     when you know the name otherwise.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This method is useful when you are using a lobby to list rooms and
        ///         know their names. A room's name has to be unique (per region and
        ///         game version), so it does not matter which lobby it's in.
        ///     </para>
        ///     <para>
        ///         If the room is full, closed or not existing, this will fail.
        ///         Override this class and implement
        ///         OnOperationResponse(OperationResponse operationResponse) to get the
        ///         errors.
        ///     </para>
        ///     <para>
        ///         <see cref="OpJoinRoom" /> can only be called while the client is
        ///         connected to a Master Server. You should check
        ///         LoadBalancingClient.Server and
        ///         LoadBalancingClient.IsConnectedAndReady before calling this method.
        ///         Alternatively, check the returned <see langword="bool" /> value.
        ///     </para>
        ///     <para>
        ///         While the server is joining the game, the State will be
        ///         ClientState.Joining. It's set immediately when this method sends the
        ///         Operation.
        ///     </para>
        ///     <para>
        ///         If successful, the LoadBalancingClient will get a Game
        ///         <see cref="Server" /> Address and use it automatically to
        ///         <see langword="switch" /> servers and join the room. When you're in
        ///         the room, this client's State will become ClientState.Joined (both,
        ///         for joining or creating it). Set a <see cref="OnStateChangeAction" />
        ///         method to check for states.
        ///     </para>
        ///     <para>
        ///         When joining a room, this client's <see cref="Player" /> Custom
        ///         Properties will be sent to the room. Use
        ///         LocalPlayer.SetCustomProperties to set them, even while not yet in
        ///         the room. Note that the player properties will be cached locally and
        ///         sent to any next room you would join, too.
        ///     </para>
        ///     <para>
        ///         It's usually better to use <see cref="OpJoinOrCreateRoom" /> for
        ///         invitations. Then it does not matter if the room is already setup.
        ///     </para>
        /// </remarks>
        /// <param name="roomName">
        ///     The name of the room to join. Must be existing already, open and
        ///     non-full or can't be joined.
        /// </param>
        /// <returns>
        ///     If the operation could be sent currently (requires connection to
        ///     Master Server).
        /// </returns>
        public bool OpJoinRoom(string roomName)
        {
            return OpJoinRoom(roomName, 0);
        }

        /// <summary>
        ///     Joins a room by roomName. If this client returns to the room, set
        ///     the previously used Player.ID as actorNumber.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This method is useful when you are using a lobby to list rooms and
        ///         know their names. A room's name has to be unique (per region and
        ///         game version), so it does not matter which lobby it's in.
        ///     </para>
        ///     <para>
        ///         If this client returns to the room, set the previously used
        ///         Player.ID as actorNumber. When you are using Custom Authentication
        ///         with unique user IDs, the server will use the userID to find the
        ///         previously assigned <paramref name="actorNumber" /> in the room.
        ///     </para>
        ///     <para>
        ///         For turnbased games, this is especially useful as rooms can be
        ///         continued after hours or days. To return to a room, set the
        ///         <paramref name="actorNumber" /> to anything but 0. It's best practice
        ///         to use -1 with Custom Authentication and unique user accounts.
        ///     </para>
        ///     <para>
        ///         If the room is full, closed or not existing, this will fail.
        ///         Override this class and implement
        ///         OnOperationResponse(OperationResponse operationResponse) to get the
        ///         errors.
        ///     </para>
        ///     <para>
        ///         <see cref="OpJoinRoom" /> can only be called while the client is
        ///         connected to a Master Server. You should check
        ///         LoadBalancingClient.Server and
        ///         LoadBalancingClient.IsConnectedAndReady before calling this method.
        ///         Alternatively, check the returned <see langword="bool" /> value.
        ///     </para>
        ///     <para>
        ///         While the server is joining the game, the State will be
        ///         ClientState.Joining. It's set immediately when this method sends the
        ///         Operation.
        ///     </para>
        ///     <para>
        ///         If successful, the LoadBalancingClient will get a Game
        ///         <see cref="Server" /> Address and use it automatically to
        ///         <see langword="switch" /> servers and join the room. When you're in
        ///         the room, this client's State will become ClientState.Joined (both,
        ///         for joining or creating it). Set a <see cref="OnStateChangeAction" />
        ///         method to check for states.
        ///     </para>
        ///     <para>
        ///         When joining a room, this client's <see cref="Player" /> Custom
        ///         Properties will be sent to the room. Use
        ///         LocalPlayer.SetCustomProperties to set them, even while not yet in
        ///         the room. Note that the player properties will be cached locally and
        ///         sent to any next room you would join, too.
        ///     </para>
        ///     <para>
        ///         It's usually better to use <see cref="OpJoinOrCreateRoom" /> for
        ///         invitations. Then it does not matter if the room is already setup.
        ///     </para>
        /// </remarks>
        /// <param name="roomName">
        ///     The name of the room to join. Must be existing already, open and
        ///     non-full or can't be joined.
        /// </param>
        /// <param name="actorNumber">
        ///     When returning to a room, use a non-0 value. For Turnbased games,
        ///     set the previously assigned Player.ID or -1 when using Custom
        ///     Authentication.
        /// </param>
        /// <returns>
        ///     If the operation could be sent currently (requires connection to
        ///     Master Server).
        /// </returns>
        public bool OpJoinRoom(string roomName, int actorNumber)
        {
            State = ClientState.Joining;
            _lastJoinType = JoinType.JoinRoom;
            _lastJoinActorNumber = actorNumber;
            _createRoomOptions = new RoomOptions();
            CurrentRoom = CreateRoom(roomName, _createRoomOptions);

            Hashtable playerPropsToSend = null;
            if (Server == ServerConnection.GameServer)
            {
                playerPropsToSend = LocalPlayer.AllProperties;
            }

            var onGameServer = Server == ServerConnection.GameServer;
            return peer.OpJoinRoom(roomName, playerPropsToSend, actorNumber, _createRoomOptions,
                null, false, onGameServer);
        }

        /// <summary>
        ///     Joins a room by name or creates new room if room with given name not
        ///     exists.
        /// </summary>
        /// <remarks>
        ///     Join will try to enter a room by roomName. Unlike OpJoinRoom, this
        ///     will create the room if it doesn't exist.
        /// </remarks>
        /// <param name="roomName">
        ///     The name of the room to join. Must be existing already, open and
        ///     non-full or can't be joined.
        /// </param>
        /// <param name="actorNumber">
        ///     An actorNumber to claim in room in case the client re-joins a room.
        ///     Use 0 to not claim an actorNumber.
        /// </param>
        /// <param name="roomOptions">
        ///     Contains the parameters and properties of the new room. See
        ///     <see cref="RoomOptions" /> class for a description of each.
        /// </param>
        /// <returns>
        ///     If the operation could be sent (has to be connected).
        /// </returns>
        [Obsolete("Use version with TypedLobby parameter.")]
        public bool OpJoinOrCreateRoom(string roomName, int actorNumber, RoomOptions roomOptions)
        {
            return OpJoinOrCreateRoom(roomName, actorNumber, roomOptions, null);
        }

        /// <summary>
        ///     Joins a specific room by name. If the room does not exist (yet), it
        ///     will be created implicitly.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Unlike OpJoinRoom, this operation does not fail if the room does not
        ///         exist. This can be useful when you send invitations to a room before
        ///         actually creating it: Any invited player (whoever is first) can call
        ///         this and on demand, the room gets created implicitly.
        ///     </para>
        ///     <para>
        ///         If you set room properties in RoomOptions, they get ignored when the
        ///         room is existing already. This avoids changing the room properties
        ///         by late joining players. Only when the room gets created, the
        ///         <see cref="RoomOptions" /> are set in this case.
        ///     </para>
        ///     <para>
        ///         If this client returns to the room, set the previously used
        ///         Player.ID as actorNumber. When you are using Custom Authentication
        ///         with unique user IDs, the server will use the userID to find the
        ///         previously assigned <paramref name="actorNumber" /> in the room.
        ///     </para>
        ///     <para>
        ///         For turnbased games, this is especially useful as rooms can be
        ///         continued after hours or days. To return to a room, set the
        ///         <paramref name="actorNumber" /> to anything but 0. It's best practice
        ///         to use -1 with Custom Authentication and unique user accounts.
        ///     </para>
        ///     <para>
        ///         If the room is full or closed, this will fail. Override this class
        ///         and implement OnOperationResponse(OperationResponse
        ///         operationResponse) to get the errors.
        ///     </para>
        ///     <para>
        ///         This method can only be called while the client is connected to a
        ///         Master Server. You should check LoadBalancingClient.Server and
        ///         LoadBalancingClient.IsConnectedAndReady before calling this method.
        ///         Alternatively, check the returned <see langword="bool" /> value.
        ///     </para>
        ///     <para>
        ///         While the server is joining the game, the State will be
        ///         ClientState.Joining. It's set immediately when this method sends the
        ///         Operation.
        ///     </para>
        ///     <para>
        ///         If successful, the LoadBalancingClient will get a Game
        ///         <see cref="Server" /> Address and use it automatically to
        ///         <see langword="switch" /> servers and join the room. When you're in
        ///         the room, this client's State will become ClientState.Joined (both,
        ///         for joining or creating it). Set a <see cref="OnStateChangeAction" />
        ///         method to check for states.
        ///     </para>
        ///     <para>
        ///         When entering the room, this client's <see cref="Player" /> Custom
        ///         Properties will be sent to the room. Use
        ///         LocalPlayer.SetCustomProperties to set them, even while not yet in
        ///         the room. Note that the player properties will be cached locally and
        ///         sent to any next room you would join, too.
        ///     </para>
        /// </remarks>
        /// <param name="roomName">
        ///     The name of the room to join (might be created implicitly).
        /// </param>
        /// <param name="actorNumber">
        ///     When returning to a room, use a non-0 value. For Turnbased games,
        ///     set the previously assigned Player.ID or -1 when using Custom
        ///     Authentication.
        /// </param>
        /// <param name="roomOptions">
        ///     Contains the parameters and properties of the new room. See
        ///     <see cref="RoomOptions" /> class for a description of each.
        /// </param>
        /// <param name="lobby">
        ///     Typed lobby to be used if the roomname is not in use (and room gets
        ///     created). If != null, it will also set CurrentLobby.
        /// </param>
        /// <returns>
        ///     If the operation could be sent currently (requires connection to
        ///     Master Server).
        /// </returns>
        public bool OpJoinOrCreateRoom(string roomName, int actorNumber, RoomOptions roomOptions, TypedLobby lobby)
        {
            if (roomOptions == null)
            {
                roomOptions = new RoomOptions();
            }
            State = ClientState.Joining;
            _lastJoinType = JoinType.JoinOrCreateRoom;
            _lastJoinActorNumber = actorNumber;
            _createRoomOptions = roomOptions;
            CurrentRoom = CreateRoom(roomName, roomOptions);

            Hashtable playerPropsToSend = null;
            if (Server == ServerConnection.GameServer)
            {
                playerPropsToSend = LocalPlayer.AllProperties;
            }

            if (lobby != null) CurrentLobby = lobby;
            var onGameServer = Server == ServerConnection.GameServer;
            return peer.OpJoinRoom(roomName, playerPropsToSend, actorNumber, roomOptions, lobby, true,
                onGameServer);
        }

        /// <summary>
        ///     Creates a new room on the server (or fails when the name is already
        ///     taken).
        /// </summary>
        [Obsolete("Use overload with RoomOptions parameter.")]
        public bool OpCreateRoom(string roomName, byte maxPlayers, Hashtable customGameProperties,
            string[] propsListedInLobby)
        {
            return OpCreateRoom(roomName, true, true, maxPlayers, customGameProperties, propsListedInLobby, null,
                LobbyType.Default);
        }

        /// <summary>
        ///     Creates a new room on the server (or fails when the name is already
        ///     taken).
        /// </summary>
        [Obsolete("Use overload with RoomOptions parameter.")]
        public bool OpCreateRoom(string roomName, bool isVisible, bool isOpen, byte maxPlayers,
            Hashtable customGameProperties, string[] propsListedInLobby)
        {
            return OpCreateRoom(roomName, isVisible, isOpen, maxPlayers, customGameProperties, propsListedInLobby,
                null, LobbyType.Default);
        }

        /// <summary>
        ///     Creates a new room on the server (or fails when the name is already
        ///     taken).
        /// </summary>
        [Obsolete("Use overload with RoomOptions parameter.")]
        public bool OpCreateRoom(string roomName, bool isVisible, bool isOpen, byte maxPlayers,
            Hashtable customGameProperties, string[] propsListedInLobby, string lobbyName, LobbyType lobbyType)
        {
            return OpCreateRoom(roomName, isVisible, isOpen, maxPlayers, customGameProperties, propsListedInLobby,
                null, LobbyType.Default, 0, 0);
        }

        /// <summary>
        ///     Creates a new room on the server (or fails when the name is already
        ///     taken).
        /// </summary>
        /// <returns>
        ///     If the operation could be sent (has to be connected).
        /// </returns>
        [Obsolete("Use overload with RoomOptions parameter.")]
        public bool OpCreateRoom(string roomName, bool isVisible, bool isOpen, byte maxPlayers,
            Hashtable customGameProperties, string[] propsListedInLobby, string lobbyName, LobbyType lobbyType,
            int playerTtl, int roomTtl)
        {
            State = ClientState.Joining;
            _lastJoinType = JoinType.CreateRoom;
            _lastJoinActorNumber = 0;
            _createRoomOptions = new RoomOptions
            {
                IsVisible = isVisible,
                IsOpen = isOpen,
                MaxPlayers = maxPlayers,
                PlayerTtl = playerTtl,
                EmptyRoomTtl = roomTtl,
                CleanupCacheOnLeave = true,
                CustomRoomProperties = customGameProperties,
                CustomRoomPropertiesForLobby = propsListedInLobby
            };
            CurrentRoom = CreateRoom(roomName, _createRoomOptions);

            Hashtable playerPropsToSend = null;
            if (Server == ServerConnection.GameServer)
            {
                playerPropsToSend = LocalPlayer.AllProperties;
            }

            CurrentLobby = new TypedLobby(lobbyName, lobbyType);

            var onGameServer = Server == ServerConnection.GameServer;
            return peer.OpCreateRoom(roomName, _createRoomOptions,
                new TypedLobby(lobbyName, lobbyType), playerPropsToSend, onGameServer);
        }


        /// <summary>
        ///     Creates a new room on the server (or fails if the name is already in
        ///     use).
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         If you don't want to create a unique room-name, pass
        ///         <see langword="null" /> or "" as name and the server will assign a
        ///         <paramref name="roomName" /> (a GUID as string). <see cref="Room" />
        ///         names are unique.
        ///     </para>
        ///     <para>
        ///         A room will be attached to the specified lobby. Use
        ///         <see langword="null" /> as <paramref name="lobby" /> to attach the
        ///         room to the <paramref name="lobby" /> you are now in. If you are in
        ///         no lobby, the default <paramref name="lobby" /> is used.
        ///     </para>
        ///     <para>
        ///         Multiple lobbies can help separate players by map or skill or game
        ///         type. Each room can only be found in one <paramref name="lobby" />
        ///         (no matter if defined by name and type or as default).
        ///     </para>
        ///     <para>
        ///         This method can only be called while the client is connected to a
        ///         Master Server. You should check LoadBalancingClient.Server and
        ///         LoadBalancingClient.IsConnectedAndReady before calling this method.
        ///         Alternatively, check the returned <see langword="bool" /> value.
        ///     </para>
        ///     <para>
        ///         Even when sent, the Operation will fail (on the server) if the
        ///         <paramref name="roomName" /> is in use. Override this class and
        ///         implement OnOperationResponse(OperationResponse operationResponse)
        ///         to get the errors.
        ///     </para>
        ///     <para>
        ///         While the server is creating the game, the State will be
        ///         ClientState.Joining. The <see cref="_state" /> Joining is used because
        ///         the client is on the way to enter a room (no matter if joining or
        ///         creating). It's set immediately when this method sends the
        ///         Operation.
        ///     </para>
        ///     <para>
        ///         If successful, the LoadBalancingClient will get a Game
        ///         <see cref="Server" /> Address and use it automatically to
        ///         <see langword="switch" /> servers and enter the room. When you're in
        ///         the room, this client's State will become ClientState.Joined (both,
        ///         for joining or creating it). Set a <see cref="OnStateChangeAction" />
        ///         method to check for states.
        ///     </para>
        ///     <para>
        ///         When entering the room, this client's <see cref="Player" /> Custom
        ///         Properties will be sent to the room. Use
        ///         LocalPlayer.SetCustomProperties to set them, even while not yet in
        ///         the room. Note that the player properties will be cached locally and
        ///         sent to any next room you would join, too.
        ///     </para>
        /// </remarks>
        /// <param name="roomName">
        ///     The name to create a room with. Must be unique and not in use or
        ///     can't be created. If null, the server will assign a GUID as name.
        /// </param>
        /// <param name="roomOptions">
        ///     Contains the parameters and properties of the new room. See
        ///     <see cref="RoomOptions" /> class for a description of each.
        /// </param>
        /// <param name="lobby">
        ///     The lobby (name and type) in which to create the room. Null uses the
        ///     current lobby or the default lobby (if not in a lobby).
        /// </param>
        /// <returns>
        ///     If the operation could be sent currently (requires connection to
        ///     Master Server).
        /// </returns>
        public bool OpCreateRoom(string roomName, RoomOptions roomOptions, TypedLobby lobby)
        {
            State = ClientState.Joining;
            _lastJoinType = JoinType.CreateRoom;
            _lastJoinActorNumber = 0;
            _createRoomOptions = roomOptions;
            CurrentRoom = CreateRoom(roomName, roomOptions);

            Hashtable playerPropsToSend = null;
            if (Server == ServerConnection.GameServer)
            {
                playerPropsToSend = LocalPlayer.AllProperties;
            }

            CurrentLobby = lobby;
            var onGameServer = Server == ServerConnection.GameServer;
            return peer.OpCreateRoom(roomName, roomOptions, lobby, playerPropsToSend, onGameServer);
        }

        /// <summary>
        ///     Leaves the <see cref="CurrentRoom" /> and returns to the Master
        ///     server (back to the lobby). <see cref="OpLeaveRoom" /> skips
        ///     execution when the room is <see langword="null" /> or the server is
        ///     not GameServer or the client is disconnecting from GS already.
        ///     <see cref="OpLeaveRoom" /> returns <see langword="false" /> in those
        ///     cases and won't change the state, so check return of this method.
        /// </summary>
        /// <remarks>
        ///     This method actually is not an operation per se. It sets a
        ///     <see cref="_state" /> and calls Disconnect(). This is is quicker than
        ///     calling OpLeave and then disconnect (which also triggers a leave).
        /// </remarks>
        /// <returns>
        ///     If the current room could be left (impossible while not in a room).
        /// </returns>
        public bool OpLeaveRoom()
        {
            return OpLeaveRoom(false); //TURNBASED
        }

        /// <summary>
        ///     Leaves the <see cref="CurrentRoom" /> and returns to the Master
        ///     server (back to the lobby). <see cref="OpLeaveRoom" /> skips
        ///     execution when the room is <see langword="null" /> or the server is
        ///     not GameServer or the client is disconnecting from GS already.
        ///     <see cref="OpLeaveRoom" /> returns <see langword="false" /> in those
        ///     cases and won't change the state, so check return of this method.
        /// </summary>
        /// <remarks>
        ///     This method actually is not an operation per se. It sets a
        ///     <see cref="_state" /> and calls Disconnect(). This is is quicker than
        ///     calling OpLeave and then disconnect (which also triggers a leave).
        /// </remarks>
        /// <param name="willReturnLater"></param>
        /// <returns>
        ///     If the current room could be left (impossible while not in a room).
        /// </returns>
        public bool OpLeaveRoom(bool willReturnLater)
        {
            if (CurrentRoom == null || Server != ServerConnection.GameServer ||
                State == ClientState.DisconnectingFromGameserver)
            {
                return false;
            }

            if (willReturnLater)
            {
                State = ClientState.DisconnectingFromGameserver;
                peer.Disconnect();
            }
            else
            {
                State = ClientState.Leaving;
                peer.OpLeaveRoom(false); //TURNBASED users can leave a room forever or return later
            }

            return true;
        }

        /// <summary>
        ///     Request the rooms and online status for a list of friends. All
        ///     clients should set a unique <see cref="UserId" /> before connecting.
        ///     The result is available in this.FriendList.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Used on Master <see cref="Server" /> to find the rooms played by a
        ///         selected list of users. The result will be stored in
        ///         LoadBalancingClient.FriendList, which is <see langword="null" />
        ///         before the first server response.
        ///     </para>
        ///     <para>
        ///         Users identify themselves by setting a <see cref="UserId" /> in the
        ///         LoadBalancingClient instance. This will send the ID in
        ///         OpAuthenticate during connect (to master and game servers). Note:
        ///         Changing a player's name doesn't make sense when using a friend
        ///         list.
        ///     </para>
        ///     <para>
        ///         The list of usernames must be fetched from some other source (not
        ///         provided by Photon).
        ///     </para>
        ///     <para>
        ///         Internal: The server response includes 2 arrays of info (each index
        ///         matching a friend from the request):
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
        public bool OpFindFriends(string[] friendsToFind)
        {
            if (peer == null)
            {
                return false;
            }

            if (_isFetchingFriendList || Server == ServerConnection.GameServer)
            {
                return false;
                // fetching friends currently, so don't do it again (avoid changing the list while fetching friends)
            }

            _isFetchingFriendList = true;
            _friendListRequested = friendsToFind;
            return peer.OpFindFriends(friendsToFind);
        }


        /// <summary>
        ///     Updates and synchronizes a Player's Custom Properties. Optionally,
        ///     <paramref name="expectedProperties" /> can be provided as condition.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Custom Properties are a set of string keys and arbitrary values
        ///         which is synchronized for the players in a Room. They are available
        ///         when the client enters the room, as they are in the response of
        ///         OpJoin and OpCreate.
        ///     </para>
        ///     <para>
        ///         Custom Properties either relate to the (current) <see cref="Room" />
        ///         or a <see cref="Player" /> (in that Room).
        ///     </para>
        ///     <para>
        ///         Both classes locally cache the current key/values and make them
        ///         available as property: CustomProperties. This is provided only to
        ///         read them. You must use the method SetCustomProperties to set/modify
        ///         them.
        ///     </para>
        ///     <para>
        ///         Any client can set any Custom Properties anytime. It's up to the
        ///         game logic to organize how they are best used.
        ///     </para>
        ///     <para>
        ///         You should call SetCustomProperties only with key/values that are
        ///         new or changed. This reduces traffic and performance.
        ///     </para>
        ///     <para>
        ///         Unless you define some expectedProperties, setting key/values is
        ///         always permitted. In this case, the property-setting client will not
        ///         receive the new values from the server but instead update its local
        ///         cache in SetCustomProperties.
        ///     </para>
        ///     <para>
        ///         If you define expectedProperties, the server will skip updates if
        ///         the server property-cache does not contain all
        ///         <paramref name="expectedProperties" /> with the same values. In this
        ///         case, the property-setting client will get an update from the server
        ///         and update it's cached key/values at about the same time as everyone
        ///         else.
        ///     </para>
        ///     <para>
        ///         The benefit of using <paramref name="expectedProperties" /> can be
        ///         only one client successfully sets a key from one known value to
        ///         another. As example: Store who owns an item in a Custom Property
        ///         "ownedBy". It's 0 initally. When multiple players reach the item,
        ///         they all attempt to change "ownedBy" from 0 to their actorNumber. If
        ///         you use <paramref name="expectedProperties" /> {"ownedBy", 0} as
        ///         condition, the first player to take the item will have it (and the
        ///         others fail to set the ownership).
        ///     </para>
        ///     <para>
        ///         Properties get saved with the game <see cref="_state" /> for Turnbased
        ///         games (which use IsPersistent = true).
        ///     </para>
        /// </remarks>
        /// <param name="actorNr">
        ///     Defines which player the Custom Properties belong to. ActorID of a
        ///     player.
        /// </param>
        /// <param name="propertiesToSet">
        ///     <see cref="Hashtable" /> of Custom Properties that changes.
        /// </param>
        /// <param name="expectedProperties">
        ///     Provide some keys/values to use as condition for setting the new
        ///     values.
        /// </param>
        public bool OpSetCustomPropertiesOfActor(int actorNr, Hashtable propertiesToSet,
            Hashtable expectedProperties = null)
        {
            var customActorProperties = new Hashtable();
            customActorProperties.MergeStringKeys(propertiesToSet);

            return OpSetPropertiesOfActor(actorNr, customActorProperties, expectedProperties);
        }

        /// <summary>
        ///     Internally used to cache and set properties (including well known
        ///     properties).
        /// </summary>
        protected internal bool OpSetPropertiesOfActor(int actorNr, Hashtable actorProperties,
            Hashtable expectedProperties = null)
        {
            if (expectedProperties == null || expectedProperties.Count == 0)
            {
                var target = CurrentRoom.GetPlayer(actorNr);
                if (target != null)
                {
                    target.CacheProperties(actorProperties);
                }
            }

            return peer.OpSetPropertiesOfActor(actorNr, actorProperties, expectedProperties);
        }


        /// <summary>
        ///     See OpSetCustomPropertiesOfRoom(Hashtable gameProperties,
        ///     <see langword="bool" /> webForward, <see cref="Hashtable" />
        ///     <paramref name="expectedProperties" /> = null).
        /// </summary>
        [Obsolete(
            "Use: OpSetCustomPropertiesOfRoom(Hashtable gameProperties, bool webForward, Hashtable expectedProperties = null)"
            )]
        public bool OpSetCustomPropertiesOfRoom(Hashtable gameProperties, Hashtable expectedProperties = null)
        {
            return OpSetCustomPropertiesOfRoom(gameProperties, false, expectedProperties);
        }

        /// <summary>
        ///     Updates and synchronizes this Room's Custom Properties. Optionally,
        ///     <paramref name="expectedProperties" /> can be provided as condition.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Custom Properties are a set of string keys and arbitrary values
        ///         which is synchronized for the players in a Room. They are available
        ///         when the client enters the room, as they are in the response of
        ///         OpJoin and OpCreate.
        ///     </para>
        ///     <para>
        ///         Custom Properties either relate to the (current) <see cref="Room" />
        ///         or a <see cref="Player" /> (in that Room).
        ///     </para>
        ///     <para>
        ///         Both classes locally cache the current key/values and make them
        ///         available as property: CustomProperties. This is provided only to
        ///         read them. You must use the method SetCustomProperties to set/modify
        ///         them.
        ///     </para>
        ///     <para>
        ///         Any client can set any Custom Properties anytime. It's up to the
        ///         game logic to organize how they are best used.
        ///     </para>
        ///     <para>
        ///         You should call SetCustomProperties only with key/values that are
        ///         new or changed. This reduces traffic and performance.
        ///     </para>
        ///     <para>
        ///         Unless you define some expectedProperties, setting key/values is
        ///         always permitted. In this case, the property-setting client will not
        ///         receive the new values from the server but instead update its local
        ///         cache in SetCustomProperties.
        ///     </para>
        ///     <para>
        ///         If you define expectedProperties, the server will skip updates if
        ///         the server property-cache does not contain all
        ///         <paramref name="expectedProperties" /> with the same values. In this
        ///         case, the property-setting client will get an update from the server
        ///         and update it's cached key/values at about the same time as everyone
        ///         else.
        ///     </para>
        ///     <para>
        ///         The benefit of using <paramref name="expectedProperties" /> can be
        ///         only one client successfully sets a key from one known value to
        ///         another. As example: Store who owns an item in a Custom Property
        ///         "ownedBy". It's 0 initally. When multiple players reach the item,
        ///         they all attempt to change "ownedBy" from 0 to their actorNumber. If
        ///         you use <paramref name="expectedProperties" /> {"ownedBy", 0} as
        ///         condition, the first player to take the item will have it (and the
        ///         others fail to set the ownership).
        ///     </para>
        ///     <para>
        ///         Properties get saved with the game <see cref="_state" /> for Turnbased
        ///         games (which use IsPersistent = true).
        ///     </para>
        /// </remarks>
        /// <param name="propertiesToSet">
        ///     <see cref="Hashtable" /> of Custom Properties that changes.
        /// </param>
        /// <param name="webForward">
        ///     Defines if the set properties should be forwarded to a WebHook.
        /// </param>
        /// <param name="expectedProperties">
        ///     Provide some keys/values to use as condition for setting the new
        ///     values.
        /// </param>
        public bool OpSetCustomPropertiesOfRoom(Hashtable propertiesToSet, bool webForward,
            Hashtable expectedProperties = null)
        {
            var customGameProps = new Hashtable();
            customGameProps.MergeStringKeys(propertiesToSet);

            return OpSetPropertiesOfRoom(customGameProps, webForward, expectedProperties);
        }

        /// <summary>
        ///     Internally used to cache and set properties (including well known
        ///     properties).
        /// </summary>
        protected internal bool OpSetPropertiesOfRoom(Hashtable gameProperties, bool webForward,
            Hashtable expectedProperties = null)
        {
            if (expectedProperties == null || expectedProperties.Count == 0)
            {
                CurrentRoom.CacheProperties(gameProperties);
            }
            return peer.OpSetPropertiesOfRoom(gameProperties, webForward, expectedProperties);
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
        ///     <see cref="Service" /> or SendOutgoingCommands.
        /// </returns>
        public virtual bool OpRaiseEvent(byte eventCode, object customEventContent, bool sendReliable,
            RaiseEventOptions raiseEventOptions)
        {
            if (peer == null)
            {
                return false;
            }

            return peer.OpRaiseEvent(eventCode, customEventContent, sendReliable, raiseEventOptions);
        }

        /// <summary>
        ///     This operation makes Photon call your custom web-service by
        ///     path/name with the given <paramref name="parameters" /> (converted
        ///     into JSon).
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This is a "Photon Turnbased" feature and you have to setup your
        ///         Photon Cloud Application prior to use.
        ///     </para>
        ///     <para>
        ///         The response by Photon will call OnOperationResponse() with Code:
        ///         OperationCode.WebRpc. It's important to understand that the
        ///         OperationResponse tells you if the WebRPC could be called or not but
        ///         the content of the response will contain the values the web-service
        ///         sent (if any). If the web-service could not execute the request, it
        ///         might return another error and a message. This is inside of the
        ///         (wrapping) OperationResponse. The class WebRpcResponse is a
        ///         helper-class that extracts the most valuable content from the WebRPC
        ///         response.
        ///     </para>
        ///     <para>
        ///         See the Turnbased Feature Overview for a short intro.
        ///         http://doc.photonengine.com/en/turnbased/current/getting-started/feature-overview
        ///     </para>
        /// </remarks>
        /// <example>
        ///     <para>
        ///         It's best to inherit a LoadBalancingClient into your own. Implement
        ///         something like:
        ///     </para>
        ///     <para>
        ///         <see langword="public" /> <see langword="override" />
        ///         <see langword="void" /> OnOperationResponse(OperationResponse
        ///         operationResponse) { base.OnOperationResponse(operationResponse); //
        ///         important to call, to keep <see cref="_state" /> up to date
        ///     </para>
        ///     <para>
        ///         <see langword="switch" /> (operationResponse.OperationCode) { case
        ///         OperationCode.WebRpc:
        ///     </para>
        ///     <para>
        ///         if (operationResponse.ReturnCode != 0) {
        ///         DebugReturn(DebugLevel.ERROR, "WebRpc failed. Response: " +
        ///         operationResponse.ToStringFull()); } else { WebRpcResponse
        ///         webResponse = new WebRpcResponse(operationResponse);
        ///         this.OnWebRpcResponse(webResponse); } break;
        ///     </para>
        ///     <para>// more code [...]</para>
        /// </example>
        public bool OpWebRpc(string uriPath, object parameters)
        {
            var opParameters = new Dictionary<byte, object>
            {
                {ParameterCode.UriPath, uriPath},
                {ParameterCode.WebRpcParameters, parameters}
            };

            return peer.OpCustom(OperationCode.WebRpc, opParameters, true);
        }

        #endregion

        #region Helpers

        /// <summary>
        ///     Privately used to read-out properties coming from the server in
        ///     events and operation responses (which might be a bit tricky).
        /// </summary>
        private void ReadoutProperties(Hashtable gameProperties, Hashtable actorProperties, int targetActorNr)
        {
            // Debug.LogWarning("ReadoutProperties game=" + gameProperties + " actors(" + actorProperties + ")=" + actorProperties + " " + targetActorNr);
            // read game properties and cache them locally
            if (CurrentRoom != null && gameProperties != null)
            {
                CurrentRoom.CacheProperties(gameProperties);
            }

            if (actorProperties != null && actorProperties.Count > 0)
            {
                if (targetActorNr > 0)
                {
                    // we have a single entry in the actorProperties with one user's name
                    // targets MUST exist before you set properties
                    var target = CurrentRoom.GetPlayer(targetActorNr);
                    if (target != null)
                    {
                        target.CacheProperties(ReadoutPropertiesForActorNr(actorProperties, targetActorNr));
                    }
                }
                else
                {
                    // in this case, we've got a key-value pair per actor (each
                    // value is a hashtable with the actor's properties then)

                    foreach (var key in actorProperties.Keys)
                    {
                        var actorNr = (int) key;
                        var props = (Hashtable) actorProperties[key];
                        var newName = (string) props[ActorProperties.PlayerName];

                        var target = CurrentRoom.GetPlayer(actorNr);
                        if (target == null)
                        {
                            target = CreatePlayer(newName, actorNr, false, props);
                            CurrentRoom.StorePlayer(target);
                        }
                        else
                        {
                            target.CacheProperties(props);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Privately used only to read properties for a distinct actor (which
        ///     might be the hashtable OR a key-pair value IN the actorProperties).
        /// </summary>
        private Hashtable ReadoutPropertiesForActorNr(Hashtable actorProperties, int actorNr)
        {
            if (actorProperties.ContainsKey(actorNr))
            {
                return (Hashtable) actorProperties[actorNr];
            }

            return actorProperties;
        }

        /// <summary>
        ///     Internally used to set the LocalPlayer's ID (from -1 to the actual
        ///     in-room ID).
        /// </summary>
        /// <param name="newId">
        ///     New actor ID (a.k.a actorNr) assigned when joining a room.
        /// </param>
        protected internal void ChangeLocalID(int newId)
        {
            if (LocalPlayer == null)
            {
                DebugReturn(DebugLevel.WARNING,
                    string.Format(
                        "Local actor is null or not in mActors! mLocalActor: {0} mActors==null: {1} newID: {2}",
                        LocalPlayer, CurrentRoom.Players == null, newId));
            }

            if (CurrentRoom == null)
            {
                // change to new actor/player ID and make sure the player does not have a room reference left
                LocalPlayer.ChangeLocalID(newId);
                LocalPlayer.RoomReference = null;
            }
            else
            {
                // remove old actorId from actor list
                CurrentRoom.RemovePlayer(LocalPlayer);

                // change to new actor/player ID
                LocalPlayer.ChangeLocalID(newId);

                // update the room's list with the new reference
                CurrentRoom.StorePlayer(LocalPlayer);

                // make this client known to the local player (used to get state and to sync values from within Player)
                LocalPlayer.LoadBalancingClient = this;
            }
        }

        /// <summary>
        ///     Internally used to clean up local instances of players and room.
        /// </summary>
        private void CleanCachedValues()
        {
            ChangeLocalID(-1);
            _isFetchingFriendList = false;

            // if this is called on the gameserver, we clean the room we were in. on the master, we keep the room to get into it
            if (Server == ServerConnection.GameServer || State == ClientState.Disconnecting ||
                State == ClientState.Uninitialized)
            {
                CurrentRoom = null; // players get cleaned up inside this, too, except LocalPlayer (which we keep)
            }

            // when we leave the master, we clean up the rooms list (which might be updated by the lobby when we join again)
            if (Server == ServerConnection.MasterServer || State == ClientState.Disconnecting ||
                State == ClientState.Uninitialized)
            {
                RoomInfoList.Clear();
            }
        }

        /// <summary>
        ///     Called internally, when a game was joined or created on the game
        ///     server. This reads the response, finds out the local player's
        ///     actorNumber (a.k.a. Player.ID) and applies properties of the room
        ///     and players.
        /// </summary>
        /// <param name="operationResponse">
        ///     Contains the server's response for an operation called by this peer.
        /// </param>
        private void GameEnteredOnGameServer(OperationResponse operationResponse)
        {
            if (operationResponse.ReturnCode != 0)
            {
                switch (operationResponse.OperationCode)
                {
                    case OperationCode.CreateGame:
                        DebugReturn(DebugLevel.ERROR,
                            "Create failed on GameServer. Changing back to MasterServer. ReturnCode: " +
                            operationResponse.ReturnCode);
                        break;
                    case OperationCode.JoinGame:
                    case OperationCode.JoinRandomGame:
                        DebugReturn(DebugLevel.ERROR, "Join failed on GameServer. Changing back to MasterServer.");

                        if (operationResponse.ReturnCode == ErrorCode.GameDoesNotExist)
                        {
                            DebugReturn(DebugLevel.INFO,
                                "Most likely the game became empty during the switch to GameServer.");
                        }

                        // TODO: add callback to join failed
                        break;
                }

                DisconnectToReconnect();
                return;
            }

            State = ClientState.Joined;


            CurrentRoom.LoadBalancingClient = this;
            CurrentRoom.IsLocalClientInside = true;

            // the local player's actor-properties are not returned in join-result. add this player to the list
            var localActorNr = (int) operationResponse[ParameterCode.ActorNr];
            ChangeLocalID(localActorNr);


            var actorProperties = (Hashtable) operationResponse[ParameterCode.PlayerProperties];
            var gameProperties = (Hashtable) operationResponse[ParameterCode.GameProperties];
            ReadoutProperties(gameProperties, actorProperties, 0);

            //TURNBASED
            var actorsInGame = (int[]) operationResponse[ParameterCode.ActorList];
            if (actorsInGame != null)
            {
                foreach (var userId in actorsInGame)
                {
                    var target = CurrentRoom.GetPlayer(userId);
                    if (target == null)
                    {
                        Debug.WriteLine("Created player that was missing so far (no property set).");
                        //TODO: decide if this could ever happen. it means the user had no props at all.
                        CurrentRoom.StorePlayer(CreatePlayer(string.Empty, userId, false, null));
                    }
                }
            }
            switch (operationResponse.OperationCode)
            {
                case OperationCode.CreateGame:
                    // TODO: add callback "game created"
                    break;
                case OperationCode.JoinGame:
                case OperationCode.JoinRandomGame:
                    // TODO: add callback "game joined"
                    break;
            }

            OnLocalPlayerJoined();
        }

        /// <summary>
        ///     Factory method to create a player instance -
        ///     <see langword="override" /> to get your own player-type with custom
        ///     features.
        /// </summary>
        /// <param name="actorName">
        ///     The name of the player to be created.
        /// </param>
        /// <param name="actorNumber">
        ///     The player ID (a.k.a. actorNumber) of the player to be created.
        /// </param>
        /// <param name="isLocal">
        ///     Sets the distinction if the player to be created is your player or
        ///     if its assigned to someone else.
        /// </param>
        /// <param name="actorProperties">
        ///     The custom properties for this new player
        /// </param>
        /// <returns>
        ///     The newly created player
        /// </returns>
        protected internal virtual Player CreatePlayer(string actorName, int actorNumber, bool isLocal,
            Hashtable actorProperties)
        {
            var newPlayer = new Player(actorName, actorNumber, isLocal, actorProperties);
            return newPlayer;
        }

        /// <summary>
        ///     Internal "factory" method to create a room-instance.
        /// </summary>
        protected internal virtual Room CreateRoom(string roomName, RoomOptions opt)
        {
            if (opt == null)
            {
                opt = new RoomOptions();
            }
            var r = new Room(roomName, opt);
            return r;
        }

        #endregion

        #region IPhotonPeerListener

        /// <summary>
        ///     Debug output of low <paramref name="level" /> api (and this client).
        /// </summary>
        /// <remarks>
        ///     This method is not responsible to keep up the <see cref="_state" /> of
        ///     a LoadBalancingClient. Calling base.DebugReturn on overrides is
        ///     optional.
        /// </remarks>
        public virtual void DebugReturn(DebugLevel level, string message)
        {
            Debug.WriteLine(message);
        }

        /// <summary>
        ///     Uses the OperationResponses provided by the server to advance the
        ///     <see langword="internal" /> <see cref="_state" /> and call ops as
        ///     needed.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         When this method finishes, it will call your
        ///         <see cref="OnOpResponseAction" /> (if any). This way, you can get any
        ///         operation response without overriding this class.
        ///     </para>
        ///     <para>
        ///         To implement a more complex game/app logic, you should implement
        ///         your own class that inherits the LoadBalancingClient. Override this
        ///         method to use your own operation-responses easily.
        ///     </para>
        ///     <para>
        ///         This method is essential to update the <see langword="internal" />
        ///         <see cref="_state" /> of a LoadBalancingClient, so overriding methods
        ///         must call base.OnOperationResponse().
        ///     </para>
        /// </remarks>
        /// <param name="operationResponse">
        ///     Contains the server's response for an operation called by this peer.
        /// </param>
        public virtual void OnOperationResponse(OperationResponse operationResponse)
        {
            // if (operationResponse.ReturnCode != 0) this.DebugReturn(DebugLevel.ERROR, operationResponse.ToStringFull());

            // use the "secret" or "token" whenever we get it. doesn't really matter if it's in AuthResponse.
            if (operationResponse.Parameters.ContainsKey(ParameterCode.Secret))
            {
                if (AuthValues == null)
                {
                    AuthValues = new AuthenticationValues();
                    //this.DebugReturn(DebugLevel.ERROR, "Server returned secret. Created AuthValues.");
                }

                AuthValues.Token = operationResponse[ParameterCode.Secret] as string;
            }

            switch (operationResponse.OperationCode)
            {
                case OperationCode.FindFriends:
                    if (operationResponse.ReturnCode != 0)
                    {
                        DebugReturn(DebugLevel.ERROR, "OpFindFriends failed: " + operationResponse.ToStringFull());
                        _isFetchingFriendList = false;
                        break;
                    }

                    var onlineList = operationResponse[ParameterCode.FindFriendsResponseOnlineList] as bool[];
                    var roomList = operationResponse[ParameterCode.FindFriendsResponseRoomIdList] as string[];

                    FriendList = new List<FriendInfo>(_friendListRequested.Length);
                    for (var index = 0; index < _friendListRequested.Length; index++)
                    {
                        var friend = new FriendInfo();
                        friend.Name = _friendListRequested[index];
                        friend.Room = roomList[index];
                        friend.IsOnline = onlineList[index];
                        FriendList.Insert(index, friend);
                    }

                    _friendListRequested = null;
                    _isFetchingFriendList = false;
                    _friendListTimestamp = Environment.TickCount;
                    if (_friendListTimestamp == 0)
                    {
                        _friendListTimestamp = 1; // makes sure the timestamp is not accidentally 0
                    }
                    break;
                case OperationCode.Authenticate:
                {
                    if (operationResponse.ReturnCode != 0)
                    {
                        DebugReturn(DebugLevel.ERROR,
                            operationResponse.ToStringFull() + " Server: " + Server + " Address: " +
                            peer.ServerAddress);

                        switch (operationResponse.ReturnCode)
                        {
                            case ErrorCode.InvalidAuthentication:
                                DisconnectedCause = DisconnectCause.InvalidAuthentication;
                                break;
                            case ErrorCode.CustomAuthenticationFailed:
                                DisconnectedCause = DisconnectCause.CustomAuthenticationFailed;
                                break;
                            case ErrorCode.InvalidRegion:
                                DisconnectedCause = DisconnectCause.InvalidRegion;
                                break;
                            case ErrorCode.MaxCcuReached:
                                DisconnectedCause = DisconnectCause.MaxCcuReached;
                                break;
                            case ErrorCode.OperationNotAllowedInCurrentState:
                                DisconnectedCause = DisconnectCause.OperationNotAllowedInCurrentState;
                                break;
                        }
                        State = ClientState.Disconnecting;
                        Disconnect();
                        break; // if auth didn't succeed, we disconnect (above) and exit this operation's handling
                    }

                    if (Server == ServerConnection.NameServer || Server == ServerConnection.MasterServer)
                    {
                        if (operationResponse.Parameters.ContainsKey(ParameterCode.UserId))
                        {
                            UserId = (string) operationResponse.Parameters[ParameterCode.UserId];
                            DebugReturn(DebugLevel.INFO,
                                string.Format("Setting UserId sent by Server:{0}", UserId));
                        }
                        if (operationResponse.Parameters.ContainsKey(233))
                        {
                            NickName = (string) operationResponse.Parameters[233];
                            DebugReturn(DebugLevel.INFO,
                                string.Format("Setting Nickname sent by Server:{0}", NickName));
                        }
                    }

                    if (Server == ServerConnection.NameServer)
                    {
                        // on the NameServer, authenticate returns the MasterServer address for a region and we hop off to there
                        MasterServerAddress = operationResponse[ParameterCode.Address] as string;
                        DisconnectToReconnect();
                    }
                    else if (Server == ServerConnection.MasterServer)
                    {
                        State = ClientState.ConnectedToMaster;

                        if (AutoJoinLobby)
                        {
                            peer.OpJoinLobby(CurrentLobby);
                        }
                    }
                    else if (Server == ServerConnection.GameServer)
                    {
                        State = ClientState.ConnectingToGameserver;

                        if (_lastJoinType == JoinType.JoinRoom || _lastJoinType == JoinType.JoinRandomRoom)
                        {
                            // if we just "join" the game, do so. if we "join-or-create", we have to set the createIfNotExists parameter to true
                            State = ClientState.Joining;
                            OpJoinRoom(CurrentRoom.Name, _lastJoinActorNumber);
                        }
                        else if (_lastJoinType == JoinType.JoinOrCreateRoom)
                        {
                            State = ClientState.Joining;
                            OpJoinOrCreateRoom(CurrentRoom.Name, _lastJoinActorNumber,
                                _createRoomOptions, CurrentLobby);
                        }
                        else if (_lastJoinType == JoinType.CreateRoom)
                        {
                            State = ClientState.Joining;
                            // yes, "joining" even though we technically create the game now on the game server
                            OpCreateRoom(CurrentRoom.Name, _createRoomOptions, CurrentLobby);
                        }
                    }
                    break;
                }

                case OperationCode.GetRegions:
                    AvailableRegions = operationResponse[ParameterCode.Region] as string[];
                    AvailableRegionsServers = operationResponse[ParameterCode.Address] as string[];
                    break;

                case OperationCode.Leave:
                    //this.CleanCachedValues(); // this is done in status change on "disconnect"
                    State = ClientState.DisconnectingFromGameserver;
                    peer.Disconnect();
                    break;

                case OperationCode.JoinLobby:
                    State = ClientState.JoinedLobby;
                    break;

                case OperationCode.JoinRandomGame:
                // this happens only on the master server. on gameserver this is a "regular" join
                case OperationCode.CreateGame:
                case OperationCode.JoinGame:
                {
                    if (Server == ServerConnection.GameServer)
                    {
                        GameEnteredOnGameServer(operationResponse);
                    }
                    else
                    {
                        if (operationResponse.ReturnCode == ErrorCode.NoRandomMatchFound)
                        {
                            // this happens only for JoinRandomRoom
                            // TODO: implement callback/reaction when no random game could be found (this is no bug and can simply happen if no games are open)
                            State = ClientState.JoinedLobby;
                            // TODO: maybe we have to return to another state here (if we didn't join a lobby)
                            break;
                        }

                        // TODO: handle more error cases

                        if (operationResponse.ReturnCode != 0)
                        {
                            if (peer.DebugOut >= DebugLevel.ERROR)
                            {
                                DebugReturn(DebugLevel.ERROR,
                                    string.Format("Getting into game failed, client stays on masterserver: {0}.",
                                        operationResponse.ToStringFull()));
                            }

                            State = ClientState.JoinedLobby;
                            // TODO: maybe we have to return to another state here (if we didn't join a lobby)
                            break;
                        }

                        GameServerAddress = (string) operationResponse[ParameterCode.Address];
                        var gameId = operationResponse[ParameterCode.RoomName] as string;
                        if (!string.IsNullOrEmpty(gameId))
                        {
                            // is only sent by the server's response, if it has not been sent with the client's request before!
                            CurrentRoom.Name = gameId;
                        }

                        DisconnectToReconnect();
                    }

                    break;
                }
            }

            if (OnOpResponseAction != null) OnOpResponseAction(operationResponse);
        }

        /// <summary>
        ///     Uses the connection's statusCodes to advance the
        ///     <see langword="internal" /> <see cref="_state" /> and call operations
        ///     as needed.
        /// </summary>
        /// <remarks>
        ///     This method is essential to update the <see langword="internal" />
        ///     <see cref="_state" /> of a LoadBalancingClient. Overriding methods
        ///     must call base.OnStatusChanged.
        /// </remarks>
        public virtual void OnStatusChanged(StatusCode statusCode)
        {
            switch (statusCode)
            {
                case StatusCode.Connect:
                    if (State == ClientState.ConnectingToNameServer)
                    {
                        if (peer.DebugOut >= DebugLevel.ALL)
                        {
                            DebugReturn(DebugLevel.ALL, "Connected to nameserver.");
                        }

                        Server = ServerConnection.NameServer;
                        if (AuthValues != null)
                        {
                            AuthValues.Token = null; // when connecting to NameServer, invalidate the secret (only)
                        }
                    }

                    if (State == ClientState.ConnectingToGameserver)
                    {
                        if (peer.DebugOut >= DebugLevel.ALL)
                        {
                            DebugReturn(DebugLevel.ALL, "Connected to gameserver.");
                        }

                        Server = ServerConnection.GameServer;
                    }

                    if (State == ClientState.ConnectingToMasterserver)
                    {
                        if (peer.DebugOut >= DebugLevel.ALL)
                        {
                            DebugReturn(DebugLevel.ALL, "Connected to masterserver.");
                        }

                        Server = ServerConnection.MasterServer;
                    }

                    peer.EstablishEncryption(); // always enable encryption

                    if (IsAuthorizeSecretAvailable)
                    {
                        // if we have a token we don't have to wait for encryption (it is encrypted anyways, so encryption is just optional later on)
                        _didAuthenticate = peer.OpAuthenticate(AppId, AppVersion,
                            AuthValues, CloudRegion, RequestLobbyStatistics);
                        if (_didAuthenticate)
                        {
                            State = ClientState.Authenticating;
                        }
                        else
                        {
                            DebugReturn(DebugLevel.ERROR,
                                "Error calling OpAuthenticateWithToken! Check log output, AuthValues and if you're connected. State: " +
                                State);
                        }
                    }
                    break;

                case StatusCode.EncryptionEstablished:
                    // on nameserver, the "process" is stopped here, so the developer/game can either get regions or authenticate with a specific region
                    if (Server == ServerConnection.NameServer)
                    {
                        State = ClientState.ConnectedToNameServer;
                    }

                    // on any other server we might now have to authenticate still, so the client can do anything at all
                    if (!_didAuthenticate && (!IsUsingNameServer || CloudRegion != null))
                    {
                        // once encryption is availble, the client should send one (secure) authenticate. it includes the AppId (which identifies your app on the Photon Cloud)
                        _didAuthenticate = peer.OpAuthenticate(AppId, AppVersion,
                            AuthValues, CloudRegion, RequestLobbyStatistics);
                        if (_didAuthenticate)
                        {
                            State = ClientState.Authenticating;
                        }
                        else
                        {
                            DebugReturn(DebugLevel.ERROR,
                                "Error calling OpAuthenticate! Did not work. Check log output, AuthValues and if you're connected. State: " +
                                State);
                        }
                    }

                    break;

                case StatusCode.Disconnect:
                    // disconnect due to connection exception is handled below (don't connect to GS or master in that case)

                    CleanCachedValues();
                    _didAuthenticate = false; // on connect, we know that we didn't

                    if (State == ClientState.Disconnecting)
                    {
                        if (AuthValues != null)
                        {
                            AuthValues.Token = null;
                            // when leaving the server, invalidate the secret (but not the auth values)
                        }
                        State = ClientState.Disconnected;
                    }
                    else if (State == ClientState.Uninitialized)
                    {
                        if (AuthValues != null)
                        {
                            AuthValues.Token = null;
                            // when leaving the server, invalidate the secret (but not the auth values)
                        }
                        State = ClientState.Disconnected;
                    }
                    else if (State != ClientState.Disconnected)
                    {
                        if (Server == ServerConnection.GameServer || Server == ServerConnection.NameServer)
                        {
                            Connect();
                        }
                        else if (Server == ServerConnection.MasterServer)
                        {
                            ConnectToGameServer();
                        }
                    }
                    break;

                case StatusCode.DisconnectByServerUserLimit:
                    DebugReturn(DebugLevel.ERROR,
                        "The Photon license's CCU Limit was reached. Server rejected this connection. Wait and re-try.");
                    if (AuthValues != null)
                    {
                        AuthValues.Token = null;
                        // when leaving the server, invalidate the secret (but not the auth values)
                    }
                    DisconnectedCause = DisconnectCause.DisconnectByServerUserLimit;
                    State = ClientState.Disconnected;
                    break;
                case StatusCode.ExceptionOnConnect:
                case StatusCode.SecurityExceptionOnConnect:
                    if (AuthValues != null)
                    {
                        AuthValues.Token = null;
                        // when leaving the server, invalidate the secret (but not the auth values)
                    }
                    DisconnectedCause = DisconnectCause.ExceptionOnConnect;
                    State = ClientState.Disconnected;
                    break;
                case StatusCode.DisconnectByServer:
                    if (AuthValues != null)
                    {
                        AuthValues.Token = null;
                        // when leaving the server, invalidate the secret (but not the auth values)
                    }
                    DisconnectedCause = DisconnectCause.DisconnectByServer;
                    State = ClientState.Disconnected;
                    break;
                case StatusCode.TimeoutDisconnect:
                    if (AuthValues != null)
                    {
                        AuthValues.Token = null;
                        // when leaving the server, invalidate the secret (but not the auth values)
                    }
                    DisconnectedCause = DisconnectCause.TimeoutDisconnect;
                    State = ClientState.Disconnected;
                    break;
                case StatusCode.Exception:
                case StatusCode.ExceptionOnReceive:
                    if (AuthValues != null)
                    {
                        AuthValues.Token = null;
                        // when leaving the server, invalidate the secret (but not the auth values)
                    }
                    DisconnectedCause = DisconnectCause.Exception;
                    State = ClientState.Disconnected;
                    break;
            }
        }

        /// <summary>
        ///     Uses the photonEvent's provided by the server to advance the
        ///     <see langword="internal" /> <see cref="_state" /> and call ops as
        ///     needed.
        /// </summary>
        /// <remarks>
        ///     This method is essential to update the <see langword="internal" />
        ///     <see cref="_state" /> of a LoadBalancingClient. Overriding methods
        ///     must call base.OnEvent.
        /// </remarks>
        public virtual void OnEvent(EventData photonEvent)
        {
            switch (photonEvent.Code)
            {
                case EventCode.GameList:
                case EventCode.GameListUpdate:
                    if (photonEvent.Code == EventCode.GameList)
                    {
                        RoomInfoList = new Dictionary<string, RoomInfo>();
                    }

                    var games = (Hashtable) photonEvent[ParameterCode.GameList];
                    foreach (string gameName in games.Keys)
                    {
                        var game = new RoomInfo(gameName, (Hashtable) games[gameName]);
                        if (game.removedFromList)
                        {
                            RoomInfoList.Remove(gameName);
                        }
                        else
                        {
                            RoomInfoList[gameName] = game;
                        }
                    }
                    break;

                case EventCode.Join:
                    var actorNr = (int) photonEvent[ParameterCode.ActorNr];
                    // actorNr (a.k.a. playerNumber / ID) of sending player
                    var isLocal = LocalPlayer.ID == actorNr;

                    var actorProperties = (Hashtable) photonEvent[ParameterCode.PlayerProperties];

                    if (!isLocal)
                    {
                        var newPlayer = CreatePlayer(string.Empty, actorNr, false, actorProperties);
                        CurrentRoom.StorePlayer(newPlayer);
                    }
                    else
                    {
                        // in this player's own join event, we get a complete list of players in the room, so check if we know each of the
                        var actorsInRoom = (int[]) photonEvent[ParameterCode.ActorList];
                        foreach (var actorNrToCheck in actorsInRoom)
                        {
                            if (LocalPlayer.ID != actorNrToCheck &&
                                !CurrentRoom.Players.ContainsKey(actorNrToCheck))
                            {
                                CurrentRoom.StorePlayer(CreatePlayer(string.Empty, actorNrToCheck, false, null));
                            }
                            else if (CurrentRoom.Players.ContainsKey(actorNrToCheck))
                            {
                                Player p = null;
                                if (CurrentRoom.Players.TryGetValue(actorNrToCheck, out p))
                                {
                                    p.IsInactive = false;
                                }
                            }
                        }
                    }

                    break;

                case EventCode.Leave:
                {
                    var actorID = (int) photonEvent[ParameterCode.ActorNr];

                    //TURNBASED
                    var isInactive = false;
                    if (photonEvent.Parameters.ContainsKey(ParameterCode.IsInactive))
                    {
                        isInactive = (bool) photonEvent.Parameters[ParameterCode.IsInactive];
                    }

                    if (isInactive)
                    {
                        CurrentRoom.MarkAsInactive(actorID);
                        //UnityEngine.Debug.Log("leave marked player as inactive "+ actorID);
                    }
                    else
                    {
                        CurrentRoom.RemovePlayer(actorID);
                        //UnityEngine.Debug.Log("leave removed player " + actorID);
                    }
                }
                    break;

                // EventCode.Disconnect was "replaced" by Leave which now has a "inactive" flag.
                //case EventCode.Disconnect:  //TURNBASED
                //    {
                //        int actorID = (int) photonEvent[ParameterCode.ActorNr];
                //        this.CurrentRoom.MarkAsInactive(actorID);
                //    }
                //    break;

                case EventCode.PropertiesChanged:
                    // whenever properties are sent in-room, they can be broadcasted as event (which we handle here)
                    // we get PLAYERproperties if actorNr > 0 or ROOMproperties if actorNumber is not set or 0
                    var targetActorNr = 0;
                    if (photonEvent.Parameters.ContainsKey(ParameterCode.TargetActorNr))
                    {
                        targetActorNr = (int) photonEvent[ParameterCode.TargetActorNr];
                    }
                    var props = (Hashtable) photonEvent[ParameterCode.Properties];

                    if (targetActorNr > 0)
                    {
                        ReadoutProperties(null, props, targetActorNr);

                        OnPlayerChangeProperties(targetActorNr, props);
                    }
                    else
                    {
                        ReadoutProperties(props, null, 0);
                    }

                    break;

                case EventCode.AppStats:
                    // only the master server sends these in (1 minute) intervals
                    PlayersInRoomsCount = (int) photonEvent[ParameterCode.PeerCount];
                    RoomsCount = (int) photonEvent[ParameterCode.GameCount];
                    PlayersOnMasterCount = (int) photonEvent[ParameterCode.MasterPeerCount];
                    break;

                case EventCode.LobbyStats:
                    var names = photonEvent[ParameterCode.LobbyName] as string[];
                    var types = photonEvent[ParameterCode.LobbyType] as byte[];
                    var peers = photonEvent[ParameterCode.PeerCount] as int[];
                    var rooms = photonEvent[ParameterCode.GameCount] as int[];

                    LobbyStatistics.Clear();
                    for (var i = 0; i < names.Length; i++)
                    {
                        var info = new TypedLobbyInfo
                        {
                            Name = names[i],
                            Type = (LobbyType) types[i],
                            PlayerCount = peers[i],
                            RoomCount = rooms[i]
                        };

                        LobbyStatistics.Add(info);
                    }
                    break;
            }

            OnEventAction(photonEvent);
        }

        /// <summary>
        ///     In Photon 4, "raw messages" will get their own callback method in
        ///     the interface. Not used yet.
        /// </summary>
        public virtual void OnMessage(object message)
        {
            DebugReturn(DebugLevel.ALL, string.Format("got OnMessage {0}", message));
        }

        #endregion
    }
}