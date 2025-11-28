using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Text;

public class RandomCodeGenerator : MonoBehaviour
{
    [Header("UI元素")]
    public TMP_Text randomCodeText;      // 用於顯示隨機碼的文字元素
    public Button copyButton;             // 複製隨機碼的按鈕
    public Button SurveyButton;

    [Header("隨機碼設定")]
    public int codeLength = 8;            // 隨機碼長度
    public bool includeNumbers = true;    // 是否包含數字
    public bool includeUppercase = true;  // 是否包含大寫字母
    public bool includeLowercase = true;  // 是否包含小寫字母

    private string currentCode;           // 目前的隨機碼
    [Header("腳本")]
    [SerializeField] private Data data;
    private void Start()
    {
        // 確保已連接所有必要的UI元素
        if (randomCodeText == null || copyButton == null)
        {
            Debug.LogError("需要連接UI元素!");
            return;
        }

        // 設定按鈕事件
        copyButton.onClick.AddListener(CopyCodeToClipboard);

        // 初始生成一個隨機碼
        GenerateNewCode();

        // 隱藏狀態文字

    }

    // 生成新的隨機碼
    public void GenerateNewCode()
    {
        StringBuilder codeBuilder = new StringBuilder();
        string chars = "";

        // 根據設定建立字元集
        if (includeNumbers) chars += "0123456789";
        if (includeUppercase) chars += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        if (includeLowercase) chars += "abcdefghijklmnopqrstuvwxyz";

        // 確保至少有一種字元類型被選中
        if (string.IsNullOrEmpty(chars))
        {
            chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        }

        // 生成隨機碼
        System.Random random = new System.Random();
        for (int i = 0; i < codeLength; i++)
        {
            int index = random.Next(0, chars.Length);
            codeBuilder.Append(chars[index]);
        }

        // 儲存並顯示隨機碼
        currentCode = codeBuilder.ToString();
        randomCodeText.text = currentCode;
        data.CodeUpdate(currentCode);
    }

    // 複製隨機碼到剪貼簿
    public void CopyCodeToClipboard()
    {
        if (!string.IsNullOrEmpty(currentCode))
        {
            // 複製到剪貼簿
            GUIUtility.systemCopyBuffer = currentCode;
            copyButton.GetComponentInChildren<Text>().text = "已複製";
            SurveyButton.gameObject.SetActive(true);
        }
    }
}