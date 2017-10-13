﻿using System.Collections.Generic;
using System.Linq;
using ExitGames.Logging;
using Warsmiths.Common.Domain;
using Warsmiths.Server.LoadBalancer;
using Warsmiths.Server.MasterServer.GameServer;

namespace Warsmiths.Server.MasterServer
{
    public class CraftApplication
    {
        public static CraftApplication I;
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
        public static Dictionary<string, BaseElement> PrimalElements = new Dictionary<string, BaseElement>();


        public CraftApplication(string applicationId, LoadBalancer<IncomingGameServerPeer> loadBalancer)
        {
            I = this;
            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("Master Craft Started", applicationId);
            }
            foreach (var a in MasterApplication.DomainConfiguration.Elements)
            { PrimalElements.Add(a.Type.ToString() + a.ColorType, a); }
        }

        public static BaseElement GetPrimalElement(string id)
        {
            lock (PrimalElements)
            {
                if (PrimalElements.ContainsKey(id))
                {
                    return PrimalElements[id];
                }
                return PrimalElements.First().Value;
            }
        }

    }
}