using System;
using System.Collections.Generic;
using ExitGames.Logging;

namespace Warsmiths.Server.Framework.Services
{
    public class ServiceManager
    {
        /// <summary>
        /// </summary>
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// </summary>
        private static readonly Dictionary<Type, IRuntimeService> Services = new Dictionary<Type, IRuntimeService>();

        public static void InstallService(IRuntimeService service)
        {
            var t = service.GetType();

            if (Services.ContainsKey(t) == false)
            {
                Services.Add(t, service);
            }
            else
            {
                throw new Exception("Service already installed");
            }
        }

        public static T Get<T>()
        {
            var pickedType = typeof (T);
            IRuntimeService service;
            if (Services.TryGetValue(pickedType, out service))
            {
                return (T)service;
            }

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat($"Cannot get Runtime service '{pickedType}'!");
            }

            throw new ArgumentException("cannot find service");
        }

        public static Dictionary<Type, IRuntimeService> GetAllServices()
        {
            return Services;
        }
    }
}