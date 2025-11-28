using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DhafinFawwaz.AnimationUILib;
using System.Threading.Tasks;

public class TutorialUIController : MonoBehaviour
{
    [Header("Image")]
    [SerializeField] private Image image;
    [Header("Tutorial Buttons and Animations")]
    [SerializeField] private Button[] tutorialButtons;
    [SerializeField] private AnimationUI[] animationUIs;

    [Header("Default Object")]
    [SerializeField] private GameObject defaultObject; // The default object to hide/show

    [Header("Settings")]
    [SerializeField] private float delayToShowObject = 0.5f; // Delay in seconds before showing object again

    private int lastButtonPressed = -1; // -1 means no button was pressed before
    private bool isTransitioning = false; // Flag to prevent multiple simultaneous transitions

    // Start is called before the first frame update
    void Start()
    {
        InitializeButtons();
    }

    private void InitializeButtons()
    {
        // Validate array sizes
        if (tutorialButtons.Length != animationUIs.Length)
        {
            Debug.LogError("The number of buttons and animation UIs must match!");
            return;
        }

        // Add click listeners to all buttons
        for (int i = 0; i < tutorialButtons.Length; i++)
        {
            int buttonIndex = i; // Create a local variable to capture the correct index
            if (tutorialButtons[i] != null)
            {
                tutorialButtons[i].onClick.AddListener(() => HandleButtonClick(buttonIndex));
            }
            else
            {
                Debug.LogWarning($"Button at index {i} is null!");
            }
        }
    }

    private async void HandleButtonClick(int buttonIndex)
    {
        // Check if button index is valid
        if (buttonIndex < 0 || buttonIndex >= animationUIs.Length)
        {
            Debug.LogError($"Button index {buttonIndex} is out of range!");
            return;
        }

        // Ignore clicks during transitions
        if (isTransitioning)
            return;

        if (buttonIndex == lastButtonPressed)
        {
            // Same button clicked twice - play animation in reverse and show object after delay
            isTransitioning = true;
            await PlayAnimationReversed(buttonIndex);
            lastButtonPressed = -1; // Reset last button pressed
            isTransitioning = false;
        }
        else if (lastButtonPressed != -1)
        {
            // Different button clicked while another is active - reverse previous animation first
            isTransitioning = true;

            // Reverse the previous animation and wait
            await PlayAnimationReversed(lastButtonPressed);

            // Now play the new animation and hide object
            PlayAnimationAndHideObject(buttonIndex);
            lastButtonPressed = buttonIndex;
            isTransitioning = false;
        }
        else
        {
            // First button clicked or after reset - play animation and hide object
            PlayAnimationAndHideObject(buttonIndex);
            lastButtonPressed = buttonIndex;
        }
    }

    private void PlayAnimationAndHideObject(int index)
    {
        AnimationUI anim = GetAnimationUI(index);

        if (anim != null && defaultObject != null)
        {
            anim.Play();
            defaultObject.SetActive(false);
            Color tempColor = image.color;
            tempColor.a = 0;
            image.color = tempColor;
        }
    }

    private async Task PlayAnimationReversed(int index)
    {
        AnimationUI anim = GetAnimationUI(index);

        if (anim != null && defaultObject != null)
        {
            anim.PlayReversed();

            // Wait for delay before showing the object again
            await Task.Delay((int)(delayToShowObject * 1000));

            // Check if the component is still valid (not destroyed)
            if (this != null && defaultObject != null)
            {
                Color tempColor = image.color;
                tempColor.a = 1;
                image.color = tempColor;
                defaultObject.SetActive(true);
            }
        }
    }

    private AnimationUI GetAnimationUI(int index)
    {
        if (index >= 0 && index < animationUIs.Length)
        {
            return animationUIs[index];
        }
        return null;
    }
}