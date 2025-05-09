using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    SceneController sceneController;
    public enum GameState { GamePlay, Paused, GameOver, LevelUp, Retry }
    public GameState currentState;
    public GameState previousState;

    [Header("Screen")]
    public GameObject pausePanel;
    public GameObject resultPanel;
    public GameObject LevelUpPanel;

    [Header("Current Stats Display")]
    public Text currentHealth;
    public Text currentRecoveryTime;
    public Text currentMoveSpeed;
    public Text currentMight;
    public Text currentProjectileSpeed;
    public Text currentMagnet;

    [Header("Result Screen Display")]
    public Image resultCharacterImage;
    public Text resultCharacterName;
    public Text resultLevel;
    public Text resultTimeSurvive;
    public List<Image> resultWeaponsUI = new List<Image>(6);
    public List<Image> resultPassiveItemUI = new List<Image>(6);

    [Header("Stop Watch")]
    float stopWatchTime;
    public Text stopWatchText;

    public bool isGameOver = false;
    public bool isUpgrade = false;

    public GameObject playerObject;
    public PlayerStats playerStats;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning($"Duplicate GameManager detected. Destroying {gameObject.name}.");
            Destroy(gameObject);
            return;
        }

        if (!ValidateComponents())
        {
            enabled = false;
            return;
        }

        ResetGameState();
    }

    private void Update()
    {
        switch (currentState)
        {
            case GameState.GamePlay:
                UpdateStopWatch();
                CheckForPauseAndResume();
                HideAllPanels();
                break;
            case GameState.Paused:
                CheckForPauseAndResume();
                break;
            case GameState.GameOver:
                if (!isGameOver)
                {
                    isGameOver = true;
                    GameOver();
                }
                break;
            case GameState.LevelUp:
                if (!isUpgrade)
                {
                    isUpgrade = true;
                    ShowLevelUpPanel();
                }
                break;
            case GameState.Retry:
                break;
            default:
                Debug.LogError($"Invalid game state: {currentState}");
                break;
        }
    }

    public void GameOver()
    {
        Time.timeScale = 0f;
        if (stopWatchText != null && resultTimeSurvive != null)
            resultTimeSurvive.text = "Time: " + stopWatchText.text;
        ChangeState(GameState.GameOver);
        ShowGameOverPanel();
        EnemySpawner spawner = FindObjectOfType<EnemySpawner>();
        if (spawner != null) spawner.ResetSpawner();
    }

    private void ChangeState(GameState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
            Debug.Log($"Changing state to {newState}");
            if (newState != GameState.GamePlay)
                DropRateManager.ClearAllDrops();
        }
    }

    public void PauseGame()
    {
        if (currentState != GameState.Paused)
        {
            previousState = currentState;
            ChangeState(GameState.Paused);
            Time.timeScale = 0f;
            ShowPausePanel();
        }
    }

    public void Resume()
    {
        if (currentState == GameState.Paused)
        {
            ChangeState(previousState);
            Time.timeScale = 1f;
            HidePausePanel();
        }
    }

    private void CheckForPauseAndResume()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Paused)
                Resume();
            else
                PauseGame();
        }
    }

    // UI Control Methods
    private void ShowPausePanel() => SetPanelActive(pausePanel, true);
    private void HidePausePanel() => SetPanelActive(pausePanel, false);
    private void ShowLevelUpPanel() => SetPanelActive(LevelUpPanel, true);
    private void HideLevelUpPanel() => SetPanelActive(LevelUpPanel, false);
    private void ShowGameOverPanel() => SetPanelActive(resultPanel, true);
    private void HideGameOverPanel() => SetPanelActive(resultPanel, false);

    private void SetPanelActive(GameObject panel, bool active)
    {
        if (panel != null) panel.SetActive(active);
    }

    public void RetrylMenu()
    {
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats is null! Cannot retry.");
            return;
        }

        ChangeState(GameState.Retry);
        ResetGameStateForRetry();
        playerStats.Retry();
        EnemySpawner spawner = FindObjectOfType<EnemySpawner>();
        if (spawner != null) spawner.ResetSpawner();

        StartCoroutine(CompleteRetry());
    }

    private IEnumerator CompleteRetry()
    {
        yield return null;
        HideAllPanels();
        ChangeState(GameState.GamePlay);
        Debug.Log("Game Retry completed.");
    }

    public void ReturnToMenu()
    {
        ResetGameState();
        Debug.Log("Returned to menu with state reset.");
        sceneController = GetComponent<SceneController>();
        sceneController.LoadScene("Menu");
    }

    private void ResetGameState()
    {
        stopWatchTime = 0f;
        UpdateStopWatchText();
        isGameOver = false;
        isUpgrade = false;
        Time.timeScale = 1f;
        HideAllPanels();
        ChangeState(GameState.GamePlay);
    }

    private void ResetGameStateForRetry()
    {
        stopWatchTime = 0f;
        UpdateStopWatchText();
        isGameOver = false;
        isUpgrade = false;
        Time.timeScale = 1f;
    }

    private void HideAllPanels()
    {
        HidePausePanel();
        HideGameOverPanel();
        HideLevelUpPanel();
    }

    public void AssignResultCharacterUI(CharacterScripTableObject resultCharacterData)
    {
        if (resultCharacterImage != null && resultCharacterName != null && resultCharacterData != null)
        {
            resultCharacterImage.sprite = resultCharacterData.Icon;
            resultCharacterName.text = resultCharacterData.Name;
        }
    }

    public void AssignResultLevelUI(int resultLevelData)
    {
        if (resultLevel != null)
            resultLevel.text = "Lv: " + resultLevelData;
    }

    public void AssignResultWeaponsAndPassiveItemUI(List<Image> resultWeaponData, List<Image> resultPassiveItemData)
    {
        // Kiểm tra null để tránh lỗi
        if (resultWeaponData == null || resultPassiveItemData == null)
        {
            Debug.LogWarning("Weapon or Passive Item data is null!");
            return;
        }

        if (resultWeaponsUI == null || resultPassiveItemUI == null)
        {
            Debug.LogWarning("Result UI lists are null!");
            return;
        }

        if (resultWeaponData.Count != resultWeaponsUI.Count || resultPassiveItemData.Count != resultPassiveItemUI.Count)
        {
            Debug.LogWarning("Mismatch in weapon/passive item UI list sizes!");
            return;
        }

        for (int i = 0; i < resultWeaponsUI.Count; i++)
        {
            if (resultWeaponsUI[i] != null && resultWeaponData[i] != null)
            {
                resultWeaponsUI[i].enabled = resultWeaponData[i].sprite != null;
                if (resultWeaponsUI[i].enabled) resultWeaponsUI[i].sprite = resultWeaponData[i].sprite;
            }
        }

        for (int i = 0; i < resultPassiveItemUI.Count; i++)
        {
            if (resultPassiveItemUI[i] != null && resultPassiveItemData[i] != null)
            {
                resultPassiveItemUI[i].enabled = resultPassiveItemData[i].sprite != null;
                if (resultPassiveItemUI[i].enabled) resultPassiveItemUI[i].sprite = resultPassiveItemData[i].sprite;
            }
        }
    }

    private void UpdateStopWatch()
    {
        stopWatchTime += Time.deltaTime;
        UpdateStopWatchText();
    }

    private void UpdateStopWatchText()
    {
        if (stopWatchText != null)
        {
            int minutes = Mathf.FloorToInt(stopWatchTime / 60f);
            int seconds = Mathf.FloorToInt(stopWatchTime % 60);
            stopWatchText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    public void StartLevelUP()
    {
        Time.timeScale = 0f;
        ChangeState(GameState.LevelUp);
        if (playerObject != null)
            playerObject.SendMessage("RemoveAppliedUpgrade", SendMessageOptions.DontRequireReceiver);
    }

    public void EndLevelUp()
    {
        if (currentState != GameState.LevelUp) return;
        isUpgrade = false;
        Time.timeScale = 1f;
        HideLevelUpPanel();
        ChangeState(GameState.GamePlay);
    }

    private bool ValidateComponents()
    {
        if (playerStats == null)
        {
            playerStats = FindObjectOfType<PlayerStats>();
            if (playerStats == null)
            {
                Debug.LogError("PlayerStats not found in scene!");
                return false;
            }
        }
        if (playerObject == null)
            playerObject = playerStats.gameObject;

        if (pausePanel == null || resultPanel == null || LevelUpPanel == null)
        {
            Debug.LogError("One or more UI panels (pausePanel, resultPanel, LevelUpPanel) are not assigned!");
            return false;
        }
        if (stopWatchText == null)
        {
            Debug.LogError("StopWatchText is not assigned!");
            return false;
        }
        return true;
    }
}