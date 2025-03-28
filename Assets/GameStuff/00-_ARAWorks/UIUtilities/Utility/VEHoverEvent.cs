using System;
using System.Collections.Generic;
using ARAWorks.Base.Extensions;
using ARAWorks.Base.Timer;
using UnityEngine;
using UnityEngine.UIElements;

namespace ARAWorks.UIToolkit
{
    public class VEHoverEvent<T> where T : VisualElement
    {

        public event Action<T> HoverEnter;
        public event Action<T> HoverMove;
        public event Action<T> HoverLeave;

        private Timer _hoverTimer;

        private T _target;
        private bool _isOverTarget;


        public VEHoverEvent(VisualElement target, float hoverTime = 0.25f)
        {
            _target = (T)target;
            _target.RegisterCallback<PointerEnterEvent>(OnPointerEnter);
            _target.RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
            _target.RegisterCallback<PointerMoveEvent>(OnPointerMove);

            _hoverTimer = new Timer(hoverTime, OnHoverTimerFinished, false);

        }

        public void Refresh()
        {
            if (_isOverTarget == true)
            {
                _hoverTimer.Restart();
            }
            else
            {
                _hoverTimer.Stop();
            }
        }

        private void OnPointerEnter(PointerEnterEvent evt)
        {
            _isOverTarget = true;

            if (_hoverTimer.IsRunning == true || _hoverTimer.IsFinished == true) return;
            _hoverTimer.Start();
        }


        private void OnPointerMove(PointerMoveEvent evt)
        {
            if (_hoverTimer.IsFinished == true)
            {
                HoverMove?.Invoke(_target);
            }
        }

        private void OnPointerLeave(PointerLeaveEvent evt)
        {
            _isOverTarget = false;

            if (_hoverTimer.IsFinished == true)
            {
                HoverLeave?.Invoke(_target);
            }

            _hoverTimer.Stop();
        }

        private void OnHoverTimerFinished()
        {
            if (_target.ContainsPoint(_target.MouseWorldToLocal()))
            {
                HoverEnter?.Invoke(_target);
            }
        }
    }
}
