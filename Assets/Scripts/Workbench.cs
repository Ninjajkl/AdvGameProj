using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

public class Workbench : Interactable
{
    [System.Serializable]
    public class NeededMaterials
    {
        public MaterialEnum materialType;
        public int amount;
    }

    [System.Serializable]
    public class WorkbenchUpgrade
    {
        public UpgradeType upgradeType;
        public int upgradeLevel;
        public NeededMaterials[] neededMaterials;
    }

    public WorkbenchUpgrade[] workbenchUpgrades;
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

    public void Fix(WorkbenchUpgrade workbenchUpgrade)
    {
        if (HaveEnoughMaterials(workbenchUpgrade))
        {
            RemoveMaterialsFromInventory(workbenchUpgrade);
            gameManager.Player.ApplyUpgrade(workbenchUpgrade.upgradeType, workbenchUpgrade.upgradeLevel);
            gameManager.PlayerUI.HideUpgradableObjectMenu();
        }
    }

    public bool HaveEnoughMaterials(WorkbenchUpgrade workbenchUpgrade)
    {
        foreach (var material in workbenchUpgrade.neededMaterials)
        {
            if (gameManager.Player.Inventory[(int)material.materialType] < material.amount)
            {
                return false;
            }
        }
        return true;
    }

    public void RemoveMaterialsFromInventory(WorkbenchUpgrade workbenchUpgrade)
    {
        foreach (var material in workbenchUpgrade.neededMaterials)
        {
            gameManager.Player.Inventory[(int)material.materialType] -= material.amount;
        }
        gameManager.PlayerUI.updateInventoryOnClick.Invoke();
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
