using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public enum RoomType 
    {
        Room,
        Hallway,
        Connector
    }

    public RoomType Type;

    public GameObject ConnectionPointParent;
    public GameObject FloorParent;

    public List<ConnectionPoint> ConnectionPoints;
    public List<Floor> Floors;

    public bool SpawnComplete { get; protected set; }
    public bool PlacementValid { get; protected set; }

    protected int currentCollisions;

    protected ProceduralMapGenerator generator;

    public void Initialize(ProceduralMapGenerator generator)
    {
        currentCollisions = 0;

        ConnectionPoints = new List<ConnectionPoint>();

        this.generator = generator;

        ConnectionPoint[] conArray = ConnectionPointParent.GetComponentsInChildren<ConnectionPoint>();
        for (int index = 0; index < conArray.Length; index++)
        {
            ConnectionPoints.Add(conArray[index]);
            conArray[index].Initialize(generator);
        }
        
        Floor[] floorArray = FloorParent.GetComponentsInChildren<Floor>();
        for (int index = 0; index < floorArray.Length; index++)
        {
            Floors.Add(floorArray[index]);
            floorArray[index].Initialize();
        }
    }

    public void RegisterNewCollision()
    {
        currentCollisions++; 
    }

    public void UnregisterCollision()
    {
        currentCollisions--; 
    }

    protected  Coroutine spawnRoutine;
    public Coroutine SpawnConnectedRooms()
    {
        if (spawnRoutine == null)
            spawnRoutine = StartCoroutine(SpawnRoutine());
        return spawnRoutine;
    }

    protected IEnumerator SpawnRoutine()
    {
        for (int index = 0; index < ConnectionPoints.Count; index++)
        {
            Debug.Log($"\t>>>Spawning rooms at Connection {index} of {ConnectionPoints.Count} Connection Points");
            yield return ConnectionPoints[index].Spawn();
            Debug.Log($"\t>>>Spawning rooms at Connection {index} of Complete");
        }
        spawnRoutine = null;
    }

    protected Coroutine placeRoutine;
    public Coroutine TryPlace(ConnectionPoint point)
    {
        if (placeRoutine == null)
            placeRoutine = StartCoroutine(PlaceRoutine(point));
        return placeRoutine;
    }

    protected IEnumerator PlaceRoutine(ConnectionPoint spawnPoint)
    {
        PlacementValid = false;

        List<ConnectionPoint> validPoints = new List<ConnectionPoint>();

        for (int index = 0; index < ConnectionPoints.Count; index++)
        {
            if (ConnectionPoints[index].ConnectorEnabled)
                validPoints.Add(ConnectionPoints[index]);
        }
        Debug.Log($"[ PLACEMENT ] Attempting to place Room with {validPoints.Count} connections");


        while (validPoints.Count > 0)
        {
            int randomPoint = Random.Range(0, validPoints.Count);
            Debug.Log($"[ PLACEMENT ] Picking point {randomPoint}");
            ConnectionPoint connectPoint = validPoints[randomPoint];

            // Record Current Relative Position
            Vector3 posOffset = connectPoint.transform.position - transform.position;
            float rotationOffset = connectPoint.transform.eulerAngles.y - transform.rotation.eulerAngles.y;


            // Move Connection Point to New Location
            connectPoint.transform.position = spawnPoint.transform.position;
            Vector3 newRot = spawnPoint.transform.eulerAngles + new Vector3(0, 180, 0);
            connectPoint.transform.rotation = Quaternion.Euler(newRot);

            // Move Room to New Location
            transform.position = connectPoint.transform.position;
            transform.rotation = connectPoint.transform.rotation;
            transform.Rotate(0, -rotationOffset, 0);
            transform.Translate(
                -posOffset.x,
                -posOffset.y,
                -posOffset.z
            );

            // Return Connection Point back to Original Relative Position
            connectPoint.transform.position = transform.position;
            connectPoint.transform.rotation = transform.rotation;
            connectPoint.transform.Translate(posOffset);
            connectPoint.transform.Rotate(0, rotationOffset, 0);

            // Check Validity
            yield return new WaitForFixedUpdate();
      
            if (currentCollisions > 0)
            {
                Debug.Log("[ PLACEMENT ] Collision! Will Remove Connection Point and reattempt");
                validPoints.Remove(connectPoint);
                transform.rotation = Quaternion.Euler(Vector3.zero);
            }
            else
            {
                // Room Placement is Valid
                Debug.Log("[ PLACEMENT ] Placement Complete");
                PlacementValid = true;
                connectPoint.EnableConnector(false);
                connectPoint.SetDoorwayActive(true);
                yield break;
            }
        }

        if (PlacementValid)
            Debug.Log("[ PLACEMENT ] Successfully Placed");
        else
            Debug.Log("[ PLACEMENT ] No successful placement");

        placeRoutine = null;
    }

}
