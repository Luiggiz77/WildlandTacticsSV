/// <summary
/// En cada comando debe indicarse los parametros que se mandan asi como su indice y su tipo, ésto dentro de su summary.
/// </summary>
public enum GameCommand : int
{
    /// <summary>
    /// Se llama para obtener el inventario del oponente.
    /// param[0] = bool lbOpponent - Bandera que nos indica de quien es el inventario si es del usuario o del oponente. True indica que es inventario del oponente.
    /// param[1] = GameplayItemTiming? loTiming - Nos indica de que momento se usan los items, por ende cualquier item con este momento será regresado en el callback. Nota: Si es null manda todo, de lo contrario lo manda filtrado.
    /// param[2] = UnityAction<IEnumerable<GameplayItemStored>> loCallback - Callback que da el inventario requerido.
    /// </summary>
    GetGameplayItemsStored,

    /// <summary>
    /// Se indica para avisar el turno durante el juego.
    /// param[0] = bool lbOpponent - Bandera que nos indica si quien termina el turno es del usuario o del oponente. True indica que es turno del oponente.
    /// </summary>
    EndTurn,

    /// <summary>
    /// Se indica para avisar el turno durante el juego.
    /// param[0] = bool lbOpponent - Bandera que nos indica si es turno del usuario o del oponente. True indica que es turno del oponente.
    /// </summary>
    SetTurn,

    /// <summary>
    /// Se indica para indicar la cantidad de turnos disponibles.
    /// param[0] = bool lbOpponent - Bandera que nos indica si es del usuario o del oponente. True indica que es del oponente.
    /// param[1] = int lnActions - Nos indica la cantidad de acciones.
    /// param[2] = int lnActionsMax - Nos indica la cantidad maxima de acciones.
    /// </summary>
    SetActionsCount,

    /// <summary>
    /// Se indica para avisar al GameManager que debe iniciar el combate, éste enviará mensaje de <see cref="SetTurn"/> para indicar quien inicia la partida.
    /// </summary>
    StartCombat,

    /// <summary>
    /// Se indica para avisar que se ha seleccionado una unidad en el tablero, se manda para que la interfaz tenga conocimiento y realice sus acciones.
    /// param[0] = int UnitPropertiesId - Id de la unidad selecionada.
    /// param[1] = bool lbOpponent - Bandera que nos indica si es del usuario o del oponente. True indica que es del oponente.
    /// param[2] = Color loColor - Color de la selección.
    /// </summary>
    SetSelectedUnit,

    /// <summary>
    /// Se indica para avisar que se debe habilitar el ataque de la unidad.
    /// param[0] = bool lbOpponent - Bandera que nos indica si es del usuario o del oponente. True indica que es del oponente.
    /// param[1] = UnityAction<int> UnitAttackEnabled - Callback que indica que se ha habilitado el ataque correctamente.
    /// </summary>
    EnableUnitAttack,

    /// <summary>
    /// Se indica para avisar que se debe habilitar el movimiento de la unidad.
    /// param[0] = bool lbOpponent - Bandera que nos indica si es del usuario o del oponente. True indica que es del oponente.
    /// param[1] = UnityAction<int> UnitMoveEnabled - Callback que indica que se ha habilitado el movimiento correctamente.
    /// </summary>
    EnableUnitMove,

    /// <summary>
    /// Se indica para avisar que se debe habilitar/deshabilitar el boton de confirmación de ataque de la unidad.
    /// param[0] = bool lbOpponent - Bandera que nos indica si es del usuario o del oponente. True indica que es del oponente.
    /// param[1] = bool lbEnable - Bandera que nos indica si debe o no estár habilitado el botón.
    /// </summary>
    SetEnableConfirmUnitAttack,

    /// <summary>
    /// Se indica para avisar que se debe habilitar/deshabilitar el boton de confirmación de movimiento de la unidad.
    /// param[0] = bool lbOpponent - Bandera que nos indica si es del usuario o del oponente. True indica que es del oponente.
    /// param[1] = bool lbEnable - Bandera que nos indica si debe o no estár habilitado el botón.
    /// </summary>
    SetEnableConfirmUnitMove,

