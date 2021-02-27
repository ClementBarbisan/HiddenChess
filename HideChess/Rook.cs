using System;
using Sifteo;

namespace Game
{
    public class Rook : Piece
    {
        public Rook() : base()
        {
            PieceType = Type.Rook;
        }

        public override void Move(HideChess.Case[,] map)
        {
            initialPos = new Vector2Int(Position.x, Position.y);
            map[initialPos.x, initialPos.y].value = 0;
            if (followKing)
            {
                Vector2Int direction = new Vector2Int(kingPosition.x - initialPos.x,
                    kingPosition.y - initialPos.y);
                if (direction.x != 0 && Math.Abs(direction.x) >= Math.Abs(direction.y))
                {
                    Position.x += direction.x;
                }
                else if (direction.y != 0)
                {
                    Position.y += direction.y;
                }
            }
            else if (Position.x < MoveCapacity.x)
            {
                Position.x += rand.Next(1, MoveCapacity.x - Position.x);
            }
            else if(Position.y < MoveCapacity.y)
            {
                Position.y += rand.Next(1, MoveCapacity.y - Position.y);
            }
            else if (Position.x > 0)
            {
                Position.x -= rand.Next(1, Math.Min(MoveCapacity.x, Position.x));
            }
            else if (Position.y > 0)
            {
                Position.y -= rand.Next(1, Math.Min(MoveCapacity.y, Position.y));
            }
            base.Move(map);
            return;
        }
    }
}