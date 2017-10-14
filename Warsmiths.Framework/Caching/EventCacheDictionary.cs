namespace YourGame.Server.Framework.Caching
{
    using System.Collections;
    using System.Collections.Generic;

    public class EventCacheDictionary : IEnumerable<KeyValuePair<int, EventCache>>
    {
        private readonly Dictionary<int, EventCache> _dictionary = new Dictionary<int, EventCache>();

        public IEnumerator<KeyValuePair<int, EventCache>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        public EventCache GetOrCreateEventCache(int actorNumber)
        {
            EventCache eventCache;
            if (TryGetEventCache(actorNumber, out eventCache) == false)
            {
                eventCache = new EventCache();
                _dictionary.Add(actorNumber, eventCache);
            }

            return eventCache;
        }

        public bool TryGetEventCache(int actorNumber, out EventCache eventCache)
        {
            return _dictionary.TryGetValue(actorNumber, out eventCache);
        }

        public bool RemoveEventCache(int actorNumber)
        {
            return _dictionary.Remove(actorNumber);
        }

        public void ReplaceEvent(int actorNumber, byte eventCode, Hashtable eventData)
        {
            var eventCache = GetOrCreateEventCache(actorNumber);
            if (eventData == null)
            {
                eventCache.Remove(eventCode);
            }
            else
            {
                eventCache[eventCode] = eventData;
            }
        }

        public bool RemoveEvent(int actorNumber, byte eventCode)
        {
            EventCache eventCache;
            if (!_dictionary.TryGetValue(actorNumber, out eventCache))
            {
                return false;
            }

            return eventCache.Remove(eventCode);
        }

        public void MergeEvent(int actorNumber, byte eventCode, Hashtable eventData)
        {
            if (eventData == null)
            {
                RemoveEvent(actorNumber, eventCode);
                return;
            }

            var eventCache = GetOrCreateEventCache(actorNumber);

            Hashtable storedEventData;
            if (eventCache.TryGetValue(eventCode, out storedEventData) == false)
            {
                eventCache.Add(eventCode, eventData);
                return;
            }

            foreach (DictionaryEntry pair in eventData)
            {
                if (pair.Value == null)
                {
                    storedEventData.Remove(pair.Key);
                }
                else
                {
                    storedEventData[pair.Key] = pair.Value;
                }
            }
        }
    }
}