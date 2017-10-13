using System.Collections.Generic;

using Photon.SocketServer;

using Warsmiths.Server.Framework.Common;

namespace Warsmiths.Server.Framework
{
    public class Actor
    {
        private readonly List<ActorGroup> _groups = new List<ActorGroup>();

        public Actor()
        {
            Properties = new PropertyBag<object>();
        }

        public Actor(PeerBase peer)
            : this()
        {
            Peer = peer;
        }

        public int ActorNr;

        public PeerBase Peer;

        public PropertyBag<object> Properties { get; private set; }

        public override string ToString()
        {
            return string.Format("Actor {0}: Peer {1}", ActorNr, Peer);
        }

        public void AddGroup(ActorGroup group)
        {
            _groups.Add(group);
            group.Add(this);
        }

        public void RemoveGroups(byte[] groupIds)
        {
            if (groupIds == null)
            {
                return;
            }

            if (groupIds.Length == 0)
            {
                RemoveAllGroups();
                return;
            }

            foreach (var group in groupIds)
            {
                RemoveGroup(group);
            }
        }

        private void RemoveGroup(byte group)
        {
            var actorGroupIndex = _groups.FindIndex(g => g.GroupId == group);
            if (actorGroupIndex == -1)
            {
                return;
            }

            _groups[actorGroupIndex].RemoveActorByPeer((PlayerPeer)Peer);
            _groups.RemoveAt(actorGroupIndex);
        }

        private void RemoveAllGroups()
        {
            foreach (var group in _groups)
            {
                group.RemoveActorByPeer((PlayerPeer)Peer);
            }

            _groups.Clear();
        }
    }
}