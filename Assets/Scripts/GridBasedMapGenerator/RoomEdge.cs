using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEdge : MonoBehaviour
{
    public GameObject Door;
    public GameObject Wall;

    public enum EdgeMode { None, Door, Wall }

    public void SetMode(EdgeMode mode)
    {
        Door.SetActive(mode == EdgeMode.Door);
        Wall.SetActive(mode == EdgeMode.Wall);
    }


}
