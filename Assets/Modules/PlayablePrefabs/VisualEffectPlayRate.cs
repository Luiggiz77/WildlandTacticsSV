using UnityEngine;
using UnityEngine.VFX;

public class VisualEffectPlayRate : MonoBehaviour
{
    public VisualEffect vfx;

    [Tooltip("Nos indica el tiempo en el que debemos aplicar el Play Rate.")]
    public float timePlayRate = 0.0f;

    [Tooltip("Nos indica el Play Rate que debemos aplicar en el tiempo dado.")]
    public float playRate = 1.0f;

    private float gnFlaggedTime = 0;

    private void OnEnable()
    {
        vfx.Reinit();
        vfx.Play();
        vfx.playRate = 1.0f;
        gnFlaggedTime = Time.time + timePlayRate;
    }

    private void Update()
    {
        if (gnFlaggedTime > Time.time) return;
        gnFlaggedTime = float.MaxValue;
        vfx.playRate = playRate;
    }

    private void LateUpdate()
    {
        if (gnFlaggedTime < float.MaxValue) return;
        if (vfx.HasAnySystemAwake()) return;
        gameObject.transform.parent.gameObject.SetActive(false);
    }
}
