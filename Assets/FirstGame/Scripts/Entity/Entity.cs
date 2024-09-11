using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirstGameProg2Game
{
    public class Entity : MonoBehaviour
    {
        [Header("Entity: References")]
        [SerializeField] private protected SpriteRenderer bodySpriteRenderer;

        [Header("Entity: Settings")]
        [SerializeField] private protected float moveSpeed = 3f;
        [SerializeField] private protected float rotationSpeed = 20f;

        public int CurrentHealth;
        public int MaxHealth = 100;

        private protected Color startingBodyColor;
        private protected bool takeDamageCoroutineStarted;

        private protected State currentState;

        private protected int team = 0;
        public int Team => team;

        private void Awake()
        {
            OnAwake();
        }

        protected virtual void OnAwake()
        {
            SetupStates();
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
            currentState.Update();

            HandleHealth();
        }

        private void FixedUpdate()
        {
            OnFixedUpdate();
        }

        protected virtual void OnFixedUpdate()
        {
            currentState.FixedUpdate();
        }

        protected virtual void SetupStates()
        {

        }

        protected virtual void StartWithState(State state)
        {
            currentState = state;
            currentState.Enter();
        }

        protected virtual void ChangeState(State newState)
        {
            currentState.Exit();
            currentState = newState;
            currentState.Enter();
        }

        protected virtual void OnDeath()
        {
            Destroy(gameObject);
        }

        public void LookAt(Vector2 pos)
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

        public void SetTeam(int newTeam)
        {
            team = newTeam;
        }

        protected float GetDistanceFromEntity(Entity entity)
        {
            return Vector3.Distance(transform.position, entity.transform.position);
        }
    }
}
