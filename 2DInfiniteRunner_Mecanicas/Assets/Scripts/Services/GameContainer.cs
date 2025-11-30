using System;
using System.Collections.Generic;

/// <summary>
/// Contenedor simple de registros/resolución por tipo.
/// Ahora acepta cualquier tipo (no solo IService).
/// </summary>
public static class GameContainer
{
    private static Dictionary<Type, object> _services = new Dictionary<Type, object>();

    public static void Register<T>(T service)
    {
        _services[typeof(T)] = service;
    }

    public static T Resolve<T>()
    {
        if (_services.TryGetValue(typeof(T), out var s))
            return (T)s;
        throw new Exception($"Service/type {typeof(T)} not registered in GameContainer");
    }

    public static bool IsRegistered<T>()
    {
        return _services.ContainsKey(typeof(T));
    }

    public static void Unregister<T>()
    {
        _services.Remove(typeof(T));
    }

    public static void ClearAll()
    {
        _services.Clear();
    }
}

