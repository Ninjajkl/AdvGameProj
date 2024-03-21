using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;
using static Refinery;

public class Refinery : Interactable
{
    [System.Serializable]
    public class RefineryRecipe
    {
        public int coalForRefined;
        public MaterialEnum rawMaterial;
        public int rawForRefined;
        public MaterialEnum refinedMaterial;
    }

    public RefineryRecipe[] refineryRecipies;
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

    public void Refine(RefineryRecipe refineryRecipe, int amountRefined)
    {
        (int amountRaw, int amountCoal) = TotalRawNeeded(refineryRecipe, amountRefined);
        if (HaveEnoughMaterials(refineryRecipe, amountRefined, amountCoal))
        {
            RemoveMaterialFromInventory(refineryRecipe.rawMaterial, amountRaw);
            RemoveMaterialFromInventory(MaterialEnum.Coal, amountCoal);
            AddMaterialToInventory(refineryRecipe.refinedMaterial, amountRefined);
        }
    }

    public bool HaveEnoughMaterials(RefineryRecipe refineryRecipe, int amountRaw, int amountCoal)
    {
        int inventoryRawMaterial = gameManager.Player.Inventory[(int)refineryRecipe.rawMaterial];
        int inventoryCoal = gameManager.Player.Inventory[(int)MaterialEnum.Coal];
        if (inventoryRawMaterial < amountRaw || inventoryCoal < amountCoal)
        {
            return false;
        }
        return true;
    }

    public (int, int) TotalRawNeeded(RefineryRecipe refineryRecipe, int amountRefined)
    {
        return (amountRefined * refineryRecipe.rawForRefined, amountRefined * refineryRecipe.coalForRefined);
    }

    public void RemoveMaterialFromInventory(MaterialEnum material, int amount)
    {
        gameManager.Player.Inventory[(int)material] -= amount;
        gameManager.PlayerUI.updateInventoryOnClick.Invoke();
    }

    public void AddMaterialToInventory(MaterialEnum material, int amount)
    {
        gameManager.Player.Inventory[(int)material] += amount;
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
