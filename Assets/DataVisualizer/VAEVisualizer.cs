using UnityEngine;
using UnityEngine.UI;
public class VAEVisualizer : MonoBehaviour
{
    private VAELoader vaeLoader;
    public RawImage rawImage;
    private Texture2D texture;
   
    [SerializeField]
    private int channelToVisualize = 0; // 要顯示哪個channel (0-3)
    [HideInInspector]
    public int donevae;
    [SerializeField] icon Icon;
    void Start()
    {
        vaeLoader = FindObjectOfType<VAELoader>();

        if (vaeLoader == null)
        {
            Debug.LogError("VAELoader not found in scene!");
            return;
        }

       
    }

    public void CreateVisualization()
    {
        float[,,,] latentArray = vaeLoader.GetLatentArray();
        int[] shape = vaeLoader.GetShape();
        float min = vaeLoader.GetMin();
        float max = vaeLoader.GetMax();

        // 創建貼圖
        texture = new Texture2D(shape[2], shape[3], TextureFormat.RGB24, false);

        // 將數據正規化並轉換為圖像
        for (int h = 0; h < shape[2]; h++)
        {
            for (int w = 0; w < shape[3]; w++)
            {
                // 獲取該位置的值並正規化到0-1
                float value = latentArray[0, channelToVisualize, h, w];
                float normalizedValue = Mathf.InverseLerp(min, max, value);

                // 創建顏色
                Color color = new Color(normalizedValue, normalizedValue, normalizedValue, 1);
                texture.SetPixel(w, h, color);
            }
        }

        texture.Apply();

        // 設置RawImage
        rawImage.texture = texture;

        // 調整顯示設定
        texture.filterMode = FilterMode.Point; // 使用最近鄰插值以清晰顯示像素
        donevae = 1;
        Icon.changeui(2);
    }

    // 提供切換channel的方法
    public void SetChannel(int channel)
    {
        if (channel >= 0 && channel < 4)
        {
            channelToVisualize = channel;
            CreateVisualization();
        }
    }

    void OnDestroy()
    {
        if (texture != null)
        {
            Destroy(texture);
        }
    }
}