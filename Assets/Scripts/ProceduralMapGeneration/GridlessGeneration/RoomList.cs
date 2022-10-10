using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/RoomList")]
public class RoomList : ScriptableObject
{
    [Serializable]
    public struct WeightedRoom 
    {
        public Room Room;
        public int Weight;
    }

    public Room.RoomType RoomType;
    public List<WeightedRoom> Rooms;

}
