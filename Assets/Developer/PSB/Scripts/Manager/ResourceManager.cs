using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

namespace Project_SL
{
    //Resource 폴더에서 에셋을 로드하고 생성하는 기능이 있는 클래스
    public class ResourceManager
    {
        //경로를 주면 해당 경로에 존재하는 Object 에셋을 메모리에 로드해서 리턴한다. 
        public T Load<T>(string path) where T : Object
        {
            //오브젝트 풀에 이미 로드되어있다면 그걸 그냥 가져다 쓴다.
            if(typeof(T) == typeof(GameObject))
            {
                string name = path;
                int idx = name.LastIndexOf('/');    //마지막의 '/'를 찾아서 인덱스 반환
                if (idx >= 0)
                    name = name.Substring(idx + 1);

                GameObject go = Managers.Pool.GetOriginal(name);
                if (go != null)
                    return go as T;
            }

            //로드하기
            T obj = Resources.Load<T>(path);
            if(obj == null)
            {
                Debug.Log("찾을 수 없는 경로입니다.");
            }
            return obj;
        }

        //Resource/Prefabs 산하의 생성하고 싶은 에셋의 이름을 매개변수로 전달하면 생성해주는 함수
        public GameObject Instantiate(string path, Transform parent = null)
        {

            GameObject origin = Load<GameObject>($"Prefabs/{path}");
            if(origin == null)
            {
                Debug.Log("fail to instantiate");
                return null;
            }

            //Poolable 오브젝트라면 풀 매니저에서 처리한다.
            if (origin.GetComponent<Poolable>() != null)
                return Managers.Pool.Pop(origin, parent).gameObject;

            GameObject go = Object.Instantiate(origin, parent);
            go.name = origin.name;
            return go;
        }

        // 게임오브젝트를 제거하는 메서드이다.
        public void Destroy(GameObject go)
        {
            if (go == null)
                return;

            Poolable poolable = go.GetComponent<Poolable>();
            //만약 풀링이 필요하다면 -> 풀링 매니저에게 위탁
            if (poolable != null)
            {
                Managers.Pool.Push(poolable);
                return;
            }
               

            Object.Destroy(go);
        }
    }
}
