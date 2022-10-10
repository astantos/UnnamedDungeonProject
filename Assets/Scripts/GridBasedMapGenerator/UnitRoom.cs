using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRoom : MonoBehaviour
{
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
}
