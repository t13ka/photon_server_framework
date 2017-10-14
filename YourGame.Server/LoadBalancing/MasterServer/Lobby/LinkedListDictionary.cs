using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace YourGame.Server.MasterServer.Lobby
{
    public class LinkedListDictionary<TKey, TValue> : IEnumerable<TValue>
    {
        private readonly Dictionary<TKey, LinkedListNode<TValue>> _dict;
        private readonly LinkedList<TValue> _linkedList;

        public LinkedListDictionary()
        {
            _linkedList = new LinkedList<TValue>();
            _dict = new Dictionary<TKey, LinkedListNode<TValue>>();
        }

        public LinkedListDictionary(int capacity)
        {
            _linkedList = new LinkedList<TValue>();
            _dict = new Dictionary<TKey, LinkedListNode<TValue>>(capacity);
        }

        public int Count
        {
            get { return _dict.Count; }
        }

        public LinkedListNode<TValue> First
        {
            get { return _linkedList.First; }
        }

        #region IEnumerable<TValue> Members

        public IEnumerator<TValue> GetEnumerator()
        {
            return _linkedList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _linkedList.GetEnumerator();
        }

        #endregion

        public void Add(TKey key, TValue value)
        {
            var node = new LinkedListNode<TValue>(value);
            _dict.Add(key, node);
            _linkedList.AddLast(node);
        }

        public bool ContainsKey(TKey key)
        {
            return _dict.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            LinkedListNode<TValue> node;
            if (_dict.TryGetValue(key, out node) == false)
            {
                return false;
            }

            _linkedList.Remove(node);
            _dict.Remove(key);

            return true;
        }

        public bool TryGet(TKey key, out TValue value)
        {
            LinkedListNode<TValue> node;
            if (_dict.TryGetValue(key, out node))
            {
                value = node.Value;
                return true;
            }

            value = default(TValue);
            return false;
        }

        public LinkedListNode<TValue> GetAtIntext(int index)
        {
            if (index >= Count)
            {
                throw new ArgumentOutOfRangeException("index", "The specified index is out of range");
            }

            if (index < Count/2)
            {
                var node = _linkedList.First;
                for (var i = 0; i < index; i++)
                {
                    Debug.Assert(node != null, "node != null");
                    node = node.Next;
                }

                return node;
            }
            else
            {
                var node = _linkedList.Last;
                for (var i = _linkedList.Count - 1; i > index; i--)
                {
                    Debug.Assert(node != null, "node != null");
                    node = node.Previous;
                }

                return node;
            }
        }
    }
}