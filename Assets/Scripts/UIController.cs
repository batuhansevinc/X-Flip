using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace xflip
{
    public class UIController : MonoBehaviour
    {
        [Header("Start Panel")]
        public GameObject TapToStartPanel;

        [Header("In Game Panel")]
        public GameObject InGamePanel;
        public Slider slider;

        [Header("Win Panel")]
        public GameObject WinPanel;

        [Header("Lose Panel")]
        public GameObject LosePanel;

        [Header("Timer")]
        public Text TimerText;
        public Text TimerLabel;
        public Text TotalFlipLabel;
        public Text TotalFlipText;
        private float time;

        GameController GameController;
        PlayerController PlayerController;

        public static UIController instance;

        private void Awake()
        {
            if (!instance)
            {
                instance = this;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            PlayerController = PlayerController.instance;
            GameController = GameController.instance;
            ShowStartPanel();
            SliderMaxValue();
        }

        private void Update()
        {
            if (GameController._isLevelStart && !GameController._isLevelFail && !GameController._isLevelDone)
            {
                slider.value = PlayerController.transform.position.z;

                time += Time.deltaTime;
                TimerText.text = time.ToString("F2");
            }
        }

        void SliderMaxValue()
        {
            slider.maxValue = GameController.FinishTrigger.transform.position.z;
        }

        void ShowStartPanel()
        {
            TapToStartPanel.SetActive(true);
        }

        void CloseStartPanel()
        {
            TapToStartPanel.SetActive(false);
        }

        public void ButtonActionStart()
        {
            CloseStartPanel();
            GameController.StartMethod();
            ShowInGamePanel();
        }

        void ShowInGamePanel()
        {
            InGamePanel.SetActive(true);
        }

        public void ShowWinPanel()
        {
            InGamePanel.SetActive(false);
            WinPanel.SetActive(true);
        }

        public void ShowLosePanel()
        {
            StartCoroutine(WaitForLosePanel());
        }

        IEnumerator WaitForLosePanel()
        {
            TimerText.transform.SetParent(LosePanel.transform);
            TimerLabel.transform.SetParent(LosePanel.transform);
            TotalFlipLabel.transform.SetParent(LosePanel.transform);
            TotalFlipText.transform.SetParent(LosePanel.transform);
            yield return new WaitForSeconds(0.5f);
            InGamePanel.SetActive(false);
            LosePanel.SetActive(true);
        }
    }
}