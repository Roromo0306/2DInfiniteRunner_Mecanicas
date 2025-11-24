using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameContainer
{
    private static Dictionary<Type, IService> _services = new Dictionary<Type, IService>();

    public static void Register<T>(T service) where T : IService
    {
        _services[typeof(T)] = service;
    }

    public static T Resolve<T>() where T : IService
    {
        if (_services.TryGetValue(typeof(T), out var s))
            return (T)s;
        throw new Exception($"Service {typeof(T)} not registered");
    }

    public static bool IsRegistered<T>() where T : IService
    {
        return _services.ContainsKey(typeof(T));
    }
}
