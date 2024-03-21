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
        SetLayerRecursively(transform, 6);
    }

    public void Refine(RefineryRecipe refineryRecipe, int amountRefined)
    {
        int amountRaw = TotalRawNeeded(refineryRecipe, amountRefined);
        if (HaveEnoughMaterials(refineryRecipe, amountRefined))
        {
            RemoveRawFromInventory(refineryRecipe, amountRaw);
            AddRefinedToInventory(refineryRecipe, amountRefined);
        }
    }

    public bool HaveEnoughMaterials(RefineryRecipe refineryRecipe, int amountRaw)
    {
        if (gameManager.Player.Inventory[(int)refineryRecipe.rawMaterial] < amountRaw)
        {
            return false;
        }
        return true;
    }

    public int TotalRawNeeded(RefineryRecipe refineryRecipe, int amountRefined)
    {
        return amountRefined * refineryRecipe.rawForRefined;
    }

    public void RemoveRawFromInventory(RefineryRecipe refineryRecipe, int amountRaw)
    {
        gameManager.Player.Inventory[(int)refineryRecipe.rawMaterial] -= amountRaw;
        gameManager.PlayerUI.updateInventoryOnClick.Invoke();
    }

    public void AddRefinedToInventory(RefineryRecipe refineryRecipe, int amountRefined)
    {
        gameManager.Player.Inventory[(int)refineryRecipe.refinedMaterial] += amountRefined;
        gameManager.PlayerUI.updateInventoryOnClick.Invoke();
    }

    public override void HoveredOver(bool hovering)
    {
        //Right now, do nothing
        return;
    }
}
