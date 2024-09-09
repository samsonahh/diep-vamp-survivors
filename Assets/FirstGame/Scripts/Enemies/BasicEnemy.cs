using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirstGameProg2Game
{
    public class BasicEnemy : EnemyController
    {
        private States currentState;

        [Header("State Machine")]
        [SerializeField] private float shootDistance = 1f;

        public enum States
        {
            Chase,
            Shoot
        }

        protected override void OnStart()
        {
            base.OnStart();

            currentState = States.Chase;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            LookAt(player.transform.position);

            SMUpdate();
        }

        protected override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            SMFixedUpdate();
        }

        private void SMUpdate()
        {
            switch (currentState)
            {
                case States.Chase:

                    if (distanceFromPlayer < shootDistance) SMChangeState(States.Shoot);

                    break;
                case States.Shoot:
                    break;
                default:
                    break;
            }
        }

        private void SMFixedUpdate()
        {
            switch (currentState)
            {
                case States.Chase:

                    transform.position = Vector3.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.fixedDeltaTime);

                    break;
                case States.Shoot:
                    break;
                default:
                    break;
            }
        }

        private void SMChangeState(States newState)
        {
            SMOnStateExit(currentState);
            currentState = newState;
            SMOnStateEnter(newState);
        }

        private void SMOnStateEnter(States newState)
        {
            switch (newState)
            {
                case States.Chase:
                    break;
                case States.Shoot:
                    break;
                default:
                    break;
            }
        }

        private void SMOnStateExit(States oldState)
        {
            switch (oldState)
            {
                case States.Chase:
                    break;
                case States.Shoot:
                    break;
                default:
                    break;
            }
        }
    }
}
