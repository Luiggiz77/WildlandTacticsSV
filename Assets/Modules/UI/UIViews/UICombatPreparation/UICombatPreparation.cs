using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UICombatPreparation : UIView
{
    [Header("Boards")]
    [Tooltip("Configuracion de los tableros de distribución.")]
    public UIDistributionBoardSettings distributionBoardSettings;

    [Tooltip("VerticalLayoutGroup del que contiene los tableros de distribución del usuario.")]
    public RectTransform panelContentDistributionBoardsUser;

    [Tooltip("RectTransform que contiene el tablero de distribución del oponente.")]
    public RectTransform panelContentDistributionBoardsOpponent;

    [Tooltip("Parefab para crear los tableros de distribución del usuario.")]
    public UIDistributionBoard prefabUIDistributionBoardUser;

    [Tooltip("Parefab para crear los tableros de distribución del oponente.")]
    public UIDistributionBoard prefabUIDistributionBoardOpponent;

    [Tooltip("Parefab para crear los espacios en el tableros de distribución.")]
    public UIDistributionBoardCell prefabUIDistributionBoardCellUser;

    [Tooltip("Parefab para crear los espacios en el tableros de distribución.")]
    public UIDistributionBoardCell prefabUIDistributionBoardCellOpponent;

    [Tooltip("Parefab para crear los espacios en el tableros de distribución central (sin funcionalidad).")]
    public RectTransform prefabUIDistributionBoardCellCenter;

    [Tooltip("Scrollrect del cual obtendremos la altura.")]
    public ScrollRect panelScrollRectPreparation;

    [Tooltip("Panel superior que muestra los tableros, se usa para asignarle su altura.")]
    public LayoutElement panelDistributionBoards;

    [Tooltip("Panel inferior que muestra los objetos y contiene el boton de iniciar batalla, se usa para asignarle su altura.")]
    public LayoutElement panelStartBattle;

    [Tooltip("HorizontalLayoutGroup de los tableros de distribución, se requiere para acomodar correctamente los multiples tableros del usuario.")]
    public HorizontalLayoutGroup horizontalLayoutGroupDistributionBoardSelectionUser;

    [Tooltip("HorizontalLayoutGroup del tablero de distribución, se requiere para acomodar correctamente el tablero del oponente.")]
    public HorizontalLayoutGroup horizontalLayoutGroupDistributionBoardSelectionOpponent;

    [Tooltip("LayoutElement del centro para darle el ancho correcto.")]
    public LayoutElement layoutElementCenterCells;

    [Tooltip("VerticalLayoutGroup para poner los cells del centro sin funcionalidad.")]
    public GridLayoutGroup gridLayoutGroupCenterCells;

    [Tooltip("Paginacion para el scrollrect que contiene los tableros de distribución.")]
    public Pagination paginationScrollRectDistributionBoardsUser;

    [Tooltip("Paginacion para el scrollrect que contiene los tableros de distribución.")]
    public Pagination paginationScrollRectDistributionBoardsOpponent;

    [Tooltip("Texto del nombre del escuadrón del usuario")]
    public TMP_Text distributionBoardNameUser;

    [Tooltip("Texto del nombre del escuadrón del oponente")]
    public TMP_Text distributionBoardNameOpponent;

    [Header("Gameplay Items")]

    [Tooltip("Padre que contiene los prefabs de UIGameplayItemSelector de los items del juego.")]
    public RectTransform panelParentGameplayItems;

    [Tooltip("Prefab del elemento que contiene el item del juego.")]
    public UIGameplayItemSelector prefabUIGameplayItemSelector;

    [Tooltip("Para poner la descripción del item.")]
    public TMP_Text textGameplayItemDescription;

    [Header("Text")]

    [Tooltip("Texto titulo preparación.")]
    public TMP_Text textTitlePreparation;
    [Tooltip("Texto objetos.")]
    public TMP_Text textObjects;
    [Tooltip("Texto botón iniciar batalla.")]
    public TMP_Text textButtonStartBattle;

    /// <summary>
    /// Nos indica el indice del tablero seleccionado del usuario.
    /// </summary>
    private int gnSelectedDistributionBoardIndex = 0;

    /// <summary>
    /// Es el tamaño del padre de los mapas de distribucion.
    /// </summary>
    private Vector2 goDistributionBoardsParentSize;

    /// <summary>
    /// Ancho del tablero de distribución (cuadrantes).
    /// </summary>
    private int gnDistributionBoardWidth = 6;

    /// <summary>
    /// Largo del tablero de distribución (cuadrantes).
    /// </summary>
    private int gnDistributionBoardLength = 7;

    /// <summary>
    /// Son los ids de nuestros tableros de distribución del usuario.
    /// </summary>
    private List<int> goDistributionBoardsIds = new List<int>();

    /// <summary>
    /// Nombres de los tableros de distribución del usuario.
    /// </summary>
    private List<string> goDistributionBoardsNames = new List<string>();

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
        textTitlePreparation.text = GameManager.GetText(GameTextUsage.Word, GameTextWord.Preparation);
        textObjects.text = GameManager.GetText(GameTextUsage.Word, GameTextWord.Objects);
        textButtonStartBattle.text = GameManager.GetText(GameTextUsage.Word, GameTextWord.StartBattle);
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
            case GameCommand.ShowUICombatPreparation: panel.SetActive(true); UpdateLayout(); break;
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
        yield return null;

        //<< Obtenemos la altura del padre del elemento que contiene los tableros de distribución para obtener la altura.
        goDistributionBoardsParentSize = (panelContentDistributionBoardsUser.transform.parent as RectTransform).sizeDelta;

        //<< Pedimos las dimensiones del tablero de distribución.
        GameManager.Send(GameCommand.GetDistributionBoardDimensions, new UnityAction<int, int>(OnGetDistributionBoardDimensions));

        //<< Pedimos nuestros tableros de distribución del usuario.
        GameManager.Send(GameCommand.GetDistributionBoards, new UnityAction<IEnumerable<DistributionBoard>>(OnGetDistributionBoardsUser));

        //<< Pedimos nuestros tableros de distribución del oponente.
        GameManager.Send(GameCommand.GetDistributionBoardOpponent, new UnityAction<DistributionBoard>(OnGetDistributionBoardOpponent));

        //<< Pedimos los ids de los objetos pre-combate.
        GameManager.Send(GameCommand.GetGameplayItemsStored, GameplayItemTiming.BeforeCombat, new UnityAction<IEnumerable<GameplayItemStored>>(OnGetGameplayItemsStoredFiltered));
    }

    /// <summary>
    /// Callback para recibir las dimensiones del tablero de distribución.
    /// </summary>
    /// <param name="loBoardProperties"></param>
    private void OnGetDistributionBoardDimensions(int lnWidth, int lnLength)
    {
        gnDistributionBoardWidth = lnWidth;
        gnDistributionBoardLength = lnLength;
    }

    /// <summary>
    /// Callback de cuando recibimos los ids de los objetos del juego filtrados. Nota: Para esta pantalla son los pre-combate. (GameplayItemTiming.BeforeCombat)
    /// </summary>
    /// <param name="loGameplayItemsStored"></param>
    private void OnGetGameplayItemsStoredFiltered(IEnumerable<GameplayItemStored> loGameplayItemsStored)
    {
        //<< Desactivamos todos los elementos de los items.
        int lnHashCodeGameplayItemSelector = prefabUIGameplayItemSelector.GetHashCode();
        GameManager.DeactivateSpawns(lnHashCodeGameplayItemSelector);

        //<< Agregamos las celdas centrales.
        foreach (GameplayItemStored loGameplayItemStored in loGameplayItemsStored)
        {
            if (loGameplayItemStored.Count < 1) continue;
            UIGameplayItemSelector loUIGameplayItemSelector = GameManager.Spawn(lnHashCodeGameplayItemSelector, prefabUIGameplayItemSelector);
            loUIGameplayItemSelector.transform.SetParent(panelParentGameplayItems, false);
            loUIGameplayItemSelector.transform.localScale = Vector3.one;
            //loUIGameplayItemSelector.layoutElement.preferredHeight = panelParentGameplayItems.rect.height;
            //loUIGameplayItemSelector.layoutElement.preferredWidth = panelParentGameplayItems.rect.height;
            loUIGameplayItemSelector.OnPointerClickGameplayItem = OnPointerClickGameplayItem;
            loUIGameplayItemSelector.SetGameplayItemId(loGameplayItemStored.GameplayItemId);
        }
    }

    /// <summary>
    /// Callback de cuando recibimos los tableros de distribución.
    /// </summary>
    /// <param name="loUnitIds"></param>
    private void OnGetDistributionBoardsUser(IEnumerable<DistributionBoard> loDistributionBoards)
    {
        Vector3 loDistributionBoardSize;
        float lnCellSize;
        float lnPaddingTop;
        float lnPadding;
        int lnHalfPadding;

        //<< Si son de usuario.
        goDistributionBoardsIds.Clear();
        goDistributionBoardsNames.Clear();
        foreach (DistributionBoard loDistributionBoard in loDistributionBoards)
        {
            goDistributionBoardsIds.Add(loDistributionBoard.Id);
            goDistributionBoardsNames.Add(loDistributionBoard.Name);
        }

        //<< Configuramos inicialmente los tableros de distribución sin asignar datos.
        UICamp.SetupDistributionBoards(loDistributionBoards, panelContentDistributionBoardsUser, panelScrollRectPreparation, goDistributionBoardsParentSize.x, goDistributionBoardsParentSize.y, gnDistributionBoardWidth, gnDistributionBoardLength, distributionBoardSettings, prefabUIDistributionBoardUser, prefabUIDistributionBoardCellUser, out loDistributionBoardSize, out lnCellSize, out lnPaddingTop);

        //<< Determinamos el padding de nuestros tableros de distribución usando su ancho.
        lnPadding = goDistributionBoardsParentSize.x - loDistributionBoardSize.x;
        lnHalfPadding = Mathf.FloorToInt(lnPadding * 0.5f);

        //<< Alineamos al centro.
        horizontalLayoutGroupDistributionBoardSelectionUser.spacing = lnPadding;
        horizontalLayoutGroupDistributionBoardSelectionUser.padding.left = lnHalfPadding;
        horizontalLayoutGroupDistributionBoardSelectionUser.padding.right = lnHalfPadding;

        //<< Asignamos el tamaño de las celdas del centro.
        gridLayoutGroupCenterCells.cellSize = new Vector2(lnCellSize, lnCellSize);
        gridLayoutGroupCenterCells.padding.top = Mathf.FloorToInt(lnPaddingTop);

        //<< Desactivamos todas las celdas del centro.
        int lnHashCodeDistributionBoardCellCenter = prefabUIDistributionBoardCellCenter.GetHashCode();
        GameManager.DeactivateSpawns(lnHashCodeDistributionBoardCellCenter);

        //<< Agregamos las celdas centrales.
        for (int i = 0; i < gnDistributionBoardWidth; i++)
        {
            RectTransform loRectTransformCellCenter = GameManager.Spawn(lnHashCodeDistributionBoardCellCenter, prefabUIDistributionBoardCellCenter);
            loRectTransformCellCenter.SetParent(gridLayoutGroupCenterCells.transform, false);
            loRectTransformCellCenter.localScale = Vector3.one;
        }

        //<< Colocamos el nombre del tablero seleccionando el tablero que se encuentra actualmente seleccionado
        UpdateBoardTextName();
    }

    /// <summary>
    /// Callback de cuando recibimos los tableros de distribución.
    /// </summary>
    /// <param name="loUnitIds"></param>
    private void OnGetDistributionBoardOpponent(DistributionBoard loDistributionBoard)
    {
        Vector3 loDistributionBoardSize;
        float lnPadding;
        int lnHalfPadding;

        //<< Colocamos el nombre del tablero
        distributionBoardNameOpponent.text = loDistributionBoard.Name;

        //<< Configuramos inicialmente los tableros de distribución sin asignar datos.
        UICamp.SetupDistributionBoard(loDistributionBoard, panelContentDistributionBoardsOpponent, panelScrollRectPreparation, goDistributionBoardsParentSize.x, goDistributionBoardsParentSize.y, gnDistributionBoardWidth, gnDistributionBoardLength, distributionBoardSettings, prefabUIDistributionBoardOpponent, prefabUIDistributionBoardCellOpponent, out loDistributionBoardSize, out _, out _);

        //<< Determinamos el padding de nuestros tableros de distribución usando su ancho.
        lnPadding = goDistributionBoardsParentSize.x - loDistributionBoardSize.x;

        lnHalfPadding = Mathf.FloorToInt(lnPadding * 0.5f);

        //<< Alineamos al centro.
        horizontalLayoutGroupDistributionBoardSelectionOpponent.spacing = lnPadding;
        horizontalLayoutGroupDistributionBoardSelectionOpponent.padding.left = lnHalfPadding;
        horizontalLayoutGroupDistributionBoardSelectionOpponent.padding.right = lnHalfPadding;
    }

    /// <summary>
    /// Se llama cuando le dan click a la flecha izquieda para cambiar de mapa de distribución.
    /// </summary>
    public void OnClickLeftArrowUser()
    {
        if (gnSelectedDistributionBoardIndex <= 0) return;
        gnSelectedDistributionBoardIndex--;
        UpdateBoardTextName();
        paginationScrollRectDistributionBoardsUser.PreviousPage();
    }

    /// <summary>
    /// Se llama cuando le dan click a la flecha derecha para cambiar de mapa de distribución.
    /// </summary>
    public void OnClickRightArrowUser()
    {
        if (gnSelectedDistributionBoardIndex >= goDistributionBoardsIds.Count - 1) return;
        gnSelectedDistributionBoardIndex++;
        UpdateBoardTextName();
        paginationScrollRectDistributionBoardsUser.NextPage();
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
    /// Se llama cuando dan click al boton de ir a campamento.
    /// </summary>
    public void OnClickGoToCamp()
    {
        panel.SetActive(false);
        GameManager.Send(GameCommand.PlaySound, SoundType.ButtonBack);
        GameManager.Send(GameCommand.ShowUICamp);
    }

    /// <summary>
    /// Se llama cuando la dan click al botón de iniciar combate.
    /// </summary>
    public void OnClickStartCombat()
    {
        panel.gameObject.SetActive(false);
        GameManager.Send(GameCommand.PlaySound, SoundType.ButtonBack);

        //<< TODO: En esta seccion pasamos a pantalla de "Buscando Combate" ó "Iniciando combate" dependiendo de si es IA o Buscar jugador.
        //<< FINDME Temporalmente se pasa directo a batalla en linea.

        //<< Obtenemos el id del tablero que se usará.
        int lnDistributionBoardId = goDistributionBoardsIds[gnSelectedDistributionBoardIndex];

        //<< Avisamos que deseamos iniciar el combate.
        GameManager.Send(GameCommand.ShowUIFindMatch, lnDistributionBoardId);
    }

    public void OnClickNameUser()
    {
        GameManager.Send(GameCommand.PlaySound, SoundType.ButtonBack);
        GameManager.Send(GameCommand.ShowUIEditStringBalloon, distributionBoardNameUser.text, new UnityAction<string>(OnEditedStringUser), nameof(DistributionBoard), nameof(DistributionBoard.Name), goDistributionBoardsIds[gnSelectedDistributionBoardIndex], "False");
    }

    /// <summary>
    /// Se llama cuando se terminó de editar el nombre.
    /// </summary>
    private void OnEditedStringUser(string lcString)
    {
        goDistributionBoardsNames[gnSelectedDistributionBoardIndex] = lcString;
        distributionBoardNameUser.text = lcString;
    }

    private void UpdateBoardTextName()
    {
        if(goDistributionBoardsNames.Count == 0) { distributionBoardNameUser.text = string.Empty; return; }
        distributionBoardNameUser.text = goDistributionBoardsNames[gnSelectedDistributionBoardIndex];
    }

    /// <summary>
    /// Se llama cuando a algun item le han dado click.
    /// </summary>
    /// <param name="lnGameplayItemId"></param>
    private void OnPointerClickGameplayItem(int lnGameplayItemId)
    {
        //<< Asignamos la descripción del item.
        textGameplayItemDescription.SetText(GameManager.GetText(GameTextUsage.GameplayItemDescription, lnGameplayItemId));
    }
}


