using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;

public class InputManager : MonoBehaviour
{
    PlayerSkill playerSkill;

    // Start is called before the first frame update
    void Start()
    {
        playerSkill = FindObjectOfType<PlayerSkill>();
    }

    // Update is called once per frame
    void Update()
    {
        ButtonDown();
    }

    void ButtonDown()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.B))
        {
            print("현재 마법 사용");
            //현재 마법 사용
            playerSkill.UseSkill();
        }

        if (OVRInput.GetDown(OVRInput.RawButton.A))
        {
            //마나 차지
            playerSkill.ChargeMpStart();
        }
        if (OVRInput.GetUp(OVRInput.RawButton.A))
        {
            //마나 차지
            playerSkill.ChargeMpStop();
        }

        if (OVRInput.GetDown(OVRInput.RawButton.X))
        {
            //이전 마법
            playerSkill.CurrentSkillIndex--;
        }

        if (OVRInput.GetDown(OVRInput.RawButton.Y))
        {
            //다음 마법 쓰기
            playerSkill.CurrentSkillIndex++;
        }
    }
}
