using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotMachineInitModel
{
    public byte[] allCards;
    public byte[] initCards;

    public SlotMachineInitModel(byte[] allCards, byte[] initCards)
    {
        this.allCards = allCards;
        this.initCards = initCards;
    }
}
