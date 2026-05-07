using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    public static class AnimationControllerData
    {
        /// <summary>
        /// Layers
        /// </summary>
        public static class Layers
        {
            public static readonly int BaseLayer = 0;
        }

        /// <summary>
        /// Estados
        /// </summary>
        public static class State
        {
            /// <summary>
            /// Solo es para revisar que estamos en idle.
            /// </summary>
            public static readonly int Idle = Animator.StringToHash("Idle");
        }

        /// <summary>
        /// Triggers y estados comparten el mismo nombre.
        /// </summary>
        public static class Triggers
        {
            /// <summary>
            /// Se usa para detener cualquier animacion y regresar al idle.
            /// </summary>
            public static readonly int StopAction = Animator.StringToHash("StopAction");

            /// <summary>
            /// Se usa para iniciar la animación de ataque en loop.
            /// </summary>
            public static readonly int AttackLoop = Animator.StringToHash("AttackLoop");

            /// <summary>
            /// Se usa para terminar la animación de ataque en loop.
            /// </summary>
            public static readonly int EndAttackLoop = Animator.StringToHash("EndAttackLoop");

            /// <summary>
            /// Se usa para iniciar la animación de movimiento en loop.
            /// </summary>
            public static readonly int Move = Animator.StringToHash("Move");

            /// <summary>
            /// Se usa para iniciar la animación de retroceso de frente.
            /// </summary>
            public static readonly int KnockBack = Animator.StringToHash("KnockBack");
            /// <summary>
            /// Se usa para iniciar la animación de retroceso de atrás.
            /// </summary>
            public static readonly int KnockFront = Animator.StringToHash("KnockFront");
            /// <summary>
            /// Se usa para iniciar la animación de retroceso del lado izquierdo.
            /// </summary>
            public static readonly int KnockLeft = Animator.StringToHash("KnockLeft");
            /// <summary>
            /// Se usa para iniciar la animación de retroceso del lado derecho.
            /// </summary>
            public static readonly int KnockRight = Animator.StringToHash("KnockRight");

            /// <summary>
            /// Se usa para terminar la animación de movimiento en loop.
            /// </summary>
            public static readonly int EndMove = Animator.StringToHash("EndMove");

            /// <summary>
            /// Se usa para hacer un ataque.
            /// </summary>
            public static readonly int Attack = Animator.StringToHash("Attack");

            /// <summary>
            /// Se usa para hacer un salto.
            /// </summary>
            public static readonly int Jump = Animator.StringToHash("Jump");

            /// <summary>
            /// Se usa para hacer una caida.
            /// </summary>
            public static readonly int Fall = Animator.StringToHash("Fall");

            /// <summary>
            /// Se usa para indicar que recibimos daño.
            /// </summary>
            public static readonly int ReceiveDamageFront = Animator.StringToHash("ReceiveDamageFront");
            public static readonly int ReceiveDamageBack = Animator.StringToHash("ReceiveDamageBack");
            public static readonly int ReceiveDamageLeft = Animator.StringToHash("ReceiveDamageLeft");
            public static readonly int ReceiveDamageRight = Animator.StringToHash("ReceiveDamageRight");

            /// <summary>
            /// Se usa para indicar que recibimos daño en la mascara.
            /// </summary>
            public static readonly int ReceiveDamageMaskFront = Animator.StringToHash("ReceiveDamageMaskFront");
            public static readonly int ReceiveDamageMaskBack = Animator.StringToHash("ReceiveDamageMaskBack");
            public static readonly int ReceiveDamageMaskLeft = Animator.StringToHash("ReceiveDamageMaskLeft");
            public static readonly int ReceiveDamageMaskRight = Animator.StringToHash("ReceiveDamageMaskRight");

            /// <summary>
            /// Se usa para indicar que nos han empujado.
            /// </summary>
            public static readonly int PushFront = Animator.StringToHash("PushFront");
            public static readonly int PushBack = Animator.StringToHash("PushBack");
            public static readonly int PushLeft = Animator.StringToHash("PushLeft");
            public static readonly int PushRight = Animator.StringToHash("PushRight");

            /// <summary>
            /// Se usa para indicar que morimos.
            /// </summary>
            public static readonly int Death = Animator.StringToHash("Death");

            /// <summary>
            /// Se usa para indicar que nos crearon.
            /// </summary>
            public static readonly int Creation = Animator.StringToHash("Creation");

            /// <summary>
            /// Se usa para indicar que rotaremos 180º.
            /// </summary>
            public static readonly int Turn180 = Animator.StringToHash("Turn180");

            /// <summary>
            /// Se usa para indicar que rotaremos 90º positivamente.
            /// </summary>
            public static readonly int Turn90Pos = Animator.StringToHash("Turn90Pos");

            /// <summary>
            /// Se usa para indicar que rotaremos 90º negativamente.
            /// </summary>
            public static readonly int Turn90Neg = Animator.StringToHash("Turn90Neg");
        }

        /// <summary>
        /// Parameters
        /// </summary>
        public static class Parameters
        {
            /// <summary>
            /// Para tener la posicion de la mano izquierda.
            /// </summary>
            public static readonly int LeftHand = Animator.StringToHash("ParameterLeftHand");
        }
    }
}
