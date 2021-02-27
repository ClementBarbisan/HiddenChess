//
// This app presents a simple demo in which a flock of dots fly around and
// across your cubes. Different gestures influence the dots in different ways:
//
// * Neighbor: allow dots to fly between cubes.
// * Press: pull dots towards the center of a cube.
// * Tilt: push the dots in the direction of the tilt.
// * Shake: push the dots towards the edges of the cube.
// * Flip: stop movement.
//
// The other classes for this app are in separate files. See
// [FlockerWrapper.cs](flockerwrapper.html) and
// [FlockerShape.cs](flockershape.html).
//

// ------------------------------------------------------------------------

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
        Map = piece.Move(Map);
      }
    }

    // ### BaseApp.FrameRate ###
    // You can manually set your game's frame rate by overriding the FrameRate
    // property.  The rate you set it to will depend on the amount of work
    // (drawing, logic, etc.) you want to do every frame.
    public override int FrameRate
    {
      get { return 18; }
    }



    // For each cube that is being simulated, tick the simulation, and then
    // paint.
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
          Map[i, j].marks = 0;
          Map[i, j].isKing = false;
        }
      }

      Map[cw.mPos.x, cw.mPos.y].isKing = true;
      foreach (Piece piece in opponents)
      {
        if (piece.followKing)
          piece.kingPosition = new Vector2Int(cw.mPos.x, cw.mPos.y);
        Bishop bishop = piece as Bishop;
        if (bishop != null)
          Map = bishop.Move(Map);
        else
        {
          Rook rook = piece as Rook;
          if (rook != null)
            Map = rook.Move(Map);
        }
      }
    }

    // Paint the pause screen on each Cube, so that users know the game is
    // paused, and not frozen.
    private void OnPause()
    {
      foreach (Cube c in this.CubeSet)
      {
        c.Image("paused", 0, 0, 0, 0, 128, 128, 1, 0);
        c.Paint();
      }
    }

    // For each Cube that is being simulated, paint over the pause screen
    // image, now that the game is no longer paused.
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

    // When a Cube is added to the CubeSet, if the new Cube hasn't been
    // initialized with a FlockerWrapper, create one.
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

    // When a Cube is lost from the CubeSet, notify the FlockerWrapper for that
    // Cube.
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