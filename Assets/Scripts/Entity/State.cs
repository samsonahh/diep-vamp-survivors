using System.Collections;
using UnityEngine;

namespace FirstGameProg2Game
{
    public abstract class State
    {
        public State(Entity entity)
        {
            
        }

        public abstract void Enter();
        public abstract void Update();
        public abstract void FixedUpdate();
        public abstract void Exit();
    }
}
