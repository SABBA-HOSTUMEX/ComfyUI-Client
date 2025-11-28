using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class icon : MonoBehaviour
{
    [Header("waiting icon")]
    [SerializeField] private GameObject[] wait;
    [Header("finish icon")]
    [SerializeField] private GameObject[] image;

    // Start is called before the first frame update
    void OnEnable()
    {
        foreach(GameObject obj in wait)
        {
            obj.SetActive(true);
        }
        foreach (GameObject obj in image)
        {
            obj.SetActive(false);
        }
    }
    public void changeui(int value)
    {
        wait[value].SetActive(false);
        image[value].SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
