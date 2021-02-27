using System;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using Sifteo;

namespace Game
{
    public enum Type
    {
        Rook = 4,
        Bishop = 5,
        Knight = 6
    }
    public class Piece
    {
        protected  Type PieceType { get; set; }
        protected Vector2Int MoveCapacity { get; set; }
        public Vector2Int Position { get; set; }
        protected Vector2Int initialPos;
        protected Random rand;
        public bool followKing = false;
        public Vector2Int kingPosition;

        public Piece()
        {
            rand = new Random(GetHashCode());
            MoveCapacity = new Vector2Int(Wrapper.WIDTH - 1, Wrapper.HEIGHT - 1);
            Position = new Vector2Int(rand.Next(1, Wrapper.WIDTH - 1), rand.Next(1, Wrapper.HEIGHT - 1));
            initialPos = Position;
        }

        // public HideChess.Case[,] Set(HideChess.Case[,] map)
        // {
        //     map[initialPos.x, initialPos.y].value = (int) PieceType;
        //     return (map);
        // }

        public virtual HideChess.Case[,] Move(HideChess.Case[,] map)
        {
            map[initialPos.x, initialPos.y].marks = 1f;
            map[initialPos.x, initialPos.y].value = (int)PieceType;
            if (Math.Min(Math.Max(0, Position.x), Wrapper.WIDTH - 1) != Position.x ||
                Math.Min(Math.Max(0, Position.y), Wrapper.HEIGHT - 1) != Position.y)
                return (map);
            if (map[Position.x, Position.y].isKing)
            {
                map[Position.x, Position.y].value = 0;
                map[Position.x, Position.y].isKing = false;
                HideChess.getKing = true;
            }
            if (map[Position.x, Position.y].value != 0)
            {
                Position = initialPos;
            }
            map[initialPos.x, initialPos.y].value = 0;
            map[Position.x, Position.y].value = (int)PieceType;
            int dist = Math.Max(Math.Abs(Position.x - initialPos.x), Math.Abs(Position.y - initialPos.y)) + 1;
            float initialMark = 1f / dist;
            float currentMark = initialMark;
            while (initialPos.x != Position.x || initialPos.y != Position.y)
            {
                Log.Debug("Distance Bisho x = " + (Position.x - initialPos.x) + ", distance bishop y = " + (Position.y - initialPos.y));
                map[initialPos.x, initialPos.y].marks = Math.Min(1f, currentMark);
                if (Position.x - initialPos.x != 0)
                    initialPos.x += (Position.x - initialPos.x) / Math.Abs(Position.x - initialPos.x);
                if (Position.y - initialPos.y != 0)
                    initialPos.y += (Position.y - initialPos.y) / Math.Abs(Position.y - initialPos.y);
                currentMark += initialMark;
            }
            map[Position.x, Position.y].marks = 1f;
            return (map);
        }
    }
}