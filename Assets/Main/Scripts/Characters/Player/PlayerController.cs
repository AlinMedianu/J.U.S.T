using UnityEngine;
public class PlayerController : MonoBehaviour
{
    private PlayerInterface playerInterface;
    
    public float moveSpeed;
    public float maxSpeed;

    private float currentMoveSpeed;

    private Animator animator;

    void Awake()
    {
        playerInterface = GetComponent<PlayerInterface>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        playerInterface.Aim += SetSlowMoveSpeed;
        playerInterface.LowerAim += SetNormalMoveSpeed;
    }

    private void OnDisable()
    {
        playerInterface.Aim -= SetSlowMoveSpeed;
        playerInterface.LowerAim -= SetNormalMoveSpeed;
        SetNormalMoveSpeed();
    }

    private void Start()
    {
        SetNormalMoveSpeed();
    }

    void Update()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        MovePlayer();
        RotatePlayer();
    }

    public void MovePlayer()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 lookDirection = transform.forward.normalized;
        lookDirection.y = 0;
        Vector3 moveDirection = (new Vector3(moveX, 0, moveZ));
        //moveDirection *= Time.deltaTime;

        float dotProductToForward = Vector3.Dot(lookDirection, moveDirection.normalized);
        if (dotProductToForward < -0.5)
        {
            transform.Translate(moveDirection.normalized * (currentMoveSpeed / 2) * Time.deltaTime, Space.World);
        }
        else
        {
            transform.Translate(moveDirection.normalized * currentMoveSpeed * Time.deltaTime, Space.World);
        }

        Vector3 crossProductProducingRight = Vector3.Cross(Vector3.up, lookDirection);


        float lateralMovement = Vector3.Dot(moveDirection, crossProductProducingRight);

        if (!(moveX == 0 && moveZ == 0))
        {
            animator.SetBool("walking", true);
            animator.SetFloat("Left-Right Blend", lateralMovement);
            animator.SetFloat("Forward-Back Blend", dotProductToForward);
        }
        else
        {
            animator.SetBool("walking", false);
        }
    }

    public void RotatePlayer()
    {
        Vector3 camPos = Camera.main.transform.position;
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.transform.position.y;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3 direction = (mousePos - camPos);

        int layerMask = 1 << 9;

        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(camPos, direction, out hit, 100, layerMask))
        {
            Vector3 lookPos = hit.point;
            lookPos.y = transform.position.y;
            transform.LookAt(lookPos);
        }
    }

    public void SetSlowMoveSpeed()
    {
        currentMoveSpeed = moveSpeed / 3;
    }

    public void SetNormalMoveSpeed()
    {
        currentMoveSpeed = moveSpeed;
    }
}
