using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
using TMPro;
namespace xflip
{
    public class AIController : MonoBehaviour
    {
        [Header("BOOLS")]
        private bool isOnGround;
        private bool isBoostActive;

        [Header("LAYER MASK")]
        [SerializeField] LayerMask groundLayer;

        [Header("PARTICLES")]
        public ParticleSystem boostParticle;

        [Header("PLACE")]
        public int place;
        [SerializeField] private TextMeshPro placeText;

        [Header("CROWN")]
        public GameObject crown;

        [Header("FLIP COUNT")]
        [SerializeField] public int flipCount;
        private float prevAngle;
        private int flips;

        [Header("VARIABLES")]
        [SerializeField] private Transform groundCheckPosition;

        [Header("Rigidbody")]
        Rigidbody RB;

        [Header("AI DRIVERS")]
        [SerializeField] private GameObject AIDriver;

        [Header("WheelColliders & Settings")]
        public WheelCollider[] WC;
        public GameObject[] Wheels;
        public float torque;
        [SerializeField] private float speed;
        [SerializeField] private float finalTorque;

        GameController GameController;
        // Start is called before the first frame update
        void Start()
        {
            GameController = GameController.instance;
            StartCoroutine(RandomSpeed());
            GetRB();
        }
            
        void GetRB()
        {
            RB = gameObject.GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (GameController._isLevelStart && !GameController._isLevelDone && !GameController._isLevelFail)
            {
                WheelRotation();

                if (isOnGround)
                {

                    if (flipCount > 1)
                    {
                        isBoostActive = true;
                        StartCoroutine(SetBoost());
                    }
                    else
                    {
                        flipCount = 0;
                    }

                    Move(speed);
                }

                else if (!isOnGround)
                {
                    CalculateFlip();
                }
            }
        }

        private void Update()
        {
            if (GameController._isLevelStart && !GameController._isLevelDone && !GameController._isLevelFail)
            {
                if (place == 1)
                {
                    crown.SetActive(true);
                }
                else
                {
                    crown.SetActive(false);
                }
                placeText.text = place.ToString();
                IsGrounded();
            }
        }
        private void IsGrounded()
        {
            RaycastHit _raycastHit;
            if (Physics.Raycast(groundCheckPosition.position, -transform.up, out _raycastHit, 3f))
            {
                isOnGround = true;
            }
            else
            {
                isOnGround = false;
            }
        }

        void Move(float acceleration)
        {
            acceleration = Mathf.Clamp(acceleration, -1, 1);
            finalTorque = acceleration * torque;

            for (int i = 0; i < 2; i++)
            {
                WC[i].motorTorque = finalTorque;
            }
        }
        void WheelRotation()
        {
                Wheels[0].transform.Rotate(500 * Time.deltaTime, 0, 0);
                Wheels[1].transform.Rotate(500 * Time.deltaTime, 0, 0);          
        }

        private void CalculateFlip()
        {
            Vector3 dir = transform.right;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if ((prevAngle > -10.0f && prevAngle < 0.0f) && angle >= 0.0)
            {
                flips++;
            }
            prevAngle = angle;
        }

        IEnumerator SetBoost()
        {
            if (isBoostActive)
            {
                isBoostActive = false;
                torque = 900;
                boostParticle.Play();
                yield return new WaitForSeconds(flipCount);
                boostParticle.Stop();
                torque = 650;
                flipCount = 0;
            }
        }

        IEnumerator RandomSpeed()
        {
            while (!GameController._isLevelDone)
            {
                speed = Random.Range(0.4f, 1f);
                yield return new WaitForSeconds(1f);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.gameObject.CompareTag("Ground"))
            {
                if (!GameController._isLevelFail)
                {
                    gameObject.tag = "Untagged";
                    AIDriver.GetComponent<FullBodyBipedIK>().enabled = false;
                }
            }
        }
    }
}

