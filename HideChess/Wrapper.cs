﻿
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Security.Cryptography;
using Sifteo;
using Sifteo.MathExt;


namespace Game {

  
  class Wrapper {

    public static int WIDTH = 4;   
    public static int HEIGHT = 4;   
    internal static readonly Color[] BackgroundColors = {new Color(75, 75, 75), new Color(150, 150, 150), new Color(0, 0, 255),
      new Color(255, 0, 0), new Color(255, 255, 0), new Color(0, 255, 255) , new Color(0, 255, 0)};
    public Vector2Int mPos;
    public bool attachToKing = false;
    internal Cube mCube { get; private set; }
    internal bool isKing = false;
    internal bool kingMove = false;
    internal HideChess.Case[,] Map { get; set; }

    public bool endOfTurn = false;
    internal Wrapper(Cube cube, string id, HideChess.Case[,] map) {
      this.mCube = cube;
      Random random = new Random(mCube.GetHashCode());
      Map = map;
      mPos = new Vector2Int(0, 0);
      if (id == mCube.UniqueId)
      {
        isKing = true;
       
        mPos = new Vector2Int(random.Next(WIDTH), random.Next(HEIGHT));
        Paint();
      }

      mCube.NeighborAddEvent += OnNeighborAdd;
      mCube.NeighborRemoveEvent += this.OnNeighborRemove;
      mCube.ButtonEvent += OnButtonPressed;
      mCube.FillScreen(Color.Black);
    }
    internal void OnLostCube() {  }

    internal void OnButtonPressed(Cube c, bool pressed)
    {
      if (isKing && pressed)
        kingMove = true;
      if (!isKing && pressed)
      {
        for (Cube.Side side = Cube.Side.TOP; side <= Cube.Side.RIGHT; ++side)
        {
          if (mCube.Neighbors[side] != null)
          {
            Wrapper data = (Wrapper) mCube.Neighbors[side].userData;
            if (data.isKing && data.kingMove && mPos.x <= WIDTH && mPos.x >= 0
                && mPos.y >= 0 && mPos.y <= HEIGHT)
            {
              isKing = true;
              data.isKing = false;
              data.kingMove = false;
              endOfTurn = true;
              if (Map[mPos.x, mPos.y].value > 2)
              {
                for (int i = 0; i < HideChess.opponents.Count; i++)
                {
                  if (HideChess.opponents[i].Position.x == mPos.x && HideChess.opponents[i].Position.y == mPos.y)
                  {
                    HideChess.opponents.RemoveAt(i);
                    HideChess.opponents[i].Dispose();
                    HideChess.opponents[i] = null;
                  }
                }
              }

              // HideChess.getKing = true;
              return;
            }
            for (Cube.Side secondSide = Cube.Side.TOP; secondSide <= Cube.Side.RIGHT; ++secondSide)
            {
              if (mCube.Neighbors[side].Neighbors[secondSide] == null)
                continue;
              data = (Wrapper) mCube.Neighbors[side].Neighbors[secondSide].userData;
              if (data == null || Math.Abs(mPos.x - data.mPos.x) > 1 || Math.Abs(mPos.y - data.mPos.y) > 1)
                continue;
              if (data.isKing && data.kingMove && mPos.x <= WIDTH && mPos.x >= 0
                  && mPos.y >= 0 && mPos.y <= HEIGHT)
              {
                isKing = true;
                data.isKing = false;
                data.kingMove = false;
                endOfTurn = true;
                if (Map[mPos.x, mPos.y].value > 2)
                {
                  for (int i = 0; i < HideChess.opponents.Count; i++)
                  {
                    if (HideChess.opponents[i].Position.x == mPos.x && HideChess.opponents[i].Position.y == mPos.y)
                    {
                      Map[mPos.x, mPos.y].value = 0;
                      Map[mPos.x, mPos.y].marks = 0;
                      Piece tmp = HideChess.opponents[i];
                      HideChess.opponents.RemoveAt(i);
                      tmp.Dispose();
                      tmp = null;
                      return;
                    }
                  }
                }
                return;
              }
            }
          }
        }

      }
    }

