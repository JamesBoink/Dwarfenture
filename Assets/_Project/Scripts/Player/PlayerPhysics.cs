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
        private bool _jumped;
        [SerializeField] private float _currentMoveSpeed = 0f;
        [SerializeField] private float _currentJumpForce = 0f;
        [SerializeField] private Vector3 _currentVelocity;
        [SerializeField] private Vector3 _rbVelocity;

        public void Update() {
            GroundCast();
            Gravity();
            ApplyMovementForce(_currentVelocity * Time.deltaTime);
        }

        private void ApplyMovementForce(Vector3 f) {
            // error below:
            // if we reach target movespeed on one axes
            // we cannot change movement direction


            // make sure we aren't constantly accelerating
            if(Mathf.Abs(_rb.linearVelocity.z) >= ControllerParameters.TargetMoveSpeed || Mathf.Abs(Mathf.Abs(_rb.linearVelocity.z) + Mathf.Abs(_rb.linearVelocity.x)) >= ControllerParameters.TargetMoveSpeed) { return; }
            _rb.AddForce(f, ForceMode.Acceleration);
            _rbVelocity = _rb.linearVelocity;
        }
        
        public void Move(Vector2 v) {
            if(v.x < 0.1f || v.y < 0.1f) { // if we're moving in one direction only just apply acceleration
                _currentVelocity.z = v.y * ControllerParameters.MoveSpeedAcceleration;
                _currentVelocity.x = v.x * ControllerParameters.MoveSpeedAcceleration;
            } else {  // if we're moving in angular direction, halve the applications so we wont move faster than allowed
                _currentVelocity.z = (v.y * ControllerParameters.MoveSpeedAcceleration) / 2f;
                _currentVelocity.x = (v.x * ControllerParameters.MoveSpeedAcceleration) / 2f;
            }           
        }

        /// <summary>
        /// Hard stop, applies cumulated negative force to rigidbody
        /// Then zeroes x,z velocities
        /// </summary>
        public void StopMoving() {
            _currentVelocity = new Vector3(0f, _currentVelocity.y, 0f);
            Vector3 negF = -_rb.GetAccumulatedForce();
            negF.y = 0;
            _rb.AddForce(negF, ForceMode.Force);
            // Has to be called twice to fully and properly zero velocities
            _rb.linearVelocity = new Vector3(0f, _rb.linearVelocity.y, 0f);
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
                _currentVelocity.y = 0f;
                StopGravity();
                return;
            }
           // if(_currentVelocity.y <= ControllerParameters.Gravity) { return; }
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

        public void OnDrawGizmos() {
            Gizmos.DrawCube(_castOrigin.position, ControllerParameters.BoxCastHalfExt * 2f);
            Gizmos.DrawCube(new Vector3(_castOrigin.position.x, _castOrigin.position.y- ControllerParameters.BoxCastDist, _castOrigin.position.z), ControllerParameters.BoxCastHalfExt * 2f);
        }
    }

}