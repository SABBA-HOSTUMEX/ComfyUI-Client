using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class ServeyCake : MonoBehaviour
{
    [SerializeField] private Button SurveyButton;
    private Data data;
    // Start is called before the first frame update
    void Start()
    {
        data = FindObjectOfType<Data>();
        SurveyButton.onClick.AddListener(OpenWebPage);
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "Cake")
            SurveyButton.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OpenWebPage()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        if(currentSceneName == "Cake")
        {
            Application.OpenURL("https://www.surveycake.com/s/nzZVQ");
        }
        if(currentSceneName == "SampleScene")
        {
            if(data.Code != null)
            {
                string CurrentCode = data.Code;
                GUIUtility.systemCopyBuffer = CurrentCode;
                Application.OpenURL("https://www.surveycake.com/s/oZNWe");
            }           
        }
    }
}
