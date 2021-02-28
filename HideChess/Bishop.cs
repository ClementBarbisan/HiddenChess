using System;

namespace Game
{
    public class Bishop : Piece
    {
        public Bishop() : base()
        {
           PieceType = Type.Bishop;
        }

        public override void Move(HideChess.Case[,]map)
        {
            initialPos = new Vector2Int(Position.x, Position.y);
            map[initialPos.x, initialPos.y].value = 0;
            int tmp = 0;
            if (followKing)
            {
                Vector2Int direction = new Vector2Int(kingPosition.x - initialPos.x,
                    kingPosition.y - initialPos.y);
                int tmpKing = Math.Min(Math.Abs(direction.x), Math.Abs(direction.y));
                if (direction.x >= 0)
                {
                    Position.x += tmpKing;
                }
                else
                {
                    Position.x -= tmpKing;
                }
                if (direction.y >= 0)
                {
                    Position.y += tmpKing;
                }
                else
                {
                    Position.y -= tmpKing;
                }
            }
            else if (Position.x > 0 && Position.y < MoveCapacity.y)
            {
                tmp = rand.Next(1, Math.Min( Math.Min(MoveCapacity.x, Position.x), MoveCapacity.y - Position.y));
                Position.x -= tmp;
                Position.y += tmp;
            }
            else if (Position.x < MoveCapacity.x && Position.y < MoveCapacity.y)
            {
                tmp = rand.Next(1, Math.Min(MoveCapacity.x - Position.x, MoveCapacity.y - Position.y));
                Position.x += tmp;
                Position.y += tmp;
            }
           
            else if (Position.y > 0 && Position.x < MoveCapacity.x)
            {
                tmp = rand.Next(1, Math.Min( Math.Min(MoveCapacity.y, Position.y), MoveCapacity.x - Position.x));
                Position.x += tmp;
                Position.y -= tmp;
            }
            else if (Position.x > 0 && Position.y > 0)
            {
                tmp = rand.Next(1, Math.Min( Math.Min(MoveCapacity.y, Position.y), Math.Min(MoveCapacity.x, Position.x)));
                Position.x -= tmp;
                Position.y -= tmp;
            }

            base.Move(map);
            return;
        }
    }
}