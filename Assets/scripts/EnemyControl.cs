using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    public int hp = 100;
    public GameObject bombEffecct;

    // 追击相关变量
    [Header("追击参数")]
    public float chaseSpeed = 3f;           // 追击移动速度
    public float detectionRange = 15f;     // 检测玩家的范围
    public float attackRange = 2f;          // 攻击范围（近身碰撞触发伤害）
    public int attackDamage = 10;           // 怪物每次攻击造成的伤害
    public float attackCooldown = 1.5f;      // 攻击冷却时间（秒）
    private Transform playerTransform;      // 玩家位置引用
    private float lastAttackTime = -999f;   // 上次攻击时间
    private bool isChasing = false;         // 是否正在追击

    void Start()
    {
        // 查找玩家对象
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("EnemyControl: 未找到标签为 'Player' 的对象，追击功能将无法工作。");
        }
    }

    void Update()
    {
        // 追击逻辑
        if (playerTransform != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            // 在检测范围内，开始追击
            if (distanceToPlayer <= detectionRange)
            {
                isChasing = true;
            }

            // 正在追击时，朝玩家移动
            if (isChasing && distanceToPlayer > attackRange)
            {
                Vector3 direction = (playerTransform.position - transform.position).normalized;
                transform.position += direction * chaseSpeed * Time.deltaTime;

                // 让怪物面朝玩家方向
                Quaternion targetRotation = Quaternion.LookRotation(playerTransform.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
            }

            // 在攻击范围内且冷却完毕，发动攻击
            if (isChasing && distanceToPlayer <= attackRange)
            {
                TryAttack();
            }

            // 玩家超出追击范围太远时放弃追击
            if (isChasing && distanceToPlayer > detectionRange * 2f)
            {
                isChasing = false;
            }
        }
    }

    // 尝试攻击玩家
    private void TryAttack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            PlayerControl playerControl = playerTransform.GetComponent<PlayerControl>();
            if (playerControl != null)
            {
                playerControl.TakeDamage(attackDamage);
            }
        }
    }

    public void GetHit(int damage)
    {
        hp -= damage;
        hp = Mathf.Max(0, hp);

        if(hp <= 0)
        {
            // 爆炸效果
            // 爆炸特效在2秒后自动销毁
            GameObject effect = Instantiate(bombEffecct, transform.position, transform.rotation);
            Destroy(effect, 2f);
            Destroy(gameObject);
        }
    }
}
