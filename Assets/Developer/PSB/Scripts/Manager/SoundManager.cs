using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Project_SL
{
    public class SoundManager
    {
        //AudioSource가 여러 개일 수 있으므로 이를 배열로 관리한다. 
        AudioSource[] _audioSources = new AudioSource[(int)Define.Sound.MaxCount - 1];
        //효과음 사운드는 자주 쓰기 때문에 미리 로드하여 보관하기 위한 딕셔너리이다. 키는 경로이름이고 값은 AudioClip이다.
        Dictionary<string, AudioClip> _EffectSoundClips = new Dictionary<string, AudioClip>();

        public AudioMixer audioMixer; 
        
        public void Init()
        {
            audioMixer = Managers.Resource.Load<AudioMixer>("Sounds/AudioMixer");
            AudioMixerGroup BGMGroup = audioMixer.FindMatchingGroups("Master")[1];
            AudioMixerGroup SFXGroup = audioMixer.FindMatchingGroups("Master")[2];

            GameObject root = GameObject.Find("@Sound");
            if(root == null)
            {
                root = new GameObject { name = "@Sound" };
                Object.DontDestroyOnLoad(root);

                //soundNames 사운드 소스의 이름이 정의된 Define.Sound 열거체에서 이름을 가져와 저장
                string[] soundNames = System.Enum.GetNames(typeof(Define.Sound));

                for(int i = 0; i < soundNames.Length - 1; i++)
                {
                    //Sound3D타입의 소리는 사운드 매니저에서 발생하는 것이 아니므로 등록하지 않는다.
                    if (soundNames[i] == "Sound3D") continue;   
                    GameObject go = new GameObject { name = soundNames[i] };
                    _audioSources[i] = go.AddComponent<AudioSource>();
                    //오디오믹서 등록
                    if (i == (int)Define.Sound.Effect)
                        _audioSources[i].outputAudioMixerGroup = SFXGroup;
                    else if(i == (int)Define.Sound.Bgm)
                        _audioSources[i].outputAudioMixerGroup = BGMGroup;

                    go.transform.parent = root.transform;
                }
                //Bgm은 무한 반복
                _audioSources[(int)Define.Sound.Bgm].loop = true;
            }
        }

        public void Play(AudioClip audioClip, Define.Sound type = Define.Sound.Effect, float volume = 1.0f, AudioSource targetSource = null)
        {

            if (audioClip == null)
                return;

            //BGM 배경음악 재생
            if (type == Define.Sound.Bgm)    
            {
                AudioSource audioSource = _audioSources[(int)Define.Sound.Bgm];
                if (audioSource.isPlaying)
                    audioSource.Stop();

                audioSource.volume = volume;
                audioSource.clip = audioClip;
                audioSource.Play();
            }
            else if(type == Define.Sound.Effect)    //효과음 재생
            {
                AudioSource audioSource = _audioSources[(int)Define.Sound.Effect];
                audioSource.volume = volume;
                audioSource.PlayOneShot(audioClip);
            }
            else if(type == Define.Sound.Sound3D)
            {
                if (targetSource == null)
                {
                    Debug.Log("소리를 출력할 오디오 소스를 찾을 수 없습니다.");
                    return;
                }
                targetSource.clip = audioClip;
                targetSource.volume = volume;
                targetSource.Play();
            }
        }

        //경로를 주면 사운드 플레이 해주는 함수 + 출력할 오디오 소스를 준다면 그 소스에서 소리를 출력하는 기능 추가
        public void Play(string path, Define.Sound type = Define.Sound.Effect,
            float volume = 1.0f, AudioSource targetSource = null)
        {
            AudioClip audioClip = GetOrAddAudioClip(path, type);
            Play(audioClip, type, volume, targetSource);
        }

        //경로를 주면 Resource 폴더 경로 타고 오디오 클립을 리턴해주는 함수
        AudioClip GetOrAddAudioClip(string path, Define.Sound type = Define.Sound.Effect)
        {
            if (path.Contains("Sounds/") == false)
                path = $"Sounds/{path}";

            AudioClip audioClip = null;

            if (type == Define.Sound.Bgm)
            {
                audioClip = Managers.Resource.Load<AudioClip>(path);
            }
            else
            {
                //효과음 딕셔너리에 없다면 추가해 주는 작업
                if (_EffectSoundClips.TryGetValue(path, out audioClip) == false)
                {
                    audioClip = Managers.Resource.Load<AudioClip>(path);
                    _EffectSoundClips.Add(path, audioClip);
                }
            }

            if (audioClip == null)
                Debug.Log($"AudioClip Missing ! {path}");

            return audioClip;
        }

        // 씬 이동할 때나 음악을 멈추기 위한 함수
        public void Clear()
        {
            //재생기 전부 멈추기, 음반 빼기
            foreach (var audioSource in _audioSources)
            {
                audioSource.clip = null;
                audioSource.Stop();
            }
            //효과음 Dictionary 비우기
            _EffectSoundClips.Clear();
        }
    }
}
