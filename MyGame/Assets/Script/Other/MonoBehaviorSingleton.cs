using UnityEngine;
using System.Collections;
using System;

public class MonoBehaviorSingleton<T> : MonoBehaviour where T : MonoBehaviour
{

    public static bool isApplicationQuit = false;

    private static T instance;
    public static T Instance
    {
        get
        {
            if (isApplicationQuit)
            {
                Debug.LogWarning("MonoBehaviorSingleton is deleted:" + typeof(T).ToString());
                return null;
            }
            if (MonoBehaviorSingleton<T>.instance == null)
            {

                T t = UnityEngine.Object.FindObjectOfType(typeof(T)) as T;
                if (t != null)
                {
                    MonoBehaviorSingleton<T>.instance = t;
                }
                else
                {
                    MonoBehaviorSingleton<T>.instance = (T)((object)new GameObject
                    {
                        name = "MonoBehaviorSingleton_" + typeof(T).Name
                    }.AddComponent<T>());
                }
                UnityEngine.Object.DontDestroyOnLoad(MonoBehaviorSingleton<T>.instance.gameObject);
            }
            return MonoBehaviorSingleton<T>.instance;
        }
    }
    public static bool IsValid()
    {
        return MonoBehaviorSingleton<T>.instance != null;
    }

    public virtual void OnDestroy()
    {
        isApplicationQuit = true;
        OnDestroyed();
    }

    protected void ApplicationQuit()
    {
        isApplicationQuit = true;
        //OnApplicationQuit();
    }

    //protected virtual void OnApplicationQuitCompleted(){}


    public static void Release()
    {
        if (MonoBehaviorSingleton<T>.instance == null)
        {
            return;
        }
        T t = MonoBehaviorSingleton<T>.instance;
        MonoBehaviorSingleton<T>.instance = (T)((object)null);
        UnityEngine.Object.DestroyImmediate(t.gameObject);
        t = (T)((object)null);
    }

    protected virtual void OnDestroyed()
    {

    }
}
