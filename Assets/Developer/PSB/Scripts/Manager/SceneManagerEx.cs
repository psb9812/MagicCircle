using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project_SL
{
    public class SceneManagerEx
    {
        //현재 씬 프로퍼티
        //public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }

        string GetSceneName(Define.Scene type)
        {
            string name = System.Enum.GetName(typeof(Define.Scene), type); // C#의 Reflection
            return name;
        }

        //Enum 타입의 원소를 받아서 씬을 로드한다.
        public void LoadScene(Define.Scene type)
        {
            Managers.Clear();

            SceneManager.LoadScene(GetSceneName(type));
        }

        //로딩씬으로 갈 때 호출할 LoadScene
        public void LoadScene(Define.Scene loadScene, Define.Scene nextScene)
        {
            Managers.Clear();

            //다음 씬의 씬 이름을 static 변수에 할당함.
            //LoadingScene.nextScene = nextScene;

            SceneManager.LoadScene(GetSceneName(loadScene));
        }

        public AsyncOperation LoadSceneAsync(Define.Scene type)
        {
            return SceneManager.LoadSceneAsync(GetSceneName(type));
        }


        public void Clear()
        {
            //CurrentScene.Clear();
        }
    }
}
