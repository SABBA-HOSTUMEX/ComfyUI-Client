using UnityEngine;
using UnityEngine.UI;

public class ShowImage : MonoBehaviour
{
    // 引用6張要顯示的圖片
    [SerializeField] private Image[] imagesToShow;

    // 引用主要顯示文字的Text組件
    [SerializeField] private Text informationText;

    // 引用與圖片一起顯示的附加Text組件
    [SerializeField] private Text imageDescriptionText;

    // 引用切換按鈕
    [SerializeField] private Button toggleButton;

    // 追蹤圖片當前是否顯示
    private bool imagesVisible = false;

    void Start()
    {

        imageDescriptionText.gameObject.SetActive(false);
        // 初始狀態：隱藏所有圖片，顯示文字
        HideAllImages();
        ShowText();

        // 為按鈕添加點擊事件
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(ToggleImagesAndText);
        }
        else
        {
            Debug.LogError("未設置切換按鈕!");
        }
    }

    // 切換圖片和文字顯示的方法
    private void ToggleImagesAndText()
    {
        // 根據當前狀態切換顯示
        if (imagesVisible)
        {
            // 如果圖片當前顯示，則隱藏圖片、描述文字並顯示主要文字
            HideAllImages();
            HideImageDescriptionText();
            ShowText();
        }
        else
        {
            // 如果圖片當前隱藏，則顯示圖片、描述文字並隱藏主要文字
            ShowAllImages();
            ShowImageDescriptionText();
            HideText();
        }

        // 更新狀態標記
        imagesVisible = !imagesVisible;
    }

    // 隱藏所有圖片的方法
    private void HideAllImages()
    {
        foreach (var image in imagesToShow)
        {
            if (image != null)
            {
                image.gameObject.SetActive(false);
            }
        }
    }

    // 顯示所有圖片的方法
    private void ShowAllImages()
    {
        foreach (var image in imagesToShow)
        {
            if (image != null)
            {
                image.gameObject.SetActive(true);
            }
        }
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

    // 顯示圖片描述文字的方法
    private void ShowImageDescriptionText()
    {
        if (imageDescriptionText != null)
        {
            imageDescriptionText.gameObject.SetActive(true);
        }
    }

    // 隱藏圖片描述文字的方法
    private void HideImageDescriptionText()
    {
        if (imageDescriptionText != null)
        {
            imageDescriptionText.gameObject.SetActive(false);
        }
    }

    // 公開方法，可從其他腳本調用以重置狀態
    public void Reset()
    {
        HideAllImages();
        HideImageDescriptionText();
        ShowText();
        imagesVisible = false;
    }
}