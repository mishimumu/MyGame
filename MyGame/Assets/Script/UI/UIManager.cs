using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UIManager
{
    public List<UIRequest> waitingRequestList = new List<UIRequest>();
    public List<UIRequest> finshedRequestList = new List<UIRequest>();
    public Queue<UIRequest> requestQueue = new Queue<UIRequest>();
    public Dictionary<string, GameObject> uiMap = new Dictionary<string, GameObject>();
    public List<string> blackName = new List<string>();//场景黑名单
                                                       //对是否可以使用返回键的界面进行分类，同时管理可返回界面的层次
    public List<string> mainUIMap = new List<string>();
    public List<string> normalUIMap = new List<string>();//用于快速返回
    public List<GameObject> curObjInQueue = new List<GameObject>();
    public Stack<UIRequest> navRequestStack = new Stack<UIRequest>();
    public bool isPause;
    public bool isLoading;

    bool IsHasRequest(string name)
    {

        foreach (var req in waitingRequestList)
        {
            if (string.Equals(name, req.name))
            {
                return true;
            }
        }

        foreach (var req in finshedRequestList)
        {
            if (string.Equals(name, req.name))
            {
                return true;
            }
        }

        return false;
    }

    public void Clear()
    {
        waitingRequestList.Clear();
        finshedRequestList.Clear();
        normalUIMap.Clear();
        mainUIMap.Clear();
        blackName.Clear();
        curObjInQueue.Clear();
        requestQueue.Clear();
        navRequestStack.Clear();
    }

    public void Close(string name)
    {

        if (string.IsNullOrEmpty(name) || !uiMap.ContainsKey(name)) return;

        foreach (var req in finshedRequestList)
        {
            if (string.Equals(name, req.name))
            {
                finshedRequestList.Remove(req);
                break;
            }
        }

        if (normalUIMap.Contains(name))
        {
            normalUIMap.Remove(name);
        }

        if (mainUIMap.Contains(name))
        {
            mainUIMap.Remove(name);
        }

        GameObject obj = uiMap[name];
        uiMap.Remove(name);
        if (curObjInQueue.Contains(obj))
        {
            curObjInQueue.Remove(obj);
        }
        UResourceManger.Instance.Destory(obj);

    }

    void Update()
    {
        if (isPause || isLoading) return;
        Excete();
        ExceteQueue();
    }


    public void Add(UIRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.name) || blackName.Contains(request.name) || IsHasRequest(request.name)) return;

        waitingRequestList.Add(request);

        Update();
    }

    public void Return(string name)
    {
        if (navRequestStack.Count > 0)
        {
            UIRequest lastRequest = navRequestStack.Peek();
            //当前界面返回时，导航中的目标界面和当前界面相同时，直接关闭界面
            if (lastRequest.name != name)
            {
                lastRequest = navRequestStack.Pop();
                waitingRequestList.Add(lastRequest);
            }
            Close(name);
        }
        else
        {
            Debug.Log("导航异常！！！！");
            Close(name);
        }
    }

    UIRequest GetUIRequest(string name)
    {
        foreach (var quest in finshedRequestList)
        {
            if (quest.name == name)
            {
                return quest;
            }
        }
        return null;

    }

    public void ReturnByReturnKey()
    {
        //ui num >1 ,close ui one by one
        //ui num =1 ,return by nav
        //ui num =0 ,quit
        if (normalUIMap.Count > 1)
        {
            Close(normalUIMap[normalUIMap.Count - 1]);
        }
        else if (normalUIMap.Count == 1)
        {
            Return(normalUIMap[normalUIMap.Count - 1]);
        }
        else
        {
            Application.Quit();
        }
    }


    public void Excete()
    {

        while (waitingRequestList.Count > 0)
        {
            isLoading = true;
            UIRequest request = waitingRequestList[0];
            waitingRequestList.Remove(request);

            if (request.isQueue)
            {
                requestQueue.Enqueue(request);
            }
            else
            {
                GameObject target = UResourceManger.Instance.Load<GameObject>();

                if (request.isNav)
                {
                    navRequestStack.Push(request);
                    //navStack.Push(target);
                }
                FinishRequest(request, target);
            }
        }

        isLoading = false;
    }

    public void ExceteQueue()
    {
        if (requestQueue.Count == 0 || curObjInQueue.Count > 0) return;

        UIRequest request = requestQueue.Dequeue();
        GameObject target = UResourceManger.Instance.Load<GameObject>();
        if (uiMap.ContainsKey(request.name))
        {
            Debug.LogWarning("该加载界面已存在");
            MonoBehaviour.Destroy(target);
        }
        else
        {
            curObjInQueue.Add(target);
            FinishRequest(request, target);
        }
    }

    void FinishRequest(UIRequest request, GameObject obj)
    {
        if (obj == null)
        {
            Debug.LogWarning("界面资源为空");
            return;
        }
        uiMap.Add(request.name, obj);
        finshedRequestList.Add(request);
        if (request.data.uiLevel == UILevelType.Main)
        {
            mainUIMap.Add(request.name);
        }
        else
        {
            normalUIMap.Add(request.name);
        }
        request.callback?.Invoke(new UIRequestRes(request.name, obj));
    }


    /// <summary>
    /// 清除导航链.当要中断导航时，调用这个方法
    /// </summary>
    public void ClearNav()
    {
        navRequestStack.Clear();
    }

}

public enum UIRequestState
{
    Default,
    Loading,
    Finished
}

/// <summary>
/// 弹窗层级
/// </summary>
public enum UILevelType
{
    Default,
    Main,
    Top
}


