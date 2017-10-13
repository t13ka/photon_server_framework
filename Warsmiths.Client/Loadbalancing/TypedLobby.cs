using Warsmiths.Client.Loadbalancing.Enums;

namespace Warsmiths.Client.Loadbalancing
{
    /// <summary>Refers to a specific lobby (and type) on the server.</summary>
    /// <remarks>
    /// The name and type are the unique identifier for a lobby.<br/>
    /// Join a lobby via PhotonNetwork.JoinLobby(TypedLobby lobby).<br/>
    /// The current lobby is stored in PhotonNetwork.lobby.
    /// </remarks>
    public class TypedLobby
    {
        /// <summary>
        /// Name of the lobby this game gets added to. Default: null, attached to default lobby. 
        /// Lobbies are unique per lobbyName plus lobbyType, so the same name can be used when several types are existing.
        /// </summary>
        public string Name;

        /// <summary>
        /// Type of the (named)lobby this game gets added to
        /// </summary>
        public LobbyType Type;

        public static readonly TypedLobby Default = new TypedLobby();

        public bool IsDefault
        {
            get { return Type == LobbyType.Default && string.IsNullOrEmpty(Name); }
        }

        public TypedLobby()
        {
            Name = string.Empty;
            Type = LobbyType.Default;
        }

        public TypedLobby(string name, LobbyType type)
        {
            Name = name;
            Type = type;
        }

        public override string ToString()
        {
            return string.Format("lobby '{0}'[{1}]", Name, Type);
        }
    }
}