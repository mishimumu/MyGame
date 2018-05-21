using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class SlotCell : BaseView
{

    public bool isToggle;
    public byte slotType;
    public byte index;
    public delegate void scrollDelegate();
    public SlotController controller;
    public float limitHeight;
    public bool isReturn;
    public float speed;
    public GameObject image;
    private byte tile;
    Vector3 pos;

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
        pos = transform.position;
    }

    protected override void OnUpDate()
    {
        // ScrollMove();
        if (pos.y != transform.position.y)
        {
            speed = Mathf.Abs(transform.position.y - pos.y);
        }
        else
        {
            speed = 0;
        }
        pos = transform.position;
    }

    public void Move(float speed)
    {
        GetComponent<RectTransform>().anchoredPosition3D -= new Vector3(0f, speed, 0f);
        isReturn = GetComponent<RectTransform>().anchoredPosition3D.y <= limitHeight;
    }

    public bool IsOverLimit()
    {
        return GetComponent<RectTransform>().anchoredPosition3D.y <= limitHeight;
    }

    public void DecreaseMove(float speed)
    {
        GetComponent<RectTransform>().anchoredPosition3D -= new Vector3(0f, speed, 0f);
    }

    public void Return(float time, Vector3 target, iTween.EaseType easetype)
    {
        iTween.MoveTo(gameObject, iTween.Hash("time", time, "position", target, "easetype", easetype, "oncomplete", "FinishAnimation", "oncompletetarget", gameObject, "islocal", false));
    }



    void ScrollMove()
    {
        if (!controller.isStop)
        {

            if (transform.localPosition.y < controller.limitHeight)
            {
                float distance = transform.localPosition.y - controller.height;
                GetComponent<RectTransform>().anchoredPosition3D += new Vector3(0f, controller.height, 0f);
            }
            else
            {
                if (isToggle && controller.isShowResult)
                {

                    float pos = 0f;
                    if (transform.localPosition.y > pos)
                    {
                        GetComponent<RectTransform>().anchoredPosition3D += new Vector3(0f, -controller.speed, 0f);
                    }
                    else
                    {
                        float value = transform.localPosition.y + controller.speed;
                        if (value >= pos)
                        {
                            GetComponent<RectTransform>().anchoredPosition3D = new Vector3(GetComponent<RectTransform>().anchoredPosition3D.x, 0f, 0);
                            if (controller.stopAction != null)
                            {
                                controller.stopAction(controller);
                            }

                        }
                        else
                        {
                            GetComponent<RectTransform>().anchoredPosition3D += new Vector3(0f, -controller.speed, 0f);
                        }
                    }
                }
                else
                {
                    GetComponent<RectTransform>().anchoredPosition3D += new Vector3(0f, -controller.speed, 0f);
                }
            }

        }



    }

    public void SetIsToggle(bool isToggle)
    {
        this.isToggle = isToggle;

    }

}
