using UnityEngine;

namespace StormPig.Player {
    [System.Serializable]
    public class PlayerPhysics {
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private Transform _castOrigin;
        [field: SerializeField] public ControllerParameters ControllerParameters { get; private set; }

        private bool _gravityReset = false;
        private bool _grounded;
        private bool _jumped;
        private float _currentMoveSpeed = 0f;
        private float _currentJumpForce = 0f;
        private Vector3 _currentVelocity;

        public void Update() {
            GroundCast();
            Gravity();
            ApplyMovementForce(_currentVelocity * Time.deltaTime);
        }

        private void ApplyMovementForce(Vector3 f) {
            _rb.AddForce(f, ForceMode.Force);

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
            _rb.angularVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
        }

        [ContextMenu("Stop")]
        /// <summary>
        /// Hard stop, applies cumulated negative force to rigidbody
        /// Then zeroes x,z velocities
        /// </summary>
        private void StopMoving() {
            _currentVelocity = new Vector3(0f, _currentVelocity.y, 0f);
            Vector3 negF = -_rb.GetAccumulatedForce();
            negF.y = 0;
            _rb.AddForce(negF, ForceMode.Force);
            _rb.linearVelocity = new Vector3(0f, _rb.linearVelocity.y, 0f);
            _rb.angularVelocity = new Vector3(0f, _rb.angularVelocity.y, 0f);
        }

        /// <summary>
        /// Applies gravity unless grounded
        /// </summary>
        private void Gravity() {
            if (_grounded) {
                _currentVelocity.y = 0f;
                StopGravity();
                return;
            }
            _currentVelocity.y += ControllerParameters.Gravity * _rb.mass;

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
    }

}