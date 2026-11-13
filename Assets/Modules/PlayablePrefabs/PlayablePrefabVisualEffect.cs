using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayablePrefabVisualEffect : PlayablePrefab
{
    private const int PlaySkipFrames = 5;

    public bool loop = false;
    public bool lateStop = false;
    public VisualEffect[] visualEffects;


    private bool gbPlayed = false;
    private int gnJustPlayed = PlaySkipFrames;
    private bool gbLoop = false;

    /// <summary>
    /// Lista de spawns.
    /// </summary>
    protected List<VisualEffect> goSpawns = new List<VisualEffect>();

    /// <summary>
    /// Se lama cuando se habilita.
    /// </summary>
    private void OnEnable()
    {
        gbLoop = loop;
        transform.SetParent(null);
    }

    private void OnDisable()
    {
        visualEffects.SetActive(false);
        goSpawns.SetActive(false);
        goSpawns.Clear();
        gbPlayed = false;
        gnJustPlayed = PlaySkipFrames;
    }

    public override void Play(object loValue, int lnAreaSections, params BoardCell<BoardUnit>[] loCells)
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
                    loPosition.y = transform.position.y;
                    loSpawn.transform.position = loPosition;
                    loSpawn.transform.localScale = loVFX.transform.localScale;
                    goSpawns.Add(loSpawn);
                    loSpawn.Reinit();
                    loSpawn.Play();
                }
            }

            gbPlayed = true;
            gnJustPlayed = PlaySkipFrames;
        }
        else
        {
            PlayAll();
        }

    }

    protected void PlayAll()
    {
        foreach (VisualEffect loItem in visualEffects)
        {
            if (loItem.gameObject.activeSelf) loItem.Stop();
            else loItem.gameObject.SetActive(true);
            loItem.Reinit();
            loItem.Play();
        }
        gbPlayed = true;
        gnJustPlayed = PlaySkipFrames;
    }

    public override IEnumerator PlayCoroutine(object loValue, int lnAreaSections, params BoardCell<BoardUnit>[] loCells)
    {
        yield break;
    }

    private void LateUpdate()
    {
        if (!gbPlayed) return;
        if (gnJustPlayed > 1) { gnJustPlayed--; return; }
        if (gbLoop) return;
        foreach (VisualEffect loItem in visualEffects)
        {
            if (!loItem.gameObject.activeSelf) continue;
            if (loItem.HasAnySystemAwake()) return;
        }
        foreach (VisualEffect loItem in goSpawns)
        {
            if (!loItem.gameObject.activeSelf) continue;
            if (loItem.HasAnySystemAwake()) return;
        }
        gameObject.SetActive(false);
    }

    public override IEnumerator PlayCoroutineBySection(object loValue, int lnDistanceMax, int lnDistanceCurrent, BoardCell<BoardUnit>[] loCells, int lnCells)
    {
        yield break;
    }

    public override bool Ended()
    {
        return true;
    }

    public override void SetValue(object loValue)
    {
    }

    public override void Stop()
    {
        if (lateStop)
        {
            gbLoop = false;
            foreach (VisualEffect loItem in visualEffects) loItem.Stop();
            foreach (VisualEffect loItem in goSpawns) loItem.Stop();
        }
        else gameObject.SetActive(false);
    }

    /// <summary>
    /// Preview
    /// </summary>
    public override void Preview()
    {
    }
}