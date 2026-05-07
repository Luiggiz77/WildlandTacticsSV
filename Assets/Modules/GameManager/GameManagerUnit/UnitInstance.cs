using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnitInstance : UnitElement
{
    [Header("UNIT INSTANCE")]
    [Tooltip("Referencia al animator")]
    public Animator animator;

    [Tooltip("Referencia a donde va la cabeza.")]
    public Transform headAttach;

    [Tooltip("Se llama cuando se va a desactivar la unidad por medio de este evento.")]
    public UnityEvent OnDeactivation;

    /// <summary>
    /// Datos de la unidad.
    /// </summary>
    [HideInInspector]
    public UnitProperties UnitProperties = null;

    /// <summary>
    /// Datos de la unidad.
    /// </summary>
    [HideInInspector]
    public UnitBattleProperties UnitBattleProperties = null;

    /// <summary>
    /// Referencia a los elementos de la unidad.
    /// </summary>
    [HideInInspector]
    public List<UnitElement> UnitElements = new List<UnitElement>();

    /// <summary>
    /// Se llama en el evento de movimiento del animator.
    /// </summary>
    [HideInInspector]
    public System.Action<UnitInstance> OnUnitAnimatorMove = null;

    /// <summary>
    /// Se llama en el evento de IK del animator.
    /// </summary>
    [HideInInspector]
    public System.Action<UnitInstance, int> OnUnitAnimatorIK = null;

    /// <summary>
    /// Bandera que se activa en algun punto de la Animación.
    /// </summary>
    private int gnAnimationFlag = 0;

    /// <summary>
    /// Insica si estamos seleccionados.
    /// </summary>
    private bool gbSelected = false;

    /// <summary>
    /// Para activar o desactivar los renderers.
    /// </summary>
    /// <param name="lbEnabled"></param>
    public override void SetRendererEnable(bool lbEnabled)
    {
        base.SetRendererEnable(lbEnabled);
        foreach (UnitElement loItem in UnitElements) loItem.SetRendererEnable(lbEnabled);
    }

    /// <summary>
    /// Cuando habilitamos.
    /// </summary>
    private void OnEnable()
    {
        //<< Registramos la unidad a los comandos.
        GameManager.AddListener(OnGameCommand);
    }

    /// <summary>
    /// OnDisable
    /// </summary>
    public void OnDisable()
    {
        //<< Desregistramos la unidad a los comandos.
        GameManager.RemoveListener(OnGameCommand);

        //<< Deseleccionamos.
        SetSelect(false, Color.black);

        //<< Mostramos la unidad en caso de que esté oculta.
        SetRendererEnable(true);

        //<< Desactivamos las partes de la unidad.
        foreach (UnitElement loItem in UnitElements)
        {
            if (loItem == null) continue;
            loItem.gameObject.SetActive(false);
        }

        //<< Liberamos partes de unidad.
        UnitElements.Clear();

        //<< Removemos referencias.
        UnitProperties = null;

        gnAnimationFlag = 0;

        //<< Regresamos los materiales a su forma original.
        //CopyPropertiesFromSharedMaterial();
    }

    /// <summary>
    /// OnGameCommand
    /// </summary>
    /// <param name="loCommand"></param>
    /// <param name="loParams"></param>
    private void OnGameCommand(GameCommand loCommand, params object[] loParams)
    {
        switch (loCommand)
        {
            case GameCommand.SetSelectedUnit:
                if (gameObject.layer != GameManager.Layer.Unit) return;//<< Si el layer no es el layer correcto no debemos seleccionarnos.
                if (UnitProperties == null) return;
                bool lbOpponent = (bool)loParams[1];
                if (lbOpponent != UnitProperties.Opponent) return;
                int lnUnitId = (int)loParams[0];
                Color lnOutlineColor = (Color)loParams[2];
                SetSelect(lnUnitId == UnitProperties.Id, lnOutlineColor);
                break;
            default: break;
        }
    }

    /// <summary>
    /// Se llama para desactivar la bandera de animación.
    /// </summary>
    public void DeactivateAnimationFlag()
    {
        gnAnimationFlag--;
    }

    /// <summary>
    /// Se llama para obtener la bandera actual de animación.
    /// </summary>
    public int GetAnimationFlag()
    {
        return gnAnimationFlag;
    }

    /// <summary>
    /// Se llama para reiniciar a cero la bandera de animación.
    /// </summary>
    public void ResetAnimationFlag()
    {
        gnAnimationFlag = 0;
    }

    /// <summary>
    /// Se llama como evento en una animación para activar la bandera de animación.
    /// </summary>
    public void ActivateAnimationFlag()
    {
        gnAnimationFlag++;
    }

    /// <summary>
    /// Se llama cuando se mueve el animator.
    /// </summary>
    public void OnAnimatorMove()
    {
        if (OnUnitAnimatorMove != null) OnUnitAnimatorMove.Invoke(this);
        else transform.rotation *= animator.deltaRotation;
    }

    /// <summary>
    /// Se llama para usar el id del animator.
    /// </summary>
    /// <param name="layerIndex"></param>
    public void OnAnimatorIK(int layerIndex)
    {
        if (OnUnitAnimatorIK != null) OnUnitAnimatorIK.Invoke(this, layerIndex);
    }

    /// <summary>
    /// Para establecer la seleccion.
    /// </summary>
    /// <param name="lbSelected"></param>
    private void SetSelect(bool lbSelected, Color loColor)
    {
        //<< Si estamos en sincronia con la selección solo regresamos.
        if (lbSelected == gbSelected) return;

        //<< Asignamos la bandera de selección.
        gbSelected = lbSelected;

        if (lbSelected)
        {
            Select(loColor);
            foreach (UnitElement loItem in UnitElements) loItem.Select(loColor);
        }
        else
        {
            Deselect();
            foreach (UnitElement loItem in UnitElements) loItem.Deselect();
        }
    }

    /// <summary>
    /// Se llama para establecer el layer de nuestra unidad.
    /// </summary>
    /// <param name="lnLayer"></param>
    public override void SetLayer(int lnLayer)
    {
        base.SetLayer(lnLayer);
        foreach (UnitElement loItem in UnitElements) loItem.SetLayer(lnLayer);
    }

#if UNITY_EDITOR
    public void OnValidate()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (animator == null) return;

        if (headAttach == null) headAttach = transform.FindNearestContains("head");
    }
#endif
}