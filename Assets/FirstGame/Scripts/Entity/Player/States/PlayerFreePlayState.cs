namespace FirstGameProg2Game
{
    public class PlayerFreePlayState : State
    {
        private Player player;

        public PlayerFreePlayState(Player player) : base(player)
        {
            this.player = player;
        }

        public override void Enter()
        {
            player.ResetFireTimer();
        }

        public override void Exit()
        {
            
        }

        public override void FixedUpdate()
        {
            player.HandleMovement();
        }

        public override void Update()
        {
            player.HandleMovementInput();
            player.HandleMousePositionInput();
            player.HandleMouseClickInput();

            player.HandleRotation();

            player.HandleFireRateTimer();
        }
    }
}
