using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public class CharacterController : MonoBehaviour
{
    public Tilemap tilemap;
    [SerializeField] private GameObject characterToMove;
    private PlayerInputActions inputActions;  // Reference to the input actions
    private Vector2 targetPosition;  // Target position to move the character to
    private bool isMoving = false;  // Flag to indicate if the character is moving
    public float moveSpeed = 5f;

    private Camera cam;
    // Start is called before the first frame update
    private void Awake()
    {
        inputActions = new PlayerInputActions();  // Instantiate the input actions
        cam = Camera.main;
    }
    
    private void OnEnable()
    {
        inputActions.Enable();  // Enable the input actions
        inputActions.Player.LeftClick.performed += OnClick;  // Subscribe to the click event
    }

    private void OnDisable()
    {
        inputActions.Player.LeftClick.performed -= OnClick;  // Unsubscribe from the click event
        inputActions.Disable();  // Disable the input actions
    }
    private void OnClick(InputAction.CallbackContext context)
    {
        Vector2 worldPosition = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());  // Convert mouse position to world position
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);  // Convert world position to cell position
        targetPosition = tilemap.GetCellCenterWorld(cellPosition);  // Get the center of the clicked tile
        //isMoving = true;  // Set the movement flag
        characterToMove.transform.position = targetPosition;
    }
    void Update()
    {
        if (isMoving)
        {
            MoveCharacter();
        }
    } 
    void MoveCharacter()
    {
        if (characterToMove)
        {
            characterToMove.transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);  // Move the character towards the target position
               
        }
        
    }
}
