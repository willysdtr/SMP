using Script;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;//UIを扱う際に必要
public class SoundChangeSlider : MonoBehaviour
{

    public static SoundChangeSlider Instance { get; private set; }
    private Vector2 m_ArrowPosition;
    [SerializeField] private Slider[] m_Sliders;
    [SerializeField] private GameObject[] m_maskImages;
    [SerializeField] private GameObject[] m_SelectImages;
    [SerializeField] AudioMixer m_AudioMixer;
    private bool m_SelectTrigger = false;
    private int m_SoundCount=0;
    private InputSystem_Actions inputActions;
    [SerializeField] private GameObject m_EventSystem;
    private float SoundInput = 0;//音量調整の入力値

    //private bool SoundOn;   
    public  bool IsSoundChange;

    float m_FinalVolumeBGM;
    float m_FinalVolumeSE;

    private void Awake()
    {
        IsSoundChange = true;

        // --- Additive対応のシングルトン管理 ---
        if (Instance != null && Instance != this)
        {
            Instance = this;
        }
        else
        {
            Instance = this;
        }

        inputActions = new InputSystem_Actions();//PlayerInputActionsのインスタンスを生成
        inputActions.Sound.SoundSelect.performed += ctx =>
        {
            SoundInput = ctx.ReadValue<float>();
            if (SoundInput == 1)//上
            {

                m_SoundCount -= 1;
                m_SoundCount = Mathf.Clamp(m_SoundCount, 0, m_Sliders.Length - 1);
            }
            else if (SoundInput == -1)//下
            {
                m_SoundCount += 1;
                m_SoundCount = Mathf.Clamp(m_SoundCount, 0, m_Sliders.Length - 1);
            }
            else if (SoundInput == 2)//右
            {
                ChangeBarPosition(1);
            }
            else if (SoundInput == 3)//左
            {
                ChangeBarPosition(-1);
            }
        };
        inputActions.Sound.Submit.performed += ctx =>
        {

        };
        inputActions.Sound.Return.performed += ctx =>
        {
            //Soundを戻す
            IsSoundChange = false;
            this.gameObject.SetActive(false);
            PauseApperance.Instance.isPause = true;

            if (Script.Pause.Instance != null)
            {
                Script.Pause.Instance.ManualEnable();
            }
            else
            {
                Debug.LogWarning("Pause.Instance が存在しません。Additiveロード後に初期化されているか確認してください。");
            }
        };
    }

    private void Start()
    {
        ////初期設定
        m_Sliders[0].value = 10;//Master
        m_Sliders[1].value = 10;//BGM
        m_Sliders[2].value = 10;//SE
        //バーを隠す用の画像のサイズ変更
        for (int i = 0; i < m_maskImages.Length; i++)
        {
            RectTransform rect = m_maskImages[i].GetComponent<RectTransform>();
            float hideRatio = 1f - (m_Sliders[i].value / 10);
            rect.sizeDelta = new Vector2(160 * hideRatio, rect.sizeDelta.y);
        }
    }


    void FixedUpdate()
    {
        if (m_SelectImages.Length == 0) return;
        for (int i = 0; i < m_SelectImages.Length; i++)
        {
            m_SelectImages[i].SetActive(false);//SliderのImageをひ表示
        }
        m_SelectImages[m_SoundCount].SetActive(true);//SliderのImageを表示
    }



