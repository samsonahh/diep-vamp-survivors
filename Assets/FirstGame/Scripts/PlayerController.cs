using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirstGame
{
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]

        [Header("Settings")]
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float rotationSpeed = 20f;

        private Vector3 moveDirection;
        private Vector3 mouseWorldPosition;

        private void Update()
        {
            HandleMovementInput();
            HandleMouseInput();

            HandleRotation();
        }

        private void FixedUpdate()
        {
            HandleMovement();
        }

        private void HandleMovementInput()
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");

            moveDirection = new Vector3(x, y, 0).normalized;
        }

        private void HandleMouseInput()
        {
            Vector3 mouseScreenPos = Input.mousePosition;

            mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        }

        private void HandleMovement()
        {
            transform.Translate(moveSpeed * Time.fixedDeltaTime * moveDirection, Space.World);
        }

        private void HandleRotation()
        {
            Vector3 dirFromPlayerToMouse = mouseWorldPosition - transform.position;

            float targetAngle = Mathf.Atan2(dirFromPlayerToMouse.y, dirFromPlayerToMouse.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}

