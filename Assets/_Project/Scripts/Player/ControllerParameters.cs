using UnityEngine;

namespace StormPig.Player {
    [System.Serializable]
    public struct ControllerParameters {
        [field: SerializeField] public float MoveSpeed { get; private set; }
        [field: SerializeField] public float RunMultiplier { get; private set; }
        [field: SerializeField] public float SprintMultiplier { get; private set; }
        [field: SerializeField] public float Gravity { get; private set; }
        [field: SerializeField] public float JumpForce { get; private set; }
        [field: SerializeField] public float AttackMoveDebuff { get; private set; }
        [field: SerializeField] public Vector3 CamOffset { get; private set; }
        [field: SerializeField] public LayerMask GroundMask { get; private set; }
        [field: SerializeField] public Vector3 BoxCastHalfExt { get; private set; }
        [field: SerializeField] public float BoxCastDist{ get; private set; }
    }
}

