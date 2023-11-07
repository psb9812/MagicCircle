using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

namespace Project_SL
{
    [Serializable]
    class SaveData
    {
        // 플레이어의 위치와 회전값
        public Vector3 playerPos;
        public Vector3 playerRot;

        //저장할 내역 추후 추가
    }

    public class SaveManager
    {
        private string SAVE_DATA_DIRECTORY;
        private string SAVE_FILENAME = "/SaveFile.txt";
        private SaveData _saveData = new SaveData();
        private GameObject _player;

        //세이브 데이터가 저장될 Json파일을 찾고 없다면 새로 만든다.
        public void Init()
        {
            _player = GameObject.FindGameObjectWithTag("Player");

            SAVE_DATA_DIRECTORY = Application.dataPath + "/SaveData/";
            
            //만약 SAVE_DATA_DIRECTORY가 없다면 만들어 주는 조건
            if (!Directory.Exists(SAVE_DATA_DIRECTORY))
            {
                Directory.CreateDirectory(SAVE_DATA_DIRECTORY);
            }

        }

        public void Save()
        {
            //현재 게임 상태를 Json 파일에 저장한다.
            _saveData.playerPos = _player.transform.position;
            _saveData.playerRot = _player.transform.eulerAngles;

            string json = JsonUtility.ToJson(_saveData);

            File.WriteAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME, json);
        }

        public void Load()
        {
            //Json 파일의 데이터를 현재 게임에 적용시킨다.
            if (File.Exists(SAVE_DATA_DIRECTORY + SAVE_FILENAME))
            {
                string loadJson = File.ReadAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME);

                _saveData = JsonUtility.FromJson<SaveData>(loadJson);

                _player.transform.position = _saveData.playerPos;
                _player.transform.eulerAngles = _saveData.playerRot;

                Debug.Log("로드 완료");
            }
            else
                Debug.Log("저장 파일이 없습니다.");
        }

        public void ResetData()
        {
            //Json 파일을 초기 상태로 되돌린다.
        }

        
    }
}
