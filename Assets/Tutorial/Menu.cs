using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using DhafinFawwaz.AnimationUILib;


public class Menu : MonoBehaviour
{
    [Header("Menu Button")]
    [SerializeField] private Button GAI;
    [SerializeField] private Button Model;
    [SerializeField] private Button Latent;
    [SerializeField] private Button Ksampler;
    [SerializeField] private Button VAE;
    [SerializeField] private Button Prompt;
    [SerializeField] private Button Tool;
    [Header("CloseWindow Button")]
    [SerializeField] private Button GAI2Home;
    [SerializeField] private Button Model2Home;
    [SerializeField] private Button Latent2Home;
    [SerializeField] private Button Ksampler2Home;
    [SerializeField] private Button VAE2Home;
    [SerializeField] private Button Prompt2Home;
    [SerializeField] private Button Tool2Home;
    [Header("UI Animation")]
    [SerializeField] private AnimationUI animationUI;
    [SerializeField] private AnimationUI BaseGAIAnimation;
    [SerializeField] private AnimationUI BaseModelAnimation;
    [SerializeField] private AnimationUI BaseLatentAnimation;
    [SerializeField] private AnimationUI BaseKsamplerAnimation;
    [SerializeField] private AnimationUI BaseVAEAnimation;
    [SerializeField] private AnimationUI BasePromptAnimation;
    [SerializeField] private AnimationUI BaseToolAnimation;
    // Start is called before the first frame update
    void Start()
    {
        GAI.onClick.AddListener(GAIpress);
        Model.onClick.AddListener(Modelpress);
        Latent.onClick.AddListener(Latentpress);
        Ksampler.onClick.AddListener(Ksamplerpress);
        VAE.onClick.AddListener(VAEpress);
        Prompt.onClick.AddListener(Promptpress);
        Tool.onClick.AddListener(Toolpress);
        GAI2Home.onClick.AddListener(GAI2home);
        Model2Home.onClick.AddListener(Model2home);
        Latent2Home.onClick.AddListener(Latent2home);
        Ksampler2Home.onClick.AddListener(Ksampler2home);
        VAE2Home.onClick.AddListener(VAE2home);
        Prompt2Home.onClick.AddListener(Prompt2home);
        Tool2Home.onClick.AddListener(Tool2home);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private async Task AnimationReversed()
    {
        animationUI.PlayReversed();

        // 假設您知道動畫的持續時間（例如 2 秒）
        await Task.Delay(900); // 2000 毫秒 = 2 秒
    }

    private async void GAIpress()
    {
        await AnimationReversed();
        BaseGAIAnimation.Play(); // 這行會在動畫完成後執行
    }
    private async void Modelpress()
    {
        await AnimationReversed();
        BaseModelAnimation.Play();
    }
    private async void Latentpress()
    {
        await AnimationReversed();
        BaseLatentAnimation.Play();
    }
    private async void Ksamplerpress()
    {
        await AnimationReversed();
        BaseKsamplerAnimation.Play();
    }
    private async void VAEpress()
    {
        await AnimationReversed();
        BaseVAEAnimation.Play();
    }
    private async void Promptpress()
    {
        await AnimationReversed();
        BasePromptAnimation.Play();
    }
    private async void Toolpress()
    {
        await AnimationReversed();
        BaseToolAnimation.Play();
    }
    private async void GAI2home()
    {
        BaseGAIAnimation.PlayReversed();
        await Task.Delay(100);
        animationUI.Play();
    }
    private async void Model2home()
    {
        BaseModelAnimation.PlayReversed();
        await Task.Delay(100);
        animationUI.Play();
    }
    private async void Latent2home()
    {
        BaseLatentAnimation.PlayReversed();
        await Task.Delay(100);
        animationUI.Play();
    }
    private async void Ksampler2home()
    {
        BaseKsamplerAnimation.PlayReversed();
        await Task.Delay(100);
        animationUI.Play();
    }
    private async void VAE2home()
    {
        BaseVAEAnimation.PlayReversed();
        await Task.Delay(100);
        animationUI.Play();
    }
    private async void Prompt2home()
    {
        BasePromptAnimation.PlayReversed();
        await Task.Delay(100);
        animationUI.Play();
    }
    private async void Tool2home()
    {
        BaseToolAnimation.PlayReversed();
        await Task.Delay(100);
        animationUI.Play();
    }
}
