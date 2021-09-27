using GTA;
using GTA.Native;
using GTA.Math;
using System;
using System.Collections.Generic;

class MissionTen : Mission
{
    enum Objectives
    {
        GoToLocation,
        KillTargets,
        Complete,
        None
    }

    enum Enemies
    {
        OuterEntrance01,
        OuterEntrance02,
        InnerEntrance01,
        BathroomGuard,
        DanceFloorDoor,
        InnerEntrance02,
        InnerEntrance03,
        OuterBar01,
        BarDoor,
        DanceFloorSeats,
        CashierGuard,
        InnerBar01,
        InnerBar02,
        Target,
        TargetBody01,
        TargetBody02,
        TargetBody03,
        DanceFloorDouble,
        DJ,
        OuterBar02
    }

    Objectives currentObjective;
    Music music;
    Vector3 objectiveLocation;
    Blip objectiveLocationBlip;
    List<MissionPed> enemies = new List<MissionPed>();
    Ped hooker;
    MissionWorld missionWorld;
    Script script;
    MostWantedMissions mostWantedMissions;
    RelationshipGroup enemiesRelGroup;
    RelationshipGroup hookerRelGroup;

    public MissionTen(Script script, MissionWorld missionWorld, RelationshipGroup enemiesRelGroup, RelationshipGroup hookerRelGroup)
    {
        this.script = script;
        this.missionWorld = missionWorld;
        this.enemiesRelGroup = enemiesRelGroup;
        this.hookerRelGroup = hookerRelGroup;

        music = new Music();
        mostWantedMissions = new MostWantedMissions();
        objectiveLocation = mostWantedMissions.MISSION_TEN_LOCATION;
    }

    public override void MissionTick(object o, EventArgs e)
    {
        if (Game.Player.WantedLevel >= 2)
        {
            Game.Player.WantedLevel = 1;
        }
        switch(currentObjective)
        {
            case Objectives.GoToLocation:
                {
                    if (!Game.Player.Character.IsInRange(objectiveLocation, 200))
                    {
                        return;
                    }
                    music.IncreaseIntensity();
                    objectiveLocationBlip.Delete();
                    var peds = mostWantedMissions.InitializeMissionTenEnemies();
                    hooker = mostWantedMissions.InitializeMissionTenNeutralPed();
                    Script.Wait(1000);
                    for (var i = 0; i < peds.Count; i++)
                    {
                        enemies.Add(new MissionPed(peds[i], enemiesRelGroup, objectiveLocation, script));
                        enemies[i].ShowBlip();
                    }
                    hooker.RelationshipGroup = hookerRelGroup;
                    StartScenearios();

                    GTA.UI.Screen.ShowSubtitle("Kill ~r~Billy Russo and his gang~w~.", 8000);
                    currentObjective = Objectives.KillTargets;
                    break;
                }
            case Objectives.KillTargets:
                {
                    if (enemies.Count > 0)
                    {
                        RemoveDeadEnemies();
                    } else
                    {
                        currentObjective = Objectives.Complete;
                    }
                    break;
                }
            case Objectives.Complete:
                {
                    missionWorld.CompleteMission();
                    RemoveVehiclesAndNeutrals();
                    currentObjective = Objectives.None;
                    Game.Player.Money += 18000;
                    Game.Player.WantedLevel = 3;
                    GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Wanted Suspect", "Good job, your cut of the reward is in your account.");
                    script.Tick -= MissionTick;
                    break;
                }
        }
    }

    public override void QuitMission()
    {
        music.StopMusic();
        if (hooker != null)
        {
            hooker.MarkAsNoLongerNeeded();
        }
        if (objectiveLocationBlip != null)
        {
            objectiveLocationBlip.Delete();
        }
        foreach (MissionPed enemy in enemies)
        {
            enemy.Delete();
        }
        script.Tick -= MissionTick;
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
        if (hooker != null)
        {
            hooker.MarkAsNoLongerNeeded();
        }
    }

    public override bool StartMission()
    {
        if (Game.Player.Character.IsInRange(objectiveLocation, 200))
        {
            GTA.UI.Notification.Show("Mission not available.");
            return false;
        }
        music.StartFunkyTwo();
        objectiveLocationBlip = World.CreateBlip(objectiveLocation);
        objectiveLocationBlip.Color = BlipColor.Yellow;
        objectiveLocationBlip.Name = "Wanted suspect location";
        objectiveLocationBlip.ShowRoute = true;

        GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Wanted suspect", "Found him, ~r~Billy Russo and his gang~w~ are hanging out at the ~y~Bahama nighclub~w~.");

        GTA.UI.Screen.ShowSubtitle("Go to the ~y~Bahama nightclub~w~.", 8000);

        currentObjective = Objectives.GoToLocation;
        script.Tick += MissionTick;
        return true;
    }

    void StartScenearios()
    {
        hooker.Task.UseMobilePhone();
        enemies[(int)Enemies.BarDoor].ped.Task.StartScenario("WORLD_HUMAN_GUARD_STAND", 0);
        enemies[(int)Enemies.BathroomGuard].ped.Task.StartScenario("WORLD_HUMAN_GUARD_STAND", 0);
        enemies[(int)Enemies.CashierGuard].ped.Task.StartScenario("WORLD_HUMAN_GUARD_STAND", 0);
        enemies[(int)Enemies.DanceFloorDoor].ped.Task.StartScenario("WORLD_HUMAN_GUARD_STAND", 0);
        enemies[(int)Enemies.DanceFloorDouble].ped.Task.StartScenario("WORLD_HUMAN_GUARD_STAND", 0);
        enemies[(int)Enemies.DanceFloorSeats].ped.Task.StartScenario("WORLD_HUMAN_GUARD_STAND", 0);
        enemies[(int)Enemies.DJ].ped.Task.StartScenario("WORLD_HUMAN_GUARD_STAND", 0);
        enemies[(int)Enemies.OuterBar01].ped.Task.StartScenario("WORLD_HUMAN_DRINKING", 0);
        enemies[(int)Enemies.OuterBar02].ped.Task.StartScenario("WORLD_HUMAN_DRINKING", 0);
        enemies[(int)Enemies.InnerEntrance01].ped.Task.StartScenario("WORLD_HUMAN_GUARD_STAND", 0);
        enemies[(int)Enemies.InnerEntrance02].ped.Task.StartScenario("WORLD_HUMAN_GUARD_STAND", 0);
        enemies[(int)Enemies.InnerEntrance03].ped.Task.StartScenario("WORLD_HUMAN_GUARD_STAND", 0);
        enemies[(int)Enemies.InnerBar01].ped.Task.ChatTo(enemies[(int)Enemies.InnerBar02].ped);
        enemies[(int)Enemies.InnerBar02].ped.Task.ChatTo(enemies[(int)Enemies.InnerBar01].ped);
        enemies[(int)Enemies.OuterEntrance01].ped.Task.StartScenario("WORLD_HUMAN_GUARD_STAND", 0);
        enemies[(int)Enemies.OuterEntrance02].ped.Task.StartScenario("WORLD_HUMAN_GUARD_STAND", 0);
        enemies[(int)Enemies.Target].ped.Task.StartScenario("WORLD_HUMAN_AA_SMOKE", 0);
        enemies[(int)Enemies.TargetBody01].ped.Task.ChatTo(enemies[(int)Enemies.Target].ped);
        enemies[(int)Enemies.TargetBody02].ped.Task.ChatTo(enemies[(int)Enemies.Target].ped);
        enemies[(int)Enemies.TargetBody03].ped.Task.ChatTo(enemies[(int)Enemies.Target].ped);
    }
}
