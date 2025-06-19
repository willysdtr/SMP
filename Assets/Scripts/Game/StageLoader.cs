using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using StageInfo;

public class StageUICanvasLoader : MonoBehaviour
{
    [Header("Stage Data")]
    public StageID stageId;  // Entire grid with front+back concatenated horizontally

    [Header("UI References")]
    public RectTransform leftPanel;    // Left fabric panel RectTransform (front)
    public RectTransform rightPanel;   // Right fabric panel RectTransform (back)
    public GameObject tileUIPrefab;    // Prefab of a UI tile (Image)

    private int rows;
    private int cols;

    private List<List<int>> stageGrid;
    private StageData stage;

    void Start()
    {
        switch(stageId)
        {
            default:
                Debug.LogWarning("No StageID in list");
                break;

            case StageID.Stage1_1:
                stage = Stage1.Stage1_1;
                break;
            case StageID.Stage1_2:
                stage = Stage1.Stage1_2;
                break;

            case StageID.Stage2_1:
                stage = Stage2.Stage2_1;
                break;
            case StageID.Stage2_2:
                stage = Stage2.Stage2_2;
                break;

            case StageID.Stage3_1:
                stage = Stage3.Stage3_1;
                break;
            case StageID.Stage3_2:
                stage = Stage3.Stage3_2;
                break;
        }

        GenerateStageGrid();

        if (stageGrid == null || stageGrid.Count == 0)
        {
            Debug.LogWarning("StageGrid not set or empty");
            return;
        }

        rows = stageGrid.Count;
        cols = stageGrid[0].Count;

        SetupGrid(leftPanel, 0, cols / 2);
        SetupGrid(rightPanel, cols / 2, cols);
    }

    private void GenerateStageGrid()
    {
        if (stage == null || tileUIPrefab == null)
        {
            Debug.LogWarning("StageData Ç‹ÇΩÇÕ TilePrefab Ç™ñ¢ê›íËÇ≈Ç∑ÅB");
            return;
        }

        int offset = stage.STAGE_WIDTH;
        int width = stage.STAGE_WIDTH * 2;
        int height = stage.STAGE_HEIGHT;

        // Resize the outer list (rows)
        stageGrid = new List<List<int>>(height);
        for (int y = 0; y < height; y++)
        {
            var row = new List<int>(width);
            for (int x = 0; x < width; x++)
            {
                row.Add(0); // Initialize all tiles to 0
            }
            stageGrid.Add(row);
        }

        // Start positions
        stageGrid[stage.START_POS_front.Y][stage.START_POS_front.X] = 1;
        stageGrid[stage.START_POS_back.Y][stage.START_POS_back.X + offset] = 1;

        // Goal positions
        stageGrid[stage.GOAL_POS_front.Y][stage.GOAL_POS_front.X] = 2;
        stageGrid[stage.GOAL_POS_back.Y][stage.GOAL_POS_back.X + offset] = 2;

        // Soul position
        if (!stage.SOUL_POS.IsLeft)
            stageGrid[stage.SOUL_POS.Y][stage.SOUL_POS.X] = 3;
        else
            stageGrid[stage.SOUL_POS.Y][stage.SOUL_POS.X + offset] = 3;

        // Set objects from arrays
        SetObjFromInt2(stageGrid, 4, stage.STEEL_front, false, offset);
        SetObjFromInt2(stageGrid, 4, stage.STEEL_back, true, offset);

        SetObjFromInt2(stageGrid, 5, stage.WRINKLE_front, false, offset);
        SetObjFromInt2(stageGrid, 5, stage.WRINKLE_back, true, offset);

        // Set wind
        SetObjFromWind(stageGrid, 6, stage.WIND_front, false, offset);
        SetObjFromWind(stageGrid, 6, stage.WIND_back, true, offset);


    }

    private void SetObjFromInt2(List<List<int>> grid, int id, IReadOnlyList<StageInfo.Int2> positions, bool isBack, int offset)
    {
        foreach (var pos in positions)
        {
            int x = pos.X + (isBack ? offset : 0);
            int y = pos.Y;
            if (y >= 0 && y < grid.Count && x >= 0 && x < grid[0].Count)
            {
                grid[y][x] = id;
            }
        }
    }

    private void SetObjFromWind(List<List<int>> grid, int id, IReadOnlyList<StageInfo.WindPos> positions, bool isBack, int offset)
    {
        foreach (var pos in positions)
        {
            int x = pos.X + (isBack ? offset : 0);
            int y = pos.Y;
            if (y >= 0 && y < grid.Count && x >= 0 && x < grid[0].Count)
            {
                grid[y][x] = id + (int)pos.Dir; // Optionally encode direction into ID (e.g. id + (int)pos.Dir)
            }
        }
    }

    void SetupGrid(RectTransform panel, int colStart, int colEnd)
    {
        int gridCols = colEnd - colStart;
        int gridRows = rows;

        // Clear existing children
        foreach (Transform child in panel)
            Destroy(child.gameObject);

        // Add or get GridLayoutGroup component for automatic grid layout
        GridLayoutGroup gridLayout = panel.GetComponent<GridLayoutGroup>();
        if (gridLayout == null)
            gridLayout = panel.gameObject.AddComponent<GridLayoutGroup>();

        // Calculate tile size to fit inside panel (account for padding if needed)
        float tileWidth = panel.rect.width / gridCols;
        float tileHeight = panel.rect.height / gridRows;
        float tileSize = Mathf.Min(tileWidth, tileHeight);

        gridLayout.cellSize = new Vector2(tileSize, tileSize);
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = gridCols;
        gridLayout.spacing = Vector2.zero;
        gridLayout.padding = new RectOffset(0, 0, 0, 0);

        // Create tiles inside this panel
        for (int y = 0; y < gridRows; y++)
        {
            for (int x = 0; x < gridCols; x++)
            {
                int tileId = stageGrid[y][colStart + x];
                GameObject tile = Instantiate(tileUIPrefab, panel);

                tile.name = $"Tile_{x}_{y}";

                // Customize tile color based on tileId (example)
                var img = tile.GetComponent<Image>();
                if (img != null)
                {
                    img.color = TileColorFromId(tileId);
                }
            }
        }
    }

    Color TileColorFromId(int id)
    {
        switch (id)
        {
            case 1: return Color.green;   // start
            case 2: return Color.red;     // goal
            case 3: return Color.yellow;  // soul
            case 4: return Color.gray;    // steel
            case 5: return new Color(1f, 0.5f, 0f); // wrinkle (orange)
            case 6:
            case 7:
            case 8:
            case 9: return Color.cyan;    // wind
            default: return Color.clear;  // empty
        }
    }
}
