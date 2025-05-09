using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(0)]
public class PlayerStats : MonoBehaviour
{
    CharacterScripTableObject characterData;
    EnemySpawner enemySpawner;
    public Vector3 startPosition;

    float currentHealth;
    float currentRecoveryTime;
    float currentMoveSpeed;
    float currentMight;
    float currentProjectileSpeed;
    float currentMagnet;

    #region Current Stats Properties
    public float CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            if (currentHealth != value)
            {
                currentHealth = Mathf.Clamp(value, 0, characterData.MaxHealth); // Giới hạn giá trị
                UpdateHealthUI();
            }
        }
    }
    public float CurrentRecoveryTime
    {
        get { return currentRecoveryTime; }
        set
        {
            if (currentRecoveryTime != value)
            {
                currentRecoveryTime = value;
                UpdateStatUI(GameManager.instance?.currentRecoveryTime, "RecoveryTime: ", currentRecoveryTime);
            }
        }
    }
    public float CurrentMoveSpeed
    {
        get { return currentMoveSpeed; }
        set
        {
            if (currentMoveSpeed != value)
            {
                currentMoveSpeed = value;
                UpdateStatUI(GameManager.instance?.currentMoveSpeed, "MoveSpeed: ", currentMoveSpeed);
            }
        }
    }
    public float CurrentMight
    {
        get { return currentMight; }
        set
        {
            if (currentMight != value)
            {
                currentMight = value;
                UpdateStatUI(GameManager.instance?.currentMight, "Might: ", currentMight);
            }
        }
    }
    public float CurrentProjectileSpeed
    {
        get { return currentProjectileSpeed; }
        set
        {
            if (currentProjectileSpeed != value)
            {
                currentProjectileSpeed = value;
                UpdateStatUI(GameManager.instance?.currentProjectileSpeed, "ProjectileSpeed: ", currentProjectileSpeed);
            }
        }
    }
    public float CurrentMagnet
    {
        get { return currentMagnet; }
        set
        {
            if (currentMagnet != value)
            {
                currentMagnet = value;
                UpdateStatUI(GameManager.instance?.currentMagnet, "Magnet: ", currentMagnet);
            }
        }
    }
    #endregion

    [Header("Experience/Level")]
    public int level = 1;
    public int experience = 0;
    public int experienceCap;

    [Header("I-Frames")]
    public float iFrameDuration;
    float invincibleTime;
    bool isInvincible;

    [Header("UI")]
    public Image healthBar;
    public Image expBar;
    public Text LvText;

    [System.Serializable]
    public class LevelRange
    {
        public int startLevel;
        public int endLevel;
        public int experienceCapIncrease;
    }
    public List<LevelRange> levelRanges;

    InventoryManager inventer;
    public int weaponIndex;
    public int passiveItemsIndex;

    public GameObject weaponTest;
    public GameObject firtpassiveTest, passiveTest;

    Player player;

    private void Awake()
    {
        // Tải dữ liệu nhân vật
        characterData = CharacterSelection.GetData();
        CharacterSelection.instance?.DestroySingleton();
        if (characterData == null)
        {
            Debug.LogWarning("Character data not loaded yet! Waiting for selection...");
            StartCoroutine(WaitForCharacterData());
            return;
        }
        // Kiểm tra các thành phần cần thiết
        if (!ValidateComponents())
        {
            enabled = false;
            return;
        }

        // Khởi tạo giá trị ban đầu
        currentHealth = characterData.MaxHealth;
        currentRecoveryTime = characterData.RecoveryTime;
        currentMoveSpeed = characterData.MoveSpeed;
        currentMight = characterData.Might;
        currentProjectileSpeed = characterData.ProjectTitleSpeed;
        currentMagnet = characterData.Magnet;

        startPosition = transform.position;

        // Sinh vũ khí và vật phẩm ban đầu
        SpawnWeapons(characterData.StartingWeapon);
        SpawnPassiveItems(passiveTest);
    }

    private void Start()
    {
        experienceCap = levelRanges[0].experienceCapIncrease;
        UpdateAllStatsUI();
        GameManager.instance?.AssignResultCharacterUI(characterData);
        UpdateHealth();
        UpdateExp();
        UpdateLvText();
    }

    private void Update()
    {
        if (invincibleTime > 0)
        {
            invincibleTime -= Time.deltaTime;
        }
        else if (isInvincible)
        {
            isInvincible = false;
        }
        Recover();
    }

    public void InCreaseExperiennce(int amount)
    {
        experience += amount;
        LevelUp();
        UpdateExp();
    }

    void LevelUp()
    {
        if (experience >= experienceCap)
        {
            level++;
            experience = 0;
            int experienceCapIncrease = 0;
            foreach (LevelRange levelRange in levelRanges)
            {
                if (level >= levelRange.startLevel && level <= levelRange.endLevel)
                {
                    experienceCapIncrease = levelRange.experienceCapIncrease;
                    break;
                }
            }
            experienceCap += experienceCapIncrease;
            UpdateLvText();
            GameManager.instance?.StartLevelUP();
        }
    }

    private void UpdateExp()
    {
        if (expBar != null)
        {
            expBar.fillAmount = (float)experience / experienceCap;
        }
    }

    private void UpdateLvText()
    {
        if (LvText != null)
        {
            LvText.text = "Lv " + level;
        }
    }

    public void TakeDamage(float damage)
    {
        if (!isInvincible)
        {
            invincibleTime = iFrameDuration;
            isInvincible = true;
            CurrentHealth -= damage;
            if (CurrentHealth <= 0)
            {
                Die();
            }
        }
        UpdateHealth();
    }

    void UpdateHealth()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = CurrentHealth / characterData.MaxHealth;
        }
    }

    private void Die()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.AssignResultLevelUI(level);
            GameManager.instance.AssignResultWeaponsAndPassiveItemUI(inventer.weaponUISlot, inventer.passiveItemsUISlot);
            GameManager.instance.currentState = GameManager.GameState.GameOver;
        }
    }

    public void Retry()
    {
        // Reset vị trí và trạng thái cơ bản
        transform.position = startPosition;
        currentHealth = characterData.MaxHealth;
        level = 1;
        experience = 0;
        experienceCap = levelRanges[0].experienceCapIncrease;
        invincibleTime = 0f;
        isInvincible = false;

        // Reset các hệ thống liên quan
        if (player != null)
        {
            player.ResetPlayerState();
        }
        if (inventer != null)
        {
            inventer.ResetToInitialState();
            weaponIndex = 0;
            passiveItemsIndex = 0;
            if (characterData.StartingWeapon != null)
            {
                SpawnWeapons(characterData.StartingWeapon);
            }
            if (passiveTest != null)
            {
                SpawnPassiveItems(passiveTest);
            }
        }
        if (enemySpawner != null)
        {
            enemySpawner.ResetSpawner();
        }

        // Cập nhật UI sau reset
        UpdateAllStatsUI();
        UpdateHealth();
        UpdateExp();
        UpdateLvText();
    }

    public void RestoreHealth(float amount)
    {
        if (CurrentHealth < characterData.MaxHealth)
        {
            CurrentHealth += amount;
        }
    }

    void Recover()
    {
        if (CurrentHealth < characterData.MaxHealth)
        {
            CurrentHealth += CurrentRecoveryTime * Time.deltaTime;
        }
    }

    public void SpawnWeapons(GameObject spawnWeapons)
    {
        if (weaponIndex >= inventer.weaponSlot.Count || spawnWeapons == null) return;

        GameObject spawnedWeapon = Instantiate(spawnWeapons, transform.position, Quaternion.identity);
        spawnedWeapon.transform.SetParent(transform);
        inventer.AddWeapon(spawnedWeapon.GetComponent<WeaponsController>(), weaponIndex);
        weaponIndex++;
    }

    public void SpawnPassiveItems(GameObject spawnPassItems)
    {
        if (passiveItemsIndex >= inventer.passiveItemsSlot.Count || spawnPassItems == null) return;

        GameObject spawnedItem = Instantiate(spawnPassItems, transform.position, Quaternion.identity);
        spawnedItem.transform.SetParent(transform);
        inventer.AddPassiveItems(spawnedItem.GetComponent<PassiveItems>(), passiveItemsIndex);
        passiveItemsIndex++;
    }

    private bool ValidateComponents()
    {
        inventer = GetComponent<InventoryManager>();
        player = GetComponent<Player>();
        enemySpawner = FindObjectOfType<EnemySpawner>();

        if (characterData == null)
        {
            Debug.LogError("Character data not loaded!");
            return false;
        }
        if (inventer == null)
        {
            Debug.LogError("InventoryManager not found!");
            return false;
        }
        if (player == null)
        {
            Debug.LogError("Player component not found!");
            return false;
        }
        if (enemySpawner == null)
        {
            Debug.LogError("EnemySpawner not found!");
            return false;
        }
        if (levelRanges == null || levelRanges.Count == 0)
        {
            Debug.LogError("LevelRanges not configured!");
            return false;
        }
        return true;
    }

    private void UpdateHealthUI()
    {
        UpdateStatUI(GameManager.instance?.currentHealth, "Health: ", currentHealth);
    }

    private void UpdateStatUI(Text uiText, string prefix, float value)
    {
        if (uiText != null)
        {
            uiText.text = prefix + value;
        }
    }

    private void UpdateAllStatsUI()
    {
        UpdateHealthUI();
        UpdateStatUI(GameManager.instance?.currentRecoveryTime, "RecoveryTime: ", currentRecoveryTime);
        UpdateStatUI(GameManager.instance?.currentMoveSpeed, "MoveSpeed: ", currentMoveSpeed);
        UpdateStatUI(GameManager.instance?.currentMight, "Might: ", currentMight);
        UpdateStatUI(GameManager.instance?.currentProjectileSpeed, "ProjectileSpeed: ", currentProjectileSpeed);
        UpdateStatUI(GameManager.instance?.currentMagnet, "Magnet: ", currentMagnet);
    }
    private IEnumerator WaitForCharacterData()
    {
        while (CharacterSelection.GetData() == null)
        {
            yield return null; // Chờ mỗi frame cho đến khi có dữ liệu
        }
        characterData = CharacterSelection.GetData();
        Awake(); // Gọi lại Awake() để khởi tạo
    }
}