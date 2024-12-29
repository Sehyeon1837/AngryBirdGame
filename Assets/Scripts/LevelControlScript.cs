using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class LevelControlScript : MonoBehaviour
{
    private int remainingPigs = 0;
    public GameObject[] levelPrefabs;
    private int nowLevel = 0;
    private GameObject currentObi;
    public GameObject NextLevelWin;
    public GameObject LastLevelWin;

    void Start()
    {
        CreateNewLevel();
        EnemyDamage.OnPigDestroyed += HandlePigDestroyed; // 이벤트
    }

    void SetValue()
    {
        remainingPigs = GameObject.FindGameObjectsWithTag("Enemy").Length;
    }

    private void HandlePigDestroyed()
    {
        remainingPigs--;

        if (remainingPigs < 0)
            return;

        if (remainingPigs == 0)
        {
            if (nowLevel == levelPrefabs.Length - 1)
            {
                Debug.Log("last stage");
                LastLevelWin.SetActive(true);
                return;
            }
            
            NextLevelWin.SetActive(true); // 다음 스테이지 창
        }
    }
    
    public void GoToNextLevel()
    {
        Destroy(currentObi);

        nowLevel++; 
        CreateNewLevel();
        
        NextLevelWin.SetActive(false);
    }

    public void RestartLevel()
    {
        Destroy(currentObi);

        CreateNewLevel();
        
        NextLevelWin.SetActive(false);
    }

    void CreateNewLevel()
    {
        currentObi = Instantiate(levelPrefabs[nowLevel], Vector3.zero, Quaternion.identity); // 해당 레벨 스테이지 prefab 생성
        SetValue();
    }
}
