using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public abstract class Interactable : MonoBehaviour
{
    public bool nearPlayer = false;
    protected AudioSource audioSource;
    protected GameManager gameManager;

    protected void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        gameManager = GameManager.Instance;
    }

    public void SetLayerRecursively(Transform parent, int layer)
    {
        //Set the layer of the parent GameObject
        parent.gameObject.layer = layer;

        //Iterate through each child of the parent
        foreach (Transform child in parent)
        {
            //Recursively set the layer for each child
            SetLayerRecursively(child, layer);
        }
    }
    public abstract void HoveredOver(bool hovering);
}
