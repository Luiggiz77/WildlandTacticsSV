public enum ClientCommand : int
{
    /// <summary>
    /// Indica que no hay comando
    /// </summary>
    None = 0,

    /// <summary>
    /// Indica que debemos reproducir la animacion de la unidad.
    /// </summary>
    PlayAnimation = 1,
    /// <summary>
    /// Indica que debemos detener la animación de la unidad.
    /// </summary>
    StopAnimation = 2,
    /// <summary>
    /// Indica que debemos reproducir la animacion de la unidad pero independientemente de que animación sea debemos aplicar rootmotion.
    /// </summary>
    PlayAnimationWithRootMotion = 3,

    /// <summary>
    /// Indica que debemos reproducir la animación adecuada para que la unidad rote a su nueva orientación.
    /// Nota: En caso de estár en la misma orientación no se reproduce nada.
    /// </summary>
    PlayFacingAnimation = 4,
    /// <summary>
    /// Indica que debemos actualizar la rotación de la unidad para estar orientado correctamente.
    /// Nota: Es una actualización inmediata sin animación.
    /// </summary>
    RotateUnitToMatchFacing = 5,

    /// <summary>
    /// Indica que debemos actualizar la posicion de la unidad con base en los datos de su posicion.
    /// </summary>
    RefreshUnitPosition = 6,

    /// <summary>
    /// Indica que debemos de aplicar el efecto de manera inmediata en las celdas del GameplayEffect con base en la celda seleccionada.
    /// </summary>
    ApplyGameplayEffect = 7,

    /// <summary>
    /// Indica que debemos desactivar la unidad por medio del evento "OnDeactivation" que está en el componente UnitInstance.
    /// </summary>
    DeactivateUnitEvent = 8,
    /// <summary>
    /// Indica que debemos modificar la visibilidad de una unidad usando en "Value" ya sea true = GameObject.SetActive(true); ó false = GameObject.SetActive(false);
    /// Nota: En caso de que "Value" no tenga valor se considera como "False".
    /// </summary>
    SetUnitVisibility = 9,
    /// <summary>
    /// Indica que debemos remover la funcionalidad de movimiento de la unidad.
    /// </summary>
    ClearOnUnitAnimatorMove = 10,
    /// <summary>
    /// Indica que debemos continuar de manera asincrona mandando el indice de la instruccion siguiente como inicio.
    /// Nota: Opcionalmente podemos mandar en "Value" un offset para determinar cuantas instrucciones pueden correrse de manera asincrona y seguir normalmente con las restantes.
    /// </summary>
    ContinueAsynchronous = 11,

    /// <summary>
    /// Indica que debemos asignar el valor al playable prefab.
    /// </summary>
    SetValuePlayablePrefab = 12,

    /// <summary>
    /// Indica que debemos asignar el valor que venga del efecto al playable prefab.
    /// </summary>
    SetValueFromEffectPlayablePrefab = 13,

    /// <summary>
    /// Indica que debemos forzar la animación en ves de usar un trigger.
    /// </summary>
    ForceAnimation = 14,

    /// <summary>
    /// Indica que debemos forzar la animación en ves de usar un trigger.
    /// </summary>
    ForceAnimationWithRootMotion = 15,

    /// <summary>
    /// Indica que debemos iniciar al animación en las unidades que estén el area de efecto.
    /// Nota: Se requiere pedir primero el area de efecto usando el TheaterCommand - "RequestGameplayEffectAreaCells"
    /// </summary>
    PlayAnimationOnGameplayEffectAreaUnits = 16,

    /// <summary>
    /// Indica que debemos detener al animación en las unidades que estén el area de efecto.
    /// Nota: Se requiere pedir primero el area de efecto usando el TheaterCommand - "RequestGameplayEffectAreaCells"
    /// </summary>
    StopAnimationOnGameplayEffectAreaUnits = 17,

    /// <summary>
    /// Indica que debemos reproducir la animación de un objetivo de una seccion de area.
    /// </summary>
    PlayAnimationOnTargetByAreaSection = 18,

    /// <summary>
    /// Indica que debemos esperar el final del frame.
    /// </summary>
    WaitForEndOfFrame = 19,
    /// <summary>
    /// Indica que debemos esperar por los segundos que indiquen en la propiedad "Value" del objeto UnitEventInstruction.
    /// </summary>
    WaitForSeconds = 20,
    /// <summary>
    /// Indica que debemos esperar a que la animación de la unidad active la bandera.
    /// </summary>
    WaitForAnimationFlag = 21,
    /// <summary>
    /// Indica que debemos esperar a que la animación de la unidad termine.
    /// </summary>
    WaitForAnimationEnd = 22,
    /// <summary>
    /// Indica que debemos esperar a que termine el movimiento de la unidad.
    /// Nota: Dentro de este comando se busca y llama el evento "UnitEventTimming.FirstHalfCell" (anidado) de existir.
    /// </summary>
    WaitForMoveEnd = 23,
    /// <summary>
    /// Indica que debemos esperar a que termine el push de la unidad.
    /// </summary>
    WaitForPushEnd = 24,
    /// <summary>
    /// Indica que debemos esperar a que el playable prefab haya terminado, se usa la bandera "Ended" en playable prefab.
    /// </summary>
    WaitForPlayablePrefabEnd = 25,
    /// <summary>
    /// Indica que debemos esperar a que el playable prefab se haya desactivado, se usa la bandera "gameObject.activeSelf".
    /// </summary>
    WaitForPlayablePrefabDeactivation = 26,

    /// <summary>
    /// Indica que debemos escalar la unidad que se encuentra en la celda con base en la valor que viene en la propiedad "Value".
    /// </summary>
    ScaleUnitInstance = 40,
    /// <summary>
    /// Indica que debemos escalar el primer hijo de la unidad (transform) que se encuentra en la celda con base en la valor que viene en la propiedad "Value".
    /// </summary>
    ScaleUnitInstanceFirstChild = 41,

    /// <summary>
    /// Indica que debemos crear un Playableprefab dado por el id establecido en la propiedad AssetBundleId.
    /// </summary>
    CreatePlayablePrefab = 50,
    /// <summary>
    /// Indica que debemos reproducir el Playableprefab dado por el id establecido en la propiedad "AssetBundleId". Nota: Debe crearse previamente usando "CreatePlayablePrefab" para ser reproducido.
    /// </summary>
    PlayPlayablePrefab = 51,
    /// <summary>
    /// Indica que debemos detener el Playableprefab se usa la propiedad "AssetBundleId" para indicar el id del Playableprefab que se detendrá.
    /// </summary>
    StopPlayablePrefab = 52,
    /// <summary>
    /// Indica que debemos remover el playable prefab de la lista de referencias ya que no requerimos mas del mismo.
    /// </summary>
    RemovePlayablePrefab = 53,
    /// <summary>
    /// Nos indica que debemos mostrar un efecto sin aplicar el play, como por ejemplo mostrar un proyectil de fuego pero sin aplicar su acción.
    /// </summary>
    PreviewPlayablePrefab = 54,

    /// <summary>
    /// Nos indican que debemos animar el playable prefab asignado a la posición almacenada en "Value" debe indicar el tiempo de la transición.
    /// Nota: Siempre se usa la curva eased in and out.
    /// </summary>
    AnimatePlayablePrefabTranslationToStoredPosition = 55,

    /// <summary>
    /// Indica que debemos convertir la posicion de mundo a posición de pantalla.
    /// Nota: Se usa regularmente para obtener la posición en mundo de elementos del UI.
    /// </summary>
    ConvertStoredPositionScreenToWorld = 56,

    /// <summary>
    /// Indica que debemos remover un GameplayEffect de la unidad.
    /// </summary>
    RemoveGameplayEffect = 57,

    /// <summary>
    /// Para reproducir la corrutina del Playableprefab dado por el id establecido en la propiedad "AssetBundleId". Nota: Es necesario crearlo previamente usando el comando "CreatePlayablePrefab".
    /// </summary>
    PlayPlayablePrefabCoroutine = 60,

    /// <summary>
    /// Para reproducir la corrutina del Playableprefab dado por el id establecido en la propiedad "AssetBundleId". Nota: Es necesario crearlo previamente usando el comando "CreatePlayablePrefab".
    /// Nota: Se llama usando "StartCoroutine"
    /// </summary>
    PlayPlayablePrefabCoroutineAsynchronous = 63,

    /// <summary>
    /// Para reproducir la corrutina del Playableprefab dado por el id establecido en la propiedad "AssetBundleId". Nota: Es necesario crearlo previamente usando el comando "CreatePlayablePrefab".
    /// Nota: Se usa para reproducir el playable prefab saltando entre secciones de objetivos (un area puede reproducirse por secciones) se usa el indice "lnChainSectionIndex" el cual nos indica la seccion del area que se está reproduciendo.
    /// Nota: Al terminar una sección se continua con la lista de "TheaterCommands" para por ejemplo reproducir animaciones de las unidades objetivo y despues continuar con la siguiente sección.
    /// </summary>
    PlayPlayablePrefabCoroutineBySection = 67,

    /// <summary>
    /// Indica que debemos anclar el Playableprefab de preparación al GameObject con el nombre especificado en "Value" del objeto UnitEventInstruction.
    /// </summary>
    AttachPlayablePrefab = 70,
    /// <summary>
    /// Indica que debemos desanclar el Playableprefab de preparación.
    /// </summary>
    DetachPlayablePrefab = 71,

    /// <summary>
    /// Indica que si hay algun elemento hijo en el nombre del transform dado en "Value" deberá ser desactivado.
    /// </summary>
    DeactivateAnyChildInTransform = 72,

    /// <summary>
    /// Indica que debemos emparentar el PlayblePrefab al UI.
    /// </summary>
    ParentPlayblePrefabToUI = 75,

    /// <summary>
    /// Indica que debemos mover el playable prefab hasta la posicion de la unidad que invocó dicha unidad en caso de ser así.
    /// </summary>
    TranslatePlayablePrefabToInvokerPosition = 77,

    /// <summary>
    /// Pone en cero la posicion y rotación local, también pone en 1 la escala.
    /// </summary>
    ResetLocalTransformPlayablePrefab = 79,

    /// <summary>
    /// Rota el Playableprefab en el eje X usando el valor especificado en "Value" del objeto UnitEventInstruction y el id establecido en la propiedad AssetBundleId.
    /// </summary>
    RotateXPlayablePrefab = 80,

    /// <summary>
    /// Rota el Playableprefab en el eje Y usando el valor especificado en "Value" del objeto UnitEventInstruction y el id establecido en la propiedad AssetBundleId.
    /// </summary>
    RotateYPlayablePrefab = 81,

    /// <summary>
    /// Rota el Playableprefab en el eje Z usando el valor especificado en "Value" del objeto UnitEventInstruction y el id establecido en la propiedad AssetBundleId.
    /// </summary>
    RotateZPlayablePrefab = 82,

    /// <summary>
    /// Rota el Playableprefab en el eje X usando el valor especificado en "Value" del objeto UnitEventInstruction y el id establecido en la propiedad AssetBundleId.
    /// </summary>
    TranslateXPlayablePrefab = 83,

    /// <summary>
    /// Rota el Playableprefab en el eje Y usando el valor especificado en "Value" del objeto UnitEventInstruction y el id establecido en la propiedad AssetBundleId.
    /// </summary>
    TranslateYPlayablePrefab = 84,

    /// <summary>
    /// Rota el Playableprefab en el eje Z usando el valor especificado en "Value" del objeto UnitEventInstruction y el id establecido en la propiedad AssetBundleId.
    /// </summary>
    TranslateZPlayablePrefab = 85,

    /// <summary>
    /// Escala el Playableprefab en el eje X usando el valor especificado en "Value" del objeto UnitEventInstruction y el id establecido en la propiedad AssetBundleId.
    /// </summary>
    ScaleXPlayablePrefab = 86,

    /// <summary>
    /// Escala el Playableprefab en el eje Y usando el valor especificado en "Value" del objeto UnitEventInstruction y el id establecido en la propiedad AssetBundleId.
    /// </summary>
    ScaleYPlayablePrefab = 87,

    /// <summary>
    /// Escala el Playableprefab en el eje Z usando el valor especificado en "Value" del objeto UnitEventInstruction y el id establecido en la propiedad AssetBundleId.
    /// </summary>
    ScaleZPlayablePrefab = 88,

    /// <summary>
    /// Escala el Playableprefab en el eje X,Y,Z usando el valor especificado en "Value" del objeto UnitEventInstruction y el id establecido en la propiedad AssetBundleId.
    /// </summary>
    ScaleXYZPlayablePrefab = 89,

    /// <summary>
    /// Indica que debemos mover el playable prefab a la posicion del hueso/tranform dado en la propiedad "Value".
    /// Nota: Value debe contener el nombre del transform/hueso.
    /// </summary>
    TranslatePlayablePrefabToTransformPosition = 90,
    /// <summary>
    /// Indica que debemos mover a mitad del camino (es decir de la posicion actual al siguiente transform/hueso a mitad del camino) el playable prefab a la posicion del hueso dado en la propiedad "Value".
    /// Nota: Value debe contener el nombre del transform/hueso.
    /// </summary>
    TranslatePlayablePrefabToTransformPositionHalfWay = 91,

    /// <summary>
    /// Indica que debemos mover el playable prefab a la posicion del root de la unidad.
    /// </summary>
    TranslatePlayablePrefabToUnitRootPosition = 92,

    /// <summary>
    /// Indica que debemos mover el playable prefab a la posicion de la celda a/o donde se llama.
    /// </summary>
    TranslatePlayablePrefabToCellPosition = 93,

    /// <summary>
    /// Indica que debemos mover el playable prefab a la posicion que se encuentre almacenada.
    /// </summary>
    TranslatePlayablePrefabToStoredPosition = 94,

    /// <summary>
    /// Indica que debemos mover el playable prefab al centro del tablero.
    /// </summary>
    TranslatePlayablePrefabToBoardCenter = 95,

    /// <summary>
    /// Indica que debemos mover el playable prefab a la posicion cero de manera local.
    /// </summary>
    TranslatePlayblePrefabToZeroLocalPosition = 96,

    /// <summary>
    /// Indica que debemos mover el playable prefab a la posicion del objetivo.
    /// </summary>
    TranslatePlayblePrefabToTargetPosition = 97,

    /// <summary>
    /// Indica que debemos rotar el playable prefab a la rotación cero de manera local.
    /// </summary>
    RotatePlayblePrefabToZeroLocalRotation = 98,

    /// <summary>
    /// Indica que debemos rotar el playable prefab a la direccion de su "Facing".
    /// </summary>
    RotatePlayablePrefabToUnitFacing = 99,



    /// <summary>
    /// Indica que debemos almacenar la posicion del hueso dado en la propiedad "Value".
    /// Nota: La posicion se almacena en una variable estatica para ser usada despues.
    /// </summary>
    StoreBonePosition = 100,

    /// <summary>
    /// Indica que debemos almacenar la posicion de un elemento del UI dado en la propiedad "Value".
    /// Nota: La posicion se almacena en una variable estatica para ser usada despues.
    /// </summary>
    StoreUIPosition = 101,

    /// <summary>
    /// Indica que debemos mover el playable prefab en la dirección frontal de la unidad, la propiedad "Value" indica el valor en el que debemos mover el playable prefab.
    /// </summary>
    TranslatePlayablePrefabToUnitForwardPosition = 102,

    /// <summary>
    /// Indica que pediremos el area de accion de un efecto y almacenaremos temporalmente ese valor para usarlo.
    /// </summary>
    RequestGameplayEffectAreaCells = 103,

    /// <summary>
    /// Indica que pediremos el area de accion de un efecto sin filtro por tipo de unidad y almacenaremos temporalmente ese valor para usarlo.
    /// </summary>
    RequestGameplayEffectAreaCellsNoFilter = 104,

    /// <summary>
    /// Indica que debemos pasarle el animator del personaje actual al playable prefab seleccionado atraves de "SetValue".
    /// </summary>
    SetValuePlayablePrefabAnimator = 105,

    /// <summary>
    /// Indica que debemos pasarle el trigger de la animación a realizar al playable prefab seleccionado atraves de "SetValue".
    /// </summary>
    SetValuePlayablePrefabAnimationTrigger = 106,

    /// <summary>
    /// Indica que debemos remover del effect area las unidades ocultas para no mostrar el efecto visual sobre las mismas.
    /// </summary>
    MaskHidenUnitsFromGameplayEffectAreaCells = 107,

    /// <summary>
    /// Indica que debemos agregar la unidad que llama el evento a la lista de unidades del area de efecto.
    /// </summary>
    AddCallerUnitToGameplayEffectAreaCells = 108,


    /// <summary>
    /// Rota el Playableprefab en el eje X usando el valor especificado en "Value" del objeto UnitEventInstruction y el id establecido en la propiedad AssetBundleId.
    /// Nota: Con base en la unidad la cual esta aplicando en su momento.
    /// </summary>
    TranslateXPlayablePrefabUnitBased = 109,

    /// <summary>
    /// Rota el Playableprefab en el eje Y usando el valor especificado en "Value" del objeto UnitEventInstruction y el id establecido en la propiedad AssetBundleId.
    /// Nota: Con base en la unidad la cual esta aplicando en su momento.
    /// </summary>
    TranslateYPlayablePrefabUnitBased = 110,

    /// <summary>
    /// Rota el Playableprefab en el eje Z usando el valor especificado en "Value" del objeto UnitEventInstruction y el id establecido en la propiedad AssetBundleId.
    /// Nota: Con base en la unidad la cual esta aplicando en su momento.
    /// </summary>
    TranslateZPlayablePrefabUnitBased = 111,

    /// <summary>
    /// Rota el Playableprefab en el eje X usando el valor especificado en "Value" del objeto UnitEventInstruction y el id establecido en la propiedad AssetBundleId.
    /// Nota: Con base en la unidad que llama el evento.
    /// </summary>
    TranslateXPlayablePrefabCallerUnitBased = 112,

    /// <summary>
    /// Rota el Playableprefab en el eje Y usando el valor especificado en "Value" del objeto UnitEventInstruction y el id establecido en la propiedad AssetBundleId.
    /// Nota: Con base en la unidad que llama el evento.
    /// </summary>
    TranslateYPlayablePrefabCallerUnitBased = 113,

    /// <summary>
    /// Rota el Playableprefab en el eje Z usando el valor especificado en "Value" del objeto UnitEventInstruction y el id establecido en la propiedad AssetBundleId.
    /// Nota: Con base en la unidad que llama el evento.
    /// </summary>
    TranslateZPlayablePrefabCallerUnitBased = 114,

    /// <summary>
    /// Indica que debemos obtener un transform de la unidad.
    /// Nota: En "Value" debemos pasar el nombre del GameObject del que se pedirá el transform, en caso de no existir se usa el root.
    /// Nota: Se almacena el transform en goStoredTransform.
    /// </summary>
    StoreUnitTransform = 115,

    /// <summary>
    /// Indica que debemos obtener un transform de la unidad que llama el evento.
    /// Nota: En "Value" debemos pasar el nombre del GameObject del que se pedirá el transform, en caso de no existir se usa el root.
    /// Nota: Se almacena el transform en goStoredTransform.
    /// </summary>
    StoreUnitCallerTransform = 116,

    /// <summary>
    /// Indica que debemos obtener un transform del UI.
    /// Nota: En "Value" debemos pasar el nombre del GameObject del que se pedirá el transform.
    /// Nota: Se almacena el transform en goStoredTransform.
    /// </summary>
    StoreUITransform = 117,

    /// <summary>
    /// Indica que debemos obtener un transform de la unidad que llama el evento.
    /// Nota: Se almacena el transform en goStoredTransform.
    /// </summary>
    StorePlayablePrefabTransform = 118,

    /// <summary>
    /// Indica que debemos almacenar la posicion del StoredTransform en el StoredPosition.
    /// </summary>
    StoreStoredTransformPosition = 119,

    /// <summary>
    /// Indica que debemos almacenar la posicion actual de la unidad.
    /// </summary>
    StoreUnitPosition = 120,

    /// <summary>
    /// Indica que debemos almacenar la posicion actual de la unidad que llama el evento.
    /// </summary>
    StoreUniCallertPosition = 121,

    /// <summary>
    /// Rota el Playableprefab en el eje X usando el valor especificado en "Value" del objeto UnitEventInstruction y el id establecido en la propiedad AssetBundleId.
    /// Nota: Con base en la unidad la cual esta aplicando en su momento.
    /// </summary>
    TranslateXStoredPositionUnitBased = 122,

    /// <summary>
    /// Rota el Playableprefab en el eje Y usando el valor especificado en "Value" del objeto UnitEventInstruction y el id establecido en la propiedad AssetBundleId.
    /// Nota: Con base en la unidad la cual esta aplicando en su momento.
    /// </summary>
    TranslateYStoredPositionUnitBased = 123,

    /// <summary>
    /// Rota el Playableprefab en el eje Z usando el valor especificado en "Value" del objeto UnitEventInstruction y el id establecido en la propiedad AssetBundleId.
    /// Nota: Con base en la unidad la cual esta aplicando en su momento.
    /// </summary>
    TranslateZStoredPositionUnitBased = 124,

    /// <summary>
    /// Rota el Playableprefab en el eje X usando el valor especificado en "Value" del objeto UnitEventInstruction y el id establecido en la propiedad AssetBundleId.
    /// Nota: Con base en la unidad que llama el evento.
    /// </summary>
    TranslateXStoredPositionCallerUnitBased = 125,

    /// <summary>
    /// Rota el Playableprefab en el eje Y usando el valor especificado en "Value" del objeto UnitEventInstruction y el id establecido en la propiedad AssetBundleId.
    /// Nota: Con base en la unidad que llama el evento.
    /// </summary>
    TranslateYStoredPositionCallerUnitBased = 126,

    /// <summary>
    /// Rota el Playableprefab en el eje Z usando el valor especificado en "Value" del objeto UnitEventInstruction y el id establecido en la propiedad AssetBundleId.
    /// Nota: Con base en la unidad que llama el evento.
    /// </summary>
    TranslateZStoredPositionCallerUnitBased = 127,

    /// <summary>
    /// Rota el Playableprefab en el eje X usando el valor especificado en "Value" del objeto UnitEventInstruction y el id establecido en la propiedad AssetBundleId.
    /// Nota: En la posición en mundo.
    /// </summary>
    TranslateXStoredPosition = 128,

    /// <summary>
    /// Rota el Playableprefab en el eje Y usando el valor especificado en "Value" del objeto UnitEventInstruction y el id establecido en la propiedad AssetBundleId.
    /// Nota: En la posición en mundo.
    /// </summary>
    TranslateYStoredPosition = 129,

    /// <summary>
    /// Rota el Playableprefab en el eje Z usando el valor especificado en "Value" del objeto UnitEventInstruction y el id establecido en la propiedad AssetBundleId.
    /// Nota: En la posición en mundo.
    /// </summary>
    TranslateZStoredPosition = 130,

    /// <summary>
    /// Indica que debemos mostrar un mensaje de que la unidad del evento se ha movido.
    /// </summary>
    ShowMessageUnitMoved = 250,

    /// <summary>
    /// Indica que debemos mostrar un mensaje de que hemos recivido un item.
    /// </summary>
    ShowMessageReceivedItem = 251,

    /// <summary>
    /// Indica que debemos pedir el icono de un item.
    /// Nota; La propiedad "Value" indica el id del item obtenido cuando viene del servidor por ahora se pone uno aleatorio. TODO: Asignar value en servidor.
    /// </summary>
    CreateBillboardGameplayItemIcon = 252,

    /// <summary>
    /// Indca que debemos asignarle de padre el playable prefab dado por la propiedad dada en AssetBundleId
    /// </summary>
    SetPlayablePrefabAsBillboardIconParent = 253,

    /// <summary>
    /// Indica que nuestra unidad copiará la animación actual de quien la invocó.
    /// </summary>
    MimeInvokerAnimation = 254,

    /// <summary>
    /// Se llama para activar o desactivar los renderers de la unidad.
    /// </summary>
    SetUnitRenderersVisibility = 255,

    /// <summary>
    /// Indica que debemos actualizar la orientación del quien invocó la unidad.
    /// </summary>
    RotateInvokerUnitToMatchFacing = 256,

    /// <summary>
    /// Indica que debemos actualizar la posicion de quien invocó la unidad.
    /// </summary>
    RefreshInvokerUnitPosition = 257,

    #region Sound
    /// <summary>
    /// Se llama para reproducir un sonido de una sola ocación. (OneShot)
    /// </summary>
    PlaySound = 301,
    /// <summary>
    /// Se llama para reproducir un sonido en modo loop en el canal 0.
    /// </summary>
    PlaySound_C0 = 302,
    /// <summary>
    /// Se llama para pausar un sonido en modo loop en el canal 0.
    /// </summary>
    PauseSound_C0 = 303,
    /// <summary>
    /// Se llama para detener un sonido en modo loop en el canal 0.
    /// </summary>
    StopSound_C0 = 304,
    /// <summary>
    /// Se llama para reproducir un sonido en modo loop en el canal 1.
    /// </summary>
    PlaySound_C1 = 305,
    /// <summary>
    /// Se llama para pausar un sonido en modo loop en el canal 1.
    /// </summary>
    PauseSound_C1 = 306,
    /// <summary>
    /// Se llama para detener un sonido en modo loop en el canal 1.
    /// </summary>
    StopSound_C1 = 307,
    #endregion

    #region STORED_TRANSFORM_ANIMATION
    /// <summary>
    /// Se llama para agregar un vector de animación, se usa lo siguiente: (p or s or r),n,x,y,z "p,0,0,1.3,0" otro ejemplo "p,1,0,0,0".
    /// El primer elemento nos indica que se va a animar, posicion (p), escala (s) o rotación (r).
    /// El segundo elemento nos indica el lugar del vector, es decir se usa un valor normalizado de 0 a 1, tomando que el 1 es el fin. Por ejemplo podemos usar 0.5 indicando que dicho vector va a la mitad.
    /// El tercero, cuarto y quinto son los datos del vector.
    /// En caso de no dar start (n 0) o end (n 1) es tomado del transform el cual se obtiene del nombre dado al usar por ejemplo el comando "AnimateLinearStoredVectorsOnTransform"
    /// </summary>
    AddStoredVector = 400,
    /// <summary>
    /// Limpia la lista de vectores almacenados.
    /// </summary>
    ClearStoredVectors = 401,
    /// <summary>
    /// Indica que debemos animar usando los vectores almacenados de manera lineal.
    /// Nota: Debemos pasar en value el tiempo de la siguiente manera, ej. "0.5"
    /// Nota: Antes de esto debemos llamar RequesUnitTransform para que se almacene en 'goSotredTransform'
    /// </summary>
    AnimateStoredVectorsOnStoredTransformLinear = 402,
    /// <summary>
    /// Indica que debemos animar usando los vectores almacenados usando una curva eased in and out.
    /// Nota: Debemos pasar en value el tiempo de la siguiente manera, ej. "0.5"
    /// Nota: Antes de esto debemos llamar RequesUnitTransform para que se almacene en 'goSotredTransform'
    /// </summary>
    AnimateStoredVectorsOnStoredTransformEased = 403,
    /// <summary>
    /// Indica que debemos animar usando los vectores almacenados usando una curva que está en el GameManager.
    /// Nota: Debemos pasar en value el tiempo de la siguiente manera, ej. "0.5"
    /// Nota: Antes de esto debemos llamar RequesUnitTransform para que se almacene en 'goSotredTransform'
    /// </summary>
    AnimateStoredVectorsOnStoredTransformGameManagerCurve = 404,

    /// <summary>
    /// Indica que debemos agregar la posición almacenada a nuestros "StoredVectors" como una posición más.
    /// Usamos "Value" que nos indica el lugar del vector, es decir se usa un valor normalizado de 0 a 1, tomando que el 1 es el fin. Por ejemplo podemos usar 0.5 indicando que dicho vector va a la mitad.
    /// </summary>
    AddStoredPositionToStoredVectors = 497,

    /// <summary>
    /// Indica que debemos convertir los vectores almacenados de posición a una curva de bezier.
    /// Nota: Usamos "Value" para indicar la cantidad de puntos extra usado para la curva. ej. "3"
    /// </summary>
    ConvertPositionStoredVectorsToBezier = 498,

    /// <summary>
    /// Indica que debemos reiniciar el transform a 1, debemos pasar el nombre del transform el cual debemos reiniciar. ej. "Body"
    /// </summary>
    ResetStoredTransform = 499,
    #endregion

    #region IF

    /// <summary>
    /// Indica que debemos pasar a la siguiente seccion del area.
    /// </summary>
    IncrementAreaSection = 500,

    /// <summary>
    /// Indica que si no se han completado todas las secciones de area debemos regresar N comandos, la cantidad de comandos a regresar se toman del "value".
    /// "Validación"
    /// </summary>
    IfAreaSectionsNotCompletedReturnNCommands = 501,

    /// <summary>
    /// Indica que si el GameplayTarget SI está en la celda debemos saltar las instrucciones.
    /// Nota: Indica que Value se debe interpretar como "GameplayTarget"
    /// </summary>
    SkipInstructionsIfGameplayTargetIsInCell = 502,

    /// <summary>
    /// Indica que si el GameplayTarget NO está en la celda debemos saltar las instrucciones.
    /// Nota: Indica que Value se debe interpretar como "GameplayTarget"
    /// </summary>
    SkipInstructionsIfGameplayTargetIsNotInCell = 503,

    /// <summary>
    /// Indica que si la condición dada se cumple debemos ignorar las intrucciones posteriores.
    /// Nota: Indica que Value se debe interpretar como "InstructionCondition"
    /// </summary>
    SkipInstructionsIfInstructionCondition = 504,

    /// <summary>
    /// Indica que Value se debe interpretar como "GameplayTarget" y que si quien llama el evento no es objetivo del "GameplayTarget" debemos ignorar las instrucciones posteriores.
    /// Nota: Indica que Value se debe interpretar como "GameplayTarget"
    /// </summary>
    SkipInstructionsIfCallerIsGameplayTarget = 505,

    /// <summary>
    /// Indica que Value se debe interpretar como "GameplayTarget" y que si quien llama el evento no es objetivo del "GameplayTarget" debemos ignorar las instrucciones posteriores.
    /// Nota: Indica que Value se debe interpretar como "GameplayTarget"
    /// </summary>
    SkipInstructionIfCallerIsNotGameplayTarget = 506,

    /// <summary>
    /// Indica que debemos determinar si hay critical hit con base en la instrucción de daño y almacenamos la bandera del critico.
    /// </summary>
    EvaluateCriticalFromInstructions = 507,

    /// <summary>
    /// Indica que debemos determinar si hay critical hit con base en la posición de espalda del atacado y almacenamos la bandera del critico (solo aplica si es por la espalda y no usa el randomness).
    /// </summary>
    EvaluateCriticalFromBackstab = 508,

    /// <summary>
    /// En caso de que se haya determinado un SI critico la bandera nos indica que debemos ignorar la cantidad de instrucciones dadas en "Value".
    /// </summary>
    SkipInstructionsIfCritical = 509,

    /// <summary>
    /// En caso de que se haya determinado un NO critico la bandera nos indica que debemos ignorar la cantidad de instrucciones dadas en "Value".
    /// </summary>
    SkipInstructionsIfNotCritical = 510,

    /// <summary>
    /// El comando nos indica que debemos saltarnos N instrucciones si es para el oponente. Nota: Debemos ignorar la cantidad de instrucciones dadas en "Value".
    /// </summary>
    SkipInstructionsIfOpponent = 511,

    /// <summary>
    /// El comando nos indica que debemos saltarnos N instrucciones si no es para el oponente. Nota: Debemos ignorar la cantidad de instrucciones dadas en "Value".
    /// </summary>
    SkipInstructionsIfNotOpponent = 512,

    /// <summary>
    /// El comando nos indica que debemos saltarnos N instrucciones si el efecto SI está aplicado en la unidad, se usa el GameplayEffectId para verificar. Nota: Debemos ignorar la cantidad de instrucciones dadas en "Value".
    /// </summary>
    SkipInstructionsIfGameplayEffectIdExists = 513,

    /// <summary>
    /// El comando nos indica que debemos saltarnos N instrucciones si el efecto NO está aplicado en la unidad, se usa el GameplayEffectId para verificar. Nota: Debemos ignorar la cantidad de instrucciones dadas en "Value".
    /// </summary>
    SkipInstructionsIfGameplayEffectIdNotExists = 514,

    /// <summary>
    /// El comando nos indica que debemos saltarnos N instrucciones si el efecto fue agregado en la unidad, se usa el GameplayEffectId para verificar. Nota: Debemos ignorar la cantidad de instrucciones dadas en "Value".
    /// </summary>
    SkipInstructionsIfGameplayEffectIdAdded = 515,

    /// <summary>
    /// El comando nos indica que debemos saltarnos N instrucciones si el efecto fue removido de la unidad, se usa el GameplayEffectId para verificar. Nota: Debemos ignorar la cantidad de instrucciones dadas en "Value".
    /// </summary>
    SkipInstructionsIfGameplayEffectIdRemoved = 516,

    /// <summary>
    /// El comando nos indica que debemos saltarnos N instrucciones si el efecto NO fue agregado en la unidad, se usa el GameplayEffectId para verificar. Nota: Debemos ignorar la cantidad de instrucciones dadas en "Value".
    /// </summary>
    SkipInstructionsIfGameplayEffectIdNotAdded = 517,

    /// <summary>
    /// El comando nos indica que debemos saltarnos N instrucciones si el efecto NO fue removido de la unidad, se usa el GameplayEffectId para verificar. Nota: Debemos ignorar la cantidad de instrucciones dadas en "Value".
    /// </summary>
    SkipInstructionsIfGameplayEffectIdNotRemoved = 518,

    /// <summary>
    /// El comando nos indica que debemos saltarnos N instrucciones cuando la energia de quien llama SI está llena.
    /// </summary>
    SkipInstructionsIfEnergyFull = 519,

    /// <summary>
    /// El comando nos indica que debemos saltarnos N instrucciones cuando la energia de quien llama NO está llena.
    /// </summary>
    SkipInstructionsIfEnergyNotFull = 520,

    #endregion

    #region RENDERER_MATERIAL
    /// <summary>
    /// Indica que debemos establecer el color del material como el color que viene en la barra de vida.
    /// Nota: "Value" debe contener el nombre del Transform que contiene el renderer al cual debemos asignar el color.
    /// </summary>
    SetRendererMaterialColorAsHealthbarColor = 601,
    #endregion

    #region SHADER
    /// <summary>
    /// Indica que deseamos cambiar una propiedad del shader de todos los materiales usados en la unidad.
    /// Nota: La propiedad "Value" debe indicar 3 propiedades separadas por un signo de dinero de la siguiente manera:
    /// Propiedad 0: Nombre de la propiedad a modificar. Nota: Debe coincidir con el nombre en el shader mayusculas y minusculas.
    /// Propiedad 1: Tipo de valor a establecer. Nota: Los valores son "float", "vector", "color", y deben estár en minisculas.
    /// Propiedad 2: Valor para asignar. Nota: Los valores son "float" > 0.25, "vector" > (0.5,1.7,3.4), "color" > #FFACDEFF
    /// Ejemplos: _RimColor$color$#FFFFFFFF _Rim$float&1.0 _RimDirection$vector$(0.3,0.7,0.5)
    /// Nota: No se pueden asignar texturas.
    /// Nota: Se usa directo sobre el material por lo tanto se crea una copia y por ende ineficiente.
    /// </summary>
    SetShaderProperty = 5100,
    /// <summary>
    /// Indica que deseamos cambiar una propiedad del shader de todos los materiales usados en la unidad.
    /// Nota: La propiedad "Value" debe indicar 3 propiedades separadas por un signo de dinero de la siguiente manera:
    /// Propiedad 0: Nombre de la propiedad a modificar. Nota: Debe coincidir con el nombre en el shader mayusculas y minusculas.
    /// Propiedad 1: Tipo de valor a establecer. Nota: Los valores son "float", "vector", "color", y deben estár en minisculas.
    /// Propiedad 2: Valor para asignar. Nota: Los valores son "float" > 0.25, "vector" > (0.5,1.7,3.4), "color" > #FFACDEFF
    /// Ejemplos: _RimColor$color$#FFFFFFFF _Rim$float&1.0 _RimDirection$vector$(0.3,0.7,0.5)
    /// Nota: No se pueden asignar texturas.
    /// Nota: Usa MaterialPropertyBlock para pasar los datos.
    /// </summary>
    SetShaderPropertyMPB = 5101,
    /// <summary>
    /// Indica que debemos cambiar el render queue de los materiales en la unidad.
    /// Se da el valor del render queue para asignar en la propiedad "Value".
    /// </summary>
    SetMaterialRenderQueue = 5102,

    /// <summary>
    /// Indica que debemos habilitar un keyword en el material.
    /// Se da el nombre en la propiedad "Value".
    /// </summary>
    MaterialEnableKeyword = 5103,

    /// <summary>
    /// Indica que debemos deshabilitar un keyword en el material.
    /// Se da el nombre en la propiedad "Value".
    /// </summary>
    MaterialDisableKeyword = 5104,

    #endregion
}