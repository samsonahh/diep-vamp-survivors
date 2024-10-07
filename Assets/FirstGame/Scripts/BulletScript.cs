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
        public int Team => team;
        private bool canHit = true;

        private Entity source;

        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody2D>();
        }

        public void Shoot(Vector2 dir, float shootStrength, int damage, int team, Entity source)
        {
            this.damage = damage;
            this.team = team;
            this.source = source;

            rigidBody.AddForce(shootStrength * dir, ForceMode2D.Impulse);

            Destroy(gameObject, selfDestroyDuration);
        }

        public void HomingShoot(Entity target, float bulletSpeed, int damage, int team, Entity source)
        {
            this.damage = damage;
            this.team = team;
            this.source = source;

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

                canHit = false;

                entity.TakeDamage(damage, source);

                Destroy(gameObject);
            }
        }

        private void CheckOtherBulletCollision(Collider2D collision)
        {
            if (collision.TryGetComponent(out BulletScript bullet))
            {
                if (!canHit) return;
                if (bullet.Team == team) return;

                canHit = false;

                Destroy(bullet.gameObject);
                Destroy(gameObject);
            }
        }

        private void CheckWallCollision(Collider2D collision)
        {
            if (collision.gameObject.tag == "Wall")
            {
                Destroy(gameObject);
            }
        }
    }

}
