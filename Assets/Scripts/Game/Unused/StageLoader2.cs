using StageInfo;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//StageLoader�̒J��������

public class StageUICanvasLoader2 : MonoBehaviour
{
    [Header("Stage Data")]
    public bool useDataFromDropdown;
    public StageID stageId;             //�X�e�[�WID����A�X�e�[�W�I�ׂ�

    [Header("UI References")]
    public RectTransform leftPanel;     //���̃X�e�[�W�z�u����ꏊ
    public RectTransform rightPanel;    //�E�̃X�e�[�W�z�u����ꏊ

    [Header("Tile Setup")]
    public GameObject tileUIPrefab;     //�X�e�[�W�I�u�W�F�N�g��Prefab�i������Ƒ��₷�j
    public TileData2[] tileDataArray;

    public GameObject king;
    public GameObject queen;

    PlayerController playerController;

    private Vector2 kingPos;
    private Vector2 queenPos;

    private Vector2 size; //����g���Ď��̃T�C�Y�ς���
    private Vector2 setScale = new (1, 1);

    private int rows;
    private int cols;

    private List<List<int>> stageGrid;
    public static StageData stage;

    private float blocksize = 10.0f;    // 1�u���b�N�̑傫��

    void Start()
    {
        //�`�F�b�N�}�[�N�t������AStageID����A�X�e�[�W���[�h�E�t���Ȃ��Ȃ�΃X�e�[�W�Z���N�g����X�e�[�WID��ݒ肷��
        if (!useDataFromDropdown)
        {
            stageId = (StageID)SMPState.CURRENT_STAGE;
        }


        switch (stageId)
        {
            default:
                Debug.LogWarning("���X�g�ɃX�e�[�W�������Ȃ�");
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

            case StageID.Stage1_1_Test://�e�X�g�p�X�e�[�W
                stage = Stage1_Test.Stage1_1_Test;
                break;
        }
        GenerateStageGridObjects();
        Debug.Log($"[StageUICanvasLoader2] Loaded stage After: {stageGrid.Count}");
        if (stageGrid == null || stageGrid.Count == 0)
        {
            Debug.LogWarning("�X�e�[�W�O���b�h����");
            return;
        }

        rows = stageGrid.Count;
        cols = stageGrid[0].Count;

        SetupGrid(leftPanel, 0, cols / 2);
        SetupGrid(rightPanel, cols / 2, cols);

        // �������g��RectTransform���擾
        RectTransform myRect = this.GetComponent<RectTransform>();

        //�v���C���[�z�u
        playerController = king.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.PlaceAtPosition(myRect, kingPos, size,blocksize);
        }

