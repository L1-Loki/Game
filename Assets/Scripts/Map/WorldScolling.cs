using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldScolling : MonoBehaviour
{
    [SerializeField] Transform playerTransform;
    Vector2Int Position = new Vector2Int(0, 0);
    Vector2Int GridPlayerPosition;

    [SerializeField] Vector2Int PlayerPosition;
    [SerializeField] float Size = 20f;
    GameObject[,] terrianTiles;

    [SerializeField] int terrianHorizonalCount;
    [SerializeField] int terrianVerticalCount;

    [SerializeField] int fieldOfVisionHeight = 2;
    [SerializeField] int fieldOfVisionWidth = 2;

    private void Awake()
    {
        terrianTiles = new GameObject[terrianHorizonalCount, terrianVerticalCount];
    }

    private void Update()
    {
        PlayerPosition.x = (int)(playerTransform.position.x / Size);
        PlayerPosition.y = (int)(playerTransform.position.y / Size);

        PlayerPosition.x -= playerTransform.position.x < 0 ? 1 : 0;
        PlayerPosition.y -= playerTransform.position.y < 0 ? 1 : 0;

        if (Position != PlayerPosition)
        {
            Position = PlayerPosition;

            GridPlayerPosition.x = CaculatePositionOnAxis(GridPlayerPosition.x, true);
            GridPlayerPosition.y = CaculatePositionOnAxis(GridPlayerPosition.y, false);
            UpdateOnScreen();
        }
    }

    private void UpdateOnScreen()
    {
        for (int povX = -(fieldOfVisionWidth / 2); povX <= fieldOfVisionWidth / 2; povX++)
        {
            for (int povY = -(fieldOfVisionHeight / 2); povY <= fieldOfVisionHeight / 2; povY++)
            {
                int ToUpDateX = CaculatePositionOnAxis(PlayerPosition.x + povX, true);     
                int ToUpDateY = CaculatePositionOnAxis(PlayerPosition.y + povY, false);

                Debug.Log("X" + ToUpDateX + "Y" + ToUpDateY);

                GameObject title = terrianTiles[ToUpDateX, ToUpDateY];

                if (title != null)
                {
                    title.transform.position = CaculatePosition(
                        PlayerPosition.x + povX,
                        PlayerPosition.y + povY
                        );
                }
                else
                {
                    Debug.LogWarning($"Tile at ({ToUpDateX}, {ToUpDateY}) is null.");
                }
            }
        }
    }

    private Vector3 CaculatePosition(int x, int y)
    {
        return new Vector3(x * Size, y * Size, 0f);
    }

    private int CaculatePositionOnAxis(float Value, bool horizontal)
    {
        if (horizontal)
        {
            if (Value >= 0)
            {
                Value = Value % terrianHorizonalCount;
            }
            else
            {
                Value += 1;
                Value = terrianHorizonalCount - 1 + Value % terrianHorizonalCount;
            }
        }
        else
        {
            if (Value >= 0)
            {
                Value = Value % terrianVerticalCount;
            }
            else
            {
                Value += 1;
                Value = terrianVerticalCount - 1 + Value % terrianVerticalCount;
            }
        }
        return (int)Value;
    }

    public void Add(GameObject _gameObject, Vector2Int _position)
    {
        terrianTiles[_position.x, _position.y] = _gameObject;
    }
}
