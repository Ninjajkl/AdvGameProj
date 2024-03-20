using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotManager : MonoBehaviour
{
    public InventorySlotManager()
    {

    }

    public InventorySlotManager(string slotName, int slotQuantity)
    {
        this.slotName.text = slotName;
        this.slotQuantity.text = $"{slotQuantity}";
    }

    public TMP_Text slotName;
    public TMP_Text slotQuantity;
    public Image slotIcon;
    public bool instianiated = false;
}
