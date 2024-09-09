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
        private bool onEnemyTeam;

        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody2D>();

            Destroy(gameObject, selfDestroyDuration);
        }

        public void Shoot(Vector2 dir, float shootStrength, int damage, bool onEnemyTeam)
        {
            this.damage = damage;
            this.onEnemyTeam = onEnemyTeam;

            rigidBody.AddForce(shootStrength * dir, ForceMode2D.Impulse);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (onEnemyTeam)
            {
                if (collision.TryGetComponent(out PlayerController player))
                {
                    player.TakeDamage(damage);

                    Destroy(gameObject);
                }
            }
            else
            {
                if (collision.TryGetComponent(out EnemyController enemy))
                {
                    enemy.TakeDamage(damage);

                    Destroy(gameObject);
                }
            }
        }
    }

}
