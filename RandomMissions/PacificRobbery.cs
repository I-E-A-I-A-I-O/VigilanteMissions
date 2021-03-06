using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;

class PacificRobbery : Mission
{
    public override bool IsMostWanted => false;
    public override bool IsJokerMission => false;

    enum Objectives
    {
        GoToLocation,
        KillTargets,
        None
    }

    Vector3 objectiveLocation;
    public override Blip ObjectiveLocationBlip { get; set; }
    Objectives currentObjective;
    RelationshipGroup robberRelGroup;
    RelationshipGroup hostagesRelGroup;
    List<MissionPed> enemies = new List<MissionPed>();
    List<MissionPed> hostages = new List<MissionPed>();
    List<MissionPed> police = new List<MissionPed>();
    List<Vehicle> vehicles = new List<Vehicle>();

    public PacificRobbery()
    {
        robberRelGroup = MissionWorld.RELATIONSHIP_MISSION_AGGRESSIVE;
        hostagesRelGroup = MissionWorld.RELATIONSHIP_MISSION_PEDESTRIAN;

        objectiveLocation = BankMissions.PACIFIC_LOCATION;
    }

    protected override void MissionTick(object o, EventArgs e)
    {
        switch (currentObjective)
        {
            case Objectives.GoToLocation:
                {
                    if (!Game.Player.Character.IsInRange(objectiveLocation, 300))
                    {
                        return;
                    }
                    Loading.LoadModel(BankMissions.RobberModel);
                    var enemyPeds = BankMissions.InitializePacificRobbers();
                    Loading.UnloadModel(BankMissions.RobberModel);
                    Loading.LoadModels(BankMissions.PoliceModels);
                    var policePeds = BankMissions.InitializePacificPolice();
                    Loading.UnloadModels(BankMissions.PoliceModels);
                    Loading.LoadModels(BankMissions.HostageModels);
                    var hostagePeds = BankMissions.InitializePacificHostages();
                    Loading.UnloadModels(BankMissions.HostageModels);
                    Loading.LoadModels(BankMissions.VehicleModels);
                    vehicles = BankMissions.InitializePacificVehicles();
                    Loading.UnloadModels(BankMissions.VehicleModels);
                    for (var i = 0; i < enemyPeds.Count; i++)
                    {
                        enemies.Add(new MissionPed(enemyPeds[i], robberRelGroup));
                        enemies[i].ShowBlip();
                        enemies[i].GetTask().StartScenario("WORLD_HUMAN_STAND_IMPATIENT", 0);
                        Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, enemies[i].GetPed().Handle, 1);
                    }
                    for (var i = 0; i < policePeds.Count; i++)
                    {
                        police.Add(new MissionPed(policePeds[i], "COP"));
                        police[i].GetTask().StartScenario("WORLD_HUMAN_STAND_IMPATIENT", 0);
                        Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, police[i].GetPed().Handle, 1);
                    }
                    for (var i = 0; i < hostagePeds.Count; i++)
                    {
                        hostages.Add(new MissionPed(hostagePeds[i], hostagesRelGroup, true));
                        hostages[i].GetTask().HandsUp(1800000);
                        hostages[i].GetPed().BlockPermanentEvents = true;
                    }
                    foreach (Vehicle vehicle in vehicles)
                    {
                        vehicle.IsSirenActive = true;
                        vehicle.IsSirenSilent = false;
                    }
                    ObjectiveLocationBlip.Delete();
                    GTA.UI.Screen.ShowSubtitle("Kill the ~r~bank robbers~w~.", 8000);
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
                        GTA.UI.Screen.ShowSubtitle("Crime scene cleared.", 8000);
                        RemoveVehiclesAndNeutrals();
                        Game.Player.Money += 3500;
                        currentObjective = Objectives.None;
                        MissionWorld.CompleteMission();
                        MissionWorld.script.Tick -= MissionTick;
                    }
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
        RemoveVehiclesAndNeutrals();
    }

    protected override void RemoveDeadEnemies()
    {
        var aliveEnemies = enemies;
        for (var i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].IsDead())
            {
                if (enemies[i].GetPed().Killer == Game.Player.Character)
                {
                    Progress.enemiesKilledCount += 1;
                }
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
        foreach (MissionPed ped in police)
        {
            ped.Delete();
        }
        foreach (MissionPed ped in hostages)
        {
            ped.Delete();
        }
    }

    public override bool StartMission()
    {
        if (Game.Player.Character.IsInRange(objectiveLocation, 500))
        {
            GTA.UI.Notification.Show("Mission not available.");
            return false;
        }
        ObjectiveLocationBlip = World.CreateBlip(objectiveLocation);
        ObjectiveLocationBlip.DisplayType = BlipDisplayType.BothMapSelectable;
        ObjectiveLocationBlip.Color = BlipColor.Yellow;
        ObjectiveLocationBlip.Name = "Pacific Standard robbery";
        ObjectiveLocationBlip.ShowRoute = true;

        GTA.UI.Screen.ShowSubtitle("Go to the ~y~Pacific Standard bank~w~ and stop the robbery.", 8000);
        currentObjective = Objectives.GoToLocation;
        MissionWorld.script.Tick += MissionTick;
        return true;
    }
}