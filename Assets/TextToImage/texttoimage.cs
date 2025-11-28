using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;
using TMPro;
using Photon.Pun;
using System.Collections.Generic;
using Photon.Realtime; 
public class texttoimage : MonoBehaviourPunCallbacks
{
    [Header("Conditioning Object")]
    [SerializeField] private Button Pre;
    [SerializeField] private Button Next;
    [SerializeField] private Text ModelName;
    [SerializeField] private string Model1 = "Normal";
    [SerializeField] private string Model2 = "Realistic";
    private bool isFirstString = false;
    [Header("InputField")]
    public TMP_InputField seedInputField;
    public TMP_InputField stepsInputField;
    public TMP_InputField cfgInputField;
    public TMP_InputField widthInputField;
    public TMP_InputField heightInputField;
    public TMP_InputField promptInputField;
    [Header("Button")]
    public Button generateButton;
    public Button EmptyLatentImageBTN;
    public Button KsamplerBTN;
    public Button ConditioningBTN;
    public Button ErrorClose;
    public Button ServerBusyClose;
    public Button NoconnectClose;
    [Header("Data Proccess")]
    [SerializeField]
    private GoogleDriveManager googleDrive;
    [SerializeField]
    private FeatureVisualizer featureVisualizer;
    [SerializeField]
    private ConditioningVisualizer conditioningVisualizer;
    [SerializeField]
    private MenuUI menuUI;
    [SerializeField]
    private PhotonPrompt photonPrompt;
    [SerializeField]
    private ResultHistory resultHistory;
    public string seed { get; set; }
    public string steps { get; set; }
    public string cfg { get; set; }
    public string width { get; set; }
    public string height { get; set; }
    public string prompt { get; set; }
    public string model { get; set; }
    // public TMP_Text process;
    public Text CheckpointValue;
    public Text LatentImageValue;
    public Text KsamplerValue;
    public PhotonPrompt networkManager;
    private string apiUrl = "http://127.0.0.1:8188/prompt";//your ComfyUI web
    [SerializeField] private GameObject error;
    [SerializeField] private GameObject ServerBusy;
    [SerializeField] private GameObject NoConnect;
    private string previousPrompt = "";
    [Header("SaveData Background")]
    [SerializeField]
    private GameObject ModelBG;
    [SerializeField]
    private GameObject LatentBG;
    [SerializeField]
    private GameObject KsamplerBG;
    [SerializeField]
    private GameObject ParameterUI;
    [SerializeField]
    private GameObject SaveUI;
    [SerializeField]
    private GameObject ChangeUI;
    [SerializeField]
    private GameObject CompareUI;
    // Start is called before the first frame update
    void Start()
    {
        ModelBG.SetActive(false);
        LatentBG.SetActive(false);
        KsamplerBG.SetActive(false);
        error.SetActive(false);
        generateButton.onClick.AddListener(SendValue);  // �]�m���s�I���ƥ�
        EmptyLatentImageBTN.onClick.AddListener(OnSaveEmptyLatentImageData);
        KsamplerBTN.onClick.AddListener(OnSaveKsamplerData);
        Pre.onClick.AddListener(changemodel);
        Next.onClick.AddListener(changemodel);
        ConditioningBTN.onClick.AddListener(savemodelname);
        ErrorClose.onClick.AddListener(CloseErrorUI);
        ServerBusyClose.onClick.AddListener(CloseServerBusy);
        NoconnectClose.onClick.AddListener(CloseNoconnect);
    }
    public void SendValue()
    {
        Debug.Log(photonPrompt.status);
        if (photonPrompt.isConnect == true)
        {
            if (photonPrompt.status == "finish")
            {
                prompt = promptInputField.text;



                string send_model = model;
                string send_width = width;
                string send_height = height;
                string send_seed = seed;
                string send_steps = steps;
                string send_cfg = cfg;
                string send_prompt = prompt;
                if (!string.IsNullOrEmpty(send_model) &&
                    !string.IsNullOrEmpty(send_width) &&
                    !string.IsNullOrEmpty(send_height) &&
                    !string.IsNullOrEmpty(send_seed) &&
                    !string.IsNullOrEmpty(send_steps) &&
                    !string.IsNullOrEmpty(send_cfg) &&
                    !string.IsNullOrEmpty(send_prompt))
                {
                    // Store current prompt for future comparison
                    previousPrompt = prompt;

                    networkManager.SendPrompt(send_model, send_width, send_height, send_seed, send_steps, send_cfg, send_prompt);
                    menuUI.ShowLoadingUI();
                    resultHistory.HistoryUpdate(send_seed, send_steps, send_cfg, send_prompt);
                }
                else
                {
                    error.SetActive(true);
                    ParameterUI.SetActive(false);
                    SaveUI.SetActive(false);
                    ChangeUI.SetActive(false);
                    CompareUI.SetActive(false);
                }
            }
            else
            {
                ServerBusy.SetActive(true);
            }
        }
        else
        { 
            NoConnect.SetActive(true);
            ParameterUI.SetActive(false);
            SaveUI.SetActive(false);
            ChangeUI.SetActive(false);
            CompareUI.SetActive(false);
        }           
    }
    // Update is called once per frame
    void Update()
    {

    }
    private void CloseErrorUI()
    {
        error.SetActive(false);
        ParameterUI.SetActive(true);
        SaveUI.SetActive(true);
        ChangeUI.SetActive(true);
        CompareUI.SetActive(true);
    }
    private void CloseServerBusy()
    {
        ServerBusy.SetActive(false);
    }
    private void CloseNoconnect()
    {
        NoConnect.SetActive(false);
        ParameterUI.SetActive(true);
        SaveUI.SetActive(true);
        ChangeUI.SetActive(true);
        CompareUI.SetActive(true);
    }
    public void changemodel()
    {
        isFirstString = !isFirstString;        
        ModelName.text = isFirstString ? Model1 : Model2;
    }
    public void savemodelname()
    {
        model = ModelName.text;
        CheckpointValue.text = model;
        Debug.Log($"Model Type : {model}");
        ModelBG.SetActive(true);
    }
    // 接回接收到的 api 參數並且設置到 api json 裡面
    public void SetAllParametertxt2img(string User_width, string User_height, string User_seed, string User_steps, string User_cfg, string User_prompt)
    {
        width = User_width;
        height = User_height;
        seed = User_seed;
        steps = User_steps;
        cfg = User_cfg;
        prompt = User_prompt;
        OnGenerateButtonClicked();
    }
    // 把輸入的參數傳給裝置 A (master client)
    public void OnSaveEmptyLatentImageData()
    {
        widthInputField.text = Math.Min(Math.Max(int.Parse(widthInputField.text), 128), 512).ToString();
        heightInputField.text = Math.Min(Math.Max(int.Parse(heightInputField.text), 128), 512).ToString();
        width = widthInputField.text;
        height = heightInputField.text;
        Debug.Log($"width : {width}  height : {height}");
        LatentImageValue.text = $"width : {width}\nheight : {height}";
        LatentBG.SetActive(true);
    }
    public void OnSaveKsamplerData()
    {
        stepsInputField.text = Math.Max(Math.Min(int.Parse(stepsInputField.text),30), 5).ToString();
        cfgInputField.text = Math.Max(Math.Min(int.Parse(cfgInputField.text),30), 1).ToString();
        seed = seedInputField.text;
        steps = stepsInputField.text;
        cfg = cfgInputField.text;
        Debug.Log($"seed : {seed}  steps : {steps}  cfg : {cfg}");
        KsamplerValue.text = $"seed : {seed}\nsteps : {steps}\ncfg : {cfg}";
        KsamplerBG.SetActive(true);
    }
    void OnGenerateButtonClicked()
    {
        if (!string.IsNullOrEmpty(prompt))
        {
            Debug.Log($"Current value => {googleDrive.donepic + featureVisualizer.donelatent + conditioningVisualizer.donecon}");
            StartCoroutine(SendPromptToServer());
        }
        else
        {
            // process.text = "Prompt cannot be empty!";
            // Debug.Log(process.text);
        }
    }

