using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SlotMachineControl : MonoBehaviour
{

    public SlotMachine[] slotMachineArr;
    public int[,] resultArr;
    public float interval;
    public Button btn;
    public Text desc;
    public int value;
    private SlotMachineModel slotMachineModel;
    private bool isClick;
    // Use this for initialization
    void Start()
    {
        EventDispatcher.AddEventListener<SlotMachineModel>(SlotConst.Slot_RefreshUI, Refresh);
        EventDispatcher.AddEventListener<List<SlotMachineInitModel>>(SlotConst.Slot_Init, Init);

        btn.onClick.AddListener(OnPlay);
        foreach (var slot in slotMachineArr)
        {
            slot.startSlotEvent += StartEvent;
            slot.FinishSlotEvent += OnFinishSlot;
        }

        List<SlotMachineInitModel> datas = new List<SlotMachineInitModel>();
        datas.Add(new SlotMachineInitModel(new byte[] { 1, 2, 3, 4, 5, 6, 7, 9, 10 }, new byte[] { 2, 4, 6, 8, 3, 2 }));
        datas.Add(new SlotMachineInitModel(new byte[] { 1, 2, 3, 4, 5, 6, 7, 9, 10 }, new byte[] { 7, 4, 6, 8, 3, 2 }));
        datas.Add(new SlotMachineInitModel(new byte[] { 1, 2, 3, 4, 5, 6, 7, 9, 10 }, new byte[] { 9, 4, 6, 8, 3, 2 }));
        datas.Add(new SlotMachineInitModel(new byte[] { 1, 2, 3, 4, 5, 6, 7, 9, 10 }, new byte[] { 1, 4, 6, 8, 3, 2 }));
        datas.Add(new SlotMachineInitModel(new byte[] { 1, 2, 3, 4, 5, 6, 7, 9, 10 }, new byte[] { 7, 4, 10, 8, 3, 2 }));

        EventDispatcher.TriggerEvent<List<SlotMachineInitModel>>(SlotConst.Slot_Init, datas);
    }

    void Refresh(SlotMachineModel data)
    {
        slotMachineModel = data;
        for (int i = 0; i < slotMachineArr.Length; i++)
        {
            slotMachineArr[i].Choose(slotMachineModel.data[i]);
        }

    }

    void Init(List<SlotMachineInitModel> datas)
    {
        int length = slotMachineArr.Length;

        if (datas == null || length != datas.Count) return;

        for (int i = 0; i < length; i++)
        {
            slotMachineArr[i].cells = datas[i].allCards;
            slotMachineArr[i].initCells = datas[i].initCards;
            slotMachineArr[i].Init();
        }
    }

    // Update is called once per frame
    void Update()
    {
        value++;
        desc.text = string.Format("状态：{0}\n速度:{1}\n时间:{2}\n帧数:{3}", slotMachineArr[0].slotState, slotMachineArr[0].speed, slotMachineArr[0].time, value);
    }

    void OnPlay()
    {
        //if (isClick) return;
        //isClick = true;

        //require data ,then rotate slotmachine
        EventDispatcher.TriggerEvent(SlotConst.Slot_Start);
        StartCoroutine(StartPlay());
        value = 0;
    }

    IEnumerator StartPlay()
    {
        foreach (var slot in slotMachineArr)
        {
            slot.isStart = true;
            slot.slotState = SlotState.SpeedUp;
            slot.startSlotEvent?.Invoke();
            yield return new WaitForSeconds(interval);
        }
    }

    void StartEvent()
    {
        foreach (var slot in slotMachineArr)
        {
            slot.isStop = true;
        }
    }

    void OnFinishSlot(SlotMachine slot)
    {
        slot.slotState = SlotState.Default;
    }
}
