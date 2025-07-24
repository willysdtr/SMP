using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class JumpLine : MonoBehaviour
{
    //ジャンプ台オブジェクトから放物線を出すスクリプト

    [Header("ジャンプ設定")]
    public Transform targetPoint;   // 着地点（Transformで指定）
    public float maxHeight = 6f;

    public LineRenderer lineRenderer;
    [Header("予測線の設定")]
    public int predictionSteps = 30;
    public float predictionTimeStep = 0.1f;

    [Header("予測線の始点(0.5で中央)")]
    public float startOffsetY = 0.5f;

    private Vector2 initialVelocity;

    private void Start()
    {
        if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();

    }

    private void Update()
    {
        CalculateInitialVelocity();
        DrawPredictionLine();
    }

    private void CalculateInitialVelocity()//放物線の作成
    {
        Vector2 start = transform.position;
        Vector2 end = targetPoint.position;
        float dx = end.x - start.x;
        float dy = end.y - start.y;
        float g = Mathf.Abs(Physics2D.gravity.y);

        float vy = Mathf.Sqrt(2 * g * maxHeight);
        float t_up = vy / g;
        float t_down = Mathf.Sqrt(2 * (maxHeight - dy) / g);
        float totalTime = t_up + t_down;

        float vx = dx / totalTime;

        initialVelocity = new Vector2(vx, vy);
    }

    private void DrawPredictionLine()//放物線の描画
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = predictionSteps;

        Vector3[] positions = new Vector3[predictionSteps];
        Vector2 startPos = (Vector2)transform.position + new Vector2(0f, startOffsetY);

        for (int i = 0; i < predictionSteps; i++)
        {
            float t = i * predictionTimeStep;
            float x = initialVelocity.x * t;
            float y = initialVelocity.y * t + 0.5f * Physics2D.gravity.y * t * t;

            positions[i] = startPos + new Vector2(x, y);
        }

        lineRenderer.SetPositions(positions);
    }


    public Vector2 GetInitialVelocity()
    {
        return initialVelocity;
    }

}
