using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UICamp : UIView
{
    [Tooltip("Configuracion de la vista.")]
    public UICampSettings settings;

    [Tooltip("Configuracion de los tableros de distribución.")]
    public UIDistributionBoardSettings distributionBoardSettings;

    [Tooltip("Prefab para seleccionar unidades en el UI.")]
    public UIUnitSelector prefabUIUnitSelector;

    [Tooltip("Grid para las unidades.")]
    public GridLayoutGroup grid;

    [Tooltip("RectTransform del Grid para las unidades.")]
    public RectTransform gridRectTransform;

    [Tooltip("HorizontalLayoutGroup del que contiene los tableros de distribución.")]
    public HorizontalLayoutGroup panelContentDistributionBoards;

    [Tooltip("ScrollRect del que contiene los tableros de distribución.")]
    public ScrollRect panelScrollRectDistributionBoards;

    [Tooltip("Parefab para crear los tableros de distribución del usuario.")]
    public UIDistributionBoard prefabUIDistributionBoardUser;

    [Tooltip("Parefab para crear los tableros de distribución del oponente.")]
    public UIDistributionBoard prefabUIDistributionBoardOpponent;

    [Tooltip("Parefab para crear los espacios en el tableros de distribución del usuario.")]
    public UIDistributionBoardCell prefabUIDistributionBoardCellUser;

    [Tooltip("Parefab para crear los espacios en el tableros de distribución del oponente.")]
    public UIDistributionBoardCell prefabUIDistributionBoardCellOpponent;

    [Tooltip("Texto del nombre del escuadrón")]
    public TMP_Text distributionBoardName;

    [Tooltip("Paginacion para el scrollrect que contiene los tableros de distribución.")]
    public Pagination paginationScrollRectDistributionBoards;

    [Tooltip("Scrollrect para pasarselo a los UIDistributionBoardCells para que se pueda hacer el doble drag.")]
    public ScrollRect scrollRectUIUnitSelectors;

    [Tooltip("Traductor para el dropdown.")]
    public TMP_Dropdown TMP_Dropdown;

    [Tooltip("Boton de intercambio de unidades de usuario a oponente (Solo se muestra en editor).")]
    public GameObject buttonSwitch;

    [Header("Text")]

    [Tooltip("Texto title my squad.")]
    public TMP_Text textTitleMySquad;

    /// <summary>
    /// Es el tamaño del padre de los mapas de distribucion.
    /// </summary>
    private Vector2 goDistributionBoardsParentSize;

    /// <summary>
    /// Ancho del tablero (cuadrantes).
    /// </summary>
    private int gnBoardWidht = 6;

    /// <summary>
    /// Largo del tablero (cuadrantes).
    /// </summary>
    private int gnBoardLenght = 7;

    /// <summary>
    /// Para pedir las instancias al momento de cambiar las unidades de posicion.
    /// </summary>
    private UIDistributionBoardCell[] goUIDistributionBoardCells;

    /// <summary>
    /// Diccionario de tipo de unidades, la llave es un indice y el valor es el id de UnitBattleProperties.
    /// </summary>
    private Dictionary<int, List<UnitRole>> goUnitTypes = new Dictionary<int, List<UnitRole>>();

    /// <summary>
    /// Son los ids de nuestros tableros de distribución del usuario.
    /// </summary>
    private List<int> goDistributionBoardsIds = new List<int>();

    /// <summary>
    /// Nombres de los tableros de distribución del usuario.
    /// </summary>
    private List<string> goDistributionBoardsNames = new List<string>();

    /// <summary>
    /// Nos indica el indice del tablero seleccionado.
    /// </summary>
    private int gnSelectedDistributionBoardIndex = 0;

    /// <summary>
    /// Start
    /// </summary>
    private void Start()
    {
        buttonSwitch.SetActive(true);

        //<< Llenamos los tipos basicos de unidades.
        goUnitTypes.Clear();

        //<< Sabemos que el 0 es "Todas las unidades", al seleccionar un elemento del dropdown usamos el indice para saber que roles son los seleccionados.
        goUnitTypes.Add(0, new List<UnitRole>() { UnitRole.Any });
        goUnitTypes.Add(1, new List<UnitRole>() { UnitRole.Mage, UnitRole.Magician, UnitRole.Sorcerer, UnitRole.Necromancer });
        goUnitTypes.Add(2, new List<UnitRole>() { UnitRole.Archer, UnitRole.Hunter, UnitRole.Sniper, UnitRole.Tracker });
        goUnitTypes.Add(3, new List<UnitRole>() { UnitRole.Warrior, UnitRole.Knight, UnitRole.Barbarian, UnitRole.Paladin });
        goUnitTypes.Add(4, new List<UnitRole>() { UnitRole.Healer, UnitRole.Cleric, UnitRole.Bard, UnitRole.Alchemist });
        goUnitTypes.Add(5, new List<UnitRole>() { UnitRole.Rogue, UnitRole.Thief, UnitRole.Assasin, UnitRole.Jester });

        UpdateText();

        UpdateLayout();//<< FINMDE Borrar despues
    }

    /// <summary>
    /// Se llama para actualizar los textos.
    /// </summary>
    private void UpdateText()
    {
        textTitleMySquad.text = GameManager.GetText(GameTextUsage.Word, GameTextWord.MySquad);

        List<string> loDropdownOptions = new List<string>();

        //<< Llenamos los tipos de unidades.
        foreach (KeyValuePair<int, List<UnitRole>> loItem in goUnitTypes) loDropdownOptions.Add(GameManager.GetText(GameTextUsage.UnitRole, loItem.Value.First()));

        //<< Llenamos el dropdown con los tipos de unidades.
        TMP_Dropdown.ClearOptions();
        TMP_Dropdown.AddOptions(loDropdownOptions);
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
            case GameCommand.ShowUICamp: panel.SetActive(true); UpdateLayout(); break;
            case GameCommand.LanguageChanged: UpdateText(); break;
            default: break;
        }
    }

    /// <summary>
    /// Se usa para actualizar el UI, como el grid con las unidades que tiene el jugador, o el tamaño de los distribution boards.
    /// </summary>
    private void UpdateLayout()
    {
        StartCoroutine(WaitForEndOfFrame());
    }

    /// <summary>
    /// Esperamos un frame para obtener medidas y establecerlas en los controles.
    /// </summary>
    /// <returns></returns>
    public IEnumerator WaitForEndOfFrame()
    {
        //<< Esperamos un frame para obtener las medidas.
        yield return new WaitForEndOfFrame();

        //<< Obtenemos las dimensiones del grid para determinar el tamaño de las celdas.
        float lnWidth = gridRectTransform.rect.width;

        //<< Obtenemos el ancho de las columnas.
        float lnUnitsWidth = lnWidth * settings.unitSelectorWidth;
        float lnColumnWidth = lnUnitsWidth / settings.gridColumns;
        grid.cellSize = new Vector2(lnColumnWidth, lnColumnWidth);

        float lnSpacing = (lnWidth - lnUnitsWidth) / (settings.gridColumns);
        grid.spacing = new Vector2(lnSpacing, lnSpacing);

        //<< Obtenemos la altura del padre del elemento que contiene los tableros de distribución para obtener la altura.
        goDistributionBoardsParentSize = (panelContentDistributionBoards.transform.parent as RectTransform).sizeDelta;

        //<< Pedimos la configuracion del tablero.
        GameManager.Send(GameCommand.GetBoardProperties, new UnityAction<int, int>(OnGetBoardDimensions));

        //<< Actualizamos nuestro tablero de distribución y unidades.
        OnUpdateDistributionBoardsAndUnits(false);
    }

    /// <summary>
    /// Callback de cuando recibimos los ids de las unidades.
    /// </summary>
    /// <param name="loUnitIds"></param>
    private void OnGetUnitIds(int[] loUnitIds)
    {
        int lnHashCode = prefabUIUnitSelector.GetHashCode();

        //<< Desactivamos todos los elementos previos instanciados.
        GameManager.DeactivateSpawns(lnHashCode);

        //<< Creamos los nuevos elementos.
        foreach (int lnId in loUnitIds)
        {
            UIUnitSelector loUIUnitSelector = GameManager.Spawn(lnHashCode, prefabUIUnitSelector);
            loUIUnitSelector.transform.SetParent(grid.transform, false);
            loUIUnitSelector.transform.localScale = Vector3.one;
            loUIUnitSelector.boxCollider2DIcon.size = grid.cellSize;
            loUIUnitSelector.SetUnitPropertiesId(lnId);
            loUIUnitSelector.SetScrollRect(scrollRectUIUnitSelectors);
            loUIUnitSelector.SetActionAllowingScrollRect(AllowDragScrollRect);
        }
    }

    /// <summary>
    /// Funcion para definir si se debe usar el scrollrect o el drag de la unidad.
    /// </summary>
    /// <param name="loDelta"></param>
    /// <returns></returns>
    private bool AllowDragScrollRect(Vector2 loDelta)
    {
        //<< Solo si se mueve a la derecha sabemos que hacemos drag del UIUnitSelector >> Icon.
        if (loDelta.x > 0.0f) return false;

        //<< Permitimos hacerle drag al scroll en ves del UIUnitSelector.
        return true;
    }

    /// <summary>
    /// Callback para recibir las propiedades del tablero.
    /// </summary>
    /// <param name="loBoardProperties"></param>
    private void OnGetBoardDimensions(int lnBoardWidht, int lnBoardLenght)
    {
        gnBoardWidht = lnBoardWidht;
        gnBoardLenght = lnBoardLenght;

        //<< Obtenemos el tamaño del array con base en el tamaño del tablero de distribución.
        int lnLenght = gnBoardWidht * gnBoardLenght;

        //<< Revisamos si tenemos que cambiar el tamaño del tablero.
        if (goUIDistributionBoardCells != null && goUIDistributionBoardCells.Length == lnLenght) return;

        //<< Creamos el array para obtener las busquedas de instancias.
        goUIDistributionBoardCells = new UIDistributionBoardCell[lnLenght];
    }

    /// <summary>
    /// Callback de cuando recibimos los tableros de distribución.
    /// </summary>
    /// <param name="loUnitIds"></param>
    private void OnGetDistributionBoards(bool lbOpponent, IEnumerable<DistributionBoard> loDistributionBoards)
    {
        //<< Configuramos los tableros de distribución.
        UIDistributionBoard loUIDistributionBoard;
        UIDistributionBoardCell loPrefabUIDistributionBoardCell;
        if (lbOpponent)
        {
            loUIDistributionBoard = prefabUIDistributionBoardOpponent;
            loPrefabUIDistributionBoardCell = prefabUIDistributionBoardCellOpponent;
        }
        else
        {
            loUIDistributionBoard = prefabUIDistributionBoardUser;
            loPrefabUIDistributionBoardCell = prefabUIDistributionBoardCellUser;
        }

        SetupDistributionBoards(loDistributionBoards, panelContentDistributionBoards.transform, panelScrollRectDistributionBoards, goDistributionBoardsParentSize.x, goDistributionBoardsParentSize.y, gnBoardWidht, gnBoardLenght, distributionBoardSettings, loUIDistributionBoard, loPrefabUIDistributionBoardCell, out Vector3 loDistributionBoardSize, out _, out _, lbOpponent);

        //<< Determinamos el padding superior e inferior que debe tener el horizontallayoutgroup
        int lnPadding = Mathf.FloorToInt(goDistributionBoardsParentSize.x - loDistributionBoardSize.x);
        int lnHalfPadding = Mathf.FloorToInt(lnPadding * 0.5f);

        //<< Alineamos al centro.
        panelContentDistributionBoards.spacing = lnPadding;
        panelContentDistributionBoards.padding.left = lnHalfPadding;
        panelContentDistributionBoards.padding.right = lnHalfPadding;

        goDistributionBoardsIds.Clear();
        goDistributionBoardsNames.Clear();

        foreach (DistributionBoard loDistributionBoard in loDistributionBoards)
        {
            goDistributionBoardsIds.Add(loDistributionBoard.DistributionBoardId);
            goDistributionBoardsNames.Add(loDistributionBoard.Name);
        }

        UpdateBoardTextName();
    }

    /// <summary>
    /// Se usa para configurar los tableros de distribución.
    /// </summary>
    /// <param name="loDistributionBoards"></param>
    /// <param name="loUIDistributionBoardsParent"></param>
    /// <param name="lnDistributionBoardWidth"></param>
    /// <param name="lnDistributionBoardHeight"></param>
    /// <param name="loBoardProperties"></param>
    /// <param name="loUIDistributionBoardSettings"></param>
    /// <param name="loPrefabUIDistributionBoard"></param>
    /// <param name="loPrefabUIDistributionBoardCell"></param>
    /// <returns>El ancho y alto del tablero de distribución.</returns>
    internal static void SetupDistributionBoards(IEnumerable<DistributionBoard> loDistributionBoards, Transform loUIDistributionBoardsParent, ScrollRect loUIDistributionBoardsScrollRect, float lnDistributionBoardWidth, float lnDistributionBoardHeight, int lnBoardWidth, int lnBoardLenght, UIDistributionBoardSettings loUIDistributionBoardSettings, UIDistributionBoard loPrefabUIDistributionBoard, UIDistributionBoardCell loPrefabUIDistributionBoardCell, out Vector3 loDistributionBoardSize, out float lnCellSize, out float lnPaddingTop, bool lbOpponent)
    {
        int lnHashCodeDistributionBoard = loPrefabUIDistributionBoard.GetHashCode();
        int lnHashCodeDistributionBoardCell = loPrefabUIDistributionBoardCell.GetHashCode();

        //<< Desactivamos todos los elementos previos instanciados.
        GameManager.DeactivateSpawns(lnHashCodeDistributionBoard);
        GameManager.DeactivateSpawns(lnHashCodeDistributionBoardCell);

        //<< Obtenemos el tamaño de los bordes.
        int lnDistributionBoardBorderThickness;

        if (lnDistributionBoardHeight < lnDistributionBoardWidth) lnDistributionBoardBorderThickness = Mathf.FloorToInt(loUIDistributionBoardSettings.borderThickness * lnDistributionBoardHeight * 0.5f);
        else lnDistributionBoardBorderThickness = Mathf.FloorToInt(loUIDistributionBoardSettings.borderThickness * lnDistributionBoardWidth * 0.5f);

        //<< Determinamos la altura del rect del nombre del tablero de distribución.
        float lnDistributionBoardNameHeight = loUIDistributionBoardSettings.nameHeight * (lnDistributionBoardHeight - lnDistributionBoardBorderThickness - lnDistributionBoardBorderThickness);

        float lnCellWidth = (lnDistributionBoardWidth - lnDistributionBoardBorderThickness - lnDistributionBoardBorderThickness) / lnBoardLenght;
        float lnCellHeight = (lnDistributionBoardHeight - lnDistributionBoardNameHeight - lnDistributionBoardBorderThickness - lnDistributionBoardBorderThickness) / lnBoardWidth;

        //<< Usamos el tamaño de celda mas pequeña.
        if (lnCellWidth < lnCellHeight)
        {
            lnCellSize = lnCellWidth;
            lnDistributionBoardHeight = lnCellSize * lnBoardWidth;
        }
        else
        {
            lnCellSize = lnCellHeight;
            lnDistributionBoardWidth = lnCellSize * lnBoardLenght;
        }

        //lnCellSize -= 30f;

        Vector2 loCellSize = new Vector2(lnCellSize, lnCellSize);
        int lnOpponent = Convert.ToInt32(lbOpponent);

        //<< Creamos los nuevos elementos.
        foreach (DistributionBoard loDistributionBoard in loDistributionBoards)
        {
            //<< Instanciamos el UI del tablero de distribucion para agregar los espacios y asignar las unidades.
            UIDistributionBoard loUIDistributionBoard = GameManager.Spawn(lnHashCodeDistributionBoard, loPrefabUIDistributionBoard);

            //<< Agregamos el tablero de distribución como contenido.
            loUIDistributionBoard.transform.SetParent(loUIDistributionBoardsParent, false);
            loUIDistributionBoard.transform.localScale = Vector3.one;

            //<< Establecemos la altura y posicion del nombre del tablero de distribución con base en los settings.
            loUIDistributionBoard.distributionBoardName.rectTransform.sizeDelta = new Vector2(loUIDistributionBoard.distributionBoardName.rectTransform.sizeDelta.x, lnDistributionBoardNameHeight);
            loUIDistributionBoard.distributionBoardName.rectTransform.anchoredPosition = new Vector2(loUIDistributionBoard.distributionBoardName.rectTransform.anchoredPosition.x, -lnDistributionBoardBorderThickness);
            loUIDistributionBoard.distributionBoardName.rectTransform.SetLeft(lnDistributionBoardBorderThickness);
            loUIDistributionBoard.distributionBoardName.rectTransform.SetRight(lnDistributionBoardBorderThickness);

            //<< Indicamos la cantidad de columnas que debe tener el grid.
            loUIDistributionBoard.gridLayoutGroup.constraintCount = lnBoardLenght;

            //<< Asignamos el id del tablero de distribución.
            loUIDistributionBoard.gnDistributionBoardId = loDistributionBoard.DistributionBoardId;

            //<< Asignamos el nombre del tablero de distribución. //<< TEMP VLAD
            //loUIDistributionBoard.distributionBoardName.text = loDistributionBoard.Name;

            //<< Establecemos la altura del tablero de distribución con base en los settings.
            loUIDistributionBoard.layoutElement.minHeight = lnDistributionBoardHeight;
            loUIDistributionBoard.layoutElement.minWidth = lnDistributionBoardWidth;

            //<< Establecemos el tamaño de las celdas.
            loUIDistributionBoard.gridLayoutGroup.cellSize = loCellSize;

            //<< Establecemos el espacio del nombre del tablero de distribución.
            loUIDistributionBoard.gridLayoutGroup.padding.top = Mathf.FloorToInt(lnDistributionBoardNameHeight + lnDistributionBoardBorderThickness);
            loUIDistributionBoard.gridLayoutGroup.padding.left = lnDistributionBoardBorderThickness;
            loUIDistributionBoard.gridLayoutGroup.padding.right = lnDistributionBoardBorderThickness;
            loUIDistributionBoard.gridLayoutGroup.padding.bottom = lnDistributionBoardBorderThickness;

            //<< Establecemos el tamaño de los bordes.
            foreach (LayoutElement loItem in loUIDistributionBoard.horizontalBorders) loItem.minHeight = lnDistributionBoardBorderThickness;
            foreach (LayoutElement loItem in loUIDistributionBoard.verticalBorders) loItem.minWidth = lnDistributionBoardBorderThickness;

            //<< Instanciamos tantos espacios como sean necesarios. Nota: Es X y Y ya que el tablero de distribución lo vemos de manera lateral.
            for (int x = 0; x < lnBoardWidth; x++)
            {
                for (int z = 0; z < lnBoardLenght; z++)
                {
                    //<< Determinamos si hay una unidad en este espacio del mapa de distribucion de unidades.
                    int lnUnitPropertiesId = loDistributionBoard.GetIdOf(x, z);

                    //<< Creamos el cell, emparentamos y configuramos.
                    UIDistributionBoardCell loUIDistributionBoardCell = GameManager.Spawn(lnHashCodeDistributionBoardCell, loPrefabUIDistributionBoardCell);
                    loUIDistributionBoardCell.transform.SetParent(loUIDistributionBoard.gridLayoutGroup.transform, false);
                    loUIDistributionBoardCell.transform.localScale = Vector3.one;
                    loUIDistributionBoardCell.boxCollider2DIcon.size = loCellSize;
                    loUIDistributionBoardCell.boxCollider2DBackground.size = loCellSize;
                    loUIDistributionBoardCell.SetDistributionBoardId(loDistributionBoard.DistributionBoardId);
                    loUIDistributionBoardCell.SetCoordinates(x, z);
                    loUIDistributionBoardCell.SetUnitPropertiesId(lnUnitPropertiesId);
                    loUIDistributionBoardCell.SetScrollRect(loUIDistributionBoardsScrollRect);
                    loUIDistributionBoardCell.SetActionAllowingScrollRect(null);
                    loUIDistributionBoardCell.name = $"{loDistributionBoard.DistributionBoardId}_{x}_{z}_{lnOpponent}";
                }
            }
        }

        //<< Obtenemos las dimensiones del tablero.
        loDistributionBoardSize = new Vector2(lnDistributionBoardWidth, lnDistributionBoardHeight);

        //<< Obtenemos el padding de la parte superior.
        lnPaddingTop = lnDistributionBoardNameHeight + lnDistributionBoardBorderThickness;
    }

    /// <summary>
    /// Se llama cuando dan click al boton de regresar.
    /// </summary>
    public void OnClickReturn()
    {
        panel.SetActive(false);
        GameManager.Send(GameCommand.PlaySound, SoundType.ButtonBack);
        GameManager.Send(GameCommand.ShowUIMainMenu);
    }

    /// <summary>
    /// Se llama cuando cambia el dropdown del tipo de unidad.
    /// </summary>
    public void OnDropdownUnitTypeChanged(int lnIndex)
    {
        //<< Sabemos que son todas las unidades.
        if (lnIndex <= 0)
        {
            GameManager.Send(GameCommand.GetUnitIds, new UnityAction<int[]>(OnGetUnitIds));
            return;
        }

        //<< Obtenemos el id del tipo de unidad que es y filtramos las unidades mostrando solo aquellas que tienen este id.
        List<UnitRole> loUnitRoles = goUnitTypes[lnIndex];

        //<< Pedimos nuestras unidades filtradas por tipo.
        GameManager.Send(GameCommand.GetUnitIdsFilteredByRoles, new UnityAction<int[]>(OnGetUnitIds), loUnitRoles);
    }

    /// <summary>
    /// Se llama para actualizar las unidades así como los tableros de distribución.
    /// </summary>
    /// <param name="lbOpponent"></param>
    private void OnUpdateDistributionBoardsAndUnits(bool lbOponent)
    {
        //<< Desactivamos los dos tipos de cells.
        GameManager.DeactivateSpawns(prefabUIDistributionBoardCellUser.GetHashCode());
        GameManager.DeactivateSpawns(prefabUIDistributionBoardCellOpponent.GetHashCode());

        GameManager.DeactivateSpawns(prefabUIDistributionBoardUser.GetHashCode());
        GameManager.DeactivateSpawns(prefabUIDistributionBoardOpponent.GetHashCode());

        //<< Pedimos nuestras unidades.
        //OnDropdownUnitTypeChanged(TMProTranslationDropdown.component.value);

        //<< Pedimos nuestras unidades.
        GameManager.Send(GameCommand.GetUnitIds, new UnityAction<int[]>(OnGetUnitIds));

        //<< Pedimos nuestros tableros de distribución.
        GameManager.Send(GameCommand.GetDistributionBoards, new UnityAction<bool, IEnumerable<DistributionBoard>>(OnGetDistributionBoards));
    }

    /// <summary>
    /// Se llama cuando le dan click a la flecha derecha para cambiar de mapa de distribución.
    /// </summary>
    public void OnClickRightArrowUser()
    {
        if (gnSelectedDistributionBoardIndex >= goDistributionBoardsIds.Count - 1) return;
        gnSelectedDistributionBoardIndex++;
        UpdateBoardTextName();
        paginationScrollRectDistributionBoards.NextPage();
    }

    /// <summary>
    /// Se llama cuando le dan click a la flecha izquieda para cambiar de mapa de distribución.
    /// </summary>
    public void OnClickLeftArrowUser()
    {
        if (gnSelectedDistributionBoardIndex <= 0) return;
        gnSelectedDistributionBoardIndex--;
        UpdateBoardTextName();
        paginationScrollRectDistributionBoards.PreviousPage();
    }

    private void UpdateBoardTextName()
    {
        distributionBoardName.text = goDistributionBoardsNames[gnSelectedDistributionBoardIndex];
    }
}