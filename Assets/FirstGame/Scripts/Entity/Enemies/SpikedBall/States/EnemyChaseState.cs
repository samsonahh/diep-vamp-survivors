using UnityEngine;

namespace FirstGameProg2Game
{
    public class EnemyChaseState : EnemyBaseState
    {
        private float chaseRadius = Mathf.Infinity;
        private bool lookAt;

        public EnemyChaseState(Enemy enemy, float chaseRadius, bool lookAt) : base(enemy)
        {
            this.chaseRadius = chaseRadius;
            this.lookAt = lookAt;
        }

        public override void Enter()
        {
            
        }

        public override void Exit()
        {

        }

        public override void FixedUpdate()
        {
            enemy.FollowTarget(enemy.CurrentTarget);
        }

        public override void Update()
        {
            enemy.AssignTarget(enemy.GetClosestEntity(enemy.GetNearbyEntities(chaseRadius, enemy.Team, true)));

            if(lookAt) enemy.LookAt(enemy.CurrentTarget);
        }
    }
}
