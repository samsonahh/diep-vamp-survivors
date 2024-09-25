using UnityEngine;

namespace FirstGameProg2Game
{
    public class ResourceEntity : Entity
    {
        [Header("Resource: Settings")]
        [SerializeField] private int deathEXPDrop = 5;

        protected override void OnAwake()
        {
            base.OnAwake();

            SetTeam(1);
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
        }
    }
}
