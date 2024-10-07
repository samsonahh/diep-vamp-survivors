using UnityEngine;

namespace FirstGameProg2Game
{
    public class ResourceEntity : Entity
    {
        [Header("Resource: Settings")]
        [SerializeField] private int deathEXPDrop = 5;
        [SerializeField] private int contactDamage = 5;

        protected override void OnAwake()
        {
            base.OnAwake();

            SetTeam(1);
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            HandleIncomingBodyDamage();
        }

        public override void Die(Entity killer)
        {
            base.Die(killer);

            Player player = killer as Player;
            if (player == null) return;

            player.AddEXP(deathEXPDrop);
        }

        protected override void OnDeath()
        {
            base.OnDeath();

            if (GameManager.Instance != null) GameManager.Instance.AddScore(deathEXPDrop);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            CheckEntityCollision(collision);
        }

        private void CheckEntityCollision(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent(out Entity entity))
            {
                if (entity.Team == team) return;

                int randomDamage = CalculateRandomDamage(contactDamage);

                entity.TakeDamage(randomDamage, this);
            }
        }
    }
}
