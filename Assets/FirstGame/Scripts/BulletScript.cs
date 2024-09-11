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

        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody2D>();

            Destroy(gameObject, selfDestroyDuration);
        }

        public void Shoot(Vector2 dir, float shootStrength, int damage, int team)
        {
            this.damage = damage;
            this.team = team;

            rigidBody.AddForce(shootStrength * dir, ForceMode2D.Impulse);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Entity entity))
            {
                if (entity.Team != team) return;

                entity.TakeDamage(damage);

                Destroy(gameObject);
            }
        }
    }

}
