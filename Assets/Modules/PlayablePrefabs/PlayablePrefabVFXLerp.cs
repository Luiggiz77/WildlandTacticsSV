using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayablePrefabVFXLerp : PlayablePrefab
{
    [Tooltip("Tiempo en el que debe realizarse el spread.")]
    public float time = 1.0f;

    [Tooltip("Es el nombre de la propiedad del VisualEffect.")]
    public string propertieName = string.Empty;

    [Tooltip("vfxs.")]
    public VisualEffect[] vfxs;

    /// <summary>
    /// Id de la propiedad a realizarle el lerp.
    /// </summary>
    protected int gnPropertieId = 0;

    /// <summary>
    /// Tiempo para reproducir los elementos.
    /// </summary>
    protected float gnTime = 0.0f;

    /// <summary>
    /// Nuestra animación.
    /// </summary>
    protected MicroAnimation goMicroAnimation = new MicroAnimation();

    /// <summary>
    /// Awake
    /// </summary>
    private void Awake()
    {
        gnPropertieId = Shader.PropertyToID(propertieName);
    }

    /// <summary>
    /// OnEnable
    /// </summary>
    public void OnEnable()
    {
        gnTime = time;
        goMicroAnimation.SetTime(gnTime);
    }

    /// <summary>
    /// Reproducimos la animacion.
    /// </summary>
    /// <param name="lcValue"></param>
    public override void Play(object loValue, int lnAreaSections, params BoardCell<BoardUnit>[] loCells)
    {
    }

    /// <summary>
    /// Reproducimos la animacion en modo corrutina con objetivo.
    /// </summary>
    /// <param name="lcValue"></param>
    /// <returns></returns>
    public override IEnumerator PlayCoroutine(object loValue, int lnAreaSections, params BoardCell<BoardUnit>[] loCells)
    {
        foreach (VisualEffect loItem in vfxs) loItem.Play();
        WaitForEndOfFrame loWaitForEndOfFrame = new WaitForEndOfFrame();
        goMicroAnimation.SetTime(gnTime);
        goMicroAnimation.SetAsAAnimateToB();
        float lnDelta;
        do
        {
            lnDelta = goMicroAnimation.GetDelta();
            foreach (VisualEffect loItem in vfxs) loItem.SetFloat(gnPropertieId, lnDelta);
            yield return loWaitForEndOfFrame;
            goMicroAnimation.Update();
        }
        while (goMicroAnimation.IsAnimating());

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
        yield return PlayCoroutine(loValue, lnDistanceMax, loCells);
    }

    public override bool Ended()
    {
        if (!gameObject.activeSelf) return true;
        if (!goMicroAnimation.IsAnimating()) return true;
        return false;
    }

    /// <summary>
    /// Establece un valor. En este caso de tiempo de esparcimiento.
    /// </summary>
    /// <param name="lcValue"></param>
    public override void SetValue(object loValue)
    {
        if (!float.TryParse(loValue as string, out float lnSpreadTime)) return;
        gnTime = lnSpreadTime;
    }

    /// <summary>
    /// Indica que debemos terminar con el prefab.
    /// </summary>
    /// <param name="lcValue"></param>
    public override void Stop()
    {
        foreach (VisualEffect loItem in vfxs) loItem.Stop();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Preview
    /// </summary>
    public override void Preview()
    {
    }
}
