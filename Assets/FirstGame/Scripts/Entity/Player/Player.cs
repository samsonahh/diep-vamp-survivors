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

        [Header("Player: Level Settings")]
        [SerializeField] private int currentLevel = 1;
        [SerializeField] private int maxLevel = 71;
        [SerializeField] private int currentEXP;
        [SerializeField] private int maxEXP;
        [SerializeField] private float levelUpAcceleration = 10f;
        [SerializeField] private int levelUpLinearGrowth = 20;
        [SerializeField] private int baseEXP = 10;

        [Header("Player: Stats Settings")]
        [SerializeField] private int healthRegen = 1;
        [SerializeField] private float healthRegenRate = 1;
        [SerializeField] private float healthRegenPauseDurationAfterHit = 2f;
        [SerializeField] private int bodyDamage = 10;
        [SerializeField] private float bulletSpeed = 15f;
        [SerializeField] private int bulletDamage = 10;
        [SerializeField] private float reloadSpeed = 1f;
        private float reloadTimer;
        private float healthRegenPauseTimer;
        private bool canRegen = true;

        private Vector3 moveDirection;
        private Vector3 mouseWorldPosition;

        public PlayerFreePlayState PlayerFreePlayState { get; private set; }

        protected override void OnAwake()
        {
            base.OnAwake();

            SetTeam(0);
        }

        protected override void OnStart()
        {
            base.OnStart();

            StartWithState(PlayerFreePlayState);
            SetDefaultState(PlayerFreePlayState);

            SetupLevel();

            StartCoroutine(RegenCoroutine());
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            HandleLevel();
        }

        public override void Die(Entity s)
        {
            base.Die(s);
        }

        protected override void OnDeath()
        {
            base.OnDeath();
        }

        protected override void SetupStates()
        {
            base.SetupStates();

            PlayerFreePlayState = new PlayerFreePlayState(this);

            DefaultState = PlayerFreePlayState;
        }

        protected override void HandleHealth()
        {
            base.HandleHealth();

            canRegen = CurrentHealth < MaxHealth && healthRegenPauseTimer > healthRegenPauseDurationAfterHit;

            if(healthRegenPauseTimer < healthRegenPauseDurationAfterHit * 2f) healthRegenPauseTimer += Time.deltaTime;
        }

        private IEnumerator RegenCoroutine()
        {
            while (true)
            {
                if (!canRegen)
                {
                    yield return null;
                    continue;
                }

                yield return new WaitForSeconds(healthRegenRate);

                Heal(healthRegen);
            }
        }

        public override void Heal(int health)
        {
            if (!canRegen) return;

            base.Heal(health);
        }

        public override bool TakeDamage(int dmg, Entity source)
        {
            if (!base.TakeDamage(dmg, source)) return false;

            healthRegenPauseTimer = 0f;
            canRegen = false;
            CameraController.Instance.ShakeCamera(3f, 0.2f);

            return true;
        }

        private void SetupLevel()
        {
            maxEXP = CalculateMaxEXPForLevel(currentLevel);
        }

        private void HandleLevel()
        {
            AddEXP(0);
        }

        public void LevelUp()
        {
            if (currentLevel >= maxLevel) return;

            int overfillEXP = currentEXP >= maxEXP ? currentEXP - maxEXP : currentEXP;

            currentLevel++;
            maxEXP = CalculateMaxEXPForLevel(currentLevel);

            currentEXP = overfillEXP;
        }

        public void AddEXP(int amt)
        {
            currentEXP += amt;

            if(currentEXP >= maxEXP)
            {
                LevelUp();
            }
        }

        private int CalculateMaxEXPForLevel(int level)
        {
            return (int)Mathf.Floor(levelUpAcceleration * level * level) + levelUpLinearGrowth * level + baseEXP;
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
            if (reloadTimer < reloadSpeed) return;

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
            if (reloadTimer < reloadSpeed * 2f) reloadTimer += Time.deltaTime;
        }

        public void ResetFireTimer()
        {
            reloadTimer = reloadSpeed;
        }

        public void FireBullet()
        {
            reloadTimer = 0f;

            Vector2 dir = ((Vector2)(mouseWorldPosition - transform.position)).normalized;

            BulletScript bullet = Instantiate(bulletPrefab, bulletSpawnTransform.position, Quaternion.identity);
            bullet.Shoot(dir, bulletSpeed, bulletDamage, team, this);

            StartCoroutine(ShootCoroutine());
        }

        private IEnumerator ShootCoroutine()
        {
            float startScale = 1f;
            float endScale = 1.2f;

            float duration = 0.15f;

            Vector3 startLocalScale = new Vector3(startScale * startingScale.x, startingScale.y, startingScale.z);
            Vector3 endLocalScale = new Vector3(endScale * startingScale.x, startingScale.y, startingScale.z);

            for(float t = 0; t < duration / 2; t += Time.deltaTime)
            {
                float parameter = EasingFunctions.OutQuint(t / duration / 2f);
                bodySpriteRenderer.transform.parent.localScale = Vector3.Lerp(startLocalScale, endLocalScale, parameter);
                yield return null;
            }
            for (float t = 0; t < duration / 2; t += Time.deltaTime)
            {
                float parameter = EasingFunctions.OutQuint(t / duration / 2f);
                bodySpriteRenderer.transform.parent.localScale = Vector3.Lerp(endLocalScale, startLocalScale, parameter);
                yield return null;
            }

            bodySpriteRenderer.transform.parent.localScale = startingScale;
        }
    }
}

