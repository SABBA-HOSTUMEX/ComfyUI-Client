using UnityEngine;
using UnityEngine.UI;

public class KSamplerImage : MonoBehaviour
{
    // 引用三個需要顯示/隱藏的物件
    [SerializeField] private GameObject objectA;
    [SerializeField] private GameObject objectB;
    [SerializeField] private GameObject objectC;

    // 引用三個按鈕
    [SerializeField] private Button buttonA;
    [SerializeField] private Button buttonB;
    [SerializeField] private Button buttonC;

    // 引用第一頁的文字組件
    [SerializeField] private Text firstPageText;

    // 追蹤當前顯示的物件索引，-1表示沒有物件顯示
    private int currentObjectIndex = -1;

    void Start()
    {
        // 初始狀態：隱藏所有物件，顯示第一頁文字
        HideAllObjects();
        ShowFirstPageText();

        // 為按鈕添加點擊事件
        if (buttonA != null)
            buttonA.onClick.AddListener(() => ToggleObject(0));

        if (buttonB != null)
            buttonB.onClick.AddListener(() => ToggleObject(1));

        if (buttonC != null)
            buttonC.onClick.AddListener(() => ToggleObject(2));
    }

    // 切換物件顯示狀態的方法
    private void ToggleObject(int index)
    {
        // 如果點擊的按鈕對應的物件已經顯示，則隱藏該物件並顯示第一頁文字
        if (currentObjectIndex == index)
        {
            HideAllObjects();
            ShowFirstPageText();
            currentObjectIndex = -1;
        }
        // 否則顯示對應的物件並隱藏第一頁文字
        else
        {
            HideAllObjects();
            ShowObject(index);
            HideFirstPageText();
            currentObjectIndex = index;
        }
    }

    // 隱藏所有物件的方法
    private void HideAllObjects()
    {
        if (objectA != null) objectA.SetActive(false);
        if (objectB != null) objectB.SetActive(false);
        if (objectC != null) objectC.SetActive(false);
    }

    // 顯示特定物件的方法
    private void ShowObject(int index)
    {
        switch (index)
        {
            case 0:
                if (objectA != null) objectA.SetActive(true);
                break;
            case 1:
                if (objectB != null) objectB.SetActive(true);
                break;
            case 2:
                if (objectC != null) objectC.SetActive(true);
                break;
        }
    }

    // 顯示第一頁文字的方法
    private void ShowFirstPageText()
    {
        if (firstPageText != null)
            firstPageText.gameObject.SetActive(true);
    }

    // 隱藏第一頁文字的方法
    private void HideFirstPageText()
    {
        if (firstPageText != null)
            firstPageText.gameObject.SetActive(false);
    }

    // 公開方法，可從其他腳本調用以重置狀態
    public void Reset()
    {
        HideAllObjects();
        ShowFirstPageText();
        currentObjectIndex = -1;
    }
}