namespace YourGame.Client.Loadbalancing.Codes
{
    using System;

    /// <summary>
    /// ErrorCode defines the default codes associated with Photon client/server communication.
    /// </summary>
    public class ErrorCode
    {
        /// <summary>
        /// (0) is always "OK", anything else an error or specific situation.
        /// </summary>
        public const int Ok = 0;

        // server - Photon low(er) level: <= 0

        /// <summary>
        /// (-3) Operation can't be executed yet (e.g. OpJoin can't be called before being authenticated, RaiseEvent cant be used before getting into a room).
        /// </summary>
        /// <remarks>
        /// Before you call any operations on the Cloud servers, the automated client workflow must complete its authorization.
        /// In PUN, wait until State is: JoinedLobby (with AutoJoinLobby = true) or ConnectedToMaster (AutoJoinLobby = false)
        /// </remarks>
        public const int OperationNotAllowedInCurrentState = -3;

        /// <summary>(-2) The operation you called is not implemented on the server (application) you connect to. Make sure you run the fitting applications.</summary>
        [Obsolete("Use InvalidOperation.")]
        public const int InvalidOperationCode = -2;

        /// <summary>(-2) The operation you called could not be executed on the server.</summary>
        /// <remarks>
        /// Make sure you are connected to the server you expect.
        ///
        /// This code is used in several cases:
        /// The arguments/parameters of the operation might be out of range, missing entirely or conflicting.
        /// The operation you called is not implemented on the server (application). Server-side plugins affect the available operations.
        /// </remarks>
        public const int InvalidOperation = -2;

        /// <summary>
        /// (-1) Something went wrong in the server. Try to reproduce and contact Exit Games.
        /// </summary>
        public const int InternalServerError = -1;

        // server - PhotonNetwork: 0x7FFF and down
        // logic-level error codes start with short.max

        /// <summary>
        /// (32767) Authentication failed. Possible cause: AppId is unknown to Photon (in cloud service).
        /// </summary>
        public const int InvalidAuthentication = 0x7FFF;

        /// <summary>
        /// (32766) GameId (name) already in use (can't create another). Change name.
        /// </summary>
        public const int GameIdAlreadyExists = 0x7FFF - 1;

        /// <summary>
        /// (32765) Game is full. This rarely happens when some player joined the room before your join completed.
        /// </summary>
        public const int GameFull = 0x7FFF - 2;

        /// <summary>
        /// (32764) Game is closed and can't be joined. Join another game.
        /// </summary>
        public const int GameClosed = 0x7FFF - 3;

        [Obsolete("No longer used, cause random matchmaking is no longer a process.")]
        public const int AlreadyMatched = 0x7FFF - 4;

        /// <summary>
        /// (32762) Not in use currently.
        /// </summary>
        public const int ServerFull = 0x7FFF - 5;

        /// <summary>
        /// (32761) Not in use currently.
        /// </summary>
        public const int UserBlocked = 0x7FFF - 6;

        /// <summary>
        /// (32760) Random matchmaking only succeeds if a room exists thats neither closed nor full. Repeat in a few seconds or create a new room.
        /// </summary>
        public const int NoRandomMatchFound = 0x7FFF - 7;

        /// <summary>
        /// (32758) Join can fail if the room (name) is not existing (anymore). This can happen when players leave while you join.
        /// </summary>
        public const int GameDoesNotExist = 0x7FFF - 9;

        /// <summary>(32757) Authorization on the Photon Cloud failed becaus the concurrent users (CCU) limit of the app's subscription is reached.</summary>
        /// <remarks>
        /// Unless you have a plan with "CCU Burst", clients might fail the authentication step during connect.
        /// Affected client are unable to call operations. Please note that players who end a game and return
        /// to the master server will disconnect and re-connect, which means that they just played and are rejected
        /// in the next minute / re-connect.
        /// This is a temporary measure. Once the CCU is below the limit, players will be able to connect an play again.
        ///
        /// OpAuthorize is part of connection workflow but only on the Photon Cloud, this error can happen.
        /// Self-hosted Photon servers with a CCU limited license won't let a client connect at all.
        /// </remarks>
        public const int MaxCcuReached = 0x7FFF - 10;

        /// <summary>(32756) Authorization on the Photon Cloud failed because the app's subscription does not allow to use a particular region's server.</summary>
        /// <remarks>
        /// Some subscription plans for the Photon Cloud are region-bound. Servers of other regions can't be used then.
        /// Check your master server address and compare it with your Photon Cloud Dashboard's info.
        /// https://cloud.photonengine.com/dashboard
        ///
        /// OpAuthorize is part of connection workflow but only on the Photon Cloud, this error can happen.
        /// Self-hosted Photon servers with a CCU limited license won't let a client connect at all.
        /// </remarks>
        public const int InvalidRegion = 0x7FFF - 11;

        /// <summary>
        /// (32755) Custom Authentication of the user failed due to setup reasons (see Cloud Dashboard) or the provided user data (like username or token). Check error message for details.
        /// </summary>
        public const int CustomAuthenticationFailed = 0x7FFF - 12;

        /// <summary>
        /// (32752) A server-side plugin failed to execute and reported an error. Check the OperationResponse.DebugMessage.
        /// </summary>
        public const int PluginReportedError = 0x7FFF - 15;

        /// <summary>
        /// (32751) CreateGame/JoinGame/Join operation fails if expected plugin does not correspond to loaded one.
        /// </summary>
        public const int PluginMismatch = 0x7FFF - 16;
    }
}