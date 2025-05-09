using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    PlayerStats playerStats;

    [HideInInspector]
    public Vector3 moveDir;
    [HideInInspector]
    public float _horizontalVector;
    [HideInInspector]
    public float _VerticalVector;
    [HideInInspector]
    public Vector2 lastMove;
    [SerializeField] float speed;

    Animator animator;

    private void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (rb == null || playerStats == null || animator == null)
        {
            Debug.LogError("Missing required components on Player!");
            enabled = false;
            return;
        }

        moveDir = new Vector3();
        lastMove = new Vector2(1f, 0f);
        ResetPlayerState(); // Khởi tạo trạng thái ban đầu
    }

    private void Update()
    {
        if (GameManager.instance.isGameOver) return;
        InputManagement();
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.isGameOver) return;
        Move();
    }

    private void InputManagement()
    {
        moveDir.x = Input.GetAxisRaw("Horizontal");
        moveDir.y = Input.GetAxisRaw("Vertical");

        // Chuẩn hóa vector để đảm bảo vận tốc đồng đều
        moveDir = new Vector3(moveDir.x, moveDir.y).normalized;

        // Cập nhật hướng di chuyển
        if (moveDir.x != 0)
        {
            _horizontalVector = moveDir.x;
            lastMove = new Vector2(_horizontalVector, 0f);
        }
        if (moveDir.y != 0)
        {
            _VerticalVector = moveDir.y;
            lastMove = new Vector2(0f, _VerticalVector);
        }
        if (moveDir.x != 0 && moveDir.y != 0)
        {
            lastMove = new Vector2(_horizontalVector, _VerticalVector);
        }

        // Cập nhật Animator
        animator.SetBool("Move", moveDir != Vector3.zero);
    }

    private void Move()
    {
        rb.velocity = new Vector3(moveDir.x * playerStats.CurrentMoveSpeed, moveDir.y * playerStats.CurrentMoveSpeed);
    }

    // Phương thức reset trạng thái của Player
    public void ResetPlayerState()
    {
        moveDir = Vector3.zero;
        _horizontalVector = 0f;
        _VerticalVector = 0f;
        lastMove = new Vector2(1f, 0f); // Hướng mặc định
        rb.velocity = Vector2.zero; // Reset vận tốc
        animator.SetBool("Move", false); // Dừng animation
    }
}