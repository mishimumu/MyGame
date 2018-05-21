using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotMachineManager : MonoBehaviorSingleton<SlotMachineManager>
{
    [SerializeField]
    private SlotMachineModel slotResultData = new SlotMachineModel();

    //tile from 0 start
    public Sprite[] sprites;

    public SlotMachineManager()
    {
        EventDispatcher.AddEventListener(SlotConst.Slot_Start, StartSlot);
    }

    public override void Init()
    {

    }

    void StartSlot()
    {
        //test
        slotResultData.dataArr.Add(new byte[] { 1, 3, 5 });
        slotResultData.dataArr.Add(new byte[] { 2, 6, 5 });
        slotResultData.dataArr.Add(new byte[] { 7, 1, 9 });
        slotResultData.dataArr.Add(new byte[] { 1, 10, 2 });
        slotResultData.dataArr.Add(new byte[] { 3, 1, 6 });

        EventDispatcher.TriggerEvent(SlotConst.Slot_ReceiveResult, slotResultData);
    }
}
