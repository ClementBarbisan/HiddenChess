
using System;
using System.Collections.Generic;
using Sifteo;


namespace Game
{

  public class HideChess : BaseApp
  {
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

    // Here we initialize our app.
    public override void Setup()
    {
      Map = new Case[Wrapper.WIDTH, Wrapper.HEIGHT];
      for (int i = 0; i < Wrapper.WIDTH; i++)
      {
        for (int j = 0; j < Wrapper.HEIGHT; j++)
        {
          Map[i, j] = new Case();
          Map[i, j].value = 0;
          Map[i, j].marks = 0;
          Map[i, j].isKing = false;
        }
      }

      images = this.Images;
      opponents = new List<Piece>();
      opponents.Add(new Bishop());
      // opponents.Add(new Bishop());
      opponents.Add(new Rook());
      // opponents.Add(new Rook());
      opponents.Add(new Queen());
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
        piece.Move(Map);
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
      for (int i = 0; i < Wrapper.WIDTH; i++)
      {
        for (int j = 0; j < Wrapper.HEIGHT; j++)
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
          ((Wrapper) (c.userData)).mPos = new Vector2Int(0, 0);
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