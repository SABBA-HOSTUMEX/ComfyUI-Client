using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ConditioningVisualizer : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI outputText;
    [SerializeField] private ConditioningMatrixLoader matrixLoader;
    [SerializeField] private Gradient importanceColorGradient;

    [Header("Gradient Bar References")]
    [SerializeField] private RawImage gradientBar;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private RectTransform arrowContainer;

    private List<ConditioningMatrixLoader.TokenInfo> tokenData;
    private float minImportance, maxImportance;
    private List<GameObject> arrows = new List<GameObject>();
    [HideInInspector]
    public int donecon;
    public icon Icon;
    private Dictionary<float, List<ConditioningMatrixLoader.TokenInfo>> groupedTokens;

    public void UpdateAfterLoad()
    {
        ClearArrows();
        LoadConditioningData();
        UpdateVisualization();
    }

    private void LoadConditioningData()
    {
        if (matrixLoader == null)
        {
            Debug.LogError("Matrix Loader reference not set!");
            return;
        }

        matrixLoader.GetTokenData(out tokenData, out minImportance, out maxImportance);
        CreateGradientTexture();

        // 按importance值分组
        groupedTokens = tokenData
            .GroupBy(t => t.Importance)
            .ToDictionary(g => g.Key, g => g.ToList());

        Debug.Log($"Loaded {tokenData.Count} tokens with importance range: {minImportance} - {maxImportance}");
    }

    private void ClearArrows()
    {
        foreach (var arrow in arrows)
        {
            if (arrow != null)
                Destroy(arrow);
        }
        arrows.Clear();
        arrowPrefab.SetActive(true);
    }

    private void CreateGradientTexture()
    {
        int width = 256;
        Texture2D gradientTexture = new Texture2D(width, 1);

        for (int i = 0; i < width; i++)
        {
            float t = i / (float)(width - 1);
            gradientTexture.SetPixel(i, 0, importanceColorGradient.Evaluate(t));
        }

        gradientTexture.Apply();
        if (gradientBar != null)
        {
            gradientBar.texture = gradientTexture;
        }
    }

    private void CreateArrowForTokenGroup(float importance, List<ConditioningMatrixLoader.TokenInfo> tokens, float normalizedPosition)
    {
        if (arrowPrefab == null || arrowContainer == null || gradientBar == null)
        {
            Debug.LogError("Missing required references for arrow creation");
            return;
        }

        GameObject arrow = Instantiate(arrowPrefab, arrowContainer, false);
        arrows.Add(arrow);
        arrow.name = $"Arrow_Group_{importance}";

        RectTransform arrowRect = arrow.GetComponent<RectTransform>();
        if (arrowRect != null)
        {
            float barWidth = gradientBar.rectTransform.rect.width;
            float xPosition = normalizedPosition * barWidth;

            arrowRect.anchorMin = new Vector2(0, 0.5f);
            arrowRect.anchorMax = new Vector2(0, 0.5f);
            arrowRect.pivot = new Vector2(0.5f, 0.5f);
            arrowRect.anchoredPosition = new Vector2(xPosition, -77f);
        }

        // 为所有相同权重的token创建标签文本
        TextMeshProUGUI label = arrow.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null)
        {
            StringBuilder labelText = new StringBuilder();
            foreach (var token in tokens)
            {
                labelText.AppendLine(token.Text);
            }
            labelText.Append($"{importance:F2}"); // 最后添加权重值
            label.text = labelText.ToString();
        }
    }

    private void UpdateVisualization()
    {
        if (tokenData == null || tokenData.Count == 0)
        {
            Debug.LogWarning("No token data available");
            return;
        }

        StringBuilder richText = new StringBuilder();

        // 为每个重要性值创建一个箭头
        foreach (var group in groupedTokens.OrderBy(g => g.Key))
        {
            float normalizedImportance = Mathf.InverseLerp(minImportance, maxImportance, group.Key);
            Color color = importanceColorGradient.Evaluate(normalizedImportance);

            // 为该组所有token添加彩色文本
            foreach (var token in group.Value)
            {
                richText.Append($"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{token.Text}</color> ");
            }

            // 为该组创建一个箭头
            CreateArrowForTokenGroup(group.Key, group.Value, normalizedImportance);
        }

        if (outputText != null)
        {
            outputText.text = richText.ToString().Trim();
        }

        arrowPrefab.SetActive(false);
        donecon = 1;
        Icon.changeui(0);
    }
}