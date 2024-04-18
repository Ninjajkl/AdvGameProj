using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class RoomManager : MonoBehaviour
{
    [SerializeField]
    public Interactable[] interactableObjects;

    protected void Awake()
    {
        GameManager.Instance.AddRoom(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out var playerController))
        {
            foreach (Interactable obj in interactableObjects)
            {
                obj.nearPlayer = true;
                obj.HoveredOver(false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out var playerController))
        {
            foreach (Interactable obj in interactableObjects)
            {
                obj.nearPlayer = false;
                obj.HoveredOver(false);
            }
        }
    }
}