    void OnChangeCubePosition(Cube c, Cube.Side s, Cube neighbor, Cube.Side neighborSide)
    {
      if (!isKing && !attachToKing)
        return;
      bool mainNeighbor = false;
      for (Cube.Side side = Cube.Side.TOP; side <= Cube.Side.RIGHT; ++side)
      {
        if (mCube.Neighbors[side] != null)
        {
          switch (side)
          {
            case Cube.Side.TOP:
            {
              Wrapper data = (Wrapper) mCube.Neighbors[Cube.Side.TOP].userData;
              if (data == null)
                break;
              if (data.isKing || data.attachToKing)
              {
                  mainNeighbor = true;
                  break;
              }
              data.mPos = new Vector2Int(mPos.x, mPos.y + 1);
              data.Paint();
              data.attachToKing = true;
              break;
            }
            case Cube.Side.LEFT:
            {
              Wrapper data = (Wrapper) mCube.Neighbors[Cube.Side.LEFT].userData;
              if (data == null)
                break;
              if (data.isKing || data.attachToKing)
              {
                  mainNeighbor = true;
                  break;
              }
              data.mPos = new Vector2Int(mPos.x - 1, mPos.y);
              data.Paint();
              data.attachToKing = true;
              break;
            }
            case Cube.Side.RIGHT:
            {
              Wrapper data = (Wrapper) mCube.Neighbors[Cube.Side.RIGHT].userData;
              if (data == null)
                break;
              if (data.isKing || data.attachToKing)
              {
                  mainNeighbor = true;
                  break;
              }
              data.mPos = new Vector2Int(mPos.x + 1, mPos.y);
              data.Paint();
              data.attachToKing = true;
              break;
            }
            case Cube.Side.BOTTOM:
            {
              Wrapper data = (Wrapper) mCube.Neighbors[Cube.Side.BOTTOM].userData;
              if (data == null)
                break;
              if (data.isKing || data.attachToKing)
              {
                  mainNeighbor = true;
                  break;
              }
              data.mPos = new Vector2Int(mPos.x, mPos.y - 1);
              data.Paint();
              data.attachToKing = true;
              break;
            }
          }
        }
      }
      attachToKing = mainNeighbor;
    }
    void OnNeighborRemove(Cube c, Cube.Side s, Cube neighbor, Cube.Side neighborSide)
    {
      OnChangeCubePosition(c, s, neighbor, neighborSide);
    }

    void OnNeighborAdd(Cube c, Cube.Side s, Cube neighbor, Cube.Side neighborSide)
    {
      OnChangeCubePosition(c, s, neighbor, neighborSide);
    }

    internal void Tick(float dt, HideChess.Case[,] map)
    {
      Map = map;
      Paint();
    }



    internal void Paint() {
      if (mPos.x < 0 || mPos.x >= WIDTH|| mPos.y >= HEIGHT|| mPos.y < 0)
        this.mCube.FillScreen(BackgroundColors[2]);
      else
      {
        Color color;
        if ((mPos.x + mPos.y) % 2 == 0)
          color = new Color(Math.Min(25 + Mathf.FloorToInt(125 * Map[mPos.x, mPos.y].marks), 255), 25, 25);
        else
          color = new Color(Math.Min(75 + Mathf.FloorToInt(200 * Map[mPos.x, mPos.y].marks), 255), 75, 75);
        this.mCube.FillScreen(color);
        if (isKing)
        {
          mCube.Image(HideChess.images["king"].name);
        }
        else if (Map[mPos.x, mPos.y].value > 3)
        {
          foreach (Piece piece in HideChess.opponents)
          {
            if (mPos.x == piece.Position.x && mPos.y == piece.Position.y)
              piece.followKing = true;
          }

          if (Map[mPos.x, mPos.y].value == (int) Type.Bishop)
          {
            mCube.Image(HideChess.images["bishop"].name);
          }
          else if (Map[mPos.x, mPos.y].value == (int) Type.Rook)
          {
            mCube.Image(HideChess.images["rook"].name);
          }
        }
      }

      this.mCube.Paint();
    }

  }

  public class Vector2Int
  {
    
    public int x;
    public int y;

    public Vector2Int(int _x, int _y)
    {
      x = _x;
      y = _y;
    }
  }
}
