using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DhafinFawwaz.AnimationUILib;
using System.Threading.Tasks;

public class KsamplerTutorial : MonoBehaviour
{
    [Header("Button")]
    [SerializeField] private Button TutorialModel;
    [SerializeField] private Button TutorialLatent;
    [SerializeField] private Button TutorialKsampler;
    [SerializeField] private Button TutorialPrompt;

    [Header("AnimationUI")]
    [SerializeField] private AnimationUI animationUI;
    [SerializeField] private AnimationUI animationUI2;
    [SerializeField] private AnimationUI animationUI3;
    [SerializeField] private AnimationUI animationUI4;

    [Header("Default Object")]
    [SerializeField] private GameObject defaultObject; // The default object to hide/show

    [Header("Settings")]
    [SerializeField] private float delayToShowObject = 0.5f; // Delay in seconds before showing object again

    private int lastButtonPressed = -1; // -1 means no button was pressed before
    private bool isTransitioning = false; // Flag to prevent multiple simultaneous transitions

    // Start is called before the first frame update
    void Start()
    {
        TutorialModel.onClick.AddListener(() => HandleButtonClick(0));
        TutorialLatent.onClick.AddListener(() => HandleButtonClick(1));
        TutorialKsampler.onClick.AddListener(() => HandleButtonClick(2));
        TutorialPrompt.onClick.AddListener(() => HandleButtonClick(3));
    }

    private async void HandleButtonClick(int buttonIndex)
    {
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
                defaultObject.SetActive(true);
            }
        }
    }

    private AnimationUI GetAnimationUI(int index)
    {
        switch (index)
        {
            case 0: return animationUI;
            case 1: return animationUI2;
            case 2: return animationUI3;
            case 3: return animationUI4;
            default: return null;
        }
    }
}