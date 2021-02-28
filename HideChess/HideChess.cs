
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sifteo;


namespace Game
{

  public class HideChess : BaseApp
  {
    public static int WIDTH = 9;   
    public static int HEIGHT = 9;   
    public struct Case
    {
      public int value;
      public float marks;
      public bool isKing;
    };

    public static bool getKing = false;
    public static bool getQueen = false;
    public static Vector2Int kingPosition;
    public static List<Piece> opponents;
    internal string idMain = null;
    public static ImageSet images;
    internal Case[,] Map;
    internal Vector2Int initialKingPos;
    internal readonly int[,] initialMap =
    {
      {0,0,0,0,0,0,0,0,7},
      {0,0,0,0,0,0,0,0,0},
      {0,0,0,0,0,0,5,0,0},
      {0,0,0,4,0,0,0,0,0},
      {0,0,0,0,0,0,0,0,0},
      {0,0,5,0,0,0,4,0,0},
      {0,0,0,0,0,0,0,0,0},
      {0,0,0,0,0,0,0,0,0},
      {0,0,0,0,0,0,0,0,9}
    };

    // Here we initialize our app.
    public override void Setup()
    {
      Map = new Case[WIDTH, HEIGHT];
      bool queenAdded = false;
      opponents = new List<Piece>();
      for (int i = 0; i < WIDTH; i++)
      {
        for (int j = 0; j < HEIGHT; j++)
        {
          int tmp = initialMap[i, j];
          if (tmp == (int) Type.Bishop)
          {
            opponents.Add(new Bishop());
            opponents[opponents.Count - 1].Position = new Vector2Int(i, HEIGHT - 1 - j);
          }
          else if (tmp == (int) Type.Rook)
          {
            opponents.Add(new Rook());
            opponents[opponents.Count - 1].Position = new Vector2Int(i, HEIGHT - 1 - j);
          }
          // if (tmp == (int)Type.Knight)
            // opponents.Add(new Knight()); 
          else if (tmp == (int) Type.Queen && !queenAdded)
          {
            opponents.Add(new Queen());
            opponents[opponents.Count - 1].Position = new Vector2Int(i, HEIGHT - 1 - j);
            queenAdded = true;
          }
          Map[i, j] = new Case();
          Map[i, j].value = 0;
          Map[i, j].isKing = false;
          if (tmp == 9)
          {
            initialKingPos = new Vector2Int(i, HEIGHT - 1 - j);
            Map[i, j].isKing = true;
          }
          else if (!queenAdded)
            Map[i, j].value = tmp;
          Map[i, j].marks = 0;
        }
      }
      images = this.Images;
      // opponents = new List<Piece>();
      // opponents.Add(new Bishop());
      // // opponents.Add(new Bishop());
      // opponents.Add(new Rook());
      // // opponents.Add(new Rook());
      // opponents.Add(new Queen());
      this.PauseEvent += OnPause;
      this.UnpauseEvent += OnUnpause;
      this.CubeSet.NewCubeEvent += OnNewCube;
      this.CubeSet.LostCubeEvent += OnLostCube;
      foreach (Cube c in CubeSet)
      {
        OnNewCube(c);
      }

      foreach (Piece piece in opponents)
      { 
        piece.Set(Map);
      }
    }

    public override int FrameRate
    {
      get { return 18; }
    }



    public override void Tick()
    {
      foreach (Cube c in CubeSet)
      {
        if (getKing)
        {
          c.FillScreen(new Color(255, 0, 0));
          c.Paint();
        }
        else if (getQueen)
        {
          c.FillScreen(new Color(0, 255, 0));
          c.Paint();
        }
        else if (c.userData != null)
        {
          Wrapper fw = (Wrapper) c.userData;
          if (fw.endOfTurn)
          {
            fw.endOfTurn = false;
            UpdateMap(fw);
          }
          fw.Tick(this.DeltaTime, Map);
        }
      }
    }

    private void UpdateMap(Wrapper cw)
    {
      for (int i = 0; i < WIDTH; i++)
      {
        for (int j = 0; j < HEIGHT; j++)
        {
          Map[i, j].marks -= 0.20f;
          Map[i, j].marks = Math.Max(0, Map[i, j].marks);
          Map[i, j].isKing = false;
        }
      }

      Map[cw.mPos.x, cw.mPos.y].isKing = true;
      foreach (Piece piece in opponents)
      {
        if (piece == null)
          continue;
        if (piece.followKing)
          piece.kingPosition = new Vector2Int(cw.mPos.x, cw.mPos.y);
        Bishop bishop = piece as Bishop;
        if (bishop != null)
          bishop.Move(Map);
        else
        {
          Rook rook = piece as Rook;
          if (rook != null)
            rook.Move(Map);
          // else
          // {
          //   Queen queen = piece as Queen;
          //   if (queen != null)
          //     queen.Move(Map);
          // }
        }
      }
    }

    private void OnPause()
    {
      foreach (Cube c in this.CubeSet)
      {
        c.Image("paused", 0, 0, 0, 0, 128, 128, 1, 0);
        c.Paint();
      }
    }

    private void OnUnpause()
    {
      foreach (Cube c in this.CubeSet)
      {
        if (c.userData != null)
        {
          Wrapper fw = (Wrapper) c.userData;
          fw.Paint();
        }
      }
    }

    private void OnNewCube(Cube c)
    {
      if (c.userData == null)
      {
        if (idMain == null)
        {
          idMain = c.UniqueId;
        }

        c.userData = new Wrapper(c, idMain, Map);
        if (((Wrapper) (c.userData)).isKing)
        {
          ((Wrapper) (c.userData)).mPos = new Vector2Int(initialKingPos.x, initialKingPos.y);
          ((Wrapper) (c.userData)).Paint();
        }
      }
    }

    private void OnLostCube(Cube c)
    {
      if (c.userData != null)
      {
        Wrapper fw = (Wrapper) c.userData;
        fw.OnLostCube();
      }
    }
  }
}