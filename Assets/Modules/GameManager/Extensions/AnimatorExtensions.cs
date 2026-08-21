using UnityEngine;

public static class AnimatorExtensions
{
    /// <summary>
    /// Nos indica si una animación está activa. Nota: El trigger y el nombre de la aniumación debe ser la misma. Ejemplo: Trigger = "PickItem" Animation = "PickItem"
    /// </summary>
    /// <param name="loAnimator"></param>
    /// <param name="lnAnimationTrigger">El nombre del trigger se comparte con el nombre del estado en el animator.</param>
    /// <returns></returns>
    public static bool IsAnimationActive(this Animator loAnimator, int lnAnimationTrigger)
    {
        if (loAnimator.GetBool(lnAnimationTrigger)) return true;
        if (loAnimator.GetNextAnimatorStateInfo(0).shortNameHash == lnAnimationTrigger) return true;
        if (loAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash == lnAnimationTrigger) return true;
        return false;
    }

    /// <summary>
    /// Revisa que estemos en estado de base sin triggers o pendientes.
    /// </summary>
    /// <param name="loAnimator"></param>
    /// <param name="lnDefaultShortNameHash"></param>
    /// <returns></returns>
    public static bool IsInDefaultState(this Animator loAnimator, int lnDefaultShortNameHash)
    {
        AnimatorStateInfo loAnimatorStateInfo = loAnimator.GetCurrentAnimatorStateInfo(0);

        //<< Revisamos si está en transición
        if (loAnimator.IsInTransition(0)) return false;

        //<< Revisamos si está en el estado base.
        if (loAnimatorStateInfo.shortNameHash != lnDefaultShortNameHash) return false;

        //<< Verificamos que no haya triggers activos.
        foreach (AnimatorControllerParameter loParam in loAnimator.parameters)
        {
            if (loParam.type == AnimatorControllerParameterType.Trigger && loAnimator.GetBool(loParam.name)) return false;
        }

        //<< Indicamos que estamos en estado de base.
        return true;
    }

    /// <summary>
    /// Nos indica si tiene el parametro o no.
    /// </summary>
    /// <param name="loAnimator"></param>
    /// <param name="lcParamName"></param>
    /// <returns></returns>
    public static bool HasParameter(this Animator loAnimator, string lcParamName)
    {
        foreach (AnimatorControllerParameter loParam in loAnimator.parameters)
        {
            if (loParam.name == lcParamName) return true;
        }
        return false;
    }

    /// <summary>
    /// Se usa para establecer todos los floats dentro de los parametros.
    /// </summary>
    /// <param name="loAnimator"></param>
    /// <param name="lnValue"></param>
    public static void SetFloats(this Animator loAnimator, float lnValue = 0.0f)
    {
        foreach (AnimatorControllerParameter loParam in loAnimator.parameters)
        {
            if (loParam.type != AnimatorControllerParameterType.Float) continue;
            loAnimator.SetFloat(loParam.name, lnValue);
        }
    }

    /// <summary>
    /// Se usa para copiar el estado de una animator a otro.
    /// </summary>
    /// <param name="loTarget"></param>
    /// <param name="loSource"></param>
    public static void CopyAnimatorParameters(this Animator loTarget, Animator loSource)
    {
        //<< 1 Copiar parámetros
        foreach (var loParam in loSource.parameters)
        {
            switch (loParam.type)
            {
                case AnimatorControllerParameterType.Bool:
                    loTarget.SetBool(loParam.nameHash, loSource.GetBool(loParam.nameHash));
                    break;

                case AnimatorControllerParameterType.Float:
                    loTarget.SetFloat(loParam.nameHash, loSource.GetFloat(loParam.nameHash));
                    break;

                case AnimatorControllerParameterType.Int:
                    loTarget.SetInteger(loParam.nameHash, loSource.GetInteger(loParam.nameHash));
                    break;

                case AnimatorControllerParameterType.Trigger:
                    // Los triggers NO se pueden leer directamente
                    // Deben manejarse manualmente si los usas
                    break;
            }
        }
    }

    /// <summary>
    /// Se usa para copiar el estado de una animator a otro.
    /// </summary>
    /// <param name="loTarget"></param>
    /// <param name="loSource"></param>
    public static void CopyAnimatorState(this Animator loTarget, Animator loSource)
    {
        //<< 2 Copiar estado por layer
        for (int lnLayer = 0; lnLayer < loSource.layerCount; lnLayer++)
        {
            var loStateInfo = loSource.GetCurrentAnimatorStateInfo(lnLayer);
            loTarget.Play(loStateInfo.fullPathHash, lnLayer, loStateInfo.normalizedTime);
            loTarget.SetLayerWeight(lnLayer, loSource.GetLayerWeight(lnLayer));
        }
    }

    /// <summary>
    /// Se llama para copiar la velocidad.
    /// </summary>
    /// <param name="loTarget"></param>
    /// <param name="loSource"></param>
    public static void CopyAnimatorSpeed(this Animator loTarget, Animator loSource)
    {
        loTarget.speed = loSource.speed;
    }
}
