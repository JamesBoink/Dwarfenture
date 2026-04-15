using UnityEngine;

namespace StormPig.Player {
    [System.Serializable]
    public struct ControllerParameters {
        [field: SerializeField] public float TargetMoveSpeed { get; private set; }
        [field: SerializeField] public float TargetRunSpeed { get; private set; }
        [field: SerializeField] public float TargetSprintSpeed { get; private set; }
        [field: SerializeField] public float RunMultiplier { get; private set; }
        [field: SerializeField] public float SprintMultiplier { get; private set; }
        [field: SerializeField] public float Gravity { get; private set; }
        [field: SerializeField] public float MaxGravity { get; private set; }
        [field: SerializeField] public float JumpForce { get; private set; }
        [field: SerializeField] public float AttackMoveDebuff { get; private set; }
        [field: SerializeField] public Vector3 CamOffset { get; private set; }  
        [field: SerializeField] public Vector3 CamRotation { get; private set; }
        [field: Space(3)]
        [field: Header("Ground")]
        [field: SerializeField] public LayerMask GroundMask { get; private set; }
        [field: SerializeField] public Vector3 BoxCastHalfExt { get; private set; }
        [field: Header("Make sure that the total distance goes from top of head to the ground")]
        [field: SerializeField] public float BoxCastDist{ get; private set; }
        [field: Space(3)]
        [field: Header("Interaction")]
        [field: SerializeField] public Vector3 IntHalfExt{ get; private set; }
        [field: SerializeField] public float IntCastDist { get; private set; }
        [field: SerializeField] public LayerMask InteractionMask { get; private set; }
    }
}

