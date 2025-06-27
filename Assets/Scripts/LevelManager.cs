using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private List<LevelData> levelDatas;
    [SerializeField] private CardSpawnManager cardSpawnManager;
    private int needCountCard;
    private int currentCountCard;
    private int indexLevel;
    void Start()
    {


    }
    public void Init()
    {
        if (indexLevel >= levelDatas.Count)
        {
            indexLevel = 0;
        }
        needCountCard = levelDatas[indexLevel].countCard;
        cardSpawnManager.SpawnCards(needCountCard);
    }
    public void StartGame()
    {
        CountdownTimer.Instance.Init();
    }
    public void CheckLevel()
    {
        currentCountCard++;
        if (currentCountCard >= needCountCard)
        {
            NextLevel();
        }
    }
    public void NextLevel()
    {
        currentCountCard = 0;
        indexLevel++;
        needCountCard = levelDatas[indexLevel].countCard;
        cardSpawnManager.SpawnCards(needCountCard);
    }
}
[System.Serializable]
public class LevelData
{
    public int countCard;


}