    /// <summary>
    /// Se indica para avisar que se debe habilitar el ataque de la unidad.
    /// param[0] = bool lbOpponent - Bandera que nos indica si es del usuario o del oponente. True indica que es del oponente.
    /// </summary>
    ConfirmUnitAttack,

    /// <summary>
    /// Se indica para avisar que se debe habilitar el movimiento de la unidad.
    /// param[0] = bool lbOpponent - Bandera que nos indica si es del usuario o del oponente. True indica que es del oponente.
    /// </summary>
    ConfirmUnitMove,

    /// <summary>
    /// Se indica para pedir el tipo de acción actual. Para saber si es de movimiento u ataque de la unidad.
    /// param[0] = UnityAction<UnitActionMode> loUnitActionMode - Callback que indica el tipo de accón actual.
    /// </summary>
    GetUnitActionMode,

    /// <summary>
    /// Se llama para indicar que se debe aplicar un efecto a una unidad.
    /// param[0] = int UnitPropertiesId - Id de la unidad selecionada.
    /// param[1] = GameplayEffectType loGameplayEffectType - Tipo de efecto que se aplica.
    /// param[2] = int lnValue - Valor con base en el efecto que se aplica.
    /// </summary>
    ApplyGameplayEffectOnUnit,

    /// <summary>
    /// Se llama para indicar que se debe remover un efecto a una unidad.
    /// param[0] = int UnitPropertiesId - Id de la unidad selecionada.
    /// param[1] = GameplayEffectType loGameplayEffectType - Tipo de efecto que se aplica.
    /// param[2] = int lnValue - Valor con base en el efecto que se aplica.
    /// </summary>
    RemoveGameplayEffectFromUnit,

    /// <summary>
    /// Es para indicarle al GameManager que se debe guardar un cambio de valor en algun objeto.
    /// param[0] = string lcValue valor el cual se establecerá. Nota: Siempre debe ser un string, se debe parsear si es de otro tipo, ejemplo int.toString().
    /// param[1] = UnityAction<string> loOnValueSet - callback para cuando se haya guardado el valor.
    /// param[2] = string lcObjectName string del nombre del objeto que se debe modificar.
    /// param[3] = string lcPropertyName string del nombre de la propiedad del objeto que se debe modificar.
    /// param[4] = int lnObjectId int del id principal del objeto que se debe modificar.
    /// param[5] = string lcData string de algun dato que pueda ser de utilidad, por ejemplo cuando dos tablas comparte el mismo tipo de objeto aqui podemos indicar a cual de las tablas dirigir.
    /// </summary>
    SetValue,

    /// <summary>
    /// Se llama cuando sucede un evento de mouse sobre el "tablero".
    /// Nota: Solo es el evento que sucede en un panel en UICombat que se reenvia al GameManager.
    /// </summary>
    OnBoardMouseDown,

    /// <summary>
    /// Se llama cuando sucede un evento de mouse sobre el "tablero".
    /// Nota: Solo es el evento que sucede en un panel en UICombat que se reenvia al GameManager.
    /// </summary>
    OnBoardMouseDrag,

    /// <summary>
    /// Se llama cuando sucede un evento de mouse sobre el "tablero".
    /// Nota: Solo es el evento que sucede en un panel en UICombat que se reenvia al GameManager.
    /// </summary>
    OnBoardMouseUp,

    /// <summary>
    /// Da las unidades del usuario.
    /// param[0] = bool lbOpponent - Bandera que nos indica si son las unidades del usuario o del oponente. True indica que son unidades del oponente.
    /// param[1] = UnityAction<bool, IEnumerable<int>> RetieveUnitIds - Callback que da los ids de las unidades del usuario y el boleano nos indica si es de oponente.
    /// </summary>
    GetUnitIds,

