using GTA;
using GTA.Native;
using GTA.Math;
using System;
using System.Collections.Generic;

class MissionSix : Mission
{
    public override bool IsMostWanted => true;
    public override bool IsJokerMission => false;

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
    List<Vehicle> vehicles = new List<Vehicle>();
    List<MissionPed> enemies = new List<MissionPed>();
    List<MissionPed> neutralPeds = new List<MissionPed>();
    Objectives currentObjective;
    RelationshipGroup enemiesRelGroup;
    RelationshipGroup neutralsRelGroup;
    public override Blip ObjectiveLocationBlip { get; set; }

    public MissionSix()
    {
        enemiesRelGroup = MissionWorld.RELATIONSHIP_MISSION_AGGRESSIVE;
        neutralsRelGroup = MissionWorld.RELATIONSHIP_MISSION_PEDESTRIAN;

        objectiveLocation = MostWantedMissions.MISSION_SIX_LOCATION;
    }

    protected override void MissionTick(object o, EventArgs e)
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
                    Music.IncreaseIntensity();
                    ObjectiveLocationBlip.Delete();
                    vehicles = MostWantedMissions.InitializeMissionSixVehicles();
                    var peds = MostWantedMissions.InitializeMissionSixPeds();
                    var neutrals = MostWantedMissions.InitializeMissionSixCivilianPeds();
                    vehicles = MissionWorld.VehicleListLoadLoop(vehicles, MostWantedMissions.InitializeMissionSixVehicles);
                    peds = MissionWorld.PedListLoadLoop(peds, MostWantedMissions.InitializeMissionSixPeds);
                    neutrals = MissionWorld.PedListLoadLoop(neutrals, MostWantedMissions.InitializeMissionSixCivilianPeds);
                    for (var i = 0; i < peds.Count; i++)
                    {
                        enemies.Add(new MissionPed(peds[i], enemiesRelGroup));
                    }
                    for (var i = 0; i < neutrals.Count; i++)
                    {
                        neutralPeds.Add(new MissionPed(neutrals[i], neutralsRelGroup, true));
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
                    MissionWorld.CompleteMission();
                    MissionWorld.script.Tick -= MissionTick;
                    break;
                }
        }
    }

    public override void QuitMission()
    {
        Music.StopMusic();
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
        RemoveVehiclesAndNeutrals();
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
            }
        }
        enemies = aliveEnemies;
    }

    protected override void RemoveVehiclesAndNeutrals()
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
        Music.StartHeistMusic();
        currentObjective = Objectives.GoToLocation;
        ObjectiveLocationBlip = World.CreateBlip(objectiveLocation, 150f);
        ObjectiveLocationBlip.Color = BlipColor.Yellow;
        ObjectiveLocationBlip.ShowRoute = true;
        ObjectiveLocationBlip.Name = "Wanted suspect location";

        GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Wanated suspect", "Ok, i tracked them down, i'm sending you the location.");
        GTA.UI.Screen.ShowSubtitle("Go to the ~y~wanted suspect~w~.");

        MissionWorld.script.Tick += MissionTick;
        return true;
    }

    void StartScenarios()
    {
        enemies.Add(new MissionPed(vehicles[0].CreatePedOnSeat(VehicleSeat.Driver, new Model(PedHash.ChiGoon01GMM)), enemies[0].GetRelGroup()));
        enemies.Add(new MissionPed(vehicles[0].CreatePedOnSeat(VehicleSeat.Passenger, new Model(PedHash.ChiGoon01GMM)), enemies[0].GetRelGroup()));

        enemies.Add(new MissionPed(vehicles[1].CreatePedOnSeat(VehicleSeat.Driver, new Model(PedHash.ChiGoon01GMM)), enemies[0].GetRelGroup()));
        enemies.Add(new MissionPed(vehicles[1].CreatePedOnSeat(VehicleSeat.Passenger, new Model(PedHash.ChiGoon01GMM)), enemies[0].GetRelGroup()));

        foreach (MissionPed ped in enemies)
        {
            ped.ShowBlip();
        }

        enemies[(int)Enemies.GuardGroup].GetTask().AimAt(neutralPeds[(int)Neutrals.OtherWoman].GetPed(), 1800000);
        enemies[(int)Enemies.GuardAiming].GetTask().Arrest(neutralPeds[(int)Neutrals.ArrestedWoman].GetPed());
        enemies[(int)Enemies.Target].GetTask().ChatTo(neutralPeds[(int)Neutrals.ArrestedWoman].GetPed());
        enemies[(int)Enemies.GuardLookingTarget].GetTask().AimAt(neutralPeds[(int)Neutrals.OtherWoman].GetPed(), 1800000);
        neutralPeds[(int)Neutrals.OtherWoman].GetTask().HandsUp(1800000);
        neutralPeds[(int)Neutrals.Man01].GetTask().HandsUp(1800000);
        neutralPeds[(int)Neutrals.Man02].GetTask().HandsUp(1800000);
        neutralPeds[(int)Neutrals.Man03].GetTask().HandsUp(1800000);

        enemies[(int)Enemies.WallGuard].GetTask().StartScenario("WORLD_HUMAN_SMOKING", 0);
        enemies[(int)Enemies.Watcher].GetTask().StartScenario("WORLD_HUMAN_BINOCULARS", 0);
        enemies[(int)Enemies.ChattingGuard01].GetTask().ChatTo(enemies[(int)Enemies.ChattingGuard02].GetPed());
        enemies[(int)Enemies.ChattingGuard02].GetTask().ChatTo(enemies[(int)Enemies.ChattingGuard01].GetPed());
        enemies[(int)Enemies.PatrollingGuard01].GetTask().GuardCurrentPosition();
        enemies[(int)Enemies.PatrollingGuard02].GetTask().GuardCurrentPosition();
    }
}
