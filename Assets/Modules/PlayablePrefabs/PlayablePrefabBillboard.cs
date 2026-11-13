using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayablePrefabBillboard : PlayablePrefab
{
    [Tooltip("Indica cuantos segundos dura el letrero de texto.")]
    public float Seconds = 3.0f;

    [Tooltip("Para interpretar el alpha.")]
    public AnimationCurve curveAlpha = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);

    [Tooltip("Para interpretar la escala en X.")]
    public AnimationCurve curveScaleX = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 1.0f);
    [Tooltip("Para interpretar la escala en Y.")]
    public AnimationCurve curveScaleY = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 1.0f);

    [Tooltip("Para interpretar el offset en Y.")]
    public AnimationCurve curveOffsetY = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 0.0f);

    [Tooltip("Para cuando se hace el play.")]
    public UnityEvent onPlay;

    [Tooltip("Para cuando se hace el stop.")]
    public UnityEvent onStop;

    [Tooltip("Es el hijo al que aplica (billboard), también debe ser el canvas.")]
    public CanvasGroup child;

    /// <summary>
    /// Para saber si ya pasó el tiempo.
    /// </summary>
    private float gnFlaggedTime = 0.0f;

    /// <summary>
    /// Nos indica cuantos segundos.
    /// </summary>
    private float gnSeconds = 0.0f;

    /// <summary>
    /// Indica si es modo corrutina o modo update.
    /// </summary>
    private PlayMode goPlayMode = PlayMode.Stopped;

    /// <summary>
    /// Para saber su posicion original del billboard.
    /// </summary>
    private Vector3 goChildLocalPosition = Vector3.zero;

    /// <summary>
    /// Start.
    /// </summary>
    private void Start()
    {
        goChildLocalPosition = child.transform.localPosition;
    }

    private enum PlayMode
    {
        Stopped = 0,
        Update = 1,
        Coroutine = 2,
    }

    private void SetTime(string lcTime)
    {
        if (float.TryParse(lcTime, out float lnTime)) SetTime(lnTime);
        else SetTime(Seconds);
    }

    private void SetTime(float lnTime)
    {
        gnSeconds = lnTime;
        gnFlaggedTime = Time.time + gnSeconds;
    }

    public override void Play(object loValue, int lnAreaSections, params BoardCell<BoardUnit>[] loCells)
    {
        SetTime(loValue as string);
        onPlay.Invoke();
        goPlayMode = PlayMode.Update;
    }

    public override IEnumerator PlayCoroutine(object loValue, int lnAreaSections, params BoardCell<BoardUnit>[] loCells)
    {
        goPlayMode = PlayMode.Coroutine;
        WaitForEndOfFrame loWaitForEndOfFrame = new WaitForEndOfFrame();
        SetTime(loValue as string);
        onPlay.Invoke();
        while (gnFlaggedTime < Time.time)
        {
            OnUpdateDelta();
            yield return loWaitForEndOfFrame;
        }
        goPlayMode = PlayMode.Stopped;
        onStop.Invoke();

        yield break;
    }

    public override IEnumerator PlayCoroutineBySection(object loValue, int lnDistanceMax, int lnDistanceCurrent, BoardCell<BoardUnit>[] loCells, int lnCells)
    {
        yield break;
    }

    public override bool Ended()
    {
        if (!gameObject.activeSelf) return true;
        if (goPlayMode == PlayMode.Stopped) return true;
        return false;
    }

    public override void Preview()
    {
        gnFlaggedTime = float.MaxValue;
        goPlayMode = PlayMode.Update;
        onPlay.Invoke();
    }

    public override void SetValue(object loValue)
    {
        if (float.TryParse(loValue as string, out float lnTime)) gnFlaggedTime = lnTime;
    }

    public override void Stop()
    {
        gnFlaggedTime = 0;
        goPlayMode = PlayMode.Stopped;
        onStop.Invoke();
    }

    private void Update()
    {
        if (goPlayMode != PlayMode.Update) return;
        OnUpdateDelta();
        if (gnFlaggedTime > Time.time) return;
        Stop();
    }

    private void OnUpdateDelta()
    {
        float lnDelta = 1.0f - ((gnFlaggedTime - Time.time) / gnSeconds);

        child.alpha = Mathf.Lerp(0.0f, 1.0f, curveAlpha.Evaluate(lnDelta));

        Vector3 loScale = Vector3.one;
        loScale.x = curveScaleX.Evaluate(lnDelta);
        loScale.y = curveScaleX.Evaluate(lnDelta);
        child.transform.localScale = loScale;

        Vector3 loOffset = Vector3.zero;
        loOffset.y = curveOffsetY.Evaluate(lnDelta);
        child.transform.localPosition = goChildLocalPosition + loOffset;
    }
}