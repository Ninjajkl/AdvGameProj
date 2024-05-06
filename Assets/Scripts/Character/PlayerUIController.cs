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
    private Vector2 upgradeObjectUIDisplayPointA = new Vector2(-50, 0f);
    //Off-Screen Point
    private Vector2 upgradeObjectUIDisplayPointB = new Vector2(200, 0f);

    [Header("Instructional Text Variables")]
    public TMP_Text flashlightPrompt;
    public TMP_Text drillPrompt;

    [Header("Inventory UI Variables")]
    // Holds references to all useable sprites
    public List<Sprite> materialSprites;
    public GameObject inventorySlotPrefab;
    public Transform sideSlotGridGroup;
    public List<InventorySlotManager> inventorySlotManagers;
    public UnityEvent updateInventoryOnClick;

    [Header("Refinery UI Variables")]
    public GameObject refineryUI;
    public Transform refineryGridGroup;
    public Refinery refineryObject;
    public GameObject refinerySlotPrefab;
    public List<RefinerySlot> refinerySlots;
    public UnityEvent updateRefineryUI;

    [Header("Workbench UI Variables")]
    public List<Sprite> drillSprites;
    public GameObject workbenchUI;
    public Workbench workbenchObject;
    public WorkbenchSlot workbenchSlot;
    public UnityEvent updateWorkbenchUI;

    [Header("Ending UI Variables")]
    public GameObject endingPromptUI;
    public RoomManager mineEntranceRoomManager;
    public Interactable endWall;

    [Header("Pause UI Variables")]
    public GameObject pauseUI;
    public GameObject settingsUI;

    private void Start()
    {
        GameManager.Instance.PlayerUI = this;
        InitializeInventorySystem();
        InitializeRefinerySystem();
        InitializeWorkbenchSystem();
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
        string objName = highlightedObject.gameObject.name;
        objName = objName.Replace("_", " ");
        objectName.text = objName;
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
                string matName = $"{highlightedObject.neededMaterials[i].materialType}";
                matName = matName.Replace("_", " ");
                materialsList.text += $"{highlightedObject.neededMaterials[i].amount} {matName}\n";
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
        updateInventoryOnClick.AddListener(UpdateInventoryDynamic);
    }

    public void UpdateInventoryDynamic()
    {
        // Updates the Inventory Slots of the first 7 materials
        for (int i = 0; i < playerController.Inventory.Length; i++)
        {
            if (inventorySlotManagers[i].instianiated == false)
            {
                if (playerController.Inventory[i] > 0)
                {
                    GameObject inventorySlot = Instantiate(inventorySlotPrefab, sideSlotGridGroup);
                    inventorySlotManagers[i] = inventorySlot.GetComponent<InventorySlotManager>();
                    string slotName = $"{ConvertIntToMaterialEnum(i)}";
                    slotName = slotName.Replace("_", " ");
                    inventorySlotManagers[i].slotName.text = slotName;
                    inventorySlotManagers[i].slotQuantity.text = $"{playerController.Inventory[i]}";
                    inventorySlotManagers[i].slotIcon.sprite = materialSprites[i];
                    inventorySlotManagers[i].instianiated = true;
                }
            }
            else
            {
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
            refinerySlots[i].refineButton.onClick.AddListener(() => playerHUDAudioManager.Play("RefineButton"));

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

    public void ShowRefineryMenu()
    {
        refineryUI.SetActive(true);
        playerController.enabled = false;
        playerController.StopSounds();
        armScript.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        updateRefineryUI.Invoke();
    }

    public void HideRefineryMenu()
    {
        refineryUI.SetActive(false);
        playerController.enabled = true;
        playerController.EnableSounds();
        armScript.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    #endregion

    #region Workbench UI Functions

    public void InitializeWorkbenchSystem()
    {
        workbenchSlot.index = 0;
        workbenchSlot.drillText.text = "Fixed Drill";
        workbenchSlot.drillImage.sprite = drillSprites[0];
        workbenchSlot.neededMaterialImage1.sprite = materialSprites[(int)workbenchObject.workbenchUpgrades[0].neededMaterials[0].materialType];
        workbenchSlot.neededMaterialImage2.sprite = materialSprites[(int)workbenchObject.workbenchUpgrades[0].neededMaterials[1].materialType];
        workbenchSlot.neededMaterialImage3.sprite = materialSprites[(int)workbenchObject.workbenchUpgrades[0].neededMaterials[2].materialType];

        string neededMaterialText1 = $"{workbenchObject.workbenchUpgrades[0].neededMaterials[0].materialType}";
        neededMaterialText1 = neededMaterialText1.Replace("_", " ");
        string neededMaterialText2 = $"{workbenchObject.workbenchUpgrades[0].neededMaterials[0].materialType}";
        neededMaterialText2 = neededMaterialText2.Replace("_", " ");
        string neededMaterialText3 = $"{workbenchObject.workbenchUpgrades[0].neededMaterials[0].materialType}";
        neededMaterialText3 = neededMaterialText3.Replace("_", " ");
        workbenchSlot.neededMaterialText1.text = $"{workbenchObject.workbenchUpgrades[0].neededMaterials[0].amount} {neededMaterialText1}";
        workbenchSlot.neededMaterialText2.text = $"{workbenchObject.workbenchUpgrades[0].neededMaterials[1].amount} {neededMaterialText2}";
        workbenchSlot.neededMaterialText3.text = $"{workbenchObject.workbenchUpgrades[0].neededMaterials[2].amount} {neededMaterialText3}";

        workbenchSlot.upgradeButton.onClick.AddListener(() => workbenchObject.UpgradeDrill(workbenchObject.workbenchUpgrades[workbenchSlot.index]));
        workbenchSlot.upgradeButton.onClick.AddListener(() => playerHUDAudioManager.Play("UpgradeButton"));
        updateWorkbenchUI.AddListener(UpdateWorkbenchUIDynamic);
    }

    public void UpdateWorkbenchUIDynamic()
    {
        if (workbenchSlot.index >= workbenchObject.workbenchUpgrades.Length)
        {
            workbenchSlot.drillText.text = "All Upgraded";
            workbenchSlot.neededMaterialImage1.enabled = false;
            workbenchSlot.neededMaterialImage2.enabled = false;
            workbenchSlot.neededMaterialImage3.enabled = false;
            workbenchSlot.neededMaterialText1.enabled = false;
            workbenchSlot.neededMaterialText2.enabled = false;
            workbenchSlot.neededMaterialText3.enabled = false;
            workbenchSlot.upgradeButton.gameObject.SetActive(false);
            workbenchSlot.plus1.enabled = false;
            workbenchSlot.plus2.enabled = false;
        }
        else
        {
            switch (workbenchSlot.index)
            {
                case 1:
                    workbenchSlot.drillText.text = "Copper Drill";
                    workbenchSlot.drillImage.sprite = drillSprites[1];
                    break;
                case 2:
                    workbenchSlot.drillText.text = "Iron Drill";
                    workbenchSlot.drillImage.sprite = drillSprites[2];
                    break;
                case 3:
                    workbenchSlot.drillText.text = "Aluminum Drill";
                    workbenchSlot.drillImage.sprite = drillSprites[3];
                    break;
                case 4:
                    workbenchSlot.drillText.text = "Gold Drill";
                    workbenchSlot.drillImage.sprite = drillSprites[4];
                    break;
            }

            workbenchSlot.neededMaterialImage1.sprite = materialSprites[(int)workbenchObject.workbenchUpgrades[workbenchSlot.index].neededMaterials[0].materialType];
            workbenchSlot.neededMaterialImage2.sprite = materialSprites[(int)workbenchObject.workbenchUpgrades[workbenchSlot.index].neededMaterials[1].materialType];
            workbenchSlot.neededMaterialImage3.sprite = materialSprites[(int)workbenchObject.workbenchUpgrades[workbenchSlot.index].neededMaterials[2].materialType];

            string neededMaterialText1 = $"{workbenchObject.workbenchUpgrades[workbenchSlot.index].neededMaterials[0].materialType}";
            neededMaterialText1 = neededMaterialText1.Replace("_", " ");
            string neededMaterialText2 = $"{workbenchObject.workbenchUpgrades[workbenchSlot.index].neededMaterials[1].materialType}";
            neededMaterialText2 = neededMaterialText2.Replace("_", " ");
            string neededMaterialText3 = $"{workbenchObject.workbenchUpgrades[workbenchSlot.index].neededMaterials[2].materialType}";
            neededMaterialText3 = neededMaterialText3.Replace("_", " ");
            workbenchSlot.neededMaterialText1.text = $"{workbenchObject.workbenchUpgrades[workbenchSlot.index].neededMaterials[0].amount} {neededMaterialText1}";
            workbenchSlot.neededMaterialText2.text = $"{workbenchObject.workbenchUpgrades[workbenchSlot.index].neededMaterials[1].amount} {neededMaterialText2}";
            workbenchSlot.neededMaterialText3.text = $"{workbenchObject.workbenchUpgrades[workbenchSlot.index].neededMaterials[2].amount} {neededMaterialText3}";

            if (!workbenchObject.HaveEnoughMaterials(workbenchObject.workbenchUpgrades[workbenchSlot.index]))
            {
                workbenchSlot.upgradeButton.interactable = false;
            }
            else
            {
                workbenchSlot.upgradeButton.interactable = true;
            }
        }
    }


    public void ShowWorkbenchMenu()
    {
        workbenchUI.SetActive(true);
        playerController.enabled = false;
        playerController.StopSounds();
        armScript.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        updateWorkbenchUI.Invoke();
    }

    public void HideWorkbenchMenu()
    {
        workbenchUI.SetActive(false);
        playerController.enabled = true;
        playerController.EnableSounds();
        armScript.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    #endregion

    #region Ending UI Functions

    public void ShowEndingPromptUI()
    {
        endingPromptUI.SetActive(true);
        playerController.enabled = false;
        armScript.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void HideEndingPromptUI()
    {
        endingPromptUI.SetActive(false);
        playerController.enabled = true;
        armScript.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        endWall.gameObject.SetActive(true);
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

    public void ExitGame()
    {
        Application.Quit();
    }

    #endregion
}
