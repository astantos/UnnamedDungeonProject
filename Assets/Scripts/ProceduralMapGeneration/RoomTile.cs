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
        GenerateTile(Vector3.forward, ValidTilesNorth, room);
        GenerateTile(Vector3.right, ValidTilesEast, room);
        GenerateTile(Vector3.back, ValidTilesSouth, room);
        GenerateTile(Vector3.left, ValidTilesWest, room);
    }

    private void GenerateTile(Vector3 direction, List<RoomTile> validTiles, Room room)
    {
        Coord newCoord = new Coord
        {
            X = (int)(RoomCoord.X + direction.x),
            Y = (int)(RoomCoord.Y + direction.z)
        };

        if (room.GetTile(newCoord) != null || validTiles.Count < 1)
        {
            return; 
        }

        RoomTile randomTile = validTiles[Random.Range(0, validTiles.Count)];
        RoomTile newTile = GameObject.Instantiate(randomTile, room.transform);

        Vector3 adjustment = 0.5f * (Dimensions + newTile.Dimensions);
        adjustment.x = direction.x != 0 ? adjustment.x * direction.x : 0;
        adjustment.y = direction.y != 0 ? adjustment.y * direction.y : 0;
        adjustment.z = direction.z != 0 ? adjustment.z * direction.z : 0;

        newTile.transform.position = transform.position + adjustment;

        newTile.Initialize(newCoord, room);
    }
}
