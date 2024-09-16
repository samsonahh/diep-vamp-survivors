using UnityEngine;

namespace FirstGameProg2Game
{
    public class EnemyChaseState : EnemyBaseState
    {
        private float chaseRadius = Mathf.Infinity;

        public EnemyChaseState(Enemy enemy, float chaseRadius) : base(enemy)
        {
            this.chaseRadius = chaseRadius;
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

            enemy.LookAt(enemy.CurrentTarget);
        }
    }
}
