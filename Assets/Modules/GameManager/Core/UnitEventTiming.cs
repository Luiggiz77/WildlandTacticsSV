using System;

/// <summary>
/// Indica el tipo de evento que realiza la unidad.
/// </summary>
[Flags]
public enum UnitEventTiming
{
    /// <summary>
    /// Indica que no es ningun tipo de evento.
    /// </summary>
    None = 0,

    /// <summary>
    /// Indica que se llama al iniciar el turno.
    /// </summary>
    StartTurn = 1,

    /// <summary>
    /// Indica que se llama al terminar el turno.
    /// </summary>
    EndTurn = 2,

    /// <summary>
    /// Indica que se llama al moverse.
    /// </summary>
    Move = 4,

    /// <summary>
    /// Se llama al atacar.
    /// </summary>
    Attack = 8,

    /// <summary>
    /// Se llama al recibir daño.
    /// </summary>
    ReceiveDamage = 16,

    /// <summary>
    /// Se llama al morir la unidad.
    /// </summary>
    Death = 32,

    /// <summary>
    /// Indica que se movió a media celda.
    /// </summary>
    FirstHalfCell = 64,

    /// <summary>
    /// Indica que nuestra unidad chocó contra otra unidad en el camino y que somos el emisor.
    /// </summary>
    CollideEmitter = 128,

    /// <summary>
    /// Indica que chocó contra nuestra unidad en el camino y que somos el receptor.
    /// </summary>
    CollideReceiver = 256,

    /// <summary>
    /// Indica que pisó el elemento. Regularmente se usa en "unidades" trampa.
    /// </summary>
    StepIn = 512,

    /// <summary>
    /// Indica que se ha creado nuestra unidad en el tablero.
    /// </summary>
    Creation = 1024,

    /// <summary>
    /// Indica que dejo de pizar el elemento. Regularmente se usa en "unidades" trampa/efecto.
    /// </summary>
    StepOut = 2048,

    /// <summary>
    /// Indica que se removió la unidad.
    /// </summary>
    Remove = 4096,

    /// <summary>
    /// Indica que se le quiere aplicar un evento negativo a la unidad pero no pasa puesto que es invulnerable.
    /// </summary>
    Invulnerable = 8192,

    /// <summary>
    /// Indica que es al inicio de la batalla.
    /// </summary>
    CombatStart = 16384,

    /// <summary>
    /// Indica que ha llegado al final del camino de la unidad.
    /// </summary>
    Arrival = 32768,

    /// <summary>
    /// Indica que se ha empujado la unidad.
    /// </summary>
    Push = 65536,

    /// <summary>
    /// Se llama cuando hay colisión por el evento "Push", solo aplica a quien está destrás de la unidad empujada.
    /// </summary>
    PushCollision = 131072,

    /// <summary>
    /// Se llama cuando una unidad se va a alejar de otra unidad.
    /// </summary>
    MoveAway = 262144,

    /// <summary>
    /// Indica que que esta unidad cotraresta un ataque (Parte inicial antes de recibir el daño por contrarestar). (Defender una unidad por ejemplo)
    /// </summary>
    CounterStart = 524288,

    /// <summary>
    /// Indica que que esta unidad contraresta un ataque (Parte final despues de contrarestar el ataque de una unidad). (Defender una unidad por ejemplo)
    /// Nota: Si muere la unidad no se llama este evento.
    /// </summary>
    CounterEnd = 1048576,

    /// <summary>
    /// Indica que en ves de recibir el daño es un aviso de que ha se ha fallado el ataque por ende no se recibe daño.
    /// </summary>
    DamageMiss = 2097152,

    /// <summary>
    /// Para cuando se crea una unidad.
    /// </summary>
    Created = 4194304,

    /// <summary>
    /// Para cuando está una unidad dentro de la celda.
    /// </summary>
    InsideCell = 8388608,

    /// <summary>
    /// Para cuando recibimos un GameplayEffect.
    /// </summary>
    GameplayEffectAdded = 16777216,

    /// <summary>
    /// Para cuando se remueve un GameplayEffect.
    /// </summary>
    GameplayEffectRemoved = 33554432,

    /// <summary>
    /// Para cuando se termina el combate y debemos llamar algo en alguna unidad, por ejemplo el Picaro para reiniciar sus materiales.
    /// </summary>
    CombatEnd = 67108864,

    /// <summary>
    /// Para cuando cambia la energia de la unidad.
    /// </summary>
    EnergyChange = 134217728,
}