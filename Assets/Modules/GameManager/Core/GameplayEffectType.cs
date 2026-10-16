public enum GameplayEffectType : int
{
    /// <summary>
    /// Indica que no tiene efecto.
    /// </summary>
    None = 0,

    #region UNIT
    /// <summary>
    /// Cambia los puntos de vida. Nota: El valor nos indica en cuanto incrementa o decrementa la vida de la unidad.
    /// </summary>
    UnitHealth = 1,

    /// <summary>
    /// Cambia la vida maxima. Nota: El valor nos indica cuanto incrementa o decrementa la vida maxima de la unidad.
    /// </summary>
    UnitMaxHealth = 2,

    /// <summary>
    /// Cambia los puntos de daño magico. Nota: El valor nos indica en cuanto incrementa o decrementa el daño magico de la unidad.
    /// </summary>
    UnitMagicalPower = 3,

    /// <summary>
    /// Indica que agrega puntos de resistenia magica a una unidad seleccionada. Nota: El valor nos indica en cuanto incrementa resistencia magica de la unidad.
    /// </summary>
    UnitMagicalResistance = 4,

    /// <summary>
    /// Cambia el daño físico. Nota: El valor nos indica en cuanto incrementa o decrementa el daño físico de las unidades.
    /// </summary>
    UnitPhysicalPower = 5,

    /// <summary>
    /// Cambia la resistenia física. Nota: El valor nos indica en cuanto incrementa o decrementa resistencia física de la unidad.
    /// </summary>
    UnitPhysicalResistance = 6,

    /// <summary>
    /// Cambia los puntos de movimiento. Nota: El valor nos indica en cuanto incrementa o decrementa el movimiento de la unidad.
    /// </summary>
    UnitMovement = 7,

    /// <summary>
    /// Cambia los puntos de rango de ataque. Nota: El valor nos indica en cuanto incrementa o decrementa el rango de ataque de la unidad.
    /// </summary>
    UnitAttackRange = 8,

    /// <summary>
    /// Cambia la cantidad de ataques. Nota: El valor nos indica en cuanto incrementan o decrementan la cantidad de ataques que puede realizar una unidad.
    /// </summary>
    UnitAttackAction = 9,

    /// <summary>
    /// Cambia la cantidad de movimiento. Nota: El valor nos indica en cuanto incrementan o decrementan la cantidad de movimientos que puede realizar una unidad.
    /// </summary>
    UnitMoveAction = 10,

    /// <summary>
    /// Cambia los puntos de vida. Nota: El valor se usa como multiplicador (Positivo o negativo) sobre el daño físico de la unidad que llama el efecto.
    /// </summary>
    UnitHealhtUsingCallerPhysicalPower = 11,

    /// <summary>
    /// Cambia los puntos de vida. Nota: El valor se usa como multiplicador (Positivo o negativo) sobre el daño mágico de la unidad que llama el efecto.
    /// </summary>
    UnitHealthUsingCallerMagicalPower = 12,

    /// <summary>
    /// Cambia los puntos de vida. Nota: El valor se usa como el daño físico.
    /// </summary>
    UnitHealhtUsingFixedPhysicalPower = 13,

    /// <summary>
    /// Cambia los puntos de vida. Nota: El valor se usa como el daño mágico.
    /// </summary>
    UnitHealthUsingFixedMagicalPower = 14,

    /// <summary>
    /// Cambia los puntos de vida. Nota: El valor se usa como multiplicador (Positivo o negativo) sobre el daño mágico de la unidad que llama el efecto y también el area/distancia para realizar el calculo. Se usa el Aux00 para limitar.
    /// </summary>
    UnitHealthUsingAreaAndCapUsingAux = 15,

    /// <summary>
    /// Cambia los puntos de vida a la mitad cuando es un jefe, de lo contrario mata a la unidad.
    /// </summary>
    UnitHealthByHalfWhenBoss = 16,

    /// <summary>
    /// Cambia los puntos de vida. Nota: Indica que al cambiar los puntos de vida se deben ignorar las animaciones.
    /// </summary>
    UnitHealthNoAnimation = 17,

    /// <summary>
    /// Indica que se le asigna una probabilidad de ataque, es decir un porcentage ya sea positivo o negativo (para errar ataques).
    /// </summary>
    UnitAttackProbability = 18,

    /// <summary>
    /// Cambia los puntos de vida. Nota: El valor se usa como multiplicador (Positivo o negativo) sobre la cantidad de 'Energy' que tiene la unidad y también se remueve la cantidad de energia usada.
    /// </summary>
    UnitHealthUsingEnergy = 19,

    /// <summary>
    /// Cambia los puntos de energia de la unidad. Nota: El valor nos indica en cuanto incrementa o decrementa la energia de la unidad.
    /// </summary>
    UnitEnergy = 20,

    /// <summary>
    /// Cambia la energia maxima. Nota: El valor nos indica cuanto incrementa o decrementa la energia maxima de la unidad.
    /// </summary>
    UnitMaxEnergy = 21,

    /// <summary>
    /// Se llama para destruir ésta unidad quien tiene el efecto, basicamente se usa para cuando una unidad debe ser unica en el tablero, los clones deben morir al crearse una nueva del mismo tipo y bando.
    /// </summary>
    DeathOnClonedGameplayUnitAlly = 24,

    /// <summary>
    /// Se llama para ocultar la unidad en el mapa.
    /// </summary>
    UnitHide = 25,

    /// <summary>
    /// Se llama para destruir la unidad, es decir se remueve del mapa permanentemente.
    /// </summary>
    Death = 26,

    /// <summary>
    /// Se llama para destruir la unidad quien llama el efecto, es decir se remueve del mapa permanentemente cuando la cuenta de turnos llega a cero. Nota: Se usa en efectos agregados a la unidad.
    /// </summary>
    DeathOnExecutionsEnd = 27,

    /// <summary>
    /// Cambia los puntos de movimiento a puntos fijos. Nota: Nos indica a cuanto debe establecerse el movimiento de la unidad.
    /// </summary>
    FixedUnitMovement = 28,

    /// <summary>
    /// Cambia la orientación de la unidad.
    /// </summary>
    UnitFacing = 29,

    /// <summary>
    /// Indica que asigna el primer turno a quien usa el objeto. Nota: El valor es ignorado.
    /// </summary>
    FirstGameplayTurn = 30,

    /// <summary>
    /// Indica que la unidad seleccionada no tiene ningun efecto y no se le puede hacer daño. Nota: El valor es ignorado.
    /// </summary>
    Invulnerability = 31,

    /// <summary>
    /// Indica que realiza un impacto directo a la unidad enemiga seleccionada ignorando armaduras. Nota: El valor indica la cantidad de daño.
    /// </summary>
    DirectImpact = 32,

    /// <summary>
    /// Indica que se invoca una unidad de juego. Nota: El valor es el id de la unidad que se llamará.
    /// </summary>
    InvokeGameplayUnit = 33,

    /// <summary>
    /// Indica que destruye las unidades de juego. Nota: Se tomará en cuenta el Target para la destruccion de las unidades del gameplay.
    /// </summary>
    DestroyGameplayUnit = 34,

    /// <summary>
    /// Indica se revivira la unidad muerta. Nota: El valor indica cuantos puntos de vida se les debe de dar al revivir.
    /// </summary>
    ReviveUnit = 35,

    /// <summary>
    /// Cambia la cantidad de acciones que puede hacer un jugador en su turno. Nota: El valor nos indica en cuanto incrementan o decrementan la cantidad de acciones que puede realizar un jugador en su turno.
    /// </summary>
    ChangeActions = 36,

    /// <summary>
    /// Indica que se invoca un efecto de juego, solo se usa atraves del trigger. Nota: Se usa el valor del Id del GameplayEffect en el objeto.
    /// </summary>
    InvokeGameplayEffectOnTrigger = 37,

    /// <summary>
    /// Indica que debemos mostrar elementos ocultos en el area.
    /// </summary>
    ShowHidden = 38,

    /// <summary>
    /// Indica que debemos dar un objeto temporal al jugador a quien pertenece quien llama el efecto.
    /// </summary>
    GiveTemporaryItem = 39,

    /// <summary>
    /// Indica que teletransportamos la unidad a la celda objetivo.
    /// </summary>
    TeleportUnitToTargetCell = 40,

    /// <summary>
    /// Indica que se invoca un evento de unidad. Nota: Se usa el valor del Id del UnitEvent en el objeto, en caso de no haber se usa UnitEventType.
    /// </summary>
    InvokeUnitEvent = 41,

    /// <summary>
    /// Indica que se invoca un efecto de juego. Nota: Se usa el valor del Id del GameplayEffect en el objeto.
    /// </summary>
    InvokeGameplayEffect = 42,

    /// <summary>
    /// Indica que la unidad tiene counter por ello se aplica su evento de counter.
    /// </summary>
    Counter = 43,

    /// <summary>
    /// Cambia los puntos de vida. Nota: El valor nos indica en cuanto incrementa o decrementa la vida de la unidad pero tiene la posibilidad de no ser critico usando 'CriticalRandomness' para detrminar si aplica o no.
    /// </summary>
    UnitHealthCritical = 44,

    /// <summary>
    /// Intercambia la posicion de la unidad la unidad que invocó ésta unidad.
    /// </summary>
    SwitchInvokerPosition = 45,

    /// <summary>
    /// Intercambia la orientación de la unidad la unidad que invocó ésta unidad.
    /// </summary>
    SwitchInvokerFacing = 46,

    /// <summary>
    /// Intercambia la posicion y orientación de la unidad la unidad que invocó ésta unidad.
    /// </summary>
    SwitchInvokerPositionAndFacing = 47,

    /// <summary>
    /// Indica que se invoca un efecto de juego sobre la unidad que invocó nuestra unidad de ser así. Nota: Se usa el valor del Id del GameplayEffect en el objeto.
    /// </summary>
    InvokeGameplayEffectOnInvoker = 48,
    #endregion

    #region UNIT ROTATION
    /// <summary>
    /// Cambia la orientación de la unidad para ver al frente.
    /// </summary>
    RotateUnitToForward = 50,
    /// <summary>
    /// Indica que debemos rotar la unidad para que se oriente en dirección de la unidad mas cercana del oponente.
    /// </summary>
    RotateUnitToNearestOpponentUnit = 51,
    /// <summary>
    /// Indica que debemos rotar la unidad para que se oriente en dirección de la unidad objetivo.
    /// </summary>
    RotateUnitToTarget = 52,
    /// <summary>
    /// Indica que debemos rotar la unidad para que se oriente en dirección de la unidad que llama el evento.
    /// </summary>
    RotateUnitToCaller = 53,
    /// <summary>
    /// Indica que debemos rotar la unidad en la misma dirección en la que el creador de la unidad.
    /// </summary>
    RotateUnitTowardsUnitCreatorFacing = 54,
    #endregion

    #region UNIT EVENT INVOCATION
    /// <summary>
    /// Indica que se invoca el evento de ataque solo si la unidad a la que se ataca es del oponente.
    /// </summary>
    InvokeUnitEventAttack = 101,

    /// <summary>
    /// Indica que se invoca el evento en la unidad que nos invocó con base en el UnitEventId de existir de lo contrario se usa UnitEvenType.
    /// </summary>
    InvokeUnitEventOnInvoker = 102,

    /// <summary>
    /// Indica que se invoca el evento de empuje de manera asincrona.
    /// </summary>
    InvokeUnitEventPushAsync = 150,
    #endregion

    #region UNIT CHOOSE TARGET
    /// <summary>
    /// Indica que debemos elegir un objetivo aleatorio con base en las propiedades de batalla de la unidad, las coordenadas del objetivo se almacenan en el UnitGameplayEffect.
    /// </summary>
    ChooseRandomTargetToAttack = 201,
    #endregion

    #region UNIT DEFEND
    /// <summary>
    /// Indica que esta unidad defiende a la unidad que recibe daño fisico, es decir en ves de que la unidad original reciba daño esta unidad lo recibirá por ella.
    /// </summary>
    DefendUnitFromPhysicalHarm = 300,

    /// <summary>
    /// Indica que esta unidad defiende a la unidad que recibe daño magico, es decir en ves de que la unidad original reciba daño esta unidad lo recibirá por ella.
    /// </summary>
    DefendUnitFromMagicalHarm = 301,
    #endregion
}