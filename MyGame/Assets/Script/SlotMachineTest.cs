using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotMachineTest : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        List<SlotMachineInitModel> list = new List<SlotMachineInitModel>();
        list.Add(new SlotMachineInitModel(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, new byte[] { 1, 3, 5, 7, 9, 10 }));
        list.Add(new SlotMachineInitModel(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, new byte[] { 1, 3, 5, 7, 9, 10 }));
        list.Add(new SlotMachineInitModel(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, new byte[] { 2, 4, 6, 8, 9, 1 }));
        list.Add(new SlotMachineInitModel(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, new byte[] { 9, 2, 8, 7, 4, 3 }));
        list.Add(new SlotMachineInitModel(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, new byte[] { 7, 8, 1, 4, 9, 5 }));

        EventDispatcher.TriggerEvent(SlotConst.Slot_Init, list);
    }

}
