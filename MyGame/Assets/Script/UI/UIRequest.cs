using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRequest
{
    public string name;
    public bool isQueue;//队列时导航无效
    public bool isNav;
    public UUIData data;
    public System.Action<UIRequestRes> callback;
}

