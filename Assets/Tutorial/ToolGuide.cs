using UnityEngine;
using UnityEngine.UI;

public class ToolGuide : MonoBehaviour
{
    // 預設顯示的物件
    [SerializeField] private GameObject defaultObject;

    // 六個可切換顯示的物件
    [SerializeField] private GameObject[] toolObjects = new GameObject[6];

    // 六個按鈕，用來顯示對應的物件
    [SerializeField] private Button[] toolButtons = new Button[6];

    // 六個關閉按鈕，每個物件都有一個
    [SerializeField] private Button[] closeButtons = new Button[6];

    // 目前顯示的物件索引，-1 表示顯示預設物件
    private int currentObjectIndex = -1;

    void Start()
    {
        // 檢查是否設置了預設物件
        if (defaultObject == null)
        {
            Debug.LogError("請設置預設物件！");
            return;
        }

        // 初始化：顯示預設物件，隱藏所有工具物件
        ShowDefaultObject();

        // 為每個工具按鈕添加點擊事件
        for (int i = 0; i < toolButtons.Length; i++)
        {
            if (toolButtons[i] == null)
            {
                Debug.LogWarning("工具按鈕 " + i + " 未設置！");
                continue;
            }

            int index = i; // 創建局部變數以在 Lambda 表達式中正確捕獲索引
            toolButtons[i].onClick.AddListener(() => ShowToolObject(index));
        }

        // 為每個關閉按鈕添加點擊事件
        for (int i = 0; i < closeButtons.Length; i++)
        {
            if (closeButtons[i] == null)
            {
                Debug.LogWarning("關閉按鈕 " + i + " 未設置！");
                continue;
            }

            closeButtons[i].onClick.AddListener(ShowDefaultObject);
        }
    }

    // 顯示工具物件的方法
    public void ShowToolObject(int index)
    {
        // 檢查索引是否有效
        if (index < 0 || index >= toolObjects.Length)
        {
            Debug.LogError("無效的工具物件索引：" + index);
            return;
        }

        // 檢查對應的工具物件是否已設置
        if (toolObjects[index] == null)
        {
            Debug.LogError("工具物件 " + index + " 未設置！");
            return;
        }

        // 隱藏預設物件和當前顯示的工具物件（如果有）
        defaultObject.SetActive(false);
        if (currentObjectIndex >= 0 && currentObjectIndex < toolObjects.Length && toolObjects[currentObjectIndex] != null)
        {
            toolObjects[currentObjectIndex].SetActive(false);
        }

        // 顯示選定的工具物件
        toolObjects[index].SetActive(true);
        currentObjectIndex = index;
    }

    // 顯示預設物件的方法
    public void ShowDefaultObject()
    {
        // 隱藏當前顯示的工具物件（如果有）
        if (currentObjectIndex >= 0 && currentObjectIndex < toolObjects.Length && toolObjects[currentObjectIndex] != null)
        {
            toolObjects[currentObjectIndex].SetActive(false);
        }

        // 顯示預設物件
        defaultObject.SetActive(true);
        currentObjectIndex = -1;
    }

    // 強制顯示指定索引的工具物件，用於外部調用
    public void ForceShowToolObject(int index)
    {
        if (index >= 0 && index < toolObjects.Length)
        {
            ShowToolObject(index);
        }
        else
        {
            Debug.LogError("嘗試顯示的工具物件索引超出範圍：" + index);
        }
    }

    // 重置為初始狀態的方法，可從其他腳本調用
    public void Reset()
    {
        ShowDefaultObject();
    }
}