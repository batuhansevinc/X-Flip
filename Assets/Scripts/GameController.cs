using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace xflip
{
    public class GameController : MonoBehaviour
    {
        [Header("BOOLS")]
        public bool _isLevelStart;
        public bool _isLevelDone;
        public bool _isLevelFail;
        public bool _isFinishFail;

        [Header("FINISH TRIGGER")]
        public GameObject FinishTrigger;

        [Header("LEVELS")]
        public int level = 1;
        public int randomIndex;
        
        [Header("DRIVERS POSITIONS")]
        [SerializeField] private float Driver;
        [SerializeField] private float Driver1;
        [SerializeField] private float Driver2;
        [SerializeField] private float Driver3;
        [SerializeField] private float Driver4;
        [SerializeField] private List<float> values = new List<float>();

        public GameObject Player;
        public GameObject AiManager;
        [SerializeField] private List<Transform> Drivers = new List<Transform>();
        [SerializeField] public List<GameObject> AIS = new List<GameObject>();
        UIController UIController;
        PlayerController PlayerController;
        public static GameController instance;

        private void Awake()
        {
            if (!instance)
            {
                instance = this;
            }
            values.Add(Driver);
            values.Add(Driver1);
            values.Add(Driver2);
            values.Add(Driver3);
            values.Add(Driver4);
        }

        // Start is called before the first frame update
        void Start()
        {
            AIPlacer();
            UIController = UIController.instance;
            PlayerController = PlayerController.instance;
            Player = GameObject.FindWithTag("Player");
        }

        void Update()
        {
            Driver = PlayerController.transform.position.z;
            Driver1 = AiManager.transform.GetChild(0).transform.position.z;
            Driver2 = AiManager.transform.GetChild(1).transform.position.z;
            Driver3 = AiManager.transform.GetChild(2).transform.position.z;
            Driver4 = AiManager.transform.GetChild(3).transform.position.z;

            values[0] = Driver;
            values[1] = Driver1;
            values[2] = Driver2;
            values[3] = Driver3;
            values[4] = Driver4;

            values.Sort();
            SortNum();
        }

        void SortNum()
        {
            PlayerController.place = Math.Abs(values.IndexOf(Driver) - 5);
            AiManager.transform.GetChild(0).GetComponent<AIController>().place = Math.Abs(values.IndexOf(Driver1) - 5);
            AiManager.transform.GetChild(1).GetComponent<AIController>().place = Math.Abs(values.IndexOf(Driver2) - 5);
            AiManager.transform.GetChild(2).GetComponent<AIController>().place = Math.Abs(values.IndexOf(Driver3) - 5);
            AiManager.transform.GetChild(3).GetComponent<AIController>().place = Math.Abs(values.IndexOf(Driver4) - 5);
        }

        private void AIPlacer()
        {
            for (int i = 0; i < AiManager.gameObject.transform.childCount; i++)
            {
                AIS.Add(AiManager.transform.GetChild(i).gameObject);
            }
        }

        public void StartMethod()
        {
            _isLevelStart = true;
            Drivers.Add(PlayerController.gameObject.transform);
            for (int i = 0; i < AIS.Count; i++)
            {
                Drivers.Add(AIS[i].transform);
            }
        }

        public void FailMethod()
        {
            _isLevelStart = false;
            _isLevelFail = true;
            UIController.ShowLosePanel();          
        }

        public void FinishLevel()
        {
            _isLevelStart = false;
            _isLevelDone = true;
            UIController.ShowWinPanel();        
        }

        public void NextLevel()
        {
            if (_isLevelDone)
            {

                if (PlayerPrefs.GetInt("Level") < 5)
                {
                    PlayerPrefs.SetInt("Level", (PlayerPrefs.GetInt("Level") + 1));
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                }

                else
                {
                    randomIndex = GetRandom();

                    if (randomIndex == SceneManager.GetActiveScene().buildIndex + 1)
                    {
                        randomIndex = GetRandom();
                    }

                    SceneManager.LoadScene(randomIndex);
                }
            }

            else if (_isLevelFail)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        private int GetRandom()
        {
            int random = UnityEngine.Random.Range(0, SceneManager.sceneCountInBuildSettings);

            if (random == SceneManager.GetActiveScene().buildIndex)
            {
                GetRandom();
            }
            return random;
        }
    }
}