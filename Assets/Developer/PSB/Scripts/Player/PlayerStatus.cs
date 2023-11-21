using Project_SL;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerStatus : MonoBehaviour
{
    static PlayerStatus _player;

    private float _maxHp = 100.0f;
    private float _currentHp = 100.0f;
    private float _maxMp = 100.0f;
    private float _currentMp = 100.0f;
    private bool _isImmune = false;
    private bool _isDie = false;
    private float _immuneDuration = 2.0f;
    

    [SerializeField]
    private MeshRenderer _attackedScreen;
    [SerializeField]
    private Slider _hpSlider;
    [SerializeField]
    private Slider _mpSlider;
    [SerializeField]
    private GameObject _dieText;

    static public PlayerStatus Player { get { return _player; } }
    public float MaxPlayerHp { get { return _maxHp; } set { _maxHp = value; } }
    public float PlayerHp { get { return _currentHp; } set { _currentHp = value; } }
    public float MaxPlayerMp { get { return _maxMp; } set { _maxMp = value; } }
    public float PlayerMp { get { return _currentMp; } set { _currentMp = value; } }
    public bool IsDie { get { return _isDie; } set { _isDie = value;} }


    private void Awake()
    {
        if (_player == null)
        {
            _player = this;
        }
        else
            Destroy(gameObject);

        IsDie = false;
        Time.timeScale = 1.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerHp <= 0)
        {
            PlayerDie();
        }
    }

    public void PlayerDie()
    {
        IsDie = true;
        Time.timeScale = 0.0f;
        _dieText.SetActive(true);

        Invoke("LoadSceneInGame", 3.0f);
    }

    public void LoadSceneInGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.CompareTo("EnemyAttack") == 0 && !_isImmune)
        {
            OnHit(other.gameObject.GetComponent<AttackStatus>().Damage);
        }
    }

    public void OnHit(float damage)
    {
        StartCoroutine(StartImmune(_immuneDuration));
        TakeDamage(damage);
        StartCoroutine(AttackScreenOn());
    }
    private void TakeDamage(float damage)
    {
        float retHp = _currentHp - damage;
        float startHP = (float)_currentHp / (float)_maxHp;
        float endHp = (float)retHp / (float)_maxHp;

        StartCoroutine(DecreaseHp(startHP, endHp, 1.0f));
        _currentHp = retHp;

        if (_currentHp <= 0)
            Die();
    }
    IEnumerator DecreaseHp(float startHp, float endHp, float time)
    {
        float per = 0.0f;
        float cur = 0.0f;
        while(per < 1.0f)
        {
            cur += Time.deltaTime;
            per = cur / time;

            _hpSlider.value = Mathf.Lerp(startHp, endHp, per);

            yield return null;
        }
    }
    public void UseMp(float cost)
    {
        float retMp = _currentMp - cost;
        float startMP = (float)_currentMp / (float)_maxMp;
        float endMp = (float)retMp / (float)_maxMp;
        if (endMp > 1.0f)
        {
            endMp = 1.0f;
        }
            

        StartCoroutine(DecreaseMp(startMP, endMp, 1.0f));
        _currentMp = retMp;
        if (_currentMp >= _maxMp)
            _currentMp = _maxMp;
    }
    IEnumerator DecreaseMp(float startMp, float endMp, float time)
    {
        float per = 0.0f;
        float cur = 0.0f;
        while (per < 1.0f)
        {
            cur += Time.deltaTime;
            per = cur / time;

            _mpSlider.value = Mathf.Lerp(startMp, endMp, per);

            yield return null;
        }
    }

    IEnumerator AttackScreenOn()
    {
        _attackedScreen.enabled = true;
        yield return new WaitForSeconds(0.5f);
        _attackedScreen.enabled = false;
    }

    IEnumerator StartImmune(float duration)
    {
        _isImmune = true;
        yield return new WaitForSeconds(duration);
        _isImmune = false;
    }

    void Die()
    {

    }
}
