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

    public List<ConnectionPoint> ConnectionPoints;

    public bool SpawnComplete { get; protected set; }
    public bool PlacementValid { get; protected set; }
    protected Dictionary<Transform, int> currentCollisions;

    protected void Start()
    {
        currentCollisions = new Dictionary<Transform, int>();
    }

    public void RegisterNewCollision(Transform col)
    {
        if (currentCollisions.ContainsKey(col) == false)
            currentCollisions.Add(col, 0);
    }

    public void UnregisterCollision(Transform col)
    {
        if (currentCollisions.ContainsKey(col))
            currentCollisions.Remove(col);
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
            yield return ConnectionPoints[index].Spawn();
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

        List<ConnectionPoint> validPoints = new List<ConnectionPoint>(ConnectionPoints);
        while (validPoints.Count > 0)
        {
            ConnectionPoint connectPoint = validPoints[Random.Range(0, validPoints.Count)];

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
            transform.Rotate( 0, -rotationOffset, 0 );
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
            yield return YieldPool.Inst.GetWaitForSeconds(0.5f);

            if (currentCollisions.Count > 0)
            {
                Debug.Log("[ WARNING ] Collision Detected, will attempt to change orientation");
                validPoints.Remove(connectPoint);
            }
            else
            {
                // Room Placement is Valid
                PlacementValid = true;
                yield break;
            }
        }
        placeRoutine = null;
    }

}
