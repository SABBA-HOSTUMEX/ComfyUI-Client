using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneSwitcher : MonoBehaviour
{
    public Button ChangeScene;
    [SerializeField] private GameObject code;
    private void Start()
    {
        if(code != null)
            DontDestroyOnLoad(code);
        ChangeScene.onClick.AddListener(SwitchScene);
    }
    public void SwitchScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        switch (currentSceneName)
        {
            case "SampleScene":
                SceneManager.LoadScene("Tutorial");
                break;
            case "Tutorial":
                SceneManager.LoadScene("SampleScene");
                break;
            case "Cake":
                SceneManager.LoadScene("Tutorial");
                break;
        }
    }
}
