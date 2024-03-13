using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.GlobalIllumination;
using static Constants;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField]
    private float walkingSpeed = 7.5f;
    [SerializeField]
    private float jumpSpeed = 8.0f;
    [SerializeField]
    private Vector3 jetpackSpeed = new Vector3(4.0f, 8.0f, 4.0f);
    [SerializeField]
    private float gravity = 20.0f;
    [SerializeField]
    private Camera playerCamera;
    [SerializeField]
    private float lookSpeed = 2.0f;
    [SerializeField]
    private float lookXLimit = 45.0f;
    [SerializeField]
    [Tooltip("Range where the player can interact with non-terrain objects")]
    private float interactRange = 10;

    [Header("Terrain Interaction Settings")]
    [Tooltip("Range where the player can interact with the terrain")]
    [SerializeField]
    private float rangeHit = 10;
    [Tooltip("Force of modifications applied to the terrain")]
    [SerializeField]
    private float modiferStrengh = 10;
    [Tooltip("Size of the brush, number of vertex modified")]
    [SerializeField]
    private float sizeHit = 6;
    [Tooltip("The tier of material or lower that can be mined. Ignored in Editor Mode")]
    [SerializeField]
    private int miningLevel = 1;
    [Tooltip("Color of the new voxels generated. Only applicable in Editor mode")]
    [Range(0, TerrainConstants.NUMBER_MATERIALS - 1)]
    [SerializeField]
    private int buildingMaterial = 0;

    //Character-Specific variables
    [Header("Character-Specific variables")]
    public Light flashlight;
    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private bool canMove = true;
    private bool flashlightUsable = false;
    public bool inventoryMenuOn = false;
    private UpgradeableObject lastHovered;
    private GameManager gameManager;
    public int[] Inventory;

    //Terrain System variables
    private RaycastHit hit;
    private ChunkManager chunkManager;

    [SerializeField]
    private PlayerUIController playerUIController;

    void Awake()
    {
        gameManager = GameManager.Instance;
        gameManager.Player = this;
        chunkManager = ChunkManager.Instance;

        Inventory = new int[Enum.GetValues(typeof(MaterialEnum)).Length];
        for (int i = 0; i < Inventory.Length; i++)
        {
            Inventory[i] = 0;
        }
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        
        //Ensure flashlight starts off
        flashlight.enabled = false;

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    #region Update Methods

    void Update()
    {
        TerrainInteraction();
        UpgradeableInteractions();
        Move();
        MiscControls();
    }

    private void TerrainInteraction()
    {
        //Terrain System
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {

            float modification = Input.GetMouseButton(1) ? modiferStrengh : -modiferStrengh;
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, rangeHit))
            {
                chunkManager.ModifyChunkData(hit.point, sizeHit, modification, buildingMaterial, miningLevel);
            }

        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && buildingMaterial != TerrainConstants.NUMBER_MATERIALS - 1)
        {
            buildingMaterial++;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && buildingMaterial != 0)
        {
            buildingMaterial--;
        }
    }

    private void UpgradeableInteractions()
    {
        //Check for Upgradeable objects
        UpgradeableObject upgradeableObject = null;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactRange))
        {
            upgradeableObject = hit.collider.GetComponentInParent<UpgradeableObject>();
        }

        //If in interacting range of upgradeable object, show if we have the materials to upgrade it or not
        if (upgradeableObject != null)
        {
            if (lastHovered != null && upgradeableObject != lastHovered)
            {
                lastHovered.HoveredOver(false);
            }
            upgradeableObject.HoveredOver(true);
            lastHovered = upgradeableObject;
            if (Input.GetKeyDown(KeyCode.E))
            {
                upgradeableObject.FixObject();
            }
        }
        //If no longer hovering, turn back to unselected
        else if (lastHovered != null)
        {
            lastHovered.HoveredOver(false);
            lastHovered = null;
        }
    }

    private void Move()
    {
        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        float curSpeedX = canMove ? (!characterController.isGrounded ? jetpackSpeed.x : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedZ = canMove ? (!characterController.isGrounded ? jetpackSpeed.z : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedZ);

        if (Input.GetButton("Jump") && canMove)
        {
            if (characterController.isGrounded)
            {
                moveDirection.y = jumpSpeed;
            }
            else
            {
                moveDirection.y = jetpackSpeed.y;
            }
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        CollisionFlags flags = characterController.Move(moveDirection * Time.deltaTime);
        if ((flags & CollisionFlags.Above) != 0)
        {
            //If hitting the ceiling, next frame move character downwards
            moveDirection.y = -2.0f;
        }

        // Player and Camera rotation
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    private void MiscControls()
    {
        //If flashlight is useable, you can enable and disable it with 'V'
        if (Input.GetKeyDown(KeyCode.V) && flashlightUsable == true)
        {
            flashlight.enabled = !flashlight.enabled;
            playerUIController.DisplayFlashlightPrompt(this);
        }

        if (Input.GetKeyDown(KeyCode.I)) {
            playerUIController.ShowInventory();
            inventoryMenuOn = !inventoryMenuOn;
        }
    }

    #endregion

    #region Upgrade Methods

    public void ApplyUpgrade(UpgradeType upgradeType, int level = 0)
    {
        switch(upgradeType)
        {
            case UpgradeType.DrillHardness:
                miningLevel += level;
                break;
            case UpgradeType.DrillStrength:
                modiferStrengh += level;
                break;
            case UpgradeType.DrillRange:
                rangeHit += level;
                break;
            case UpgradeType.DrillSize:
                sizeHit += level;
                break;
            case UpgradeType.FlashlightUnlock:
                //You can disable the flashlight by passing a 0 for level
                flashlightUsable = level != 0;
                playerUIController.flashlightPrompt.gameObject.SetActive(true);
                break;
            case UpgradeType.FlashlightRange:
                flashlight.range += level;
                break;
            case UpgradeType.FlashlightIntensity:
                flashlight.intensity += level;
                break;
            default:
                Debug.Log("Error: Unhandled upgrade");
                break;
        }
    }

    #endregion
}
