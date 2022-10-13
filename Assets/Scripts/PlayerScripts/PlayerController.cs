using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    public Camera MainCamera;
    public BasicMovement Movement;

    protected Coroutine movementRoutine;

    public override void OnStartClient()
    {
        Debug.Log("[ CLIENT ] On Start Client");
        base.OnStartClient();
        if (!isLocalPlayer) GameObject.Destroy(MainCamera.gameObject);
    }

    public override void OnStartLocalPlayer()
    {
        Debug.Log("[ CLIENT ] Local Player started");
        base.OnStartLocalPlayer();
        if (movementRoutine == null)
            movementRoutine = StartCoroutine(MovementInputRoutine());
    }

    protected IEnumerator MovementInputRoutine()
    {
        while (true)
        {
            Vector3 direction = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) direction.z += 1;
            if (Input.GetKey(KeyCode.S)) direction.z -= 1;
            if (Input.GetKey(KeyCode.A)) direction.x -= 1;
            if (Input.GetKey(KeyCode.D)) direction.x += 1;
            Movement.SetSpeed(direction);
            yield return null;
        }
    }
}
