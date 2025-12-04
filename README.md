# ComfyUI Visualizer

<div align="center">

![放專案 Banner 圖]

**AI 生圖過程視覺化工具 - 讓你看見 Stable Diffusion 的內部運作**

[![Unity](https://img.shields.io/badge/Unity-2021.3+-black?style=flat&logo=unity)](https://unity.com/)
[![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![ComfyUI](https://img.shields.io/badge/ComfyUI-Compatible-green)](https://github.com/comfyanonymous/ComfyUI)

[放展示 GIF]

</div>

---

## 📖 關於專案

這是一個基於 Unity 開發的互動式視覺化應用，讓使用者能夠即時觀察 Stable Diffusion 在 ComfyUI 中的運作過程。不再只是等待圖片生成，而是能看見 AI 如何理解提示詞、如何在潛空間中逐步構建圖像。

### 為什麼做這個專案？

- 🔍 **理解 AI 黑盒**：將抽象的擴散模型過程具象化
- 🎓 **教育工具**：幫助研究者和學習者理解生成式 AI
- 🤝 **協作平台**：多人即時觀察同一個生成過程

---

## ✨ 核心功能

### 1️⃣ Conditioning Matrix 視覺化

追蹤每個 token 在提示詞中的重要性權重

![放 Conditioning 視覺化截圖]

- ✅ 顏色梯度表示影響力強弱
- ✅ 智能分組相同權重的 tokens
- ✅ 動態箭頭標註系統
```csharp
// 核心實作：按權重分組 tokens
groupedTokens = tokenData
    .GroupBy(t => t.Importance)
    .ToDictionary(g => g.Key, g => g.ToList());
```

---

### 2️⃣ Latent Space 特徵視覺化

4 通道潛空間即時呈現降噪過程

![放 Latent Space 四通道視覺化截圖]

| 功能 | 說明 |
|------|------|
| 📊 多通道顯示 | 同時查看 4 個潛空間通道 |
| ⏯️ 步驟控制 | 逐步觀看降噪過程 |
| 🌡️ 熱力圖 | 視覺化特徵分布 |

![放步驟切換的 GIF]

---

### 3️⃣ VAE Decode 追蹤

記錄 VAE 解碼前後的數據變化

![放 VAE 解碼對比圖]

---

### 4️⃣ 多人協作模式

基於 Photon PUN 2 實現即時同步

![放多人協作介面截圖]

- 🔐 房間碼分享系統
- 🔄 參數即時同步
- 👥 團隊協作觀察

---

## 🎮 操作介面

### 參數控制面板

![放參數設定介面截圖]

| 參數 | 範圍 | 說明 |
|------|------|------|
| Model | Normal / Realistic | 選擇模型風格 |
| Width/Height | 128-512 px | 圖像尺寸 |
| Seed | 任意整數 | 隨機種子 |
| Steps | 5-30 | 降噪步驟 |
| CFG Scale | 1-30 | 提示詞遵循度 |

---

## 🛠️ 技術架構
```
┌─────────────────────────────────────────────┐
│            Unity Frontend (C#)              │
├─────────────────────────────────────────────┤
│  ┌───────────────┐  ┌───────────────────┐  │
│  │ Visualizers   │  │  Network Layer    │  │
│  │ - Conditioning│  │  - Photon PUN 2   │  │
│  │ - Latent      │  │  - Room System    │  │
│  │ - VAE         │  │                   │  │
│  └───────────────┘  └───────────────────┘  │
├─────────────────────────────────────────────┤
│            ComfyUI REST API                 │
│         (http://127.0.0.1:8188)             │
├─────────────────────────────────────────────┤
│          Google Drive Storage               │
└─────────────────────────────────────────────┘
```

### 技術棧

| 類別 | 技術 |
|------|------|
| 引擎 | Unity 2021.3+ |
| 語言 | C# |
| 網路 | Photon PUN 2 |
| 資料處理 | Newtonsoft.Json |
| API | ComfyUI REST API |
| 雲端 | Google Drive API |

---

## 📁 專案結構
```
Assets/
├── 📊 DataVisualizer/
│   ├── ConditioningVisualizer.cs    # Conditioning 矩陣視覺化
│   ├── FeatureVisualizer.cs         # Latent 特徵視覺化
│   ├── VAEVisualizer.cs             # VAE 解碼追蹤
│   └── icon.cs                      # UI 狀態管理
│
├── 📂 FileAction/
│   ├── LatentDataLoader.cs          # 潛空間資料載入
│   ├── ConditioningMatrixLoader.cs  # Conditioning 資料載入
│   ├── VAELoader.cs                 # VAE 資料載入
│   ├── VisualizeDataListener.cs     # 檔案監聽系統
│   └── StartVisualize.cs            # 視覺化流程控制
│
├── 🖼️ TextToImage/
│   └── texttoimage.cs               # ComfyUI API 整合
│
├── ☁️ GoogleDrive/
│   └── GoogleDriveManager.cs        # 雲端同步管理
│
└── 🎬 TurorialScene/
    └── SceneSwitcher.cs             # 場景切換
```

---

## 🚀 快速開始

### 環境需求

- Unity 2021.3 或更新版本
- ComfyUI 本地運行（預設 port 8188）
- 需安裝 ComfyUI 自定義節點：
  - `DebugConditioning`
  - `SaveLatentToFile`
  - `VAEDebugSave`

### 安裝步驟
```bash
# 1. Clone 專案
git clone https://github.com/yourusername/comfyui-visualizer.git

# 2. 用 Unity 開啟專案
# File > Open Project > 選擇專案資料夾

# 3. 確認 ComfyUI 已啟動
# 預設連接: http://127.0.0.1:8188/prompt

# 4. 執行場景
# 開啟 Assets/Scenes/SampleScene.unity
# 按下 Play 按鈕
```

### 使用流程

![放使用流程圖]

1. **設定模型** → 選擇 Normal 或 Realistic 風格
2. **調整參數** → 設定圖像尺寸、steps、CFG 等
3. **輸入提示詞** → 描述你想生成的圖像
4. **開始生成** → 即時觀察視覺化過程
5. **逐步檢視** → 切換 steps 查看降噪過程

---

## 💡 技術亮點

### 🎯 智能資料分組

使用 LINQ 將相同權重的 tokens 自動分組，減少 UI 元素數量：
```csharp
// ConditioningVisualizer.cs
groupedTokens = tokenData
    .GroupBy(t => t.Importance)
    .ToDictionary(g => g.Key, g => g.ToList());

foreach (var group in groupedTokens.OrderBy(g => g.Key))
{
    CreateArrowForTokenGroup(group.Key, group.Value, normalizedImportance);
}
```

### 🎨 動態色彩映射系統

透過 Gradient 將數值無縫轉換為視覺化顏色：
```csharp
// FeatureVisualizer.cs
float normalizedValue = Mathf.InverseLerp(minVal, maxVal, val);

Color color;
if (normalizedValue < 0.5f)
    color = Color.Lerp(lowValueColor, midValueColor, normalizedValue * 2);
else
    color = Color.Lerp(midValueColor, highValueColor, (normalizedValue - 0.5f) * 2);
```

### ⚡ 非同步資料處理

使用 Coroutine 避免 UI 卡頓：
```csharp
// texttoimage.cs
IEnumerator SendPromptToServer()
{
    string jsonPayload = CreateTexttoImageJson();
    UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
    // ... 設定 request
    yield return request.SendWebRequest();
    // ... 處理回應
}
```

### 🏗️ 模組化架構

每個視覺化元件獨立運作，透過事件系統協調：
```csharp
// StartVisualize.cs
private void VisualizeUpdate()
{
    driveManager.ProcessPNGFile(png);
    latentDataloader.LoadLatentData(latent);
    conditioningMatrixloader.LoadConditioningData(conditioning);
    vAELoader.LoadVAEData(vae);
}
```

---

## 📊 ComfyUI Workflow

專案使用的 ComfyUI 節點結構：
```json
{
  "3": "KSampler",           // 降噪取樣器
  "4": "CheckpointLoader",   // 模型載入
  "5": "EmptyLatentImage",   // 潛空間初始化
  "6": "CLIPTextEncode",     // 正面提示詞編碼
  "8": "VAEDecode",          // VAE 解碼
  "9": "SaveImage",          // 儲存圖像
  "10": "DebugConditioning", // Conditioning 調試 *
  "11": "SaveLatentToFile",  // Latent 儲存 *
  "12": "VAEDebugSave",      // VAE 調試儲存 *
  "13": "CLIPTextEncode"     // 負面提示詞編碼
}
```

*標記為自定義節點

---

## 🎥 展示影片

![放 Demo 影片縮圖]

[▶️ 觀看完整展示影片](https://www.youtube.com/your-video-link)

---

## 🗺️ 未來規劃

- [ ] 支援 ControlNet 視覺化
- [ ] 匯出視覺化過程為影片
- [ ] 即時調整參數並觀察變化
- [ ] 支援更多 ComfyUI 自定義節點
- [ ] 加入 LoRA 權重視覺化
- [ ] 歷史記錄比對功能

---

## 🤝 貢獻

歡迎提交 Issue 和 Pull Request！

1. Fork 這個專案
2. 創建你的特性分支 (`git checkout -b feature/AmazingFeature`)
3. 提交你的修改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 開啟一個 Pull Request

---

## 📝 開發心得

這個專案讓我深入理解了擴散模型的運作原理，特別是：

- 🧠 如何將抽象的數學概念轉化為直觀的視覺呈現
- 🔧 Unity 與外部 API 的整合最佳實踐
- ⚡ 大量資料的即時視覺化效能優化
- 🌐 多人協作系統的架構設計

透過這個專案，我學到了如何設計良好的資料流架構，以及如何在保持程式碼可維護性的同時實現複雜的功能。

---

## 📧 聯絡方式

- 📫 Email: your.email@example.com
- 💼 LinkedIn: [Your LinkedIn Profile](https://linkedin.com/in/yourprofile)
- 🌐 Portfolio: [Your Portfolio Website](https://yourwebsite.com)

---

## 📄 授權

本專案採用 MIT 授權條款 - 詳見 [LICENSE](LICENSE) 文件

---

<div align="center">

**⭐ 如果這個專案對你有幫助，請給個星星！**

Made with ❤️ by [Your Name]

</div>
