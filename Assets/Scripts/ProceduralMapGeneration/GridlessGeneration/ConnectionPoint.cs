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

    [Header("Parameters")]
    public int ConnectionIndex;
    public RoomDictionary RoomDictionary;
    public GameObject EnabledObject;
    public GameObject DisabledObject;

    [Header("Connections")]
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
        transform.localScale = ConnectorEnabled ? Vector3.one : Vector3.zero;
    }

    public void SetDoorwayActive(bool status)
    {
        if (EnabledObject != null)
            EnabledObject.SetActive(status);
        
        if (DisabledObject != null)
            DisabledObject.SetActive(!status);
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
        if (ConnectorEnabled)
        {
            PlacementValid = false;

            List<ConnectionType> validConnections = new List<ConnectionType>(ValidRoomTypes);
            Debug.Log($"\t\t>>>There are {validConnections.Count} valid connections");
            ConnectionType connection;

            while (validConnections.Count > 0)
            {
                connection = GetRandomConnection(validConnections);
                Room.RoomType roomType = connection.Type;

                List<RoomList.WeightedRoom> validRooms = GetValidRooms(roomType);
                Debug.Log($"\t\t\t>>>There are {validRooms.Count} valid rooms");

                while (validRooms.Count > 0)
                {
                    RoomList.WeightedRoom weightedRoom = GetRandomRoom(validRooms);
                    Room room = GameObject.Instantiate(weightedRoom.Room);
                    room.Initialize(generator);
                    yield return room.TryPlace(this);
                    if (room.PlacementValid)
                    {
                        PlacementValid = true;
                        generator.AddRoom(room);
                        break;
                    }
                    else
                    {
                        // Selected Room Cannot Be Placed
                        room.StopAllCoroutines();
                        GameObject.Destroy(room.gameObject);
                        validRooms.Remove(weightedRoom);
                    }
                }

                if (PlacementValid)
                {
                    EnableConnector(false);
                    break;
                }
                else
                {
                    validConnections.Remove(connection);
                }
            }

            EnableConnector(false);
            SetDoorwayActive(validConnections.Count > 0);
        }
    }

    #region UtilityFunctions
    protected List<RoomList.WeightedRoom> GetValidRooms(Room.RoomType roomType)
    {
        List<RoomList.WeightedRoom> validRooms = new List<RoomList.WeightedRoom>();

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
    protected RoomList.WeightedRoom GetRandomRoom(List<RoomList.WeightedRoom> weightedRooms)
    {
        int totalWeight = 0;
        for (int index = 0; index < weightedRooms.Count; index++)
        {
            totalWeight += weightedRooms[index].Weight;
        }

        if (totalWeight > 0)
        {
            int randomTarget = Random.Range(0, totalWeight);
            int current = 0;
            for (int index = 0; index < weightedRooms.Count; index++)
            {
                current += weightedRooms[index].Weight;
                if (current >= randomTarget)
                    return weightedRooms[index];
            }
        }

        Debug.LogWarning("Returning first as default");
        return weightedRooms[0];
    }
    #endregion

}
