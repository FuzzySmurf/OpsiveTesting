using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ARAWorks.Damage
{
    [Serializable]
    public class CharacterResource
    {
        /// <summary>
        /// Triggered when either Current or Max is changed and provides the amount it was changes by.
        /// </summary>
        public event Action<CharacterResource> Changed;

        /// <summary>
        /// Triggered when Current has increased and provides the amount it was increased by.
        /// </summary>
        public event Action<CharacterResource, float> Increased;

        /// <summary>
        /// Triggered when Current has decreased and provides the amount it was decreased by.
        /// </summary>
        public event Action<CharacterResource, float> Decreased;

        /// <summary>
        /// Triggered when Current is empty (0)
        /// </summary>
        public event Action<CharacterResource> Depleted;


        public float Current { get { return _current; } set { HandleCurrentResource(value); } }

        public float Max { get { return _max; } set { HandleMaxResource(value); } }


        public float Percent => (Current == 0 || Max == 0) ? 0 : Current / Max;

        [ShowInInspector, ReadOnly] private float _current;
        [ShowInInspector, ReadOnly] private float _max;

        public void Initialize(float max)
        {
            Max = max;
            _current = max;
        }

        public void Add(int value)
        {
            float newAmount = value;
            if (Current + newAmount > Max)
                newAmount = Max - Current;

            Current += newAmount;
        }

        public void Subtract(int value)
        {
            float newAmount = value;
            if (Current - newAmount < 0)
                newAmount = Current;
            
            Current -= newAmount;
        }

        private void HandleCurrentResource(float value)
        {
            if (_current == value) return;

            if (_current < value) //If we are adding resource
            {
                if (_current == Max) return;

                //Trigger increased callback
                Increased?.Invoke(this, value - _current);
            }
            if (_current > value) //If we are subtracting resource
            {
                //Trigger decreased callback
                Decreased?.Invoke(this, value - _current);
            }

            //Set value
            _current = Mathf.Clamp(value, 0, Max);

            //Trigger changed callback
            Changed?.Invoke(this);

            if (_current == 0)
            {
                Depleted?.Invoke(this);
            }
        }

        private void HandleMaxResource(float value)
        {
            if (_max == value) return;

            _max = value;

            if (_current > _max)
            {
                _current = _max;
            }

            //Trigger changed callback
            Changed?.Invoke(this);
        }
    }
}