using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FirstGameProg2Game
{
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform bulletSpawnTransform;
        [SerializeField] private BulletScript bulletPrefab;
        [SerializeField] private SpriteRenderer bodySpriteRenderer;

        [Header("Settings")]
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float rotationSpeed = 20f;

        public int CurrentHealth;
        public int MaxHealth = 100;

        [SerializeField] private int damage = 10;
        [SerializeField] private float fireRate = 1f;
        [SerializeField] private float shootStrength = 1f;
        private float fireTimer;
        private Color startingBodyColor;
        private bool takeDamageCoroutineStarted;

        private Vector3 moveDirection;
        private Vector3 mouseWorldPosition;

        [HideInInspector] public UnityEvent OnPlayerDeath = new UnityEvent();

        private void Start()
        {
            CurrentHealth = MaxHealth;
         
            fireTimer = fireRate;

            startingBodyColor = bodySpriteRenderer.color;
        }

        private void Update()
        {
            HandleMovementInput();
            HandleMousePositionInput();
            HandleMouseClickInput();

            HandleRotation();
            HandleFireRateTimer();
            HandleHealth();
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

        private void HandleMousePositionInput()
        {
            Vector3 mouseScreenPos = Input.mousePosition;

            mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        }

        private void HandleMouseClickInput()
        {
            if (fireTimer < fireRate) return;

            if (Input.GetKey(KeyCode.Mouse0))
            {
                FireBullet();
            }
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

        private void HandleFireRateTimer()
        {
            if (fireTimer < fireRate * 2f) fireTimer += Time.deltaTime;
        }

        private void HandleHealth()
        {
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
        }

        private void FireBullet()
        {
            fireTimer = 0f;

            Vector2 dir = ((Vector2)(mouseWorldPosition - transform.position)).normalized;

            BulletScript bullet = Instantiate(bulletPrefab, bulletSpawnTransform.position, Quaternion.identity);
            bullet.Shoot(dir, shootStrength, damage, false);
        }

        public void TakeDamage(int dmg)
        {
            if (!takeDamageCoroutineStarted) StartCoroutine(TakeDamageCoroutine());
            CurrentHealth -= dmg;

            if(CurrentHealth <= 0)
            {
                OnPlayerDeath?.Invoke();
            }
        }

        private IEnumerator TakeDamageCoroutine()
        {
            takeDamageCoroutineStarted = true;

            bodySpriteRenderer.color = Color.red;

            yield return new WaitForSeconds(0.15f);

            bodySpriteRenderer.color = startingBodyColor;

            takeDamageCoroutineStarted = false;
        }
    }
}

