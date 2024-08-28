using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WASD : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float movementSpeed = 5f;

    Vector3 movementDirection;

    void Start()
    {
        
    }

    void Update()
    {
        ReadMovementInput();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void ReadMovementInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        movementDirection = new Vector3(x, y, 0).normalized;
    }

    private void HandleMovement()
    {
        transform.Translate(movementSpeed * Time.fixedDeltaTime * movementDirection);
    }
}
