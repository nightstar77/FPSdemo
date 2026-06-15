using UnityEngine;

public class RecoilControl : MonoBehaviour
{
    public float X = -3f;
    public float speed = 10;
    public float returnSpeed = 5;

    private float targetRotation = 0;
    private float currentRotation = 0;


    // Update is called once per frame
    void Update()
    {
        //恢复旋转角度
        targetRotation = Mathf.Lerp(targetRotation, 0, returnSpeed * Time.deltaTime); // 旋转角度恢复到0
        // 计算当前旋转角度
        currentRotation = Mathf.Lerp(currentRotation, targetRotation, speed * Time.deltaTime);
        // 设置旋转角度
        transform.localRotation = Quaternion.Euler(currentRotation, transform.localEulerAngles.y, 0);
        
    }

    public void Fire()
    {
        targetRotation += X;
    }
}
