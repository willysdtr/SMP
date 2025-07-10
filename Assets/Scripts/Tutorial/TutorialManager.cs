using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [Header("UI ���t�@�����X")]
    public GameObject tutorialPanel;
    public Image tutorialImage;
    public StageUICanvasLoader loader;

    [Header("Stage Tutorial Images")]
    public List<StageTutorialSet> stageTutorialSets;

    private List<Sprite> currentStageTutorials;

    [Header("�f�o�b�O�p")]
    public bool alwaysShowTutorial = false; 

    private int currentIndex = 0;
    private string sceneKey;        //�V�[���L�[�Ń`���[�g���A���������Ƃ��邩�ǂ��������߂�A�S�����Z�b�g���鎞�APlayerPrefs.DeleteAll();

    void Start()
    {
        if (loader == null)
        {
            Debug.LogError("StageUICanvasLoader reference not set!");
            tutorialPanel.SetActive(false);
            return;
        }

        StageID currentStage;
        if (!loader.useDataFromDropdown)
        {
            currentStage = (StageID)SMPState.CURRENT_STAGE;
        }
        else
        {
            currentStage = loader.stageId;
        }

        //�V�[���L�[�ݒ�
        sceneKey = "SeenTutorial_" + SceneManager.GetActiveScene().name;

        var set = stageTutorialSets.Find(x => x.stage == currentStage);

        if (set == null || set.tutorialSlides == null || set.tutorialSlides.Length == 0)
        {
            tutorialPanel.SetActive(false);
            return;
        }

        currentStageTutorials = new List<Sprite>(set.tutorialSlides);

        //�܂����̃V�[���s�������ƂȂ��ꍇ�`���[�g���A���p�l���o��
        if (alwaysShowTutorial || PlayerPrefs.GetInt(sceneKey, 0) == 0)
        {
            tutorialPanel.SetActive(true);
            currentIndex = 0;
            ShowImage();
        }
        else
        {
            tutorialPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (tutorialPanel.activeSelf && Mouse.current.leftButton.wasPressedThisFrame) 
        {
            ShowNextImage();
        }
    }

    public void ShowNextImage()
    {
        currentIndex++;
        if (currentIndex < currentStageTutorials.Count)
        {
            ShowImage();
        }
        else
        {
            EndTutorial();
        }
    }

    private void ShowImage()
    {
        tutorialImage.sprite = currentStageTutorials[currentIndex];
    }

    private void EndTutorial()
    {
        tutorialPanel.SetActive(false);

        if(!alwaysShowTutorial)
        {
            PlayerPrefs.SetInt(sceneKey, 1);
            PlayerPrefs.Save(); 
        }
    }
}

[System.Serializable]
public class StageTutorialSet
{
    public StageID stage;
    public Sprite[] tutorialSlides;
}