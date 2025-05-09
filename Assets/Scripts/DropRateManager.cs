using UnityEngine;
using System.Collections.Generic;

public class DropRateManager : MonoBehaviour
{
    [System.Serializable]
    public class Drops
    {
        public string dropName;
        [Range(0f, 100f)]
        public float dropRate;
        public GameObject itemPrefab;
    }

    [Header("Drop Configuration")]
    public List<Drops> drops;
    private static List<GameObject> spawnedItems = new List<GameObject>();
    private static bool isQuitting = false;

    // Đặt cờ khi ứng dụng thoát để ngăn sinh vật phẩm.
    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

    // Sinh vật phẩm khi quái bị phá hủy, chỉ trong trạng thái GamePlay.
    private void OnDestroy()
    {
        // Thoát nếu ứng dụng đang tắt hoặc scene không hoạt động
        if (isQuitting || !gameObject.scene.isLoaded)
        {
            return;
        }

        // Chỉ sinh vật phẩm trong trạng thái GamePlay
        if (GameManager.instance != null && GameManager.instance.currentState == GameManager.GameState.GamePlay)
        {
            float randomNumber = Random.Range(0f, 100f);
            List<Drops> possibleDrops = new List<Drops>();

            foreach (Drops drop in drops)
            {
                if (drop.itemPrefab == null)
                {
                    Debug.LogWarning($"{gameObject.name}: Item prefab for '{drop.dropName}' is null!");
                    continue;
                }

                if (randomNumber <= drop.dropRate)
                {
                    possibleDrops.Add(drop);
                }
            }

            if (possibleDrops.Count > 0)
            {
                Drops selectedDrop = possibleDrops[Random.Range(0, possibleDrops.Count)];
                GameObject spawnedItem = Instantiate(selectedDrop.itemPrefab, transform.position, Quaternion.identity);
                spawnedItems.Add(spawnedItem);
                Debug.Log($"{gameObject.name}: Dropped '{selectedDrop.dropName}' at {transform.position}");
            }
        }
    }

    // Hủy tất cả vật phẩm đã rơi, được gọi khi chuyển trạng thái.
    public static void ClearAllDrops()
    {
        for (int i = spawnedItems.Count - 1; i >= 0; i--)
        {
            if (spawnedItems[i] != null)
            {
                Object.Destroy(spawnedItems[i]);
            }
            spawnedItems.RemoveAt(i);
        }
        Debug.Log("All dropped items have been cleared.");
    }

    private void Start()
    {
        if (drops == null || drops.Count == 0)
        {
            Debug.LogWarning($"{gameObject.name}: Drop list is empty or null!");
        }
        else
        {
            foreach (Drops drop in drops)
            {
                if (drop.itemPrefab == null)
                {
                    Debug.LogWarning($"{gameObject.name}: Drop '{drop.dropName}' has no prefab assigned!");
                }
                if (drop.dropRate < 0f || drop.dropRate > 100f)
                {
                    Debug.LogWarning($"{gameObject.name}: Drop '{drop.dropName}' has invalid drop rate ({drop.dropRate})!");
                }
            }
        }
    }

    private void OnDisable()
    {
        spawnedItems.RemoveAll(item => item == null);
    }
}