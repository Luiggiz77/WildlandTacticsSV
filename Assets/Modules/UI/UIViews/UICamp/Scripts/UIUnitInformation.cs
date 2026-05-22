using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System.Collections.Generic;

public class UIUnitInformation : UIView
{
    [Tooltip("Image al cual se le asignará el icono de la unidad.")]
    public RawImage unitIcon;

    [Tooltip("AspectRatioFitter para darle el ancho adecuado al recttransform del nombre de la unidad.")]
    public AspectRatioFitter aspectRatioFitterUnitName;

    [Tooltip("Text al cual se le asignará el nombre de la unidad.")]
    public TMP_Text unitName;

    [Tooltip("Text al cual se le asignará el nombre de la clase de la unidad.")]
    public TMP_Text unitClass;

    [Tooltip("Text al cual se le asignará el nombre del ataque de la unidad.")]
    public TMP_Text unitAttackName;

    [Tooltip("Text al cual se le asignará la descripción del ataque de la unidad.")]
    public TMP_Text unitAttackDescription;

    [Tooltip("Text al cual se le asignará la descripción del movimiento de la unidad.")]
    public TMP_Text unitMovementDescription;

    [Tooltip("ScrollRect que contiene los paneles con información.")]
    public ScrollRect scrollRectInformation;

    [Tooltip("Son los paneles que contienen los datos de la unidad.")]
    public LayoutElement[] scrollRectInformationChilds;

    [Tooltip("Son los puntos de selección.")]
    public GameObject[] selectionPoints;

    [Tooltip("Prefab qeu se instanciará para mostrar los íconos y valores de las estadísticas de la unidad")]
    public UIUnitStat statPrefab;

    [Tooltip("Padre en donde se instanciarán los valores de los stats.")]
    public RectTransform statsParent;

    [Tooltip("Grid que contiene los stats.")]
    public GridLayoutGroup gridLayoutStats;

    public Sprite iconHealth;
    public Sprite iconPhysicalDamage;
    public Sprite iconMagicalDamage;
    public Sprite iconPhysicalResistance;
    public Sprite iconMagicalResistance;
    public Sprite iconMovementPoints;
    public Sprite iconAttackRange;

    public bool isPopUp = false;

    [Header("Text")]

    [Tooltip("Texto subtitulo 'movimiento'.")]
    public TMP_Text textMovement;

    /// <summary>
    /// Id de la unidad de la que deseamos ver sus datos.
    /// </summary>
    private int gnUnitPropertiesId = 0;

    private List<UIUnitStat> goStatsInstantiated = new List<UIUnitStat>();

    /// <summary>
    /// Instancia de nuestra unidad.
    /// </summary>
    private UnitInstance goUnitInstance = null;

    /// <summary>
    /// Indica el rol de la unidad.
    /// </summary>
    private UnitRole goUnitRole;

    /// <summary>
    /// Start
    /// </summary>
    private void Start()
    {
        UpdateText();
    }

    /// <summary>
    /// Se llama para actualizar los textos.
    /// </summary>
    private void UpdateText()
    {
        textMovement.text = GameManager.GetText(GameTextUsage.Word, GameTextWord.Movement);
    }

    /// <summary>
    /// Esperamos un frame para obtener medidas y establecerlas en los controles.
    /// </summary>
    /// <returns></returns>
    public IEnumerator WaitForEndOfFrame()
    {
        //<< Esperamos un frame para obtener las medidas.
        yield return new WaitForEndOfFrame();

        //<< Calculamos el aspecttratiofitter para el nombre de la unidad.
        //RectTransform loRectTransformParentUnitName = aspectRatioFitterUnitName.transform.parent as RectTransform;
        //aspectRatioFitterUnitName.aspectRatio = (loRectTransformParentUnitName.rect.width / loRectTransformParentUnitName.rect.height) * 0.5f;

        //<< Obtenemos las dimensiones del recttransform para pasarselas a sus hijos.
        float lnWidth = (scrollRectInformation.transform as RectTransform).sizeDelta.x;

        //<< Establecemos el ancho que deben tener los hijos con respecto al padre que es el scrollrect.
        foreach (LayoutElement loItem in scrollRectInformationChilds) loItem.minWidth = lnWidth;



        Vector2 gridSize = (statsParent.transform.parent as RectTransform).sizeDelta;

        float cellWidth = (gridSize.x * .97f) / 4;
        float cellHeight = gridSize.y / 2;

        gridLayoutStats.cellSize = new Vector2(cellWidth, cellHeight);
    }

