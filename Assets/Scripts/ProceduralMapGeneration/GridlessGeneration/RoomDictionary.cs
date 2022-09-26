using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/RoomDictionary")]
public class RoomDictionary : ScriptableObject
{
    public List<RoomList> RoomLists;
}

[CreateAssetMenu(menuName = "ScriptableObjects/RoomList")]
public class RoomList : ScriptableObject
{
    public Room.RoomType RoomType; 
    public List<Room> Rooms;
}
