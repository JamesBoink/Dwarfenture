using UnityEngine;

namespace StormPig.Player {
    [System.Serializable]
    public class PlayerPhysics {
        [SerializeField] private Rigidbody _rb;
        [Header("Make sure to set origin on head position of object")]
        [SerializeField] private Transform _castOrigin;
        [SerializeField] private Transform _intCastOrigin;
        [field: SerializeField] public ControllerParameters ControllerParameters { get; private set; }

        // serialized for debugging
        private bool _gravityReset = false;
        private bool _grounded;
        private float _currentMoveSpeed = 0f;
        private float _currentJumpForce = 0f;
       // [SerializeField] private Vector3 _currentVelocity;
        private Vector3 _rbVelocity;

        private RaycastHit _interaction;
        private GameObject _selectedInteractable = null;
        public Interactables.IInteractable CurrentInteractable = null;


        public void Update() {
            GroundCast();
            Gravity();
            InteractionBox();
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

        /// <summary>
        /// Casts a box that detects interactable objects
        /// </summary>
        private void InteractionBox() {
            if (Physics.BoxCast(_intCastOrigin.position, ControllerParameters.IntHalfExt, Vector3.forward, out _interaction, Quaternion.identity, ControllerParameters.IntCastDist, ControllerParameters.InteractionMask.value)) {
                // Null and cache check, to avoid unnecessary TryGetComponent calls
                // but still display interactability if possible
                if(_selectedInteractable != null && _selectedInteractable == _interaction.collider.gameObject) { 
                    if(CurrentInteractable != null) {
                        CurrentInteractable.Selected();
                    }
                    return; 
                }

                // Cache for check above
                _selectedInteractable = _interaction.collider.gameObject;

                // If we got interactable here, call its selected eg. higlight door, item, station
                // if not nullify interactable ref
                if (_selectedInteractable.TryGetComponent(out Interactables.IInteractable i)) {
                    CurrentInteractable = i;
                    CurrentInteractable.Selected();
                } else {
                    CleanupInteractable();
                }
            } else {
                _selectedInteractable = null;
                CleanupInteractable();
            }
        }

        private void CleanupInteractable() {
            if (CurrentInteractable != null) {
                Global.Events.CleanupInteractionPanel?.Invoke();
                CurrentInteractable = null; 
            }
        }

        public void OnDrawGizmos() {
            Gizmos.DrawCube(_castOrigin.position, ControllerParameters.BoxCastHalfExt * 2f);
            Gizmos.DrawCube(new Vector3(_castOrigin.position.x, _castOrigin.position.y- ControllerParameters.BoxCastDist, _castOrigin.position.z), ControllerParameters.BoxCastHalfExt * 2f);

            Gizmos.DrawCube(_intCastOrigin.position, ControllerParameters.IntHalfExt * 2f);
            Gizmos.DrawCube(new Vector3(_intCastOrigin.position.x, _intCastOrigin.position.y, _intCastOrigin.position.z + ControllerParameters.IntCastDist), ControllerParameters.IntHalfExt * 2f);
        }
    }

}