using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project_SL
{
    public class Portal : MonoBehaviour
    {
        [SerializeField]
        Define.Scene nextScene;
        //public FadeScript fade;

        //트리거에 들어오면 로딩씬으로 넘김
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Managers.PlayerStop = true;
                //fade.FadeOut();
            }
        }
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                //if (fade.IsBlack)
                    //Managers.Scene.LoadScene(Define.Scene.LoadingScene, nextScene);
            }
        }
    }
}
