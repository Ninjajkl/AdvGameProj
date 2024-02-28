using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Constants;

public class GameManager : Singleton<GameManager>
{
    public PlayerController Player;
    public PlayerUIController PlayerUI;
    //public int[] Inventory;
    //Access each index with (int)MaterialEnum.Type

    public override void Awake()
    {
        base.Awake();
        //Changed, Player must set itself when spawned
        //Make sure Player is set. If not, find it and set it.
        //if (Player == null)
        //{
        //    Player = FindObjectOfType<PlayerController>();
        //}

        //Initialize the Inventory to have 0 of everything
        //Inventory = new int[Enum.GetValues(typeof(MaterialEnum)).Length];
        //for (int i = 0; i < Inventory.Length; i++)
        //{
        //    Inventory[i] = 0;
        //}
    }
}
