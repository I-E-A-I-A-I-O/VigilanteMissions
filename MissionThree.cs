using GTA;
using GTA.Native;
using GTA.Math;
using System;
using System.Collections.Generic;

class MissionThree : Mission
{
    enum Objectives
    {
        GoToLocation,
        KillTargets,
        Completed,
        None
    }

    enum Enemies
    {
        Madrazo,
        HouseEntranceGuard,
        SideFrontDoor,
        SideFrontDoorLeft01,
        SideFrontDoorLeft02,
        GolfGuard01,
        GolfGuard02,
        GolfGuard03,
        ChefGuard,
        PoolGuard01,
        PoolGuard02,
        GardenGuard,
        SideBackGuard01,
        SideBackGuard02,
        SideBackGuard03,
        GarageGuard01,
        GarageGuard02,
        SideBackGuard,
        FrontGardenGuard
    }

    enum Neutrals
    {
        Patricia,
        Chef
    }

    Vector3 objectiveLocation;
    List<MissionPed> enemies = new List<MissionPed>();
    List<MissionPed> neutralPeds = new List<MissionPed>();
    Music music;
    Script script;
    RelationshipGroup enemiesRelGroup;
    RelationshipGroup neutralsRelGroup;
    MostWantedMissions mostWantedMissions;
    Blip objectiveLocationBlip;
    Objectives currentObjective;
    MissionWorld missionWorld;

    public MissionThree(Script script, RelationshipGroup enemiesRelGroup, RelationshipGroup neutralsRelGroup, MissionWorld missionWorld)
    {
        this.script = script;
        this.enemiesRelGroup = enemiesRelGroup;
        this.neutralsRelGroup = neutralsRelGroup;
        this.missionWorld = missionWorld;

        music = new Music();
        mostWantedMissions = new MostWantedMissions();
        objectiveLocation = mostWantedMissions.MISSION_THREE_LOCATION;
    }

    public override void MissionTick(object o, EventArgs e)
    {
        if (Game.Player.WantedLevel >= 2)
        {
            Game.Player.WantedLevel = 1;
        }
        switch (currentObjective)
        {
            case Objectives.GoToLocation:
                {
                    if (!Game.Player.Character.IsInRange(objectiveLocation, 200f))
                    {
                        return;
                    }
                    music.IncreaseIntensity();
                    objectiveLocationBlip.Delete();
                    var peds = mostWantedMissions.InitializeMissionThreePeds();
                    var neutrals = mostWantedMissions.InitializeMissionThreeCivilianPeds();
                    Script.Wait(1000);
                    for (var i = 0; i < peds.Count; i++)
                    {
                        enemies.Add(new MissionPed(peds[i], enemiesRelGroup, objectiveLocation, script));
                    }
                    for (var i = 0; i < neutrals.Count; i++)
                    {
                        neutralPeds.Add(new MissionPed(neutrals[i], neutralsRelGroup, objectiveLocation, script, true));
                    }
                    foreach (MissionPed enemy in enemies)
                    {
                        enemy.ShowBlip();
                    }
                    GTA.UI.Screen.ShowSubtitle("Kill the ~r~targets~w~.", 8000);
                    currentObjective = Objectives.KillTargets;
                    StartScenarios();
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
                    RemoveVehiclesAndNeutrals();
                    GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Wanted Suspect", "Good job, your cut of the reward is already in your account.");
                    Game.Player.Money += 15000;
                    Game.Player.WantedLevel = 3;
                    currentObjective = Objectives.None;
                    missionWorld.CompleteMission();
                    script.Tick -= MissionTick;
                    break;
                }
        }
    }

    public override void QuitMission()
    {
        music.StopMusic();
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
        RemoveVehiclesAndNeutrals();
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
        foreach (MissionPed neutral in neutralPeds)
        {
            neutral.Delete();
        }
    }

    public override bool StartMission()
    {
        if (Game.Player.Character.IsInRange(objectiveLocation, 200f))
        {
            GTA.UI.Notification.Show("Mission not available.");
            return false;
        }
        music.StartHeistMusic();
        currentObjective = Objectives.GoToLocation;
        objectiveLocationBlip = World.CreateBlip(objectiveLocation, 150f);
        objectiveLocationBlip.Color = BlipColor.Yellow;
        objectiveLocationBlip.ShowRoute = true;
        objectiveLocationBlip.Name = "Wanted suspect location";

        GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Wanated suspect", "Ok, i tracked them down, i'm sending you the location.");
        GTA.UI.Screen.ShowSubtitle("Go to the ~y~wanted suspect~w~.");

        script.Tick += MissionTick;
        return true;
    }

    void StartScenarios()
    {
        enemies[(int)Enemies.Madrazo].ped.Task.ChatTo(neutralPeds[(int)Neutrals.Patricia].ped);
        neutralPeds[(int)Neutrals.Patricia].ped.Task.StartScenario("WORLD_HUMAN_MAID_CLEAN", 0);

        enemies[(int)Enemies.FrontGardenGuard].ped.Task.GuardCurrentPosition();
        enemies[(int)Enemies.HouseEntranceGuard].ped.Task.StartScenario("WORLD_HUMAN_GUARD_STAND", 0);
        enemies[(int)Enemies.SideFrontDoor].ped.Task.StartScenario("WORLD_HUMAN_GUARD_STAND", 0);
        enemies[(int)Enemies.SideFrontDoorLeft01].ped.Task.UseMobilePhone();
        enemies[(int)Enemies.SideFrontDoorLeft02].ped.Task.StartScenario("WORLD_HUMAN_LEANING", 0);

        enemies[(int)Enemies.ChefGuard].ped.Task.ChatTo(neutralPeds[(int)Neutrals.Chef].ped);
        neutralPeds[(int)Neutrals.Chef].ped.Task.ChatTo(enemies[(int)Enemies.ChefGuard].ped);

        enemies[(int)Enemies.GardenGuard].ped.Task.GuardCurrentPosition();
        enemies[(int)Enemies.GolfGuard01].ped.Task.UseMobilePhone();
        enemies[(int)Enemies.GolfGuard02].ped.Task.StartScenario("WORLD_HUMAN_GOLF_PlAYER", 0);
        enemies[(int)Enemies.GolfGuard03].ped.Task.StartScenario("WORLD_HUMAN_AA_SMOKE", 0);
        enemies[(int)Enemies.GarageGuard01].ped.Task.StartScenario("WORLD_HUMAN_VEHICLE_MECHANIC", 0);
        enemies[(int)Enemies.GarageGuard02].ped.Task.ChatTo(enemies[(int)Enemies.GarageGuard01].ped);
        enemies[(int)Enemies.PoolGuard01].ped.Task.ChatTo(enemies[(int)Enemies.PoolGuard01].ped);
        enemies[(int)Enemies.PoolGuard02].ped.Task.UseMobilePhone();
        enemies[(int)Enemies.SideBackGuard01].ped.Task.StartScenario("WOLRD_HUMAN_DRUG_DEALER", 0);
        enemies[(int)Enemies.SideBackGuard02].ped.Task.StartScenario("WORLD_HUMAN_LEANING", 0);
        enemies[(int)Enemies.SideBackGuard03].ped.Task.UseMobilePhone();
        enemies[(int)Enemies.SideBackGuard].ped.Task.StartScenario("WORLD_HUMAN_GUARD_STAND", 0);
    }
}