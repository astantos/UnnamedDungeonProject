using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralMapGenerator : MonoBehaviour
{
    public bool Generate;

    public Room StartingRoom;

    protected List<Room> SpawnedRooms;

    protected void Start()
    {
        SpawnedRooms = new List<Room>();
        SpawnedRooms.Add(StartingRoom);
    }

    protected void Update()
    {
        if (Generate)
        {
            Generate = false;
            Random.InitState(GameData.RNG_SEED);
            StartCoroutine(SpawnRoutine());
        }
    }

    protected IEnumerator SpawnRoutine()
    {
        for (int index = 0; index < SpawnedRooms.Count; index++)
        {
            yield return SpawnedRooms[index].SpawnConnectedRooms();
        }
        Debug.Log("[ PROGRESS ] Spawn Complete");
    }
}
