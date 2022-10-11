using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRoom : MonoBehaviour
{
    public enum RoomDirection {East, North, South, West, Directions }

    public RoomEdge North;
    public RoomEdge South;
    public RoomEdge East;
    public RoomEdge West;

    public GameObject Floor;

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


}
