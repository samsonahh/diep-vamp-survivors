using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FirstGameProg2Game
{
    public class Entity : MonoBehaviour
    {
        [Header("Entity: References")]
        [SerializeField] private protected SpriteRenderer bodySpriteRenderer;
        [SerializeField] private protected Slider hpSlider;
        [SerializeField] private protected HitNumber hitNumberPrefab;

        [Header("Entity: Settings")]
        [SerializeField] private protected float moveSpeed = 3f;
        [SerializeField] private protected float rotationSpeed = 20f;

        public int CurrentHealth;
        public int MaxHealth = 100;

        private protected Color startingBodyColor;
        private protected bool takeDamageCoroutineStarted;
        private protected Vector3 startingScale;

        private protected bool deathCoroutineStarted;

        private Vector3 hpSliderCanvasStartPos;

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

            hpSliderCanvasStartPos = hpSlider.transform.parent.localPosition;
        }

        private void Update()
        {
            OnUpdate();
        }

        protected virtual void OnUpdate()
        {
            currentState?.Update();

            HandleHealth();
            HandleHealthBar();
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
            if(!deathCoroutineStarted) StartCoroutine(DeathFadeCoroutine());

            Destroy(hpSlider.transform.parent.gameObject);
        }

        protected virtual void OnDeath()
        {
            
        }

        private IEnumerator DeathFadeCoroutine()
        {
            deathCoroutineStarted = true;

            enabled = false;

            float duration = 0.15f;

            float startScale = 1f;
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                float parameter = EasingFunctions.OutCubic(t / duration);
                bodySpriteRenderer.transform.parent.localScale = Vector3.Lerp(startScale * startingScale, Vector3.zero, parameter);
                yield return null;
            }

            bodySpriteRenderer.transform.parent.localScale = Vector3.zero;

            OnDeath();

            Destroy(gameObject);
        }

        public void LookAt(Vector2 pos)
        {
            Vector2 dir = pos - (Vector2)transform.position;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

            bodySpriteRenderer.transform.parent.localRotation = Quaternion.Lerp(bodySpriteRenderer.transform.parent.localRotation, targetRotation, rotationSpeed * Time.deltaTime);
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

        private void HandleHealthBar()
        {
            hpSlider.transform.parent.position = transform.position + transform.localScale.x * hpSliderCanvasStartPos;
            hpSlider.transform.parent.rotation = Quaternion.identity;

            float targetValue = MaxHealth != 0 ? CurrentHealth / (float)MaxHealth : 0;

            hpSlider.value = Mathf.Lerp(hpSlider.value, targetValue, 10f * Time.unscaledDeltaTime);

            hpSlider.transform.parent.gameObject.SetActive(hpSlider.value < 0.99f);
        }

        public virtual void TakeDamage(int dmg, Entity source)
        {
            if (!takeDamageCoroutineStarted) StartCoroutine(TakeDamageCoroutine());

            CurrentHealth -= dmg;

            Vector3 randomHitNumDir = (Vector3)Random.insideUnitCircle.normalized;
            HitNumber hitNumber = Instantiate(hitNumberPrefab, transform.position + startingScale.x/2f * randomHitNumDir, Quaternion.identity);
            hitNumber.Play(dmg, randomHitNumDir);

            if (CurrentHealth <= 0 && MaxHealth != 0)
            {
                Die(source);
            }

            CameraController.Instance.ShakeCamera(0.5f, 0.1f);
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

        private protected int CalculateRandomDamage(int baseDamage)
        {
            return baseDamage + (int)Mathf.Ceil(baseDamage * Random.Range(-0.25f, 0.5f));
        }
    }
}
