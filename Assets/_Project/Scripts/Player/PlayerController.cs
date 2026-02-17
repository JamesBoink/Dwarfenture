using UnityEngine;

namespace StormPig.Player {
    public class PlayerController : MonoBehaviour {
        [SerializeField] private Camera _cam;
        [SerializeField] private PlayerPhysics _pP;

        private void Awake() {
            Initial();
        }

        private void Update() {
            _pP.Update();
            CamFollow();
        }

        private void Initial() {
            SetMoveSpeedNormal();
        }

        private void CamFollow() {
            _cam.transform.position = transform.position + _pP.ControllerParameters.CamOffset;
        }

        private void Move() {
           // _currentVelocity.z = _controllerParameters.MoveSpeed;
        }

        private void Jump() {
         //  _currentVelocity.y = _controllerParameters.JumpForce;
        }

        private void Run() {
         //   _currentMoveSpeed = _controllerParameters.MoveSpeed * _controllerParameters.RunMultiplier;
        }

        private void Sprint() {
          //  _currentMoveSpeed = _controllerParameters.MoveSpeed * _controllerParameters.SprintMultiplier;
        }

        private void SetMoveSpeedNormal() {
          //  _currentMoveSpeed = _controllerParameters.MoveSpeed;
        }
    }
}
