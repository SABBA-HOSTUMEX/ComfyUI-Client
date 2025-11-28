using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Collections.Generic;
public class FileDownloader : MonoBehaviour
{
    [SerializeField] private GoogleDriveManager googleDriveManager;
    [SerializeField] private Text statusText;
    private bool isDownloading = false;
    private string basePath;

    private void Start()
    {
        if (googleDriveManager == null)
        {
            googleDriveManager = FindObjectOfType<GoogleDriveManager>();
        }

        // 使用 Pictures 資料夾作為基礎路徑
        basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),"AppData");

        // 或者使用 Unity 的 persistentDataPath
        // basePath = Path.Combine(Application.persistentDataPath, "AppData");
    }

    public async void StartDownload(string pngId, string latentId, string conditioningId, string vaeId)
    {
        if (isDownloading)
        {
            Debug.Log("Already downloading files...");
            return;
        }
        isDownloading = true;
        UpdateStatus("開始下載檔案...");

        try
        {
            // 確保目錄存在
            string[] folders = { "Picture", "LatentData", "ConditioningData", "VAEData" };
            Dictionary<string, string> paths = new Dictionary<string, string>();

            foreach (var folder in folders)
            {
                string path = Path.Combine(basePath, folder);
                paths[folder] = path;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    Debug.Log($"Created directory: {path}");
                }
            }

            // 下載 PNG 檔案
            if (!string.IsNullOrEmpty(pngId))
            {
                UpdateStatus("下載圖片中...");
                await googleDriveManager.DownloadAndProcessFile(pngId, paths["Picture"]);
                var files = Directory.GetFiles(paths["Picture"]);
                Debug.Log($"Picture directory contains {files.Length} files");
            }

            if (!string.IsNullOrEmpty(latentId))
            {
                UpdateStatus("下載 Latent 中...");
                await googleDriveManager.DownloadAndProcessFile(latentId, paths["LatentData"]);
                var files = Directory.GetFiles(paths["LatentData"]);
                Debug.Log($"LatentData directory contains {files.Length} files");
            }

            if (!string.IsNullOrEmpty(vaeId))
            {
                UpdateStatus("下載 VAE 中...");
                await googleDriveManager.DownloadAndProcessFile(vaeId, paths["VAEData"]);
                var files = Directory.GetFiles(paths["VAEData"]);
                Debug.Log($"VAE directory contains {files.Length} files");
            }

            if (!string.IsNullOrEmpty(conditioningId))
            {
                UpdateStatus("下載 Conditioning 中...");
                await googleDriveManager.DownloadAndProcessFile(conditioningId, paths["ConditioningData"]);
                var files = Directory.GetFiles(paths["ConditioningData"]);
                Debug.Log($"ConditioningData directory contains {files.Length} files");
            }

            UpdateStatus("所有檔案下載完成");
        }
        catch (System.Exception e)
        {
            string errorMessage = $"下載過程發生錯誤: {e.Message}\nStack Trace: {e.StackTrace}";
            Debug.LogError(errorMessage);
            UpdateStatus($"下載錯誤: {e.Message}");
        }
        finally
        {
            isDownloading = false;
        }
    }

    private void UpdateStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
        Debug.Log(message);
    }
}