using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SlotMachineControl : MonoBehaviour
{
    //table
    [SerializeField]
    private SlotMachine[] slotMachineArr;
    [SerializeField]
    private float interval;
    [SerializeField]
    private Button btn;
    private bool isClick;
    // Use this for initialization

    private void Awake()
    {
        EventDispatcher.AddEventListener<SlotMachineModel>(SlotConst.Slot_ReceiveResult, OnReceivedResult);
        EventDispatcher.AddEventListener<List<SlotMachineInitModel>>(SlotConst.Slot_Init, InitTable);
    }


    void OnReceivedResult(SlotMachineModel mo)
    {
        if (mo == null) return;

        for (int i = 0; i < slotMachineArr.Length; i++)
        {
            slotMachineArr[i].OnReceived(mo.dataArr[i]);
        }
    }

    void InitTable(List<SlotMachineInitModel> datas)
    {
        int length = slotMachineArr.Length;

        if (datas == null || length != datas.Count) return;

        for (int i = 0; i < length; i++)
        {
            slotMachineArr[i].InitLine(datas[i]);
        }

        btn.onClick.AddListener(OnPlay);

        foreach (var slot in slotMachineArr)
        {
            slot.FinishSlotEvent += OnFinishSlot;
        }
    }
    void OnPlay()
    {
        if (isClick) return;
        isClick = true;

        //require data ,then rotate slotmachine
        EventDispatcher.TriggerEvent(SlotConst.Slot_Start);
        StartCoroutine(StartPlay());
    }

    IEnumerator StartPlay()
    {
        foreach (var slot in slotMachineArr)
        {
            slot.OnStart(); 
            yield return new WaitForSeconds(interval);
        }
    }


    void OnFinishSlot(SlotMachine slot)
    {
        isClick = false;
    }
}
