using UnityEngine;

[CreateAssetMenu(fileName = "Sounds", menuName = "GameManager/Audio/Sounds", order = 1000)]
public class Sounds : ScriptableObject
{
    [Tooltip("Para mapear los sonidos a sus llaves 'string' de FMOD")]
    public SoundKey[] soundKeys;
}