using System;
using System.Collections.Generic;

public class EventBus : IEventBus
{
    private Dictionary<Type, Delegate> _events = new Dictionary<Type, Delegate>();

    public void Subscribe<T>(Action<T> handler)
    {
        var type = typeof(T);
        if (_events.TryGetValue(type, out var existing))
            _events[type] = Delegate.Combine(existing, handler);
        else
            _events[type] = handler;
    }

    public void Unsubscribe<T>(Action<T> handler)
    {
        var type = typeof(T);
        if (_events.TryGetValue(type, out var existing))
        {
            var current = Delegate.Remove(existing, handler);
            if (current == null) _events.Remove(type);
            else _events[type] = current;
        }
    }

    public void Publish<T>(T evt)
    {
        var type = typeof(T);
        if (_events.TryGetValue(type, out var d))
        {
            var action = d as Action<T>;
            action?.Invoke(evt);
        }
    }
}
