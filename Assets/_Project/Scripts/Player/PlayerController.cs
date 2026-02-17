using UnityEngine;
using UnityEngine.InputSystem;

namespace StormPig.Player {
    public class PlayerController : MonoBehaviour {
        [SerializeField] private Camera _cam;
        [SerializeField] private PlayerPhysics _pP;
        [SerializeField] private bool _debugBoxCast;
        public Vector2 InputVector { get; private set; }
        public bool IsJumping { get; private set; }

        private void Update() {
            _pP.Update();
            CamFollow();
        }

        private void OnDrawGizmos() {
            if (!_debugBoxCast) { return;  }
            _pP.OnDrawGizmos();
        }

        private void CamFollow() {
            _cam.transform.position = transform.position + _pP.ControllerParameters.CamOffset;
        }

      

        public void JumpInput(InputAction.CallbackContext context) {
            if (context.ReadValue<float>() > 0.1f) {
                IsJumping = true;
            } else {
                IsJumping = false;
            }
        }

        public void MoveInput(InputAction.CallbackContext context) {
            if (context.ReadValue<Vector2>().x < 0f || context.ReadValue<Vector2>().x > 0f || context.ReadValue<Vector2>().y < 0f || context.ReadValue<Vector2>().y > 0f) {
                InputVector = context.ReadValue<Vector2>();
                _pP.Move(InputVector);
            } else {
                InputVector = Vector2.zero;
                _pP.StopMoving();
            }
        ;
        }
    }
}
