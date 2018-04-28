using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRequestRes
{
    public string name;
    public GameObject uiObj;

    public UIRequestRes(string name, GameObject uiObj)
    {
        this.name = name;
        this.uiObj = uiObj;
    }
}