    /// <summary>
    /// Da los ids de las unidades pero filtradas por...
    /// param[0] = bool lbOpponent - Bandera que nos indica si son las unidades del usuario o del oponente. True indica que son unidades del oponente.
    /// param[1] = UnityAction<bool, IEnumerable<int>> RetieveUnitIds - Callback que da los ids de las unidades del usuario y el boleano nos indica si es de oponente.
    /// param[2] = List<UnitRole> loUnitRoles - Lista de roles por los que debemos filtrar.
    /// </summary>
    GetUnitIdsFilteredByRoles,

    /// <summary>
    /// Da las propiedades del tablero.
    /// param[0] = UnityAction<int,int> GetBoardDimensions - Callback que recibe las dimensiones del tablero.
    /// </summary>
    GetBoardDimensions,

    /// <summary>
    /// Da las dimensiones del tablero de distribución.
    /// param[0] = UnityAction<int,int> GetDistributionBoardDimensions - Callback que recibe las dimensiones del tablero de distribución.
    /// </summary>
    GetDistributionBoardDimensions,

    /// <summary>
    /// Da las propiedades del tablero.
    /// param[0] = UnityAction<BoardProperties> GetBoardProperties - Callback que recibe configuración del tablero.
    /// </summary>
    GetBoardProperties,

    /// <summary>
    /// Da las propiedades de la batalla.
    /// param[0] = UnityAction<BattleProperties> GetBattleProperties - Callback que recibe configuración de batalla.
    /// </summary>
    GetBattleProperties,

    /// <summary>
    /// Da los tableros de distribución del usuario.
    /// param[0] = UnityAction<IEnumerable<DistributionBoard>> RetieveDistributionBoards - Callback que da los tableros de distribucion almacenados.
    /// </summary>
    GetDistributionBoards,

    /// <summary>
    /// Nos da las propiedades de una unidad.
    /// param[0] = int UnitPropertiesId - Id de la unidad de la que deseamos las propiedades.
    /// param[1] = UnityAction<UnitBattleProperties,UnitProperties> RetieveUnitProperties - Callback que nos da las propiedades de la unidad incluyendo las de batalla.
    /// </summary>
    GetUnitProperties,

    /// <summary>
    /// Nos crea una instancia de una unidad.
    /// param[0] = int UnitPropertiesId - Id de la unidad de la que deseamos la instancia.
    /// param[1] = UnityAction<UnitInstance> RetieveUnitInstance - Callback que nos da una instancia de la unidad.
    /// </summary>
    CreateUnitInstance,

    /// <summary>
    /// Nos da la textura 2D requerida.
    /// param[0] = UnityAction<Texture2D> Texture2DCallback - Callback que nos da la textura 2D.
    /// param[1] = GameTexture2DUsage Usage - Indica el uso al cual se le dará la textura.
    /// param[2] = int Key - Indica la llave de la textura, puede venir de un Id o del valor de un enum.
    /// </summary>
    GetTexture2D,

    /// <summary>
    /// Nos da el color de nuestra unidad.
    /// param[0] = UnityAction<Color32> Texture2DColorCallback - Callback que nos da el color de la textura2D.
    /// param[1] = GameTexture2DUsage Usage - Indica el uso al cual se le dará la textura.
    /// param[2] = int Key - Indica la llave de la textura, puede venir de un Id o del valor de un enum.
    /// </summary>
    GetTexture2DColor,

    /// <summary>
    /// Agrega una unidad en el tablero de distribución dado.
    /// param[0] = int lnDistributionBoardId id del tablero de distribución.
    /// param[1] = int lnUnitPropertiesId id de la unidad que se asigna al tablero de distribución.
    /// param[2] = int lnDistributionBoardX coordenada en X del tablero de distribución.
    /// param[3] = int lnDistributionBoardZ coordenada en Z del tablero de distribución.
    /// </summary>
    AddUnitToDistributionBoard,

    /// <summary>
    /// Indica que se agregó una unidad en el tablero de distribución dado.
    /// param[0] = int lnDistributionBoardId id del tablero de distribución.
    /// param[1] = int lnUnitPropertiesId id de la unidad que se asigna al tablero de distribución.
    /// param[2] = int lnDistributionBoardX coordenada en X del tablero de distribución.
    /// param[3] = int lnDistributionBoardZ coordenada en Z del tablero de distribución.
    /// </summary>
    AddedUnitToDistributionBoard,

