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

        List<Room> validRooms = GetValidRooms();

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
        }
        else
        {
            //Debug.Log($"[ CONNECTION POINT ] {gameObject.name}: No Valid Placements, disabling connector");
            EnableConnector(false);
        }
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
