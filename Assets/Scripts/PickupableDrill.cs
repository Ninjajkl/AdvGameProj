using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static Constants;

public class PickupableDrill : MonoBehaviour
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
        //Set Drill Layer to Unselected by default
        Drill.layer = 6;
    }

    public void FixObject()
    {
        Drill.SetActive(false);
        //TODO, actually show drill for player

        /*
        if (!objectFixed && HaveEnoughMaterials())
        {
            objectFixed = true;
            fixedState.SetActive(true);
            brokenState.SetActive(false);

            gameManager.Player.ApplyUpgrade(upgradeType, upgradeLevel);
            gameManager.PlayerUI.HideUpgradableObjectMenu();
        }
        */
    }

    public void HoveredOver(bool hovering)
    {
        int newLayer;

        //Check if is being hovered over, then set appropriate layer
        if (hovering)
        {
            newLayer = 7;
            //gameManager.PlayerUI.ShowUpgradableObjectMenu(this);
        }
        else
        {
            newLayer = 6;
            //gameManager.PlayerUI.HideUpgradableObjectMenu();
        }
        if (Drill.layer != newLayer)
        {
            Drill.layer = newLayer;
        }
    }
}
