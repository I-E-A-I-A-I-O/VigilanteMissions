using GTA;
using GTA.Native;
using GTA.Math;
using System;
using System.Collections.Generic;

class SuspectOnTheRun : Mission
{
    public override bool IsMostWanted => false;
    public override bool IsJokerMission => false;

    enum Objectives
    {
        GoToLocation,
        KillTargets,
        Completed,
        None
    }

    Vector3 objectiveLocation;
    RelationshipGroup enemiesRelGroup;
    List<MissionPed> enemies = new List<MissionPed>();
    Objectives currentObjective;
    public override Blip ObjectiveLocationBlip { get; set; }

    public SuspectOnTheRun()
    {
        enemiesRelGroup = MissionWorld.RELATIONSHIP_MISSION_NEUTRAL;
    }

    protected override void MissionTick(object o, EventArgs e)
    {
        switch (currentObjective)
        {
            case Objectives.GoToLocation:
                {
                    if (!Game.Player.Character.IsInRange(objectiveLocation, 200f))
                    {
                        return;
                    }
                    ObjectiveLocationBlip.Delete();

                    var ped = RandomMissions.CreateCriminal(objectiveLocation);
                    ped = (Ped)MissionWorld.EntityLoadLoop(ped, RandomMissions.CreateCriminal, objectiveLocation);

                    enemies.Add(new MissionPed(ped, enemiesRelGroup));
                    enemies[0].ShowBlip();
                    enemies[0].GetTask().FleeFrom(Game.Player.Character);
                    GTA.UI.Screen.ShowSubtitle("Kill the ~r~target~w~.", 8000);
                    currentObjective = Objectives.KillTargets;
                    break;
                }
            case Objectives.KillTargets:
                {
                    if (enemies.Count > 0)
                    {
                        RemoveDeadEnemies();
                    }
                    else
                    {
                        currentObjective = Objectives.Completed;
                    }
                    break;
                }
            case Objectives.Completed:
                {
                    GTA.UI.Screen.ShowSubtitle("Crime scene cleared.", 8000);
                    Game.Player.Money += 1000;
                    currentObjective = Objectives.None;
                    MissionWorld.CompleteMission();
                    MissionWorld.script.Tick -= MissionTick;
                    break;
                }
        }
    }

    public override void QuitMission()
    {
        currentObjective = Objectives.None;
        MissionWorld.script.Tick -= MissionTick;
        foreach (MissionPed enemy in enemies)
        {
            enemy.Delete();
        }
        if (ObjectiveLocationBlip.Exists())
        {
            ObjectiveLocationBlip.Delete();
        }
    }

    protected override void RemoveDeadEnemies()
    {
        var aliveEnemies = enemies;
        for (var i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].IsDead())
            {
                enemies[i].Delete();
                aliveEnemies.RemoveAt(i);
                if (enemies[i].GetPed().Killer == Game.Player.Character)
                {
                    Progress.enemiesKilledCount += 1;
                }
            }
        }
        enemies = aliveEnemies;
    }

    protected override void RemoveVehiclesAndNeutrals()
    {
        //Not needed
    }

    public override bool StartMission()
    {
        try
        {
            do
            {
                objectiveLocation = RandomMissions.GetRandomLocation(RandomMissions.LocationType.Foot);
            } while (Game.Player.Character.IsInRange(objectiveLocation, 200f));

            currentObjective = Objectives.GoToLocation;
            ObjectiveLocationBlip = World.CreateBlip(objectiveLocation);
            ObjectiveLocationBlip.DisplayType = BlipDisplayType.BothMapSelectable;
            ObjectiveLocationBlip.Color = BlipColor.Yellow;
            ObjectiveLocationBlip.ShowRoute = true;
            ObjectiveLocationBlip.Name = "Crime scene";
            GTA.UI.Screen.ShowSubtitle("Go to the ~y~crime scene~w~.", 8000);

            MissionWorld.script.Tick += MissionTick;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}