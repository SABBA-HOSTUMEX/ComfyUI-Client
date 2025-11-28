using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Threading;
using Google.Apis.Util.Store;

public class GoogleDriveManager : MonoBehaviour
{
    private DriveService driveService;
    private const string APPLICATION_NAME = "Unity Google Drive Integration";
    private const string PNG_TARGET_FOLDER_ID = "1y1JtuKcYC8RoQtcVrWK3JyMughLBs9rz";  // PNG 資料夾
    private const string Latent_TARGET_FOLDER_ID = "1AeYmyPN7N4RkhFOTBbKf8jGQNHRWooj5";  // Latent 資料夾
    private const string Conditioning_TARGET_FOLDER_ID = "1f21vvumwUjwAe55a7LOVeOPkeSzeCQNv";  // Conditioning 資料夾

    // 從您的Google Cloud Console憑證中獲取
    private readonly string[] Scopes = { DriveService.Scope.DriveFile };
    private const string ClientId = "";
    private const string ClientSecret = "";
    [SerializeField] private PhotonPrompt photonPrompt;
    public RawImage rawImage;
    public RawImage rawImage2;
    private string lastPngId;
    private string lastLatentId;
    private string lastConditioningId;
    [HideInInspector]
    public int donepic;
    public bool IsProcessingComplete { get; private set; }
    [SerializeField] icon Icon;
    [SerializeField] private ResultHistory resultHistory;
    [SerializeField] private VAEVisualizer vAEVisualizer;
    [SerializeField] private FeatureVisualizer featureVisualizer;
    private async void Start()
    {
        await InitializeDriveService();
    }

    private async Task InitializeDriveService()
    {
        try
        {
            string credPath;
            // 判斷是否為 Android 平台
            if (Application.platform == RuntimePlatform.Android)
            {
                credPath = Path.Combine(Application.persistentDataPath, "GoogleDriveCredentials");

                // 確保目錄存在
                if (!Directory.Exists(credPath))
                {
                    Directory.CreateDirectory(credPath);
                }
                Debug.Log($"Android 平台 - 使用路徑: {credPath}");
            }
            else
            {
                credPath = "Drive.Api.Auth.Store";
                Debug.Log($"非 Android 平台 - 使用路徑: {credPath}");
            }

            var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = ClientId,
                    ClientSecret = ClientSecret
                },
                Scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(credPath, true)  // 加入 true 參數以確保創建目錄
            );

            Debug.Log("Credential created successfully");

