using UnityEngine;
using UnityEngine.EventSystems;

namespace SpaceInvaders.Gameplay.Input
{
    public enum MobileControlAction
    {
        MoveLeft,
        MoveRight,
        Fire
    }

    public sealed class MobileControlButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [SerializeField] private MobilePlayerInput _input;
        [SerializeField] private MobileControlAction _action;

        public void OnPointerDown(PointerEventData eventData)
        {
            SetPressed(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            SetPressed(false);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SetPressed(false);
        }

        private void OnDisable()
        {
            SetPressed(false);
        }

        private void SetPressed(bool isPressed)
        {
            if (_input == null)
            {
                return;
            }

            switch (_action)
            {
                case MobileControlAction.MoveLeft:
                    _input.SetLeftPressed(isPressed);
                    break;

                case MobileControlAction.MoveRight:
                    _input.SetRightPressed(isPressed);
                    break;

                case MobileControlAction.Fire:
                    _input.SetFirePressed(isPressed);
                    break;
            }
        }
    }
}