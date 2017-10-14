namespace YourGame.Client.Loadbalancing
{
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_WII || UNITY_IPHONE || UNITY_ANDROID || UNITY_PS3 || UNITY_XBOX360 || UNITY_NACL  || UNITY_FLASH  || UNITY_BLACKBERRY || UNITY_PSP2 || UNITY_WEBGL
    using Hashtable = ExitGames.Client.Photon.Hashtable;
#endif

    public class FriendInfo
    {
        public string Name { get; internal protected set; }
        public bool IsOnline { get; internal protected set; }
        public string Room { get; internal protected set; }

        public bool IsInRoom
        {
            get { return string.IsNullOrEmpty(Room); }
        }

        public override string ToString()
        {
            return string.Format("{0}\t({1})\tin room: '{2}'", Name, (this.IsOnline) ? "on" : "off", Room);
        }
    }
}