//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.SignalR.Client;
//using UnityEngine;

//public class SignalRManager : MonoBehaviour
//{
//    private HubConnection _connection;

//    async void Start()
//    {
//        // 1. Construir la conexión apuntando a tu servidor backend
//        _connection = new HubConnectionBuilder()
//            .WithUrl("https://tu-servidor-backend.com/chathub")
//            .WithAutomaticReconnect() // Reconexión automática si se cae el internet
//            .Build();

//        // 2. Escuchar eventos que envía el servidor
//        _connection.On<string, string>("RecibirMensaje", (usuario, mensaje) =>
//        {
//            Debug.Log($"{usuario}: {mensaje}");
//            // Aquí puedes actualizar la interfaz de Unity o mover un objeto
//        });

//        try
//        {
//            // 3. Iniciar la conexión de forma asíncrona
//            await _connection.StartAsync();
//            Debug.Log("¡Conectado a SignalR desde Unity!");
//        }
//        catch (Exception ex)
//        {
//            Debug.LogError($"Error al conectar: {ex.Message}");
//        }
//    }

//    // 4. Método para enviar datos al servidor desde Unity
//    public async Task EnviarMensajeAlServidor(string usuario, string mensaje)
//    {
//        if (_connection.State == HubConnectionState.Connected)
//        {
//            await _connection.SendAsync("EnviarMensaje", usuario, mensaje);
//        }
//    }

//    private async void OnDestroy()
//    {
//        // Limpiar la conexión al cerrar el juego o cambiar de escena
//        if (_connection != null)
//        {
//            await _connection.DisposeAsync();
//        }
//    }
//}