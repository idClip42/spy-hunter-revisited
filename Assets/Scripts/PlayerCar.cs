using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpyHunter.Game;

namespace SpyHunter.Car
{

    [RequireComponent(typeof(Rigidbody))]
    public class PlayerCar : Car
    {
        [Header("Input")]
        public KeyCode gearShift = KeyCode.LeftShift;
        public bool holdGearShiftKey = false;
        public KeyCode spinOutTest = KeyCode.Space;

        [Header("Player Movement")]
        public bool autoDrive = true;

        protected override void Update()
        {
            if (!Alive) return;
            MovementInput();
            GearShift();
        }

        void MovementInput()
        {
            // Gets our inputs
            moveInput.y = Input.GetAxis("Vertical");
            moveInput.x = Input.GetAxis("Horizontal");


            // If we are in auto drive mode
            if (autoDrive)
            {
                // Gets the raw Y input
                // So we know exactly what the key state is
                float rawY = Input.GetAxisRaw("Vertical");

                // If there is no forward input
                if (rawY.Equals(0))
                {
                    // Sets the forward input to 1
                    moveInput.y = 1;
                }
            }
        }

        void GearShift()
        {
            if (holdGearShiftKey == true)
            {
                if (Input.GetKey(gearShift))
                    inHighGear = true;
                else
                    inHighGear = false;
            }
            else
            {
                if (Input.GetKeyDown(gearShift))
                    inHighGear = !inHighGear;
            }
        }

        void SpinOutInput()
        {
            if (Input.GetKeyDown(spinOutTest))
            {
                SpinOut(new Vector3(0, 0, -40));
            }
        }
    }
}