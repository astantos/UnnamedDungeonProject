using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBasedMapGenerator : MonoBehaviour
{
    public UnitRoom RoomPrefab;

    [Header("Generation Parameters")]
    public int Width;
    public int Height;
    public int HoleCount;
    public int MinCoverage;
    public int WallRemovalCount;
    public int DoorCount;

    protected UnitRoom[][] rooms;

    public void Generate()
    {
        Reset();
        GenerateRooms();
        GenerateHoles();
        GenerateRandomSpawnPoint();
        CleanIslands();
        RemoveWalls();
        GenerateDoors();
    }

    protected void Reset()
    {
        if (rooms == null)
            return;

        for (int x = 0; x < rooms.Length; x++)
        {
            for (int z = 0; z < rooms[x].Length; z++)
            {
                if (rooms[x][z] != null)
                {
                    GameObject.Destroy(rooms[x][z].gameObject);
                    rooms[x][z] = null;
                }
            }
        }

        rooms = null;
        MarkedRooms = null;
    }

    protected void GenerateRooms()
    {
        rooms = new UnitRoom[Width][];
        for (int x = 0; x < rooms.Length; x++)
        {
            rooms[x] = new UnitRoom[Height];
            for (int z = 0; z < rooms[x].Length; z++)
            {
                rooms[x][z] = GameObject.Instantiate(RoomPrefab);
                Vector2 dimensions = rooms[x][z].GetDimensions();
                rooms[x][z].transform.position = new Vector3
                (
                    x * dimensions.x,
                    0,
                    z * dimensions.y
                );
            }
        }
    }

    protected void GenerateHoles()
    {
        for (int index = 0; index < HoleCount; index++)
        {
            int xRand = Random.Range(0, rooms.Length);
            int zRand = Random.Range(0, rooms[xRand].Length);

            if (rooms[xRand][zRand] != null)
            {
                GameObject.Destroy(rooms[xRand][zRand].gameObject);
                rooms[xRand][zRand] = null;
            }
            else
            {
                index--;
            }
        }
    }

    protected void GenerateRandomSpawnPoint()
    {
        int xRand = Random.Range(0, rooms.Length);
        int zRand = Random.Range(0, rooms[xRand].Length);
    
        if (rooms[xRand][zRand] == null)
        {
            GenerateRandomSpawnPoint();
        }
        else
        {
            ResetMarkedRooms();
            int coverage = MarkCellAndContinue(xRand, zRand, 0); 
            if (coverage < MinCoverage)
            {
                Debug.LogError("[ ERROR ] INSUFFICIENT COVERAGE");
            }
            Debug.Log($"Coverage Amount: {coverage}");
        }
    }

    protected void CleanIslands()
    {
        for (int x = 0; x < rooms.Length; x++)
        {
            for (int z = 0; z < rooms[x].Length; z++)
            {
                if (MarkedRooms[x][z] == false && rooms[x][z] != null)
                {
                    GameObject.Destroy(rooms[x][z].gameObject);
                    rooms[x][z] = null;
                }
            }
        }
    }

    protected void RemoveWalls()
    {
        for (int index = 0; index < WallRemovalCount; index++)
        {
            int xRand = Random.Range(0, rooms.Length);
            int zRand = Random.Range(0, rooms[xRand].Length);
            UnitRoom.RoomDirection direction = (UnitRoom.RoomDirection)Random.Range(0, (int)UnitRoom.RoomDirection.Directions);
            int nXRand = xRand;
            int nZRand = zRand;

            UnitRoom room = rooms[xRand][zRand];
            UnitRoom neighbour = null;

            if (direction == UnitRoom.RoomDirection.East)
                nXRand++;
            else if (direction == UnitRoom.RoomDirection.North)
                nZRand++;
            else if (direction == UnitRoom.RoomDirection.South)
                nZRand--;
            else 
                nXRand--;

            if (nXRand >= 0 && nXRand < rooms.Length && nZRand >= 0 && nZRand < rooms[nXRand].Length)
                neighbour = rooms[nXRand][nZRand];

            if (room == null || neighbour == null)
            {
                index--;
            }
            else
            {
                Debug.Log($"Direction {direction}: Removing Walls from {{{xRand}}},{{{zRand}}} and {{{nXRand}}},{{{nZRand}}}");
                RoomEdge roomEdge;
                RoomEdge neighbourEdge;
                if (direction == UnitRoom.RoomDirection.East)
                {
                    roomEdge = room.GetEdge(UnitRoom.RoomDirection.East);
                    neighbourEdge = neighbour.GetEdge(UnitRoom.RoomDirection.West);
                }
                else if (direction == UnitRoom.RoomDirection.North)
                {
                    roomEdge = room.GetEdge(UnitRoom.RoomDirection.North);
                    neighbourEdge = neighbour.GetEdge(UnitRoom.RoomDirection.South);
                }
                else if (direction == UnitRoom.RoomDirection.South)
                {
                    roomEdge = room.GetEdge(UnitRoom.RoomDirection.South);
                    neighbourEdge = neighbour.GetEdge(UnitRoom.RoomDirection.North);
                }
                else
                {
                    roomEdge = room.GetEdge(UnitRoom.RoomDirection.West);
                    neighbourEdge = neighbour.GetEdge(UnitRoom.RoomDirection.East);
                }

                roomEdge.SetMode(RoomEdge.EdgeMode.None);
                neighbourEdge.SetMode(RoomEdge.EdgeMode.None);
            }
        }
    }

    protected void GenerateDoors()
    {
        for (int index = 0; index < DoorCount; index++)
        {
            int xRand = Random.Range(0, rooms.Length);
            int zRand = Random.Range(0, rooms[xRand].Length);
            UnitRoom.RoomDirection direction = (UnitRoom.RoomDirection)Random.Range(0, (int)UnitRoom.RoomDirection.Directions);
            int nXRand = xRand;
            int nZRand = zRand;

            UnitRoom room = rooms[xRand][zRand];
            UnitRoom neighbour = null;

            if (direction == UnitRoom.RoomDirection.East)
                nXRand++;
            else if (direction == UnitRoom.RoomDirection.North)
                nZRand++;
            else if (direction == UnitRoom.RoomDirection.South)
                nZRand--;
            else 
                nXRand--;

            if (nXRand >= 0 && nXRand < rooms.Length && nZRand >= 0 && nZRand < rooms[nXRand].Length)
                neighbour = rooms[nXRand][nZRand];

            if (room == null || neighbour == null)
            {
                index--;
            }
            else
            {
                Debug.Log($"Direction {direction}: Removing Walls from {{{xRand}}},{{{zRand}}} and {{{nXRand}}},{{{nZRand}}}");
                RoomEdge roomEdge;
                RoomEdge neighbourEdge;
                if (direction == UnitRoom.RoomDirection.East)
                {
                    roomEdge = room.GetEdge(UnitRoom.RoomDirection.East);
                    neighbourEdge = neighbour.GetEdge(UnitRoom.RoomDirection.West);
                }
                else if (direction == UnitRoom.RoomDirection.North)
                {
                    roomEdge = room.GetEdge(UnitRoom.RoomDirection.North);
                    neighbourEdge = neighbour.GetEdge(UnitRoom.RoomDirection.South);
                }
                else if (direction == UnitRoom.RoomDirection.South)
                {
                    roomEdge = room.GetEdge(UnitRoom.RoomDirection.South);
                    neighbourEdge = neighbour.GetEdge(UnitRoom.RoomDirection.North);
                }
                else
                {
                    roomEdge = room.GetEdge(UnitRoom.RoomDirection.West);
                    neighbourEdge = neighbour.GetEdge(UnitRoom.RoomDirection.East);
                }

                if (roomEdge.CurrentMode == RoomEdge.EdgeMode.None)
                {
                    index--;
                }
                else
                {
                    roomEdge.SetMode(RoomEdge.EdgeMode.Door);
                    neighbourEdge.SetMode(RoomEdge.EdgeMode.Door);
                }
            }
        }
    }

    #region Utility
    protected bool[][] MarkedRooms;
    protected void ResetMarkedRooms()
    {
        MarkedRooms = new bool[rooms.Length][];
        for (int x = 0; x < MarkedRooms.Length; x++)
            MarkedRooms[x] = new bool[rooms[x].Length];

    }

    protected int MarkCellAndContinue(int x, int z, int currentCount)
    {
        if (x < 0 || z < 0 || x > rooms.Length - 1 || z > rooms[x].Length - 1)
            return currentCount;

        UnitRoom room = rooms[x][z];
        if (room == null || MarkedRooms[x][z])
            return currentCount;

        MarkedRooms[x][z] = true;

        int result = currentCount;
        result++;
        result = MarkCellAndContinue(x, z + 1, result);
        result = MarkCellAndContinue(x + 1, z, result);
        result = MarkCellAndContinue(x, z - 1, result);
        result = MarkCellAndContinue(x - 1, z, result);
        return result;
    }
    #endregion
}
