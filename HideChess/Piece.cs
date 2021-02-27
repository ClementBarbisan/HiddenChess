using System;
using System.Collections.Generic;
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
    public class Piece : IDisposable
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

        public HideChess.Case[,] Set(HideChess.Case[,] map)
        {
            map[initialPos.x, initialPos.y].value = (int) PieceType;
            return (map);
        }

        public virtual void Move(HideChess.Case[,] map)
        {
            if (Math.Min(Math.Max(0, Position.x), Wrapper.WIDTH - 1) != Position.x ||
                Math.Min(Math.Max(0, Position.y), Wrapper.HEIGHT - 1) != Position.y)
                return;
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
            int dist = Math.Max(Math.Abs(Position.x - initialPos.x), Math.Abs(Position.y - initialPos.y)) + 1;
            float initialMark = 1f / dist;
            float currentMark = initialMark;
            map[initialPos.x, initialPos.y].value = 0;
            List<Vector2Int> listMove = new List<Vector2Int>();
            while (initialPos.x != Position.x || initialPos.y != Position.y)
            {
                map[initialPos.x, initialPos.y].marks = Math.Min(1f, currentMark);
                listMove.Add(new Vector2Int(initialPos.x,initialPos.y));
                if (Position.x - initialPos.x != 0)
                    initialPos.x += (Position.x - initialPos.x) / Math.Abs(Position.x - initialPos.x);
                if (Position.y - initialPos.y != 0)
                    initialPos.y += (Position.y - initialPos.y) / Math.Abs(Position.y - initialPos.y);
                if (map[initialPos.x, initialPos.y].value != 0)
                {
                    Position = listMove[0];
                    foreach (Vector2Int v in listMove)
                        map[v.x, v.y].marks = 0f;
                    map[Position.x, Position.y].value = (int) PieceType;
                    map[Position.x, Position.y].marks = 1f;
                    return;
                }

                currentMark += initialMark;
            }
            map[Position.x, Position.y].value = (int)PieceType;
            map[Position.x, Position.y].marks = 1f;
            return;
        }

        private void ReleaseUnmanagedResources()
        {
            // TODO release unmanaged resources here
        }

        protected virtual void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Piece()
        {
            Dispose(false);
        }
    }
}