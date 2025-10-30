using StageInfo;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//StageLoaderの改造版

public class StageUILoader : MonoBehaviour
{
    [Header("Stage Data")]
    public bool useDataFromDropdown;
    public StageID stageId;             //ステージIDから、ステージ選べる

    [Header("UI References")]
    public RectTransform leftPanel;     //左のステージ配置する場所
    public RectTransform rightPanel;    //右のステージ配置する場所

    [Header("Tile Setup")]
    public GameObject tileUIPrefab;     //ステージオブジェクトのPrefab（後もっと増やす）
    public TileData[] tileDataArray;

    public GameObject king;
    public GameObject queen;

    PlayerController playerController;

    private Vector2 kingPos;
    private Vector2 queenPos;

    private Vector2 size; //これ使って糸のサイズ変える
    private Vector2 setScale = new(1, 1);

    private int rows;
    private int cols;

    private List<List<int>> stageGrid;
    public static StageData stage;

    private float blocksize = 10.0f;    // 1ブロックの大きさ

    void Awake() // Awakeでないと糸のOffSetがおかしくなるので、Startから変更
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
            case StageID.Stage1_3:
                stage = Stage1.Stage1_3;
                break;
            case StageID.Stage1_4:
                stage = Stage1.Stage1_4;
                break;
            case StageID.Stage1_5:
                stage = Stage1.Stage1_5;
                break;

            case StageID.Stage2_1:
                stage = Stage2.Stage2_1;
                break;
            case StageID.Stage2_2:
                stage = Stage2.Stage2_2;
                break;
            case StageID.Stage2_3:
                stage = Stage2.Stage2_3;
                break; 
            case StageID.Stage2_4:
                stage = Stage2.Stage2_4;
                break;
            case StageID.Stage2_5:
                stage = Stage2.Stage2_5;
                break;

            case StageID.Stage3_1:
                stage = Stage3.Stage3_1;
                break;
            case StageID.Stage3_2:
                stage = Stage3.Stage3_2;
                break;
            case StageID.Stage3_3:
                stage = Stage3.Stage3_3;
                break;
            case StageID.Stage3_4:
                stage = Stage3.Stage3_4;
                break;
            case StageID.Stage3_5:
                stage = Stage3.Stage3_5;
                break;

            case StageID.Stage1_1_Test://テスト用ステージ
                stage = Stage1_Test.Stage1_1_Test;
                break;

