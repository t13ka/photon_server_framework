namespace YourGame.Server.Framework.Caching
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using YourGame.Server.Framework.Events;
    using YourGame.Server.Framework.Operations;

    [Serializable]
    public class RoomEventCache : IEnumerable<CustomEvent>
    {
        #region Constants and Fields

        private readonly List<CustomEvent> _cachedRoomEvents = new List<CustomEvent>();

        #endregion

        #region Methods

        private static bool Compare(Hashtable h1, Hashtable h2)
        {
            if (h1 == null && h2 == null)
            {
                return true;
            }

            if (h1 == null || h2 == null)
            {
                return false;
            }

            foreach (DictionaryEntry entry in h1)
            {
                if (h2.ContainsKey(entry.Key) == false)
                {
                    return false;
                }

                var cachedParam = h2[entry.Key];
                if (entry.Value == null)
                {
                    if (cachedParam != null)
                    {
                        return false;
                    }

                    continue;
                }

                if (entry.Value.Equals(cachedParam) == false)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Public Methods

        public void AddEevents(IEnumerable<CustomEvent> e)
        {
            foreach (var ev in e)
            {
                AddEvent(ev);
            }
        }

        public void AddEvent(CustomEvent customeEvent)
        {
            _cachedRoomEvents.Add(customeEvent);
        }

        public void RemoveEvents(RaiseEventRequest raiseEventRequest)
        {
            if (raiseEventRequest.EvCode == 0 && raiseEventRequest.Actors == null && raiseEventRequest.Data == null)
            {
                _cachedRoomEvents.Clear();
                return;
            }

            for (var i = _cachedRoomEvents.Count - 1; i >= 0; i--)
            {
                var cachedEvent = _cachedRoomEvents[i];

                if (raiseEventRequest.EvCode != 0 && cachedEvent.Code != raiseEventRequest.EvCode)
                {
                    continue;
                }

                if (raiseEventRequest.Actors != null && raiseEventRequest.Actors.Length > 0)
                {
                    var actorMatch = false;
                    for (var a = 0; a < raiseEventRequest.Actors.Length; a++)
                    {
                        if (cachedEvent.ActorNr != raiseEventRequest.Actors[a])
                        {
                            continue;
                        }

                        actorMatch = true;
                        break;
                    }

                    if (actorMatch == false)
                    {
                        continue;
                    }
                }

                if (raiseEventRequest.Data == null)
                {
                    _cachedRoomEvents.RemoveAt(i);
                    continue;
                }

                if (Compare(raiseEventRequest.Data as Hashtable, cachedEvent.Data as Hashtable))
                {
                    _cachedRoomEvents.RemoveAt(i);
                }
            }
        }

        public void RemoveEventsByActor(int actorNumber)
        {
            for (var i = _cachedRoomEvents.Count - 1; i >= 0; i--)
            {
                if (_cachedRoomEvents[i].ActorNr == actorNumber)
                {
                    _cachedRoomEvents.RemoveAt(i);
                }
            }
        }

        public void RemoveEventsForActorsNotInList(IEnumerable<int> actorsNumbers)
        {
            var hashSet = new HashSet<int>(actorsNumbers);

            for (var i = _cachedRoomEvents.Count - 1; i >= 0; i--)
            {
                if (hashSet.Contains(_cachedRoomEvents[i].ActorNr) == false)
                {
                    _cachedRoomEvents.RemoveAt(i);
                }
            }
        }

        #endregion

        #region Implemented Interfaces

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _cachedRoomEvents.GetEnumerator();
        }

        #endregion

        #region IEnumerable<CustomEvent>

        public IEnumerator<CustomEvent> GetEnumerator()
        {
            return _cachedRoomEvents.GetEnumerator();
        }

        #endregion

        #endregion
    }
}