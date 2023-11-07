using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;

public class PlayerSkill : MonoBehaviour
{
    [SerializeField]
    ParticleSystem magicCircleL;
    [SerializeField]
    ParticleSystem magicCircleR;
    [SerializeField]
    ParticleSystem chargeMpEffect;
    [SerializeField]
    Transform firePos;

    [SerializeField]
    List<Skill> skills;
    
    [SerializeField]
    int currentSkillIndex = 0;

    public int CurrentSkillIndex 
    {
        get { return currentSkillIndex; } 
        set 
        {
            currentSkillIndex = value;
            if (currentSkillIndex < 0)
                currentSkillIndex = skills.Count - 1;
            else
            {
                currentSkillIndex = currentSkillIndex % skills.Count;
            }
        } 
    }

    void Start()
    {
        TurnOffMagicCircle();
        chargeMpEffect.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UseSkill()
    {
        if (PlayerStatus.Player.PlayerMp < skills[currentSkillIndex].MpCost)
            return;

        TurnOnMagicCircle();
        print(currentSkillIndex);
        skills[currentSkillIndex].UseSkill(firePos);

        Invoke("TurnOffMagicCircle", 3.0f);
    }

    public void ChargeMpStart()
    {
        chargeMpEffect.Play();
        StartCoroutine(ChargeMpCoroutine());
    }

    IEnumerator ChargeMpCoroutine()
    {
        while(OVRInput.Get(OVRInput.RawButton.A))
        {
            yield return new WaitForSeconds(0.5f);
            PlayerStatus.Player.UseMp(-10.0f);
            yield return new WaitForSeconds(1.0f);
        }
        yield break;
    }
    public void ChargeMpStop()
    {
        chargeMpEffect.Stop();
        StopCoroutine(ChargeMpCoroutine());
    }

    public void TurnOnMagicCircle()
    {
        magicCircleL.Play();
        magicCircleR.Play();
    }
    public void TurnOffMagicCircle()
    {
        magicCircleL.Stop();
        magicCircleR.Stop();
    }
}
