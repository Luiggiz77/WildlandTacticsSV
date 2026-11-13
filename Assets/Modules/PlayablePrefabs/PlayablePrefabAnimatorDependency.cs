using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

public class PlayablePrefabAnimatorDependency : PlayablePrefab
{
    [Tooltip("Es el nombre de la propiedad del VisualEffect.")]
    public string propertieName = string.Empty;

    [Tooltip("Es la curva para pasarle los valores al efecto.")]
    public AnimationCurve curve;

    [Tooltip("vfxs.")]
    public VisualEffect[] vfxs;

    [Tooltip("Extras.")]
    public UnityEvent<float> onDelta;

    /// <summary>
    /// Id de la propiedad a realizarle el lerp.
    /// </summary>
    protected int gnPropertieId = 0;

    /// <summary>
    /// Es nuestro animator del cual dependemos para hacer las transiciones.
    /// </summary>
    private Animator goAnimator = null;

    /// <summary>
    /// Nos indica el has del cual debemos correr.
    /// </summary>
    private int gnAnimationTrigger = 0;

    /// <summary>
    /// Awake
    /// </summary>
    private void Awake()
    {
        gnPropertieId = Shader.PropertyToID(propertieName);
    }

    public override void Play(object loValue, int lnAreaSections, params BoardCell<BoardUnit>[] loCells)
    {
    }

    public override IEnumerator PlayCoroutine(object loValue, int lnAreaSections, params BoardCell<BoardUnit>[] loCells)
    {
        if (goAnimator == null) yield break;
        foreach (VisualEffect loItem in vfxs) loItem.Play();
        WaitForEndOfFrame loWaitForEndOfFrame = new WaitForEndOfFrame();
        float lnDelta;

        AnimatorStateInfo loAnimatorStateInfo;
        do
        {
            loAnimatorStateInfo = goAnimator.GetCurrentAnimatorStateInfo(0);
            lnDelta = curve.Evaluate(loAnimatorStateInfo.normalizedTime);
            foreach (VisualEffect loItem in vfxs) loItem.SetFloat(gnPropertieId, lnDelta);
            onDelta.Invoke(lnDelta);
            yield return loWaitForEndOfFrame;
        }
        while (goAnimator.IsAnimationActive(gnAnimationTrigger));

        Stop();

        yield break;
    }

    public override IEnumerator PlayCoroutineBySection(object loValue, int lnDistanceMax, int lnDistanceCurrent, BoardCell<BoardUnit>[] loCells, int lnCells)
    {
        yield break;
    }

    public override bool Ended()
    {
        return !gameObject.activeSelf;
    }

    public override void Preview()
    {
    }

    public override void SetValue(object loValue)
    {
        if (loValue is Animator) goAnimator = loValue as Animator;
        else if(loValue is int) gnAnimationTrigger = (int)loValue;
    }

    public override void Stop()
    {
        foreach (VisualEffect loItem in vfxs)
        {
            loItem.Stop();
            loItem.Reinit();
            loItem.SetFloat(gnPropertieId, 0.0f);
        }
        onDelta.Invoke(0.0f);
        goAnimator = null;
        gameObject.SetActive(false);
    }
}