using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    public Camera MainCamera;
    public BasicMovement Movement;

    protected Coroutine movementRoutine;
    protected Coroutine lookRoutine;

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
        RequestPlayerRegistration();

        if (movementRoutine == null) movementRoutine = StartCoroutine(MovementInputRoutine());
        if (lookRoutine == null) lookRoutine = StartCoroutine(LookInputRoutine());
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

    protected IEnumerator LookInputRoutine()
    {
        Cursor.lockState = CursorLockMode.Locked;
        while (true)
        {
            Vector2 mouseAxis = new Vector2
            (
                Input.GetAxis("Mouse X"),
                -Input.GetAxis("Mouse Y")
            );

            MainCamera.transform.localRotation = Quaternion.Euler
            (
                MainCamera.transform.localRotation.eulerAngles.x + mouseAxis.y,
                MainCamera.transform.localRotation.eulerAngles.y,
                MainCamera.transform.localRotation.eulerAngles.z
            );

            transform.localRotation = Quaternion.Euler
            (
                transform.localRotation.eulerAngles.x,
                transform.localRotation.eulerAngles.y + mouseAxis.x,
                transform.localRotation.eulerAngles.z
            );

            yield return null;
        }    
    }

    #region Server
    [Command]
    public void RequestPlayerRegistration(NetworkConnectionToClient conn = null)
    {
        bool success = GameManager.Inst.RegisterPlayer(gameObject);
        PlayerRegistrationResponse(conn, success);
    }

    [TargetRpc]
    public void PlayerRegistrationResponse(NetworkConnection conn, bool success)
    {
        Debug.Log($"[ CLIENT ] Registration Response {(success ? "SUCCESS" : "FAILURE")}");
    }
    #endregion
}
