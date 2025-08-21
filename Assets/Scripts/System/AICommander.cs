using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(-999)]
public class AICommander : MonoBehaviour
{
    public event Action<AIAgent> RequestReport;

    public static AICommander Instance {get; private set;}

    List<SoldierReport> reports;
    public readonly struct SoldierReport
    {
        public readonly AIAgent soldier;
        public readonly Vector3 location;
        public SoldierReport(AIAgent soldier, Vector3 location)
        {
            this.soldier = soldier;
            this.location = location;
        }
    }

    [SerializeField] int maxReinforcement = 3;
    Vector3 enemyLocation;
    float commandCoolDown = 5f;
    float commandCountDown;

    void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(this);
        Debug.Log("AICommander Initialized");
        reports = new();
    }
    
    public void ReceiveReport(AIAgent soldier, Vector3 location)
        => reports.Add(new SoldierReport(soldier, location));

    public void CallReinforcement(AIAgent sender, Vector3 location)
    {
        if(commandCountDown > 0) return;
        commandCountDown = commandCoolDown;

        RequestReport?.Invoke(sender);
        enemyLocation = location;
        StartCoroutine(IssuingOrder());
    }

    IEnumerator IssuingOrder()
    {
        yield return new WaitForSeconds(0.5f); //wait for soldiers to report back

        AIAgent[] troop = SelectSoldiers();
        SendingTroop(troop);

        reports.Clear();
    }

    AIAgent[] SelectSoldiers()
    {
        var selected = reports
            .Where(r => r.soldier != null)
            .OrderBy(r => Vector3.Distance(r.location, enemyLocation))
            .Select(r => r.soldier)
            .Take(maxReinforcement)
            .ToArray();

        return selected;
    }

    void SendingTroop(AIAgent[] troop)
    {
        foreach(var soldier in troop) 
            soldier.Reinforce(enemyLocation  + (Vector3) UnityEngine.Random.insideUnitCircle * 2);
    }

    void Update()
    {
        if(commandCountDown > 0) commandCountDown -= Time.deltaTime;
    }
}