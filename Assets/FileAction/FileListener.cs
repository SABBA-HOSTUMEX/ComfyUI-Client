using UnityEngine;
using TMPro;
using System;
using System.IO;
using System.Collections.Generic;

public class FileListener : MonoBehaviour
{
    [Header("Watching Paths")]
    [SerializeField] private string ConditioningFolderPath = @"C:\Users\admin\Desktop\ComfyUI_windows_portable\ComfyUI\output\ConditioningData";
    [SerializeField] private string LatentFolderPath = @"C:\Users\admin\Desktop\ComfyUI_windows_portable\ComfyUI\output\LatentData";
    [SerializeField] private string PNGFolderPath = @"C:\Users\admin\Desktop\ComfyUI_windows_portable\ComfyUI\output";
    public GoogleDriveManager googleDriveManager;
    // 定義委派和事件
    public delegate void FileCreatedHandler(string filePath, string folderName);
    public static event FileCreatedHandler OnFileCreated;

    private List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();

    void Start()
    {
        SetupFileWatchers();
    }

    private void SetupFileWatchers()
    {
        try
        {
            // 設定三個資料夾的監聽
            SetupWatcher(ConditioningFolderPath, "ConditioningData");
            SetupWatcher(LatentFolderPath, "LatentData");
            SetupWatcher(PNGFolderPath, "PNG");
            Debug.Log("All file watchers setup complete");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error setting up file watchers: {e.Message}");
        }
    }

    private void SetupWatcher(string path, string watcherName)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        FileSystemWatcher watcher = new FileSystemWatcher(path);

        watcher.NotifyFilter = NotifyFilters.Attributes
                             | NotifyFilters.CreationTime
                             | NotifyFilters.DirectoryName
                             | NotifyFilters.FileName
                             | NotifyFilters.LastAccess
                             | NotifyFilters.LastWrite
                             | NotifyFilters.Security
                             | NotifyFilters.Size;

        string currentWatcherName = watcherName;

        watcher.Created += (sender, e) => OnNewFileDetected(e, currentWatcherName);
        watcher.EnableRaisingEvents = true;

        watchers.Add(watcher);
        Debug.Log($"File watcher {watcherName} setup complete at: {path}");
    }

    private async void OnNewFileDetected(FileSystemEventArgs e, string watcherName)
    {
        // 更新 UI
        Debug.Log($"New file detected in {watcherName} at: {e.FullPath}");

        // 等待一小段時間確保檔案完全寫入
        await System.Threading.Tasks.Task.Delay(1000);

        // 根據資料夾類型選擇對應的上傳方法
        switch (watcherName)
        {
            case "PNG":
                await googleDriveManager.UploadPNGFile(e.FullPath);
                break;
            case "LatentData":
                await googleDriveManager.UploadLatentFile(e.FullPath);
                break;
            case "ConditioningData":
                await googleDriveManager.UploadConditioningFile(e.FullPath);
                break;
            default:
                Debug.LogError($"Unknown folder type: {watcherName}");
                break;
        }
    }

    void OnDestroy()
    {
        foreach (var watcher in watchers)
        {
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
        }
        watchers.Clear();
    }
}