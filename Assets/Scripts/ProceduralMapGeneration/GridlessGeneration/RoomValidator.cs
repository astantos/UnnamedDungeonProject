using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomValidator : MonoBehaviour
{
    protected BoxCollider boxCollider;

    protected Room room; 

    public void Setup(Vector3 position, Vector3 size, Room room)
    {
        boxCollider = GetComponent<BoxCollider>();

        boxCollider.size = size;
        transform.position = position;

        this.room = room;
    }

    public void OnTriggerEnter(Collider collider)
    {
        room.RegisterNewCollision(collider.transform);
        Debug.LogWarning("!!!!!!!!!!!! COLLISION ");
    }

    public void OnTriggerExit(Collider collider)
    {
        room.UnregisterCollision(collider.transform);
    }
}
