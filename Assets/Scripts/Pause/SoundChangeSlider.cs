using Script;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;//UI�������ۂɕK�v
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
    private float SoundInput = 0;//���ʒ����̓��͒l

    //private bool SoundOn;   
    public  bool IsSoundChange;

    float m_FinalVolumeBGM;
    float m_FinalVolumeSE;

    private void Awake()
    {
        IsSoundChange = true;

        // --- Additive�Ή��̃V���O���g���Ǘ� ---
        if (Instance != null && Instance != this)
        {
            Instance = this;
        }
        else
        {
            Instance = this;
        }

        inputActions = new InputSystem_Actions();//PlayerInputActions�̃C���X�^���X�𐶐�
        inputActions.Sound.SoundSelect.performed += ctx =>
        {
            SoundInput = ctx.ReadValue<float>();
            if (SoundInput == 1)//��
            {

                m_SoundCount -= 1;
                m_SoundCount = Mathf.Clamp(m_SoundCount, 0, m_Sliders.Length - 1);
            }
            else if (SoundInput == -1)//��
            {
                m_SoundCount += 1;
                m_SoundCount = Mathf.Clamp(m_SoundCount, 0, m_Sliders.Length - 1);
            }
            else if (SoundInput == 2)//�E
            {
                ChangeBarPosition(1);
            }
            else if (SoundInput == 3)//��
            {
                ChangeBarPosition(-1);
            }
        };
        inputActions.Sound.Submit.performed += ctx =>
        {

        };
        inputActions.Sound.Return.performed += ctx =>
        {
            //Sound��߂�
            IsSoundChange = false;
            this.gameObject.SetActive(false);
            PauseApperance.Instance.isPause = true;

            if (Script.Pause.Instance != null)
            {
                Script.Pause.Instance.ManualEnable();
            }
            else
            {
                Debug.LogWarning("Pause.Instance �����݂��܂���BAdditive���[�h��ɏ���������Ă��邩�m�F���Ă��������B");
            }
        };
    }

    private void Start()
    {
        ////�����ݒ�
        m_Sliders[0].value = 10;//Master
        m_Sliders[1].value = 10;//BGM
        m_Sliders[2].value = 10;//SE
        //�o�[���B���p�̉摜�̃T�C�Y�ύX
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
            m_SelectImages[i].SetActive(false);//Slider��Image���Е\��
        }
        m_SelectImages[m_SoundCount].SetActive(true);//Slider��Image��\��
    }



    public void ChangeBarPosition(int Num)//Slider��position��ύX
    {
        switch (m_SoundCount)//����֐������ċ��L�����l����
        {
            case 0://Maste
                if (!m_SelectTrigger)
                {
                    //�I�𒆂�Slider�̒l��ύX
                    // // 0�`10��int�i�K���A0.0001�`1.0�Ƀ}�b�s���O�iLog�Ή��͈̔́j
                    if (m_Sliders != null && m_Sliders.Length > 0 && m_Sliders[m_SoundCount] != null)
                    {

                        m_Sliders[m_SoundCount].value += Num;
                        m_Sliders[m_SoundCount].value = Mathf.Clamp(m_Sliders[m_SoundCount].value, 0, 10);
                        // �}�X�^�[���ʂ̌v�Z
                        float masterValue = m_Sliders[m_SoundCount].value / 10f;
                        float masterDB = Mathf.Lerp(-80f, 0f, masterValue);
                        //BGM��Volume���}�X�^�[��ς���Ƃ��ɓ��I�ɕς���
                        m_FinalVolumeBGM = masterValue * m_Sliders[1].value / 10f;
                        float normalizedVolume = Mathf.Lerp(0.0001f, 1.0f, m_FinalVolumeBGM);
                        float bgmDB = Mathf.Log10(normalizedVolume) * 20f;
                        // SE��Volume���}�X�^�[��ς���Ƃ��ɓ��I�ɕς���
                        m_FinalVolumeSE = masterValue * m_Sliders[2].value / 10f;
                        float normalizedVolumeSE = Mathf.Lerp(0.0001f, 1.0f, m_FinalVolumeSE);
                        float seDB = Mathf.Log10(normalizedVolumeSE) * 20f;
                        //�Z�b�g
                        m_AudioMixer.SetFloat("MasterVolume", masterDB);
                        m_AudioMixer.SetFloat("BGMVolume", bgmDB);
                        m_AudioMixer.SetFloat("SEVolume", seDB);    
                        //�o�[���B���p�̉摜�̃T�C�Y�ύX
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
                        // �}�X�^�[���ʂ��擾
                        float masterValue = m_Sliders[0].value / 10f;
                        float bgmValue = m_Sliders[m_SoundCount].value / 10f;
                        // BGM�̉��ʂ��v�Z
                        m_FinalVolumeBGM = masterValue * bgmValue; // 0.0�`1.0
                        float normalizedVolume = Mathf.Lerp(0.0001f, 1.0f, m_FinalVolumeBGM);
                        float dB = Mathf.Log10(normalizedVolume) * 20f;
                        //�Z�b�g
                        m_AudioMixer.SetFloat("BGMVolume", dB);
                        //�o�[���B���p�̉摜�̃T�C�Y�ύX
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
                        // �I�𒆂�Slider�̒l��ύX
                        m_Sliders[m_SoundCount].value += Num;
                        // �}�X�^�[���ʂ��擾
                        float masterValue = m_Sliders[0].value / 10f;
                        float seValue = m_Sliders[m_SoundCount].value / 10f;
                        // SE�̉��ʂ��v�Z
                        m_FinalVolumeSE = masterValue * seValue; // 0.0�`1.0

                        // �ŏ��l��0.0001�ɐ������Ă���dB�ϊ�
                        float normalizedVolume = Mathf.Lerp(0.0001f, 1.0f, m_FinalVolumeSE);
                        float dB = Mathf.Log10(normalizedVolume) * 20f;

                        m_AudioMixer.SetFloat("SEVolume", dB);
                        //�o�[���B���p�̉摜�̃T�C�Y�ύX
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
    //    if (SoundOn && !IsSoundChange)//�o�[���������Ă��Ȃ��Ƃ���
    //    {
    //        return true;//true��Ԃ�
    //    }
    //    return false;//(Sound�o�[���������Ă���Ƃ���Cansel�𔽉������Ȃ�)
    //}

    void OnEnable()
    {
        inputActions.Sound.Enable();//PlayerInputActions��L����
    }

    void OnDisable()
    {
        Debug.Log("SoundChangeSlider Disable");
        inputActions.Sound.Disable();//PlayerInputActions�𖳌���
    }

}