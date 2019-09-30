using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace JetBlack.JsonConsoleLogger
{
    internal class FormattedLogValues : IEnumerable<KeyValuePair<string, object>>, IReadOnlyCollection<KeyValuePair<string, object>>, IReadOnlyList<KeyValuePair<string, object>>
    {
        private static Type? _type;
        private static PropertyInfo? _countPropertyInfo, _itemPropertyInfo;
        private static MethodInfo? _getEnumeratorMethodInfo, _toStringMethodInfo;

        private readonly object _instance;

        public FormattedLogValues(object instance)
        {
            _instance = instance;
        }

        private Type Type => _type ?? (_type = _instance.GetType());
        private PropertyInfo CountPropertyInfo => _countPropertyInfo ?? (_countPropertyInfo = Type.GetProperty("Count"));
        private PropertyInfo ItemPropertyInfo => _itemPropertyInfo ?? (_itemPropertyInfo = Type.GetProperty("Item"));
        private MethodInfo GetEnumeratorMethodInfo => _getEnumeratorMethodInfo ?? (_getEnumeratorMethodInfo = Type.GetMethod("GetEnumerator"));
        private MethodInfo ToStringMethodInfo => _toStringMethodInfo ?? (_toStringMethodInfo = Type.GetMethod("ToString"));

        public int Count => (int)CountPropertyInfo.GetValue(_instance);
        public KeyValuePair<string, object> this[int index] => (KeyValuePair<string, object>)ItemPropertyInfo.GetValue(_instance, new object[] { index });

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return (IEnumerator<KeyValuePair<string, object>>)GetEnumeratorMethodInfo.Invoke(_instance, null);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString() => (string)ToStringMethodInfo.Invoke(_instance, null);
    }
}