namespace YourGame.Server.Framework.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using ExitGames.Logging;

    using Photon.SocketServer;

    using YourGame.Common;

    public class HandlerPicker
    {
        /// <summary>
        /// </summary>
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// </summary>
        private static readonly Dictionary<OperationCode, BaseHandler> Handlers =
            new Dictionary<OperationCode, BaseHandler>();

        private static readonly Dictionary<OperationCode, Type> Operations = new Dictionary<OperationCode, Type>();

        /// <summary>
        /// </summary>
        public HandlerPicker()
        {
            var types = Assembly.GetCallingAssembly().GetTypes();

            foreach (var t in types.Where(t => t.BaseType != null && t.BaseType.Name == "BaseHandler"))
            {
                var type = t;

                if (type.IsAbstract) continue;

                var instance = Activator.CreateInstance(type);

                if (instance != null)
                {
                    var handlerInstance = (BaseHandler)instance;

                    Handlers.Add(handlerInstance.ControlCode, handlerInstance);
                }
                else
                {
                    Log.Warn($"Cannot init Handler '{type.Name}'!");
                }
            }

            var onlyOperationRequests = types.Where(t => t.Name.EndsWith("Request")).ToList();
            foreach (var handler in Handlers)
            {
                var hname = handler.Value.GetType().Name;
                var cleanName = hname.Replace("Handler", string.Empty).Replace("handler", string.Empty);
                var operationType =
                    onlyOperationRequests.FirstOrDefault(t => t.BaseType != null && t.Name.Contains(cleanName));
                if (operationType != null)
                {
                    Operations.Add(handler.Value.ControlCode, operationType);
                }
            }
        }

        public BaseHandler GetHandler(OperationRequest request)
        {
            BaseHandler h;
            object obj;
            if (!request.Parameters.TryGetValue((byte)ParameterCode.ControlCode, out obj))
            {
                var control = (OperationCode)request.OperationCode;
                if (Handlers.TryGetValue(control, out h))
                {
                }
            }
            else
            {
                var control = (OperationCode)obj;
                if (Handlers.TryGetValue(control, out h))
                {
                    request.Parameters.Remove((byte)ParameterCode.ControlCode);
                }
            }

            return h;
        }
    }
}