using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotMachine : MonoBehaviour
{

    //启动速度从0到最大速度时间
    //最大速度转动时间
    //最大速度减速到最小速度时间
    //移动n-1格的时间，计算出归位速度，当减速小于等于这个速度时，用itween归位（这样使用动画曲线实现不同效果）
    public RectTransform[] objs;
    public List<SlotCell> slotList;
    public float startTime;
    public float endTime;
    public float transitionTime;
    public float returnTime;
    public bool isStart;//是否开始转动，是开始加速
    public bool isEnd;//转动完成，结束
    public float width;
    public float height;
    public float xSpace;
    public float ySpace;
    public float x;
    public float y;
    public bool isStop;//是否结束转动，是开始减速，否继续转动
    public AnimationCurve startAnimation;
    public AnimationCurve endAnimation;
    public float time;
    public float speed;
    public SlotState slotState;
    public float limitHeight;
    public System.Action startSlotEvent;//开始转动事件
    public System.Action slowDownEvent;//开始减速事件，这个点需要奖励结果
    public System.Action returnOriginEvent;//最后时刻移动单位回到起点
    public System.Action<SlotMachine> FinishSlotEvent;//完成一轮老虎机
    public iTween.EaseType easetype;
    public List<Vector3> tp = new List<Vector3>();
    public List<GameObject> temp = new List<GameObject>();
    public static bool isItween = true;
    public byte[] cells;
    public byte[] initCells;
    public List<byte> leftTiles;
    public SlotMachineTilePool tilePool;
    /*
     *              ---------
     *              *  5 
     *              *  4      <---显示区域上方        转动初始所需位置为[0,5]，2N
     *              *  3
     *              ---------
     *              *  2
     *              *  1      <---显示区域         
     *              *  0                                
     *              ---------
     *              *  -1
     *              *  -2      <----显示区域下方     tp位置为[-3,2]，2N. 超过-1则return到上方
     *              *  -3
     *              --------
     *              最后复位时,
     *              0 -> -3   
     *              1 -> -2
     *              2 -> -1
     *              3 -> 0
     *              4 -> 1
     *              5 -> 2
     */
    private int animationFinishTimes;
    //解决归位问题

    // Use this for initialization
    void Start()
    {
        foreach (var obj in temp)
        {

            if (isItween)
            {
                tp.Add(obj.transform.position);
            }
            else
            {
                tp.Add(obj.GetComponent<RectTransform>().anchoredPosition3D);
            }
        }

        if (isItween)
        {
            endAnimation.AddKey(returnTime, y * 3 / (endTime - returnTime) / 30);
        }
    }

    public void Init()
    {
        tilePool = new SlotMachineTilePool(cells);
        for (int i = 0; i < slotList.Count; i++)
        {
            slotList[i].Tile = initCells[i];
            slotList[i].gameObject.SetActive(true);
        }
    }

    void Fresh()
    {

    }

    public void Choose(byte[] result)
    {
        //for (int i = 0; i < cells.Length; i++)
        //{
        //    // slotList[i].sprite=
        //    for (int y = 0; y < result.Length; y++)
        //    {
        //        if (result[y] == cells[i]) break;
        //    }
        //    leftTiles.Add(cells[i]);
        //}

        tilePool.Reward(result);
    }

    // Update is called once per frame
    void Update()
    {
        if (isStart)
        {
            time += Time.deltaTime;
            switch (slotState)
            {
                case SlotState.Default:
                    isStart = false;
                    isStop = false;
                    time = 0;
                    break;
                case SlotState.SpeedUp:
                    OnSpeedUp(ref time);
                    break;
                case SlotState.Transition:
                    OnTransition(ref time);
                    break;
                case SlotState.SlowDown:
                    //减速开始将开奖结果移除循环表中，待即将展示时，放到屏幕外第一个位置
                    OnSlowdown(endTime);
                    /*
                    speed = endAnimation.Evaluate(time);
                    if (isItween)
                    {
                      //  float t = (endTime - returnTime);
                        animationFinishTimes = 0;
                        slotState = SlotState.Return;
                        foreach (var cell in slotList)
                        {
                            //  cell.Return(t, tp[cell.index],easetype);
                            iTween.MoveTo(cell.gameObject, iTween.Hash("time", endTime, "position", tp[cell.index], "easetype", easetype, "oncomplete", "FinishAnimation", "oncompletetarget", gameObject, "islocal", false));
                        }
                        returnOriginEvent?.Invoke();
                        //if (time >= returnTime)
                        //{
                        //    float t = (endTime - returnTime);
                        //    animationFinishTimes = 0;
                        //    slotState = SlotState.Return;
                        //    foreach (var cell in slotList)
                        //    {
                        //        //  cell.Return(t, tp[cell.index],easetype);
                        //        iTween.MoveTo(cell.gameObject, iTween.Hash("time", t, "position", tp[cell.index], "easetype", easetype, "oncomplete", "FinishAnimation", "oncompletetarget", gameObject, "islocal", false));
                        //    }
                        //    returnOriginEvent?.Invoke();
                        //}
                        //else
                        //{
                        //    foreach (var cell in slotList)
                        //    {
                        //        cell.Move(speed);
                        //    }
                        //    Return();
                        //}
                    }
                    else
                    {
                        foreach (var cell in slotList)
                        {
                            cell.Move(speed);
                        }
                        Return();
                        if (time >= endTime)
                        {
                            slotState = SlotState.Return;
                            returnOriginEvent?.Invoke();
                        }
                    }
                    */
                    break;
                case SlotState.Return:
                    if (!isItween)
                    {
                        //4-6结果变化
                        //是否允许出现相同的，不行的话最后展示结果的时候
                        if (Vector3.Distance(slotList[3].GetComponent<RectTransform>().anchoredPosition3D, tp[slotList[3].index]) < speed)
                        {
                            float value = Mathf.Abs(Vector3.Distance(slotList[3].GetComponent<RectTransform>().anchoredPosition3D, tp[slotList[3].index]));
                            for (int i = 0; i < slotList.Count; i++)
                            {
                                slotList[i].Move(value);
                            }
                            slotState = SlotState.End;
                        }
                        else
                        {
                            foreach (var cell in slotList)
                            {
                                cell.Move(speed);
                            }
                        }
                    }

                    break;
                case SlotState.End:
                    isStart = false;
                    isEnd = true;
                    time = 0;
                    moveCount = 0;
                    animationFinishTimes = 0;
                    tilePool.Reset();
                    FinishSlotEvent?.Invoke(this);
                    break;
            }

        }
    }

    void OnSpeedUp(ref float time)
    {
        speed = startAnimation.Evaluate(time);
        MoveCell(speed);
        Return();
        if (time >= startTime)
        {
            time -= startTime;
            slotState = SlotState.Transition;
        }
    }

    void OnTransition(ref float time)
    {
        MoveCell(speed);
        Return();
        if (time >= transitionTime && isStop)
        {
            time -= transitionTime;
            slotState = SlotState.SlowDown;
            tilePool.RemoveReward();
            slowDownEvent?.Invoke();
        }
    }

    void OnSlowdown(float time)
    {
        //减速开始将开奖结果移除循环表中，待即将展示时，放到屏幕外第一个位置
        if (moveCount < 3)
        {
            MoveCell(speed);
            Return();
        }
        else
        {
         
            slotState = SlotState.Return;
            foreach (var cell in slotList)
            {
                iTween.MoveTo(cell.gameObject, iTween.Hash("time", endTime, "position", tp[cell.index], "easetype", easetype, "oncomplete", "FinishAnimation", "oncompletetarget",gameObject, "islocal", false));
            }
        }

    }

    void MoveCell(float speed)
    {
        foreach (var cell in slotList)
        {
            cell.Move(speed);
        }
    }

    public List<SlotCell> slots = new List<SlotCell>();
    public bool isMove = true;
    public int moveCount;
    void Return()
    {
        slots = new List<SlotCell>();
        for (int i = 0; i < slotList.Count; i++)
        {
            if (!slotList[i].isReturn)
            {
                slots.Add(slotList[i]);
                slotList[i].index = (byte)(slots.Count - 1);
            }
        }
        if (slots.Count == 0) return;
        Vector3 pos = slots[slots.Count - 1].GetComponent<RectTransform>().anchoredPosition3D;
        for (int i = 0; i < slotList.Count; i++)
        {
            if (slotList[i].isReturn)
            {
                slots.Add(slotList[i]);
                slotList[i].index = (byte)(slots.Count - 1);

                slotList[i].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(pos.x, pos.y + ySpace + y, pos.z);
                pos = slots[slots.Count - 1].GetComponent<RectTransform>().anchoredPosition3D;
                if (slotState == SlotState.SlowDown)
                {
                    // IsMatchTile(slotList[i].tile);
                  //  Debug.Log("MOve!!!!!!!!!");
                    if (moveCount < 3)
                    {
                        slotList[i].Tile = tilePool.RewardTiles[moveCount++];
                    }
                    else
                    {
                        slotList[i].Tile = tilePool.GetTiles(slotList[i].Tile);
                    }
                }
                else
                {
                    slotList[i].Tile = tilePool.GetTiles(slotList[i].Tile);
                }

            }
        }
        slotList = slots;
    }

    bool IsMatchTile(byte index)
    {
        foreach (var i in cells)
        {
            if (i == index)
            {
                return true;
            }
        }
        return false;
    }


    void Replace()
    {

    }

    void FinishAnimation()
    {
        animationFinishTimes++;
        if (animationFinishTimes == slotList.Count)
        {
            slotState = SlotState.End;
            if (slotList[3].Tile != tilePool.RewardTiles[0] || slotList[4].Tile != tilePool.RewardTiles[1] || slotList[5].Tile != tilePool.RewardTiles[2])
            {
                Debug.LogError("转动异常!!!");
            }
        }
    }
}

public enum SlotState
{
    Default,
    SpeedUp,
    Transition,
    SlowDown,
    Return,
    End
}
