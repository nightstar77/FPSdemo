using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 屏幕中心十字准星
/// 使用 Unity UI 在屏幕中心绘制十字准星
/// </summary>
public class Crosshair : MonoBehaviour
{
    [Header("准星样式")]
    public Color crosshairColor = Color.white;  // 准星颜色
    public float crosshairSize = 10f;           // 准星半边长度
    public float lineWidth = 2f;                // 线条宽度

    [Header("准星偏移")]
    public float gap = 4f;                      // 中心留空的间隙

    private RectTransform canvasRect;
    private Image lineTop;
    private Image lineBottom;
    private Image lineLeft;
    private Image lineRight;

    void Start()
    {
        // 初始化时创建准星 UI 画布
        CreateCrosshairCanvas();
    }

    // 创建准星 UI 画布和四条线段
    private void CreateCrosshairCanvas()
    {
        // 创建 Canvas
        GameObject canvasGO = new GameObject("CrosshairCanvas");
        canvasGO.transform.SetParent(transform);

        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;  // 确保在最上层

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasGO.AddComponent<GraphicRaycaster>();

        canvasRect = canvas.GetComponent<RectTransform>();
        canvasRect.anchorMin = Vector2.zero;
        canvasRect.anchorMax = Vector2.one;
        canvasRect.sizeDelta = Vector2.zero;

        // 创建四条线
        lineTop = CreateLine("LineTop");
        lineBottom = CreateLine("LineBottom");
        lineLeft = CreateLine("LineLeft");
        lineRight = CreateLine("LineRight");

        UpdateCrosshairPosition();
    }

    private Image CreateLine(string name)
    {
        GameObject lineGO = new GameObject(name);
        lineGO.transform.SetParent(canvasRect);

        Image img = lineGO.AddComponent<Image>();
        img.color = crosshairColor;

        RectTransform rect = img.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);

        return img;
    }

    private void UpdateCrosshairPosition()
    {
        // 上
        SetLineRect(lineTop, 0, -(gap + crosshairSize / 2), lineWidth, crosshairSize);
        // 下
        SetLineRect(lineBottom, 0, gap + crosshairSize / 2, lineWidth, crosshairSize);
        // 左
        SetLineRect(lineLeft, -(gap + crosshairSize / 2), 0, crosshairSize, lineWidth);
        // 右
        SetLineRect(lineRight, gap + crosshairSize / 2, 0, crosshairSize, lineWidth);
    }

    private void SetLineRect(Image line, float x, float y, float width, float height)
    {
        RectTransform rect = line.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(x, y);
        rect.sizeDelta = new Vector2(width, height);
    }

    void Update()
    {
        // 可以在这里添加动态效果，比如射击时准星扩散
    }
}
