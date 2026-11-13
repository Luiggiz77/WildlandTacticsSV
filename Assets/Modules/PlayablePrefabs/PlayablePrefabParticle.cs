using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayablePrefabParticle : PlayablePrefab
{
    public bool loop = false;
    public bool lateStop = false;
    public ParticleSystem[] particles;

    private bool gbLoop = false;

    /// <summary>
    /// Lista de spawns.
    /// </summary>
    protected List<ParticleSystem> goSpawns = new List<ParticleSystem>();

    /// <summary>
    /// Se lama cuando se habilita.
    /// </summary>
    private void OnEnable()
    {
        gbLoop = loop;
        transform.SetParent(null);
    }

    public override void Play(object loValue, int lnAreaSections, params BoardCell<BoardUnit>[] loCells)
    {
        string lcValue = loValue as string;
        if (lcValue.Contains("target"))
        {
            particles.SetActive(false);

            int lnHashCode = GetHashCode();
            goSpawns.Clear();

            Vector3 loPosition;
            foreach (var loCell in loCells)
            {
                foreach (var loParticle in particles)
                {
                    ParticleSystem loSpawn = GameManager.Spawn(lnHashCode, loParticle);
                    loPosition = loCell.Vector;
                    loPosition.y = transform.position.y;
                    loSpawn.transform.position = loPosition;
                    loSpawn.transform.localScale = loParticle.transform.localScale;
                    goSpawns.Add(loSpawn);
                    loSpawn.Play();
                }
            }
        }
        else
        {
            particles.SetActive(true);
            PlayAll();
        }
    }

    protected void PlayAll()
    {
        foreach (ParticleSystem loItem in particles)
        {
            loItem.Stop();
            loItem.Play();
        }
    }

    public override IEnumerator PlayCoroutine(object loValue, int lnAreaSections, params BoardCell<BoardUnit>[] loCells)
    {
        if (loCells.Length == 0) yield break;
        particles.SetActive(true);
        particles.SetPosition(loCells[0].Vector);
        foreach (ParticleSystem loItem in particles) loItem.Play();
        yield break;
    }

    public override IEnumerator PlayCoroutineBySection(object loValue, int lnDistanceMax, int lnDistanceCurrent, BoardCell<BoardUnit>[] loCells, int lnCells)
    {
        yield return PlayCoroutine(loValue, lnDistanceMax, loCells);
    }

    public override void SetValue(object loValue)
    {
    }

    public override bool Ended()
    {
        return !gameObject.activeSelf;
    }

    public override void Stop()
    {
        if (lateStop)
        {
            gbLoop = false;
            foreach (ParticleSystem loItem in particles) loItem.Stop();
            foreach (ParticleSystem loItem in goSpawns) loItem.Stop();
        }
        else LateStop();
    }

    private void LateStop()
    {
        particles.SetActive(false);
        gameObject.SetActive(false);
        goSpawns.SetActive(false);
        goSpawns.Clear();
    }

    /// <summary>
    /// Se llama cuando la particula ha terminado.
    /// </summary>
    private void OnParticleSystemStopped()
    {
        Stop();
    }

    /// <summary>
    /// Preview
    /// </summary>
    public override void Preview()
    {
    }
}