using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static Constants;

public class PickupableDrill : Interactable
{
    [SerializeField]
    private GameObject Drill;

    private GameManager gameManager;

    /*
     * Some gross hard-coded layer values
     * 0 - Default
     * 6 - OutlineUnselected
     * 7 - OutlineUpgradeable
     * 8 - OutlineNonUpgradeable
     */

    void Awake()
    {
        gameManager = GameManager.Instance;
    }

    private void Start()
    {
        HoveredOver(false);
    }

    public void PickupDrill()
    {
        Drill.SetActive(false);
        //TODO, actually show drill for player
        //Should also remove this from the game as no longer needed
    }

    public override void HoveredOver(bool hovering)
    {
        int newLayer = 0;
        //if too far from player, turn off outline
        if (!nearPlayer)
        {
            newLayer = 0;
        }
        //Otherwise, turn on unselected outline
        else
        {
            newLayer = 6;
        }
        if (transform.gameObject.layer != newLayer)
        {
            SetLayerRecursively(transform, newLayer);
        }
    }
}
