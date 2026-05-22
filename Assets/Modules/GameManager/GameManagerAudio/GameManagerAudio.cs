using System.Collections.Generic;
using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    [Header("AUDIO")]
    [Tooltip("Sonidos existentes")]
    public Sounds sounds;
    //public StudioEventEmitter audioEmitter;

    //EventInstance loInstance = RuntimeManager.CreateInstance(EventReference.Find(popUpOpen));
    //loInstance.start();
    //                loInstance.release();

    /// <summary>
    /// Nuestros sonidos mapeados a los string de FMOD.
    /// </summary>
    private Dictionary<SoundType, string> goSounds = new Dictionary<SoundType, string>();

    /// <summary>
    /// Se llama en el Awake del GameManager
    /// </summary>
    void AwakeAudio()
    {
        //<< Mapeamos los sonidos.
        foreach (SoundKey loItem in sounds.soundKeys) goSounds.Add(loItem.sound, loItem.key);

        //<< Nos conectamos a los comandos.
        AddListener(OnGameCommandAudio);
    }

    /// <summary>
    /// Para recepcion de comandos del juego.
    /// </summary>
    /// <param name="loCommand"></param>
    /// <param name="loParams"></param>
    private void OnGameCommandAudio(GameCommand loCommand, params object[] loParams)
    {
        switch (loCommand)
        {
            case GameCommand.PlaySound:
                {
                    SoundType loSoundType = (SoundType)loParams[0];
                    if(!goSounds.ContainsKey(loSoundType))
                    {
                        Debug.Log($"GameManager: SoundType inexistente - {loSoundType}");
                        return;
                    }
                    string lcKey = goSounds[loSoundType];

                    //<< Reproducimos en FMOD usando la llave.
                }
                break;
            default: break;
        }
    }
}
