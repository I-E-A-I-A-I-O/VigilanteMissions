using GTA;
using System;
using System.Collections.Generic;

public abstract class Mission
{
    public abstract bool IsMostWanted
    {
        get;
    }
    public abstract Blip ObjectiveLocationBlip 
    {
        get; set;
    }
    public abstract void MissionTick(object o, EventArgs e);
    public abstract bool StartMission();
    public abstract void QuitMission();
    public abstract void RemoveDeadEnemies();
    public abstract void RemoveVehiclesAndNeutrals();
}
