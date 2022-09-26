using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionPoint : MonoBehaviour
{
    public RoomDictionary RoomDictionary;

    public GameObject EnabledObject;
    public GameObject DisabledObject;

    public bool FreeConnection;
    public List<Room.RoomType> ValidRoomTypes;

    protected Coroutine spawnRoutine;


    public void EnableConnector(bool status)
    {
        EnabledObject?.SetActive(status);
        DisabledObject?.SetActive(!status);
    }

    public Coroutine Spawn()
    {
        if (spawnRoutine == null)
            spawnRoutine = StartCoroutine(SpawnRoutine());
        else
            Debug.LogError("[ ERROR ] Spawn Routine is already running.");

        return spawnRoutine;
    }

    protected IEnumerator SpawnRoutine()
    {
        List<Room> validRooms = GetValidRooms();

        while(validRooms.Count > 0)
        {
            int roomIndex = Random.Range(0, validRooms.Count);
            Room room = GameObject.Instantiate(validRooms[roomIndex]);
            room.TryPlace(this);
            break;
        }

        yield return null;
    }

    #region UtilityFunctions
    protected List<Room> GetValidRooms()
    {
        List<Room> validRooms = new List<Room>();

        for (int roomList = 0; roomList< RoomDictionary.RoomLists.Count; roomList++)
        {
            if (ValidRoomTypes.Contains(RoomDictionary.RoomLists[roomList].RoomType))
            {
                for (int index = 0; index < RoomDictionary.RoomLists[roomList].Rooms.Count; index++)
                {
                    validRooms.Add(RoomDictionary.RoomLists[roomList].Rooms[index]);
                }
            }
        }
        return validRooms;
    }
    #endregion

}
