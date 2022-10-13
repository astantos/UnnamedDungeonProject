using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEdge : MonoBehaviour 
{
    public GameObject Door;
    public GameObject Wall;

    public enum EdgeMode { None, Door, Wall }

    public EdgeMode CurrentMode { get; protected set; }

    protected RoomEdge()
    {
        CurrentMode = EdgeMode.Wall;
    }

    public void SetMode(EdgeMode mode)
    {
        CurrentMode = mode;
        Door.SetActive(CurrentMode == EdgeMode.Door);
        Wall.SetActive(CurrentMode == EdgeMode.Wall);
    }


}