    /// <summary>
    /// Remover la unidad del tablero de distribución.
    /// param[0] = int lnDistributionBoardId id del tablero de distribución.
    /// param[1] = int lnUnitPropertiesId - Id de la unidad a remover.
    /// </summary>
    RemoveUnitFromDistributionBoard,

    /// <summary>
    /// Indica que se ha removido exitosamente la unidad del tablero de distribución.
    /// param[0] = int lnDistributionBoardId id del tablero de distribución.
    /// param[1] = int lnUnitPropertiesId - Id de la unidad removida.
    /// </summary>
    RemovedUnitFromDistributionBoard,

    /// <summary>
    /// Nos indica si la unidad existe en algun tablero de distribución.
    /// param[0] = int lnDistributionBoardId - Id del tablero de distribución.
    /// param[1] = int lnUnitPropertiesId - Id de las propiedades de la unidad.
    /// param[2] = UnityAction<bool> UnitExistsOnDistributionBoard - Callback que nos indica si la unidad se encuentra en algún tablero de distribución.
    /// </summary>
    UnitExistsOnDistributionBoard,

    /// <summary>
    /// Se llama para indicar que debemos mostrar el globo intermediario para la de edicion de un string. Nota: Este manda los parametros recibidos a la pantalla de edicion de string.
    /// param[0] = string lcString string el cual se editará.
    /// param[1] = UnityAction<string> loOnStringEdited - callback para cuando se haya editado el string.
    /// param[2] = string lcObjectName string del nombre del objeto que se debe modificar.
    /// param[3] = string lcPropertyName string del nombre de la propiedad del objeto que se debe modificar.
    /// param[4] = int lnObjectId int del id principal del objeto que se debe modificar.
    /// param[5] = string lcData string de algun dato que pueda ser de utilidad, por ejemplo cuando dos tablas comparte el mismo tipo de objeto aqui podemos indicar a cual de las tablas dirigir.
    /// </summary>
    ShowUIEditStringBalloon,

    /// <summary>
    /// Se llama para indicar que debemos usar uno de los items.
    /// param[0] = int lnGameplayItemId int del id del objeto a usar.
    /// param[1] = bool lbOpponent - Indica si es item del usuario o del oponente.
    /// param[2] = UnityAction<bool> loOnUseGameplayItem - callback que indica si el item se usará o no.
    /// </summary>
    UseGameplayItem,

    /// <summary>
    /// Se llama para indicar que el item estará o no seleccionado. Nota: Se usa regulamente para objetos de uso previo combate o despues del combate.
    /// param[0] = int lnGameplayItemId int del id del objeto.
    /// param[1] = bool lbOpponent - Indica si es item del usuario o del oponente.
    /// param[2] = UnityAction<bool> loOnSetGameplayItemSelection - callback que indica si el item está seleccionado o no.
    /// </summary>
    SetGameplayItemSelection,

    /// <summary>
    /// Se llama para preguntar si el item está seleccionado. Nota: Se usa regulamente para objetos de uso previo combate o despues del combate.
    /// param[0] = int lnGameplayItemId int del id del objeto.
    /// param[1] = bool lbOpponent - Indica si es item del usuario o del oponente.
    /// param[2] = UnityAction<bool> loOnGetGameplayItemSelection - callback que indica si el item está seleccionado o no.
    /// </summary>
    GetGameplayItemSelection,

    /// <summary>
    /// Se llama para intentar mover una unidad por los ids de las celdas dados, este camino es validado por el servidor previamente.
    /// param[0] = int lnUnitPropertiesId - Id de las propiedades visuales de la unidad.
    /// param[1] = bool lbOpponent - Indica si la unidad es del usuario o del oponente.
    /// param[2] = int[] loCellIds - Son los ids de las celdas por las que pasa la unidad en orden.
    /// </summary>
    UnitMove,

