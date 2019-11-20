using UnityEngine;
using System;

namespace Assets.Scripts.Helper_Scripts
{
    class TimeManager
    {
        private float counter = 0f;

        private float movementFrequency;

        public event Action LimitReachedEvent;

        public TimeManager(float frequency)
        {
            movementFrequency = frequency;
        }

        public void ResetCounter()
        {
            counter = 0;
        }

        public void CheckMovementFrequency()
        {
            counter += Time.deltaTime;

            if (counter >= movementFrequency)
            {
                counter -= movementFrequency;
                LimitReachedEvent();
            }
        }
    }
}
