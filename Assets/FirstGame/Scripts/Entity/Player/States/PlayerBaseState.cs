namespace FirstGameProg2Game
{
    public abstract class PlayerBaseState : State
    {
        private protected Player player;

        public PlayerBaseState(Player player) : base(player)
        {
            this.player = player;
        }
    }
}
