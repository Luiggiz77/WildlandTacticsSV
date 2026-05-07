using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    public static class Layer
    {
        public static int Unit = LayerMask.NameToLayer("Unit");
        public static int BoardCell = LayerMask.NameToLayer("BoardCell");

        public static int UnitAttackLoop = LayerMask.NameToLayer("UnitAttackLoop");

        public static int UnitPortraitUser = LayerMask.NameToLayer("UnitPortraitUser");
        public static int UnitPortraitOpponent = LayerMask.NameToLayer("UnitPortraitOpponent");

        /// <summary>
        /// Se usa para el raycast y que permita solo este layer.
        /// </summary>
        public static LayerMask UnitMask = 1 << Unit;

        /// <summary>
        /// Se usa para el raycast y que permita solo este layer.
        /// </summary>
        public static LayerMask BoardCellMask = 1 << BoardCell;
    }
}
