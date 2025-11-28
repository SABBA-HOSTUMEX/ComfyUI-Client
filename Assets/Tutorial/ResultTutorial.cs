using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultTutorial : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button[] resultButtons = new Button[4];

    [Header("Images")]
    [SerializeField] private GameObject[] resultImages = new GameObject[4];

    [Header("Default Text")]
    [SerializeField] private GameObject defaultText;

    private int activeImageIndex = -1; // -1 means no image is active

    // Start is called before the first frame update
    void Start()
    {
        InitializeButtons();
        HideAllImages();
        ShowDefaultText();
    }

    private void InitializeButtons()
    {
        // Add click listeners to all buttons
        for (int i = 0; i < resultButtons.Length; i++)
        {
            int buttonIndex = i; // Create a local variable to capture the correct index
            if (resultButtons[i] != null)
            {
                resultButtons[i].onClick.AddListener(() => HandleButtonClick(buttonIndex));
            }
            else
            {
                Debug.LogWarning($"Button at index {i} is null!");
            }
        }
    }

    private void HandleButtonClick(int buttonIndex)
    {
        // Check if button index is valid
        if (buttonIndex < 0 || buttonIndex >= resultImages.Length)
        {
            Debug.LogError($"Button index {buttonIndex} is out of range!");
            return;
        }

        // If the same button is clicked twice
        if (buttonIndex == activeImageIndex)
        {
            // Hide the active image and show default text
            HideImage(buttonIndex);
            ShowDefaultText();
            activeImageIndex = -1;
        }
        else
        {
            // Hide the previously active image (if any)
            if (activeImageIndex != -1)
            {
                HideImage(activeImageIndex);
            }

            // Show the new image and hide default text
            ShowImage(buttonIndex);
            HideDefaultText();
            activeImageIndex = buttonIndex;
        }
    }

    private void ShowImage(int index)
    {
        if (index >= 0 && index < resultImages.Length && resultImages[index] != null)
        {
            resultImages[index].SetActive(true);
        }
    }

    private void HideImage(int index)
    {
        if (index >= 0 && index < resultImages.Length && resultImages[index] != null)
        {
            resultImages[index].SetActive(false);
        }
    }

    private void HideAllImages()
    {
        for (int i = 0; i < resultImages.Length; i++)
        {
            HideImage(i);
        }
    }

    private void ShowDefaultText()
    {
        if (defaultText != null)
        {
            defaultText.SetActive(true);
        }
    }

    private void HideDefaultText()
    {
        if (defaultText != null)
        {
            defaultText.SetActive(false);
        }
    }
}