using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyCam : MonoBehaviour
{
    public float movementSpeed = 10f;
    public float rotationSpeed = 10f;

    private bool mouseDown = false;

    private float forward;
    private float right;
    private float up;

    private float mouseX;
    private float mouseY;

    public bool canMove = true;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    void Update()
    {
        if (!canMove) return;

        // input
        if (Input.GetMouseButtonDown(1))
        {
            mouseDown = true;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }
        if (Input.GetMouseButtonUp(1))
        {
            mouseDown = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (mouseDown)
        {
            forward = Input.GetAxis("Forward");
            right = Input.GetAxis("Right");
            up = Input.GetAxis("Up");

            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");
        }
    }

    private void LateUpdate()
    {
        if (!canMove) return;

        // camera
        if (mouseDown)
        {
            // position
            transform.position += transform.forward * forward * Time.deltaTime * movementSpeed;
            transform.position += transform.right * right * Time.deltaTime * movementSpeed;
            transform.position += transform.up * up * Time.deltaTime * movementSpeed;

            // rotation
            transform.Rotate(0, mouseX * rotationSpeed, 0, Space.World);
            transform.Rotate(mouseY * rotationSpeed, 0, 0);
        }

    }

    public void ResetCamera()
    {
        transform.position = originalPosition;
        transform.rotation = originalRotation;
    }

}
