using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerBeam : Skill
{
    bool _isUsingBeam = false;
    ParticleSystem _particleSystem;

    public override void UseSkill(Transform spawnPosition)
    {
        Instantiate(gameObject, spawnPosition.position, Quaternion.LookRotation(Camera.main.transform.forward), spawnPosition);
        
    }

    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }
    // Start is called before the first frame update
    void Start()
    {
        print("start");
        _isUsingBeam = true;
        _particleSystem.Play();
        StartCoroutine(UseMpCo());
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetUp(OVRInput.RawButton.B))
        {
            _isUsingBeam = false;
            Destroy(gameObject);
            print("destroy");
        }

        
    }

    IEnumerator UseMpCo()
    {
        while(PlayerStatus.Player.PlayerMp > mpCost && _isUsingBeam)
        {
            print("useMp");
            yield return new WaitForSeconds(1.0f);
            _particleSystem.Play();
            PlayerStatus.Player.UseMp(mpCost);
        }

        _isUsingBeam = false;
        Destroy(gameObject);
        print(PlayerStatus.Player.PlayerMp + "    " + mpCost + "   "  + _isUsingBeam);

        print("destroy");
    }
}
