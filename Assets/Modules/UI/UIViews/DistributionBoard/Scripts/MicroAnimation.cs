using UnityEngine;
using UnityEngine.Events;

public sealed class MicroAnimation
{
    /// <summary>
    /// Se llama cuando llegamos a A.
    /// </summary>
    public UnityAction OnA = null;
    /// <summary>
    /// Se llama cuando llegamos a B.
    /// </summary>
    public UnityAction OnB = null;

    /// <summary>
    /// Indica la direccion de la animacion.
    /// </summary>
    private float gnDirection = 0.0f;

    /// <summary>
    /// Delta de la animacion.
    /// </summary>
    private float gnDelta = 0.0f;

    /// <summary>
    /// Velocidad de la animacion.
    /// </summary>
    private float gnSpeed = 1.0f;

    /// <summary>
    /// Es el valor maximo al que deseamos llegar.
    /// </summary>
    private float gnMax = 1.0f;

    /// <summary>
    /// Es el valor minimo al que deseamos llegar.
    /// </summary>
    private float gnMin = 0.0f;

    /// <summary>
    /// Constructor
    /// </summary>
    public MicroAnimation()
    {
    }

    public MicroAnimation(float lnTime)
    {
        SetTime(lnTime);
    }

    public MicroAnimation(float lnSpeed, float lnDelta)
    {
        gnSpeed = lnSpeed;
        gnDelta = lnDelta;
    }

    public MicroAnimation(float lnSpeed, float lnDelta, float lnMaxDelta)
    {
        gnSpeed = lnSpeed;
        gnDelta = lnDelta;
        gnMax = lnMaxDelta;
    }

    /// <summary>
    /// Establece el tiempo de la animacion.
    /// </summary>
    /// <param name="lnTime"></param>
    public void SetTime(float lnTime)
    {
        gnSpeed = 1.0f / lnTime;
    }

    /// <summary>
    /// Establece la velocidad de la animacion.
    /// </summary>
    /// <param name="lnTime"></param>
    public void SetSpeed(float lnSpeed)
    {
        gnSpeed = lnSpeed;
    }

    /// <summary>
    /// 
    /// </summary>
    public void Update()
    {
        //<< Si no estamos animando solo regresamos.
        if (gnDirection == 0.0f) return;

        //<< Avanzamos el delta con respecto a el tiempo y velocidad.
        gnDelta += Time.deltaTime * gnSpeed * gnDirection;

        //<< Clamp Value.
        if (gnDelta > gnMax)
        {
            gnDelta = gnMax;
            gnDirection = 0.0f;
            if (OnB != null) OnB();
        }
        else if (gnDelta < gnMin)
        {
            gnDelta = gnMin;
            gnDirection = 0.0f;
            if (OnA != null) OnA();
        }
    }

    /// <summary>
    /// Detenemos la animacion.
    /// </summary>
    public void Stop()
    {
        gnDirection = 0.0f;
    }

    /// <summary>
    /// Anima la barra para que se agrande.
    /// </summary>
    public void AnimateToB()
    {
        if (gnDelta >= gnMax) { gnDirection = 0.0f; return; }
        gnDirection = 1.0f;
    }

    /// <summary>
    /// Anima la barra para que se reduzca.
    /// </summary>
    public void AnimateToA()
    {
        if (gnDelta <= gnMin) { gnDirection = 0.0f; return; }
        gnDirection = -1.0f;
    }

    /// <summary>
    /// Indica si estamos animando el elemento.
    /// </summary>
    /// <returns></returns>
    public bool IsAnimating()
    {
        return gnDirection != 0.0f;
    }

    /// <summary>
    /// Indica si estamos animando el elemento.
    /// </summary>
    /// <returns></returns>
    public bool IsNotAnimating()
    {
        return gnDirection == 0.0f;
    }

    /// <summary>
    /// Indica si se está animando hacia A ó 0.0f
    /// </summary>
    /// <returns></returns>
    public bool IsAnimatingToA()
    {
        if (gnDirection == -1.0f) return true;
        return false;
    }

    /// <summary>
    /// Indica si se está animando hacia B ó 1.0f
    /// </summary>
    /// <returns></returns>
    public bool IsAnimatingToB()
    {
        if (gnDirection == 1.0f) return true;
        return false;
    }

    /// <summary>
    /// Anima en direccion de B.
    /// </summary>
    public void SetAsAAnimateToB()
    {
        SetAsA();
        AnimateToB();
    }

    /// <summary>
    /// Anima en direccion de A.
    /// </summary>
    public void SetAsBAnimateToA()
    {
        SetAsB();
        AnimateToA();
    }

    /// <summary>
    /// Establece nuestro elemento como si en A ó 0.0f.
    /// </summary>
    public void SetAsA()
    {
        gnDelta = gnMin;
        gnDirection = 0.0f;
    }

    /// <summary>
    /// Establece nuestro elemento como si en B ó 1.0f.
    /// </summary>
    public void SetAsB()
    {
        gnDelta = gnMax;
        gnDirection = 0.0f;
    }


    /// <summary>
    /// Nos indica si estamos en estado A.
    /// </summary>
    public bool IsAtA()
    {
        if (gnDelta <= gnMin) return true;
        return false;
    }

    /// <summary>
    /// Nos indica si estamos en estado B.
    /// </summary>
    public bool IsAtB()
    {
        if (gnDelta >= gnMax) return true;
        return false;
    }

    /// <summary>
    /// Obtenemos el delta de la animacion.
    /// </summary>
    /// <returns></returns>
    public float GetDelta()
    {
        return gnDelta;
    }

    /// <summary>
    /// Obtenemos el maximo de la animacion.
    /// </summary>
    /// <returns></returns>
    public float GetMax()
    {
        return gnMax;
    }

    /// <summary>
    /// Establecemos el delta maximo
    /// </summary>
    /// <param name="lnMax"></param>
    public void SetMax(float lnMax)
    {
        gnMax = lnMax;
    }

    /// <summary>
    /// Establecemos el delta minimo
    /// </summary>
    /// <param name="lnMax"></param>
    public void SetMin(float lnMin)
    {
        gnMin = lnMin;
    }

    /// <summary>
    /// Obtenemos el tiempo total que requiere la microanimacion para completarse.
    /// </summary>
    /// <param name="lnMax"></param>
    public float GetTime()
    {
        return 1.0f / gnSpeed;
    }

    /// <summary>
    /// Anima al estado A a B ó de B a A dependiento de la direccion.
    /// </summary>
    public void AnimateToOposite()
    {
        if (gnDirection == 0)
        {
            if (gnDelta >= gnMax) AnimateToA();
            else AnimateToB();
            return;
        }

        if (gnDirection <= -1.0f) AnimateToB();
        else AnimateToA();
    }
}