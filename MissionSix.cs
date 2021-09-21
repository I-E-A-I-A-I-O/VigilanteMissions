using GTA;
using GTA.Native;
using GTA.Math;
using System;
using System.Collections.Generic;

class MissionSix : Mission
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
        GuardGroup,
        GuardAiming,
        Watcher,
        GuardLookingTarget,
        Target,
        ChattingGuard01,
        ChattingGuard02,
        PatrollingGuard01,
        WallGuard,
        PatrollingGuard02,
    }

    enum Neutrals
    {
        OtherWoman,
        ArrestedWoman,
        Man01,
        Man02,
        Man03
    }

    Vector3 objectiveLocation;
    Script script;
    List<Vehicle> vehicles = new List<Vehicle>();
    List<MissionPed> enemies = new List<MissionPed>();
    List<MissionPed> neutralPeds = new List<MissionPed>();
    Objectives currentObjective;
    Music music;
    RelationshipGroup enemiesRelGroup;
    RelationshipGroup neutralsRelGroup;
    MissionWorld missionWorld;
    MostWantedMissions mostWantedMissions;
    Blip objectiveLocationBlip;

    public MissionSix(Script script, MissionWorld missionWorld, RelationshipGroup enemiesRelGroup, RelationshipGroup neutralsRelGroup)
    {
        this.script = script;
        this.missionWorld = missionWorld;
        this.enemiesRelGroup = enemiesRelGroup;
        this.neutralsRelGroup = neutralsRelGroup;

        music = new Music();
        mostWantedMissions = new MostWantedMissions();
        objectiveLocation = mostWantedMissions.MISSION_SIX_LOCATION;
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
                    vehicles = mostWantedMissions.InitializeMissionSixVehicles();
                    var peds = mostWantedMissions.InitializeMissionSixPeds();
                    var neutrals = mostWantedMissions.InitializeMissionSixCivilianPeds();
                    Script.Wait(1000);
                    for (var i = 0; i < peds.Count; i++)
                    {
                        enemies.Add(new MissionPed(peds[i], enemiesRelGroup, objectiveLocation, script));
                    }
                    for (var i = 0; i < neutrals.Count; i++)
                    {
                        neutralPeds.Add(new MissionPed(neutrals[i], neutralsRelGroup, objectiveLocation, script, true));
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
        foreach (Vehicle vehicle in vehicles)
        {
            vehicle.MarkAsNoLongerNeeded();
        }
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
        enemies.Add(new MissionPed(vehicles[0].CreatePedOnSeat(VehicleSeat.Driver, new Model(PedHash.ChiGoon01GMM)), enemies[0].ped.RelationshipGroup, vehicles[0].Position, script));
        enemies.Add(new MissionPed(vehicles[0].CreatePedOnSeat(VehicleSeat.Passenger, new Model(PedHash.ChiGoon01GMM)), enemies[0].ped.RelationshipGroup, vehicles[0].Position, script));

        enemies.Add(new MissionPed(vehicles[1].CreatePedOnSeat(VehicleSeat.Driver, new Model(PedHash.ChiGoon01GMM)), enemies[0].ped.RelationshipGroup, vehicles[1].Position, script));
        enemies.Add(new MissionPed(vehicles[1].CreatePedOnSeat(VehicleSeat.Passenger, new Model(PedHash.ChiGoon01GMM)), enemies[0].ped.RelationshipGroup, vehicles[1].Position, script));

        foreach (MissionPed ped in enemies)
        {
            ped.ShowBlip();
        }

        enemies[(int)Enemies.GuardGroup].ped.Task.AimAt(neutralPeds[(int)Neutrals.OtherWoman].ped, 1800000);
        enemies[(int)Enemies.GuardAiming].ped.Task.Arrest(neutralPeds[(int)Neutrals.ArrestedWoman].ped);
        enemies[(int)Enemies.Target].ped.Task.ChatTo(neutralPeds[(int)Neutrals.ArrestedWoman].ped);
        enemies[(int)Enemies.GuardLookingTarget].ped.Task.AimAt(neutralPeds[(int)Neutrals.OtherWoman].ped, 1800000);
        neutralPeds[(int)Neutrals.OtherWoman].ped.Task.HandsUp(1800000);
        neutralPeds[(int)Neutrals.Man01].ped.Task.HandsUp(1800000);
        neutralPeds[(int)Neutrals.Man02].ped.Task.HandsUp(1800000);
        neutralPeds[(int)Neutrals.Man03].ped.Task.HandsUp(1800000);

        enemies[(int)Enemies.WallGuard].ped.Task.StartScenario("WORLD_HUMAN_SMOKING", 0);
        enemies[(int)Enemies.Watcher].ped.Task.StartScenario("WORLD_HUMAN_BINOCULARS", 0);
        enemies[(int)Enemies.ChattingGuard01].ped.Task.ChatTo(enemies[(int)Enemies.ChattingGuard02].ped);
        enemies[(int)Enemies.ChattingGuard02].ped.Task.ChatTo(enemies[(int)Enemies.ChattingGuard01].ped);
        enemies[(int)Enemies.PatrollingGuard01].ped.Task.GuardCurrentPosition();
        enemies[(int)Enemies.PatrollingGuard02].ped.Task.GuardCurrentPosition();
    }
}