    /// <summary>
    /// OnGameCommand
    /// </summary>
    /// <param name="loCommand"></param>
    /// <param name="loParams"></param>
    protected override void OnGameCommand(GameCommand loCommand, params object[] loParams)
    {
        switch (loCommand)
        {
            case GameCommand.LanguageChanged: UpdateText(); break;
            case GameCommand.ShowUIUnitInformation:
                Deactivate();

                if ((bool)loParams[1] != isPopUp) return;

                //<< Si el modo de aparición es por medio de un pop up, entonces debe enviar el popUp al final para siempre tener prioridad de vista
                if (isPopUp)
                {
                    transform.SetAsLastSibling();
                    GameManager.Send(GameCommand.PlaySound, SoundType.PopUpOpen);
                }

                panel.SetActive(true);

                //<< Pedimos activar la camara del loop de ataque.
                GameManager.Send(GameCommand.SetActiveCameraAttackLoop, true);

                //<< Si es nuestra primera ves sabemos que tenemos que calcular algunas cosas de la interfaz.
                if (gnUnitPropertiesId == 0) StartCoroutine(WaitForEndOfFrame());

                //<< Obtenemos el id de la unidad para hacer las peticiones de datos.
                gnUnitPropertiesId = (int)loParams[0];

                //<< Pedimos el icono de la unidad.
                GameManager.Send(GameCommand.GetTexture2D, new UnityAction<Texture2D>(OnGetUnitIcon), GameTexture2DUsage.UnitRoleIcon, (int)goUnitRole);

                //<< Pedimos los datos de la unidad.
                GameManager.Send(GameCommand.GetUnitProperties, gnUnitPropertiesId, new UnityAction<UnitBattleProperties, UnitProperties>(OnGetUnitProperties));

                //<< Pedimos una instancia de la unidad.
                GameManager.Send(GameCommand.CreateUnitInstance, gnUnitPropertiesId, new UnityAction<UnitInstance>(OnGetUnitInstance));
                break;
            default: break;
        }
    }

    /// <summary>
    /// Recibimos el icono de la unidad.
    /// </summary>
    /// <param name="loSprite"></param>
    private void OnGetUnitIcon(Texture2D loTexture2D)
    {
        unitIcon.texture = loTexture2D;
    }

    /// <summary>
    /// Pedimos las propiedades de la unidad.
    /// </summary>
    /// <param name="loUnitProperties"></param>
    private void OnGetUnitProperties(UnitBattleProperties loUnitBattleProperties, UnitProperties loUnitProperties)
    {
        goUnitRole = loUnitBattleProperties.Role;

        //<< Asignamos nombre de unidad.
        unitName.text = loUnitProperties.Name;

        //<< Actualizamos los datos de batalla de la unidad.
        UpdateUnitBattleProperties(loUnitBattleProperties);

        //<< Asignamos la descripcion de ataque de la unidad.
        unitClass.text = GameManager.GetText(GameTextUsage.UnitRole, loUnitBattleProperties.Role);
        unitAttackName.text = GameManager.GetText(GameTextUsage.UnitAttackName, loUnitBattleProperties.Id);
        unitAttackDescription.text = GameManager.GetText(GameTextUsage.UnitAttackDescription, loUnitBattleProperties.Id);
        unitMovementDescription.text = GameManager.GetText(GameTextUsage.UnitMovementDescription, loUnitBattleProperties.Id);
    }

