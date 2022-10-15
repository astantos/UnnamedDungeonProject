using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class UnitRoom : NetworkBehaviour
{
    public enum RoomDirection {East, North, South, West, Directions }

    [SyncVar] public int X;
    [SyncVar] public int Z;


    public RoomEdge North;
    public RoomEdge South;
    public RoomEdge East;
    public RoomEdge West;

    public GameObject Floor;

    public void Initialize(int x, int z)
    {
        X = x;
        Z = z;
        gameObject.name = $"Room [{x}, {z}]";
        East.SetMode(RoomEdge.EdgeMode.Wall);
        North.SetMode(RoomEdge.EdgeMode.Wall);
        South.SetMode(RoomEdge.EdgeMode.Wall);
        West.SetMode(RoomEdge.EdgeMode.Wall);
    }

    public Vector2 GetDimensions()
    {
        Vector3 size = Floor.GetComponent<Renderer>().bounds.size;
        return new Vector2(size.x, size.z);
    }

    public RoomEdge GetEdge(RoomDirection dir)
    {
        if (dir == RoomDirection.East) return East;
        else if (dir == RoomDirection.North) return North;
        else if (dir == RoomDirection.South) return South;
        else return West;
    }

    public int GetEdgeCount(RoomEdge.EdgeMode mode)
    {
        int count = 0;
        if (East.CurrentMode == mode) count++;
        if (North.CurrentMode == mode) count++;
        if (South.CurrentMode == mode) count++;
        if (West.CurrentMode == mode) count++;
        return count;
    }

    #region Server
    [ClientRpc]
    public void SetEdgeMode(int edge, int mode)
    {
        RoomDirection direction = (RoomDirection)edge;
        GetEdge(direction).SetMode((RoomEdge.EdgeMode)mode);
    }
    #endregion
}
