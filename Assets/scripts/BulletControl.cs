using UnityEngine;

public class BulletControl : MonoBehaviour
{
    public float speed = 30;
    public GameObject effectPrefab;

    private Rigidbody rb;

    // 对象池相关变量
    private float lifeTimer = 0f;           // 子弹存活计时器
    private const float MAX_LIFE_TIME = 3f; // 子弹最大存活时间（防止未碰撞的子弹一直飞行）

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// 由 WeaponControl 调用，在设置好位置和旋转后启动子弹
    /// </summary>
    public void Launch()
    {
        // 同步物理系统，确保刚设置的位置和旋转立即生效
        Physics.SyncTransforms();

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(transform.forward * speed, ForceMode.Impulse);
        lifeTimer = 0f;

        // 防止子弹生成时与玩家/武器立即碰撞
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Collider bulletCollider = GetComponent<Collider>();
            if (bulletCollider != null)
            {
                Collider[] playerColliders = player.GetComponentsInChildren<Collider>();
                foreach (Collider col in playerColliders)
                {
                    Physics.IgnoreCollision(bulletCollider, col, true);
                }
                CancelInvoke(nameof(RestorePlayerCollision));
                Invoke(nameof(RestorePlayerCollision), 0.1f);
            }
        }
    }

    // 恢复与玩家的碰撞
    private void RestorePlayerCollision()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Collider bulletCollider = GetComponent<Collider>();
            if (bulletCollider != null)
            {
                Collider[] playerColliders = player.GetComponentsInChildren<Collider>();
                foreach (Collider col in playerColliders)
                {
                    Physics.IgnoreCollision(bulletCollider, col, false);
                }
            }
        }
    }

    void Update()
    {
        // 子弹超时自动回收
        lifeTimer += Time.deltaTime;
        if (lifeTimer >= MAX_LIFE_TIME)
        {
            ReturnToPool();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //如果碰到可破坏物体
        if(collision.gameObject.tag == "DES")
        {
            Rigidbody rbody = collision.gameObject.GetComponent<Rigidbody>();
            if(rbody == null)
            {
                rbody = collision.gameObject.AddComponent<Rigidbody>(); // 创建一个Rigidbody组件
            }
            rbody.AddForceAtPosition(transform.forward * 100, collision.contacts[0].point, ForceMode.Impulse);
            //Destroy(collision.gameObject.GetComponent<Collider>(), 0.04f);
            Destroy(collision.gameObject, 2f);
        }

        //如果碰到敌人
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<EnemyControl>().GetHit(10);
        }

        var go = Instantiate(effectPrefab, transform.position, Quaternion.LookRotation(collision.contacts[0].normal));
        Destroy(go, 1f); // 击中效果消失

        // 碰撞后回收回对象池，不再直接 Destroy
        ReturnToPool();
    }

    /// <summary>
    /// 将当前子弹回收回对象池
    /// </summary>
    private void ReturnToPool()
    {
        if (BulletPool.Instance != null)
        {
            BulletPool.Instance.ReturnBullet(gameObject);
        }
        else
        {
            // 如果对象池不存在，回退到 Destroy
            Destroy(gameObject);
        }
    }
}
