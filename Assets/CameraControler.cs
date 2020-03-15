using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControler : MonoBehaviour
{
    public CameraHolder cameraHolder;
    [Header("Rotation")]
    public float mouseSesitivity = 10f;
    public float minAngleX = 0f;
    public float maxAngelX = 80f;
    public float rotationSmoothTime = 0.12f;

    [Header("Zoom")]
    public float minDistance = 5f;
    public float maxDistance = 20f;
    public float zoomSmoothTime = 0.12f;

    [Header("Movement")]
    public float movementSpeed = 5f;

    private Transform target;
    private Vector3 currentRotation;
    private Vector3 currentRotationVelocity;
    private Vector3 desireRotation;

    private float currentDistance;
    private float currentZoomVelocity;
    private float desireDistance;

    private Vector3 desireMovement;
    private Controls controls;

    private void Awake() => controls = new Controls();
    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void Start()
    {
        ResetTarget();
        currentRotation = transform.eulerAngles;
        desireRotation = currentRotation;
        currentDistance = (minDistance + maxDistance) / 2f;
        desireDistance = currentDistance;
    }
    private void LateUpdate()
    {
        HandleRotation();
        HandleZoom();
    }
    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector2 moveInput = controls.Camera.Move.ReadValue<Vector2>();
        desireMovement = Quaternion.Euler(0, transform.eulerAngles.y, 0) * new Vector3(moveInput.x, 0, moveInput.y);
        Debug.Log(desireMovement);
        cameraHolder.Move(desireMovement * movementSpeed * Time.deltaTime);
    }

    private void HandleZoom()
    {
        desireDistance -= controls.Camera.Zoom.ReadValue<float>() * Time.deltaTime;
        desireDistance = Mathf.Clamp(desireDistance, minDistance, maxDistance);
        currentDistance = Mathf.SmoothDamp(currentDistance, desireDistance, ref currentZoomVelocity, zoomSmoothTime);
        transform.position = target.position - transform.forward * currentDistance;
    }

    private void HandleRotation()
    {
        if (controls.Camera.MouseRotation.ReadValue<float>() > 0)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            desireRotation.x -= mouseDelta.y * mouseSesitivity * Time.deltaTime;
            desireRotation.y += mouseDelta.x * mouseSesitivity * Time.deltaTime;
            desireRotation.x = Mathf.Clamp(desireRotation.x, minAngleX, maxAngelX);
        }
        currentRotation = Vector3.SmoothDamp(currentRotation, desireRotation, ref currentRotationVelocity, rotationSmoothTime);
        transform.eulerAngles = currentRotation;
    }

    public void ResetTarget()
    {
        SetTarget(cameraHolder.transform);
    }

    private void SetTarget(Transform targetTransform)
    {
        if (targetTransform == target) return;
        if(targetTransform == cameraHolder.transform && target != null)
        {
            targetTransform.position = target.position;
        }
        target = targetTransform;
    }
}
