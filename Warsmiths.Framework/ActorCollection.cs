namespace YourGame.Server.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Photon.SocketServer;

    public class ActorCollection : List<Actor>
    {
        public Actor GetActorByNumber(int actorNumber)
        {
            return this.FirstOrDefault(actor => actor.ActorNr == actorNumber);
        }

        public Actor GetActorByPeer(PeerBase peer)
        {
            return this.FirstOrDefault(actor => actor.Peer == peer);
        }

        public IEnumerable<int> GetActorNumbers()
        {
            return this.Select(actor => actor.ActorNr);
        }

        public IEnumerable<Actor> GetExcludedList(Actor actorToExclude)
        {
            return this.Where(actor => actor != actorToExclude);
        }

        public Actor RemoveActorByPeer(PlayerPeer peer)
        {
            var index = FindIndex(actor => actor.Peer == peer);
            if (index == -1)
            {
                return null;
            }

            var result = this[index];
            RemoveAt(index);
            return result;
        }

        public IEnumerable<Actor> GetActorsByNumbers(int[] actors)
        {
            if (!IsSorted(actors))
            {
                var clone = new int[actors.Length];
                Array.Copy(actors, clone, actors.Length);
                Array.Sort(clone);
                actors = clone;
            }

            for (int i = 0, j = 0; i < actors.Length && j < Count; i++)
            {
                // since both lists are sorted we don't have to start at the beginning at each iteration
                for (; j < Count; j++)
                {
                    if (this[j].ActorNr == actors[i])
                    {
                        yield return this[j];
                        break;
                    }
                }
            }

            //// return this.Where(actor => actors.Contains(actor.ActorNr));
        }

        private static bool IsSorted(int[] array)
        {
            for (var i = 1; i < array.Length; i++)
            {
                if (array[i - 1] > array[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}