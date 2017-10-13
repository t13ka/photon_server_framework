using Warsmiths.Client.Loadbalancing.Enums;

namespace Warsmiths.Client.Loadbalancing
{
    /// <summary>
    /// Container for user authentication in Photon. Set AuthValues before you connect - all else is handled.
    /// </summary>
    /// <remarks>
    /// On Photon, user authentication is optional but can be useful in many cases.
    /// If you want to FindFriends, a unique ID per user is very practical.
    ///
    /// There are basically three options for user authentification: None at all, the client sets some UserId
    /// or you can use some account web-service to authenticate a user (and set the UserId server-side).
    ///
    /// Custom Authentication lets you verify end-users by some kind of login or token. It sends those
    /// values to Photon which will verify them before granting access or disconnecting the client.
    ///
    /// The Photon Cloud Dashboard will let you enable this feature and set important server values for it.
    /// https://www.photonengine.com/dashboard
    /// </remarks>
    public class AuthenticationValues
    {
        /// <summary>
        /// The type of custom authentication provider that should be used. 
        /// Currently only "Custom" or "None" (turns this off).
        /// </summary>
        public CustomAuthenticationType AuthType = CustomAuthenticationType.None;

        /// <summary>
        /// This string must contain any (http get) parameters expected by the used authentication service. 
        /// By default, username and token.
        /// </summary>
        /// <remarks>
        /// Standard http get parameters are used here and passed on to the service that's defined in the server (Photon Cloud Dashboard).
        /// </remarks>
        public string AuthGetParameters;

        /// <summary>
        /// Data to be passed-on to the auth service via POST. Default: null (not sent). 
        /// Either string or byte[] (see setters).
        /// </summary>
        public object AuthPostData { get; private set; }

        /// <summary>
        /// After initial authentication, Photon provides a token for this client / user, 
        /// which is subsequently used as (cached) validation.
        /// </summary>
        public string Token;

        /// <summary>
        /// The UserId should be a unique identifier per user. 
        /// This is for finding friends, etc..
        /// </summary>
        public string UserId ;

        /// <summary>Creates empty auth values without any info.</summary>
        public AuthenticationValues()
        {
        }

        /// <summary>
        /// Creates minimal info about the user. 
        /// If this is authenticated or not, depends on the set AuthType.
        /// </summary>
        /// <param name="userId">
        /// Some UserId to set in Photon.
        /// </param>
        public AuthenticationValues(string userId)
        {
            UserId = userId;
        }

        /// <summary>
        /// Sets the data to be passed-on to the auth service via POST.
        /// </summary>
        /// <param name="stringData"></param>
        public virtual void SetAuthPostData(string stringData)
        {
            AuthPostData = (string.IsNullOrEmpty(stringData)) ? null : stringData;
        }

        /// <summary>
        /// Sets the data to be passed-on to the auth service via POST.
        /// </summary>
        /// <param name="byteData">
        /// Binary token / auth-data to pass on.
        /// </param>
        public virtual void SetAuthPostData(byte[] byteData)
        {
            AuthPostData = byteData;
        }

        /// <summary>
        /// Adds a key-value pair to the get-parameters used for Custom Auth.
        /// </summary>
        /// <remarks>
        /// This method does uri-encoding for you.
        /// </remarks>
        /// <param name="key">Key for the value to set.</param>
        /// <param name="value">Some value relevant for Custom Authentication.</param>
        public virtual void AddAuthParameter(string key, string value)
        {
            var ampersand = string.IsNullOrEmpty(this.AuthGetParameters) ? "" : "&";
            AuthGetParameters = string.Format("{0}{1}{2}={3}", AuthGetParameters, ampersand,
                System.Uri.EscapeDataString(key), System.Uri.EscapeDataString(value));
        }

        public override string ToString()
        {
            return string.Format("AuthenticationValues UserId: {0}, GetParameters: {1} Token available: {2}", UserId,
                AuthGetParameters, Token != null);
        }
    }
}