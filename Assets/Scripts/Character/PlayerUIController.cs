using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Constants;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    public AudioManager playerHUDAudioManager;
    [SerializeField]
    private ArmScript armScript;
    [SerializeField]
    private LevelManager levelManager;

    [Header("Upgradable Object UI Variables")]
    public GameObject upgradeableObjectUI;
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
    private Vector2 upgradeObjectUIDisplayPointA = new Vector2(-200, 0f);
    //Off-Screen Point
    private Vector2 upgradeObjectUIDisplayPointB = new Vector2(200, 0f);

    [Header("Instructional Text Variables")]
    public TMP_Text flashlightPrompt;
    public TMP_Text drillPrompt;

    [Header("Inventory UI Variables")]
    // Holds references to all useable sprites
    public List<Sprite> materialSprites;
    public GameObject inventoryUI;
    public GameObject inventorySlotPrefab;
    public List<InventorySlotManager> inventorySlotManagers; // For Big Inventory
    public Transform slotGridGroup;

    [Header("Side Inventory UI Variables")]
    public GameObject sideInventoryUI;
    public Transform sideSlotGridGroup;
    public GameObject miniInventorySlotPrefab;
    public List<InventorySlotManager> miniInventorySlotManagers; // For Side Inventory
    public UnityEvent updateInventoryOnClick;

    [Header("Refinery IU Variables")]
    public GameObject refineryUI;
    public Transform refineryGridGroup;
    public Refinery refineryObject;
    public GameObject refinerySlotPrefab;
    public List<RefinerySlot> refinerySlots;
    public UnityEvent updateRefineryUI;

    [Header("Pause UI Variables")]
    public GameObject pauseUI;
    public GameObject settingsUI;

    private void Start()
    {
        GameManager.Instance.PlayerUI = this;
        InitializeInventorySystem();
        InitializeRefinerySystem();
    }

    #region Upgradable Object UI Functions

    public void ShowUpgradableObjectMenu(UpgradeableObject highlightedObject)
    {
        upgradeableObjectUI.gameObject.SetActive(true);
        GetUpgradeableObjectInfo(highlightedObject);
        if (!highlightedObject.HaveEnoughMaterials())
        {
            upgradeButton.interactable = false;
        }
        else
        {
            upgradeButton.interactable = true;
        }

        // Menu Appearance Slide
        upgradeableObjectUITransform.anchoredPosition = Vector2.Lerp(upgradeableObjectUITransform.anchoredPosition, upgradeObjectUIDisplayPointA, Time.deltaTime * 3f);
    }

    public void HideUpgradableObjectMenu()
    {
        upgradeableObjectUI.SetActive(false);
        upgradeableObjectUITransform.anchoredPosition = upgradeObjectUIDisplayPointB;
    }

    public void GetUpgradeableObjectInfo(UpgradeableObject highlightedObject)
    {
        objectName.text = highlightedObject.gameObject.name;
        upgradeText.text = $"+{highlightedObject.upgradeLevel} {highlightedObject.upgradeType}";

        if (highlightedObject.neededMaterials.Length == 0)
        {
            materialsList.text = "None";
        }
        else
        {
            materialsList.text = "";
            for (int i = 0; i < highlightedObject.neededMaterials.Length; i++)
            {
                materialsList.text += $"{highlightedObject.neededMaterials[i].amount} {highlightedObject.neededMaterials[i].materialType}\n";
            }
        }
    }

    #endregion

    #region Instructional Prompts UI Functions

    public void DisplayFlashlightPrompt(PlayerController playerController)
    {
        if (!playerController.flashlight.enabled)
        {
            flashlightPrompt.text = "Press 'V' to Turn On Flashlight";
        }
        else
        {
            flashlightPrompt.text = "Press 'V' to Turn Off Flashlight";
        }
    }

    #endregion

    #region Inventory UI Functions

    public void InitializeInventorySystem()
    {
        for (int i = 0; i < playerController.Inventory.Length; i++)
        {
            InventorySlotManager inventorySlot = gameObject.AddComponent<InventorySlotManager>();
            inventorySlotManagers.Add(inventorySlot);
        }

        for (int i = 0; i < 7; i++)
        {
            InventorySlotManager miniInventorySlot = gameObject.AddComponent<InventorySlotManager>();
            miniInventorySlotManagers.Add(miniInventorySlot);
        }

        updateInventoryOnClick.AddListener(UpdateInventoryDynamic);
    }

    public void ShowInventory()
    {
        if (playerController.inventoryMenuOn == false)
        {
            inventoryUI.SetActive(true);
            sideInventoryUI.SetActive(false);
        }
        else
        {
            inventoryUI.SetActive(false);
            sideInventoryUI.SetActive(true);
        }
    }

    public void UpdateInventoryDynamic()
    {
        // Updates the Inventory Slots of the first 7 materials
        for (int i = 0; i < 7; i++)
        {
            if (playerController.Inventory[i] > 0)
            {
                if (miniInventorySlotManagers[i].instianiated == false)
                {
                    GameObject inventorySlot = Instantiate(miniInventorySlotPrefab, sideSlotGridGroup);
                    miniInventorySlotManagers[i] = inventorySlot.GetComponent<InventorySlotManager>();
                    miniInventorySlotManagers[i].slotQuantity.text = $"{playerController.Inventory[i]}";
                    miniInventorySlotManagers[i].slotIcon.sprite = materialSprites[i];
                    miniInventorySlotManagers[i].instianiated = true;
                }
                miniInventorySlotManagers[i].slotQuantity.text = $"{playerController.Inventory[i]}";
            }
        }

        // Updates for big inventory menu
        for (int i = 0; i < playerController.Inventory.Length; i++)
        {
            if (playerController.Inventory[i] > 0)
            {
                if (inventorySlotManagers[i].instianiated == false)
                {
                    GameObject inventorySlot = Instantiate(inventorySlotPrefab, slotGridGroup);
                    inventorySlotManagers[i] = inventorySlot.GetComponent<InventorySlotManager>();
                    inventorySlotManagers[i].slotName.text = $"{ConvertIntToMaterialEnum(i)}";
                    inventorySlotManagers[i].slotQuantity.text = $"{playerController.Inventory[i]}";
                    inventorySlotManagers[i].slotIcon.sprite = materialSprites[i];
                    inventorySlotManagers[i].instianiated = true;
                }
                inventorySlotManagers[i].slotQuantity.text = $"{playerController.Inventory[i]}";
            }
        }
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

    #region Refinery UI Functions

    public void InitializeRefinerySystem()
    {
        for (int i = 0; i < refineryObject.refineryRecipies.Length; i++)
        {
            RefinerySlot refinerySlot = gameObject.AddComponent<RefinerySlot>();
            refinerySlot.index = i;
            refinerySlots.Add(refinerySlot);
        }

        updateRefineryUI.AddListener(UpdateRefineryUIDynamic);
    }

    public void ShowRefineryMenu()
    {
        refineryUI.SetActive(true);
        playerController.enabled = false;
        armScript.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        updateRefineryUI.Invoke();
    }

    public void UpdateRefineryUIDynamic()
    {
        for (int i = 0; i < refineryObject.refineryRecipies.Length; i++)
        {
            int index = i;
            if (refinerySlots[i].instianiated == false)
            {
                GameObject refineryslot = Instantiate(refinerySlotPrefab, refineryGridGroup);
                refinerySlots[i] = refineryslot.GetComponent<RefinerySlot>();
                refinerySlots[i].refinedQuantityInputField = refineryslot.GetComponentInChildren<TMP_InputField>();
                refinerySlots[i].refinedQuantityInputField.onValueChanged.AddListener(inputNum => OnInputChanged(inputNum, index));
                refinerySlots[i].refinedQuantityInputField.onValueChanged.AddListener(inputNum => playerHUDAudioManager.Play("InputChanged"));

                string refinedMaterialText = $"{refineryObject.refineryRecipies[i].refinedMaterial}";
                refinedMaterialText = refinedMaterialText.Replace("_", " ");
                refinerySlots[i].refinedMaterialText.text = $"{refinedMaterialText}";
                refinerySlots[i].neededCoalText.text = $"{refineryObject.refineryRecipies[i].coalForRefined} Coal";
                refinerySlots[i].neededMaterialText.text = $"{refineryObject.refineryRecipies[i].rawForRefined} {refineryObject.refineryRecipies[i].rawMaterial}";
                refinerySlots[i].refinedMaterialQuantityText.text = $"1 {refineryObject.refineryRecipies[i].refinedMaterial}";
                refinerySlots[i].neededMaterialImage.sprite = materialSprites[(int)refineryObject.refineryRecipies[i].rawMaterial];
                refinerySlots[i].refinedMaterialImage.sprite = materialSprites[(int)refineryObject.refineryRecipies[i].refinedMaterial];
                refinerySlots[i].instianiated = true;
            }
            int multiple = int.Parse(refinerySlots[i].refinedQuantityInputField.text);

            refinerySlots[i].refineButton.onClick.RemoveAllListeners();
            refinerySlots[i].refineButton.onClick.AddListener(() => refineryObject.Refine(refineryObject.refineryRecipies[index], multiple));
            refinerySlots[i].refineButton.onClick.AddListener(() => playerHUDAudioManager.Play("Click"));

            string instanRefinedMaterialText = $"{refineryObject.refineryRecipies[i].refinedMaterial}";
            instanRefinedMaterialText = instanRefinedMaterialText.Replace("_", " ");
            refinerySlots[i].neededCoalText.text = $"{refineryObject.refineryRecipies[i].coalForRefined * multiple} Coal";
            refinerySlots[i].neededMaterialText.text = $"{refineryObject.refineryRecipies[i].rawForRefined * multiple} {refineryObject.refineryRecipies[i].rawMaterial}";
            refinerySlots[i].refinedMaterialQuantityText.text = $"{multiple} {instanRefinedMaterialText}";

            if (!refineryObject.HaveEnoughMaterials(refineryObject.refineryRecipies[i], refineryObject.refineryRecipies[i].rawForRefined * multiple, refineryObject.refineryRecipies[i].coalForRefined * multiple))
            {
                refinerySlots[i].refineButton.interactable = false;
            }
            else
            {
                refinerySlots[i].refineButton.interactable = true;
            }
        }
    }

    public void OnInputChanged(string inputNum, int index)
    {
        //Debug.Log($"Num = {inputNum}");
        if (string.IsNullOrEmpty(inputNum) || inputNum == "-")
        {
            //Debug.Log($"Input Invalid!");
            refinerySlots[index].refinedQuantityInputField.text = "1";
            refinerySlots[index].refinedQuantityInputField.textComponent.text = "1";
        }
        else
        {
            int num = int.Parse(inputNum);
            if (num < 1)
            {
                //Debug.Log($"Index: {index}");
                refinerySlots[index].refinedQuantityInputField.text = "1";
                refinerySlots[index].refinedQuantityInputField.textComponent.text = "1";
            }
        }
        updateRefineryUI.Invoke();
    }

    public void HideRefineryMenu()
    {
        refineryUI.SetActive(false);
        playerController.enabled = true;
        armScript.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    #endregion

    #region Pause UI Functions

    public void ShowPauseMenu()
    {
        pauseUI.SetActive(true);
        playerController.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        pauseUI.SetActive(false);
        playerController.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;
    }

    public void ShowSettingsMenu()
    {
        pauseUI.SetActive(false);
        settingsUI.SetActive(true);
    }

    public void HideSettingsMenu()
    {
        pauseUI.SetActive(true);
        settingsUI.SetActive(false);
    }

    public void GotoMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    #endregion
}
