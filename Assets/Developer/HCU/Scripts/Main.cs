using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project_SL
{
    public class Main : MonoBehaviour
    {
        public void ExitIntro()
        {
            SceneManager.LoadScene("Cliff");
        }
    }
}
