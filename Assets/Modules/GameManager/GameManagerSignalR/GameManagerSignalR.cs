using UnityEngine;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using System.Collections;
using System.Threading;
using System.Collections.Concurrent;
using System;

public partial class GameManager : MonoBehaviour
{
    /// <summary>
    /// Endpoint del Hub.
    /// </summary>
    private const string HubEndpoint = "gamehub";

    /// <summary>
    /// Es el nombre del metodo de recepcion en el cliente. (Unico)
    /// </summary>
    private const string ReceiveMethod = "SignalRReceive";

    /// <summary>
    /// Para la recepcion de datos.
    /// </summary>
    private ConcurrentQueue<Tuple<SignalRCommand, string>> goConcurrentQueue = new ConcurrentQueue<Tuple<SignalRCommand, string>>();

    /// <summary>
    /// Conexion al Hub.
    /// </summary>
    private HubConnection goHubConnection;

    /// <summary>
    /// Para cancelar la conexión.
    /// </summary>
    private CancellationTokenSource goCancellationTokenSource;

    /// <summary>
    /// Se llama en el Awake del GameManager
    /// </summary>
    private void AwakeSignalR()
    {
        //<< Nos conectamos a los comandos.
        AddListener(OnGameCommandSignalR);
    }

    /// <summary>
    /// Para recepcion de comandos del juego.
    /// </summary>
    /// <param name="loCommand"></param>
    /// <param name="loParams"></param>
    private void OnGameCommandSignalR(GameCommand loCommand, params object[] loParams)
    {
        switch (loCommand)
        {
            case GameCommand.ShowUIFindMatch:
                gnSelectedDistributionBoardId = (int)loParams[0];
                FindMatch();
                break;

            case GameCommand.CancelPairing: Run(CancelPairing(), nameof(CancelPairing)); break;
            case GameCommand.EndTurn: Run(EndTurn(), nameof(EndTurn)); break;


            default: break;
        }
    }

    /// <summary>
    /// Para encontrar batalla.
    /// </summary>
    private void FindMatch()
    {
        StartCoroutine(StartSignalR());
    }

    /// <summary>
    /// Para cancelar emparejamiento.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CancelPairing()
    {
        //<< Mostramos pantalla de carga con fondo.
        GameManager.Send(GameCommand.ShowUILoading, true);

        //<< Si estamos conectados mandamos mensaje de que deseamos desconectarnos.
        if (goHubConnection != null && goHubConnection.State == HubConnectionState.Connected)
        {
            Task loTaskQuitMessage = Task.Run(async () => await SendAsync(goHubConnection, SignalRCommand.Quit, string.Empty));
            while (!loTaskQuitMessage.IsCompleted) yield return null;
        }

        //<< Liberamos conexión.
        yield return CancelSignalR();

        //<< Ocultamos pantalla de carga con fondo.
        GameManager.Send(GameCommand.HideUILoading);
        GameManager.Send(GameCommand.HideUIPairing);
        GameManager.Send(GameCommand.ShowUICombatPreparation);
    }

    /// <summary>
    /// Para terminar el turno.
    /// </summary>
    /// <returns></returns>
    private IEnumerator EndTurn()
    {
        //<< Mostramos pantalla de carga con fondo.
        GameManager.Send(GameCommand.ShowUILoading, true);

        //<< Revisamos si tenemos turnos pendientes.
        int lnActionsUser = goBoard.UserIdA == gnUserId ? goBoard.ActionsUserA : goBoard.ActionsUserB;

        //<< Preguntamos si realmente quiere derjar ir sus acciones pendientes.
        if (lnActionsUser > 0)
        {
            CoroutineResult loCoroutineResultModal = new CoroutineResult();
            GameManager.Send(GameCommand.ShowUIModal, GameManager.GetText(GameTextUsage.Sentence, GameTextSentence.EndTurnConfirmation), GameManager.GetText(GameTextUsage.Word, GameTextWord.Yes), GameManager.GetText(GameTextUsage.Word, GameTextWord.No), null, loCoroutineResultModal);

            //<< Esperamos que nos den indicación de si desean continuar con la operación.
            while (!loCoroutineResultModal.Completed) yield return null;

            //<< Si nos indican que no deseamos continuar salimos y ocultamos pantalla de carga.
            if (!loCoroutineResultModal.Succeeded)
            {
                GameManager.Send(GameCommand.HideUILoading);
                yield break;
            }
        }

        //<< Si estamos conectados mandamos mensaje de que deseamos terminar turno.
        if (goHubConnection != null && goHubConnection.State == HubConnectionState.Connected)
        {
            Task loTaskEndTurnMessage = Task.Run(async () => await SendAsync(goHubConnection, SignalRCommand.EndTurn, string.Empty));
            while (!loTaskEndTurnMessage.IsCompleted) yield return null;
        }

        //<< Ocultamos pantalla de carga con fondo.
        GameManager.Send(GameCommand.HideUILoading);
    }

