using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirstGameProg2Game
{
    public class BulletScript : MonoBehaviour
    {
        private Rigidbody2D rigidBody;

        [Header("Settings")]
        [SerializeField] private float selfDestroyDuration = 10f;
        private int damage;
        private int team;
        private bool piercing;
        public int Team => team;
        private bool canHit = true;

        private Entity source;

        private Vector3 startScale;
        private bool deathCoroutineStarted;

        private List<Entity> hitEnitities = new List<Entity>();

        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            startScale = transform.localScale;
        }

        private void Update()
        {

        }

        public void Shoot(Vector2 dir, float shootStrength, int damage, bool piercing, int team, Entity source)
        {
            this.damage = damage;
            this.team = team;
            this.source = source;
            this.piercing = piercing;

            rigidBody.AddForce(shootStrength * dir, ForceMode2D.Impulse);

            Destroy(gameObject, selfDestroyDuration);
        }

        public void HomingShoot(Entity target, float bulletSpeed, int damage, bool piercing, int team, Entity source)
        {
            this.damage = damage;
            this.team = team;
            this.source = source;
            this.piercing = piercing;

            StartCoroutine(HomingShootCoroutine(target, bulletSpeed));
        }

        private IEnumerator HomingShootCoroutine(Entity target, float bulletSpeed)
        {
            Vector2 currentShootDirection = ((Vector2)(target.transform.position - transform.position)).normalized;

            bool selfDestruct = false;

            while (true)
            {
                if(target == null)
                {
                    if (!selfDestruct)
                    {
                        selfDestruct = true;
                        Destroy(gameObject, selfDestroyDuration);
                    }

                    transform.Translate(currentShootDirection * Time.fixedDeltaTime);
                }
                else
                {
                    currentShootDirection = ((Vector2)(target.transform.position - transform.position)).normalized;

                    transform.position = Vector3.MoveTowards(transform.position, target.transform.position, bulletSpeed * Time.fixedDeltaTime);
                }
                yield return new WaitForFixedUpdate();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            CheckEntityCollision(collision);

            CheckOtherBulletCollision(collision);

            CheckWallCollision(collision);
        }

        private void CheckEntityCollision(Collider2D collision)
        {
            if (collision.TryGetComponent(out Entity entity))
            {
                if (!canHit) return;
                if (entity.Team == team) return;
                if(hitEnitities.Contains(entity)) return;

                if(!piercing) canHit = false;

                entity.TakeDamage(damage, source);
                hitEnitities.Add(entity);

                if (!piercing) Die();
            }
        }

        private void CheckOtherBulletCollision(Collider2D collision)
        {
            if (collision.TryGetComponent(out BulletScript bullet))
            {
                if (!canHit) return;
                if (bullet.Team == team) return;

                canHit = false;

                bullet.Die();
                Die();
            }
        }

        private void CheckWallCollision(Collider2D collision)
        {
            if (collision.gameObject.tag == "Wall")
            {
                Die();
            }
        }

        public void Die()
        {
            if (!deathCoroutineStarted) StartCoroutine(DeathFadeCoroutine());
        }

        private void OnDeath()
        {

        }

        private IEnumerator DeathFadeCoroutine()
        {
            deathCoroutineStarted = true;

            rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;

            enabled = false;

            float duration = 0.15f;

            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                float parameter = EasingFunctions.OutCubic(t / duration);
                transform.localScale = Vector3.Lerp(startScale, Vector3.zero, parameter);
                yield return null;
            }

            transform.localScale = Vector3.zero;

            OnDeath();

            Destroy(gameObject);
        }
    }

}
