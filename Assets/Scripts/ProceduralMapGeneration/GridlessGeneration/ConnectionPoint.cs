using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ConnectionPoint : MonoBehaviour
{
    [Serializable]
    public struct ConnectionType
    {
        public Room.RoomType Type;
        public int Weight;
    }

    public RoomDictionary RoomDictionary;

    public GameObject EnabledObject;
    public GameObject DisabledObject;

    public bool FreeConnection;
    public List<ConnectionType> ValidRoomTypes;

    protected Coroutine spawnRoutine;

    public bool PlacementValid { get; protected set; }
    protected ProceduralMapGenerator generator;

    public bool ConnectorEnabled { get; protected set; }

    protected void Awake()
    {
        EnableConnector(true);
    }

    public void Initialize(ProceduralMapGenerator generator)
    {
        this.generator = generator;
    }

    public void EnableConnector(bool status)
    {
        ConnectorEnabled = status;

        if (EnabledObject != null)
            EnabledObject.SetActive(ConnectorEnabled);
        
        if (DisabledObject != null)
            DisabledObject.SetActive(!ConnectorEnabled);

        transform.localScale = ConnectorEnabled ? Vector3.one : Vector3.zero;
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
        //Debug.Log($"[ CONNECTION POINT ] {gameObject.name}: Attempting Placement");
        PlacementValid = false;

        List<ConnectionType> validConnections = new List<ConnectionType>(ValidRoomTypes);
        ConnectionType connection;

        while(validConnections.Count > 0)
        {
            connection = GetRandomConnection(validConnections);
            Room.RoomType roomType = connection.Type;

            List<Room> validRooms = GetValidRooms(roomType);

            while(validRooms.Count > 0)
            {
                int roomIndex = Random.Range(0, validRooms.Count);
                Room room = GameObject.Instantiate(validRooms[roomIndex]);
                room.Initialize(generator);
                //room.gameObject.name = $"{room.gameObject.name}_Current";
                //Debug.Log($"[ CONNECTION POINT ] {gameObject.name}: Attempting to Place {room.gameObject.name}");
                yield return room.TryPlace(this);
                if (room.PlacementValid)
                {
                    //Debug.Log($"[ CONNECTION POINT ] {gameObject.name}: Placement Successful");
                    PlacementValid = true;
                    generator.AddRoom(room);
                    //room.gameObject.name = $"{room.gameObject.name}_PLACED";
                    break;
                }
                else
                {
                    // Selected Room Cannot Be Placed
                    //Debug.Log($"[ CONNECTION POINT ] {gameObject.name}: Placement Not Successful, removing {room.gameObject.name} as valid");
                    room.StopAllCoroutines();
                    //room.gameObject.name = $"{room.gameObject.name}_DESTROYED";
                    GameObject.Destroy(room.gameObject);
                    validRooms.RemoveAt(roomIndex);
                }
            }

            if (PlacementValid)
            {
                EnableConnector(false);
                break;
            }
            else
            {
                //Debug.Log($"[ CONNECTION POINT ] {gameObject.name}: No Valid Placements, disabling connector");
                validConnections.Remove(connection);
            }
        }


        if (PlacementValid == false)
        {
            EnableConnector(false);
        }
    }

    #region UtilityFunctions
    protected List<Room> GetValidRooms(Room.RoomType roomType)
    {
        List<Room> validRooms = new List<Room>();

        for (int roomList = 0; roomList< RoomDictionary.RoomLists.Count; roomList++)
        {
            if (RoomDictionary.RoomLists[roomList].RoomType.Equals(roomType))
            {
                for (int index = 0; index < RoomDictionary.RoomLists[roomList].Rooms.Count; index++)
                {
                    validRooms.Add(RoomDictionary.RoomLists[roomList].Rooms[index]);
                }
            }
        }
        return validRooms;
    }

    protected ConnectionType GetRandomConnection(List<ConnectionType> validTypes)
    {
        int totalWeight = 0;
        for (int index = 0; index < validTypes.Count; index++)
        {
            totalWeight += validTypes[index].Weight;
        }

        if (totalWeight > 0)
        {
            int randomTarget = Random.Range(0, totalWeight);
            int current = 0;
            for (int index = 0; index < validTypes.Count; index++)
            {
                current += validTypes[index].Weight;
                if (current >= randomTarget)
                    return validTypes[index];
            }
        }

        Debug.LogWarning("Returning first as default");
        return validTypes[0];
    }

    #endregion

}
