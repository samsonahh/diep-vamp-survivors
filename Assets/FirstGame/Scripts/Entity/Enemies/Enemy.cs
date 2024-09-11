using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirstGameProg2Game
{
    public class Enemy : Entity
    {
        private protected Entity currentTarget;
        public Entity CurrentTarget => currentTarget;

        private protected float distanceFromTarget => Vector3.Distance(currentTarget.transform.position, transform.position);

        protected override void OnAwake()
        {
            base.OnAwake();
        }

        protected override void OnStart()
        { 
            base.OnStart();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
        }

        protected override void OnDeath()
        {
            base.OnDeath();

            Destroy(gameObject);
        }

        protected virtual List<Entity> GetNearbyEntities(float radius)
        {
            List<Entity> result = new List<Entity>();

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

            foreach(Collider2D hit in hits)
            {
                Entity entity = hit.GetComponent<Entity>();
                entity = entity == null ? hit.GetComponentInParent<Entity>() : entity;

                if (entity == null) continue;
                
                result.Add(entity);
            }

            return result;
        }
    }

}
