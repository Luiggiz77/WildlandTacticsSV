using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UICell : MonoBehaviour
{
    public GridLayoutGroup playerGridLayout;
    public GameObject squadCellPrefab;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(WaitForEndOfFrame());


      

    }

    public IEnumerator WaitForEndOfFrame()
    {
        yield return new WaitForEndOfFrame();

        Vector2 gridSize = playerGridLayout.GetComponent<RectTransform>().rect.size;

        float cellSize = Mathf.Ceil(gridSize.x / 3);
        Debug.Log(cellSize);

        float spacing = cellSize * 0.05f;

        float gridHeight = (cellSize - spacing) * 6f;

        float difference = (gridSize.y - gridHeight) /2f;

        playerGridLayout.cellSize = new Vector2 (cellSize, cellSize);
        playerGridLayout.spacing = new Vector2 (-(float)spacing, -spacing);

        playerGridLayout.padding.top = Mathf.RoundToInt(difference);
        playerGridLayout.padding.bottom = Mathf.RoundToInt(difference);
    }



}
