using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    [Header("Upgradable Object UI Variables")]
    public  GameObject UpgradeableObjectUI;
    [SerializeField]
    private RectTransform UpgradeableObjectUITransform;
    [SerializeField]
    private TMP_Text objectName;
    [SerializeField]
    private TMP_Text upgradeText;
    [SerializeField]
    private TMP_Text materialsList;
    [SerializeField]
    private Button upgradeButton;
    //On-Screen Point
    private Vector2 UpgradeObjectUIDisplayPointA = new Vector2(-200,0f);
    //Off-Screen Point
    private Vector2 UpgradeObjectUIDisplayPointB = new Vector2(200,0f);

    [Header("Instructional Text Variables")]
    public TMP_Text FlashlightPrompt;
    public TMP_Text DrillPrompt;

    #region Upgradable Object UI Functions

    public void ShowUpgradableObjectMenu(UpgradeableObject highlightedObject) {
        UpgradeableObjectUI.gameObject.SetActive(true);
        GetUpgradeableObjectInfo(highlightedObject);
        if(!highlightedObject.HaveEnoughMaterials()) {
            upgradeButton.interactable = false;
        } else {
            upgradeButton.interactable = true;
        }
        
        // Menu Appearance Slide
        UpgradeableObjectUITransform.anchoredPosition = Vector2.Lerp(UpgradeableObjectUITransform.anchoredPosition, UpgradeObjectUIDisplayPointA, Time.deltaTime*3f);
    }

    public void HideUpgradableObjectMenu() {
        UpgradeableObjectUI.SetActive(false);
        UpgradeableObjectUITransform.anchoredPosition = UpgradeObjectUIDisplayPointB;
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
}
