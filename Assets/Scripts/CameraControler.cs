using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControler : MonoBehaviour
{
    public Transform cameraTransform;
    public Transform followTransform;
    
    [Header("Movement")]
    public float movementSpeed = 5f;
    public bool useEdgeScreenMovement = true;
    public float edgeScreenBorder = 15f;
    public float edgeScreeMoveSpeed = 5f;

    [Header("Rotation")]
    public float rotationSpeed = 10f;
    public float rotationSmoothTime = 0.12f;
    public float minRotationX = 0f;
    public float maxRotationX = 80f;

    private Vector3 desireRotation;
    private Vector3 currentRotation;
    private Vector3 currentRotationVelocity;


    [Header("Zoom")]
    public float zoomSpeed = 1f;
    public float zoomSmoothTime = 0.12f;
    public float minZoom = 5f;
    public float maxZoom = 20;
    public float moveSpeedModifier = 2;

    private float desireZoom;
    private float currentZoom;
    private float currentZoomVelocity;
    
    private CharacterController characterController;
    private GameInputAction inputActions;

    private static CameraControler instance;
    public static CameraControler Instance {
        get {
            if (instance == null) instance = FindObjectOfType<CameraControler>();
            return instance;
        }
    }
    private void Awake()
    {
        instance = this;
        characterController = GetComponent<CharacterController>();
        inputActions = new GameInputAction();
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();

    private void Start()
    {
        currentRotation = transform.localEulerAngles;
        desireRotation = currentRotation;
        currentZoom = (minZoom + maxZoom) / 2f;
        desireZoom = currentZoom;
    }

    private void Update()
    {
        
        HandleRotation();
        HandleZoom();
    }
    private void FixedUpdate()
    {
        if(followTransform != null)
        {
            transform.position = followTransform.position;
        }
        else
        {
            HandleMovement();
        }
    }

    private void HandleMovement()
    {
        Vector2 input = inputActions.Camera.Move.ReadValue<Vector2>() * movementSpeed;
        if (useEdgeScreenMovement)
        {
            Rect left = new Rect(0f, 0f, edgeScreenBorder, Screen.height);
            Rect right = new Rect(Screen.width - edgeScreenBorder, 0f, edgeScreenBorder, Screen.height);
            Rect top = new Rect(0f, Screen.height - edgeScreenBorder, Screen.width, edgeScreenBorder);
            Rect bottom = new Rect(0f, 0f, Screen.width, edgeScreenBorder);
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            input.x += left.Contains(mousePosition) ? -edgeScreeMoveSpeed : right.Contains(mousePosition) ? edgeScreeMoveSpeed : 0;
            input.y += bottom.Contains(mousePosition) ? -edgeScreeMoveSpeed : top.Contains(mousePosition) ? edgeScreeMoveSpeed : 0;
        }
        var moveDirection = Quaternion.Euler(0, transform.eulerAngles.y, 0) * new Vector3(input.x, 0f, input.y);
        var currentSpeedModifier = Mathf.Lerp(1, moveSpeedModifier, Mathf.InverseLerp(minZoom, maxZoom, currentZoom));
        moveDirection *= currentSpeedModifier;
        if(characterController != null)
        {
            characterController.SimpleMove(moveDirection);
        }
        else
        {
            transform.Translate(moveDirection * Time.deltaTime, Space.World);
        }
    }
    private void HandleRotation()
    {
        Vector2 input = inputActions.Camera.Rotate.ReadValue<Vector2>();
        desireRotation.x -= input.y * rotationSpeed * Time.deltaTime;
        desireRotation.y += input.x * rotationSpeed * Time.deltaTime;
        desireRotation.x = Mathf.Clamp(desireRotation.x, minRotationX, maxRotationX);

        currentRotation = Vector3.SmoothDamp(currentRotation, desireRotation, ref currentRotationVelocity, rotationSmoothTime);
        transform.localEulerAngles = currentRotation;
    }
    private void HandleZoom()
    {
        desireZoom -= inputActions.Camera.Zoom.ReadValue<float>() * zoomSpeed * Time.deltaTime;
        desireZoom = Mathf.Clamp(desireZoom, minZoom, maxZoom);
        currentZoom = Mathf.SmoothDamp(currentZoom, desireZoom, ref currentZoomVelocity, zoomSmoothTime);
        cameraTransform.localPosition = Vector3.back * currentZoom;
    }
}
