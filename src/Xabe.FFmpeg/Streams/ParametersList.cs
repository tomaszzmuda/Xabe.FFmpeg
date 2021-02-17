using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xabe.FFmpeg.Streams
{
    internal class ParametersList<T> : IEnumerable<T>
    {
        Dictionary<T, T> _items = new Dictionary<T, T>();

        public IEnumerator<T> GetEnumerator()
        {
            return _items.Select(x => x.Value).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        internal void Add(T item)
        {
            _items[item] = item;
        }

    }
}
