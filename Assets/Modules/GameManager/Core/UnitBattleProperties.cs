public class UnitBattleProperties
{
    /// <summary>
    /// Id de las propiedades..
    /// </summary>
    public int Id = 0;

    /// <summary>
    /// Nombre original de la unidad.
    /// </summary>
    public string Name;

    /// <summary>
    /// Descripción de la unidad.
    /// </summary>
    public string Description;

    /// <summary>
    /// Puntos de vida de la unidad.
    /// </summary>
    public int Health = 10;

    /// <summary>
    /// Puntos de energia de la unidad.
    /// </summary>
    public int Energy = 1;

    /// <summary>
    /// Puntos de poder físico.
    /// </summary>
    public int PhysicalPower = 2;

    /// <summary>
    /// Puntos de poder mágico.
    /// </summary>
    public int MagicalPower = 2;

    /// <summary>
    /// Resistensia al daño fisico.
    /// </summary>
    public int PhysicalResistance = 1;

    /// <summary>
    /// Resistensia al daño magico.
    /// </summary>
    public int MagicalResistance = 1;

    /// <summary>
    /// Puntos de movimiento.
    /// </summary>
    public int Movement = 1;

    /// <summary>
    /// Rango de ataque de la unidad.
    /// </summary>
    public int AttackRange = 1;

    /// <summary>
    /// Nos indica el tipo de area de ataque.
    /// </summary>
    public AttackAreaType AttackAreaType;

    /// <summary>
    /// Indica si la unidad puede atacar a espacios vacios o no.
    /// </summary>
    public bool AllowAttackEmptySpace = false;

    /// <summary>
    /// Indica si la unidad se teletransporta.
    /// </summary>
    public bool Teleports = false;

    /// <summary>
    /// Nos indica que tipo de unidad es.
    /// </summary>
    public UnitType UnitType = UnitType.Unit;

    /// <summary>
    /// Nos indica que unidad puede reemplazar esta unidad.
    /// </summary>
    public UnitType Replaceable;

    /// <summary>
    /// Nos indica que la unidad puede ser agregada al tablero siempre y cuando las banderas coincidan con las que tienen las unidades en la celda, de lo contrario no se agrega.
    /// </summary>
    public UnitType Surpass;

    /// <summary>
    /// Nos indica si debemos mostrar barra de vida.
    /// </summary>
    public bool Healthbar;

    /// <summary>
    /// Indica el rol de la unidad.
    /// </summary>
    public UnitRole Role = UnitRole.Any;
}