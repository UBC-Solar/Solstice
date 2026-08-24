using System;
using System.Collections.Generic;
using UnityEngine;

namespace Solstice
{
    [Serializable]
    public class Signal<T>
    {
        [field: SerializeField]
        public T Value { get; private set; }
        public event Action<T> Changed;

        public Signal(T initial) => Value = initial;

        public void Set(T value)
        {
            if (EqualityComparer<T>.Default.Equals(Value, value)) return;
            Value = value;
            Changed?.Invoke(value);
        }
    }
}