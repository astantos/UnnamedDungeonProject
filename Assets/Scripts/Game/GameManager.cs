using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    public static GameManager Inst
    {
        get
        {
            if (_inst == null)
            {
                _inst = FindObjectOfType<GameManager>();
            }

            if (_inst == null)
            {
                Debug.LogError("[ CRITICAL ERROR ] There is no GameManager present in the scene");
                return null;
            }
            return _inst;
        }
    }
    public static GameManager _inst;

    public GridBasedMapGenerator MapGeneratorPrefab;
    public SpawnPoint SpawnPointPrefab;


    [Header("Serialized Variables")]
    // Map \\
    [SerializeField] protected GridBasedMapGenerator mapGenerator;

    // Players \\
    [SerializeField] protected Player PlayerOne;
    [SerializeField] protected Player PlayerTwo;

    [SerializeField] protected SpawnPoint GroundSpawnPointOne;
    [SerializeField] protected SpawnPoint GroundSpawnPointTwo;

    public void StartGame()
    {
        Debug.Log("Game Started");
        mapGenerator = GameObject.Instantiate(MapGeneratorPrefab);
        NetworkServer.Spawn(mapGenerator.gameObject);

        mapGenerator.Generate();
        SetGroundSpawnPoints();
    }

    protected void SetGroundSpawnPoints()
    {
        GroundSpawnPointOne = GameObject.Instantiate(SpawnPointPrefab);
        GroundSpawnPointTwo = GameObject.Instantiate(SpawnPointPrefab);

        UnitRoom spawnPointRoomOne = mapGenerator.GetRandomRoom();
        UnitRoom spawnPointRoomTwo = null;
        while (spawnPointRoomTwo == null)
        {
            spawnPointRoomTwo = mapGenerator.GetRandomRoom();
            if (spawnPointRoomTwo == spawnPointRoomOne)
                spawnPointRoomTwo = null;
        }

        GroundSpawnPointOne.SetPosition(spawnPointRoomOne.transform.position);
        GroundSpawnPointTwo.SetPosition(spawnPointRoomTwo.transform.position);
    }

    public bool RegisterPlayer(GameObject player)
    {
        Debug.Log($"[ SERVER ] Client Player requesting registration");
        Player playerScript = player.GetComponent<Player>();
        bool success = false;

        if (player != null)
        {
            if (PlayerOne == null)
            {
                PlayerOne = playerScript;
                success = true;
            }
            else if (PlayerTwo == null)
            {
                PlayerTwo = playerScript;
                success = true;
            }
        }

        return success;
    }


    #region Server
    #endregion
}
