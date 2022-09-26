using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralMapGenerator : MonoBehaviour
{
    public bool Generate;

    public Room StartingRoom;

    private void OnValidate()
    {
        if (Generate)
        {
            Generate = false;
            StartingRoom.SpawnConnectedRooms();
        }
    }
}
