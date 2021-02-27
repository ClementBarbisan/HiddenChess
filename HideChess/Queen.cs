namespace Game
{
    public class Queen : Piece
    {
        public Queen() : base()
        {
            PieceType = Type.Queen;
            Position = new Vector2Int(rand.Next(2, Wrapper.WIDTH - 1), rand.Next(2, Wrapper.HEIGHT - 1));
            initialPos = Position;
        }

        public override void Move(HideChess.Case[,] map)
        {
            map[Position.x, Position.y].value = (int)PieceType;
        }
    }
}