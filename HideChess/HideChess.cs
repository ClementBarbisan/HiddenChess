
using System;
using System.Collections.Generic;
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

    public static readonly int[,] map0 =
    {
      {7, 0, 0},
      {0, 0, 0},
      {0, 0, 9}
    };
    public static readonly int[,] map1 =
    {
      {7, 0, 0},
      {0, 8, 0},
      {0, 0, 9}
    };
    
    public static readonly int[,] map2 =
    {
      {7, 0, 0},
      {0, 5, 0},
      {0, 0, 9}
    };
    public static readonly int[,] map3 =
    {
      {7, 0, 0},
      {0, 4, 0},
      {0, 0, 9}
    };
    
    public static readonly int[,] map4 =
    {
      {7, 0, 0, 0},
      {0, 1, 1, 0},
      {0, 1, 1, 0},
      {0, 0, 0, 9}
    };
    
    public static readonly int[,] map5 =
    {
      {7, 0, 0, 0},
      {0, 0, 4, 0},
      {0, 4, 0, 0},
      {0, 0, 0, 9}
    };
    
    public static readonly int[,] map6 =
    {
      {7, 0, 0, 0},
      {0, 0, 5, 0},
      {0, 5, 0, 0},
      {0, 0, 0, 9}
    };
    
    public static readonly int[,] map7 =
    {
      {5, 0, 4, 0, 7, 4, 0, 5},
      {8, 8, 8, 8, 8, 8, 8, 8},
      {0, 0, 0, 0, 0, 0, 0, 0},
      {0, 0, 0, 0, 0, 0, 0, 0},
      {0, 0, 0, 0, 0, 0, 0, 0},
      {0, 0, 0, 0, 0, 0, 0, 0},
      {0, 0, 0, 0, 0, 0, 0, 0},
      {0, 0, 0, 9, 0, 0, 0, 0}
    };
    
    public readonly List<int[,]> maps = new List<int[,]>()
    {
      map0,
      map1,
      map2,
      map3,
      map4,
      map5,
      map6,
      map7
    };

    public static bool getKing;
    public static bool getQueen;
    public static bool reset;
    public static List<Piece> opponents;
    internal string idMain = null;
    public static ImageSet images;
    internal Case[,] Map;
    internal Vector2Int initialKingPos;

    internal int indexMap = 0;
  
    // Here we initialize our app.
    public override void Setup()
    {
      if (reset)
      {
        base.Setup();
        if (getQueen)
          indexMap++;
        idMain = null;
        this.PauseEvent -= OnPause;
        this.UnpauseEvent -= OnUnpause;
        this.CubeSet.NewCubeEvent -= OnNewCube;
        this.CubeSet.LostCubeEvent -= OnLostCube;
        foreach (Cube c in CubeSet)
        {
          ((Wrapper)c.userData).Dispose();
          c.userData = null;
          if (indexMap >= maps.Count)
          {
            c.FillScreen(new Color(0, 0, 0));
            c.Image(Images["king"].name);
            c.Paint();
          }
        }
        Map = null;
        if (opponents != null)
        {
          for (int i = 0; i < opponents.Count; i++)
          {
            Piece tmp = opponents[i];
            tmp.Dispose();
            tmp = null;
          }
          opponents.Clear();
        }
        GC.Collect();
        if (indexMap >= maps.Count)
        {
          getKing = true;
          getQueen = true;
          return;
        }
      }
      opponents = new List<Piece>();
      bool queenAdded = false;
      reset = false;
      getKing = false;
      getQueen = false;
      int[,] initialMap = maps[indexMap];
      HEIGHT = initialMap.GetLength(1);
      WIDTH = initialMap.GetLength(0);

      Map = new Case[WIDTH, HEIGHT];
      for (int i = 0; i < WIDTH; i++)
      {
        for (int j = 0; j < HEIGHT; j++)
        {
          int tmp = initialMap[i, j];
          if (tmp == (int) Type.Bishop)
          {
            opponents.Add(new Bishop());
            opponents[opponents.Count - 1].Position = new Vector2Int(i, j);
          }
          else if (tmp == (int) Type.Rook)
          {
            opponents.Add(new Rook());
            opponents[opponents.Count - 1].Position = new Vector2Int(i, j);
          }
          else if (tmp == (int) Type.Knight)
          {
            opponents.Add(new Knight());
            opponents[opponents.Count - 1].Position = new Vector2Int(i, j);
          }
          else if (tmp == (int) Type.Pawn)
          {
            opponents.Add(new Pawn());
            opponents[opponents.Count - 1].Position = new Vector2Int(i, j);
          } 
          else if (tmp == (int) Type.Queen && !queenAdded)
          {
            opponents.Add(new Queen());
            opponents[opponents.Count - 1].Position = new Vector2Int(i, j);
            queenAdded = true;
          } 
          Map[i, j] = new Case();
          Map[i, j].value = 0;
          Map[i, j].isKing = false;
          if (tmp == 9)
          {
            initialKingPos = new Vector2Int(i, j);
            Map[i, j].isKing = true;
          }
          else if (tmp != 7 || queenAdded)
            Map[i, j].value = tmp;
          Map[i, j].marks = 0;
        }
      }
      images = this.Images;
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
      if (reset)
      {
        Setup();
        return;
      }
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
        {
          bishop.Move(Map);
          continue;
        } 
        Rook rook = piece as Rook;
        if (rook != null)
        {
          rook.Move(Map);
          continue;
        }
        Knight knight = piece as Knight;
        if (knight != null)
        {
          knight.Move(Map);
          continue;
        }
        Pawn pawn = piece as Pawn;
        if (pawn != null)
        {
          pawn.Move(Map);
          continue;
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