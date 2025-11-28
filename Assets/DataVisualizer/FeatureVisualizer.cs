using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FeatureVisualizer : MonoBehaviour
{
    [Header("UI References")]
    public RawImage[] channelDisplays;
    public TextMeshProUGUI infoText;
    public TextMeshProUGUI stepInfoText;

    [Header("Visualization Settings")]
    public Color lowValueColor = Color.blue;
    public Color midValueColor = Color.green;
    public Color highValueColor = Color.red;
    public float visualScale = 4f;

    private LatentDataLoader dataLoader;
    private Texture2D[] channelTextures;
    private int currentStep = 1;
    [HideInInspector]
    public int donelatent;
    [SerializeField] icon Icon;
    void Start()
    {
        dataLoader = GetComponent<LatentDataLoader>();
    }

    public void StartVisualize()
    {
        donelatent = 0;
        currentStep = dataLoader.GetAvailableSteps()[0];
        InitializeTextures();
        UpdateVisualization();
    }

    private void InitializeTextures()
    {
        channelTextures = new Texture2D[4];
        for (int i = 0; i < 4; i++)
        {
            if (channelDisplays[i] != null)
            {
                channelTextures[i] = new Texture2D(dataLoader.dataWidth, dataLoader.dataHeight, TextureFormat.RGBA32, false);
                channelTextures[i].filterMode = FilterMode.Point;
                channelDisplays[i].texture = channelTextures[i];
            }
        }
    }

    public void UpdateVisualization()
    {
        float[,,] currentData = dataLoader.GetStepData(currentStep);
        if (currentData == null)
        {
            Debug.LogError($"No data available for step {currentStep}!");
            return;
        }

        if (stepInfoText != null)
        {
            stepInfoText.text = $"Step: {currentStep}";
        }

        for (int channel = 0; channel < dataLoader.channelCount; channel++)
        {
            UpdateChannelVisualization(channel, currentData);
        }
    }

    private void UpdateChannelVisualization(int channel, float[,,] data)
    {
        float minVal = float.MaxValue;
        float maxVal = float.MinValue;

        for (int y = 0; y < dataLoader.dataHeight; y++)
        {
            for (int x = 0; x < dataLoader.dataWidth; x++)
            {
                float val = data[channel, y, x];
                minVal = Mathf.Min(minVal, val);
                maxVal = Mathf.Max(maxVal, val);
            }
        }

        for (int y = 0; y < dataLoader.dataHeight; y++)
        {
            for (int x = 0; x < dataLoader.dataWidth; x++)
            {
                float val = data[channel, y, x];
                float normalizedValue = Mathf.InverseLerp(minVal, maxVal, val);

                Color color;
                if (normalizedValue < 0.5f)
                {
                    color = Color.Lerp(lowValueColor, midValueColor, normalizedValue * 2);
                }
                else
                {
                    color = Color.Lerp(midValueColor, highValueColor, (normalizedValue - 0.5f) * 2);
                }

                channelTextures[channel].SetPixel(x, dataLoader.dataHeight - 1 - y, color);
            }
        }

        channelTextures[channel].Apply();
        donelatent = 1;
        Icon.changeui(1);
    }

    public void NextStep()
    {
        int[] steps = dataLoader.GetAvailableSteps();
        int currentIndex = System.Array.IndexOf(steps, currentStep);
        if (currentIndex < steps.Length - 1)
        {
            currentStep = steps[currentIndex + 1];
            UpdateVisualization();
        }
    }

    public void PreviousStep()
    {
        int[] steps = dataLoader.GetAvailableSteps();
        int currentIndex = System.Array.IndexOf(steps, currentStep);
        if (currentIndex > 0)
        {
            currentStep = steps[currentIndex - 1];
            UpdateVisualization();
        }
    }

    public void ShowFeatureInfo(int channel, Vector2 position)
    {
        if (infoText == null) return;

        float[,,] currentData = dataLoader.GetStepData(currentStep);
        if (currentData == null) return;

        int x = Mathf.FloorToInt(position.x * dataLoader.dataWidth);
        int y = Mathf.FloorToInt(position.y * dataLoader.dataHeight);

        if (x >= 0 && x < dataLoader.dataWidth && y >= 0 && y < dataLoader.dataHeight)
        {
            float value = currentData[channel, y, x];
            string info = $"Step {currentStep}\nChannel {channel}\nPosition: ({x}, {y})\nValue: {value:F4}";
            infoText.text = info;
        }
    }
}