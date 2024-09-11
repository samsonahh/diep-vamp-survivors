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
        }

        protected void MoveTowards(Vector3 targetPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.fixedDeltaTime);
        }

        public virtual void FollowTarget(Entity target)
        {
            if (target == null) return;

            MoveTowards(target.transform.position);
        }

        protected List<Entity> GetNearbyEntities(float radius)
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

        protected Entity GetClosestEntity(float radius)
        {
            List<Entity> nearbyEntities = GetNearbyEntities(radius);

            if (nearbyEntities.Count == 0) return null;
            if(nearbyEntities.Count == 1) return nearbyEntities[0];

            int closestEntityIndex = 0;
            for(int i = 0; i <  nearbyEntities.Count; i++)
            {
                float closestDistance = GetDistanceFromEntity(nearbyEntities[closestEntityIndex]);
                float newDistance = GetDistanceFromEntity(nearbyEntities[i]);

                if (newDistance < closestDistance) closestEntityIndex = i;
            }

            return nearbyEntities[closestEntityIndex];
        }

        protected virtual void AssignTarget(Entity target)
        {
            if (target == null) return;
            currentTarget = target;
        } 
    }

}