    /// <summary>
    /// Pedimos una instancia de la unidad.
    /// </summary>
    /// <param name="loUnitInstance"></param>
    private void OnGetUnitInstance(UnitInstance loUnitInstance)
    {
        goUnitInstance = loUnitInstance;
        goUnitInstance.SetLayer(GameManager.Layer.UnitAttackLoop);
        goUnitInstance.animator.SetTrigger(GameManager.AnimationControllerData.Triggers.AttackLoop);
    }

    /// <summary>
    /// Se llama cuando nso van a pasar las propiedades de batalla de la unidad.
    /// </summary>
    /// <param name="loUnitBattleProperties"></param>
    private void UpdateUnitBattleProperties(UnitBattleProperties loUnitBattleProperties)
    {
        int lnHashCode = statPrefab.GetHashCode();
        foreach (UIUnitStat loItem in goStatsInstantiated) loItem.gameObject.SetActive(false);
        goStatsInstantiated.Clear();

        InstantiateStatPrefab(lnHashCode, iconHealth, loUnitBattleProperties.Health.ToString(), statsParent);
        InstantiateStatPrefab(lnHashCode, iconPhysicalDamage, loUnitBattleProperties.PhysicalPower.ToString(), statsParent);
        InstantiateStatPrefab(lnHashCode, iconMagicalDamage, loUnitBattleProperties.MagicalPower.ToString(), statsParent);
        InstantiateStatPrefab(lnHashCode, iconPhysicalResistance, loUnitBattleProperties.PhysicalResistance.ToString(), statsParent);
        InstantiateStatPrefab(lnHashCode, iconMagicalResistance, loUnitBattleProperties.MagicalResistance.ToString(), statsParent);
        InstantiateStatPrefab(lnHashCode, iconMovementPoints, loUnitBattleProperties.Movement.ToString(), statsParent);
        InstantiateStatPrefab(lnHashCode, iconAttackRange, loUnitBattleProperties.AttackRange.ToString(), statsParent);
    }

    private void InstantiateStatPrefab(int lnHashCode, Sprite loIcon, string lcStatValue, Transform loParent)
    {
        UIUnitStat loUIUnitStat = GameManager.Spawn(lnHashCode, statPrefab);
        loUIUnitStat.transform.SetParent(loParent, false);
        loUIUnitStat.SetStat(loIcon, lcStatValue);
        goStatsInstantiated.Add(loUIUnitStat);
    }


    /// <summary>
    /// Se llama para llevar la unidad al entrenamiento?
    /// </summary>
    public void OnClickGoToTraining()
    {
        //<< TODO: Falta definir como es el campamento de entrenamiento.
        Deactivate();
    }

    /// <summary>
    /// Se llama cuando desactivamos el panel
    /// </summary>
    public void Deactivate()
    {
        if (isPopUp && panel.activeSelf) GameManager.Send(GameCommand.PlaySound, SoundType.PopUpClose);

        //<< Desactivamos el panel del UI.
        panel.SetActive(false);

        //<< Pedimos desactivar la camara del loop de ataque.
        GameManager.Send(GameCommand.SetActiveCameraAttackLoop, false);

        //<< Removemos el estado de la unidad y la misma.
        if (goUnitInstance != null)
        {
            goUnitInstance.animator.SetTrigger(GameManager.AnimationControllerData.Triggers.EndAttackLoop);
            goUnitInstance.gameObject.SetActive(false);
            goUnitInstance = null;
        }
    }

    /// <summary>
    /// Se llama cuando le dan click al boton de editar nombre de unidad.
    /// </summary>
    public void OnClickEditUnitName()
    {
        GameManager.Send(GameCommand.ShowUIStringEditor, unitName.text, new UnityAction<string>(OnEditedUnitName), nameof(UnitProperties), nameof(UnitProperties.Name), gnUnitPropertiesId, null);
    }

    /// <summary>
    /// Callback de cuando se editó el nombre de la unidad.
    /// </summary>
    /// <param name="lcNewUnitName"></param>
    private void OnEditedUnitName(string lcNewUnitName)
    {
        unitName.text = lcNewUnitName;
    }
}