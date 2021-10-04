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
    public abstract bool IsJokerMission
    {
        get;
    }
    protected abstract void MissionTick(object o, EventArgs e);
    public abstract bool StartMission();
    public abstract void QuitMission();
    protected abstract void RemoveDeadEnemies();
    protected abstract void RemoveVehiclesAndNeutrals();
}
