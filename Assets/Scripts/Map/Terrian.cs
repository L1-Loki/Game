using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrian : MonoBehaviour
{
    [SerializeField] Vector2Int position;

    void Start()
    {
        GetComponentInParent<WorldScolling>().Add(gameObject, position);

        transform.position = new Vector3(-10, -10, 0);
    }

}
