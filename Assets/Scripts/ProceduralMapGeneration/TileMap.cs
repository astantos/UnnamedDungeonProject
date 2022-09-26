using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMap : MonoBehaviour
{
    public Tile StartingTilePrefab;
    public List<List<Tile>> TileGrid;

    private void Start()
    {
        TileGrid = new List<List<Tile>>();
        TileGrid.Add(new List<Tile>());
    }

    public bool DoGenerate;
    private void OnValidate()
    {
        if (DoGenerate)
        {
            DoGenerate = false;
            Generate();
        }
    }

    public void Generate()
    {
        UnityEngine.Random.InitState(GameData.RNG_SEED);
        StartCoroutine(GenerateRoutine());
    }

    public IEnumerator GenerateRoutine()
    {
        Tile startTile = GameObject.Instantiate(StartingTilePrefab, transform);
        startTile.Initialize(new Tile.Coord { X = 0, Y = 0 }, this);
        yield return StartCoroutine(startTile.Generate(this));
    }

    public void AddTile(Tile tile)
    {
        CorrectCoords(tile);

        if (tile.RoomCoord.X > TileGrid.Count - 1)
        {
            TileGrid.Add(new List<Tile>());
        }

        if (tile.RoomCoord.Y >= TileGrid[tile.RoomCoord.X].Count)
        {
            for (int index = TileGrid[tile.RoomCoord.X].Count; index < tile.RoomCoord.Y; index++)
            {
                TileGrid[tile.RoomCoord.X].Add(null);
            }
            TileGrid[tile.RoomCoord.X].Add(tile);
        }
        else
        {
            TileGrid[tile.RoomCoord.X][tile.RoomCoord.Y] = tile;
        }
    }

    public Tile GetTile(Tile.Coord coord)
    {
        if (coord.X < 0 || coord.X >= TileGrid.Count || coord.Y < 0 || coord.Y >= TileGrid[coord.X].Count)
            return null;
        return TileGrid[coord.X][coord.Y];
    }

    protected void CorrectCoords(Tile tile)
    {
        bool addedX = false, addedY = false;

        if (tile.RoomCoord.X < 0)
        {
            TileGrid.Insert(0, new List<Tile>());
            addedX = true;
        }
        if (tile.RoomCoord.Y < 0)
        {
            for (int x = 0; x < TileGrid.Count; x++)
            {
                TileGrid[x].Insert(0, null);
            }
            addedY = true;
        }

        for (int x = 0; x < TileGrid.Count; x++)
        {
            for (int y = 0; y < TileGrid[x].Count; y++)
            {
                if (TileGrid[x][y] != null)
                {
                    Tile.Coord curCoord = TileGrid[x][y].RoomCoord;
                    TileGrid[x][y].RoomCoord = new Tile.Coord
                    {
                        X = addedX ? curCoord.X + 1 : curCoord.X,
                        Y = addedY ? curCoord.Y + 1 : curCoord.Y
                    };
                }
            }
        }

        Tile.Coord tileCoord = tile.RoomCoord;
        tile.RoomCoord = new Tile.Coord
        {
            X = addedX ? tileCoord.X + 1 : tileCoord.X,
            Y = addedY ? tileCoord.Y + 1 : tileCoord.Y
        };
    }
}
