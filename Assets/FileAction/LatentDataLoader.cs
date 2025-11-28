using UnityEngine;
using System.IO;
using System;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;

public class LatentDataLoader : MonoBehaviour
{
    public FeatureVisualizer featureVisualizer;
    public Dictionary<int, float[,,]> stepChannelData { get; private set; }  // step number -> channel data
    public int dataWidth { get; private set; }
    public int dataHeight { get; private set; }
    public int channelCount { get; private set; }
    public bool IsLoadingComplete { get; private set; }

    private const string StepStartTag = "=== Step ";
    private const string ShapeTag = "Shape: ";
    private const string ArrayDataTag = "Array Data:";
    private string path = @"C:\Users\admin\Desktop\ComfyUI_windows_portable\ComfyUI\output\LatentData\latent_20250212_102600.txt";
    private void Start()
    {
        //LoadLatentData(path);
    }
    public bool LoadLatentData(string filePath)
    {
        try
        {
            string[] lines = File.ReadAllLines(filePath);
            stepChannelData = new Dictionary<int, float[,,]>();
            int currentStep = -1;
            int[] shape = null;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                // 找到新的 step
                if (line.StartsWith(StepStartTag))
                {
                    string stepStr = line.Replace(StepStartTag, "").Replace("===", "").Trim();
                    currentStep = int.Parse(stepStr);
                }

                // 讀取 Shape 資訊
                if (line.Contains(ShapeTag))
                {
                    string shapeStr = line.Replace(ShapeTag, "").Trim('(', ')');
                    shape = shapeStr.Split(',').Select(s => int.Parse(s.Trim())).ToArray();

                    // 設定維度資訊
                    channelCount = shape[1];
                    dataHeight = shape[2];
                    dataWidth = shape[3];
                }

                // 讀取陣列資料
                if (line.StartsWith(ArrayDataTag) && currentStep != -1 && shape != null)
                {
                    string dataLine = lines[i + 1];
                    float[] rawData = dataLine.Split(',')
                                            .Select(s => float.Parse(s.Trim(), CultureInfo.InvariantCulture))
                                            .ToArray();

                    // 建立該 step 的 channel data
                    float[,,] channelData = new float[channelCount, dataHeight, dataWidth];
                    int index = 0;

                    // 重組三維陣列
                    for (int c = 0; c < channelCount; c++)
                    {
                        for (int h = 0; h < dataHeight; h++)
                        {
                            for (int w = 0; w < dataWidth; w++)
                            {
                                channelData[c, h, w] = rawData[index++];
                            }
                        }
                    }

                    stepChannelData[currentStep] = channelData;
                    Debug.Log($"Loaded Step {currentStep} data");
                }
            }

            Debug.Log($"Successfully loaded {stepChannelData.Count} steps of {channelCount} channels of {dataWidth}x{dataHeight} data");
            featureVisualizer.StartVisualize();
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading latent data: {e.Message}");
            return false;
        }
    }

    public float[,,] GetStepData(int step)
    {
        if (stepChannelData.ContainsKey(step))
        {
            return stepChannelData[step];
        }
        return null;
    }

    public int[] GetAvailableSteps()
    {
        return stepChannelData.Keys.OrderBy(k => k).ToArray();
    }
}