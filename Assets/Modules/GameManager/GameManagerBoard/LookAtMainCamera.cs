using UnityEngine;

public class LookAtMainCamera : MonoBehaviour
{
    /// <summary>
    /// LateUpdate
    /// </summary>
    public void LateUpdate()
    {
        transform.LookAt(transform.position + Camera.main.transform.forward);
    }
}
