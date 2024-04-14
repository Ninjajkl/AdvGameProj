using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RefinerySlot : MonoBehaviour
{
    public RefinerySlot()
    {

    }

    public int index;
    public TMP_Text refinedMaterialText;
    public TMP_Text neededMaterialText;
    public TMP_Text neededCoalText;
    public TMP_Text refinedMaterialQuantityText;
    public Image refinedMaterialImage;
    public Image neededMaterialImage;
    public TMP_InputField refinedQuantityInputField;
    public Button refineButton;
    public int coalQuantity;
    public int rawMaterialQuantity;
    public bool instianiated = false;
}
