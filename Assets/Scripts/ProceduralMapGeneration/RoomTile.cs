using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomTile : MonoBehaviour
{
    [Serializable]
    public struct ValidTile
    {
        public RoomTile Tile;
        public int Weighting;
    }

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

    [Header("Valid Tiles Weighted")]
    public List<ValidTile> ValidTilesNorthWeighted;
    public List<ValidTile> ValidTilesEastWeighted;
    public List<ValidTile> ValidTilesSouthWeighted;
    public List<ValidTile> ValidTilesWestWeighted;



    public Coord RoomCoord;

    public void Initialize(Coord coord, Room room)
    {
        RoomCoord = coord;
        room.AddTile(this);
    }

    public IEnumerator Generate(Room room)
    {
        yield return StartCoroutine(GenerateTile(Vector3.forward, ValidTilesNorthWeighted, room));
        yield return StartCoroutine(GenerateTile(Vector3.right, ValidTilesEastWeighted, room));
        yield return StartCoroutine(GenerateTile(Vector3.back, ValidTilesSouthWeighted, room));
        yield return StartCoroutine(GenerateTile(Vector3.left, ValidTilesWestWeighted, room));
    }

    private IEnumerator GenerateTile(Vector3 direction, List<ValidTile> validTiles, Room room)
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

        RoomTile randomTile = GetRandomTile(validTiles);
        RoomTile newTile = GameObject.Instantiate(randomTile, room.transform);

        Vector3 adjustment = 0.5f * (Dimensions + newTile.Dimensions);
        adjustment.x = direction.x != 0 ? adjustment.x * direction.x : 0;
        adjustment.y = direction.y != 0 ? adjustment.y * direction.y : 0;
        adjustment.z = direction.z != 0 ? adjustment.z * direction.z : 0;

        newTile.transform.position = transform.position + adjustment;

        newTile.Initialize(newCoord, room);
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(newTile.Generate(room));
    }

    #region Utility
    protected RoomTile GetRandomTile(List<ValidTile> validTiles)
    {
        string debugStr = "";

        int totalWeight = 0;
        for (int index = 0; index < validTiles.Count; index++)
        {
            totalWeight += validTiles[index].Weighting;
        }

        int random = Random.Range(0, totalWeight);
        int current = 0;
       
        debugStr = $"Total Weight: {totalWeight} | random num: {random}";

        for (int index = 0; index < validTiles.Count; index++)
        {
            current += validTiles[index].Weighting;
            if (current > random)
            {
                debugStr = $"{debugStr} | returning index {index}";
                Debug.Log(debugStr);
                return validTiles[index].Tile;
            }
        }


        // Should never reach here to return null
        Debug.LogError("[ ERROR ] Broke out of Weighting Loop with invalid result");
        return null;
    }
    #endregion
}
