using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 胜利条件管理器
/// 检测场景中所有怪物是否被消灭，全部消灭后触发胜利界面
/// </summary>
public class VictoryManager : MonoBehaviour
{
    [Header("胜利配置")]
    public List<GameObject> initialEnemies = new List<GameObject>();  // 初始放置的怪物
    public VictoryUI victoryUI;                                        // 胜利界面UI

    // 当前存活怪物数量
    private int aliveEnemyCount = 0;
    private bool victoryTriggered = false;

    void Start()
    {
        // 初始化存活数量
        // 过滤掉空引用（有些怪物可能未赋值）
        initialEnemies.RemoveAll(e => e == null);
        aliveEnemyCount = initialEnemies.Count;

        if (aliveEnemyCount == 0)
        {
            Debug.LogWarning("VictoryManager: 初始怪物列表为空，请把场景中的怪物拖到列表中。");
        }
    }

    void Update()
    {
        // 每帧检查存活数量
        if (!victoryTriggered && aliveEnemyCount > 0)
        {
            // 统计实际存活的怪物
            int currentAlive = 0;
            foreach (GameObject enemy in initialEnemies)
            {
                if (enemy != null)
                {
                    currentAlive++;
                }
            }

            // 如果数量变化了（有怪物死亡）
            if (currentAlive != aliveEnemyCount)
            {
                aliveEnemyCount = currentAlive;
                Debug.Log($"剩余怪物: {aliveEnemyCount}");

                // 全部消灭，触发胜利
                if (aliveEnemyCount == 0)
                {
                    TriggerVictory();
                }
            }
        }
    }

    // 触发胜利
    private void TriggerVictory()
    {
        victoryTriggered = true;
        Debug.Log("胜利！所有怪物已被消灭！");

        // 解锁鼠标
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 显示胜利界面
        if (victoryUI != null)
        {
            victoryUI.ShowVictory();
        }
    }
}
