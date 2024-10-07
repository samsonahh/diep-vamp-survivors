using UnityEngine;

namespace FirstGameProg2Game
{
    public class EntityStunnedState : State
    {
        private Entity entity;

        private float stunDuration;
        private float stunTimer;

        public EntityStunnedState(Entity entity) : base(entity)
        {
            this.entity = entity;
        }

        public override void Enter()
        {
            stunTimer = 0;
        }

        public override void Exit()
        {
            
        }

        public override void FixedUpdate()
        {
           
        }

        public override void Update()
        {
            stunTimer += Time.deltaTime;

            if(stunTimer > stunDuration)
            {
                entity.ChangeState(entity.DefaultState);
            }
        }

        public void SetStunDuration(float duration)
        {
            stunDuration = duration;
        }
    }
}