    /// <summary>
    /// Se llama para intentar atacar con la unidad por los ids de las celdas dados, esta area es validada por el servidor previamente.
    /// param[0] = int lnUnitPropertiesId - Id de las propiedades visuales de la unidad.
    /// param[1] = int[] loCellIds - Son los ids de las celdas a las que atacará la unidad.
    /// </summary>
    UnitAttack,

    /// <summary>
    /// Se llama cuando el servidor validó el camino seleccionado para la unidad y procede a moverse la unidad.
    /// param[0] = int lnUnitPropertiesId - Id de las propiedades visuales de la unidad.
    /// param[1] = bool lbOpponent - Indica si la unidad es del usuario o del oponente.
    /// param[2] = int lnMoves - Indica la cantidad de movimientos disponibles.
    /// </summary>
    UnitMoved,

    /// <summary>
    /// Se llama cuando el servidor validó el ataque de la unidad y procede al ataque.
    /// param[0] = int lnUnitPropertiesId - Id de las propiedades visuales de la unidad.
    /// param[1] = bool lbOpponent - Indica si la unidad es del usuario o del oponente.
    /// param[2] = int lnAttacks - Indica la cantidad de ataques disponibles.
    /// </summary>
    UnitAttacked,

    /// <summary>
    /// Se llama para activar o desactivar la camara que captura el ataque de la unidad en loop.
    /// param[0] = bool lbActive nos indica la bandera para activar o desactivar.
    /// </summary>
    SetActiveCameraAttackLoop,

    /// <summary>
    /// Se le indica a la pantalla de Combate que debe asignar a un elemento hijo que es usado como efecto.
    /// param[0] = Transform loTransformChild - Es el transform del hijo que debe agregarse al panel que se usa para UIEffects.
    /// </summary>
    UICombatSetUIEffectChild,

    /// <summary>
    /// Se llama para pedir una posicion del UI para algun efecto que se base en el UI, como por ejemplo cambiar la cantidad de acciones.
    /// param[0] = bool lbOpponent - Indica si la unidad es del usuario o del oponente.
    /// param[1] = string lcUIName - Indica el nombre del GameObject del UI que se pide.
    /// param[2] = UnityAction<Vector3> loOnGetPosition - callback que para dar la posicion del UI requerido.
    /// </summary>
    RequestUIPosition,

    /// <summary>
    /// Se llama para pedir un transform del UI para algun efecto que se base en el UI, como por ejemplo animar el transform.
    /// param[0] = bool lbOpponent - Indica si la unidad es del usuario o del oponente.
    /// param[1] = string lcUIName - Indica el nombre del GameObject del UI que se pide.
    /// param[2] = UnityAction<Transform> loOnGetTransform - callback que para dar el transform del UI requerido.
    /// </summary>
    RequestUITransform,

    /// <summary>
    /// Se llama para indicar que el nombre de una unidad cambió.
    /// param[0] = int lnUnitPropertiesId - Es el id de UnitProperties.
    /// param[1] = string lcName - Es el nuevo nombre asignado de la unidad.
    /// </summary>
    UnitPropertiesNameChanged,

    #region Terrain
    /// <summary>
    /// Se llama para obtener el id del terreno seleccionado.
    /// param[0] = UnityAction<int> loOnGetTerrainId - Es el id del terreno que está seleccionado.
    /// </summary>
    GetTerrain,

    /// <summary>
    /// Se llama para indicar el terreno que debemos mostrar con base en su Id.
    /// param[0] = int lnTerrainId - Es el id del terreno que debemos mostrar.
    /// </summary>
    SetTerrain,

    /// <summary>
    /// Se llama para obtener los ids de los terrenos disponibles.
    /// param[0] = UnityAction<IEnumerable<int>> loOnGetTerrainIds - callback que para dar los ids de los terrenos disponibles.
    /// </summary>
    GetTerrainIds,

    GetPurchasables,

    UpdatePurchasables,
    #endregion

    /// <summary>
    /// Se llama para indicarle a la pantalla de descarga que debe poner cierto porcentaje de descarga.
    /// param[0] = float lnPercent - Es el porcentaje de descarga.
    /// </summary>
    SetUIDownloadingPercent,

