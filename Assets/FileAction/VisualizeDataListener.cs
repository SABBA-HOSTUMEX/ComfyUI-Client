using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class VisualizeDataListener : MonoBehaviour
{
    private string basePath;
    private FileSystemWatcher[] watchers;
    private Dictionary<string, string> paths;

    // 儲存各檔案路徑
    [HideInInspector]
    public string picturePath;
    [HideInInspector]
    public string latentPath;
    [HideInInspector]
    public string conditioningPath;
    [HideInInspector]
    public string vaePath;

    // 檢查檔案是否準備好
    private bool isPictureReady = false;
    private bool isLatentReady = false;
    private bool isConditioningReady = false;
    private bool isVAEReady = false;
    [HideInInspector]
    public bool ALL_FILE_READY = false;

    private void Start()
    {
        basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "AppData");
        InitializePaths();
        SetupWatchers();
    }

    private void InitializePaths()
    {
        paths = new Dictionary<string, string>
        {
            { "Picture", Path.Combine(basePath, "Picture") },
            { "LatentData", Path.Combine(basePath, "LatentData") },
            { "ConditioningData", Path.Combine(basePath, "ConditioningData") },
            { "VAEData", Path.Combine(basePath, "VAEData") }
        };

        foreach (var path in paths.Values)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Debug.Log($"Created directory: {path}");
            }
        }
    }

    private void SetupWatchers()
    {
        watchers = new FileSystemWatcher[paths.Count];
        int index = 0;

        foreach (var pathEntry in paths)
        {
            var watcher = new FileSystemWatcher();
            watcher.Path = pathEntry.Value;
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
            watcher.Created += OnFileCreated;
            watcher.EnableRaisingEvents = true;
            watchers[index] = watcher;
            index++;
            Debug.Log($"Started watching directory: {pathEntry.Key} at {pathEntry.Value}");
        }
    }

    private void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        Debug.Log($"發現新檔案 {e.FullPath}");
        HandleFileCreated(e);
    }

    private async void HandleFileCreated(FileSystemEventArgs e)
    {
        try
        {
            string fileName = Path.GetFileName(e.Name);
            string filePath = e.FullPath;
            string directory = Path.GetDirectoryName(filePath);

            if (await IsFileReady(filePath))
            {
                if (directory == paths["Picture"] && fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                {
                    picturePath = filePath;
                    isPictureReady = true;
                    Debug.Log("Picture 完成");
                }
                else if (directory == paths["LatentData"] && fileName.StartsWith("latent", StringComparison.OrdinalIgnoreCase))
                {
                    latentPath = filePath;
                    isLatentReady = true;
                    Debug.Log("Latent 完成");
                }
                else if (directory == paths["ConditioningData"] && fileName.StartsWith("conditioning", StringComparison.OrdinalIgnoreCase))
                {
                    conditioningPath = filePath;
                    isConditioningReady = true;
                    Debug.Log("Conditioning 完成");
                }
                else if (directory == paths["VAEData"] && fileName.StartsWith("vae", StringComparison.OrdinalIgnoreCase))  // 修改：移除檔案名稱限制
                {
                    vaePath = filePath;
                    isVAEReady = true;
                    Debug.Log($"VAE 完成");
                }

                // 檢查是否所有檔案都準備好
                if (isPictureReady && isLatentReady && isConditioningReady && isVAEReady)
                {
                    ProcessAllFiles();
                    Debug.Log($"所有檔案準備完成");
                    ALL_FILE_READY = true;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"檔案處理發生錯誤: {ex.Message}");
        }
    }

    private void ProcessAllFiles()
    {
        // 在這裡處理所有檔案
        ProcessPictureFile(picturePath);
        ProcessLatentFile(latentPath);
        ProcessConditioningFile(conditioningPath);

        // 重置狀態
        isPictureReady = false;
        isLatentReady = false;
        isConditioningReady = false;
    }

    private void ProcessPictureFile(string path)
    {
        // TODO: 實作處理圖片檔案的邏輯
    }

    private void ProcessLatentFile(string path)
    {
        // TODO: 實作處理 latent 檔案的邏輯
    }

    private void ProcessConditioningFile(string path)
    {
        // TODO: 實作處理 conditioning 檔案的邏輯
    }

    private async Task<bool> IsFileReady(string filepath)
    {
        try
        {
            if (!File.Exists(filepath))
            {
                return false;
            }

            using (FileStream stream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                return true;
            }
        }
        catch (IOException)
        {
            return false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"檢查檔案狀態時發生錯誤: {ex.Message}");
            return false;
        }
    }

    private void OnDestroy()
    {
        if (watchers != null)
        {
            foreach (var watcher in watchers)
            {
                if (watcher != null)
                {
                    watcher.EnableRaisingEvents = false;
                    watcher.Dispose();
                }
            }
        }
    }
}