using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 玩家屏幕血条（HUD）
/// 显示在屏幕左上角，固定位置
/// </summary>
public class PlayerHealthBar : MonoBehaviour
{
    [Header("UI组件")]
    public Image fillImage;              // 填充图片
    public Text hpText;                  // 血量文字（如 80/100）

    [Header("样式")]
    public Vector2 barSize = new Vector2(200, 20);  // 血条尺寸
    public Vector2 position = new Vector2(20, -20); // 距离左上角的偏移

    private Canvas canvas;
    private RectTransform rectTransform;

    // 在Awake阶段创建玩家血条UI
    void Awake()
    {
        CreateHealthBar();
    }

    private void CreateHealthBar()
    {
        // 创建Canvas（屏幕空间覆盖）
        GameObject canvasGO = new GameObject("PlayerHealthBarCanvas");
        canvasGO.transform.SetParent(transform);
        canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // 创建血条背景
        GameObject bgGO = new GameObject("Background");
        bgGO.transform.SetParent(canvasGO.transform);
        Image bgImage = bgGO.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

        RectTransform bgRect = bgGO.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0, 1);  // 左上角锚点
        bgRect.anchorMax = new Vector2(0, 1);
        bgRect.pivot = new Vector2(0, 1);
        bgRect.anchoredPosition = position;
        bgRect.sizeDelta = barSize;

        // 创建填充
        GameObject fillGO = new GameObject("Fill");
        fillGO.transform.SetParent(bgGO.transform);
        fillImage = fillGO.AddComponent<Image>();
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        fillImage.fillOrigin = 0;
        fillImage.color = Color.green;

        RectTransform fillRect = fillGO.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = Vector2.zero;
        fillRect.anchoredPosition = Vector2.zero;

        // 创建文字
        GameObject textGO = new GameObject("HpText");
        textGO.transform.SetParent(bgGO.transform);
        hpText = textGO.AddComponent<Text>();
        hpText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        hpText.fontSize = 14;
        hpText.color = Color.white;
        hpText.alignment = TextAnchor.MiddleCenter;

        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;
    }

    // 更新血条显示，根据当前血量与最大血量刷新填充比例、颜色及文字
    public void UpdateHealth(float currentHp, float maxHp)
    {
        if (fillImage != null)
        {
            float ratio = Mathf.Clamp01(currentHp / maxHp);
            fillImage.fillAmount = ratio;

            // 根据血量改变颜色
            if (ratio > 0.5f)
                fillImage.color = Color.green;
            else if (ratio > 0.2f)
                fillImage.color = Color.yellow;
            else
                fillImage.color = Color.red;
        }

        if (hpText != null)
        {
            hpText.text = $"{currentHp}/{maxHp}";
        }
    }
}
