using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GridBasedMapGenerator : NetworkBehaviour
{
    public UnitRoom RoomPrefab;

    [Header("Generation Parameters")]
    public int Width;
    public int Height;
    public int HoleCount;
    [Range(0, 1)]
    public float MinCoverage;
    public int WallRemovalCount;
    public int DoorCount;

    [Header("Exit Failsafes")]
    public int GenerationAttempts;
    public int CoverageAttempts;

    [Tooltip("Total multiplier number of attempts to find targets, per function (e.g. GenerateDoors(100) => 100 x BreakoutAttepts)")]
    public int BreakoutAttempts;

    protected UnitRoom[][] rooms;

    protected int generationAttempts;

    public void Generate(int attemptCount = 0)
    {
        if (generationAttempts > GenerationAttempts)
        {
            Debug.LogError($"[ ERROR ] Generation Exceeded {GenerationAttempts}. Consider changing some parameters.");
            return;
        }

        if (HoleCount > (Width * Height))
        {
            Debug.LogError($"[ ERROR ] Desired Hole Count ({HoleCount}) exceeds the total available tiles ({Width * Height})! Generation cancelled.");
        }

        generationAttempts = attemptCount;

        Debug.Log($"[ GENERATION ] Attempt {generationAttempts}");

        Reset();
        GenerateRooms();
        GenerateHoles();
        RemoveWalls();
        GenerateDoors();
        CheckCoverage();
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
                    NetworkServer.UnSpawn(rooms[x][z].gameObject);
                    rooms[x][z] = null;
                }
            }
        }

        rooms = null;
        MarkedRooms = null;
    }

    protected void GenerateRooms()
    {
        Initialize2DArray(Width, Height);
    }

    protected void GenerateHoles()
    {
        for (int index = 0; index < HoleCount; index++)
        {
            int xRand = Random.Range(0, rooms.Length);
            int zRand = Random.Range(0, rooms[xRand].Length);

            if (rooms[xRand][zRand] != null)
            {
                Unspawn(rooms[xRand][zRand]);
                rooms[xRand][zRand] = null;
            }
            else
            {
                index--;
            }
        }
    }

    protected void CheckCoverage(int attemptCount = 0)
    {
        Debug.Log($"\tCoverage Check Attempt: {attemptCount}");
        int xRand = Random.Range(0, rooms.Length);
        int zRand = Random.Range(0, rooms[xRand].Length);

        if (rooms[xRand][zRand] == null)
        {
            CheckCoverage();
        }
        else
        {
            ResetMarkedRooms();

            int cellsNeeded = (int)(MinCoverage * Width * Height);
            int coverage = MarkCellAndContinue(xRand, zRand, 0);

            Debug.Log($"\tCells Covered: {coverage} of {cellsNeeded}");
            if (coverage < cellsNeeded)
            {
                attemptCount++;
                if (attemptCount >= CoverageAttempts)
                {
                    Debug.LogWarning("[ ERROR ] Unable to find sufficient coverage. Regenerating . . . ");
                    Generate(generationAttempts + 1);
                }
                else
                {
                    CheckCoverage(attemptCount);
                }
            }
            else
            {
                CleanIslands();
            }
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
                    Unspawn(rooms[x][z]);
                    rooms[x][z] = null;
                }
            }
        }
    }

    protected void RemoveWalls()
    {
        if (WallRemovalCount > CountWalls())
        {
            Debug.LogError($"[ ERROR ] Desired Wall Removal Count ({WallRemovalCount}) exceeds remaining Wall Count");
        }

        int currentBreakout = 0;
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

            currentBreakout++;
            if (currentBreakout > BreakoutAttempts * WallRemovalCount)
            {
                Debug.LogError("[ ERROR ] RemoveWalls() exceeded Breakout Attempts. There might not be enough walls to remove.");
                return;
            }
        }
    }

    protected void GenerateDoors()
    {
        if (DoorCount > CountWalls())
        {
            Debug.LogError($"[ ERROR ] Desired Door Count ({DoorCount}) exceeds remaining Wall Count");
        }


        int currentBreakout = 0;
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

                currentBreakout++;
                if (currentBreakout > BreakoutAttempts * DoorCount)
                {
                    Debug.LogError("[ ERROR ] GenerateDoors() exceeded Breakout Attempts. There might not be enough walls left to add a door to.");
                    return;
                }
            }
        }
    }

    #region Utility
    public void Initialize2DArray(int width, int height)
    {
        rooms = new UnitRoom[width][];
        for (int x = 0; x < rooms.Length; x++)
        {
            rooms[x] = new UnitRoom[height];
            for (int z = 0; z < rooms[x].Length; z++)
            {
                rooms[x][z] = Spawn(RoomPrefab);
                Vector2 dimensions = rooms[x][z].GetDimensions();
                rooms[x][z].transform.position = new Vector3
                (
                    x * dimensions.x,
                    0,
                    z * dimensions.y
                );
                rooms[x][z].transform.parent = transform;
            }
        }
    }
    protected int CountWalls()
    {
        int count = 0;
        for (int x = 0; x < rooms.Length; x++)
        {
            for (int z = 0; z < rooms[x].Length; z++)
            {
                if (rooms[x][z] == null)
                    continue;
                count += rooms[x][z].GetEdgeCount(RoomEdge.EdgeMode.Wall);
            }
        }
        return count;
    }

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

        if (room.East.CurrentMode == RoomEdge.EdgeMode.Door || room.East.CurrentMode == RoomEdge.EdgeMode.None)
            result = MarkCellAndContinue(x + 1, z, result);

        if (room.North.CurrentMode == RoomEdge.EdgeMode.Door || room.North.CurrentMode == RoomEdge.EdgeMode.None)
            result = MarkCellAndContinue(x, z + 1, result);

        if (room.South.CurrentMode == RoomEdge.EdgeMode.Door || room.South.CurrentMode == RoomEdge.EdgeMode.None)
            result = MarkCellAndContinue(x, z - 1, result);

        if (room.West.CurrentMode == RoomEdge.EdgeMode.Door || room.West.CurrentMode == RoomEdge.EdgeMode.None)
            result = MarkCellAndContinue(x - 1, z, result);

        return result;
    }
    #endregion

    #region Networking
    public UnitRoom Spawn(UnitRoom prefab)
    {
        UnitRoom room = GameObject.Instantiate(RoomPrefab);
        NetworkServer.Spawn(room.gameObject);
        return room;
    }

    public void Unspawn(UnitRoom room)
    {
        NetworkServer.UnSpawn(room.gameObject);
        GameObject.Destroy(room.gameObject);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!isServer)
            RequestRooms();
    }

    [Command(requiresAuthority = false)]
    public void RequestRooms()
    {
        Debug.Log("[ SERVER ] Client is requesting rooms");
        for (int x = 0; x < rooms.Length; x++)
        {
            for (int z = 0; z < rooms[x].Length; z++)
            {
                if (rooms[x][z] == null)
                    continue;

                UnitRoom room = rooms[x][z];
                room.SetEdgeMode((int)UnitRoom.RoomDirection.East, (int)room.GetEdge(UnitRoom.RoomDirection.East).CurrentMode);
                room.SetEdgeMode((int)UnitRoom.RoomDirection.North, (int)room.GetEdge(UnitRoom.RoomDirection.North).CurrentMode);
                room.SetEdgeMode((int)UnitRoom.RoomDirection.South, (int)room.GetEdge(UnitRoom.RoomDirection.South).CurrentMode);
                room.SetEdgeMode((int)UnitRoom.RoomDirection.West, (int)room.GetEdge(UnitRoom.RoomDirection.West).CurrentMode);
            }

        }

    }
    #endregion
}
