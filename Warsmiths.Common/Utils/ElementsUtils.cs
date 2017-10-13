using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Warsmiths.Common.Domain;
using Warsmiths.Common.Domain.Elements;

namespace Warsmiths.Common.Utils
{
    [Obsolete]
    public static class ElementsUtils
    {
        /// <summary>
        /// </summary>
        private static readonly Dictionary<Type, BaseElement> Elements = new Dictionary<Type, BaseElement>();

        [Obsolete]
        static ElementsUtils()
        {
            var query = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof (BaseElement)));

            foreach (var t in query)
            {
                var type = t;

                if (type.IsAbstract)
                    continue;

                var instance = Activator.CreateInstance(type);

                if (instance != null)
                {
                    var typedInstance = (BaseElement) instance;

                    Elements.Add(typedInstance.GetType(), typedInstance);
                }
            }
        }

        [Obsolete]
        public static IEnumerable<BaseElement> GetAllElements()
        {
            return Elements.Select(t => t.Value);
        }

        [Obsolete]
        public static BaseElement Get<T>()
        {
            BaseElement e;
            Elements.TryGetValue(typeof (T), out e);
            return e;
        }
    }
}