    /// <summary>
    /// Se llama para cancelar la conexión con SignalR.
    /// </summary>
    private IEnumerator CancelSignalR()
    {
        //<< Iniciamos cancelación del HubTask.
        if (goCancellationTokenSource != null) goCancellationTokenSource.Cancel();

        //<< Si tenemos conexión limpiamos.
        if (goHubConnection != null)
        {
            Task loTask = CleanupConnection(goHubConnection);
            while (!loTask.IsCompleted) yield return null;
            goHubConnection = null;
        }
    }

    /// <summary>
    /// Se llama para iniciar la conexión con SignalR.
    /// </summary>
    private IEnumerator StartSignalR()
    {
        //<< Mostramos pantalla de carga con fondo.
        GameManager.Send(GameCommand.ShowUILoading, true);

        while (true)
        {
            //<< Si tenemos un task previo cancelamos y esperamos la salida.
            yield return CancelSignalR();

            //<< Tenemos que hacer login por si no hay sesión ya que requerimos el token para conectarnos al servidor.
            CoroutineResultValue<string> loCoroutineResult = new CoroutineResultValue<string>();
            yield return Login(loCoroutineResult);

            //<< Si no se pudo hacer login avisamos y preguntamos si deseamos reintentar.
            if (!loCoroutineResult.Succeeded)
            {
                GameManager.Send(GameCommand.ShowUIModal, GameManager.GetText(GameTextUsage.Error, (int)GameTextError.Retry), GameManager.GetText(GameTextUsage.Word, (int)GameTextWord.Retry), GameManager.GetText(GameTextUsage.Word, (int)GameTextWord.Cancel), null, loCoroutineResult);

                while (!loCoroutineResult.Completed) yield return null;

                if (loCoroutineResult.Succeeded) continue;

                //<< Ocultamos pantalla de carga con fondo.
                GameManager.Send(GameCommand.HideUILoading);
                yield break;
            }

            //<< Creamos un cancellation Token para el task del SignalR.
            goCancellationTokenSource = new CancellationTokenSource();

            ValueTask<HubConnection> loValueTask = CreateConnection(goCancellationTokenSource.Token, goConcurrentQueue);
            while (!loValueTask.IsCompleted) yield return null;

            //<< Revisamos si hubo algún error.
            if (loValueTask.Result == null)
            {
                GameManager.Send(GameCommand.ShowUIModal, GameManager.GetText(GameTextUsage.Error, (int)GameTextError.Retry), GameManager.GetText(GameTextUsage.Word, (int)GameTextWord.Retry), GameManager.GetText(GameTextUsage.Word, (int)GameTextWord.Cancel), null, loCoroutineResult);

                while (!loCoroutineResult.Completed) yield return null;

                if (loCoroutineResult.Succeeded) continue;

                //<< Ocultamos pantalla de carga con fondo.
                GameManager.Send(GameCommand.HideUILoading);
                yield break;
            }

            //<< Asignamos la conexión para liberarla mas adelante al salir.
            goHubConnection = loValueTask.Result;

            //<< Ocultamos pantalla de carga con fondo.
            GameManager.Send(GameCommand.HideUILoading);

            //<< Mostramos pantalla de emparejamiento.
            GameManager.Send(GameCommand.ShowUIPairing);

            yield break;
        }
    }

    /// <summary>
    /// Se llama para crear la conexión con el GameHub.
    /// </summary>
    /// <param name="loCancellationToken"></param>
    /// <returns></returns>
    private static async ValueTask<HubConnection> CreateConnection(CancellationToken loCancellationToken, ConcurrentQueue<Tuple<SignalRCommand, string>> loConcurrentQueue)
    {
        //<< Creamos la conexión y la asignamos directamente a goConnection
        HubConnection loHubConnection = new HubConnectionBuilder().WithUrl(WebsiteURL + HubEndpoint, options => { options.AccessTokenProvider = () => Task.FromResult(Token); }).Build();

        //<< Escuchamos los eventos del servidor
        loHubConnection.On<uint, string>(ReceiveMethod, (lnSignalRCommand, lcJson) =>
        {
            loConcurrentQueue.Enqueue(new Tuple<SignalRCommand, string>((SignalRCommand)lnSignalRCommand, lcJson));
        });

        try
        {
            await loHubConnection.StartAsync(loCancellationToken);
        }
        catch (System.Exception loException)
        {
#if UNITY_EDITOR
            Debug.Log(loException.Message);
#endif
            await CleanupConnection(loHubConnection);
            return null;
        }

        return loHubConnection;
    }

