using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayablePrefabAreaSpread : PlayablePrefabVisualEffect
{
    [Tooltip("Tiempo en el que debe realizarse el spread.")]
    public float spreadTime = 1.0f;

    /// <summary>
    /// Tiempo para esparcir los elementos en cruz.
    /// </summary>
    private float gnSpreadTime = 0.0f;

    /// <summary>
    /// OnEnable
    /// </summary>
    public void OnEnable()
    {
        gnSpreadTime = spreadTime;
    }

    /// <summary>
    /// Reproducimos la animacion en modo corrutina con objetivo.
    /// </summary>
    /// <param name="lcValue"></param>
    /// <returns></returns>
    public override IEnumerator PlayCoroutine(object loValue, int lnAreaSections, params BoardCell<BoardUnit>[] loCells)
    {
        string lcValue = loValue as string;
        if (lcValue.Contains("target"))
        {
            visualEffects.SetActive(false);

            int lnHashCode = GetHashCode();
            goSpawns.Clear();

            Vector3 loPosition;
            foreach (var loCell in loCells)
            {
                foreach (var loVFX in visualEffects)
                {
                    VisualEffect loSpawn = GameManager.Spawn(lnHashCode, loVFX);
                    loPosition = loCell.Vector;
                    loSpawn.transform.position = loPosition;
                    goSpawns.Add(loSpawn);
                }
            }

            WaitForEndOfFrame loWaitForEndOfFrame = new WaitForEndOfFrame();
            float lnDeltaTime = gnSpreadTime;
            foreach (var loVFX in goSpawns)
            {
                lnDeltaTime += Time.deltaTime;
                if (lnDeltaTime < gnSpreadTime) continue;
                loVFX.Play();
                lnDeltaTime = 0.0f;
                yield return loWaitForEndOfFrame;
            }
        }
        else
        {
            visualEffects.SetActive(true);
            PlayAll();
        }

        yield break;
    }

    /// <summary>
    /// Reproducimos la animacion en modo corrutina con datos de efecto.
    /// </summary>
    /// <param name="lnDistanceMax"></param>
    /// <param name="lnDistanceCurrent"></param>
    /// <param name="loCells"></param>
    /// <param name="lnCells"></param>
    /// <returns></returns>
    public override IEnumerator PlayCoroutineBySection(object loValue, int lnDistanceMax, int lnDistanceCurrent, BoardCell<BoardUnit>[] loCells, int lnCells)
    {
        yield break;
    }

    /// <summary>
    /// Establece un valor. En este caso de tiempo de esparcimiento.
    /// </summary>
    /// <param name="lcValue"></param>
    public override void SetValue(object loValue)
    {
        if (!float.TryParse(loValue as string, out float lnSpreadTime)) return;
        gnSpreadTime = lnSpreadTime;
    }
}