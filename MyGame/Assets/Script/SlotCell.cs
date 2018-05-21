using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class SlotCell : MonoBehaviour
{
    private byte index;
    public byte Index
    {
        get
        {
            return index;
        }

        set
        {
            index = value;
        }
    }

    private float limitHeight;
    private bool isReturn;
    public bool IsReturn
    {
        get
        {
            return isReturn;
        }

        private set
        {
            isReturn = value;
        }
    }
    private byte tile;
    public byte Tile
    {
        get
        {
            return tile;
        }

        set
        {
            tile = value;
            GetComponent<Image>().sprite = SlotMachineManager.Instance.sprites[tile - 1];
        }
    }

    public void Init(float limitHeight)
    {
        this.limitHeight = limitHeight;
        gameObject.SetActive(true);
    }

    public void Move(float speed)
    {
        GetComponent<RectTransform>().anchoredPosition3D -= new Vector3(0f, speed, 0f);
        IsReturn = GetComponent<RectTransform>().anchoredPosition3D.y <= limitHeight;
    }

}
