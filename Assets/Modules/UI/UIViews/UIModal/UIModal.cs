using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UIModal : UIView
{
    [Tooltip("Texto de a mostrar en el modal.")]
    public TMP_Text text;

    [Tooltip("Texto del boton de Ok.")]
    public TMP_Text textButtonOk;

    [Tooltip("Texto del boton de Cancelar.")]
    public TMP_Text textButtonCancel;

    [Tooltip("Boton de Cancelar.")]
    public GameObject buttonCancel;

    /// <summary>
    /// Evento que se llama cuando le dan click al boton "Ok" (true) o "Cancel" (false).
    /// </summary>
    private UnityAction<bool> goOnClick = null;

    /// <summary>
    /// Awake
    /// </summary>
    protected override void Awake()
    {
        text.text = string.Empty;
        base.Awake();
    }

    /// <summary>
    /// OnDisable.
    /// </summary>
    private void OnDisable()
    {
        text.text = string.Empty;
    }

    /// <summary>
    /// Para recepcion de comandos del juego.
    /// </summary>
    /// <param name="loCommand"></param>
    /// <param name="loParams"></param>
    protected override void OnGameCommand(GameCommand loCommand, params object[] loParams)
    {
        switch (loCommand)
        {
            case GameCommand.ShowUIModal:
                text.text = (string)loParams[0];
                textButtonOk.text = (string)loParams[1];
                textButtonCancel.text = (string)loParams[2];
                buttonCancel.SetActive(!string.IsNullOrWhiteSpace(textButtonCancel.text));
                goOnClick = (UnityAction<bool>)loParams[3];
                panel.SetActive(true);
                break;
            default: break;
        }
    }

    /// <summary>
    /// Se llama cuando le dan click al boton "Ok".
    /// </summary>
    public void OnClickOk()
    {
        goOnClick?.Invoke(true);
    }

    /// <summary>
    /// Se llama cuando le dan click al boton "Cancel".
    /// </summary>
    public void OnClickCancel()
    {
        goOnClick?.Invoke(false);
    }
}
