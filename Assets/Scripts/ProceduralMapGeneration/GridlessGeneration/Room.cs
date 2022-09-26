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

    public List<RoomValidator> Validators;

    public bool SpawnComplete { get; protected set; }

    protected  Coroutine spawnRoutine;
    public void SpawnConnectedRooms()
    {
        if (spawnRoutine == null)
            spawnRoutine = StartCoroutine(SpawnRoutine());
    }

    protected IEnumerator SpawnRoutine()
    {
        for (int index = 0; index < ConnectionPoints.Count; index++)
        {
            yield return ConnectionPoints[index].Spawn();
        }
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
        float rotation = spawnPoint.transform.eulerAngles.y;

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

            connectPoint.transform.position = transform.position;
            connectPoint.transform.rotation = transform.rotation;
            connectPoint.transform.Translate(posOffset);
            connectPoint.transform.Rotate(0, rotationOffset, 0);

            break;

            yield return YieldPool.Inst.WaitForEndOfFrame;
        }
        
        yield return null;
    }

}
