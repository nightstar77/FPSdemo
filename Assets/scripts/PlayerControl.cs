using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [Header("组件")]
    private Rigidbody rb;
    private Animator ani;
    private float xRotation = 0f;
    private Vector3 velocity;
    private bool jump = false;

    [Header("移动变量")]
    public float speed = 3f;
    public float jumpForce = 5f;

    [Header("灵敏度")]
    public float xSensitivity = 10f;
    public float ySensitivity = 10f;

    [HideInInspector]
    public bool highSpeed = false;
    [HideInInspector]
    public bool isAiming = false;

    // 玩家生命值相关变量
    [Header("生命值")]
    public int maxHp = 100;               // 玩家最大生命值
    public int currentHp;                  // 玩家当前生命值

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ani = GetComponentInChildren<Animator>();
        Cursor.lockState = CursorLockMode.Locked;

        // 初始化生命值
        currentHp = maxHp;
    }

    
    void Update()
    {
        Aim();
        Mouse();
        HighSpeed();
        Move();
        Jump();
    }


    void Aim()
    {
        if (Input.GetMouseButton(1))
        {
            isAiming = true;
            ani.SetBool("Aim", true);
            float aim = ani.GetFloat("Aiming");
            ani.SetFloat("Aiming", Mathf.Lerp(aim, 1, 0.1f));
        }
        else
        {
            isAiming = false;
            ani.SetBool("Aim", false);
            float aim = ani.GetFloat("Aiming");
            ani.SetFloat("Aiming", Mathf.Lerp(aim, 0, 0.1f));
        }
    }

    // 鼠标旋转
    void Mouse()
    {
        //上下旋转
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");

        xRotation -= y * ySensitivity;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        ani.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        //左右旋转
        transform.Rotate(Vector3.up * x * xSensitivity);
    }
    // 移动
    void Move()
    {
        //获取输入
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        //创建向量
        Vector3 dir = (transform.forward * vertical + transform.right * horizontal).normalized;
        //速度
        velocity = dir * speed;
        velocity.y = rb.linearVelocity.y;
        //移动动画
        ani.SetFloat("Movement",dir.magnitude);
    }

    void HighSpeed()
    {
        if (Input.GetKey(KeyCode.LeftShift) && IsGround())
        {
            highSpeed = true;
            speed = 6f;
            ani.SetBool("Holstered", true);
        }
        else
        {
            highSpeed = false;
            speed = 3f;
            ani.SetBool("Holstered", false);
        }
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGround())
        {
            jump = true;
        }
    }

    public bool IsGround()
    {
        RaycastHit hit;
        bool res = Physics.Raycast(transform.position + Vector3.up * 0.2f, -Vector3.up, out hit, 0.4f,
            LayerMask.GetMask("Ground"));
        return res;
    }

    private void FixedUpdate()
    {
        if (jump)
        {
            jump = false;
            velocity.y = jumpForce;
        }
        rb.linearVelocity = velocity;
    }

    // 玩家受击函数
    /// <summary>
    /// 玩家受到伤害，由 EnemyControl 的追击攻击调用
    /// </summary>
    /// <param name="damage">受到的伤害值</param>
    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        currentHp = Mathf.Max(0, currentHp);
        Debug.Log($"玩家受到 {damage} 点伤害，剩余生命值: {currentHp}/{maxHp}");

        // 更新血条
        UpdateHealthBar();

        if (currentHp <= 0)
        {
            currentHp = 0;
            PlayerDeath();
        }
    }

    /// <summary>
    /// 玩家死亡处理
    /// </summary>
    private void PlayerDeath()
    {
        Debug.Log("玩家已死亡！");
        // 可以在这里添加死亡动画、游戏结束UI等逻辑
        // Cursor.lockState = CursorLockMode.None; // 解锁鼠标
        // Time.timeScale = 0f; // 暂停游戏
    }

    // 血条相关
    private PlayerHealthBar healthBar;

    void Awake()
    {
        // 创建屏幕血条（HUD）
        GameObject hbGO = new GameObject("PlayerHealthBar");
        healthBar = hbGO.AddComponent<PlayerHealthBar>();
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.UpdateHealth(currentHp, maxHp);
        }
    }
}
