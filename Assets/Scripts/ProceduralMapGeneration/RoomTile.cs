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
    }

    public IEnumerator Generate(Room room)
    {
        yield return StartCoroutine(GenerateTile(Vector3.forward, ValidTilesNorth, room));
        yield return StartCoroutine(GenerateTile(Vector3.right, ValidTilesEast, room));
        yield return StartCoroutine(GenerateTile(Vector3.back, ValidTilesSouth, room));
        yield return StartCoroutine(GenerateTile(Vector3.left, ValidTilesWest, room));
    }

    private IEnumerator GenerateTile(Vector3 direction, List<RoomTile> validTiles, Room room)
    {
        Coord newCoord = new Coord
        {
            X = (int)(RoomCoord.X + direction.x),
            Y = (int)(RoomCoord.Y + direction.z)
        };

        if (room.GetTile(newCoord) != null || validTiles.Count < 1)
        {
            yield break; 
        }

        RoomTile randomTile = validTiles[Random.Range(0, validTiles.Count)];
        RoomTile newTile = GameObject.Instantiate(randomTile, room.transform);

        Vector3 adjustment = 0.5f * (Dimensions + newTile.Dimensions);
        adjustment.x = direction.x != 0 ? adjustment.x * direction.x : 0;
        adjustment.y = direction.y != 0 ? adjustment.y * direction.y : 0;
        adjustment.z = direction.z != 0 ? adjustment.z * direction.z : 0;

        newTile.transform.position = transform.position + adjustment;

        newTile.Initialize(newCoord, room);
        yield return new WaitForSeconds(0.25f);
        yield return StartCoroutine(newTile.Generate(room));
    }
}
