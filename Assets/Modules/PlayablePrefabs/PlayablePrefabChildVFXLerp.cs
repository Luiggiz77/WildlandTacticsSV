using UnityEngine;
using UnityEngine.VFX;

public class PlayablePrefabChildVFXLerp : PlayablePrefabChild
{
    [Tooltip("Es el nombre de la propiedad del VisualEffect.")]
    public string propertieName = "curveLerp";

    [Tooltip("Los VFX que se usan para la electricidad.")]
    public VisualEffect[] vfxs;

    /// <summary>
    /// Id de la propiedad a modificar.
    /// </summary>
    protected int gnPropertieId = 0;

    /// <summary>
    /// Awake
    /// </summary>
    private void Awake()
    {
        gnPropertieId = Shader.PropertyToID(propertieName);
    }

    /// <summary>
    /// Se llama para actualizar el delta.
    /// </summary>
    /// <param name="lnDelta"></param>
    public override void SetDelta(float lnDelta)
    {
        foreach (VisualEffect loItem in vfxs) loItem.SetFloat(gnPropertieId, lnDelta);
    }

    /// <summary>
    /// Se llama para detener.
    /// </summary>
    public override void Stop()
    {
        foreach (VisualEffect loItem in vfxs) loItem.Stop();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Para reproducir.
    /// </summary>
    public override void Play()
    {
        foreach (VisualEffect loItem in vfxs) loItem.Play();
    }

    public override void SetPositions(Vector3 loStart, Vector3 loEnd)
    {
    }
}
