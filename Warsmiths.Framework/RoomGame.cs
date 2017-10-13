using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Photon.SocketServer;

using Warsmiths.Server.Framework.Caching;
using Warsmiths.Server.Framework.Diagnostics.OperationLogging;
using Warsmiths.Server.Framework.Events;
using Warsmiths.Server.Framework.Messages;
using Warsmiths.Server.Framework.Operations;

namespace Warsmiths.Server.Framework
{
    public class RoomGame : Room
    {
        #region Ctor

        public RoomGame(string gameName, RoomCacheBase roomCache, int emptyRoomLiveTime = 0)
            : base(gameName, roomCache, emptyRoomLiveTime)
        {
            LogQueue = new LogQueue("Game " + gameName, LogQueue.DefaultCapacity);
        }

        #endregion

        protected bool DeleteCacheOnLeave;

        protected bool SuppressRoomEvents;

        #region Constants and Fields

        protected readonly LogQueue LogQueue;

        protected readonly EventCacheDictionary ActorEventCache = new EventCacheDictionary();

        protected readonly RoomEventCache EventCache = new RoomEventCache();

        protected readonly Dictionary<byte, ActorGroup> ActorGroups = new Dictionary<byte, ActorGroup>();

        private int _actorNumberCounter;

        #endregion

        #region Methods

        public override string ToString()
        {
            var sb = new StringBuilder(base.ToString());

            sb.AppendFormat("Events cached: {0}", EventCache.Count());
            sb.AppendLine();
            return sb.ToString();
        }

