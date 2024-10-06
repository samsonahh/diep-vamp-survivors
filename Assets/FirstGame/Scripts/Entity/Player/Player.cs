using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FirstGameProg2Game
{
    public class Player : Entity
    {
        [Header("Player: References")]
        [SerializeField] private Transform bulletSpawnTransform;
        [SerializeField] private BulletScript bulletPrefab;

        [Header("Player: Level Settings")]
        [SerializeField] private Slider expSlider;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private int currentLevel = 1;
        [SerializeField] private int maxLevel = 64;
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
        [SerializeField] private protected float iFrameDuration = 0.5f;
        private float iFrameTimer = Mathf.Infinity;

        [Header("Player: Upgrades UI")]
        [SerializeField] private Slider healthRegenSlider;
        [SerializeField] private Slider maxHealthSlider;
        [SerializeField] private Slider bodyDamageSlider;
        [SerializeField] private Slider bulletSpeedSlider;
        [SerializeField] private Slider bulletDamageSlider;
        [SerializeField] private Slider reloadSpeedSlider;
        [SerializeField] private Slider movementSpeedSlider;
        [SerializeField] private TMP_Text upgradesCountText;

        [Header("Player: Upgrades")]
        [SerializeField] private int availableUpgrades = 0;
        private int maxUpgrades = 9;
        [SerializeField] private int healthRegenUpgrades = 0;
        [SerializeField] private int maxHealthUpgrades = 0;
        [SerializeField] private int bodyDamageUpgrades = 0;
        [SerializeField] private int bulletSpeedUpgrades = 0;
        [SerializeField] private int bulletDamageUpgrades = 0;
        [SerializeField] private int reloadSpeedUpgrades = 0;
        [SerializeField] private int movementSpeedUpgrades = 0;

        [Header("Player: Upgrade Add/Mult Amounts")]
        [SerializeField] private int healthRegenAddAmount = 1;
        [SerializeField] private int maxHealthAddAmount = 50;
        [SerializeField] private int bodyDamageAddAmount = 10;
        [SerializeField] private float bulletSpeedAddAmount = 5f;
        [SerializeField] private float bulletDamageMultiplyAmount = 1.26f;
        [SerializeField] private float reloadSpeedMultiplyAmount = 0.85f;
        [SerializeField] private float movementSpeedMultiplyAmount = 1.1f;

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

            HandleUpgradeButtonsInput();

            HandleIFrames();

            HandleLevel();
            HandleUpgradesUI();
        }

        public override void Die(Entity s)
        {
            base.Die(s);
        }

        protected override void OnDeath()
        {
            base.OnDeath();

            GameManager.Instance.ChangeState(GameState.END);
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

        private void HandleIFrames()
        {
            if (iFrameTimer < iFrameDuration * 2f) iFrameTimer += Time.deltaTime;
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

        public override void TakeDamage(int dmg, Entity source)
        {
            if (iFrameTimer < iFrameDuration) return;

            iFrameTimer = 0f;

            base.TakeDamage(dmg, source);

            healthRegenPauseTimer = 0f;
            canRegen = false;
            CameraController.Instance.ShakeCamera(1.5f, 0.2f);
        }

        private void SetupLevel()
        {
            maxEXP = CalculateMaxEXPForLevel(currentLevel);
        }

        private void HandleLevel()
        {
            AddEXP(0);

            float targetSliderValue = currentEXP / (float)maxEXP;

            levelText.text = $"Lvl {currentLevel}";
            expSlider.value = Mathf.Lerp(expSlider.value, targetSliderValue, 10f * Time.unscaledDeltaTime);
        }

        public void LevelUp()
        {
            if (currentLevel >= maxLevel) return;

            int overfillEXP = currentEXP >= maxEXP ? currentEXP - maxEXP : currentEXP;

            currentLevel++;
            maxEXP = CalculateMaxEXPForLevel(currentLevel);

            currentEXP = overfillEXP;

            availableUpgrades++;
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

        public void HandleUpgradeButtonsInput()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) AddHealthRegen();
            if (Input.GetKeyDown(KeyCode.Alpha2)) AddMaxHealth();
            if (Input.GetKeyDown(KeyCode.Alpha3)) AddBodyDamage();
            if (Input.GetKeyDown(KeyCode.Alpha4)) AddBulletSpeed();
            if (Input.GetKeyDown(KeyCode.Alpha5)) AddBulletDamage();
            if (Input.GetKeyDown(KeyCode.Alpha6)) AddReloadSpeed();
            if (Input.GetKeyDown(KeyCode.Alpha7)) AddMovementSpeed();
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

            int randomDamage = CalculateRandomDamage(bulletDamage);

            BulletScript bullet = Instantiate(bulletPrefab, bulletSpawnTransform.position, Quaternion.identity);
            bullet.Shoot(dir, bulletSpeed, randomDamage, team, this);

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

        private void OnCollisionStay2D(Collision2D collision)
        {
            CheckEntityCollision(collision);
        }

        private void CheckEntityCollision(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent(out Enemy enemy))
            {
                if (enemy.Team == team) return;
                if (!enemy.CanTakeBodyDamage()) return;

                int randomDamage = CalculateRandomDamage(bodyDamage);

                enemy.TakeDamage(randomDamage, this);
                enemy.ResetBodyDamageTimer();

                CameraController.Instance.ShakeCamera(1.5f, 0.2f);
            }
        }

        private void HandleUpgradesUI()
        {
            upgradesCountText.gameObject.SetActive(availableUpgrades > 0);
            upgradesCountText.text = $"x{availableUpgrades}";

            float targetHealthRegenFraction = healthRegenUpgrades / (float)maxUpgrades;
            healthRegenSlider.value = Mathf.Lerp(healthRegenSlider.value, targetHealthRegenFraction, 10f * Time.unscaledDeltaTime);

            float targetMaxHealthFraction = maxHealthUpgrades / (float)maxUpgrades;
            maxHealthSlider.value = Mathf.Lerp(maxHealthSlider.value, targetMaxHealthFraction, 10f * Time.unscaledDeltaTime);

            float targetBodyDamageFraction = bodyDamageUpgrades / (float)maxUpgrades;
            bodyDamageSlider.value = Mathf.Lerp(bodyDamageSlider.value, targetBodyDamageFraction, 10f * Time.unscaledDeltaTime);

            float targetBulletSpeedFraction = bulletSpeedUpgrades / (float)maxUpgrades;
            bulletSpeedSlider.value = Mathf.Lerp(bulletSpeedSlider.value, targetBulletSpeedFraction, 10f * Time.unscaledDeltaTime);

            float targetBulletDamageFraction = bulletDamageUpgrades / (float)maxUpgrades;
            bulletDamageSlider.value = Mathf.Lerp(bulletDamageSlider.value, targetBulletDamageFraction, 10f * Time.unscaledDeltaTime);

            float targetReloadSpeedFraction = reloadSpeedUpgrades / (float)maxUpgrades;
            reloadSpeedSlider.value = Mathf.Lerp(reloadSpeedSlider.value, targetReloadSpeedFraction, 10f * Time.unscaledDeltaTime);

            float targetMovementSpeedFraction = movementSpeedUpgrades / (float)maxUpgrades;
            movementSpeedSlider.value = Mathf.Lerp(movementSpeedSlider.value, targetMovementSpeedFraction, 10f * Time.unscaledDeltaTime);

        }

        public void AddHealthRegen()
        {
            if (availableUpgrades <= 0) return;
            if (healthRegenUpgrades >= maxUpgrades) return;

            healthRegenUpgrades++;
            healthRegen+=healthRegenAddAmount;
            availableUpgrades--;
        }

        public void AddMaxHealth()
        {
            if (availableUpgrades <= 0) return;
            if (maxHealthUpgrades >= maxUpgrades) return;

            maxHealthUpgrades++;
            MaxHealth += maxHealthAddAmount;
            availableUpgrades--;
        }

        public void AddBodyDamage()
        {
            if (availableUpgrades <= 0) return;
            if (bodyDamageUpgrades >= maxUpgrades) return;

            bodyDamageUpgrades++;
            bodyDamage += bodyDamageAddAmount;
            availableUpgrades--;
        }

        public void AddBulletSpeed()
        {
            if (availableUpgrades <= 0) return;
            if (bulletSpeedUpgrades >= maxUpgrades) return;

            bulletSpeedUpgrades++;
            bulletSpeed += bulletSpeedAddAmount;
            availableUpgrades--;
        }

        public void AddBulletDamage()
        {
            if (availableUpgrades <= 0) return;
            if (bulletDamageUpgrades >= maxUpgrades) return;

            bulletDamageUpgrades++;
            bulletDamage = (int)Mathf.Ceil(bulletDamage * bulletDamageMultiplyAmount);
            availableUpgrades--;
        }

        public void AddReloadSpeed()
        {
            if (availableUpgrades <= 0) return;
            if (reloadSpeedUpgrades >= maxUpgrades) return;

            reloadSpeedUpgrades++;
            reloadSpeed *= reloadSpeedMultiplyAmount;
            availableUpgrades--;
        }

        public void AddMovementSpeed()
        {
            if (availableUpgrades <= 0) return;
            if (movementSpeedUpgrades >= maxUpgrades) return;

            movementSpeedUpgrades++;
            moveSpeed *= movementSpeedMultiplyAmount;
            availableUpgrades--;
        }
    }
}

