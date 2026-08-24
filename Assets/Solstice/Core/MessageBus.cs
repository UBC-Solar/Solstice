using System;
using System.Collections.Generic;

// Needed for records as implementations of IMessage to compile. Just a compiler flag, no implementation needed.
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

namespace Solstice
{
    // Marker interface — every message type implements this.
    // Using records means every message is immutable by construction.
    public interface IMessage { }

    // --- The bus itself ---
    public class MessageBus
    {
        // Keyed by message Type, value is an Action<T> boxed as a plain Delegate.
        private readonly Dictionary<Type, Delegate> _handlers = new();

        public void Subscribe<T>(Action<T> handler) where T : IMessage
        {
            var type = typeof(T);
            if (_handlers.TryGetValue(type, out var existing))
                _handlers[type] = Delegate.Combine(existing, handler);
            else
                _handlers[type] = handler;
        }

        public void Unsubscribe<T>(Action<T> handler) where T : IMessage
        {
            var type = typeof(T);
            if (!_handlers.TryGetValue(type, out var existing)) return;

            var remaining = Delegate.Remove(existing, handler);
            if (remaining == null)
                _handlers.Remove(type);
            else
                _handlers[type] = remaining;
        }

        public void Publish<T>(T message) where T : IMessage
        {
            if (_handlers.TryGetValue(typeof(T), out var existing))
                ((Action<T>)existing).Invoke(message);
        }
    }
}
