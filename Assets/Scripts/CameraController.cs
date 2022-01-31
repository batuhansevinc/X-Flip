using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace xflip
{
    public class CameraController : MonoBehaviour
    {
        [Header("Settings")]
        public Vector3 offSet;
        private Vector3 targetPos;
        public float smoothness;
        private Transform TargetPos;
        private Vector3 finalPos;
        public GameObject FinishTrigger;

        GameController GameController;
        PlayerController PlayerController;
        public static CameraController instance;

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

            GameController = GameController.instance;
            PlayerController = PlayerController.instance;
            TargetPos = PlayerController.transform;
            finalPos = FinishTrigger.transform.position;
            finalPos.y += 15f;
            finalPos.x += 15f;
        }

        // Update is called once per frame
        public void LateUpdate()
        {
            if (GameController._isLevelStart && !GameController._isLevelDone && !GameController._isLevelFail && !GameController._isFinishFail)
            {
                targetPos = TargetPos.transform.position + offSet;

                transform.position = Vector3.Lerp(transform.position, targetPos, smoothness);
            }

            else if (GameController._isLevelDone || GameController._isFinishFail)
            {
                transform.LookAt(TargetPos);
                transform.position = Vector3.Lerp(transform.position, finalPos, Time.deltaTime);
            }
        }
    }
}