using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralMapGenerator : MonoBehaviour
{
    public Room StartingRoom;

    protected List<Room> SpawnedRooms;

    protected void Start()
    {
        SpawnedRooms = new List<Room>();
        StartingRoom.Initialize(this);
        SpawnedRooms.Add(StartingRoom);
    }

    public void AddRoom(Room room)
    {
        SpawnedRooms.Add(room);
    }

    public void Generate()
    {
        //Random.InitState(GameData.RNG_SEED);
        StartCoroutine(SpawnRoutine());
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
