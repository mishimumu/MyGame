using UnityEngine;
using System.Collections;
using System;

public class Singleton<T> where T : new()
{

    private static T instance;
    public static T Instance
    {
        get
        {
            if (Singleton<T>.instance == null)
            {
                Singleton<T>.instance = ((default(T) == null) ? Activator.CreateInstance<T>() : default(T));
            }
            return Singleton<T>.instance;
        }
    }
    public static void Release()
    {
        Singleton<T>.instance = default(T);
    }
    public static bool IsInstanceValid()
    {
        return Singleton<T>.instance != null;
    }
}
