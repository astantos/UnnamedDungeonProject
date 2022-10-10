using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/RoomDictionary")]
public class RoomDictionary : ScriptableObject
{
    public List<RoomList> RoomLists;
}
