using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 子弹对象池管理器
/// 统一管理子弹的预创建、获取和回收，避免频繁的 Instantiate/Destroy
/// </summary>
public class BulletPool : MonoBehaviour
{
    // ====== 单例模式 ======
    public static BulletPool Instance { get; private set; }

    [Header("对象池配置")]
    public GameObject bulletPrefab;         // 子弹预制体
    public int poolSize = 50;               // 池的初始容量
    public Transform poolParent;            // 池对象的父节点（可选，用于Hierarchy整理）

    private Queue<GameObject> bulletPool = new Queue<GameObject>();

    void Awake()
    {
        // 单例初始化
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 预创建子弹对象
        InitializePool();
    }

    /// <summary>
    /// 预创建对象池中的所有子弹
    /// </summary>
    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewBullet();
        }
    }

    /// <summary>
    /// 创建一个新的子弹并加入池中
    /// </summary>
    private void CreateNewBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab);
        bullet.SetActive(false);

        if (poolParent != null)
        {
            bullet.transform.SetParent(poolParent);
        }

        bulletPool.Enqueue(bullet);
    }

    /// <summary>
    /// 从对象池中获取一个子弹
    /// </summary>
    /// <returns>可用的子弹 GameObject</returns>
    public GameObject GetBullet()
    {
        // 如果池空了，动态扩容
        if (bulletPool.Count == 0)
        {
            CreateNewBullet();
        }

        GameObject bullet = bulletPool.Dequeue();

        // 注意：位置和旋转由 WeaponControl 设置，这里只负责激活
        bullet.SetActive(true);
        return bullet;
    }

    /// <summary>
    /// 将子弹回收回对象池
    /// </summary>
    /// <param name="bullet">要回收的子弹对象</param>
    public void ReturnBullet(GameObject bullet)
    {
        if (bullet == null) return;

        bullet.SetActive(false);

        // 重置子弹状态（防止残留物理速度等）
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            // 将子弹移到一个远离场景的位置，避免残留碰撞
            bullet.transform.position = new Vector3(0, -1000, 0);
        }

        if (poolParent != null)
        {
            bullet.transform.SetParent(poolParent);
        }

        bulletPool.Enqueue(bullet);
    }
}
