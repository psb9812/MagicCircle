using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Project_SL
{

    //모든 매니저를 관리하는 매니저 클래스이다.
    //싱글톤으로 구현하여 여기서 모든 매니저들을 접근할 프로퍼티를 가진다.
    public class Managers : MonoBehaviour
    {
        static Managers s_ManagerInstance;
        
        public static Managers ManagerInstance { get { Init(); return s_ManagerInstance; } }

        SoundManager _sound = new SoundManager();
        ResourceManager _resource = new ResourceManager();
        SceneManagerEx _scene = new SceneManagerEx();
        SaveManager _save = new SaveManager();
        PoolManager _pool = new PoolManager();

        public static SoundManager Sound { get { return ManagerInstance._sound; } }
        public static ResourceManager Resource { get { return ManagerInstance._resource; } }
        public static SceneManagerEx Scene { get { return ManagerInstance._scene; } }
        public static SaveManager Save { get { return ManagerInstance._save; } }
        public static PoolManager Pool { get { return ManagerInstance._pool; } }

        static bool isPlayerStop = false;

        public static bool PlayerStop { get { return isPlayerStop; } set { isPlayerStop = value; } }

        private void Awake()
        {
            Init();
        }

        private void Start()
        {
            s_ManagerInstance._sound.Init();
            s_ManagerInstance._save.Init();
        }

        private void Update()
        {
        }
        //Managers를 초기화 해주는 함수.
        //_instance가 초기화가 되지 않았다면 @Managers를 찾고 (없다면 생성)
        //_instance를 초기화 한 후 @Managers를 DontDestroyOnLoad로 처리한다.
        static void Init()
        {
            if(s_ManagerInstance == null)
            {
                GameObject gm = GameObject.Find("@Manager");
                if (gm == null)
                {
                    gm = new GameObject { name = "@Manager" };
                    gm.AddComponent<Managers>();
                }

                s_ManagerInstance = gm.GetComponent<Managers>();
                DontDestroyOnLoad(gm);

                s_ManagerInstance._pool.Init();
            }
        }

        public static void Clear()
        {
            Sound.Clear();
            //Scene.Clear();
            Pool.Clear();
        }
    }
}