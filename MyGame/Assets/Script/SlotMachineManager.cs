using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotMachineManager : MonoBehaviorSingleton<SlotMachineManager>
{
    [SerializeField]
    private SlotMachineModel slotResultData=new SlotMachineModel();

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
        slotResultData.data.Add(new byte[] { 1, 3, 5 });
        slotResultData.data.Add(new byte[] { 1, 3, 5 });
        slotResultData.data.Add(new byte[] { 1, 3, 5 });
        slotResultData.data.Add(new byte[] { 1, 3, 5 });
        slotResultData.data.Add(new byte[] { 1, 3, 5 });
        EventDispatcher.TriggerEvent(SlotConst.Slot_RefreshUI, slotResultData);
    }
}
