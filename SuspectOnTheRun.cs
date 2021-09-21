using GTA;
using GTA.Native;
using GTA.Math;
using System;
using System.Collections.Generic;

class SuspectOnTheRun : Mission
{
    enum Objectives
    {
        GoToLocation,
        KillTargets,
        Completed,
        None
    }

    MissionWorld missionWorld;
    Vector3 objectiveLocation;
    RelationshipGroup enemiesRelGroup;
    List<MissionPed> enemies = new List<MissionPed>();
    Objectives currentObjective;
    Script script;
    RandomMissions randomMissions;
    Blip objectiveLocationBlip;

    public SuspectOnTheRun(Script script, MissionWorld missionWorld, RelationshipGroup relationshipGroup)
    {
        this.script = script;
        this.missionWorld = missionWorld;
        enemiesRelGroup = relationshipGroup;

        randomMissions = new RandomMissions();
    }

    public override void MissionTick(object o, EventArgs e)
    {
        switch (currentObjective)
        {
            case Objectives.GoToLocation:
                {
                    if (!Game.Player.Character.IsInRange(objectiveLocation, 200f))
                    {
                        return;
                    }
                    objectiveLocationBlip.Delete();
                    GTA.UI.Screen.ShowSubtitle("Kill the ~r~targets~w~.", 8000);
                    enemies[0].ShowBlip();
                    enemies[0].ped.Task.FleeFrom(Game.Player.Character);
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
                    missionWorld.CompleteMission();
                    script.Tick -= MissionTick;
                    break;
                }
        }
    }

    public override void QuitMission()
    {
        currentObjective = Objectives.None;
        script.Tick -= MissionTick;
        foreach (MissionPed enemy in enemies)
        {
            enemy.Delete();
        }
        if (objectiveLocationBlip.Exists())
        {
            objectiveLocationBlip.Delete();
        }
    }

    public override void RemoveDeadEnemies()
    {
        var aliveEnemies = enemies;
        for (var i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].IsDead())
            {
                enemies[i].Delete();
                aliveEnemies.RemoveAt(i);
            }
        }
        enemies = aliveEnemies;
    }

    public override void RemoveVehiclesAndNeutrals()
    {
        //Not needed
    }

    public override bool StartMission()
    {
        try
        {
            do
            {
                objectiveLocation = randomMissions.GetRandomLocation(RandomMissions.LocationType.Foot);
            } while (Game.Player.Character.IsInRange(objectiveLocation, 200f));

            var ped = randomMissions.CreateCriminal(objectiveLocation);
            Script.Wait(1000);
            enemies.Add(new MissionPed(ped, enemiesRelGroup, objectiveLocation, script));

            currentObjective = Objectives.GoToLocation;
            objectiveLocationBlip = World.CreateBlip(objectiveLocation, 150f);
            objectiveLocationBlip.Color = BlipColor.Yellow;
            objectiveLocationBlip.ShowRoute = true;
            objectiveLocationBlip.Name = "Crime scene";
            GTA.UI.Screen.ShowSubtitle("Go to the ~y~crime scene~w~.", 8000);

            script.Tick += MissionTick;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}