// MissionManager.cs
using UnityEngine;
using System.Collections.Generic;

public class Mission
{
    public string id;
    public string description;
    public bool active;
    public bool completed;
}

public class MissionManager : MonoBehaviour
{
    public List<Mission> missions = new List<Mission>();

    public void StartMission(string id)
    {
        var m = missions.Find(x => x.id == id);
        if (m != null) m.active = true;
    }
    public void CompleteMission(string id)
    {
        var m = missions.Find(x => x.id == id);
        if (m != null)
        {
            m.completed = true;
            m.active = false;
            // give rewards, trigger next mission etc.
        }
    }
}
