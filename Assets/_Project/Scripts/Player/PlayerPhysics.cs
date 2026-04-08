using UnityEngine;

namespace StormPig.Player {
    [System.Serializable]
    public class PlayerPhysics {
        [SerializeField] private Rigidbody _rb;
        [Header("Make sure to set origin on head position of object")]
        [SerializeField] private Transform _castOrigin;
        [field: SerializeField] public ControllerParameters ControllerParameters { get; private set; }

        // serialized for debugging
        private bool _gravityReset = false;
        [SerializeField] private bool _grounded;
        [SerializeField] private float _currentMoveSpeed = 0f;
        [SerializeField] private float _currentJumpForce = 0f;
       // [SerializeField] private Vector3 _currentVelocity;
        [SerializeField] private Vector3 _rbVelocity;

        public void Update() {
            GroundCast();
            Gravity();
            _rbVelocity = _rb.linearVelocity;
        }
        
        public void Move(Vector2 v) {
            if(v.x == 0f || v.y == 0f) { // if we're moving in one direction only just apply acceleration
                _rb.linearVelocity = new Vector3(v.x * ControllerParameters.TargetMoveSpeed, _rb.linearVelocity.y, v.y * ControllerParameters.TargetMoveSpeed);
            } else {  // if we're moving in angular direction, halve the applications so we wont move faster than allowed                      
                _rb.linearVelocity = new Vector3((Mathf.Sign(v.x) * (ControllerParameters.TargetMoveSpeed / 2f)), _rb.linearVelocity.y, (Mathf.Sign(v.y) * (ControllerParameters.TargetMoveSpeed / 2f)));
            }

        }

        public void Jump() {
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, ControllerParameters.JumpForce, _rb.linearVelocity.z);
        }

        /// <summary>
        /// Hard stop, applies cumulated negative force to rigidbody
        /// Then zeroes x,z velocities
        /// </summary>
        public void StopMoving() {
            _rb.linearVelocity = new Vector3(0f, _rb.linearVelocity.y, 0f);
        }

        /// <summary>
        /// Hard gravity stop
        /// </summary>
        private void StopGravity() {
            Vector3 negF = -_rb.GetAccumulatedForce();
            negF.x = 0;
            negF.z = 0;
            _rb.AddForce(negF, ForceMode.Force);
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
        }



        /// <summary>
        /// Applies gravity unless grounded
        /// </summary>
        private void Gravity() {
            if (_grounded) {
                _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
                StopGravity();
                return;
            }

            if (_rb.linearVelocity.y <= ControllerParameters.MaxGravity) { return; }
           _rb.linearVelocity += new Vector3(0f, ControllerParameters.Gravity, 0f);
        }

        /// <summary>
        /// Detects ground with a box cast
        /// </summary>
        private void GroundCast() {
            if (Physics.BoxCast(_castOrigin.position, ControllerParameters.BoxCastHalfExt, Vector3.down, Quaternion.identity, ControllerParameters.BoxCastDist, ControllerParameters.GroundMask.value)) {
                _grounded = true;
            } else {
                _grounded = false;
            }
        }

        public void OnDrawGizmos() {
            Gizmos.DrawCube(_castOrigin.position, ControllerParameters.BoxCastHalfExt * 2f);
            Gizmos.DrawCube(new Vector3(_castOrigin.position.x, _castOrigin.position.y- ControllerParameters.BoxCastDist, _castOrigin.position.z), ControllerParameters.BoxCastHalfExt * 2f);
        }
    }

}