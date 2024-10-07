using System.Numerics;

namespace FirstGameProg2Game
{
    public class Dummy : Enemy
    {
        protected override void OnUpdate()
        {
            base.OnUpdate();
            
            AssignTarget(GetClosestEntity(GetNearbyEntities(5, team, true)));

            LookAt(currentTarget);
        }
    }
}
