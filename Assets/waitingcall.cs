using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class waitingcall : MonoBehaviour
{
    [SerializeField]
    private Button show;
    private bool isprocessing;
    [SerializeField]
    private GoogleDriveManager googleDrive;
    [SerializeField]
    private FeatureVisualizer featureVisualizer;
    [SerializeField]
    private ConditioningVisualizer conditioningVisualizer;
    [SerializeField]
    private VAEVisualizer vAEVisualizer;
    // Start is called before the first frame update
    void Start()
    {
        isprocessing = false;
        show.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        Func<bool> isAllDone = () => googleDrive.donepic + featureVisualizer.donelatent + conditioningVisualizer.donecon + vAEVisualizer.donevae == 4;
        show.gameObject.SetActive(isAllDone());
    }
}