        playerController = queen.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.PlaceAtPosition(myRect, queenPos, size, blocksize);
        }

    }

    private void GenerateStageGridObjects()
    {
        if (stage == null || tileUIPrefab == null)
        {
            Debug.LogWarning("StageData �܂��� TilePrefab �����ݒ�ł��B");
            return;
        }

        int offset = stage.STAGE_WIDTH;
        int width = stage.STAGE_WIDTH * 2;
        int height = stage.STAGE_HEIGHT;

        //�O���b�h�ݒ�
        stageGrid = new List<List<int>>(height);
        for (int y = 0; y < height; y++)
        {
            var row = new List<int>(width);
            for (int x = 0; x < width; x++)
            {
                row.Add(0); //�S���O�ɂ���
            }
            stageGrid.Add(row);
        }

        // �X�^�[�g�z�u
        stageGrid[stage.START_POS_front.Y][stage.START_POS_front.X] = 1;
        stageGrid[stage.START_POS_back.Y][stage.START_POS_back.X + offset] = 1;

        // �S�[���z�u
        stageGrid[stage.GOAL_POS_front.Y][stage.GOAL_POS_front.X] = 2;
        stageGrid[stage.GOAL_POS_back.Y][stage.GOAL_POS_back.X + offset] = 2;

        // ���z�u
        if (!stage.SOUL_POS.IsLeft)
            stageGrid[stage.SOUL_POS.Y][stage.SOUL_POS.X] = 3;
        else
            stageGrid[stage.SOUL_POS.Y][stage.SOUL_POS.X + offset] = 3;

        // �S�z�u
        SetObjFromInt2(stageGrid, 4, stage.STEEL_front, false, offset);
        SetObjFromInt2(stageGrid, 4, stage.STEEL_back, true, offset);

        // ����z�u
        SetObjFromInt2(stageGrid, 5, stage.WRINKLE_front, false, offset);
        SetObjFromInt2(stageGrid, 5, stage.WRINKLE_back, true, offset);

        // �����z�u
        SetObjFromWind(stageGrid, 6, stage.WIND_front, false, offset);
        SetObjFromWind(stageGrid, 6, stage.WIND_back, true, offset);

        // �V�[�\�[
        SetObjFromSeeSaw(stageGrid, 10, stage.seeSaw_front, false, offset);
        SetObjFromSeeSaw(stageGrid, 10, stage.seeSaw_back, true, offset);
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
                grid[y][x] = id + (int)pos.Dir; // ����������A������t����
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

        // �O���b�h�ݒ�
        GridLayoutGroup gridLayout = panel.GetComponent<GridLayoutGroup>();
        if (gridLayout == null)
            gridLayout = panel.gameObject.AddComponent<GridLayoutGroup>();

        // �^�C���T�C�Y�ݒ�
        float tileWidth = panel.rect.width / gridCols;
        float tileHeight = panel.rect.height / gridRows;
        float tileSize = Mathf.Min(tileWidth, tileHeight);

        StringManager_Canvas myStr = this.GetComponent<StringManager_Canvas>();
        size = new(tileSize * 5.900001f, tileSize * 5.627693f);//���̒l�͍��E��Canvas��Scale
        setScale = new(setScale.x * 5.900001f, setScale.y * 5.900001f);
        myStr.SetStringSize(size, setScale);

        blocksize = tileSize * 5.627693f;
        size = new Vector2(tileSize * 4.8f, tileSize * 6);//player�p�ɒ���

        gridLayout.cellSize = new Vector2(tileSize, tileSize);
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = gridCols;
        gridLayout.spacing = Vector2.zero;
        gridLayout.padding = new RectOffset(0, 0, 0, 0);

        // �X�e�[�W�I�u�W�F�N�g�쐬�i���ɐF�Â��l�p�o���j
        for (int y = 0; y < gridRows; y++)
        {
            Debug.Log($"Row {y} / {gridRows}");
            for (int x = 0; x < gridCols; x++)
            {
                int tileId = stageGrid[y][colStart + x];
                TileData2 tileData = GetTileData(tileId);
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
                    collider.size = new Vector2(collider.size.x * setScale.x, collider.size.y * setScale.y);//���ΓI�ȃT�C�Y�ύX
                    if(tileId == 1 || tileId == 2)
                    {
                        collider.offset = new (collider.offset.x * setScale.x, collider.offset.y * setScale.y);//Start�̏ꍇ��offset�ύX���s��
                    }
                }

                tile.name = $"Tile_{x}_{y}";
                tile.tag = tileData.tag;

                Transform fill = tile.transform.Find("Fill");
                if (fill != null && fill.TryGetComponent<Image>(out var fillImage))
                {
                    fillImage.color = Color.white;
                    fillImage.sprite = tileData.sprite;
                    if (tile.tag == "Empty") fillImage.color = Color.clear;
                    else if (tile.tag == "Void")
                    {
                        Image tileImage = tile.GetComponent<Image>();
                        fillImage.color = Color.clear;
                        tileImage.color = Color.clear;
                    }
                }
            }
        }

        // ===== ���C�A�E�g�m�� =====
        LayoutRebuilder.ForceRebuildLayoutImmediate(panel);
        Debug.Log($"[After LayoutRebuilder] panel.localPos={panel.localPosition}, anchoredPos={panel.anchoredPosition}, rect={panel.rect}");

        // ===== id==1 �̃^�C�����W���擾 =====
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

    TileData2 GetTileData(int id)
    {
        foreach (var data in tileDataArray)
        {
            //TEMP��ŏ���
            if (id == 6 || id == 7 /*|| id == 8*/)
            {
                id = 9;
            }

            if (data.id == id)
                return data;
        }

        // �����ĂȂ��ꍇ
        return new TileData2 { tag = "Untagged", sprite = null };
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
}



[System.Serializable]
public struct TileData2
{
    public int id;
    public string tag;
    public Sprite sprite;
    public GameObject prefab;
}