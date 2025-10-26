using Script;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class PauseApperance : MonoBehaviour
{
    public static PauseApperance Instance { get; private set; }
    private InputSystem_Actions inputActions;
    public bool isPause = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // --- Additive�Ή��̃V���O���g���Ǘ� ---
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("����Pause�̃C���X�^���X�����݂��邽�߁A�Â����̂�u�������܂��B");
            Instance = this;
        }
        else
        {
            Instance = this;
        }

        inputActions = new InputSystem_Actions();//PlayerInputActions�̃C���X�^���X�𐶐�
        inputActions.PauseApperance.Apperance.performed += ctx =>//�����̏�����SMP_SceneManager�Ɉړ������悤�I
        {
            Debug.Log("PauseSceneLoad");
            SMPState.Instance.m_CurrentGameState = SMPState.GameState.Pause;//Pause��Ԃɂ���
            inputActions.Select.Disable();//PlayerInputActions�𖳌���
            SceneManager.LoadScene("PauseScene", LoadSceneMode.Additive);
            isPause = true;
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnEnable()
    {
        inputActions.Enable();//PlayerInputActions��L����
    }

    void OnDisable()
    {
        inputActions.Disable();//PlayerInputActions�𖳌���
    }
}
