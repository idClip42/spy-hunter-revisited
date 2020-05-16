using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpyHunter.Car
{
    public class CarCamera : MonoBehaviour
    {
        [Header("Car")]
        public Car carToFollow;

        [Header("Transform")]
        public Vector3 offset;
        public Vector3 rotation;
        [Range(0, 1)] public float tilt;

        [Header("Speed")]
        public float maxMoveSpeed = 160;
        public float maxMoveSpeedDead = 30;
        public float minMoveSpeed = 10;
        public float turnSpeedMultiplier = 1.5f;
        public float turnAngleBuffer = 30;
        public float maxTurnSpeed = 100;
        public float minTurnSpeed = 0;

        [Header("Acceleration")]
        public float moveAcceleration = 50;
        public float turnAcceleration = 50; 

        [Header("Extra")]
        public float minHeight = 2;

        float currentMoveSpeed;
        float currentTurnSpeed;

        void FixedUpdate()
        {
            float carSpeedPerc = carToFollow.Velocity.magnitude / carToFollow.lowTopSpeed;

            // Gets our movement speed
            // Based on whether car is alive,
            // on car velocity,
            // and on camera acceleration
            currentMoveSpeed = Mathf.MoveTowards(
                currentMoveSpeed,
                carToFollow.Alive ?
                    maxMoveSpeed * carSpeedPerc :
                    maxMoveSpeedDead,
                moveAcceleration * Time.fixedDeltaTime
                );

            // Clamps move speed between min and max
            currentMoveSpeed = Mathf.Clamp(currentMoveSpeed, minMoveSpeed, maxMoveSpeed);

            // Gets a percentage of the turn speed we want,
            // based on an angle buffer used with the angle of the car
            float turnSpeedPerc = Mathf.InverseLerp(
                0,
                turnAngleBuffer,
                Vector3.Angle(
                    Vector3.ProjectOnPlane(transform.forward, Vector3.up),
                    Vector3.ProjectOnPlane(carToFollow.transform.forward, Vector3.up)
                    )
                );

            // Accelerates the turn speed towards the desired one
            currentTurnSpeed = Mathf.MoveTowards(
                currentTurnSpeed,
                maxTurnSpeed * turnSpeedPerc,
                turnAcceleration * Time.fixedDeltaTime
                );

            // Clamps turn speed between min and max
            currentTurnSpeed = Mathf.Clamp(currentTurnSpeed, minTurnSpeed, maxTurnSpeed);

            // We get the forward vector of the car,
            // and determine how much to keep the camera level
            // vs follow the car's tilt
            Vector3 carForward = Vector3.Lerp(
                Vector3.ProjectOnPlane(carToFollow.transform.forward, Vector3.up),
                carToFollow.transform.forward,
                tilt
                );

            // We set our desired rotation
            Quaternion desiredRotation = Quaternion.Euler(
                Quaternion.LookRotation(carForward).eulerAngles + rotation);

            // We move the camera towards that desired rotation
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                desiredRotation,
                currentTurnSpeed * Time.fixedDeltaTime
                );

            // Gets the desired position of the camera,
            // based on both car position
            // and camera direction
            Vector3 desiredPosition = transform.TransformDirection(offset) + carToFollow.transform.position;
            transform.position = Vector3.MoveTowards(
                transform.position,
                desiredPosition,
                currentMoveSpeed * Time.fixedDeltaTime);

            // If we're below the minHeight threshold
            if (transform.position.y < minHeight)
            {
                // We move up to the threshold
                transform.position += Vector3.up * (minHeight - transform.position.y);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            const float RIGHT_OFFSET = 3;
            string text = "";
            text += "Cam Move Speed: " + currentMoveSpeed + "\n";
            text += "Cam Turn Speed: " + currentTurnSpeed + "\n";
            UnityEditor.Handles.Label(
                carToFollow.transform.position + carToFollow.transform.right * RIGHT_OFFSET, 
                text);
        }
#endif
    }
}