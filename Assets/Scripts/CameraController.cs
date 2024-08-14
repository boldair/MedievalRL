using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f; // Speed of the camera movement
    public float dragSpeed = 0.1f; // Speed of the camera drag movement
    public float zoomSpeed = 2f; // Speed of the camera zoom
    public float minZoom = .5f; // Minimum zoom level
    public float maxZoom = 5f; // Maximum zoom level

    private PlayerInputActions _inputActions;
    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private bool _isRightMouseButtonHeld;
    private float zoomInput;
    [SerializeField]
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void Awake()
    {
        _inputActions = new PlayerInputActions();

        
        _inputActions.Player.MoveKeys.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _inputActions.Player.MoveKeys.canceled += _ => _moveInput = Vector2.zero;

        _inputActions.Player.Look.performed += ctx => _lookInput = ctx.ReadValue<Vector2>();
        _inputActions.Player.Look.canceled += ctx => _lookInput = Vector2.zero;

        
        _inputActions.Player.MoveMouse.performed += ctx => _isRightMouseButtonHeld = ctx.ReadValue<float>() > 0;
        _inputActions.Player.MoveMouse.canceled += ctx => _isRightMouseButtonHeld = false;
        
        _inputActions.Player.Zoom.performed += ctx => zoomInput = ctx.ReadValue<Vector2>().y;
        _inputActions.Player.Zoom.canceled += ctx => zoomInput = 0f;
    }

    private void OnEnable()
    {
        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Disable();
    }

    private void Update()
    {
        HandleMovement();
        HandleMouseDrag();
        HandleZoom();
    }

    void HandleMovement()
    {
        // Keyboard input for movement
        Vector3 move = new Vector3(_moveInput.x,  _moveInput.y, 0) * (moveSpeed * Time.deltaTime);
        transform.Translate(move, Space.World);
    }

    void HandleMouseDrag()
    {
        if (_isRightMouseButtonHeld)
        {
            // Calculate movement
            float moveX = -_lookInput.x * dragSpeed;
            float moveY = -_lookInput.y * dragSpeed;

            Vector3 move = new Vector3(moveX, moveY, 0);
            transform.Translate(move, Space.World);
        }
    }
    void HandleZoom()
    {
        // Handle zooming in and out
        if (zoomInput != 0f)
        {
            _camera.orthographicSize -= zoomInput * zoomSpeed * Time.deltaTime;
            _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize, minZoom, maxZoom);
        }
    }
}
