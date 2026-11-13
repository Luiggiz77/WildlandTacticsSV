using UnityEngine;

public abstract class PlayablePrefabChild : MonoBehaviour
{
    public abstract void SetDelta(float lnDelta);
    public abstract void Play();
    public abstract void Stop();
    public abstract void SetPositions(Vector3 loStart, Vector3 loEnd);
}