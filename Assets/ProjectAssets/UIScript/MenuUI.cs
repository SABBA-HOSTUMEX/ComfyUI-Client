using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DhafinFawwaz.AnimationUILib;

public class MenuUI : MonoBehaviour
{
    [Header("Canva")]
    public Camera camera;
    ////////// workflow ui //////////
    public Canvas TextToImagePromptUI;
    public Canvas KSamplerUI;
    public Canvas EmptyLatentImageUI;
    public Canvas CheckpointLoaderSimpleUI;
    ////////// workflow ui //////////
    [Header("Button")]
    public Button txt2img;
    public Button Homepage;
    //public Button generatehide;
    public Button seeresult;
    public Button compare;
    public Button backresult;
    public TMP_Text MenuUItxt;
    [Header("GameObject")]
    public GameObject buttonlist;
    public GameObject Changeworkflowui_txt2img;
    public GameObject TextToImage;
    public GameObject error;
    public GameObject ServerBusy;
    public GameObject Noconnect;
    public GameObject ServeyCake;
    [Header("Canva Group")]
    [SerializeField]
    private CanvasGroup ShowConsquence;
    [SerializeField]
    private GameObject WaitForResult;
    [SerializeField]
    private CanvasGroup VideoCanva;
    [Header("Data Process")]
    [SerializeField]
    private GoogleDriveManager googleDrive;
    [SerializeField]
    private FeatureVisualizer featureVisualizer;
    [SerializeField]
    private ConditioningVisualizer conditioningVisualizer;
    private string statusstring;
    public AnimationUI _animationUI;
    public AnimationUI _animationUI2;
    public AnimationUI _animationUI3;
    // Start is called before the first frame update
    void Start()
    {
        txt2img.onClick.AddListener(Text2Image);
        Homepage.onClick.AddListener(SetHomepage);
        //generatehide.onClick.AddListener(ShowLoadingUI);
        seeresult.onClick.AddListener(SeeResult);
        backresult.onClick.AddListener(BackToResult);
        compare.onClick.AddListener(SeeCompare);
        VideoCanva.alpha = 0;
        VideoCanva.interactable = false;
        VideoCanva.blocksRaycasts = false;
        SetHomepage();
    }
    // Update is called once per frame
    private void SetHomepage()
    {
        VideoCanva.alpha = 0;
        ////////// main Scene object //////////
        camera.clearFlags = CameraClearFlags.SolidColor;
        Camera.main.backgroundColor = Color.black;
        Homepage.gameObject.SetActive(false);
        // ConnectUI.gameObject.SetActive(false);      
        Changeworkflowui_txt2img.gameObject.SetActive(false);
        ////////// main Scene object //////////

        ////////// menu ui //////////
        MenuUItxt.gameObject.SetActive(true);
        buttonlist.SetActive(true);
        ////////// menu ui //////////
        TextToImage.SetActive(false);
        WaitForResult.SetActive(false);
        ShowConsquence.gameObject.SetActive(false);
        error.SetActive(false);
        ServerBusy.SetActive(false);
        Noconnect.SetActive(false);
        ServeyCake.SetActive(false);
    }
    private void SetScene()
    {
        ////////// main Scene object //////////
        camera.clearFlags = CameraClearFlags.Skybox;
        Camera.main.backgroundColor = Color.black;
        Homepage.gameObject.SetActive(true);
        
        ////////// main Scene object //////////
        
        
        ////////// workflow ui //////////
        TextToImagePromptUI.gameObject.SetActive(false);
        KSamplerUI.gameObject.SetActive(false);
        EmptyLatentImageUI.gameObject.SetActive(false);
        CheckpointLoaderSimpleUI.gameObject.SetActive(true);
        ////////// workflow ui //////////
        

        ////////// menu ui //////////
        MenuUItxt.gameObject.SetActive(false);
        buttonlist.SetActive(false);
        ////////// menu ui //////////
    }

    public void Text2Image()
    {
        ////////// Text2Image Scene object //////////
        TextToImage.SetActive(true);
        Changeworkflowui_txt2img.SetActive(true);
        Homepage.gameObject.SetActive(true);
        ////////// Text2Image Scene object //////////

        ////////// menu ui //////////
        MenuUItxt.gameObject.SetActive(false);
        buttonlist.SetActive(false);
        ////////// menu ui //////////      
        ShowConsquence.gameObject.SetActive(false);
        VideoCanva.alpha = 1;
        _animationUI.Play();    
        _animationUI2.Play();    
        _animationUI3.Play();    
    }
    public void hidehomepage()
    {
        Homepage.gameObject.SetActive(false);
    }
    public void ShowLoadingUI()
    {
        Changeworkflowui_txt2img.SetActive(false);
        WaitForResult.SetActive(true);
        TextToImage.SetActive(false);
        VideoCanva.alpha = 0;
    }
    public void SeeResult()
    {
        VideoCanva.alpha = 1;
        googleDrive.donepic = 0;
        featureVisualizer.donelatent = 0;
        conditioningVisualizer.donecon = 0;
        WaitForResult.SetActive(false);
        ShowConsquence.gameObject.SetActive(true);       
    }
    public void BackToResult()
    {
        ShowLoadingUI();
        SeeResult();
    }
    public void SeeCompare()
    {
        TextToImage.SetActive(false);
        Changeworkflowui_txt2img.SetActive(false);
        Homepage.gameObject.SetActive(false);
        VideoCanva.alpha = 0;
    }
}
