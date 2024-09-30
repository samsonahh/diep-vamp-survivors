using UnityEngine;

namespace FirstGameProg2Game
{
    public class SpikedBallEnemy : Enemy
    {
        [Header("Spiked Ball Enemy: Settings")]
        [SerializeField] private int contactDamage = 10;
        [SerializeField] private float chaseRadius = Mathf.Infinity;

        public EnemyChaseState EnemyChaseState { get; private set; }

        protected override void OnAwake()
        {
            base.OnAwake();
        }

        protected override void OnStart()
        {
            base.OnStart();

            StartWithState(EnemyChaseState);
            SetDefaultState(EnemyChaseState);
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            bodySpriteRenderer.transform.parent.Rotate(500f * Time.deltaTime * new Vector3(0, 0, 1));
        }

        protected override void SetupStates()
        {
            base.SetupStates();

            EnemyChaseState = new EnemyChaseState(this, chaseRadius, false);
        }

        public override void Die(Entity s)
        {
            base.Die(s);
        }

        protected override void OnDeath()
        {
            base.OnDeath();
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

                entity.TakeDamage(contactDamage, this);
            }
        }
    }
}
