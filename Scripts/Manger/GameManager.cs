using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public CharacterStats playerStats;
    List<IEndGameobserver> endGameObservers = new List<IEndGameobserver>();
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
    }
    public void RigisterPlayer(CharacterStats player)
    {
        playerStats = player;
    }
    public void Addbserver(IEndGameobserver observer)
    {
        endGameObservers.Add(observer);
    }
    public void RemoveObserver(IEndGameobserver observer)
    {
        endGameObservers.Remove(observer);
    }
    public void Notifyobservers()
    {
        foreach (var observer in endGameObservers)
        {
            observer.EndNotify();
        }
    }
}
