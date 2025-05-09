using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public List<WeaponsController> weaponSlot = new List<WeaponsController>(6);
    public int[] weaponLevel = new int[6];
    public List<Image> weaponUISlot = new List<Image>(6);

    public List<PassiveItems> passiveItemsSlot = new List<PassiveItems>(6);
    public int[] passiveItemsLevel = new int[6];
    public List<Image> passiveItemsUISlot = new List<Image>(6);

    [System.Serializable]
    public class WeaponUpgrade
    {
        public int weaponUpgradeIndex;
        public WeaponsSciptTableObject weaponData;
        public GameObject initialWeapon;
    }

    [System.Serializable]
    public class PassiveItemsUpgrade
    {
        public int passiveItemUpgradeIndex;
        public PassiveItemsScripTableObject passiveItemsData;
        public GameObject initialPassiveItem;
    }

    [System.Serializable]
    public class UpgradeUI
    {
        public Text upgradeName;
        public Text upgradeDescirption;
        public Image upgradeIcon;
        public Button upgradeButton;
    }

    public List<WeaponUpgrade> weaponUpgrades = new List<WeaponUpgrade>();
    public List<PassiveItemsUpgrade> passiveItemsUpgrades = new List<PassiveItemsUpgrade>();
    public List<UpgradeUI> upgradeUI = new List<UpgradeUI>();

    PlayerStats player;

    private void Awake()
    {
        // Kiểm tra các thành phần cần thiết
        player = GetComponent<PlayerStats>();
        if (player == null)
        {
            Debug.LogError("PlayerStats not found on InventoryManager!");
            enabled = false;
            return;
        }

        if (!ValidateUIComponents())
        {
            enabled = false;
            return;
        }
    }

    private void Start()
    {
        ApplyUpradeOption();
    }

    public void AddWeapon(WeaponsController weapon, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= weaponSlot.Count) return;

        weaponSlot[slotIndex] = weapon;
        weaponLevel[slotIndex] = weapon.weaponData.Level;
        weaponUISlot[slotIndex].enabled = true;
        weaponUISlot[slotIndex].sprite = weapon.weaponData.Icon;

        GameManagerEndLevelUp();
    }

    public void AddPassiveItems(PassiveItems passiveItems, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= passiveItemsSlot.Count) return;

        passiveItemsSlot[slotIndex] = passiveItems;
        passiveItemsLevel[slotIndex] = passiveItems.passiveItemsData.Level;
        passiveItemsUISlot[slotIndex].enabled = true;
        passiveItemsUISlot[slotIndex].sprite = passiveItems.passiveItemsData.Icon;

        GameManagerEndLevelUp();
    }

    public void LevelUpWeapon(int slotIndex, int upgradeIndex)
    {
        if (slotIndex < 0 || slotIndex >= weaponSlot.Count || upgradeIndex < 0 || upgradeIndex >= weaponUpgrades.Count) return;

        WeaponsController weapon = weaponSlot[slotIndex];
        if (weapon == null || !weapon.weaponData.NextLevelPrefab)
        {
            Debug.LogError("Invalid weapon or no next level prefab!");
            return;
        }

        GameObject upgradedWeapon = Instantiate(weapon.weaponData.NextLevelPrefab, transform.position, Quaternion.identity);
        upgradedWeapon.transform.SetParent(transform);
        AddWeapon(upgradedWeapon.GetComponent<WeaponsController>(), slotIndex);

        if (weapon.gameObject.scene.IsValid())
        {
            Destroy(weapon.gameObject);
        }
        weaponUpgrades[upgradeIndex].weaponData = upgradedWeapon.GetComponent<WeaponsController>().weaponData;
        weaponLevel[slotIndex] = upgradedWeapon.GetComponent<WeaponsController>().weaponData.Level;

        GameManagerEndLevelUp();
    }

    public void LevelUpPassiveItems(int slotIndex, int upgradeIndex)
    {
        if (slotIndex < 0 || slotIndex >= passiveItemsSlot.Count || upgradeIndex < 0 || upgradeIndex >= passiveItemsUpgrades.Count) return;

        PassiveItems passiveItem = passiveItemsSlot[slotIndex];
        if (passiveItem == null || !passiveItem.passiveItemsData.NextLevelPrefab)
        {
            Debug.LogError("Invalid passive item or no next level prefab!");
            return;
        }

        GameObject upgradedPassiveItem = Instantiate(passiveItem.passiveItemsData.NextLevelPrefab, transform.position, Quaternion.identity);
        upgradedPassiveItem.transform.SetParent(transform);
        AddPassiveItems(upgradedPassiveItem.GetComponent<PassiveItems>(), slotIndex);

        if (passiveItem.gameObject.scene.IsValid())
        {
            Destroy(passiveItem.gameObject);
        }
        passiveItemsUpgrades[upgradeIndex].passiveItemsData = upgradedPassiveItem.GetComponent<PassiveItems>().passiveItemsData;
        passiveItemsLevel[slotIndex] = upgradedPassiveItem.GetComponent<PassiveItems>().passiveItemsData.Level;

        GameManagerEndLevelUp();
    }

    void ApplyUpradeOption()
    {
        List<WeaponUpgrade> availableWeaponUpgrades = new List<WeaponUpgrade>(weaponUpgrades);
        List<PassiveItemsUpgrade> availablePassiveItemUpgrades = new List<PassiveItemsUpgrade>(passiveItemsUpgrades);

        foreach (UpgradeUI ui in upgradeUI)
        {
            if (availableWeaponUpgrades.Count == 0 && availablePassiveItemUpgrades.Count == 0)
            {
                DisableUpgradeUI(ui);
                continue;
            }

            int upgradeType = (availableWeaponUpgrades.Count == 0) ? 2 :
                             (availablePassiveItemUpgrades.Count == 0) ? 1 :
                             Random.Range(1, 3);

            ui.upgradeButton.onClick.RemoveAllListeners();

            if (upgradeType == 1) // Weapon upgrade
            {
                WeaponUpgrade upgrade = availableWeaponUpgrades[Random.Range(0, availableWeaponUpgrades.Count)];
                availableWeaponUpgrades.Remove(upgrade);
                ConfigureWeaponUpgradeUI(ui, upgrade);
            }
            else // Passive item upgrade
            {
                PassiveItemsUpgrade upgrade = availablePassiveItemUpgrades[Random.Range(0, availablePassiveItemUpgrades.Count)];
                availablePassiveItemUpgrades.Remove(upgrade);
                ConfigurePassiveItemUpgradeUI(ui, upgrade);
            }
        }
    }

    public void ResetToInitialState()
    {
        // Xóa và reset weapon slots
        for (int i = 0; i < weaponSlot.Count; i++)
        {
            if (weaponSlot[i] != null && weaponSlot[i].gameObject.scene.IsValid())
            {
                Destroy(weaponSlot[i].gameObject);
            }
            weaponSlot[i] = null;
            weaponLevel[i] = 0;
            weaponUISlot[i].enabled = false;
            weaponUISlot[i].sprite = null;
        }

        // Xóa và reset passive item slots
        for (int i = 0; i < passiveItemsSlot.Count; i++)
        {
            if (passiveItemsSlot[i] != null && passiveItemsSlot[i].gameObject.scene.IsValid())
            {
                Destroy(passiveItemsSlot[i].gameObject);
            }
            passiveItemsSlot[i] = null;
            passiveItemsLevel[i] = 0;
            passiveItemsUISlot[i].enabled = false;
            passiveItemsUISlot[i].sprite = null;
        }

        // Reset danh sách upgrade về trạng thái ban đầu
        ResetUpgradeData();
        RemoveUpgradeOption();
        // ApplyUpradeOption(); // Cập nhật lại UI upgrade
    }

    void RemoveUpgradeOption()
    {
        foreach (UpgradeUI ui in upgradeUI)
        {
            ui.upgradeButton.onClick.RemoveAllListeners();
            DisableUpgradeUI(ui);
        }
    }

    void GameManagerEndLevelUp()
    {
        if (GameManager.instance != null && GameManager.instance.isUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
    }

    public void RemoveAppliedUpgrade()
    {
        RemoveUpgradeOption();
        ApplyUpradeOption();
    }

    void DisableUpgradeUI(UpgradeUI ui)
    {
        if (ui.upgradeName != null && ui.upgradeName.transform.parent != null)
        {
            ui.upgradeName.transform.parent.gameObject.SetActive(false);
        }
    }

    void EnableUpgradeUI(UpgradeUI ui)
    {
        if (ui.upgradeName != null && ui.upgradeName.transform.parent != null)
        {
            ui.upgradeName.transform.parent.gameObject.SetActive(true);
        }
    }

    private bool ValidateUIComponents()
    {
        if (weaponUISlot.Count != 6 || passiveItemsUISlot.Count != 6)
        {
            Debug.LogError("WeaponUISlot or PassiveItemsUISlot size must be 6!");
            return false;
        }
        if (weaponLevel.Length != 6 || passiveItemsLevel.Length != 6)
        {
            Debug.LogError("WeaponLevel or PassiveItemsLevel size must be 6!");
            return false;
        }
        foreach (UpgradeUI ui in upgradeUI)
        {
            if (ui.upgradeName == null || ui.upgradeDescirption == null || ui.upgradeIcon == null || ui.upgradeButton == null)
            {
                Debug.LogError("UpgradeUI components missing!");
                return false;
            }
        }
        return true;
    }

    private void ConfigureWeaponUpgradeUI(UpgradeUI ui, WeaponUpgrade upgrade)
    {
        EnableUpgradeUI(ui);
        bool isNewWeapon = true;

        for (int i = 0; i < weaponSlot.Count; i++)
        {
            if (weaponSlot[i] != null && weaponSlot[i].weaponData == upgrade.weaponData)
            {
                isNewWeapon = false;
                if (upgrade.weaponData.NextLevelPrefab)
                {
                    ui.upgradeButton.onClick.AddListener(() => LevelUpWeapon(i, upgrade.weaponUpgradeIndex));
                    ui.upgradeDescirption.text = upgrade.weaponData.NextLevelPrefab.GetComponent<WeaponsController>().weaponData.Description;
                    ui.upgradeName.text = upgrade.weaponData.NextLevelPrefab.GetComponent<WeaponsController>().weaponData.Name;
                }
                else
                {
                    DisableUpgradeUI(ui);
                }
                break;
            }
        }

        if (isNewWeapon)
        {
            ui.upgradeButton.onClick.AddListener(() => player.SpawnWeapons(upgrade.initialWeapon));
            ui.upgradeDescirption.text = upgrade.weaponData.Description;
            ui.upgradeName.text = upgrade.weaponData.Name;
        }
        ui.upgradeIcon.sprite = upgrade.weaponData.Icon;
    }

    private void ConfigurePassiveItemUpgradeUI(UpgradeUI ui, PassiveItemsUpgrade upgrade)
    {
        EnableUpgradeUI(ui);
        bool isNewPassiveItem = true;

        for (int i = 0; i < passiveItemsSlot.Count; i++)
        {
            if (passiveItemsSlot[i] != null && passiveItemsSlot[i].passiveItemsData == upgrade.passiveItemsData)
            {
                isNewPassiveItem = false;
                if (upgrade.passiveItemsData.NextLevelPrefab)
                {
                    ui.upgradeButton.onClick.AddListener(() => LevelUpPassiveItems(i, upgrade.passiveItemUpgradeIndex));
                    ui.upgradeDescirption.text = upgrade.passiveItemsData.NextLevelPrefab.GetComponent<PassiveItems>().passiveItemsData.Description;
                    ui.upgradeName.text = upgrade.passiveItemsData.NextLevelPrefab.GetComponent<PassiveItems>().passiveItemsData.Name;
                }
                else
                {
                    DisableUpgradeUI(ui);
                }
                break;
            }
        }

        if (isNewPassiveItem)
        {
            ui.upgradeButton.onClick.AddListener(() => player.SpawnPassiveItems(upgrade.initialPassiveItem));
            ui.upgradeDescirption.text = upgrade.passiveItemsData.Description;
            ui.upgradeName.text = upgrade.passiveItemsData.Name;
        }
        ui.upgradeIcon.sprite = upgrade.passiveItemsData.Icon;
    }

    private void ResetUpgradeData()
    {
        foreach (WeaponUpgrade upgrade in weaponUpgrades)
        {
            upgrade.weaponData = upgrade.initialWeapon.GetComponent<WeaponsController>().weaponData;
        }
        foreach (PassiveItemsUpgrade upgrade in passiveItemsUpgrades)
        {
            upgrade.passiveItemsData = upgrade.initialPassiveItem.GetComponent<PassiveItems>().passiveItemsData;
        }
    }
}