    /// <summary>
    /// Se llama para indicarle a la pantalla de descarga que debe poner cierto texto.
    /// param[0] = string lcText - Es el texto que deseamos mostrar en la pantalla de descarga.
    /// </summary>
    SetUIDownloadingText,

    #region Show UI
    /// <summary>
    /// Mostrar la pantalla de descarga.
    /// </summary>
    ShowUIDownloading,

    /// <summary>
    /// Mostrar la pantalla de carga para realizar una corrutina de otra interfaz u objeto que lo requiera.
    /// param[0] = string lcText - Es el texto a mostrar en el modal.
    /// param[1] = string lcTextButtonOk - Es el texto a mostrar en el boton de Ok.
    /// param[2] = string lcTextButtonCancel - Es el texto a mostrar en el boton de Cancel. Nota: Si es null o empty no se muestra el botón de cancelar.
    /// param[3] = UnityAction<bool> loCallback - Evento para indicar si le dieron click a Ok (true) o Cancel (false). (Opcional)
    /// param[4] = CoroutineResultStruct<bool> loObjectCallback - Objeto para indicar si le dieron click a Ok (true) o Cancel (false). (Opcional)
    /// </summary>
    ShowUIModal,

    /// <summary>
    /// Mostrar la pantalla de carga para realizar una corrutina de otra interfaz u objeto que lo requiera.
    /// param[0] = IEnumerator loCoroutine - Es la corrutina que debe ejecutarse en la pantalla de carga.
    /// </summary>
    ShowUILoading,

    /// <summary>
    /// Mostrar la pantalla de menú principal.
    /// </summary>
    ShowUIMainMenu,

    /// <summary>
    /// Mostrar la pantalla de un solo jugador.
    /// </summary>
    ShowUISinglePlayer,

    /// <summary>
    /// Nos muestra la pantalla de información de una unidad. 
    /// param[0] = int lnUnitPropertiesId id de la unidad de la cual deseamos ver su información.
    /// </summary>
    ShowUIUnitInformation,

    /// <summary>
    /// Nos muestra la pantalla de información de un objeto 
    /// param[0] = int lnGameplayItemId id del objeto del cual deseamos ver su información.
    /// </summary>
    ShowUIGameplayItemInformation,

    /// <summary>
    /// Mostrar la pantalla de historia.
    /// </summary>
    ShowUIHistory,

    /// <summary>
    /// Mostrar la pantalla de progreción.
    /// </summary>
    ShowUIProgression,

    /// <summary>
    /// Mostrar la pantalla de dialogo.
    /// param[0] = string lcMessage - Es el texto del mensaje.
    /// param[1] = UnityAction<bool?> loOnDialogresult - callback que indica si se presionó el boton de "Yes" = true, "No" = false ó "Cancel" = null.
    /// param[2] = bool lbShowNoButton - Para indicar si debemos mostrar el botón de "No".
    /// param[3] = HorizontalAlignmentOptions loHorizontalAligment - Para indicar como se debe alinear horizontalmente el texto del mensaje.
    /// </summary>
    ShowUIDialog,

    /// <summary>
    /// Mostrar la pantalla de recompensas.
    /// </summary>
    ShowUIRewards,

    /// <summary>
    /// Mostrar la pantalla de escuadrón.
    /// </summary>
    ShowUISquad,

    /// <summary>
    /// Mostrar la pantalla de desafios.
    /// </summary>
    ShowUIChallenge,

    /// <summary>
    /// Mostrar la pantalla de plaza.
    /// </summary>
    ShowUIFleaMarket,

    /// <summary>
    /// Mostrar la pantalla de carpa mistica.
    /// </summary>
    ShowUIMysticCarp,

    /// <summary>
    /// Mostrar la pantalla del teatro.
    /// </summary>
    ShowUITheater,

    /// <summary>
    /// Mostrar la pantalla del puerto.
    /// </summary>
    ShowUIPort,

    /// <summary>
    /// Mostrar la pantalla de mercado.
    /// </summary>
    ShowUISquare,

