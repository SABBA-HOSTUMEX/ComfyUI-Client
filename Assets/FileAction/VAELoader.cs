using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class VAELoader : MonoBehaviour
{
    // 儲存latent array的資料結構
    private float[,,,] latentArray;
    private int[] shape = new int[4]; // (1, 4, 64, 64)
    private float mean;
    private float std;
    private float min;
    private float max;
    [SerializeField]
    private VAEVisualizer vAEVisualizer;
    // Getter方法供其他腳本使用
    public float[,,,] GetLatentArray() => latentArray;
    public float GetMean() => mean;
    public float GetStd() => std;
    public float GetMin() => min;
    public float GetMax() => max;
    public int[] GetShape() => shape;

    //private string filePath = @"C:\Users\admin\Pictures\AppData\VAEData\vae_debug_20250213_110239.txt";

    void Start()
    {
        //if (string.IsNullOrEmpty(filePath))
        //{
        //    Debug.LogError("Please set the VAE data file path!");
        //    return;
        //}
        //LoadVAEData(filePath);
    }

    // 提供公開方法以便在運行時更改和重新載入檔案
    public void LoadNewFile(string newFilePath)
    {
        if (string.IsNullOrEmpty(newFilePath))
        {
            Debug.LogError("File path cannot be empty!");
            return;
        }
        //filePath = newFilePath;
        //LoadVAEData(filePath);
    }

    public void LoadVAEData(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                Debug.LogError($"File not found at path: {path}");
                return;
            }
            string[] lines = File.ReadAllLines(path);

            // 解析基本資訊
            foreach (string line in lines)
            {
                if (line.Contains("Shape:"))
                {
                    string shapePart = line.Split(':')[1].Trim();
                    shapePart = shapePart.Trim('(', ')');
                    string[] dimensions = shapePart.Split(',');
                    for (int i = 0; i < dimensions.Length; i++)
                    {
                        shape[i] = int.Parse(dimensions[i].Trim());
                    }
                }
                else if (line.Contains("Mean:"))
                {
                    mean = float.Parse(line.Split(':')[1].Trim());
                }
                else if (line.Contains("Std:"))
                {
                    std = float.Parse(line.Split(':')[1].Trim());
                }
                else if (line.Contains("Min:"))
                {
                    min = float.Parse(line.Split(':')[1].Trim());
                }
                else if (line.Contains("Max:"))
                {
                    max = float.Parse(line.Split(':')[1].Trim());
                }
            }

            // 初始化陣列
            latentArray = new float[shape[0], shape[1], shape[2], shape[3]];

            // 找到Array Data:後的資料
            bool foundData = false;
            List<float> values = new List<float>();

            foreach (string line in lines)
            {
                if (line.Contains("Array Data:"))
                {
                    foundData = true;
                    continue;
                }

                if (foundData && !string.IsNullOrWhiteSpace(line))
                {
                    string[] numbers = line.Split(',');
                    foreach (string num in numbers)
                    {
                        if (!string.IsNullOrWhiteSpace(num))
                        {
                            values.Add(float.Parse(num));
                        }
                    }
                }
            }

            // 填充陣列
            int index = 0;
            for (int n = 0; n < shape[0]; n++)
            {
                for (int c = 0; c < shape[1]; c++)
                {
                    for (int h = 0; h < shape[2]; h++)
                    {
                        for (int w = 0; w < shape[3]; w++)
                        {
                            if (index < values.Count)
                            {
                                latentArray[n, c, h, w] = values[index++];
                            }
                        }
                    }
                }
            }

            Debug.Log("VAE data loaded successfully!");
            vAEVisualizer.CreateVisualization();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading VAE data: {e.Message}");
        }
    }
}