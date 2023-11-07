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
            print("���� ���� ���");
            //���� ���� ���
            playerSkill.UseSkill();
        }

        if (OVRInput.GetDown(OVRInput.RawButton.A))
        {
            //���� ����
            playerSkill.ChargeMpStart();
        }
        if (OVRInput.GetUp(OVRInput.RawButton.A))
        {
            //���� ����
            playerSkill.ChargeMpStop();
        }

        if (OVRInput.GetDown(OVRInput.RawButton.X))
        {
            //���� ����
            playerSkill.CurrentSkillIndex--;
        }

        if (OVRInput.GetDown(OVRInput.RawButton.Y))
        {
            //���� ���� ����
            playerSkill.CurrentSkillIndex++;
        }
    }
}
