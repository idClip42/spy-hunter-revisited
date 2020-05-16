using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpyHunter.Car
{

    [RequireComponent(typeof(Rigidbody))]
    public class Car : MonoBehaviour
    {
        [Header("Movement")]
        public bool allowReverse = false;
        public bool freezeXInAir = true;

        [Header("Speed and Acceleration")]
        public float lowTopSpeed = 50;
        public float highTopSpeed = 150;
        public float acceleration = 60;
        public float slowFromHighSpeedBuffer = 10;
        public float slowFromHighSpeedMult = 2;

        [Header("Steering")]
        public float turnSpeed = 20;
        public float turnVelocityImpact = 0.1f;
        [Range(0, 1)] public float redirectSpeedOnTurn = 1;
        public bool autoCenterRotation = true;
        public float autoCenterRotationSpeed = 4;
        public bool constrainRotation = true;
        public float constrainRotationMaxValue = 60;

        [Header("Damage")]
        public float spinOutTorque = 100000000;
        public float crashSparksMinSpeed = 10;
        public float crashDeathMinSpeed = 75;
        public float crashEffectLifetime = 5;
        public GameObject flamesOnCar;
        public GameObject sparksPrefab;
        public AudioClip crashSound;
        public float crashSoundVolume = 0.05f;

        [Header("Ground")]
        public string[] groundTags = { "Ground", "SideGround" };
        public string[] deadlyGroundTags = { "SideGround" };
        public string roadTriggerBoxTag = "RoadTriggerBox";

        public bool Alive { get; protected set; } = true;
        public Vector3 Velocity { get { return rb.velocity; } }
        public Vector3 AngularVelocity { get; private set; }

        protected bool inHighGear = false;
        protected Vector2 moveInput;

        Rigidbody rb;
        RigidbodyConstraints startConstraints;
        Vector3 roadDirection;
        public bool Grounded { get; private set; } = true;





        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody>();
            startConstraints = rb.constraints;
        }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {

        }

        protected void Die()
        {
            Alive = false;
            rb.constraints = RigidbodyConstraints.None;

            if (flamesOnCar != null)
                flamesOnCar.SetActive(true);
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
            if (speed < topSpeed)
            {
                // Then we accelerate
                rb.AddRelativeForce(
                    new Vector3(0f, 0f,
                        acceleration * moveInput.y),
                    ForceMode.Acceleration);
            }

            // If we're significantly faster than top speed
            // i.e. If we just switched down from high gear
            if (speed > topSpeed + slowFromHighSpeedBuffer)
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

            AngularVelocity = new Vector3(0, rotateSpeed, 0);

            // Rotates the vehicle
            transform.Rotate(AngularVelocity * Time.fixedDeltaTime);

            // If we're moving forward
            if (direction > 0)
            {
                // We want to redirect the velocity
                rb.velocity = Vector3.Lerp(
                    rb.velocity,
                    transform.forward * speed,
                    redirectSpeedOnTurn);
            }

            // If we're auto-centering the rotation
            if (autoCenterRotation == true)
            {
                // We nudge the car direction towards that of the road
                transform.forward = Vector3.RotateTowards(
                    transform.forward,
                    roadDirection,
                    autoCenterRotationSpeed * Time.fixedDeltaTime,
                    0);
            }

            // If we're constraining the rotation
            if (constrainRotation == true)
            {
                // We get our angle compared to the road
                float angle = Vector3.SignedAngle(
                    roadDirection,
                    transform.forward,
                    Vector3.up);

                // If our angle is wider than our max set angle
                if (Mathf.Abs(angle) > constrainRotationMaxValue)
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

        protected void SpinOut(Vector3 direction)
        {
            Die();
            rb.AddTorque(0, spinOutTorque, 0);
            rb.velocity = rb.velocity + direction;
        }


        static bool ArrayContains<T>(T[] array, T item) 
        {
            for (int n = 0; n < array.Length; ++n)
                if (array[n].Equals(item))
                    return true;

            return false;
        }



        void SetGrounded(bool isG)
        {
            Grounded = isG;
            if (freezeXInAir)
            {
                rb.constraints = isG ?
                    startConstraints :
                    RigidbodyConstraints.FreezeRotationX;
            }
        }

        void SetRoadDirection(GameObject road)
        {
            roadDirection = road.transform.forward;
        }



        protected virtual void OnCollisionEnter(Collision col)
        {
            // Determines if we've collided with the ground
            bool collidedWithGround = ArrayContains(groundTags, col.gameObject.tag);

            // If we have entered contact with the ground
            if (collidedWithGround)
            {
                // We set ourselves as grounded
                SetGrounded(true);

                // We set our general road direction with this ground
                SetRoadDirection(col.gameObject);
            }

            // If we have collided with ground of a type
            // that is deadly to cars,
            // we die
            if(ArrayContains(deadlyGroundTags, col.gameObject.tag))
            {
                Die();
            }

            // We get the relative speed of the collision
            float collisionSpeed = col.relativeVelocity.magnitude;

            if(collisionSpeed > crashSparksMinSpeed)
            {
                if (crashSound != null)
                {
                    AudioSource.PlayClipAtPoint(
                        crashSound,
                        this.transform.position,
                        crashSoundVolume
                        );
                }

                if (sparksPrefab != null)
                {
                    GameObject spark = (GameObject)Instantiate(
                        sparksPrefab,
                        col.contacts[0].point,
                        Quaternion.identity
                        );
                    Destroy(spark, crashEffectLifetime);
                }
            }

            if(collisionSpeed > crashDeathMinSpeed)
            {
                Die();
            }
        }

        protected virtual void OnCollisionStay(Collision col)
        {
            // If we have entered contact with the ground
            if (ArrayContains(groundTags, col.gameObject.tag))
            {
                // We set ourselves as grounded
                SetGrounded(true);
            }
        }

        protected virtual void OnCollisionExit(Collision col)
        {
            if (ArrayContains(groundTags, col.gameObject.tag))
            {
                SetGrounded(false);
            }
        }


        // Probably don't need this, as we can set this direction via collision, right?
        //protected virtual void OnTriggerEnter(Collider c)
        //{
        //    // If we've passed through a road trigger
        //    if (c.gameObject.tag == roadTriggerBoxTag)
        //    {
        //        // Don't yet know what this does
        //        //roadScript.updatePlayerPosition(c.gameObject.transform.parent.gameObject);

        //        // Orients the understood direction of the road
        //        // with the one indicated by the trigger
        //        roadDirection = c.gameObject.transform.parent.forward;
        //    }
        //}

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (rb == null) return;
            string text = "";
            text += "Velocity: " + Velocity + "\n";
            text += "Angular Velocity: " + AngularVelocity + "\n";
            text += "Grounded: " + Grounded + "\n";
            UnityEditor.Handles.Label(transform.position, text);
        }
#endif
    }
}