using System;

namespace Warsmiths.Server.Framework.Common
{
    [Serializable]
    public class Property<TKey>
    {
        private object _value;

        public Property(TKey key, object value)
        {
            Key = key;
            Value = value;
        }

        public TKey Key { get; private set; }

        public object Value
        {
            get
            {
                return _value;
            }

            set
            {
                if (_value != value)
                {
                    _value = value;
                    RaisePropertyChanged();
                }
            }
        }

        public event EventHandler PropertyChanged;

        private void RaisePropertyChanged()
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, EventArgs.Empty);
            }
        }
    }
}