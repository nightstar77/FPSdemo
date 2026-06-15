using UnityEngine;

public class WeaponControl : MonoBehaviour
{
    //武器发射点
    public GameObject FirePoint;
    //武器子弹预设
    public GameObject BulletPre;
    //枪火效果
    public GameObject FirePre;
    //时间间隔
    public float bulletInterval = 0.3f;
    private float timer = 0;
    private PlayerControl pc;
    private RecoilControl rc;

    // 武器后坐力同时控制摄像机和 FirePoint
    [Header("后坐力参数")]
    public float recoilX = -3f;             // 每次射击上跳角度
    public float recoilSpeed = 10;          // 后坐力恢复速度
    public float recoilReturnSpeed = 5;     // 后坐力回归速度
    private float recoilTargetRotation = 0;
    private float recoilCurrentRotation = 0;

    private Transform cameraTransform;      // 摄像机引用
    private float originalCameraX = 0f;     // 摄像机原始X旋转

    void Start()
    {
        pc = GetComponent<PlayerControl>();
        rc = GetComponent<RecoilControl>();

        // 获取摄像机引用
        cameraTransform = GetComponentInChildren<Camera>()?.transform;
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main?.transform;
        }

        // 禁用旧的 RecoilControl，改由 WeaponControl 统一控制
        if (rc != null)
        {
            rc.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 更新后坐力，同时应用到摄像机和 FirePoint
        recoilTargetRotation = Mathf.Lerp(recoilTargetRotation, 0, recoilReturnSpeed * Time.deltaTime);
        recoilCurrentRotation = Mathf.Lerp(recoilCurrentRotation, recoilTargetRotation, recoilSpeed * Time.deltaTime);

        // 应用到摄像机（玩家视角看到上抬）
        if (cameraTransform != null)
        {
            cameraTransform.localRotation = Quaternion.Euler(originalCameraX + recoilCurrentRotation, 0, 0);
        }

        // 应用到 FirePoint（子弹发射方向同步）
        if (FirePoint != null)
        {
            FirePoint.transform.localRotation = Quaternion.Euler(recoilCurrentRotation, 0, 0);
        }

        timer += Time.deltaTime;
        if(Input.GetMouseButton(0) && timer >= bulletInterval && !pc.highSpeed)
        {
            timer = 0;
            //后坐力
            // 增加后坐力
            recoilTargetRotation += recoilX;

            //创建子弹
            // 从对象池获取子弹，替代 Instantiate
            if (BulletPool.Instance != null)
            {
                GameObject bullet = BulletPool.Instance.GetBullet();
                bullet.transform.SetParent(null);   // 从对象池父节点移出，避免坐标变换问题

                // 直接设置位置和旋转，然后启动子弹
                bullet.transform.position = FirePoint.transform.position;
                bullet.transform.rotation = FirePoint.transform.rotation;
                bullet.GetComponent<BulletControl>()?.Launch();
            }
            else
            {
                // 如果对象池不存在，回退到 Instantiate
                Instantiate(BulletPre, FirePoint.transform.position, FirePoint.transform.rotation);
            }
            //显示效果
            Destroy(Instantiate(FirePre, FirePoint.transform.position, FirePoint.transform.rotation), 0.1f);
        }

    }
}
