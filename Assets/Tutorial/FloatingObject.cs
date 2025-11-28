using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    // 漂浮的高度範圍
    public float floatHeight = 0.5f;

    // 漂浮的速度
    public float floatSpeed = 1.0f;

    // 可選參數：是否使用正弦波動或者平滑的來回運動
    public bool useSineWave = true;

    // 可選參數：為漂浮效果添加一點隨機性
    public bool addRandomness = false;
    public float randomFactor = 0.1f;

    // 記錄初始位置
    private Vector3 startPosition;
    private float randomOffset;

    void Start()
    {
        // 保存物件的初始位置
        startPosition = transform.position;

        // 如果使用隨機性，生成一個隨機偏移值
        if (addRandomness)
        {
            randomOffset = Random.Range(0f, 2f * Mathf.PI);
        }
    }

    void Update()
    {
        // 根據時間計算新的Y位置
        float newY;

        if (useSineWave)
        {
            // 使用正弦波動，產生更自然的漂浮效果
            float time = Time.time * floatSpeed + randomOffset;
            newY = startPosition.y + Mathf.Sin(time) * floatHeight;
        }
        else
        {
            // 使用Ping-Pong函數，在指定範圍內平滑來回移動
            float pingPong = Mathf.PingPong(Time.time * floatSpeed, 2f) - 1f; // 範圍從-1到1
            newY = startPosition.y + pingPong * floatHeight;
        }

        // 如果啟用隨機因子，添加額外的微小隨機移動
        if (addRandomness)
        {
            newY += Mathf.Sin(Time.time * floatSpeed * 2.5f + randomOffset * 3.7f) * randomFactor;
        }

        // 更新物件位置，只改變Y坐標
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}