using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static Constants;

public class UpgradeableObject : MonoBehaviour
{
    [System.Serializable]
    public class NeededMaterials
    {
        public MaterialEnum materialType;
        public int amount;
    }

    //[SerializeField]
    public UpgradeType upgradeType;
    //[SerializeField]
    public int upgradeLevel;
    //[SerializeField]
    public NeededMaterials[] neededMaterials;
    [SerializeField]
    private GameObject brokenState;
    [SerializeField]
    private GameObject fixedState;

    private bool objectFixed = false;
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
        //On start make sure only the brokenState is active
        brokenState.SetActive(true);
        fixedState.SetActive(false);
        //Set brokenState Layer to Unselected by default
        brokenState.layer = 6;
    }

    public void FixObject()
    {
        if (!objectFixed && HaveEnoughMaterials())
        {
            objectFixed = true;
            fixedState.SetActive(true);
            brokenState.SetActive(false);

            gameManager.Player.ApplyUpgrade(upgradeType, upgradeLevel);
            gameManager.PlayerUI.HideUpgradableObjectMenu();
        }
    }
    
    public bool HaveEnoughMaterials()
    {
        foreach(var material in neededMaterials) 
        {
            if (gameManager.Inventory[(int)material.materialType] < material.amount)
            {
                return false;
            }
        }
        return true;
    }

    public void HoveredOver(bool hovering)
    {
        //Do nothing if object fixed
        if (objectFixed)
        {
            return;
        }
        int newLayer;

        //Check if is being hovered over 
        if (hovering)
        {
            //Check if the player has enough resources to upgrade
            newLayer = HaveEnoughMaterials() ? 7 : 8;

            gameManager.PlayerUI.ShowUpgradableObjectMenu(this);
        }
        else
        {
            newLayer = 6;
            gameManager.PlayerUI.HideUpgradableObjectMenu();
        }
        if(brokenState.layer != newLayer)
        {
            brokenState.layer = newLayer;
        }
    }
}
