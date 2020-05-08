using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpyHunter.Car
{
    public class CarCamera : MonoBehaviour
    {
        public CarControl carToFollow;
        public float moveSpeed;
        public float deadMoveSpeed;
        public float turnSpeed;
        public float minHeight = 2;

        void FixedUpdate()
        {
            // Gets our movement speed
            // Based on whether car is alive
            float speed = carToFollow.Alive ? moveSpeed : deadMoveSpeed;

            // TODO: Camera acceleration

            // Moves our camera towards the car
            transform.position = Vector3.MoveTowards(
                transform.position,
                carToFollow.cameraDestination.position,
                speed * Time.fixedDeltaTime);

            // Rotates our camera to face the correct direction
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                carToFollow.cameraDestination.rotation,
                turnSpeed * Time.fixedDeltaTime);

            // If we're below the minHeight threshold
            if(transform.position.y < minHeight)
            {
                // We move up to the threshold
                transform.position += Vector3.up * (minHeight - transform.position.y);
            }
        }
    }
}