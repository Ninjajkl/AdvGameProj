using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static Constants;

public class GameManager : Singleton<GameManager>
{
    public PlayerController Player;
    public PlayerUIController PlayerUI;
    public RoomManager[] Rooms;
    public int ObjectsToRepair = 0;

    public override void Awake()
    {
        base.Awake();
        Rooms = new RoomManager[0];
    }

    public void AddRoom(RoomManager room)
    {
        Rooms = Rooms.Append(room).ToArray();
        foreach (Interactable interactable in room.interactableObjects)
        {
            if (interactable is UpgradeableObject upgradeableObject)
            {
                ObjectsToRepair++;
            }
            else if (interactable is Workbench workbench)
            {
                ObjectsToRepair += workbench.workbenchUpgrades.Length;
            }
        }
    }

    public void ReduceRepairsLeft()
    {
        ObjectsToRepair--;
        if(ObjectsToRepair == 0)
        {
            Debug.Log("All repairs complete!");
            //TODO - Add end sequence
        }
    }
}
