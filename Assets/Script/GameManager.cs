using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int ind,score=0;
    private LevelLoader levelLoader;
    private LevelData levelData;
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI scoreText,levelNumberText;
    private bool levelLoaded = false;
    [SerializeField]private LevelCounter levelCounter;
    private ShooterBlockManager shooterBlockManager;
    [SerializeField] private GameObject WinninngSceneUI;
    [SerializeField] private GameObject ShooterBlockHolder;
    [SerializeField] private GameObject ColorBlockUi;
    [SerializeField] private GameObject MultipleColorBlockUi;
    [SerializeField] private GameObject RevealColorUI;
    [SerializeField] private GameObject LevelRiseHigherUI;
    private int amountScore;
    public Slider Slider
    {
        get => slider;
        set => slider = value;
    }

    void Start()
    {
       // levelCounter.level++;
        WinninngSceneUI.SetActive(false);
        shooterBlockManager=FindObjectOfType<ShooterBlockManager>();
       // levelCounter = FindObjectOfType<LevelCounter>();
        levelLoader = FindObjectOfType<LevelLoader>();
        levelData = FindObjectOfType<LevelData>();
        if (levelCounter == null)
            levelCounter = FindObjectOfType<LevelCounter>();
        LevelUpdater();
        levelNumberText.text = "Level " + levelCounter.level.ToString();
    }


    private void Update()
    {
        if (levelLoader.TotalNumberOfBlocks <= 0)
        {
            LevelFinished();
        }
       
    }
    

    void LevelFinished()
    {
        
        shooterBlockManager.OutofSpaceUi.SetActive(false);
        WinninngSceneUI.SetActive(true);

      levelLoader.colorCount.Clear();
     
      levelLoaded = true;
      Debug.Log("Level Finished");
      Invoke(nameof(LevelUpdater), 20f);
      PlayerPrefs.SetInt("CurrentLevel",levelCounter.level);
      PlayerPrefs.Save();
      StartCoroutine(SceneLoader()) ;
     
    }

    IEnumerator SceneLoader()
    {
         yield return new WaitForSeconds(8f);
         WinninngSceneUI.SetActive(false);
         levelCounter.level++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void LevelUpdater()
    {
        if (levelCounter.level == 1)
        {
            ShooterBlockHolder.SetActive(false);
            shooterBlockManager.ResetShooterTargets();
            ColorBlockUi.SetActive(true);
            levelLoader.LoadLevel(levelData, 1);
            slider.maxValue = 120;
            levelNumberText.text = "Level " + levelCounter.level;
            scoreText.text = $"Score: {levelCounter.score}";
        }

        else if (levelCounter.level == 2)
        {
            ShooterBlockHolder.SetActive(false);
            shooterBlockManager.ResetShooterTargets();
            MultipleColorBlockUi.SetActive(true);
            levelLoader.LoadLevel(levelData, 2);
            slider.maxValue = 120;
            scoreText.text = $"Score: {levelCounter.score+=10}";
            levelNumberText.text = "Level " + levelCounter.level;
        }
        else if (levelCounter.level == 3)
        {
            ShooterBlockHolder.SetActive(false);
            shooterBlockManager.ResetShooterTargets();

            levelLoader.LoadLevel(levelData, 3);
            slider.maxValue = 120;
            scoreText.text = $"Score: {levelCounter.score+=10}";
            levelNumberText.text = "Level " + levelCounter.level;
      
        }
        else if (levelCounter.level >= 4 && levelCounter.level <= 7)
        {
            shooterBlockManager.ResetShooterTargets();
            levelLoader.LoadLevel(levelData, 4);
            slider.maxValue = 120;
            scoreText.text = $"Score: {levelCounter.score+=10}";
            levelNumberText.text = "Level " + levelCounter.level;
        } 
        else if (levelCounter.level==8)
        {
            WinninngSceneUI.SetActive(false);
            shooterBlockManager.ResetShooterTargets();
            RevealColorUI.SetActive(true);
            levelLoader.LoadLevel(levelData, 4);
            slider.maxValue = 120;
            scoreText.text = $"Score: {levelCounter.score+=10}";
            levelNumberText.text = "Level " + levelCounter.level;
        }  
        else if (levelCounter.level>=9 && levelCounter.level<=10)
        {
            WinninngSceneUI.SetActive(false);
            shooterBlockManager.ResetShooterTargets();
            levelLoader.LoadLevel(levelData, 4);
            slider.maxValue = 120;
            scoreText.text = $"Score: {levelCounter.score+=10}";
            levelNumberText.text = "Level " + levelCounter.level;
        }   
        else if (levelCounter.level ==11)
        {
            WinninngSceneUI.SetActive(false);
            shooterBlockManager.ResetShooterTargets();
            LevelRiseHigherUI.SetActive(true);
            levelLoader.LoadLevel(levelData, 4);
            slider.maxValue = 120;
            scoreText.text = $"Score: {levelCounter.score+=20}";
            levelNumberText.text = "Level " + levelCounter.level;
        }
        else 
        {
            WinninngSceneUI.SetActive(false);
            shooterBlockManager.ResetShooterTargets();
            levelLoader.LoadLevel(levelData, 4);
            slider.maxValue = 120;
            scoreText.text = $"Score: {levelCounter.score+=20}";
            levelNumberText.text = "Level " + levelCounter.level;
        }
    }
}