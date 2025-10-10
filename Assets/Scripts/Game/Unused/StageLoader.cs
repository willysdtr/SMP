using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using StageInfo;

public class StageUICanvasLoader : MonoBehaviour
{
    [Header("Stage Data")]
    public bool useDataFromDropdown;
    public StageID stageId;             //ステージIDから、ステージ選べる

    [Header("UI References")]
    public RectTransform leftPanel;     //左のステージ配置する場所
    public RectTransform rightPanel;    //右のステージ配置する場所
    [SerializeField] private RectTransform canvasRect;

    [Header("Tile Setup")]
    public GameObject tileUIPrefab;     //ステージオブジェクトのPrefab（後もっと増やす）
    public _TileData[] tileDataArray;

    private int rows;
    private int cols;

    private List<List<int>> stageGrid;
    private StageData stage;

    private float tilesize;
    public float TileSize => tilesize;

    private RectTransform m_FrontStartPos;
    private RectTransform m_FrontGoalPos;
    private RectTransform m_BackStartPos;
    private RectTransform m_BackGoalPos;
    public RectTransform FrontStartPos => m_FrontStartPos;
    public RectTransform FrontGoalPos => m_FrontGoalPos;
    public RectTransform BackStartPos => m_BackStartPos;
    public RectTransform BackGoalPos => m_BackGoalPos;

    void Start()
    {
        //チェックマーク付いたら、StageIDから、ステージロード・付けないならばステージセレクトからステージIDを設定する
        if (!useDataFromDropdown) 
        {
            stageId = (StageID)SMPState.CURRENT_STAGE;
        }      

        
        switch (stageId)
        {
            default:
                Debug.LogWarning("リストにステージが見つけない");
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

        GenerateStageGridObjects();

        if (stageGrid == null || stageGrid.Count == 0)
        {
            Debug.LogWarning("ステージグリッドが空");
            return;
        }

        rows = stageGrid.Count;
        cols = stageGrid[0].Count;

        SetupGrid(leftPanel, 0, cols / 2, true);
        SetupGrid(rightPanel, cols / 2, cols, false);
    }

    private void GenerateStageGridObjects()
    {
        if (stage == null || tileUIPrefab == null)
        {
            Debug.LogWarning("StageData または TilePrefab が未設定です。");
            return;
        }

        int offset = stage.STAGE_WIDTH;
        int width = stage.STAGE_WIDTH * 2;
        int height = stage.STAGE_HEIGHT;

        //グリッド設定
        stageGrid = new List<List<int>>(height);
        for (int y = 0; y < height; y++)
        {
            var row = new List<int>(width);
            for (int x = 0; x < width; x++)
            {
                row.Add(0); //全部０にして
            }
            stageGrid.Add(row);
        }

        // スタート配置
        stageGrid[stage.START_POS_front.Y][stage.START_POS_front.X] = 1;
        stageGrid[stage.START_POS_back.Y][stage.START_POS_back.X + offset] = 1;

        // ゴール配置
        stageGrid[stage.GOAL_POS_front.Y][stage.GOAL_POS_front.X] = 2;
        stageGrid[stage.GOAL_POS_back.Y][stage.GOAL_POS_back.X + offset] = 2;

        // 魂配置
        if (!stage.SOUL_POS.IsLeft)
            stageGrid[stage.SOUL_POS.Y][stage.SOUL_POS.X] = 3;
        else
            stageGrid[stage.SOUL_POS.Y][stage.SOUL_POS.X + offset] = 3;

        // 鉄板配置
        SetObjFromInt2(stageGrid, 4, stage.STEEL_front, false, offset);
        SetObjFromInt2(stageGrid, 4, stage.STEEL_back, true, offset);

        // しわ配置
        SetObjFromInt2(stageGrid, 5, stage.WRINKLE_front, false, offset);
        SetObjFromInt2(stageGrid, 5, stage.WRINKLE_back, true, offset);

        // 風穴配置
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
                grid[y][x] = id + (int)pos.Dir; // 風穴だから、向きを付ける
            }
        }
    }

    void SetupGrid(RectTransform panel, int colStart, int colEnd, bool isFront)
    {
        int gridCols = colEnd - colStart;
        int gridRows = rows;

        foreach (Transform child in panel)
            Destroy(child.gameObject);

        // グリッド設定
        GridLayoutGroup gridLayout = panel.GetComponent<GridLayoutGroup>();
        if (gridLayout == null)
            gridLayout = panel.gameObject.AddComponent<GridLayoutGroup>();

        // タイルサイズ設定
        float tileWidth = panel.rect.width / gridCols;
        float tileHeight = panel.rect.height / gridRows;
        float tileSize = Mathf.Min(tileWidth, tileHeight);

        tilesize = tileSize;

        gridLayout.cellSize = new Vector2(tileSize, tileSize);
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = gridCols;
        gridLayout.spacing = Vector2.zero;
        gridLayout.padding = new RectOffset(0, 0, 0, 0);

        // ステージオブジェクト作成（仮に色づき四角出す）
        for (int y = 0; y < gridRows; y++)
        {
            Debug.Log($"Row {y} / {gridRows}");
            for (int x = 0; x < gridCols; x++)
            {
                int tileId = stageGrid[y][colStart + x];

                GameObject tile = Instantiate(tileUIPrefab, panel);
                tile.name = $"Tile_{x}_{y}";

                // 中身の「Fill」オブジェクトを探す
                Transform fill = tile.transform.Find("Fill");

                _TileData tileData = GetTileData(tileId);
                tile.tag = tileData.tag;             
              
                if (fill != null && fill.TryGetComponent<Image>(out var fillImage))
                {
                    fillImage.color = Color.white;
                    fillImage.sprite = tileData.sprite;

                    if (tile.tag == "Empty")
                        fillImage.color = Color.clear;

                    else if (tile.tag == "Void")
                    {
                        Image tileImage = tile.GetComponent<Image>();

                        fillImage.color = Color.clear;
                        // 親も透明にする
                        
                            tileImage.color = Color.clear;
                        
                    }
                }

                if (tile.tag == "Start")
                {
                    if (isFront)
                        m_FrontStartPos = tile.GetComponent<RectTransform>();
                    else
                        m_BackStartPos = tile.GetComponent<RectTransform>();
                }

                if (tile.tag == "Goal")
                {
                    if (isFront)
                        m_FrontGoalPos = tile.GetComponent<RectTransform>();
                    else
                        m_BackGoalPos = tile.GetComponent<RectTransform>();
                }
            }
        }
    }

    _TileData GetTileData(int id)
    {
        foreach (var data in tileDataArray)
        {
            //TEMP後で消す
            if (id == 6 || id == 7 /*|| id == 8*/)
            {
                id = 9;
            }

            if (data.id == id)
                return data;
        }

        // 見つけてない場合
        return new _TileData { tag = "Untagged", sprite = null };
    }
}




[System.Serializable]
public struct _TileData
{
    public int id;
    public string tag;
    public Sprite sprite;
}