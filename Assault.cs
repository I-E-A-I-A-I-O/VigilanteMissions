using GTA;
using GTA.Native;
using GTA.Math;
using System;
using System.Collections.Generic;

class Assault : Mission
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
    RelationshipGroup neutralsRelGroup;
    List<MissionPed> enemies;
    List<MissionPed> neutralPeds;
    Objectives currentObjective;
    Script script;
    RandomMissions randomMissions;
    Blip objectiveLocationBlip;

    public Assault(Script script, MissionWorld missionWorld, RelationshipGroup enemiesRelGroup, RelationshipGroup neutralsRelGroup)
    {
        this.script = script;
        this.missionWorld = missionWorld;
        this.enemiesRelGroup = enemiesRelGroup;
        this.neutralsRelGroup = neutralsRelGroup;

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
                    enemies[0].ped.Task.AimAt(neutralPeds[0].ped, 1800000);
                    neutralPeds[0].ped.Task.HandsUp(1800000);
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
                    RemoveVehiclesAndNeutrals();
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
        foreach(MissionPed neutral in neutralPeds)
        {
            neutral.Delete();
        }
    }

    public override bool StartMission()
    {
        try
        {
            do
            {
                objectiveLocation = randomMissions.GetRandomLocation(RandomMissions.LocationType.Foot);
            } while (Game.Player.Character.IsInRange(objectiveLocation, 200f));

            var enemy = randomMissions.CreateCriminal(objectiveLocation);
            var neutral = randomMissions.CreateVictim(objectiveLocation);
            Script.Wait(1000);
            enemies.Add(new MissionPed(enemy, enemiesRelGroup, objectiveLocation, script));
            neutralPeds.Add(new MissionPed(neutral, neutralsRelGroup, objectiveLocation, script, true));

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
