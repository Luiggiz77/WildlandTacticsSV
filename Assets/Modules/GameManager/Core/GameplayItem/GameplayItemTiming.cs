using UnityEngine;

public enum GameplayItemTiming
{
    [Tooltip("Indica que el uso es previo al combate.")]
    BeforeCombat = 0,
    [Tooltip("Indica que el uso es durante.")]
    OnCombat = 1,
    [Tooltip("Indica que el uso es despues del combate.")]
    AfterCombat = 2,
}
