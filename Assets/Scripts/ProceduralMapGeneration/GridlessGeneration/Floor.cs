using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    public RoomValidator ValidatorPrefab;
    public Room room;
    public Transform ValidatorParent;

    public void Initialize()
    {
        RoomValidator validator = Instantiate(ValidatorPrefab, ValidatorParent);
        validator.Setup(transform.position, gameObject.GetComponent<Renderer>().bounds.size, room);
    }
}