    /// <summary>
    /// Se llama para limpiar la conexión.
    /// </summary>
    /// <param name="loHubConnection"></param>
    /// <returns></returns>
    private static async Task CleanupConnection(HubConnection loHubConnection)
    {
        try
        {
            if (loHubConnection != null)
            {
                await loHubConnection.StopAsync(CancellationToken.None);
                await loHubConnection.DisposeAsync();
            }
        }
        catch (System.Exception loException)
        {
#if UNITY_EDITOR
            Debug.Log(loException.Message);
#endif
        }
    }

    /// <summary>
    /// Se intenta enviar un mensaje al hub.
    /// </summary>
    /// <param name="loCommand"></param>
    /// <param name="lcJson"></param>
    private void Send(SignalRCommand loCommand, string lcJson = "")
    {
        _ = Task.Run(async () => await SendAsync(goHubConnection, loCommand, lcJson));
    }

    /// <summary>
    /// Se intenta enviar un mensaje al hub.
    /// </summary>
    /// <param name="loCommand"></param>
    /// <param name="lcJson"></param>
    /// <returns></returns>
    private static async Task SendAsync(HubConnection loHubConnection, SignalRCommand loCommand, string lcJson = "")
    {
        try
        {
            await loHubConnection.SendAsync(ReceiveMethod, loCommand, lcJson).ConfigureAwait(false);
        }
        catch (Exception loException)
        {
            Debug.Log($"SendException - Command: {loCommand} Exception: {loException.Message}");
        }
    }

    /// <summary>
    /// Se llama en el Update de GameManager.
    /// </summary>
    private void UpdateSignalR()
    {
        if (goHubConnection == null) return;
        while (goConcurrentQueue.TryDequeue(out Tuple<SignalRCommand, string> loMessage))
        {
            Debug.Log($"{loMessage.Item1.ToString()}: {loMessage.Item2}");
            switch (loMessage.Item1)
            {
                //<< Nos indican que debemos reiniciar al estado de busqueda.
                case SignalRCommand.Restart: break;
                //<< Nos piden el id del tablero de distribución deseado.
                case SignalRCommand.RequestDistributionBoardId: Send(loMessage.Item1, JsonUtility.ToJson(new DTO1<int>(gnSelectedDistributionBoardId))); break;

                //<< Nos indican que nos estan mandando el tablero.
                case SignalRCommand.Board:
                    {
                        //<< En este punto recibimos el tablero.
                        SetBoard((Board<BoardUnit>)JsonUtility.FromJson(loMessage.Item2, typeof(Board<BoardUnit>)));

                        //<< Reenviamos mensaje con el mismo comando para indicar que recibimos y cargamos el tablero.
                        Send(loMessage.Item1);
                    }
                    break;
                //<< Nos preguntan si debemos esperar al oponente, aqui si mandamos que no deseamos esperar al oponente.
                case SignalRCommand.WaitForOpponent:
                    //<< TODO: Mandar mensaje de texto de usuario tardando.
                    break;
                //<< Nos indican que debemos mostrar ya el tablero y ocultar pantalla de carga.
                case SignalRCommand.ShowBoard:
                    //<< Ocultamos pantalla de emparejamiento.
                    GameManager.Send(GameCommand.HideUIPairing);

                    //<< Reenviamos mensaje con el mismo comando para indicar que recibimos y que ya mostramos el tablero.
                    Send(loMessage.Item1);
                    break;

                case SignalRCommand.StartTimer:
                    //<< Mostramos pantalla de combate.
                    GameManager.Send(GameCommand.ShowUICombat);

                    //<< Mostramos y activamos timer.
                    GameManager.Send(GameCommand.StartTimer, goBoard.TimeoutRoom);

                    //<< Ocultamos pantalla de emparejamiento.
                    GameManager.Send(GameCommand.HideUIPairing);
                    break;

                //<< Nos indican que hay un cambio de turno.
                case SignalRCommand.SetTurn:
                    {
                        //<< Recibimos el turno y el timer.
                        DTO2<int, int> loTurn = (DTO2<int, int>)JsonUtility.FromJson(loMessage.Item2, typeof(DTO2<int, int>));

                        //<< Convertimos de milisegundos a segundos.
                        float lnTime = loTurn.Item2 / 1000.0f;

                        //<< Indicamos de quien es turno.
                        if (loTurn.Item1 == gnUserId) GameManager.Send(GameCommand.SetTurn, lnTime);
                        else GameManager.Send(GameCommand.SetTurnOpponent, lnTime);
                    }
                    break;

                default: break;
            }
        }
    }
}