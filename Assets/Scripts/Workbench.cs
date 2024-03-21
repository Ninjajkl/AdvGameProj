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
        SetLayerRecursively(transform, 6);
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
        //Right now, do nothing
        return;
    }
}
