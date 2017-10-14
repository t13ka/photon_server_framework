namespace YourGame.Server.Framework.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    [Serializable]
    public class PropertyBag<TKey>
    {
        private readonly Dictionary<TKey, Property<TKey>> _dictionary;

        public PropertyBag()
        {
            _dictionary = new Dictionary<TKey, Property<TKey>>();
        }

        public PropertyBag(IEnumerable<KeyValuePair<TKey, object>> values)
            : this()
        {
            foreach (var item in values)
            {
                Set(item.Key, item.Value);
            }
        }

        public PropertyBag(IDictionary values)
            : this()
        {
            foreach (TKey key in values.Keys)
            {
                Set(key, values[key]);
            }
        }

        public int Count
        {
            get
            {
                return _dictionary.Count;
            }
        }

        public event EventHandler<PropertyChangedEventArgs<TKey>> PropertyChanged;

        public IDictionary<TKey, Property<TKey>> AsDictionary()
        {
            return _dictionary;
        }

        public IList<Property<TKey>> GetAll()
        {
            var properties = new Property<TKey>[_dictionary.Count];
            _dictionary.Values.CopyTo(properties, 0);
            return properties;
        }

        public Hashtable GetProperties()
        {
            var result = new Hashtable(_dictionary.Count);
            CopyPropertiesToHashtable(result);
            return result;
        }

        public Hashtable GetProperties(IList<TKey> propertyKeys)
        {
            if (propertyKeys == null)
            {
                return GetProperties();
            }

            var result = new Hashtable(propertyKeys.Count);
            CopyPropertiesToHashtable(result, propertyKeys);
            return result;
        }

        public Hashtable GetProperties(IEnumerable<TKey> propertyKeys)
        {
            if (propertyKeys == null)
            {
                return GetProperties();
            }

            var result = new Hashtable();
            CopyPropertiesToHashtable(result, propertyKeys);
            return result;
        }

        public Hashtable GetProperties(IEnumerable propertyKeys)
        {
            if (propertyKeys == null)
            {
                return GetProperties();
            }

            var result = new Hashtable();
            CopyPropertiesToHashtable(result, propertyKeys);
            return result;
        }

        public Property<TKey> GetProperty(TKey key)
        {
            Property<TKey> value;
            _dictionary.TryGetValue(key, out value);
            return value;
        }

        public void Set(TKey key, object value)
        {
            Property<TKey> property;

            if (_dictionary.TryGetValue(key, out property))
            {
                property.Value = value;
            }
            else
            {
                property = new Property<TKey>(key, value);
                property.PropertyChanged += OnPropertyPropertyChanged;
                _dictionary.Add(key, property);
                RaisePropertyChanged(key, value);
            }
        }

        public void SetProperties(IDictionary values)
        {
            foreach (TKey key in values.Keys)
            {
                Set(key, values[key]);
            }
        }

        public void SetProperties(IDictionary<TKey, object> values)
        {
            foreach (var keyValue in values)
            {
                Set(keyValue.Key, keyValue.Value);
            }
        }

        public bool TryGetValue(TKey key, out object value)
        {
            Property<TKey> property;
            if (_dictionary.TryGetValue(key, out property))
            {
                value = property.Value;
                return true;
            }

            value = null;
            return false;
        }

        private void CopyPropertiesToHashtable(IDictionary hashtable)
        {
            foreach (var keyValue in _dictionary)
            {
                hashtable.Add(keyValue.Key, keyValue.Value.Value);
            }
        }

        private void CopyPropertiesToHashtable(IDictionary hashtable, IEnumerable<TKey> propertyKeys)
        {
            foreach (var key in propertyKeys)
            {
                Property<TKey> property;
                if (_dictionary.TryGetValue(key, out property))
                {
                    hashtable.Add(key, property.Value);
                }
            }
        }

        private void CopyPropertiesToHashtable(IDictionary hashtable, IEnumerable propertyKeys)
        {
            foreach (TKey key in propertyKeys)
            {
                Property<TKey> property;
                if (_dictionary.TryGetValue(key, out property))
                {
                    hashtable.Add(key, property.Value);
                }
            }
        }

        private void OnPropertyPropertyChanged(object sender, EventArgs e)
        {
            var property = (Property<TKey>)sender;
            RaisePropertyChanged(property.Key, property.Value);
        }

        private void RaisePropertyChanged(TKey key, object value)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs<TKey>(key, value));
            }
        }
    }
}