            case StageID.Stage_2_Test://テスト用ステージ
                stage = Stage1_Test.Stage2_Test;
                break;
        }
        GenerateStageGridObjects();
        Debug.Log($"[StageUICanvasLoader2] Loaded stage After: {stageGrid.Count}");
        if (stageGrid == null || stageGrid.Count == 0)
        {
            Debug.LogWarning("ステージグリッドが空");
            return;
        }

        rows = stageGrid.Count;
        cols = stageGrid[0].Count;

        SetupGrid(leftPanel, 0, cols / 2);
        SetupGrid(rightPanel, cols / 2, cols);


        StringManager_Canvas myStr = GetComponent<StringManager_Canvas>();
        myStr.SetCursor(GetTopLeftTilePositionInCanvas());

    }

    private void Start()
    {

        // 自分自身のRectTransformを取得
        RectTransform myRect = this.GetComponent<RectTransform>();
        //プレイヤー配置
        playerController = king.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.PlaceAtPosition(myRect, kingPos, size, blocksize, stage.isKING_LEFT);
        }

        playerController = queen.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.PlaceAtPosition(myRect, queenPos, size, blocksize, stage.isQUEEN_LEFT);
        }
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

        // シーソー
        SetObjFromSeeSaw(stageGrid, 10, stage.seeSaw_front, false, offset);
        SetObjFromSeeSaw(stageGrid, 10, stage.seeSaw_back, true, offset);

        // バネ配置
        SetObjFromInt2(stageGrid, 12, stage.SPRING_front, false, offset);
        SetObjFromInt2(stageGrid, 12, stage.SPRING_back, true, offset);

        // ハサミ配置
        SetObjFromInt2(stageGrid, 13, stage.SCISSOR_front, false, offset);
        SetObjFromInt2(stageGrid, 13, stage.SCISSOR_back, true, offset);
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

    private void SetObjFromSeeSaw(List<List<int>> grid, int id, IReadOnlyList<StageInfo.SeeSaw> positions, bool isBack, int offset)
    {
        foreach (var pos in positions)
        {
            int x = pos.X + (isBack ? offset : 0);
            int y = pos.Y;
            if (y >= 0 && y < grid.Count && x >= 0 && x < grid[0].Count)
            {
                // encode left/right into the grid (0 = false/left, 1 = true/right)
                grid[y][x] = id + (pos.isLeftRight ? 1 : 0);
            }
        }
    }

    void SetupGrid(RectTransform panel, int colStart, int colEnd)
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

        StringManager_Canvas myStr = GetComponent<StringManager_Canvas>();
        size = new(tileSize * 5.900001f, tileSize * 5.627693f);//この値は左右のCanvasのScale
        setScale = new(setScale.x * 5.900001f, setScale.y * 5.900001f);
        myStr.SetStringSize(size, setScale);

        blocksize = tileSize * 5.627693f;
        size = new Vector2(tileSize * 4.8f, tileSize * 6);//player用に調整

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
                TileData tileData = GetTileData(tileId);
                GameObject tile;
                if (tileData.prefab == null)
                {
                    tile = Instantiate(tileUIPrefab, panel);

                }
                else
                {
                    tile = Instantiate(tileData.prefab, panel);
                    Transform fill2 = tile.transform.Find("Fill");

                    RectTransform rect = tile.GetComponent<RectTransform>();
                    BoxCollider2D collider = fill2.GetComponent<BoxCollider2D>();
                    setScale = new(tileSize / rect.sizeDelta.x, tileSize / rect.sizeDelta.y);
                    collider.size = new Vector2(collider.size.x * setScale.x, collider.size.y * setScale.y);//相対的なサイズ変更
                    collider.offset = new(collider.offset.x * setScale.x, collider.offset.y * setScale.y);//offset変更
                    fill2.localPosition = new(fill2.localPosition.x * setScale.x, fill2.localPosition.y * setScale.y);//fillの相対位置を変更

                }

                tile.name = $"Tile_{x}_{y}";
                tile.tag = tileData.tag;

                Transform fill = tile.transform.Find("Fill");
                if (fill != null)
                {
       
                    var allImages = fill.GetComponentsInChildren<Image>(includeInactive: true);

                    foreach (var img in allImages)
                    {                       
                        img.color = Color.white;
                        if (img.sprite == null && tileData.sprite != null)
                            img.sprite = tileData.sprite;

                        if (tile.tag == "Empty")
                            img.color = Color.clear;
                        else if (tile.tag == "Void")
                            img.color = Color.clear;
                    }

                    if (tile.tag == "SeesawF")
                    {
                        RectTransform fillRect = fill as RectTransform;
                        fillRect.localEulerAngles = new Vector3(0f, 180f, 390f);
                    }
                }
            }
        }

        // ===== レイアウト確定 =====
        LayoutRebuilder.ForceRebuildLayoutImmediate(panel);
        Debug.Log($"[After LayoutRebuilder] panel.localPos={panel.localPosition}, anchoredPos={panel.anchoredPosition}, rect={panel.rect}");

        // ===== id==1 のタイル座標を取得 =====
        for (int y = 0; y < gridRows; y++)
        {
            for (int x = 0; x < gridCols; x++)
            {
                string tileName = $"Tile_{x}_{y}";
                Transform tileTf = panel.Find(tileName);
                if (tileTf == null) continue;

                int tileId = stageGrid[y][colStart + x];
                if (tileId != 1) continue;

                RectTransform tileRect = tileTf as RectTransform;
                Vector2 tilePosInCanvas = GetTilePositionInCanvasLocal(tileRect, panel);

                if (colStart == 0)
                    kingPos = tilePosInCanvas;
                else
                    queenPos = tilePosInCanvas;

                Debug.Log($"[TileID=1] name={tileName}, anchoredPos={tileRect.anchoredPosition}, worldPos={tileRect.position}, king/queenPos={tilePosInCanvas}");
            }
        }



    }

    TileData GetTileData(int id)
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
        return new TileData { tag = "Untagged", sprite = null };
    }

    Vector2 GetTilePositionInCanvasLocal(RectTransform tileRect, RectTransform panel)
    {
        RectTransform canvasRect = this.GetComponent<RectTransform>();
        Canvas canvas = canvasRect.GetComponent<Canvas>();
        Camera cam = canvas != null ? canvas.worldCamera : Camera.main;

        Vector3 tileWorldPos = tileRect.TransformPoint(Vector3.zero);
        Vector2 screenPoint = cam != null
            ? (Vector2)cam.WorldToScreenPoint(tileWorldPos)
            : RectTransformUtility.WorldToScreenPoint(null, tileWorldPos);

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, cam, out localPoint);

        Debug.Log($"    [GetTilePositionInCanvasLocal] tileWorldPos={tileWorldPos}, screenPoint={screenPoint}, localPoint={localPoint}, cam={cam?.name}");

        return localPoint;
    }

    public static Vector2 GetTopLeftInCanvas(RectTransform target, RectTransform canvasRect)
    {
        // ターゲットのワールド座標での四隅を取得
        Vector3[] corners = new Vector3[4];
        target.GetWorldCorners(corners); // 左下→左上→右上→右下の順
        Vector3 worldTopLeft = corners[1];

        // Canvas基準のローカル座標に変換
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            RectTransformUtility.WorldToScreenPoint(null, worldTopLeft),
            null, // Screen Space - Overlay の場合 null
            out localPoint
        );

        return localPoint;
    }
    public Vector2 GetTopLeftTilePositionInCanvas()
    {
        // 左パネルの最初のタイルを探す（GridLayoutGroupは左上→右下で配置）
        Transform firstTile = leftPanel.childCount > 0 ? leftPanel.GetChild(0) : null;
        if (firstTile == null)
        {
            Debug.LogWarning("左パネルにタイルが存在しません。");
            return Vector2.zero;
        }

        // このスクリプトが付いているCanvasのRectTransformを取得
        RectTransform canvasRect = GetComponent<RectTransform>();

        // そのタイルの左上座標をCanvas基準に変換
        RectTransform tileRect = firstTile as RectTransform;
        Vector2 topLeftInCanvas = GetTopLeftInCanvas(tileRect, canvasRect);

        Debug.Log($"[GetTopLeftTilePositionInCanvas] leftTopInCanvas={topLeftInCanvas}");
        return topLeftInCanvas;
    }

}



[System.Serializable]
public struct TileData
{
    public int id;
    public string tag;
    public Sprite sprite;
    public GameObject prefab;
}
