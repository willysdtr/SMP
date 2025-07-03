using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [Header("UI レファレンス")]
    public GameObject tutorialPanel;
    public Image tutorialImage;
    public StageUICanvasLoader loader;

    [Header("Stage Tutorial Images")]
    public List<StageTutorialSet> stageTutorialSets;

    private List<Sprite> currentStageTutorials;

    [Header("デバッグ用")]
    public bool alwaysShowTutorial = false; 

    private int currentIndex = 0;
    private string sceneKey;        //シーンキーでチュートリアル見たことあるかどうかを決める、全部リセットする時、PlayerPrefs.DeleteAll();

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

        //シーンキー設定
        sceneKey = "SeenTutorial_" + SceneManager.GetActiveScene().name;

        var set = stageTutorialSets.Find(x => x.stage == currentStage);

        if (set == null || set.tutorialSlides == null || set.tutorialSlides.Length == 0)
        {
            tutorialPanel.SetActive(false);
            return;
        }

        currentStageTutorials = new List<Sprite>(set.tutorialSlides);

        //まだこのシーン行ったことない場合チュートリアルパネル出す
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