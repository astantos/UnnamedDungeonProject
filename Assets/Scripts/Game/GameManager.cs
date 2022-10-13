using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour 
{
    public GridBasedMapGenerator MapGeneratorPrefab;

    protected GridBasedMapGenerator mapGenerator;

    public void StartGame()
    {
        Debug.Log("Game Started");
        mapGenerator = GameObject.Instantiate(MapGeneratorPrefab);
        NetworkServer.Spawn(mapGenerator.gameObject);
        
        mapGenerator.Generate();
    }
}
