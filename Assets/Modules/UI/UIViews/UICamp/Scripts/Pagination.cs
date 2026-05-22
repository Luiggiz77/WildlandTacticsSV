using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Pagination : MonoBehaviour
{
    [Tooltip("Para llevar el control de la paginas.")]
    public ScrollRect scrollrect;

    [Tooltip("Velocidad de animación.")]
    public float animationSpeed = 7.0f;

    [Tooltip("En caso de requerir marcas de página.")]
    public GameObject[] markers;

    [Tooltip("Se llama cuando se cambia la pagina")]
    public UnityEvent onPageScroll;

    /// <summary>
    /// Para realizar la animacion del paginado.
    /// </summary>
    private MicroAnimation goMicroAnimation = new MicroAnimation();

    /// <summary>
    /// Indice de nuestra pagina actual.
    /// </summary>
    private int gnPage = 0;

    /// <summary>
    /// Posicion normalizada del scrollrect antes de hacer la animacion de paginación.
    /// </summary>
    private float gnFromNormalizedPosition = 0.0f;

    /// <summary>
    /// Posicion objetivo de nuestro scrollrect.
    /// </summary>
    private float gnToNormalizedPosition = 0.0f;

    /// <summary>
    /// Awake
    /// </summary>
    private void Awake()
    {
        UpdateMarkers();
    }

    /// <summary>
    /// Se llama cuando deseamos movernos a la siguiente pagina.
    /// </summary>
    public void NextPage()
    {
        //<< Determinamos si podemos movernos a la siguiente pagina.
        if (gnPage >= scrollrect.content.childCount - 1) return;

        gnPage++;

        onPageScroll.Invoke();

        UpdateMarkers();
        EvaluateAnimation();
    }

    /// <summary>
    /// Se llama cuando deseamos movernos a la pagina anterior.
    /// </summary>
    public void PreviousPage()
    {
        //<< Determinamos si podemos movernos a la pagina anterior.
        if (gnPage <= 0) return;

        gnPage--;

        onPageScroll.Invoke();

        UpdateMarkers();
        EvaluateAnimation();
    }

    /// <summary>
    /// Actualizamos los marcadores de paginas.
    /// </summary>
    private void UpdateMarkers()
    {
        if (markers.Length == 0) return;
        foreach (GameObject loItem in markers) loItem.SetActive(false);
        markers[gnPage].SetActive(true);
    }

    /// <summary>
    /// Evalua las posiciones del scrollrect para hacer la animación.
    /// </summary>
    public void EvaluateAnimation()
    {
        //<< Calculamos el desplazamiento de la pagina.
        gnFromNormalizedPosition = scrollrect.horizontalNormalizedPosition;

        //<< Cuenta únicamente los hijos activos para asegurar el correcto paginado
        int totalChilds = scrollrect.content.childCount;
        int activeChilds = 0;

        for (int i = 0; i < totalChilds; i++)
        {
            if (scrollrect.content.GetChild(i).gameObject.activeSelf) activeChilds++;
        }

        gnToNormalizedPosition = (1.0f / (activeChilds - 1)) * gnPage;

        //goMicroAnimation.SetSpeed(1.0f / Mathf.Abs(scrollrect.velocity.x));
        goMicroAnimation.SetSpeed(animationSpeed);

        scrollrect.velocity = Vector2.zero;

        //<< Iniciamos animación.
        goMicroAnimation.SetAsAAnimateToB();
    }

    /// <summary>
    /// Update
    /// </summary>
    private void Update()
    {
        //<< Si no estamos animando regresamos.
        if (!goMicroAnimation.IsAnimating()) return;

        //<< Actualizamos la animación.
        goMicroAnimation.Update();

        //<< Realizamos la posicion que debe tener nuestro recttransform.
        scrollrect.horizontalNormalizedPosition = Mathf.Lerp(gnFromNormalizedPosition, gnToNormalizedPosition, goMicroAnimation.GetDelta());
    }
}
