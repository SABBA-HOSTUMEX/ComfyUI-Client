using UnityEngine;
using UnityEngine.UI;

public class ImageSwitch : MonoBehaviour
{
    // 七張圖片的引用
    [SerializeField] private Image[] images;

    // 七個按鈕的引用
    [SerializeField] private Button[] buttons;

    // 文字組件引用
    [SerializeField] private Text informationText;

    // 追蹤當前顯示的圖片索引，-1表示沒有圖片顯示
    private int currentImageIndex = -1;

    void Start()
    {
        // 確保陣列長度正確
        
        // 初始狀態：隱藏所有圖片，顯示文字
        HideAllImages();
        ShowText();

        // 為每個按鈕添加點擊事件
        for (int i = 0; i < buttons.Length; i++)
        {
            int buttonIndex = i; // 創建本地變數以便在Lambda表達式中使用
            buttons[i].onClick.AddListener(() => ToggleImage(buttonIndex));
        }
    }
    public void Reset()
    {
        HideAllImages();
        ShowText();
    }
    //
    // 隱藏所有圖片的方法
    private void HideAllImages()
    {
        foreach (var image in images)
        {
            image.gameObject.SetActive(false);
        }
        currentImageIndex = -1;
    }

    // 顯示文字的方法
    private void ShowText()
    {
        if (informationText != null)
        {
            informationText.gameObject.SetActive(true);
        }
    }

    // 隱藏文字的方法
    private void HideText()
    {
        if (informationText != null)
        {
            informationText.gameObject.SetActive(false);
        }
    }

    // 切換圖片顯示的方法
    private void ToggleImage(int index)
    {
        // 如果點擊的按鈕對應的圖片已經顯示，則隱藏該圖片並顯示文字
        if (currentImageIndex == index)
        {
            HideAllImages();
            ShowText();
        }
        // 否則顯示對應的圖片並隱藏文字
        else
        {
            HideAllImages();
            images[index].gameObject.SetActive(true);
            currentImageIndex = index;
            HideText();
        }
    }
}