            driveService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = APPLICATION_NAME
            });

            Debug.Log("DriveService initialized successfully");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to initialize Drive service: {ex.Message}");
            Debug.LogError($"Stack trace: {ex.StackTrace}");
        }
    }

    public async Task<string> UploadFile(string filePath, string folderId)
    {
        donepic = 0;
        if (driveService == null)
        {
            Debug.LogError("Drive service not initialized");
            return null;
        }

        try
        {
            Debug.Log($"Starting upload of file: {filePath}");

            if (!File.Exists(filePath))
            {
                Debug.LogError($"File does not exist: {filePath}");
                return null;
            }

            string normalizedPath = filePath.Replace('\\', '/');
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = Path.GetFileName(normalizedPath),
                Parents = new[] { folderId }
            };

            using (var stream = new FileStream(normalizedPath, FileMode.Open))
            {
                var request = driveService.Files.Create(fileMetadata, stream, "application/octet-stream");
                request.Fields = "id, name";

                var uploadProgress = await request.UploadAsync();
                if (uploadProgress.Status == UploadStatus.Completed)
                {
                    var file = request.ResponseBody;
                    Debug.Log($"File uploaded successfully. File ID: {file.Id}");
                    return file.Id;
                }
                else
                {
                    Debug.LogError($"Upload failed with status: {uploadProgress.Status}");
                    return null;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Upload error: {ex.Message}");
            Debug.LogError($"Stack trace: {ex.StackTrace}");
            return null;
        }
    }

    public async Task UploadPNGFile(string filePath)
    {
        var fileId = await UploadFile(filePath, PNG_TARGET_FOLDER_ID);
        if (!string.IsNullOrEmpty(fileId))
        {
            OnUploadComplete(fileId, "png");
        }
    }

    public async Task UploadLatentFile(string filePath)
    {
        var fileId = await UploadFile(filePath, Latent_TARGET_FOLDER_ID);
        if (!string.IsNullOrEmpty(fileId))
        {
            OnUploadComplete(fileId, "latent");
        }
    }

    public async Task UploadConditioningFile(string filePath)
    {
        var fileId = await UploadFile(filePath, Conditioning_TARGET_FOLDER_ID);
        if (!string.IsNullOrEmpty(fileId))
        {
            OnUploadComplete(fileId, "conditioning");
        }
    }

    private void OnUploadComplete(string fileId, string fileType)
    {
        switch (fileType.ToLower())
        {
            case "png":
                lastPngId = fileId;
                Debug.Log($"PNG file uploaded with ID: {fileId}");
                break;
            case "latent":
                lastLatentId = fileId;
                Debug.Log($"Latent file uploaded with ID: {fileId}");
                break;
            case "conditioning":
                lastConditioningId = fileId;
                Debug.Log($"Conditioning file uploaded with ID: {fileId}");
                break;
        }

        // 如果所有需要的文件都已上傳，發送ID們
        if (!string.IsNullOrEmpty(lastPngId) &&
            !string.IsNullOrEmpty(lastLatentId) &&
            !string.IsNullOrEmpty(lastConditioningId))
        {
            Debug.Log("All files uploaded, sending IDs to clients...");
            photonPrompt?.SendFileIds(lastPngId, lastLatentId, lastConditioningId);

            // 清除已使用的ID
            lastPngId = null;
            lastLatentId = null;
            lastConditioningId = null;
        }
    }
    public async Task DownloadAndProcessFile(string fileId, string savePath)
    {
        if (driveService == null)
        {
            throw new System.InvalidOperationException("Drive service not initialized");
        }

        Debug.Log($"開始下載檔案 ID: {fileId} 到路徑: {savePath}");

        var request = driveService.Files.Get(fileId);
        var file = await request.ExecuteAsync();

        if (file == null)
        {
            throw new System.InvalidOperationException($"找不到檔案 ID: {fileId}");
        }

        string fullPath = Path.Combine(savePath, file.Name);

        using (var stream = new MemoryStream())
        {
            await request.DownloadAsync(stream);
            stream.Position = 0;

            using (var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
            {
                await stream.CopyToAsync(fileStream);
            }
        }

        // 驗證檔案是否成功下載
        if (File.Exists(fullPath))
        {
            var fileInfo = new FileInfo(fullPath);
            Debug.Log($"檔案成功下載：{fullPath}，大小：{fileInfo.Length} bytes");
        }
        else
        {
            throw new System.IO.IOException($"檔案下載後未找到：{fullPath}");
        }
    }

    public void ProcessPNGFile(string filePath)
    {
        try
        {
            if (rawImage != null)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    try
                    {
                        byte[] imageData = File.ReadAllBytes(filePath);
                        Texture2D texture = new Texture2D(2, 2);
                        texture.LoadImage(imageData);

                        // 設置 RawImage 的紋理
                        rawImage.texture = texture;
                        rawImage2.texture = texture;

                        // 直接設置 RawImage 的尺寸為圖片尺寸
                        RectTransform rt1 = rawImage.GetComponent<RectTransform>();
                        rt1.sizeDelta = new Vector2(texture.width, texture.height);

                        RectTransform rt2 = rawImage2.GetComponent<RectTransform>();
                        rt2.sizeDelta = new Vector2(texture.width, texture.height);

                        RectTransform rt3 = vAEVisualizer.rawImage.GetComponent<RectTransform>();
                        rt3.sizeDelta = new Vector2(texture.width, texture.height);
                        //featureVisualizer.visualizationWidth = texture.width;
                        //featureVisualizer.visualizationHeight = texture.height;
                        resultHistory.HistoryImageUpdate(texture);
                        Debug.Log($"PNG image loaded with dimensions: {texture.width}x{texture.height}");
                        donepic = 1;
                        Icon.changeui(3);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Error processing texture in main thread: {ex.Message}");
                    }
                });
            }
            else
            {
                Debug.LogError("RawImage reference is null");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error reading PNG file: {ex.Message}");
        }
    }
    public async Task ListFiles()
    {
        if (driveService == null)
        {
            Debug.LogError("Drive service not initialized");
            return;
        }
        try
        {
            // 列出 PNG 資料夾的檔案
            var pngRequest = driveService.Files.List();
            pngRequest.Q = $"'{PNG_TARGET_FOLDER_ID}' in parents";
            pngRequest.Fields = "files(id, name)";
            var pngResult = await pngRequest.ExecuteAsync();
            Debug.Log("Files in PNG folder:");
            foreach (var file in pngResult.Files)
            {
                Debug.Log($"PNG file: {file.Name} ({file.Id})");
            }

            // 列出 Latent 資料夾的檔案
            var latentRequest = driveService.Files.List();
            latentRequest.Q = $"'{Latent_TARGET_FOLDER_ID}' in parents";
            latentRequest.Fields = "files(id, name)";
            var latentResult = await latentRequest.ExecuteAsync();
            Debug.Log("Files in Latent folder:");
            foreach (var file in latentResult.Files)
            {
                Debug.Log($"Latent file: {file.Name} ({file.Id})");
            }

            // 列出 Conditioning 資料夾的檔案
            var conditioningRequest = driveService.Files.List();
            conditioningRequest.Q = $"'{Conditioning_TARGET_FOLDER_ID}' in parents";
            conditioningRequest.Fields = "files(id, name)";
            var conditioningResult = await conditioningRequest.ExecuteAsync();
            Debug.Log("Files in Conditioning folder:");
            foreach (var file in conditioningResult.Files)
            {
                Debug.Log($"Conditioning file: {file.Name} ({file.Id})");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"List files error: {ex.Message}");
        }
    }
}