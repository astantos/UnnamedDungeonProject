using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CharacterController : NetworkBehaviour
{
    public BasicMovement Movement;

    protected Coroutine movementRoutine;

    public void Update()
    {
        if(isLocalPlayer && movementRoutine == null)
        {
            movementRoutine = StartCoroutine(MovementInputRoutine());
        }
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
            Debug.Log(direction);
            yield return null;
        }
    }
}
