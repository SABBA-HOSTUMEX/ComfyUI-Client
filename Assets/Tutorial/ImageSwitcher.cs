using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ImageSwitcher : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject[] imageObjects;  // 圖片GameObject陣列
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;

    // 移除loopImages設定，因為我們總是循環顯示圖片

    private int currentImageIndex = 0;
    private int totalImages = 0;

    void Start()
    {
        totalImages = imageObjects.Length;
        nextButton.onClick.AddListener(NextImage);
        prevButton.onClick.AddListener(PreviousImage);
        UpdateDisplay();
    }

    public void NextImage()
    {
        if (currentImageIndex < totalImages - 1)
        {
            currentImageIndex++;
        }
        else
        {
            // 到最後一張後，直接循環到第一張
            currentImageIndex = 0;
        }
        UpdateDisplay();
    }

    public void PreviousImage()
    {
        if (currentImageIndex > 0)
        {
            currentImageIndex--;
        }
        else
        {
            // 在第一張時，直接循環到最後一張
            currentImageIndex = totalImages - 1;
        }
        UpdateDisplay();
    }


    private void UpdateDisplay()
    {
        // 隱藏所有圖片
        for (int i = 0; i < imageObjects.Length; i++)
        {
            if (imageObjects[i] != null)
            {
                imageObjects[i].SetActive(i == currentImageIndex);
            }
        }  
    }

    void OnDestroy()
    {
        // 移除按鈕監聽
        if (nextButton != null)
        {
            nextButton.onClick.RemoveListener(NextImage);
        }
        if (prevButton != null)
        {
            prevButton.onClick.RemoveListener(PreviousImage);
        }
    }
}