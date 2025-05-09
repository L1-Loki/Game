using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkTrigger : MonoBehaviour
{
    Map m;
    public GameObject targetMap;
    void Start()
    {
        m = FindObjectOfType<Map>();
    }
    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            m._currentChunk = targetMap;
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            if (m._currentChunk == targetMap)
            {
                m._currentChunk = null;
            }
        }
    }
}
