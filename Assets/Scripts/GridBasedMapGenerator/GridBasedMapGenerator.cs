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
            int direction = Random.Range(0, 4); // N S E W
            int nXRand = xRand;
            int nZRand = zRand;

            UnitRoom room = rooms[xRand][zRand];
            UnitRoom neighbour = null;

            if (direction == 0)
                nZRand++;
            else if (direction == 1)
                nZRand--;
            else if (direction == 2)
                nXRand++;
            else if (direction == 3)
                nXRand--;
            else
                Debug.LogError("[ ERROR ] Unknown Direction");

            if (nXRand >= 0 && nXRand < rooms.Length && nZRand >= 0 && nZRand < rooms[nXRand].Length)
                neighbour = rooms[nXRand][nZRand];

            if (room == null || neighbour == null)
            {
                index--;
            }
            else
            {
                Debug.Log($"Direction {direction}: Removing Walls from {{{xRand}}},{{{zRand}}} and {{{nXRand}}},{{{nZRand}}}");
                if (direction == 0) // NORTH
                {
                    room.North.SetMode(RoomEdge.EdgeMode.None);
                    neighbour.South.SetMode(RoomEdge.EdgeMode.None);
                }
                else if (direction == 1) // South
                {
                    room.South.SetMode(RoomEdge.EdgeMode.None);
                    neighbour.North.SetMode(RoomEdge.EdgeMode.None);
                }
                else if (direction == 2) // East
                {
                    room.East.SetMode(RoomEdge.EdgeMode.None);
                    neighbour.West.SetMode(RoomEdge.EdgeMode.None);
                }
                else if (direction == 3) // West
                {
                    room.West.SetMode(RoomEdge.EdgeMode.None);
                    neighbour.East.SetMode(RoomEdge.EdgeMode.None);
                }
            }
        }
    }

    protected void GenerateDoors()
    {
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
