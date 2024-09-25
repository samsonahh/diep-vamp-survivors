﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirstGameProg2Game
{
    public class Entity : MonoBehaviour
    {
        [Header("Entity: References")]
        [SerializeField] private protected SpriteRenderer bodySpriteRenderer;
        [SerializeField] private protected HitNumber hitNumberPrefab;

        [Header("Entity: Settings")]
        [SerializeField] private protected float moveSpeed = 3f;
        [SerializeField] private protected float rotationSpeed = 20f;

        public int CurrentHealth;
        public int MaxHealth = 100;
        [SerializeField] private protected float iFrameDuration = 0.5f;
        private float iFrameTimer = Mathf.Infinity;

        private protected Color startingBodyColor;
        private protected bool takeDamageCoroutineStarted;
        private protected Vector3 startingScale;

        private protected State currentState;
        public State DefaultState { get; protected set; }

        public EntityStunnedState EntityStunnedState { get; private set; }

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
            startingScale = bodySpriteRenderer.transform.parent.localScale;
        }

        private void Update()
        {
            OnUpdate();
        }

        protected virtual void OnUpdate()
        {
            currentState?.Update();

            HandleHealth();
            HandleIFrames();
        }

        private void FixedUpdate()
        {
            OnFixedUpdate();
        }

        protected virtual void OnFixedUpdate()
        {
            currentState?.FixedUpdate();
        }

        protected virtual void SetupStates()
        {
            EntityStunnedState = new EntityStunnedState(this);
        }

        protected virtual void StartWithState(State state)
        {
            currentState = state;
            currentState.Enter();
        }

        protected virtual void SetDefaultState(State state)
        {
            DefaultState = state;
        }

        public virtual void ChangeState(State newState)
        {
            currentState.Exit();
            currentState = newState;
            currentState.Enter();
        }

        public virtual void Die(Entity source)
        {
            Destroy(gameObject);

            OnDeath();
        }

        protected virtual void OnDeath()
        {

        }

        public void LookAt(Vector2 pos)
        {
            Vector2 dir = pos - (Vector2)transform.position;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        public void LookAt(Entity target)
        {
            if (target == null) return;

            LookAt(target.transform.position);
        }

        protected virtual void HandleHealth()
        {
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
        }

        private void HandleIFrames()
        {
            if (iFrameTimer < iFrameDuration * 2f) iFrameTimer += Time.deltaTime;
        }

        public virtual bool TakeDamage(int dmg, Entity source)
        {
            if (iFrameTimer < iFrameDuration) return false;

            if (!takeDamageCoroutineStarted) StartCoroutine(TakeDamageCoroutine());

            iFrameTimer = 0f;

            CurrentHealth -= dmg;

            Vector3 randomHitNumDir = (Vector3)Random.insideUnitCircle.normalized;
            HitNumber hitNumber = Instantiate(hitNumberPrefab, transform.position + startingScale.x/2f * randomHitNumDir, Quaternion.identity);
            hitNumber.Play(dmg, randomHitNumDir);

            if (CurrentHealth <= 0 && MaxHealth != 0)
            {
                Die(source);
            }

            return true;
        }

        private IEnumerator TakeDamageCoroutine()
        {
            float duration = 0.15f;
            
            takeDamageCoroutineStarted = true;

            bodySpriteRenderer.color = Color.red;

            float startScale = 1f;
            float endScale = 0.92f;
            for(float t = 0; t < duration/2f; t += Time.deltaTime)
            {
                float parameter = EasingFunctions.OutCubic(t / (duration / 2f));
                bodySpriteRenderer.transform.parent.localScale = Vector3.Lerp(startScale * startingScale, endScale * startingScale, parameter);
                yield return null;
            }
            for (float t = 0; t < duration / 2f; t += Time.deltaTime)
            {
                float parameter = EasingFunctions.InCubic(t / (duration / 2f));
                bodySpriteRenderer.transform.parent.localScale = Vector3.Lerp(endScale * startingScale, startScale * startingScale, parameter);
                yield return null;
            }

            bodySpriteRenderer.transform.parent.localScale = startingScale;
            bodySpriteRenderer.color = startingBodyColor;

            takeDamageCoroutineStarted = false;
        }

        public virtual void Heal(int health)
        {
            CurrentHealth += health;
        }

        public void Stun(float duration)
        {
            EntityStunnedState.SetStunDuration(duration);
            ChangeState(EntityStunnedState);
        }

        public void SetTeam(int newTeam)
        {
            team = newTeam;
        }

        public float GetDistanceFromEntity(Entity entity)
        {
            return Vector3.Distance(transform.position, entity.transform.position);
        }

        public List<Entity> GetNearbyEntities(float radius, int team, bool teamMeansHostile)
        {
            List<Entity> result = new List<Entity>();

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

            foreach (Collider2D hit in hits)
            {
                if (hit.gameObject == gameObject) continue;

                Entity entity = hit.GetComponent<Entity>();
                entity = entity == null ? hit.GetComponentInParent<Entity>() : entity;

                if (entity == null) continue;

                if (teamMeansHostile)
                {
                    if (entity.team != team) result.Add(entity);
                }
                else
                {
                    if (entity.team == team) result.Add(entity);
                }
            }

            return result;
        }

        public Entity GetClosestEntity(List<Entity> nearbyEntities)
        {
            if (nearbyEntities.Count == 0) return null;
            if (nearbyEntities.Count == 1) return nearbyEntities[0];

            int closestEntityIndex = 0;
            for (int i = 0; i < nearbyEntities.Count; i++)
            {
                float closestDistance = GetDistanceFromEntity(nearbyEntities[closestEntityIndex]);
                float newDistance = GetDistanceFromEntity(nearbyEntities[i]);

                if (newDistance < closestDistance) closestEntityIndex = i;
            }

            return nearbyEntities[closestEntityIndex];
        }
    }
}
