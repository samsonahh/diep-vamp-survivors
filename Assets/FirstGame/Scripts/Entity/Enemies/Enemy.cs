﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirstGameProg2Game
{
    public class Enemy : Entity
    {
        [Header("Enemy: Settings")]
        [SerializeField] private int deathEXPDrop = 5;

        private protected Entity currentTarget;
        public Entity CurrentTarget => currentTarget;

        private protected float distanceFromTarget => Vector3.Distance(currentTarget.transform.position, transform.position);

        protected override void OnAwake()
        {
            base.OnAwake();

            SetTeam(1);
        }

        protected override void OnStart()
        { 
            base.OnStart();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
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

        protected void MoveTowards(Vector3 targetPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.fixedDeltaTime);
        }

        public virtual void FollowTarget(Entity target)
        {
            if (target == null) return;

            MoveTowards(target.transform.position);
        }

        public virtual void AssignTarget(Entity target)
        {
            if (target == null) return;
            currentTarget = target;
        } 
    }
}