    /// <summary>
    /// Mostrar la pantalla de multijugador.
    /// </summary>
    ShowUIMultiplayer,

    /// <summary>
    /// Mostrar la pantalla de pelea en trincheras.
    /// </summary>
    ShowUITrenches,

    /// <summary>
    /// Mostrar la pantalla de carnet.
    /// </summary>
    ShowUILicense,

    /// <summary>
    /// Mostrar la pantalla de logros.
    /// </summary>
    ShowUIAchievements,

    /// <summary>
    /// Mostrar la pantalla de edición de licencia.
    /// </summary>
    ShowUILicenseEdit,

    /// <summary>
    /// Mostrar la pantalla de configuración.
    /// </summary>
    ShowUISettings,

    /// <summary>
    /// Mostrar la pantalla de combate.
    /// param[0] = int lnDistributionBoardIdUser - Es el id del tablero del usuario.
    /// param[1] = int lnDistributionBoardIdOpponent - Es el id del tablero del oponente.
    /// </summary>
    ShowUICombat,

    /// <summary>
    /// Mostrar la pantalla de pausa.
    /// </summary>
    ShowUIPause,

    /// <summary>
    /// Mostrar la pantalla de resultados.
    /// Se indica para avisar que ha terminado la partida indicando quien es el ganador.
    /// param[0] = bool lbUserWins - Bandera que nos indica si el usuario ha ganado.
    /// param[1] = bool lbOpponentWins - Bandera que nos indica si el oponente ha ganado.
    /// param[2] = Color loColorUser - Color del usuario.
    /// param[3] = Color loColorOpponent - Color del oponente.
    /// param[4] = UnityAction loOnReturn - callback que indica que salimos de la pantalla de resultados.
    /// </summary>
    ShowUIResults,

    /// <summary>
    /// Mostrar la pantalla de campamento.
    /// </summary>
    ShowUICamp,

    /// <summary>
    /// Mostrar la pantalla de emparejamiento para batalla.
    /// </summary>
    ShowUIMatching,

    /// <summary>
    /// Mostrar la pantalla de preparación de combate.
    /// </summary>
    ShowUICombatPreparation,

    /// <summary>
    /// Mostrar la pantalla para editar un string dado.
    /// param[0] = string lcString string el cual se editará.
    /// param[1] = UnityAction<string> loOnStringEdited - callback para cuando se haya editado el string.
    /// param[2] = string lcObjectName string del nombre del objeto que se debe modificar.
    /// param[3] = string lcPropertyName string del nombre de la propiedad del objeto que se debe modificar.
    /// param[4] = int lnObjectId int del id principal del objeto que se debe modificar.
    /// param[5] = string lcData string de algun dato que pueda ser de utilidad, por ejemplo cuando dos tablas comparte el mismo tipo de objeto aqui podemos indicar a cual de las tablas dirigir.
    /// </summary>
    ShowUIStringEditor,

    /// <summary>
    /// Se llama para mostrar el bondo de la interfaz.
    /// </summary>
    ShowUIBackground,
    #endregion

    #region Hide UI

    /// <summary>
    /// Se llama para ocultar la pantalla de carga.
    /// </summary>
    HideUIDownloading,

    /// <summary>
    /// Se llama para ocultar el bondo de la interfaz.
    /// </summary>
    HideUIBackground,

    #endregion

    #region MISC
    /// <summary>
    /// Se llama para pedir el lenguaje al sitio web.
    /// param[0] = UnityAction<bool, string> loCallbackJson - Es el callback que regresa el json dado por el sitio web.
    /// param[1] = DTO2<string, string>[] loPostParams - Son los parametros que se envian al sitio web para determinar si se debe mandar actualización del lenguaje o no.
    /// </summary>
    RequestLanguage,

    /// <summary>
    /// Se llama para avisar que ha cambiado el lenguaje.
    /// </summary>
    LanguageChanged,

    /// <summary>
    /// Se llama para reproducir un sonido.
    /// param[0] = SoundType loSoundType - Indica el tipo de sonido a reproducir.
    /// </summary>
    PlaySound,
    #endregion
}