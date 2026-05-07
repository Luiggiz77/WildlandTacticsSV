public enum UnitRole
{
    /// <summary>
    /// Cualquiera
    /// </summary>
    Any = 0,

    /// <summary>
    /// Indica que es el fantasma de una unidad.
    /// </summary>
    GhostUnit = 1,

    /// <summary>
    /// Indica que es una flama de fenix.
    /// </summary>
    PhoenixFlame = 2,

    /// <summary>
    /// Indica que es un jefe.
    /// </summary>
    Boss = 3,

    /// <summary>
    /// Indica que es un trofeo.
    /// </summary>
    Trophy = 4,

    /// <summary>
    /// Indica que es un tesoro.
    /// </summary>
    Treasure = 5,

    /// <summary>
    /// Indica que es una unidad roca pequeña.
    /// </summary>
    RockUnitSmall = 6,

    /// <summary>
    /// Indica que es una unidad roca grande.
    /// </summary>
    RockUnitBig = 7,

    /// <summary>
    /// Indica que es una casilla bendita.
    /// </summary>
    BlessedCell = 8,

    /// <summary>
    /// Indica que es una unidad clonada pero como unidad del gameplay.
    /// </summary>
    ClonedUnit = 9,

    #region TRAPS 50-70

    /// <summary>
    /// Indica que es una trampa de acido
    /// </summary>
    AcidTrap = 50,

    /// <summary>
    /// Indica que es un pilar de fuego.
    /// </summary>
    FireTrap = 51,

    /// <summary>
    /// Indica que es vapor de agua.
    /// </summary>
    SteamTrap = 52,

    #endregion


    /// <summary>
    /// Mago
    /// </summary>
    Mage = 100,
    /// <summary>
    /// Hechicero
    /// </summary>
    Magician = 101,
    /// <summary>
    /// Brujo
    /// </summary>
    Sorcerer = 102,
    /// <summary>
    /// Necromano
    /// </summary>
    Necromancer = 103,


    /// <summary>
    /// Arquero
    /// </summary>
    Archer = 120,
    /// <summary>
    /// Cazador
    /// </summary>
    Hunter = 121,
    /// <summary>
    /// Francotirador
    /// </summary>
    Sniper = 122,
    /// <summary>
    /// Rastreador
    /// </summary>
    Tracker = 123,

    /// <summary>
    /// Guerrero
    /// </summary>
    Warrior = 140,
    /// <summary>
    /// Caballero
    /// </summary>
    Knight = 141,
    /// <summary>
    /// Barbaro
    /// </summary>
    Barbarian = 142,
    /// <summary>
    /// Paladin
    /// </summary>
    Paladin = 143,


    /// <summary>
    /// Curador
    /// </summary>
    Healer = 160,
    /// <summary>
    /// Clerigo
    /// </summary>
    Cleric = 161,
    /// <summary>
    /// Bardo
    /// </summary>
    Bard = 162,
    /// <summary>
    /// Alquimista
    /// </summary>
    Alchemist = 163,

    /// <summary>
    /// Picaro
    /// </summary>
    Rogue = 180,
    /// <summary>
    /// Ladrón
    /// </summary>
    Thief = 181,
    /// <summary>
    /// Asesino
    /// </summary>
    Assasin = 182,
    /// <summary>
    /// Bufón
    /// </summary>
    Jester = 183,


    /// <summary>
    /// Druida
    /// </summary>
    Druid = 220,
    /// <summary>
    /// Piromano
    /// </summary>
    Pyromaniac = 221,
    /// <summary>
    /// Hidrosofista
    /// </summary>
    Hydrosophist = 222,
    /// <summary>
    /// Geomante
    /// </summary>
    Geomancer = 223,
}
