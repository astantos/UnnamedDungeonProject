using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralMapGenerator : MonoBehaviour
{
    public bool Generate;

    public List<GameObject> RoomPieces;

    

    private void OnValidate()
    {
        if (Generate)
        {
            Generate = false;
        }
    }
}
