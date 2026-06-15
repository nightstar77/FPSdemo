using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 胜利界面UI
/// 显示祝贺信息，提供再次游玩和退出游戏选项
/// </summary>
public class VictoryUI : MonoBehaviour
{
    [Header("UI组件")]
    public GameObject victoryPanel;         // 胜利面板（包含所有UI元素）
    public Text victoryText;                // 祝贺文字
    public Button restartButton;            // 再次游玩按钮
    public Button quitButton;               // 退出游戏按钮

    [Header("文字内容")]
    public string victoryMessage = "恭喜胜利！所有怪物已被消灭！";

    void Start()
    {
        // 初始隐藏胜利界面
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }

        // 绑定按钮事件
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartClicked);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitClicked);
        }
    }

    // 显示胜利界面
    public void ShowVictory()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }

        if (victoryText != null)
        {
            victoryText.text = victoryMessage;
        }

        // 暂停游戏（可选）
        // Time.timeScale = 0f;
    }

    // 再次游玩：恢复时间流速并重新加载当前场景
    private void OnRestartClicked()
    {
        Debug.Log("重新开始游戏");

        // 恢复时间流速
        Time.timeScale = 1f;

        // 重新加载当前场景
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // 退出游戏：编辑器模式下停止播放，构建后退出应用
    private void OnQuitClicked()
    {
        Debug.Log("退出游戏");

#if UNITY_EDITOR
        // 编辑器模式下停止播放
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 构建后退出游戏
        Application.Quit();
#endif
    }
}
