using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Constants;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;

    [Header("Upgradable Object UI Variables")]
    public  GameObject upgradeableObjectUI;
    [SerializeField]
    private RectTransform upgradeableObjectUITransform;
    [SerializeField]
    private TMP_Text objectName;
    [SerializeField]
    private TMP_Text upgradeText;
    [SerializeField]
    private TMP_Text materialsList;
    [SerializeField]
    private Button upgradeButton;
    //On-Screen Point
    private Vector2 upgradeObjectUIDisplayPointA = new Vector2(-200,0f);
    //Off-Screen Point
    private Vector2 upgradeObjectUIDisplayPointB = new Vector2(200,0f);

    [Header("Instructional Text Variables")]
    public TMP_Text flashlightPrompt;
    public TMP_Text drillPrompt;

    [Header("Inventory UI Variables")]
    public GameObject inventoryUI;
    public GameObject inventorySlotPrefab;
    public InventorySlotManager inventorySlotManager;
    public Transform slotGridGroup;

    #region Upgradable Object UI Functions

    public void ShowUpgradableObjectMenu(UpgradeableObject highlightedObject) {
        upgradeableObjectUI.gameObject.SetActive(true);
        GetUpgradeableObjectInfo(highlightedObject);
        if(!highlightedObject.HaveEnoughMaterials()) {
            upgradeButton.interactable = false;
        } else {
            upgradeButton.interactable = true;
        }
        
        // Menu Appearance Slide
        upgradeableObjectUITransform.anchoredPosition = Vector2.Lerp(upgradeableObjectUITransform.anchoredPosition, upgradeObjectUIDisplayPointA, Time.deltaTime*3f);
    }

    public void HideUpgradableObjectMenu() {
        upgradeableObjectUI.SetActive(false);
        upgradeableObjectUITransform.anchoredPosition = upgradeObjectUIDisplayPointB;
    }

    public void GetUpgradeableObjectInfo(UpgradeableObject highlightedObject) {
        objectName.text = highlightedObject.gameObject.name;
        upgradeText.text = $"+{highlightedObject.upgradeLevel} {highlightedObject.upgradeType}";

        if(highlightedObject.neededMaterials.Length == 0) {
            materialsList.text = "None";
        } else {
            materialsList.text = "";
            for(int i = 0; i < highlightedObject.neededMaterials.Length; i++) {
                materialsList.text += $"{highlightedObject.neededMaterials[i].amount} {highlightedObject.neededMaterials[i].materialType}\n";
            }
        }
    }

    #endregion

    #region Instructional Prompts UI Functions

    public void DisplayFlashlightPrompt(PlayerController playerController) {
        if(!playerController.flashlight.enabled) {
                flashlightPrompt.text = "Press 'V' to Turn On Flashlight";
            } else {
                flashlightPrompt.text = "Press 'V' to Turn Off Flashlight";
            }
    }

    #endregion

    #region Inventory UI Functions

    public void ShowInventory() {
        //if(playerController.inventoryMenuOn == false) {
            inventoryUI.SetActive(true);

            // Reset all inventory slots
            foreach(Transform child in slotGridGroup) {
                Destroy(child.gameObject);
            }

            // Add slots with existing material into inventory
            for(int i = 0; i < playerController.Inventory.Length; i++) {
                if(playerController.Inventory[i] > 0) {
                    //GameObject inventorySlot = Instantiate(inventorySlotPrefab, slotGridGroup);
                    Instantiate(inventorySlotPrefab, slotGridGroup);
                    //InventorySlotManager inventorySlotManager = inventorySlot.GetComponent<InventorySlotManager>();
                    inventorySlotManager.slotName.text = $"{ConvertIntToMaterialEnum(i)}";
                    inventorySlotManager.slotQuantity.text = $"{playerController.Inventory[i]}";
                }
            }
            /*
        } else {
            inventoryUI.SetActive(false);
        }*/
    }

    public MaterialEnum ConvertIntToMaterialEnum(int enumValue)
    {
        if (Enum.IsDefined(typeof(MaterialEnum), enumValue))
        {
            return (MaterialEnum)Enum.ToObject(typeof(MaterialEnum), enumValue);
        }
        else
        {
            Debug.LogError("Invalid enum value");
            return MaterialEnum.Stone; // Or any default value you prefer
        }
    }

    #endregion
}
