using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultHistory : MonoBehaviour
{
    [Header("Image")]
    [SerializeField]
    private RawImage PreImage;
    [SerializeField]
    private RawImage NewImage;
    [Header("NewData")]
    [SerializeField]
    private TMP_Text NewSeed;
    [SerializeField]
    private TMP_Text NewStep;
    [SerializeField]
    private TMP_Text NewCFG;
    [SerializeField]
    private TMP_Text NewPrompt;
    [Header("PreData")]
    [SerializeField]
    private TMP_Text PreSeed;
    [SerializeField]
    private TMP_Text PreStep;
    [SerializeField]
    private TMP_Text PreCFG;
    [SerializeField]
    private TMP_Text PrePrompt;
    [Header("Script")]
    [SerializeField]
    private texttoimage text2image;
    [Header("CanvaGroup")]
    [SerializeField]
    public CanvasGroup resulthistory;
    // Start is called before the first frame update
    void Start()
    {
        resulthistory.alpha = 0;
        resulthistory.interactable = false;
        resulthistory.blocksRaycasts = false;
    }
    // Update is called once per frame
    void Update()
    {
       
    }
    public void showcompare()
    {
        resulthistory.alpha = 1;
        resulthistory.interactable = true;
        resulthistory.blocksRaycasts = true;
    }
    public void hidecompare()
    {
        resulthistory.alpha = 0;
        resulthistory.interactable = false;
        resulthistory.blocksRaycasts = false;
    }
    public void HistoryImageUpdate(Texture2D texture)
    {
        // 保存之前的圖片
        if (NewImage.texture != null)
        {
            PreImage.texture = NewImage.texture;

            // 調整 PreImage 的尺寸以匹配其紋理
            if (PreImage.texture is Texture2D preTexture)
            {
                RectTransform preRectTransform = PreImage.GetComponent<RectTransform>();
                preRectTransform.sizeDelta = new Vector2(preTexture.width, preTexture.height);
            }
        }

        // 設置新圖片
        NewImage.texture = texture;

        // 調整 NewImage 的尺寸以匹配新紋理
        RectTransform newRectTransform = NewImage.GetComponent<RectTransform>();
        newRectTransform.sizeDelta = new Vector2(texture.width, texture.height);
    }
    public void HistoryUpdate(string seed, string step, string cfg, string prompt)
    {
        if(NewSeed.text != null || NewStep.text != null || NewCFG.text != null || NewPrompt.text != null)
        {
            PreSeed.text = NewSeed.text;
            PreStep.text = NewStep.text;
            PreCFG.text = NewCFG.text;
            PrePrompt.text = NewPrompt.text;
        }
        else
        {
            PreSeed.text = "None";
            PreStep.text = "None";
            PreCFG.text = "None";
            PrePrompt.text = "None";
        }
        NewSeed.text = $"Seed : " + seed;
        NewStep.text = $"Step : " + step;
        NewCFG.text = $"CFG : " + cfg;
        NewPrompt.text = $"Prompt : " + prompt;
    }
}