        protected override void ExecuteOperation(
            PlayerPeer peer,
            OperationRequest operationRequest,
            SendParameters sendParameters)
        {
            try
            {
                base.ExecuteOperation(peer, operationRequest, sendParameters);

                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("Executing operation {0}", (OperationCode)operationRequest.OperationCode);
                }

                switch ((OperationCode)operationRequest.OperationCode)
                {
                    case OperationCode.Join:
                        {
                            var joinRequest = new JoinRequest(peer.Protocol, operationRequest);
                            if (peer.ValidateOperation(joinRequest, sendParameters) == false)
                            {
                                return;
                            }

                            if (LogQueue.Log.IsDebugEnabled)
                            {
                                LogQueue.Add(
                                    new LogEntry(
                                        "ExecuteOperation: " + (OperationCode)operationRequest.OperationCode,
                                        "Peer=" + peer.ConnectionId));
                            }

                            joinRequest.OnStart();
                            HandleJoinOperation(peer, joinRequest, sendParameters);
                            joinRequest.OnComplete();
                            break;
                        }

                    case OperationCode.Leave:
                        {
                            var leaveOperation = new LeaveRequest(peer.Protocol, operationRequest);
                            if (peer.ValidateOperation(leaveOperation, sendParameters) == false)
                            {
                                return;
                            }

                            if (LogQueue.Log.IsDebugEnabled)
                            {
                                LogQueue.Add(
                                    new LogEntry(
                                        "ExecuteOperation: " + (OperationCode)operationRequest.OperationCode,
                                        "Peer=" + peer.ConnectionId));
                            }

                            leaveOperation.OnStart();
                            HandleLeaveOperation(peer, leaveOperation, sendParameters);
                            leaveOperation.OnComplete();
                            break;
                        }

                    case OperationCode.CustomOp:
                        // todo: here implemented custom logic
                        // get handler for opp
                        break;

                    case OperationCode.RaiseEvent:
                        {
                            var raiseEventOperation = new RaiseEventRequest(peer.Protocol, operationRequest);
                            if (peer.ValidateOperation(raiseEventOperation, sendParameters) == false) return;

                            raiseEventOperation.OnStart();
                            HandleRaiseEventOperation(peer, raiseEventOperation, sendParameters);
                            raiseEventOperation.OnComplete();
                            break;
                        }

                    case OperationCode.GetProperties:
                        {
                            var getPropertiesOperation = new GetPropertiesRequest(peer.Protocol, operationRequest);
                            if (peer.ValidateOperation(getPropertiesOperation, sendParameters) == false) return;

                            getPropertiesOperation.OnStart();
                            HandleGetPropertiesOperation(peer, getPropertiesOperation, sendParameters);
                            getPropertiesOperation.OnComplete();
                            break;
                        }

                    case OperationCode.SetProperties:
                        {
                            var setPropertiesOperation = new SetPropertiesRequest(peer.Protocol, operationRequest);
                            if (peer.ValidateOperation(setPropertiesOperation, sendParameters) == false) return;

                            setPropertiesOperation.OnStart();
                            HandleSetPropertiesOperation(peer, setPropertiesOperation, sendParameters);
                            setPropertiesOperation.OnComplete();
                            break;
                        }

                    case OperationCode.Ping:
                        {
                            peer.SendOperationResponse(
                                new OperationResponse { OperationCode = operationRequest.OperationCode },
                                sendParameters);
                            break;
                        }

                    case OperationCode.ChangeGroups:
                        {
                            var changeGroupsOperation = new ChangeGroups(peer.Protocol, operationRequest);
                            if (peer.ValidateOperation(changeGroupsOperation, sendParameters) == false) return;

                            changeGroupsOperation.OnStart();
                            HandleChangeGroupsOperation(peer, changeGroupsOperation, sendParameters);
                            changeGroupsOperation.OnComplete();
                            break;
                        }

                    default:
                        {
                            var message = $"Unknown operation code {(OperationCode)operationRequest.OperationCode}";
                            peer.SendOperationResponse(
                                new OperationResponse
                                    {
                                        OperationCode = operationRequest.OperationCode,
                                        ReturnCode = -1,
                                        DebugMessage = message
                                    },
                                sendParameters);

                            if (Log.IsWarnEnabled)
                            {
                                Log.Warn(message);
                            }
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        protected Actor GetActorByPeer(PlayerPeer peer)
        {
            var actor = Actors.GetActorByPeer(peer);
            if (actor == null)
            {
                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("Actor not found for peer: {0}", peer.ConnectionId);
                }
            }

            return actor;
        }

        protected override void ProcessMessage(IMessage message)
        {
            try
            {
                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("ProcessMessage {0}", message.Action);
                }

                switch ((GameMessageCodes)message.Action)
                {
                    case GameMessageCodes.RemovePeerFromGame:
                        var peer = (PlayerPeer)message.Message;
                        RemovePeerFromGame(peer, null);
                        if (LogQueue.Log.IsDebugEnabled)
                        {
                            LogQueue.Add(
                                new LogEntry(
                                    "ProcessMessage: " + (GameMessageCodes)message.Action,
                                    "Peer=" + peer.ConnectionId));
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        protected void PublishEventCache(PlayerPeer playerPeer)
        {
            var @event = new CustomEvent(0, 0, null);
            foreach (var entry in ActorEventCache)
            {
                var actor = entry.Key;
                var cache = entry.Value;
                @event.ActorNr = actor;
                foreach (var eventEntry in cache)
                {
                    @event.Code = @eventEntry.Key;
                    @event.Data = @eventEntry.Value;

                    var eventData = new EventData(@event.Code, @event);
                    playerPeer.SendEvent(eventData, new SendParameters());
                }
            }

            foreach (var customEvent in EventCache)
            {
                var eventData = new EventData(customEvent.Code, customEvent);
                playerPeer.SendEvent(eventData, new SendParameters());
            }
        }

        protected virtual void PublishJoinEvent(PlayerPeer peer, JoinRequest joinRequest)
        {
            if (SuppressRoomEvents)
            {
                return;
            }

            var actor = GetActorByPeer(peer);
            if (actor == null)
            {
                return;
            }

            // generate a join event and publish to all actors in the room
            var joinEvent = new JoinEvent(actor.ActorNr, Actors.GetActorNumbers().ToArray());

            if (joinRequest.BroadcastActorProperties)
            {
                joinEvent.ActorProperties = joinRequest.ActorProperties;
            }

            PublishEvent(joinEvent, Actors, new SendParameters());
        }

        protected virtual void PublishLeaveEvent(Actor actor, LeaveRequest leaveRequest)
        {
            if (SuppressRoomEvents)
            {
                return;
            }

            if (Actors.Count > 0 && actor != null)
            {
                var actorNumbers = Actors.GetActorNumbers();
                var leaveEvent = new LeaveEvent(actor.ActorNr, actorNumbers.ToArray());
                PublishEvent(leaveEvent, Actors, new SendParameters());
            }
        }

        protected virtual int RemovePeerFromGame(PlayerPeer peer, LeaveRequest leaveRequest)
        {
            var actor = Actors.RemoveActorByPeer(peer);
            if (actor == null)
            {
                if (Log.IsWarnEnabled)
                {
                    Log.WarnFormat("RemovePeerFromGame - Actor to remove not found for peer: {0}", peer.ConnectionId);
                }

                return -1;
            }

            ActorEventCache.RemoveEventCache(actor.ActorNr);

            if (DeleteCacheOnLeave)
            {
                EventCache.RemoveEventsByActor(actor.ActorNr);
            }

            // raise leave event
            PublishLeaveEvent(actor, leaveRequest);

            return actor.ActorNr;
        }

        protected virtual bool TryAddPeerToGame(PlayerPeer peer, int actorNr, out Actor actor)
        {
            // check if the peer already exists in this game
            actor = Actors.GetActorByPeer(peer);
            if (actor != null) return false;

            if (actorNr != 0)
            {
                actor = Actors.GetActorByNumber(actorNr);
                if (actor != null) return false;
            }

            // create new actor instance 
            actor = new Actor(peer);
            if (actorNr != 0)
            {
                actor.ActorNr = actorNr;
            }
            else
            {
                _actorNumberCounter++;
                actor.ActorNr = _actorNumberCounter;
            }

            Actors.Add(actor);

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("Actor added: {0} to game: {1}", actor.ActorNr, Name);
            }

            return true;
        }

        protected bool UpdateEventCache(Actor actor, RaiseEventRequest raiseEventRequest, out string msg)
        {
            msg = null;
            CustomEvent customEvent;

            switch (raiseEventRequest.Cache)
            {
                case (byte)CacheOperation.DoNotCache: return true;

                case (byte)CacheOperation.AddToRoomCache:
                    customEvent = new CustomEvent(actor.ActorNr, raiseEventRequest.EvCode, raiseEventRequest.Data);
                    EventCache.AddEvent(customEvent);
                    return true;

                case (byte)CacheOperation.AddToRoomCacheGlobal:
                    customEvent = new CustomEvent(0, raiseEventRequest.EvCode, raiseEventRequest.Data);
                    EventCache.AddEvent(customEvent);
                    return true;
            }

            // cache operations for the actor event cache currently only working with hashtable data
            Hashtable eventData;
            if (raiseEventRequest.Data == null || raiseEventRequest.Data is Hashtable)
            {
                eventData = (Hashtable)raiseEventRequest.Data;
            }
            else
            {
                msg = string.Format(
                    "Cache operation '{0}' requires a Hashtable as event data.",
                    raiseEventRequest.Cache);
                return false;
            }

            switch (raiseEventRequest.Cache)
            {
                case (byte)CacheOperation.MergeCache:
                    ActorEventCache.MergeEvent(actor.ActorNr, raiseEventRequest.EvCode, eventData);
                    return true;

                case (byte)CacheOperation.RemoveCache:
                    ActorEventCache.RemoveEvent(actor.ActorNr, raiseEventRequest.EvCode);
                    return true;

                case (byte)CacheOperation.ReplaceCache:
                    ActorEventCache.ReplaceEvent(actor.ActorNr, raiseEventRequest.EvCode, eventData);
                    return true;

                default:
                    msg = string.Format("Unknown cache operation '{0}'.", raiseEventRequest.Cache);
                    return false;
            }
        }

        #endregion

        #region Handlers

        protected virtual void HandleGetPropertiesOperation(
            PlayerPeer peer,
            GetPropertiesRequest getPropertiesRequest,
            SendParameters sendParameters)
        {
            var response = new GetPropertiesResponse();

            // check if game properties should be returned
            if ((getPropertiesRequest.PropertyType & (byte)PropertyType.Game) == (byte)PropertyType.Game)
            {
                response.GameProperties = Properties.GetProperties(getPropertiesRequest.GamePropertyKeys);
            }

            // check if actor properties should be returned
            if ((getPropertiesRequest.PropertyType & (byte)PropertyType.Actor) == (byte)PropertyType.Actor)
            {
                response.ActorProperties = new Hashtable();

                if (getPropertiesRequest.ActorNumbers == null)
                {
                    foreach (var actor in Actors)
                    {
                        var actorProperties = actor.Properties.GetProperties(getPropertiesRequest.ActorPropertyKeys);
                        response.ActorProperties.Add(actor.ActorNr, actorProperties);
                    }
                }
                else
                {
                    foreach (var actorNumber in getPropertiesRequest.ActorNumbers)
                    {
                        var actor = Actors.GetActorByNumber(actorNumber);
                        if (actor != null)
                        {
                            var actorProperties =
                                actor.Properties.GetProperties(getPropertiesRequest.ActorPropertyKeys);
                            response.ActorProperties.Add(actorNumber, actorProperties);
                        }
                    }
                }
            }

            peer.SendOperationResponse(
                new OperationResponse(getPropertiesRequest.OperationRequest.OperationCode, response),
                sendParameters);
        }

        protected virtual Actor HandleJoinOperation(
            PlayerPeer peer,
            JoinRequest joinRequest,
            SendParameters sendParameters)
        {
            if (IsDisposed)
            {
                // join arrived after being disposed - repeat join operation                
                if (Log.IsWarnEnabled)
                {
                    Log.WarnFormat("Join operation on disposed game. GameName={0}", Name);
                }

                return null;
            }

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("Join operation from IP: {0} to port: {1}", peer.RemoteIP, peer.LocalPort);
            }

            // create an new actor
            Actor actor;
            if (TryAddPeerToGame(peer, joinRequest.ActorNr, out actor) == false)
            {
                peer.SendOperationResponse(
                    new OperationResponse
                        {
                            OperationCode = joinRequest.OperationRequest.OperationCode,
                            ReturnCode = -1,
                            DebugMessage = "Peer already joined the specified game."
                        },
                    sendParameters);
                return null;
            }

            // check if a room removal is in progress and cancel it if so
            if (RemoveTimer != null)
            {
                RemoveTimer.Dispose();
                RemoveTimer = null;
            }

            // set game properties for join from the first actor (game creator)
            if (Actors.Count == 1)
            {
                DeleteCacheOnLeave = joinRequest.DeleteCacheOnLeave;
                SuppressRoomEvents = joinRequest.SuppressRoomEvents;

                if (joinRequest.GameProperties != null)
                {
                    Properties.SetProperties(joinRequest.GameProperties);
                }
            }

            // set custom actor properties if defined
            if (joinRequest.ActorProperties != null)
            {
                actor.Properties.SetProperties(joinRequest.ActorProperties);
            }

            // set operation return values and publish the response
            var joinResponse = new JoinResponse { ActorNr = actor.ActorNr };

            if (Properties.Count > 0)
            {
                joinResponse.CurrentGameProperties = Properties.GetProperties();
            }

            foreach (var t in Actors)
            {
                if (t.ActorNr != actor.ActorNr && t.Properties.Count > 0)
                {
                    if (joinResponse.CurrentActorProperties == null)
                    {
                        joinResponse.CurrentActorProperties = new Hashtable();
                    }

                    var actorProperties = t.Properties.GetProperties();
                    joinResponse.CurrentActorProperties.Add(t.ActorNr, actorProperties);
                }
            }

            peer.SendOperationResponse(
                new OperationResponse(joinRequest.OperationRequest.OperationCode, joinResponse),
                sendParameters);

            // publish join event
            PublishJoinEvent(peer, joinRequest);

            PublishEventCache(peer);

            return actor;
        }

        protected virtual void HandleLeaveOperation(
            PlayerPeer peer,
            LeaveRequest leaveRequest,
            SendParameters sendParameters)
        {
            RemovePeerFromGame(peer, leaveRequest);

            // is always reliable, so it gets a response
            peer.SendOperationResponse(
                new OperationResponse { OperationCode = leaveRequest.OperationRequest.OperationCode },
                sendParameters);
        }

        protected virtual void HandleRaiseEventOperation(
            PlayerPeer peer,
            RaiseEventRequest raiseEventRequest,
            SendParameters sendParameters)
        {
            // get the actor who send the operation request
            var actor = GetActorByPeer(peer);
            if (actor == null)
            {
                return;
            }

            sendParameters.Flush = raiseEventRequest.Flush;

            if (raiseEventRequest.Cache == (byte)CacheOperation.RemoveFromRoomCache)
            {
                EventCache.RemoveEvents(raiseEventRequest);
                var response =
                    new OperationResponse(raiseEventRequest.OperationRequest.OperationCode) { ReturnCode = 0 };
                peer.SendOperationResponse(response, sendParameters);
                return;
            }

            if (raiseEventRequest.Cache == (byte)CacheOperation.RemoveFromCacheForActorsLeft)
            {
                var currentActorNumbers = Actors.GetActorNumbers();
                EventCache.RemoveEventsForActorsNotInList(currentActorNumbers);
                var response =
                    new OperationResponse(raiseEventRequest.OperationRequest.OperationCode) { ReturnCode = 0 };
                peer.SendOperationResponse(response, sendParameters);
                return;
            }

            // publish the custom event
            var customEvent = new CustomEvent(actor.ActorNr, raiseEventRequest.EvCode, raiseEventRequest.Data);

            var updateEventCache = false;
            IEnumerable<Actor> recipients;

            if (raiseEventRequest.Actors != null && raiseEventRequest.Actors.Length > 0)
            {
                recipients = Actors.GetActorsByNumbers(raiseEventRequest.Actors);
            }
            else if (raiseEventRequest.Group != 0)
            {
                ActorGroup group;
                if (ActorGroups.TryGetValue(raiseEventRequest.Group, out group))
                {
                    recipients = group.GetExcludedList(actor);
                }
                else
                {
                    return;
                }
            }
            else
            {
                switch ((ReceiverGroup)raiseEventRequest.ReceiverGroup)
                {
                    case ReceiverGroup.All:
                        recipients = Actors;
                        updateEventCache = true;
                        break;

                    case ReceiverGroup.Others:
                        recipients = Actors.GetExcludedList(actor);
                        updateEventCache = true;
                        break;

                    case ReceiverGroup.MasterClient:
                        recipients = new[] { Actors[0] };
                        break;

                    default:
                        peer.SendOperationResponse(
                            new OperationResponse
                                {
                                    OperationCode = raiseEventRequest.OperationRequest.OperationCode,
                                    ReturnCode = -1,
                                    DebugMessage =
                                        "Invalid ReceiverGroup " + raiseEventRequest.ReceiverGroup
                                },
                            sendParameters);
                        return;
                }
            }

            if (updateEventCache && raiseEventRequest.Cache != (byte)CacheOperation.DoNotCache)
            {
                string msg;
                if (!UpdateEventCache(actor, raiseEventRequest, out msg))
                {
                    peer.SendOperationResponse(
                        new OperationResponse
                            {
                                OperationCode = raiseEventRequest.OperationRequest.OperationCode,
                                ReturnCode = -1,
                                DebugMessage = msg
                            },
                        sendParameters);
                    return;
                }
            }

            PublishEvent(customEvent, recipients, sendParameters);
        }

        protected virtual void HandleSetPropertiesOperation(
            PlayerPeer peer,
            SetPropertiesRequest setPropertiesRequest,
            SendParameters sendParameters)
        {
            // check if peer has joined this room instance
            var sender = GetActorByPeer(peer);
            if (sender == null)
            {
                var response = new OperationResponse
                                   {
                                       OperationCode =
                                           setPropertiesRequest.OperationRequest.OperationCode,
                                       ReturnCode = -1,
                                       DebugMessage = "Room not joined"
                                   };

                peer.SendOperationResponse(response, sendParameters);
                return;
            }

            if (setPropertiesRequest.ActorNumber > 0)
            {
                var actor = Actors.GetActorByNumber(setPropertiesRequest.ActorNumber);
                if (actor == null)
                {
                    peer.SendOperationResponse(
                        new OperationResponse
                            {
                                OperationCode = setPropertiesRequest.OperationRequest.OperationCode,
                                ReturnCode = -1,
                                DebugMessage = string.Format(
                                    "Actor with number {0} not found.",
                                    setPropertiesRequest.ActorNumber)
                            },
                        sendParameters);
                    return;
                }

                actor.Properties.SetProperties(setPropertiesRequest.Properties);
            }
            else
            {
                Properties.SetProperties(setPropertiesRequest.Properties);
            }

            peer.SendOperationResponse(
                new OperationResponse { OperationCode = setPropertiesRequest.OperationRequest.OperationCode },
                sendParameters);

            // if the optional paramter Broadcast is set a EvPropertiesChanged
            // event will be send to room actors
            if (setPropertiesRequest.Broadcast)
            {
                var actor = Actors.GetActorByPeer(peer);
                var recipients = Actors.GetExcludedList(actor);
                var propertiesChangedEvent =
                    new PropertiesChangedEvent(actor.ActorNr)
                        {
                            TargetActorNumber = setPropertiesRequest.ActorNumber,
                            Properties = setPropertiesRequest.Properties
                        };

                PublishEvent(propertiesChangedEvent, recipients, sendParameters);
            }
        }

        protected virtual void HandleChangeGroupsOperation(
            PlayerPeer peer,
            ChangeGroups changeGroupsRequest,
            SendParameters sendParameters)
        {
            // get the actor who send the operation request
            var actor = GetActorByPeer(peer);
            if (actor == null)
            {
                return;
            }

            actor.RemoveGroups(changeGroupsRequest.Remove);

            if (changeGroupsRequest.Add != null)
            {
                if (changeGroupsRequest.Add.Length > 0)
                {
                    foreach (var groupId in changeGroupsRequest.Add)
                    {
                        ActorGroup group;
                        if (!ActorGroups.TryGetValue(groupId, out group))
                        {
                            group = new ActorGroup(groupId);
                            ActorGroups.Add(groupId, group);
                        }

                        actor.AddGroup(group);
                    }
                }
                else
                {
                    foreach (var group in ActorGroups.Values)
                    {
                        actor.AddGroup(group);
                    }
                }
            }
        }

        #endregion
    }
}