using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public RoomTile StartingTilePrefab;
    public List<List<RoomTile>> RoomGrid;

    private void Start()
    {
        RoomGrid = new List<List<RoomTile>>();
        RoomGrid.Add(new List<RoomTile>());
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
        StartCoroutine(GenerateRoutine());
    }

    public IEnumerator GenerateRoutine()
    {
        RoomTile startTile = GameObject.Instantiate(StartingTilePrefab, transform);
        startTile.Initialize(new RoomTile.Coord { X = 0, Y = 0 }, this);
        yield return StartCoroutine(startTile.Generate(this));
    }

    public void AddTile(RoomTile tile)
    {
        CorrectCoords(tile);

        if (tile.RoomCoord.X > RoomGrid.Count - 1)
        {
            RoomGrid.Add(new List<RoomTile>());
        }

        if (tile.RoomCoord.Y >= RoomGrid[tile.RoomCoord.X].Count)
        {
            for (int index = RoomGrid[tile.RoomCoord.X].Count; index < tile.RoomCoord.Y; index++)
            {
                RoomGrid[tile.RoomCoord.X].Add(null);
            }
            RoomGrid[tile.RoomCoord.X].Add(tile);
        }
        else
        {
            RoomGrid[tile.RoomCoord.X][tile.RoomCoord.Y] = tile;
        }
    }

    public RoomTile GetTile(RoomTile.Coord coord)
    {
        if (coord.X < 0 || coord.X >= RoomGrid.Count || coord.Y < 0 || coord.Y >= RoomGrid[coord.X].Count)
            return null;
        return RoomGrid[coord.X][coord.Y];
    }

    protected void CorrectCoords(RoomTile tile)
    {
        bool addedX = false, addedY = false;

        if (tile.RoomCoord.X < 0)
        {
            RoomGrid.Insert(0, new List<RoomTile>());
            addedX = true;
        }
        if (tile.RoomCoord.Y < 0)
        {
            for (int x = 0; x < RoomGrid.Count; x++)
            {
                RoomGrid[x].Insert(0, null);
            }
            addedY = true;
        }

        for (int x = 0; x < RoomGrid.Count; x++)
        {
            for (int y = 0; y < RoomGrid[x].Count; y++)
            {
                if (RoomGrid[x][y] != null)
                {
                    RoomTile.Coord curCoord = RoomGrid[x][y].RoomCoord;
                    RoomGrid[x][y].RoomCoord = new RoomTile.Coord
                    {
                        X = addedX ? curCoord.X + 1 : curCoord.X,
                        Y = addedY ? curCoord.Y + 1 : curCoord.Y
                    };
                }
            }
        }

        RoomTile.Coord tileCoord = tile.RoomCoord;
        tile.RoomCoord = new RoomTile.Coord
        {
            X = addedX ? tileCoord.X + 1 : tileCoord.X,
            Y = addedY ? tileCoord.Y + 1 : tileCoord.Y
        };
    }
}
