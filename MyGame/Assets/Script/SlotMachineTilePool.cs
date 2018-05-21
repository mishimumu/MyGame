using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotMachineTilePool 
{

    private byte[] tiles;

    private byte[] rewardTiles;

    private List<byte> enableTiles;

    public byte[] RewardTiles { get; private set; }

    public SlotMachineTilePool(byte[] tiles)
    {
        this.tiles = tiles;
    }

    public void Reward(byte[] tiles)
    {
        RewardTiles = tiles;
    }

    public void RemoveReward()
    {
        //remove reward tile
        foreach (var tile in RewardTiles)
        {
            if (enableTiles.Contains(tile))
            {
                enableTiles.Remove(tile);
            }
        }
    }

    public void Reset()
    {
        enableTiles = new List<byte>(tiles);
    }

    public byte GetTiles(byte tile)
    {
        int random = Random.Range(0, enableTiles.Count);
        byte newTile = enableTiles[random];
        enableTiles.Remove(newTile);
        enableTiles.Add(tile);
        return newTile;
    }

  
}
