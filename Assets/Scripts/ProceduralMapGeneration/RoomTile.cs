using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomTile : MonoBehaviour
{
    public struct Coord 
    {
        public int X;
        public int Y;
    }

    public Vector3 Dimensions;

    [Header("Valid Tiles")]
    public List<RoomTile> ValidTilesNorth;
    public List<RoomTile> ValidTilesEast;
    public List<RoomTile> ValidTilesSouth;
    public List<RoomTile> ValidTilesWest;

    public Coord RoomCoord;

    public void Initialize(Coord coord, Room room)
    {
        RoomCoord = coord;
        room.AddTile(this);
        Generate(room);
    }

    public void Generate(Room room)
    {
        GenerateTile(Vector2.up, ValidTilesNorth, room);
        GenerateTile(Vector2.right, ValidTilesEast, room);
        GenerateTile(Vector2.down, ValidTilesSouth, room);
        GenerateTile(Vector2.left, ValidTilesWest, room);
    }

    private void GenerateTile(Vector2 direction, List<RoomTile> validTiles, Room room)
    {
        Coord newCoord = new Coord
        {
            X = (int)(RoomCoord.X + direction.x),
            Y = (int)(RoomCoord.Y + direction.y)
        };

        if (room.GetTile(newCoord) != null || validTiles.Count < 1)
        {
            return; 
        }

        RoomTile randomTile = validTiles[Random.Range(0, validTiles.Count)];
        RoomTile newTile = GameObject.Instantiate(randomTile, room.transform);
        newTile.Initialize(newCoord, room);

        Vector3 adjustment = 0.5f * (Dimensions + newTile.Dimensions);
        adjustment.x = direction.x != 0 ? adjustment.x * direction.x : 0;
        adjustment.z = direction.y != 0 ? adjustment.z * direction.y : 0;

        newTile.transform.position = transform.position + adjustment;

        newTile.Generate(room);
    }
}
