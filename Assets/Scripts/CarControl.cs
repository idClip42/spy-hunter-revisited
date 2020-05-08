using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpyHunter.Game;

namespace SpyHunter.Car
{
    // TODO: Abstract out generic car class


    [RequireComponent(typeof(Rigidbody))]
    public class CarControl : MonoBehaviour
    {
        [Header("Input")]
        public KeyCode gearShift = KeyCode.LeftShift;
        public bool holdGearShiftKey = false;
        public KeyCode spinOutTest = KeyCode.Space;

        [Header("Acceleration")]
        public float lowTopSpeed;
        public float highTopSpeed;
        public float acceleration;
        public bool autoDrive = true;
        public bool allowReverse = false;
        public float slowFromHighSpeedBuffer = 10;
        public float slowFromHighSpeedMult = 2;

        [Header("Steering")]
        public float turnSpeed;
        public float turnVelocityImpact = 0.1f;
        [Range(0,1)]public float redirectSpeedOnTurn = 1;
        public bool autoCenterRotation = true;
        public float autoCenterRotationSpeed;
        public bool constrainRotation = true;
        public float constrainRotationMaxValue = 60;

        [Header("Camera")]
        public Transform cameraDestination;

        public bool Alive { get; private set; }

        Rigidbody rb;
        bool inHighGear = false;
        Vector2 moveInput;
        Vector3 roadDirection;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        void Update()
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
                SpinOut();
            }
        }

        void SpinOut()
        {
            Die();
            rb.AddTorque(0, 100000000, 0);
            rb.velocity = rb.velocity + new Vector3(0, 0, -40);
        }

        void Die()
        {
            Alive = false;
            rb.constraints = RigidbodyConstraints.None;
            //flame.SetActive(true);
        }

        void FixedUpdate()
        {
            Acceleration();
            Steering();
        }

        void Acceleration()
        {
            // Gets our desired speed, based on the gear shift
            float topSpeed = (inHighGear) ? highTopSpeed : lowTopSpeed;

            // Gets our current speed, for comparison
            float speed = rb.velocity.magnitude;

            // If we're slower than our current topSpeed
            if(speed < topSpeed)
            {
                // Then we accelerate
                rb.AddRelativeForce(
                    new Vector3(0f, 0f,
                        acceleration * moveInput.y),
                    ForceMode.Acceleration);
            }

            // If we're significantly faster than top speed
            // i.e. If we just switched down from high gear
            if(speed > topSpeed + slowFromHighSpeedBuffer)
            {
                // Then we decelerate
                rb.AddRelativeForce(
                    new Vector3(0f, 0f,
                        acceleration * -slowFromHighSpeedMult),
                    ForceMode.Acceleration);
            }

            // If we are not allowed to drive in reverse
            if (allowReverse == false)
            {
                // If our motion is backwards
                if (Vector3.Dot(transform.forward, rb.velocity) < 0)
                {
                    // We stop the motion
                    rb.velocity = Vector3.zero;
                }
            }
        }

        void Steering()
        {
            // Get's the z component of our motion,
            // In local car space
            // So an exact value for how fast we're moving forward
            // (and whether it's forward or backward)
            float zVelocity = transform.InverseTransformDirection(rb.velocity).z;

            // Gets a directional multiplier for the turning
            // When in reverse, turns are reversed
            int direction = (zVelocity >= 0) ? 1 : -1;

            // Gets the speed
            float speed = rb.velocity.magnitude;

            // Gets our rotation speed
            float rotateSpeed = moveInput.x * direction * turnSpeed * speed * turnVelocityImpact;

            // Rotates the vehicle
            transform.Rotate(0, rotateSpeed * Time.fixedDeltaTime, 0);

            // If we're moving forward
            if(direction > 0)
            {
                // We want to redirect the velocity
                rb.velocity = Vector3.Lerp(
                    rb.velocity,
                    transform.forward * speed,
                    redirectSpeedOnTurn);
            }

            // If we're auto-centering the rotation
            if(autoCenterRotation == true)
            {
                // We nudge the car direction towards that of the road
                transform.forward = Vector3.RotateTowards(
                    transform.forward,
                    roadDirection,
                    autoCenterRotationSpeed * Time.fixedDeltaTime,
                    0);
            }

            // If we're constraining the rotation
            if(constrainRotation == true)
            {
                // We get our angle compared to the road
                float angle = Vector3.SignedAngle(
                    roadDirection,
                    transform.forward,
                    Vector3.up);

                // If our angle is wider than our max set angle
                if(Mathf.Abs(angle) > constrainRotationMaxValue)
                {
                    // We find the amount we have to rotate by
                    float diff = (angle > 0) ?
                        angle + constrainRotationMaxValue :
                        angle - constrainRotationMaxValue;

                    // We rotate
                    transform.Rotate(0, diff, 0);
                }
            }
        }
    }
}