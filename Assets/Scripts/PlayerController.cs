using System.Collections;
using System.Collections.Generic;
using System.Collections;
using RootMotion.FinalIK;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace xflip
{
    public class PlayerController : MonoBehaviour
    {    
        [Header("BOOLS")]
        private bool isOnGround;
        private bool isAccelerating;
        private bool flipping;
        private bool isBoosterActivated;
        private bool animIndex;

        [Header("RIGIDBODY")]
        [SerializeField] private Rigidbody PlayerRB;

        [Header("PARTICLES")]
        [SerializeField] private ParticleSystem Particle;
        [SerializeField] private ParticleSystem boostfireParticle;

        [Header("FLIP COUNT")]
        private float prevAngle;
        public int flips;
        public int totalflips;

        [Header("DRIVER")]
        public GameObject Player;

        [Header("PLACE")]
        [SerializeField] private TextMeshPro placeText;
        public int place;      
        public GameObject crown;

        [Header("Wheels and Colliders")]
        public WheelCollider[] whellColliders;
        public Transform[] wheels;

        [Header("VARIABLES")]
        [SerializeField] Text flipValueText;
        [SerializeField] Text totalflipValueText;
        [SerializeField] private float speed;
        [SerializeField] private float finalRotationSpeed;
        public float rotationSpeed = 900;
        public GameObject FinishTrigger;

        GameController GameController;
        UIController UIController;     
        public static PlayerController instance;

        /// <summary>
        /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        int animindex = 2;// Animasyonlarda hata var Mixamodan cikti alirken sorun olustu sadece 1 animasyon calisiyor. Ancak random icin calisan kodu ekledim.
        Animator animator;
        /// <summary>
        /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>

        private void Awake()
        {
            if (!instance)
            {
                instance = this;
            }
        }

        private void Start()
        {
            GameController = GameController.instance;
            UIController = UIController.instance;
            PlayerRB = gameObject.GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
        }

        void FixedUpdate()
        {
            if (GameController._isLevelStart && !GameController._isLevelDone && !GameController._isLevelFail)
            {               
                WheelRotation();

                if (isOnGround)
                {

                    if (flips > 1)
                    {
                        isBoosterActivated = true;
                        StartCoroutine(Booster());
                    }
                    else
                    {
                        flips = 0;
                    }

                    if (isAccelerating)
                    {
                        speed = Input.GetAxis("Fire1");
                        Move(speed);
                    }

                    else
                    {
                        Move(speed);
                    }
                }

                else if (!isOnGround)
                {

                    if (flipping)
                    {
                        transform.RotateAround(transform.position, Vector3.right * -5, 10.5f);
                        CalculateFlip();
                    }
                }
            }

            else if (!GameController._isLevelStart && GameController._isLevelDone)
            {
                PlayerRB.mass = 1600;
                Particle.Stop();
                
                if (PlayerRB.angularVelocity.magnitude > 0)
                {
                    PlayerRB.angularVelocity = new Vector3(-25f, 0, 0);
                }
            }
        }

        private void Update()
        {
           

            if (GameController._isLevelStart && !GameController._isLevelFail && !GameController._isLevelDone)
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
                
                RayCast();

                if (isOnGround)
                {
                    Particle.Play();
                    animIndex = false;
                    Player.GetComponent<FullBodyBipedIK>().enabled = true;
                    Player.GetComponentInParent<Animator>().SetBool("flipping", false);
                    if (Input.GetMouseButton(0))
                    {
                        isAccelerating = true;
                    }

                    else
                    {
                        isAccelerating = false;
                    }
                }

                else if (!isOnGround)
                {
                    Particle.Stop();
                    isAccelerating = false;

                    if (PlayerRB.angularVelocity.magnitude > 0)
                    {
                        PlayerRB.angularVelocity = new Vector3(-0.25f, 0, 0);
                    }

                    if (Input.GetMouseButton(0))
                    {
                        flipping = true;

                        if (!animIndex)
                        {                           
                            /*animIndex = true;
                            Player.GetComponentInParent<Animator>().SetInteger("random", Random.Range(0, 2));
                            //animindex = Random.Range(0, 2);*/
                            
                            if (animindex == 2)
                            {
                                animator.Play("hiphop");
                            }

                        }

                        //Player.GetComponent<FullBodyBipedIK>().enabled = false;
                        Player.GetComponentInParent<Animator>().SetBool("flipping", true);
                    }

                    else
                    {
                        flipping = false;
                    }
                }

            }
        }

        void Move(float acceleration)
        {
            acceleration = Mathf.Clamp(acceleration, -1, 1);
            finalRotationSpeed = acceleration * rotationSpeed;

            for (int i = 0; i < 2; i++)
            {
                whellColliders[i].motorTorque = finalRotationSpeed;
            }
        }

        void WheelRotation()
        {
            wheels[0].transform.Rotate(500 * Time.deltaTime, 0, 0);
            wheels[1].transform.Rotate(500 * Time.deltaTime, 0, 0);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.gameObject.CompareTag("Ground"))
            {
                Player.GetComponentInParent<Animator>().enabled = false;
                Player.GetComponent<FullBodyBipedIK>().enabled = false;
                gameObject.tag = "Untagged";
                GameController.FailMethod();
                //Player.transform.SetParent(null);             

            }

            else if (other.CompareTag("FinishTrigger"))
            {
                if (place == 1)
                {
                    GameController.FinishLevel();
                }

                else
                {
                    GameController._isFinishFail = true;
                    GameController.FailMethod();
                }
            }
        }

        void RayCast()
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position + Vector3.up, transform.TransformDirection(Vector3.down), out hit, 7.5f))
            {
                if (hit.transform.gameObject.CompareTag("Ground"))
                {
                    isOnGround = true;
                }
            }

            else
            {
                isOnGround = false;
            }
        }

        private void CalculateFlip()
        {
            Vector3 dir = transform.right;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if ((prevAngle > -10.0f && prevAngle < 0.0f) && angle >= 0.0)
            {               
                flips++;
                totalflips++;
                Debug.Log("FLIPS: " + flips);
                //flips = int.Parse(flipValueText.text);
                flipValueText.text = totalflips.ToString();
                totalflipValueText.text = flips.ToString();
                UIController.TotalFlipText.text = flips.ToString();
            }
            prevAngle = angle;
        }

        IEnumerator Booster()
        {
            if (isBoosterActivated)
            {
                isBoosterActivated = false;
                rotationSpeed = 800;
                boostfireParticle.Play();
                yield return new WaitForSeconds(flips * 2.2f);
                boostfireParticle.Stop();
                rotationSpeed = 600;
                flips = 0;
            }
        }
    }
}
