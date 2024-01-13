using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : NetworkBehaviour, IBeforeUpdate
{
    [SerializeField] private Vector2 moveDirection;
    [SerializeField] private Vector2 lookDirection;
    [SerializeField] private bool jump;


    Vector2 previousMousePosition;
    Vector2 currentMousePosition;

    public Vector2 MoveDirection { get { return moveDirection; } }
    public Vector2 LookDirection { get { return lookDirection; } set { lookDirection = value; } }

    public bool Jump { get { return jump; } set { jump = value; } }

    void IBeforeUpdate.BeforeUpdate()
    {
        lookDirection = default;
        UpdateNetwork();
    }

    public void UpdateNetwork()
    {
        moveDirection.x = Input.GetAxis("Horizontal");
        moveDirection.y = Input.GetAxis("Vertical");

        //UpdateMouseDelta();
        //lookDirection = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        Mouse mouse = Mouse.current;
        if (mouse != null)
        {
            Vector2 mouseDelta = mouse.delta.ReadValue();
            lookDirection += new Vector2(mouseDelta.y, mouseDelta.x);
        }

        if (Input.GetButtonDown("Jump")) jump = true;
    }

    void UpdateMouseDelta()
    {
        currentMousePosition = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        Vector2 mouseDelta = currentMousePosition - previousMousePosition;

        lookDirection += mouseDelta;
        // Use mouseDelta for your calculations

        previousMousePosition = currentMousePosition;
    }






}
