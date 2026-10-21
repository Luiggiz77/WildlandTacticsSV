using System;

[Flags]
public enum GameplayTarget : int
{
    /// <summary>
    /// Indica que no hay objetivo.
    /// </summary>
    None = 0,

    /// <summary>
    /// Indica que es para poner efecto sobre celdas vacias.
    /// </summary>
    EmptyCell = 1,

    /// <summary>
    /// Indica que es solo para unidades.
    /// </summary>
    Unit = 2,

    /// <summary>
    /// Indica que afectan solo a las unidades del gameplay.
    /// </summary>
    GameplayUnit = 4,

    /// <summary>
    /// Indica que solo es para los aliadas. (Se incluyen los GameplayUnits)
    /// </summary>
    Ally = 8,

    /// <summary>
    /// Indica que solo es para los oponentes. (Se incluyen los GameplayUnits)
    /// </summary>
    Opponent = 16,

    /// <summary>
    /// Indica que no debe aplicarse a si mismo.
    /// </summary>
    NotSelf = 32,

    /// <summary>
    /// Indica que aplica solo a nuestra unidad.
    /// </summary>
    SelfOnly = 64,

    /// <summary>
    /// Indica que aplica a trofeos.
    /// </summary>
    TrophyOnly = 128,

    /// <summary>
    /// Indica que aplica a tesoros.
    /// </summary>
    TreasureOnly = 256,

    /// <summary>
    /// Indica que aplica a la unidad que es cazadora.
    /// </summary>
    HunterOnly = 512,

    /// <summary>
    /// Indica que aplica al aunidad rastreadora.
    /// </summary>
    TrackerOnly = 1024,

    /// <summary>
    /// Indica que aplica solo a trampas.
    /// </summary>
    TrapsOnly = 2048,

    /// <summary>
    /// Indica que son unidades estrictamente, es decir que las unidades que vienen de GameplayUnits no aplican.
    /// </summary>
    UnitsOnly = 4096,

    /// <summary>
    /// Indica que no se debe aplicar a unidades.
    /// </summary>
    NotUnit = 8192,
}