using UnityEngine;
using Photon.Pun;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Vector3 offset;
    public float smoothAmount;
    private Transform target;
    private Vector3 playerForward;
    private bool movedDown;
    private bool focused;

    void Start()
    {
        focused = false;
    }

    void LateUpdate()
    {
        if (target)
        {
            transform.position = Vector3.Slerp(transform.position, target.position + offset, smoothAmount * Time.deltaTime);
            if (Mathf.Abs(target.rotation.y) >= 0.85f && !movedDown)
            {
                offset.z -= 2;
                movedDown = true;
            }
            else if (Mathf.Abs(target.rotation.y) < 0.85f && movedDown)
            {
                offset.z += 2;
                movedDown = false;
            }
        }
        else
        {
            target = GameObject.Find(PhotonNetwork.NickName).transform;

        }
    }

    public void FocusCamera()
    {
        Debug.Log("focus");
        if (!focused)
        {
            playerForward = target.forward;
            offset += playerForward * 2f + transform.forward;
            focused = true;
        }
        //offset += playerForward * 2f;
    }

    public void UnfocusCamera()
    {
        Debug.Log("unfocus");
        if (focused)
        {
            offset -= playerForward * 2f + transform.forward;
            focused = false;
        }
        //offset -= playerForward * 2f;
    }
}