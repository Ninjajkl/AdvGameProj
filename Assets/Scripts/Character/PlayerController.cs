using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float walkingSpeed = 7.5f;
    [SerializeField]
    private float runningSpeed = 11.5f;
    [SerializeField] 
    private float jumpSpeed = 8.0f;
    [SerializeField] 
    private float gravity = 20.0f;
    [SerializeField]
    private Camera playerCamera;
    [SerializeField]
    private float lookSpeed = 2.0f;
    [SerializeField]
    private float lookXLimit = 45.0f;

    [Tooltip("Range where the player can interact with the terrain")] 
    [SerializeField]
    private float rangeHit = 100;
    [Tooltip("Force of modifications applied to the terrain")] 
    [SerializeField]
    private float modiferStrengh = 10;
    [Tooltip("Size of the brush, number of vertex modified")] 
    [SerializeField]
    private float sizeHit = 6;
    [Tooltip("Color of the new voxels generated")]
    [Range(0, TerrainConstants.NUMBER_MATERIALS - 1)] 
    [SerializeField]
    private int buildingMaterial = 0;

    //Character-Specific variables
    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private bool canMove = true;

    //Terrain System variables
    private RaycastHit hit;
    private ChunkManager chunkManager;

    void Awake()
    {
        chunkManager = ChunkManager.Instance;
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        ModifyTerrain();
        Move();
    }

    private void ModifyTerrain()
    {
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {

            float modification = (Input.GetMouseButton(0)) ? modiferStrengh : -modiferStrengh;
            if (Physics.Raycast(transform.position, transform.forward, out hit, rangeHit))
            {
                chunkManager.ModifyChunkData(hit.point, sizeHit, modification, buildingMaterial);
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

    private void Move()
    {
        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
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
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

}
