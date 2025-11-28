using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class ConditioningMatrixLoader : MonoBehaviour
{
    [SerializeField]
    private ConditioningVisualizer conditioningVisualizer;
    public bool IsLoadingComplete { get; private set; }
    private string path = @"C:\Users\admin\Desktop\ComfyUI_windows_portable\ComfyUI\output\ConditioningData\conditioning_debug_20250212_102558.txt";

    // 儲存每個 token 的資訊
    public class TokenInfo
    {
        public string Text { get; set; }
        public int ID { get; set; }
        public float Importance { get; set; }
    }

    public List<TokenInfo> Tokens { get; private set; }

    void Start()
    {
        Tokens = new List<TokenInfo>();
        //LoadConditioningData(path);
    }
    
    public bool LoadConditioningData(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError($"File not found: {filePath}");
                return false;
            }

            string[] lines = File.ReadAllLines(filePath);
            Tokens.Clear();

            bool isParsingTokens = false;
            TokenInfo currentToken = null;

            foreach (string line in lines)
            {
                if (line.Contains("=== Token Analysis ==="))
                {
                    isParsingTokens = true;
                    continue;
                }

                if (isParsingTokens)
                {
                    if (line.StartsWith("Token:"))
                    {
                        if (currentToken != null)
                        {
                            Tokens.Add(currentToken);
                        }
                        currentToken = new TokenInfo();
                        currentToken.Text = line.Replace("Token:", "").Trim();
                    }
                    else if (line.StartsWith("ID:") && currentToken != null)
                    {
                        currentToken.ID = int.Parse(line.Replace("ID:", "").Trim());
                    }
                    else if (line.StartsWith("Importance:") && currentToken != null)
                    {
                        currentToken.Importance = float.Parse(line.Replace("Importance:", "").Trim());
                    }
                }
            }

            // 加入最後一個 token
            if (currentToken != null)
            {
                Tokens.Add(currentToken);
            }

            Debug.Log($"Successfully loaded {Tokens.Count} tokens");
            conditioningVisualizer.UpdateAfterLoad();
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading conditioning data: {e.Message}");
            Debug.LogError(e.StackTrace);
            return false;
        }
    }

    public void GetTokenData(out List<TokenInfo> tokenData,
                           out float minImportance, out float maxImportance)
    {
        tokenData = new List<TokenInfo>(Tokens);
        minImportance = Tokens.Min(t => t.Importance);
        maxImportance = Tokens.Max(t => t.Importance);
    }
}