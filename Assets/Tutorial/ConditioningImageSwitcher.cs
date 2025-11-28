using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConditioningImageSwitcher : MonoBehaviour
{
    [Header("Image References")]
    [SerializeField] private Image image1;
    [SerializeField] private Image image2;

    [Header("Sprites for Image 1")]
    [SerializeField] private Sprite[] image1Sprites = new Sprite[3];

    [Header("Sprites for Image 2")]
    [SerializeField] private Sprite[] image2Sprites = new Sprite[3];

    // 使用一個按鈕同時切換兩個圖片的sprite
    public void SwitchBothImageSprites(int spriteIndex)
    {
        // 檢查是否有效的索引值
        if (spriteIndex < 0 || spriteIndex >= 3)
        {
            Debug.LogError($"Invalid sprite index: {spriteIndex}. Valid range is 0-2");
            return;
        }

        // 切換第一個圖片的sprite
        if (image1 != null)
        {
            if (spriteIndex < image1Sprites.Length && image1Sprites[spriteIndex] != null)
            {
                image1.sprite = image1Sprites[spriteIndex];
            }
            else
            {
                Debug.LogWarning($"Sprite at index {spriteIndex} for Image 1 is invalid or null!");
            }
        }
        else
        {
            Debug.LogError("Image 1 reference is missing!");
        }

        // 切換第二個圖片的sprite
        if (image2 != null)
        {
            if (spriteIndex < image2Sprites.Length && image2Sprites[spriteIndex] != null)
            {
                image2.sprite = image2Sprites[spriteIndex];
            }
            else
            {
                Debug.LogWarning($"Sprite at index {spriteIndex} for Image 2 is invalid or null!");
            }
        }
        else
        {
            Debug.LogError("Image 2 reference is missing!");
        }
    }
}
