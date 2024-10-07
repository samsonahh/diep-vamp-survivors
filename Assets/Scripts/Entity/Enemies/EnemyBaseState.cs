namespace FirstGameProg2Game
{
    public abstract class EnemyBaseState : State
    {
        private protected Enemy enemy;

        protected EnemyBaseState(Enemy enemy) : base(enemy)
        {
            this.enemy = enemy;
        }
    }
}
