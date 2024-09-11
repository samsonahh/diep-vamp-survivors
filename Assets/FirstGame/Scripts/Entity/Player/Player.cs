using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirstGameProg2Game
{
    public class Player : Entity
    {
        [Header("Player: References")]
        [SerializeField] private Transform bulletSpawnTransform;
        [SerializeField] private BulletScript bulletPrefab;

        [Header("Player: Settings")]
        [SerializeField] private int damage = 10;
        [SerializeField] private float fireRate = 1f;
        [SerializeField] private float shootStrength = 15f;
        private float fireTimer;

        private Vector3 moveDirection;
        private Vector3 mouseWorldPosition;

        private PlayerFreePlayState moveState;

        protected override void OnAwake()
        {
            base.OnAwake();

            SetTeam(0);
        }

        protected override void OnStart()
        {
            base.OnStart();

            StartWithState(moveState);
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
        }

        protected override void OnDeath()
        {
            base.OnDeath();

            Destroy(gameObject);
        }

        protected override void SetupStates()
        {
            base.SetupStates();

            moveState = new PlayerFreePlayState(this);
        }

        public void HandleMovementInput()
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");

            moveDirection = new Vector3(x, y, 0).normalized;
        }

        public void HandleMousePositionInput()
        {
            Vector3 mouseScreenPos = Input.mousePosition;

            mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        }

        public void HandleMouseClickInput()
        {
            if (fireTimer < fireRate) return;

            if (Input.GetKey(KeyCode.Mouse0))
            {
                FireBullet();
            }
        }

        public void HandleMovement()
        {
            transform.Translate(moveSpeed * Time.fixedDeltaTime * moveDirection, Space.World);
        }

        public void HandleRotation()
        {
            LookAt(mouseWorldPosition);
        }

        public void HandleFireRateTimer()
        {
            if (fireTimer < fireRate * 2f) fireTimer += Time.deltaTime;
        }

        public void ResetFireTimer()
        {
            fireTimer = fireRate;
        }

        public void FireBullet()
        {
            fireTimer = 0f;

            Vector2 dir = ((Vector2)(mouseWorldPosition - transform.position)).normalized;

            BulletScript bullet = Instantiate(bulletPrefab, bulletSpawnTransform.position, Quaternion.identity);
            bullet.Shoot(dir, shootStrength, damage, team);
        }
    }
}

