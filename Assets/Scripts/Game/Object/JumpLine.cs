using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class JumpLine : MonoBehaviour
{
    [Header("ジャンプ設定")]
    public Transform targetPoint;   // 着地点（Transformで指定）
    public float maxHeight = 6f;

    public LineRenderer lineRenderer;
    [Header("予測線の設定")]
    public int predictionSteps = 30;
    public float predictionTimeStep = 0.1f;

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

    private void CalculateInitialVelocity()
    {
        Vector2 start = transform.position;
        Vector2 end = targetPoint.position;
        float dx = end.x - start.x;
        float dy = end.y - start.y;
        float g = Mathf.Abs(Physics2D.gravity.y);

        float Vy = Mathf.Sqrt(2 * g * maxHeight);
        float t_up = Vy / g;
        float t_down = Mathf.Sqrt(2 * (maxHeight - dy) / g);
        float totalTime = t_up + t_down;

        float Vx = dx / totalTime;

        initialVelocity = new Vector2(Vx, Vy);
    }

    private void DrawPredictionLine()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = predictionSteps;

        Vector3[] positions = new Vector3[predictionSteps];
        Vector2 startPos = transform.position;

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
