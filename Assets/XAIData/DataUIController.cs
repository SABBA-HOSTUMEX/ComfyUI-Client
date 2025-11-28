using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DhafinFawwaz.AnimationUILib;

public class DataUIController : MonoBehaviour
{
    [SerializeField] private Button ConditioningUIBTN;
    [SerializeField] private Button LatentUIBTN;
    [SerializeField] private Button PNGUIBTN;
    [SerializeField] private Button VAEUIBTN;
    [SerializeField] private Button BACKTORESULT;
    [SerializeField] private Button COMPARE;
    [SerializeField] private CanvasGroup ConditioningCanva;
    [SerializeField] private CanvasGroup LatentCanva;
    [SerializeField] private CanvasGroup PNGCanva;
    [SerializeField] private CanvasGroup VAECanva;
    [SerializeField] private GameObject txt2img;
    private CanvasGroup currentActiveCanvas = null;
    // Start is called before the first frame update
    void Start()
    {
        HideAllCanvas();
        ConditioningUIBTN.onClick.AddListener(conditioningbtn);
        LatentUIBTN.onClick.AddListener(latentbtn);
        PNGUIBTN.onClick.AddListener(pngbtn);
        VAEUIBTN.onClick.AddListener(vaebtn);
    }
    public void back2gai()
    {
        txt2img.SetActive(true);
        BACKTORESULT.gameObject.SetActive(true);
        COMPARE.gameObject.SetActive(true);
        HideAllCanvas();
    }
    public void HideAllCanvas()
    {
        SetCanvasState(ConditioningCanva, false);
        SetCanvasState(LatentCanva, false);
        SetCanvasState(PNGCanva, false);
        SetCanvasState(VAECanva, false);
        currentActiveCanvas = null;
    }
    private void SetCanvasState(CanvasGroup canvas, bool isVisible)
    {
        canvas.alpha = isVisible ? 1f : 0f;
        canvas.interactable = isVisible;
        canvas.blocksRaycasts = isVisible;
    }
    private void SwitchToCanvas(CanvasGroup newCanvas)
    {
        // 如果點擊的是當前已經顯示的 Canvas，不做任何事
        if (currentActiveCanvas == newCanvas)
        {
            return;
        }

        // 隱藏所有 Canvas
        SetCanvasState(ConditioningCanva, false);
        SetCanvasState(LatentCanva, false);
        SetCanvasState(PNGCanva, false);
        SetCanvasState(VAECanva, false);


        // 顯示新選擇的 Canvas
        SetCanvasState(newCanvas, true);
        currentActiveCanvas = newCanvas;
    }

    private void conditioningbtn()
    {
        SwitchToCanvas(ConditioningCanva);
    }
    private void latentbtn()
    {
        SwitchToCanvas(LatentCanva);
    }
    private void pngbtn()
    {
        SwitchToCanvas(PNGCanva);
    }
    private void vaebtn()
    {
        SwitchToCanvas(VAECanva);
    }
}
