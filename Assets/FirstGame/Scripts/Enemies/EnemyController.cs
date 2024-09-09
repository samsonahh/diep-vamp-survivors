using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirstGameProg2Game
{
    public class EnemyController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private protected Transform bulletSpawnTransform;
        [SerializeField] private protected BulletScript bulletPrefab;
        [SerializeField] private protected SpriteRenderer bodySpriteRenderer;
        private protected PlayerController player;

        [Header("Settings")]
        [SerializeField] private protected float moveSpeed = 3f;
        [SerializeField] private protected float rotationSpeed = 20f;

        public int CurrentHealth;
        public int MaxHealth = 100;

        [SerializeField] private protected int damage = 10;
        [SerializeField] private protected float fireRate = 1f;
        [SerializeField] private protected float shootStrength = 1f;
        private protected Color startingBodyColor;
        private protected bool takeDamageCoroutineStarted;

        private protected float distanceFromPlayer => Vector3.Distance(transform.position, player.transform.position);

        private protected List<Coroutine> stateCoroutines = new List<Coroutine>();

        private void Awake()
        {
            OnAwake();
        }

        protected virtual void OnAwake()
        {
            player = FindObjectOfType<PlayerController>();
        }

        private void Start()
        {
            OnStart();
        }

        protected virtual void OnStart()
        {
            CurrentHealth = MaxHealth;

            startingBodyColor = bodySpriteRenderer.color;
        }

        private void Update()
        {
            OnUpdate();
        }

        protected virtual void OnUpdate()
        {
            HandleHealth();
        }

        private void FixedUpdate()
        {
            OnFixedUpdate();
        }

        protected virtual void OnFixedUpdate()
        {

        }

        protected virtual void OnDeath()
        {
            Destroy(gameObject);
        }

        protected virtual void LookAt(Vector2 pos)
        {
            Vector2 dir = pos - (Vector2)transform.position;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        private void HandleHealth()
        {
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
        }

        public void TakeDamage(int dmg)
        {
            if (!takeDamageCoroutineStarted) StartCoroutine(TakeDamageCoroutine());

            CurrentHealth -= dmg;

            if (CurrentHealth <= 0)
            {
                OnDeath();
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
