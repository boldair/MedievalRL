using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private GameObject characterToMove;
    private PlayerInputActions _inputActions;  // Reference to the input actions
    private Vector2 _targetPosition;  // Target position to move the character to
    private bool _isMoving = false;  // Flag to indicate if the character is moving
    public float moveSpeed = 5f;
    private Pathfinding _pathfinding; 
    private Camera _cam;
    // Start is called before the first frame update
    private void Awake()
    {
        _inputActions = new PlayerInputActions();  // Instantiate the input actions
        _cam = Camera.main;
        
    }
    
    private void OnEnable()
    {
        _inputActions.Enable();  // Enable the input actions
        _inputActions.Player.LeftClick.performed += OnClick;  // Subscribe to the click event
    }

    private void OnDisable()
    {
        _inputActions.Player.LeftClick.performed -= OnClick;  // Unsubscribe from the click event
        _inputActions.Disable();  // Disable the input actions
    }
    private void OnClick(InputAction.CallbackContext context)
    {
        if (characterToMove == null )
        {
            Vector2 mousePosition = _cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            // Perform a raycast at the clicked position
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null && hit.collider.CompareTag("PlayableCharacter"))
            {
                // Assign the clicked player character as the characterToMove
                characterToMove = hit.collider.gameObject;
                _pathfinding = characterToMove.GetComponent<Pathfinding>();
                _pathfinding.enabled = true;
                
                if (_pathfinding == null)
                {
                    Debug.LogWarning("No Pathfinding component found on the selected character.");
                    characterToMove = null; // Reset characterToMove if there's no Pathfinding component
                    return;
                }
            }
            else
            {
                // No player character was found at the clicked position
                Debug.LogWarning("No player character found at the clicked location.");
                return;
            }
        }
        else
        {
            if (_pathfinding.Path.Count > 0 && !_isMoving)
            {
                StartCoroutine(MoveCharacterTroughNodes(_pathfinding.Path));
            }
        }
    }
     
    private IEnumerator MoveCharacterTroughNodes(List<Node> nodesInPath)
    {
        _isMoving = true;
        foreach (var node in nodesInPath)
        {
            // Move towards the next tile
            yield return StartCoroutine(MoveToTile(node));
        }

        _isMoving = false;
    }

    private IEnumerator MoveToTile(Node node)
    {
        _targetPosition = node.GetCenter();

        while (Vector2.Distance(characterToMove.transform.position, _targetPosition) > 0.01f)
        {
            characterToMove.transform.position = Vector2.MoveTowards(characterToMove.transform.position, _targetPosition, moveSpeed * Time.deltaTime);
            yield return null; // Wait for the next frame
        }

        // Ensure the final position is exactly the target position
        characterToMove.transform.position = _targetPosition;
    }
}
