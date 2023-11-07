using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project_SL
{

    //ResourceManager를 보조함
    //리소스를 로드 하기 전에 풀에 있는지 체크하기.
    //존재하면 풀에 있는 것을 가져다 쓰고 돌려주기.
    //존재하지 않으면 새로 생성
    public class PoolManager
    {
        #region Pool
        class Pool
        {
            public GameObject Original { get; private set; }
            public Transform Root { get; set; }

            Stack<Poolable> _poolStack = new Stack<Poolable>();

            public void Init(GameObject original, int count = 5)
            {
                Original = original;
                Root = new GameObject().transform;
                Root.name = $"{original.name}_Root";

                for (int i = 0; i < count; i++)
                    Push(Create());
            }

            public Poolable Create()
            {
                GameObject go = Object.Instantiate<GameObject>(Original);
                go.name = Original.name;
                return go.GetComponent<Poolable>();
            }

            public void Push(Poolable poolable)
            {
                if (poolable == null)
                    return;
                poolable.transform.parent = Root;
                poolable.gameObject.SetActive(false);
                poolable.isUsing = false;

                _poolStack.Push(poolable);
            }

            public Poolable Pop(Transform parent)
            {
                Poolable poolable;

                if (_poolStack.Count > 0)
                    poolable = _poolStack.Pop();
                else
                    poolable = Create();

                poolable.gameObject.SetActive(true);
                poolable.transform.parent = parent;
                poolable.isUsing = true;

                return poolable;
            }
        }
        #endregion

        Dictionary<string, Pool> _pool = new Dictionary<string, Pool>();

        

        Transform _root;

        public void Init()
        {
            if(_root == null)
            {
                _root = new GameObject { name = "@PoolRoot" }.transform;
                Object.DontDestroyOnLoad(_root);
            }
        }



        public void CreatePool(GameObject origin, int count = 5)
        {
            //이미 존재하는 풀이라면 생성 하지 않는다.
            if (_pool.ContainsKey(origin.name))
                return;

            Pool pool = new Pool();
            pool.Init(origin, count);
            pool.Root.parent = _root;

            _pool.Add(origin.name, pool);
        }

        //사용했던 풀링 오브젝트를 다시 풀에 반환하는 메서드
        public void Push(Poolable poolable)
        {
            string name = poolable.gameObject.name;

            //만약 풀에 존재 하지 않는데 반환하려 하면 그냥 파괴시킨다.
            if(_pool.ContainsKey(name) == false)
            {
                GameObject.Destroy(poolable.gameObject);
                return;
            }

            _pool[name].Push(poolable);
        }
        

        public Poolable Pop(GameObject origin, Transform parent = null)
        {
            //없으면 새로 생성해 줘야 한다.
            if (_pool.ContainsKey(origin.name) == false)
            {
                CreatePool(origin);
            }

            return _pool[origin.name].Pop(parent);
        }

        //이름으로 해당 풀에 들어가 Original을 리턴
        public GameObject GetOriginal(string name)
        {
            GameObject original = null;
            if (_pool.ContainsKey(name))
            {
                original = _pool[name].Original;
            }

            return original;
        }

        //특정 오브젝트의 풀의 루트 오브젝트를 반환하는 메서드
        public Transform GetPoolRoot(string key)
        {
            if (!_pool.ContainsKey(key))
                return null;

            return _pool[key].Root;
        }

        public void Clear()
        {
            //루트 산하의 모든 오브젝트를 날린다.
            foreach(Transform child in _root)
            {
                GameObject.Destroy(child.gameObject);
            }

            _pool.Clear();
        }
    }
}