    IEnumerator SendPromptToServer()
    {
        // �Ы� JSON �ó]�m prompt
        string jsonPayload = CreateTexttoImageJson();

        // �o�e API �ШD
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        Debug.Log(jsonPayload);

        // ���ݦ^��
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // process.text = "Prompt sent successfully! Checking model generation...";
        }
        else
        {
            // process.text = "Error sending prompt: " + request.error;
            // Debug.Log(process.text);
        }
    }
    string CreateTexttoImageJson()
    {
        // 創建主要的工作流程物件
        var workflow = new JObject
        {
            // KSampler 節點
            ["3"] = new JObject
            {
                ["inputs"] = new JObject
                {
                    ["seed"] = seed,
                    ["steps"] = steps,
                    ["cfg"] = cfg,
                    ["sampler_name"] = "euler",
                    ["scheduler"] = "normal",
                    ["denoise"] = 1,
                    ["model"] = new JArray { "4", 0 },
                    ["positive"] = new JArray { "10", 0 },
                    ["negative"] = new JArray { "13", 0 },  // 新增 negative prompt 連接
                    ["latent_image"] = new JArray { "5", 0 }
                },
                ["class_type"] = "KSampler",
                ["_meta"] = new JObject { ["title"] = "KSampler" }
            },

            // 模型載入節點
            ["4"] = new JObject
            {
                ["inputs"] = new JObject
                {
                    ["ckpt_name"] = "sdxlNijiSeven_sdxlNijiSeven.safetensors"
                },
                ["class_type"] = "CheckpointLoaderSimple",
                ["_meta"] = new JObject { ["title"] = "Load Checkpoint" }
            },

            // 空白潛空間圖像節點
            ["5"] = new JObject
            {
                ["inputs"] = new JObject
                {
                    ["width"] = width,
                    ["height"] = height,
                    ["batch_size"] = 1
                },
                ["class_type"] = "EmptyLatentImage",
                ["_meta"] = new JObject { ["title"] = "Empty Latent Image" }
            },

            // 正面提示詞編碼節點
            ["6"] = new JObject
            {
                ["inputs"] = new JObject
                {
                    ["text"] = prompt,
                    ["clip"] = new JArray { "4", 1 }
                },
                ["class_type"] = "CLIPTextEncode",
                ["_meta"] = new JObject { ["title"] = "CLIP Text Encode (Prompt)" }
            },

            // VAE 解碼節點
            ["8"] = new JObject
            {
                ["inputs"] = new JObject
                {
                    ["samples"] = new JArray { "11", 0 },
                    ["vae"] = new JArray { "4", 2 }
                },
                ["class_type"] = "VAEDecode",
                ["_meta"] = new JObject { ["title"] = "VAE Decode" }
            },

            // 儲存圖像節點
            ["9"] = new JObject
            {
                ["inputs"] = new JObject
                {
                    ["filename_prefix"] = "ComfyUI",
                    ["images"] = new JArray { "12", 0 }
                },
                ["class_type"] = "SaveImage",
                ["_meta"] = new JObject { ["title"] = "Save Image" }
            },

            // 正面提示詞調試節點
            ["10"] = new JObject
            {
                ["inputs"] = new JObject
                {
                    ["show_full_matrix"] = true,
                    ["num_values"] = 10,
                    ["save_to_file"] = true,
                    ["conditioning"] = new JArray { "6", 0 }
                },
                ["class_type"] = "DebugConditioning",
                ["_meta"] = new JObject { ["title"] = "Debug Conditioning Matrix" }
            },

            // 潛空間保存節點
            ["11"] = new JObject
            {
                ["inputs"] = new JObject
                {
                    ["add_timestamp"] = true,
                    ["latent"] = new JArray { "3", 0 }
                },
                ["class_type"] = "SaveLatentToFile",
                ["_meta"] = new JObject { ["title"] = "Save Latent to TXT" }
            },

            // VAE調試保存節點
            ["12"] = new JObject
            {
                ["inputs"] = new JObject
                {
                    ["add_timestamp"] = true,
                    ["samples"] = new JArray { "8", 0 },
                    ["original_latent"] = new JArray { "3", 0 }
                },
                ["class_type"] = "VAEDebugSave",
                ["_meta"] = new JObject { ["title"] = "Save VAE Debug Info" }
            },

            // 負面提示詞編碼節點
            ["13"] = new JObject
            {
                ["inputs"] = new JObject
                {
                    ["text"] = "Bad Things",  // 新增負面提示詞參數
                    ["clip"] = new JArray { "4", 1 }
                },
                ["class_type"] = "CLIPTextEncode",
                ["_meta"] = new JObject { ["title"] = "CLIP Text Encode (Prompt)" }
            }
        };

        var finalPayload = new JObject { ["prompt"] = workflow };
        return finalPayload.ToString(Formatting.None);
    }
}
