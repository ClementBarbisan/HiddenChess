using System;

namespace Game
{
    public class Pawn : Piece
    {
        public Pawn() : base()
        {
            PieceType = Type.Pawn;
            int tmpX = rand.Next(0, 1);
            MoveCapacity = new Vector2Int(tmpX, 1 - tmpX);
        }

        public override void Move(HideChess.Case[,] map)
        {
            initialPos = new Vector2Int(Position.x, Position.y);
            map[initialPos.x, initialPos.y].value = 0;
            if (followKing)
            {
                Vector2Int direction = new Vector2Int(kingPosition.x - initialPos.x,
                    kingPosition.y - initialPos.y);
                if (Math.Abs(direction.x) == 1 && Math.Abs(direction.y) == 1)
                {
                    Position.x += direction.x;
                    Position.y += direction.y;
                }
                else if (direction.x != 0 && MoveCapacity.x > 0 && Math.Abs(direction.x) >= Math.Abs(direction.y))
                {
                    Position.x += MoveCapacity.x;
                }
                else if (direction.y != 0)
                {
                    Position.y += MoveCapacity.y;
                }
            }
            else if (Position.x < MoveCapacity.x)
            {
                Position.x += MoveCapacity.x;
            }
            else if(Position.y < MoveCapacity.y)
            {
                Position.y += MoveCapacity.y;
            }
            else if (Position.x > 0 && MoveCapacity.x > 0)
            {
                Position.x -= MoveCapacity.x;
            }
            else if (Position.y > 0 && MoveCapacity.y > 0)
            {
                Position.y -= MoveCapacity.y;
            }
            base.Move(map);
            return;
        }
    }
}