    public void ChangeBarPosition(int Num)//Sliderのpositionを変更
    {
        switch (m_SoundCount)//これ関数化して共有化を考える
        {
            case 0://Maste
                if (!m_SelectTrigger)
                {
                    //選択中のSliderの値を変更
                    // // 0～10のint段階を、0.0001～1.0にマッピング（Log対応の範囲）
                    if (m_Sliders != null && m_Sliders.Length > 0 && m_Sliders[m_SoundCount] != null)
                    {

                        m_Sliders[m_SoundCount].value += Num;
                        m_Sliders[m_SoundCount].value = Mathf.Clamp(m_Sliders[m_SoundCount].value, 0, 10);
                        // マスター音量の計算
                        float masterValue = m_Sliders[m_SoundCount].value / 10f;
                        float masterDB = Mathf.Lerp(-80f, 0f, masterValue);
                        //BGMのVolumeもマスターを変えるときに動的に変える
                        m_FinalVolumeBGM = masterValue * m_Sliders[1].value / 10f;
                        float normalizedVolume = Mathf.Lerp(0.0001f, 1.0f, m_FinalVolumeBGM);
                        float bgmDB = Mathf.Log10(normalizedVolume) * 20f;
                        // SEのVolumeもマスターを変えるときに動的に変える
                        m_FinalVolumeSE = masterValue * m_Sliders[2].value / 10f;
                        float normalizedVolumeSE = Mathf.Lerp(0.0001f, 1.0f, m_FinalVolumeSE);
                        float seDB = Mathf.Log10(normalizedVolumeSE) * 20f;
                        //セット
                        m_AudioMixer.SetFloat("MasterVolume", masterDB);
                        m_AudioMixer.SetFloat("BGMVolume", bgmDB);
                        m_AudioMixer.SetFloat("SEVolume", seDB);    
                        //バーを隠す用の画像のサイズ変更
                        RectTransform rect = m_maskImages[m_SoundCount].GetComponent<RectTransform>();
                        float hideRatio = 1f - (m_Sliders[m_SoundCount].value / 10);
                        rect.sizeDelta= new Vector2(160 * hideRatio, rect.sizeDelta.y);
                    }
                }
                break;
            case 1://BGM
                if (!m_SelectTrigger)
                {
                    if (m_Sliders != null && m_Sliders.Length > 0 && m_Sliders[m_SoundCount] != null)
                    {
                        m_Sliders[m_SoundCount].value += Num;
                        // マスター音量を取得
                        float masterValue = m_Sliders[0].value / 10f;
                        float bgmValue = m_Sliders[m_SoundCount].value / 10f;
                        // BGMの音量を計算
                        m_FinalVolumeBGM = masterValue * bgmValue; // 0.0～1.0
                        float normalizedVolume = Mathf.Lerp(0.0001f, 1.0f, m_FinalVolumeBGM);
                        float dB = Mathf.Log10(normalizedVolume) * 20f;
                        //セット
                        m_AudioMixer.SetFloat("BGMVolume", dB);
                        //バーを隠す用の画像のサイズ変更
                        RectTransform rect = m_maskImages[m_SoundCount].GetComponent<RectTransform>();
                        float hideRatio = 1f - (m_Sliders[m_SoundCount].value / 10);
                        // Debug.Log(rect.sizeDelta.x);
                        rect.sizeDelta = new Vector2(160 * hideRatio, rect.sizeDelta.y);
                    }
                }
                break;
            case 2://SE
                if (!m_SelectTrigger)
                {
                    if (m_Sliders != null && m_Sliders.Length > 0 && m_Sliders[m_SoundCount] != null)
                    {
                        // 選択中のSliderの値を変更
                        m_Sliders[m_SoundCount].value += Num;
                        // マスター音量を取得
                        float masterValue = m_Sliders[0].value / 10f;
                        float seValue = m_Sliders[m_SoundCount].value / 10f;
                        // SEの音量を計算
                        m_FinalVolumeSE = masterValue * seValue; // 0.0～1.0

                        // 最小値を0.0001に制限してからdB変換
                        float normalizedVolume = Mathf.Lerp(0.0001f, 1.0f, m_FinalVolumeSE);
                        float dB = Mathf.Log10(normalizedVolume) * 20f;

                        m_AudioMixer.SetFloat("SEVolume", dB);
                        //バーを隠す用の画像のサイズ変更
                        RectTransform rect = m_maskImages[m_SoundCount].GetComponent<RectTransform>();
                        float hideRatio = 1f - (m_Sliders[m_SoundCount].value / 10);
                        // Debug.Log(rect.sizeDelta.x);
                        rect.sizeDelta = new Vector2(160 * hideRatio, rect.sizeDelta.y);
                    }
                }
                break;
        }
    }


    //public void SetSoundOnFlg(bool i)
    //{
    //    SoundOn = i;
    //}
    //public bool CanCanselFlg()
    //{
    //    if (SoundOn && !IsSoundChange)//バーをいじっていないときは
    //    {
    //        return true;//trueを返す
    //    }
    //    return false;//(SoundバーをいじっているときはCanselを反応させない)
    //}

    void OnEnable()
    {
        inputActions.Sound.Enable();//PlayerInputActionsを有効化
    }

    void OnDisable()
    {
        Debug.Log("SoundChangeSlider Disable");
        inputActions.Sound.Disable();//PlayerInputActionsを無効化
    }

}