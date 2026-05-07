using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIUnit : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    /// <summary>
    /// Indica la cantidad de colliders que revisamos.
    /// </summary>
    private const int gnColliders2DResult = 12;

    /// <summary>
    /// Para recibir los resultados de los colliders.
    /// </summary>
    private static Collider2D[] goColliders2DResult = new Collider2D[gnColliders2DResult];

    /// <summary>
    /// Filtro para colliders.
    /// </summary>
    private static ContactFilter2D goContactFilter2D = new ContactFilter2D();

    public Transform unitContainer;

    [Tooltip("Icono de la unidad.")]
    public RawImage unitIcon;

    [Tooltip("Nombre de la unidad.")]
    public TMP_Text unitName;

    [Tooltip("BoxCollider del icono de la unidad.")]
    public BoxCollider2D boxCollider2DIcon;

    [Tooltip("Canvas de la celda que permite sobreponerlo al hacer drag")]
    public Canvas cellCanvas;

    [Tooltip("Determina si debe mostrar el nombre de unidad o no")]
    public bool hasName = false;

    [Tooltip("Indica si debe mostrar el popup global de informaci�n de unidades")]
    public bool showUnitPopUp = true;

    [Tooltip("Indica si debe escalarse al hacer drag")]
    public bool scaleWhileDragging = false;

    [Tooltip("Referencia al audio de cuando se levanta una ficha")]
    public string pickUpUnitAudio;

    [Tooltip("Referencia al audio de cuando se suelta una ficha")]
    public string dropUnitAudio;

    /// <summary>
    /// Funcion que nos indica que debemos permitir el drag del scrollrect.
    /// </summary>
    protected Func<Vector2, bool> goAllowScrollRectDrag;

    /// <summary>
    /// Id de la unidad.
    /// </summary>
    private int gnUnitPropertiesId = 0;

    /// <summary>
    /// Para que se mueva el scrollrect pese al drag and drop.
    /// </summary>
    private ScrollRect goScrollRect;

    /// <summary>
    /// Bandera que nos indica que el drag pertenece al scrollrect.
    /// </summary>
    private bool gbDragScrollRect = false;

    /// <summary>
    /// Bandera que nos indica que debemos ignorar el click.
    /// </summary>
    private bool gbIgnoreClick = false;

    /// <summary>
    /// Referencia a las propiedades de la unidad para desregistrarnos de los eventos.
    /// </summary>
    private UnitProperties goUnitProperties;

    /// <summary>
    /// Id del mapa de distribuci�n.
    /// </summary>
    private int gnDistributionBoardId = 0;

    /// <summary>
    /// Coordenada en X del cell.
    /// </summary>
    private int gnX = 0;

    /// <summary>
    /// Coordenada en Z del cell.
    /// </summary>
    private int gnZ = 0;

    /// <summary>
    /// Establecemos el id del mapa de distribucion al que pertenecemos.
    /// </summary>
    /// <param name="lnDistributionBoardId"></param>
    public void SetDistributionBoardId(int lnDistributionBoardId)
    {
        gnDistributionBoardId = lnDistributionBoardId;
    }

    /// <summary>
    /// Establecemos sus coordenadas en el mapa de distribuci�n.
    /// </summary>
    /// <param name="lnX"></param>
    /// <param name="lnZ"></param>
    public void SetCoordinates(int lnX, int lnZ)
    {
        gnX = lnX;
        gnZ = lnZ;
    }

    /// <summary>
    /// Nos da la coordenada en X.
    /// </summary>
    public int GetX()
    {
        return gnX;
    }

    /// <summary>
    /// Nos da la coordenada en Z.
    /// </summary>
    public int GetZ()
    {
        return gnZ;
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
    /// OnDisable.
    /// </summary>
    public virtual void OnDisable()
    {
        //<< Desregistramos la unidad a los comandos.
        GameManager.RemoveListener(OnGameCommand);

        gbDragScrollRect = false;
        gbIgnoreClick = false;

        //<< Desactivamos collider del icono.
        boxCollider2DIcon.enabled = false;
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
            case GameCommand.UnitPropertiesNameChanged:
                int lnUnitPropertiesId = (int)loParams[0];
                if (goUnitProperties.Id == lnUnitPropertiesId) unitName.text = (string)loParams[1];
                break;
            default: break;
        }
    }

    /// <summary>
    /// Asingnamos Scrollrect para que se mueva el scrollrect pese al drag and drop.
    /// </summary>
    /// <param name="loScrollRect"></param>
    public void SetScrollRect(ScrollRect loScrollRect)
    {
        goScrollRect = loScrollRect;
    }

    /// <summary>
    /// Se llama para establecer la funcion que define si se debe o no usar el drag del scrollrect o del drag de la unidad.
    /// </summary>
    /// <param name="loAllowScrollRectDrag"></param>
    public void SetActionAllowingScrollRect(Func<Vector2, bool> loAllowScrollRectDrag)
    {
        goAllowScrollRectDrag = loAllowScrollRectDrag;
    }

    /// <summary>
    /// Se llama para establecer el id de la unidad a la que representa este control.
    /// NOTA IMPORTANTE: Esta funcion debe llamarse despues de asignar el tipo de cell llamando la funcion "SetCellType".
    /// </summary>
    /// <param name="lnUnitPropertiesId"></param>
    public virtual void SetUnitPropertiesId(int lnUnitPropertiesId)
    {
        gnUnitPropertiesId = lnUnitPropertiesId;
        unitIcon.gameObject.SetActive(false);
        unitName.gameObject.SetActive(false);
        unitContainer.gameObject.SetActive(false);
        boxCollider2DIcon.enabled = false;

        if (gnUnitPropertiesId <= 0) return;

        //<< Pedimos las propiedades de batalla para obtener el nombre de la unidad.
        GameManager.Send(GameCommand.GetUnitProperties, lnUnitPropertiesId, new UnityAction<UnitBattleProperties, UnitProperties>(OnGetUnitProperties));
    }

    /// <summary>
    /// Nos da el id de la unidad.
    /// </summary>
    /// <returns></returns>
    public int GetUnitId()
    {
        return gnUnitPropertiesId;
    }

    /// <summary>
    /// Nos da el id del tablero de distribucion de la unidad.
    /// </summary>
    /// <returns></returns>
    public int GetDistributionBoardId()
    {
        return gnDistributionBoardId;
    }

    /// <summary>
    /// Se llama para obtener las propiedades de la unidad a la que representamos.
    /// </summary>
    /// <param name="loUnitProperties"></param>
    private void OnGetUnitProperties(UnitBattleProperties loUnitBattleProperties, UnitProperties loUnitProperties)
    {
        //<< Almacenamos referencia a las propiedades de la unidad.
        goUnitProperties = loUnitProperties;

        //<< Asignamos el nombre de la unidad.
        if (hasName)
        {
            unitName.gameObject.SetActive(true);
            unitName.text = goUnitProperties.Name;
        }

        //<< Pedimos el icono de la unidad.
        GameManager.Send(GameCommand.GetTexture2D, new UnityAction<Texture2D>(OnGetUnitIcon), GameTexture2DUsage.UnitRoleIcon, (int)loUnitBattleProperties.Role);
        GameManager.Send(GameCommand.GetTexture2DColor, new UnityAction<Color32>(OnGetUnitColor), GameTexture2DUsage.UnitRoleIcon, (int)loUnitBattleProperties.Role);
    }

    /// <summary>
    /// Se llama para obtener el icono de nuestra unidad.
    /// </summary>
    /// <param name="loTexture2D"></param>
    private void OnGetUnitIcon(Texture2D loTexture2D)
    {
        unitIcon.gameObject.SetActive(true);
        unitContainer.gameObject.SetActive(true);
        unitIcon.texture = loTexture2D;
    }

    private void OnGetUnitColor(Color32 loColor32)
    {
        unitContainer.GetComponent<Image>().color = loColor32;
    }

    /// <summary>
    /// Se llama cuando inicia el drag.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        gbIgnoreClick = true;

        //<< Revisamos si debemos ignorar el drag.
        if (gnUnitPropertiesId == 0 || (goScrollRect != null && goAllowScrollRectDrag != null && goAllowScrollRectDrag.Invoke(eventData.delta)))
        {
            //Debug.Log("Ignora");
            goScrollRect?.OnBeginDrag(eventData);
            gbDragScrollRect = true;
            return;
        }

        if (scaleWhileDragging)
            unitContainer.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        //<< Activamos el sorting del canvas para que el �cono se muestre sobre las celdas
        cellCanvas.overrideSorting = true;
        cellCanvas.sortingOrder = 1;

        boxCollider2DIcon.enabled = true;

        //EventInstance loInstance = RuntimeManager.CreateInstance(EventReference.Find(pickUpUnitAudio));
        //loInstance.start();
        //loInstance.release();
    }

    /// <summary>
    /// Se llama cuando se está haciendo el drag.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        //<< Revisamos si debemos ignorar el drag.
        if (gnUnitPropertiesId == 0 || (goScrollRect != null && gbDragScrollRect))
        {
            goScrollRect?.OnDrag(eventData);
            return;
        }

        unitContainer.position = Input.mousePosition;
    }

    /// <summary>
    /// Se llama cuando se termina el drag.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        //<< Revisamos si debemos ignorar el drag.
        if (gnUnitPropertiesId == 0 || (goScrollRect != null && gbDragScrollRect))
        {
            goScrollRect?.OnEndDrag(eventData);
            gbDragScrollRect = false;
            return;
        }

        //EventInstance loInstance = RuntimeManager.CreateInstance(EventReference.Find(dropUnitAudio));
        //loInstance.start();
        //loInstance.release();

        Vector3 loUnitIconPosition = unitContainer.position;

        //<< Drag del icono.
        unitContainer.localPosition = Vector3.zero;
        boxCollider2DIcon.Overlap(goContactFilter2D, goColliders2DResult);

        unitContainer.localScale = Vector3.one;


        //<< Desactivamos collider del icono.
        boxCollider2DIcon.enabled = false;

        //<< Desactivamos el override del canvas y seteamos a 0 nuevamente
        cellCanvas.overrideSorting = false;
        cellCanvas.sortingOrder = 0;

        //<< Se tiene que revisar esto ya que las mascara solo es para las graficos, es decir los colliders2D aun siguen en los scrollrects.
        string lcEndsWith = "_0";
        if (goUnitProperties.Opponent) lcEndsWith = "_1";

        //<< Revisamos cual de los colliders contiene realmente la posicion.
        string lcObjectName = null;
        Collider2D loCollider2D;
        for (int i = 0; i < gnColliders2DResult; i++)
        {
            loCollider2D = goColliders2DResult[i];
            if (loCollider2D == null) continue;
            if (!loCollider2D.name.EndsWith(lcEndsWith)) continue;
            if (!loCollider2D.OverlapPoint(loUnitIconPosition)) continue;
            lcObjectName = loCollider2D.name;
            break;
        }

        //<< Si no se encontr� ningun nombre solo regresamos.
        if (lcObjectName == null)
        {
            if (gnDistributionBoardId != 0) GameManager.Send(GameCommand.RemoveUnitFromDistributionBoard, gnUnitPropertiesId, gnDistributionBoardId);
            return;
        }

        //<< Obtenemos el nombre del objeto que contiene el collider ya que sabemos que tiene el id del tablero y las coordenadas del cell.
        int[] loData = lcObjectName.ToIntArray();
        int lnDistributionBoardId = loData[0];
        int lnDistributionBoardX = loData[1];
        int lnDistributionBoardZ = loData[2];

        //<< Solo si es el mismo tablero de distribuci�n realizamos el movimiento de unidad.
        if (gnDistributionBoardId != 0 && gnDistributionBoardId != lnDistributionBoardId) return;

        //<< Pedimos que nos asignen al tablero de distribuci�n.
        GameManager.Send(GameCommand.AddUnitToDistributionBoard, gnUnitPropertiesId, lnDistributionBoardId, lnDistributionBoardX, lnDistributionBoardZ);
    }

    /// <summary>
    /// Se llama cuando le hacen click al elemento.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (gbIgnoreClick || gnUnitPropertiesId == 0)
        {
            gbIgnoreClick = false;
            return;
        }

        //<< Avisamos que deseamos ver la informaci�n de la unidad.
        GameManager.Send(GameCommand.ShowUIUnitInformation, gnUnitPropertiesId, showUnitPopUp);
    }
}
