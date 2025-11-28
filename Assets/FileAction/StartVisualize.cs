using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartVisualize : MonoBehaviour
{
    [SerializeField]
    private VisualizeDataListener visualizeDatalistener;
    [SerializeField]
    private LatentDataLoader latentDataloader;
    [SerializeField]
    private ConditioningMatrixLoader conditioningMatrixloader;
    [SerializeField]
    private VAELoader vAELoader;
    [SerializeField]
    private GoogleDriveManager driveManager;
    private string png;
    private string conditioning;
    private string latent;
    private string vae;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        if(visualizeDatalistener.ALL_FILE_READY)
        {
            VisualizeUpdate();
            visualizeDatalistener.ALL_FILE_READY = false;
        }
    }
    private void VisualizeUpdate()
    {
        png = visualizeDatalistener.picturePath;
        conditioning = visualizeDatalistener.conditioningPath;
        latent = visualizeDatalistener.latentPath;
        vae = visualizeDatalistener.vaePath;
        driveManager.ProcessPNGFile(png);
        latentDataloader.LoadLatentData(latent);
        conditioningMatrixloader.LoadConditioningData(conditioning);
        vAELoader.LoadVAEData(vae);
    }
}
