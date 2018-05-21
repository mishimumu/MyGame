using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotMachineLine : MonoBehaviour
{
    //line

    //divided into some stages
    //1.start slot,speed up to max speed and require data from server 
    //2.run some time by max speed
    //3.Replace the result with the list
    //4.move to the start potision by itween
    [SerializeField]
    private List<SlotCell> slotList;
    [SerializeField]
    private float startTime;
    [SerializeField]
    private float endTime;
    [SerializeField]
    private float transitionTime;
    private bool isStart;
    [SerializeField]
    private float width;
    [SerializeField]
    private float height;
    [SerializeField]
    private float xSpace;
    [SerializeField]
    private float ySpace;
    [SerializeField]
    private AnimationCurve startAnimation;
    [SerializeField]
    private SlotState slotState;
    public System.Action startSlotEvent;
    public System.Action slowDownEvent;
    public System.Action<SlotMachineLine> FinishSlotEvent;
    [SerializeField]
    private iTween.EaseType easetype;
    [SerializeField]
    private List<Vector3> targetPos = new List<Vector3>();
    [SerializeField]
    private List<GameObject> returnPos = new List<GameObject>();
    private SlotMachineTilePool tilePool;

    private int animationFinishTimes;
    private int moveCount;
    [SerializeField]
    private float time;
    private float speed;
    private bool isReceivedResult;//Check if the result is received 

    /*
     *              ---------
     *              *  5 
     *              *  4      <---Top display area        The initial desired position for rotation is [0,5], 2N
     *              *  3
     *              ---------
     *              *  2
     *              *  1      <---Display area             Less than height(-1) back to top
     *              *  0                                
     *              ---------
     *              *  -1
     *              *  -2      <----Lower display area      Reset position is [-3, 2], 2N. 
     *              *  -3
     *              --------
     *              reset :,
     *              0 -> -3   
     *              1 -> -2
     *              2 -> -1
     *              3 -> 0
     *              4 -> 1
     *              5 -> 2
     */


    private void Start()
    {
        foreach (var obj in returnPos)
        {
            targetPos.Add(obj.transform.position);
        }
    }


    public void InitLine(SlotMachineInitModel data)
    {
        if (data == null)
        {
            Debug.LogError("SlotMachineInitModel is null");
            return;
        }

        tilePool = new SlotMachineTilePool(data.allCards);
        for (int i = 0; i < slotList.Count; i++)
        {
            slotList[i].Tile = data.initCards[i];
            slotList[i].Init(-height);
        }
    }

    public void OnStart()
    {
        isStart = true;
        time = 0;
        moveCount = 0;
        animationFinishTimes = 0;
        tilePool.Reset();
        slotState = SlotState.SpeedUp;
        startSlotEvent?.Invoke();
    }

    public void OnReceived(byte[] result)
    {
        isReceivedResult = true;
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
                case SlotState.SpeedUp:
                    OnSpeedUp(ref time);
                    break;
                case SlotState.Transition:
                    OnTransition(ref time);
                    break;
                case SlotState.SlowDown:
                    OnSlowdown();
                    break;
                case SlotState.Return:
                    break;
                case SlotState.End:
                    isStart = false;
                    FinishSlotEvent?.Invoke(this);
                    slotState = SlotState.Default;
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
        if (time >= transitionTime && isReceivedResult)
        {
            time -= transitionTime;
            slotState = SlotState.SlowDown;
            tilePool.RemoveReward();
            slowDownEvent?.Invoke();
        }
    }

    void OnSlowdown()
    {
        // Move the result one by one to the list by three moves
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
                iTween.MoveTo(cell.gameObject, iTween.Hash("time", endTime, "position", targetPos[cell.Index], "easetype", easetype, "oncomplete", "FinishAnimation", "oncompletetarget", gameObject, "islocal", false));
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


    void Return()
    {
        List<SlotCell> slots = new List<SlotCell>();
        for (int i = 0; i < slotList.Count; i++)
        {
            if (!slotList[i].IsReturn)
            {
                slots.Add(slotList[i]);
                slotList[i].Index = (byte)(slots.Count - 1);
            }
        }
        if (slots.Count == 0) return;
        Vector3 pos = slots[slots.Count - 1].GetComponent<RectTransform>().anchoredPosition3D;
        for (int i = 0; i < slotList.Count; i++)
        {
            if (slotList[i].IsReturn)
            {
                slots.Add(slotList[i]);
                slotList[i].Index = (byte)(slots.Count - 1);
                slotList[i].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(pos.x, pos.y + ySpace + height, pos.z);
                if (slotState == SlotState.SlowDown && moveCount < 3)
                {
                    slotList[i].Tile = tilePool.RewardTiles[moveCount++];
                }
                else
                {
                    slotList[i].Tile = tilePool.GetTiles(slotList[i].Tile);
                }

            }
        }
        slotList = slots;
    }


    void FinishAnimation()
    {
        animationFinishTimes++;
        if (animationFinishTimes == slotList.Count)
        {
            slotState = SlotState.End;
            if (slotList[3].Tile != tilePool.RewardTiles[0] || slotList[4].Tile != tilePool.RewardTiles[1] || slotList[5].Tile != tilePool.RewardTiles[2])
            {
                Debug.LogError("roatate exception!!!");
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
