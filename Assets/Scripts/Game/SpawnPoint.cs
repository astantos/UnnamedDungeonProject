using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public int SpawnHeight;

    public void SetPosition(Vector3 position)
    {
        transform.position = new Vector3(
            position.x,
            position.y + SpawnHeight,
            position.z
        );
    }
}
