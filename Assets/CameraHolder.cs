using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHolder : MonoBehaviour
{
    private CharacterController cc;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    public void Move(Vector3 movement)
    {
        if (!cc.isGrounded) movement.y -= 10;
        cc.Move(movement);
    }
}
