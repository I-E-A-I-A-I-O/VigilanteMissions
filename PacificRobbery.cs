using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;

class PacificRobbery : Mission
{
    enum Objectives
    {
        GoToLocation,
        KillTargets,
        None
    }

    Vector3 objectiveLocation;
    Blip objectiveLocationBlip;
    Objectives currentObjective;
    RelationshipGroup policeRelGroup;
    RelationshipGroup robberRelGroup;
    RelationshipGroup hostagesRelGroup;
    List<MissionPed> enemies = new List<MissionPed>();
    List<MissionPed> hostages = new List<MissionPed>();
    List<MissionPed> police = new List<MissionPed>();
    List<Vehicle> vehicles = new List<Vehicle>();
    BankMissions bankMissions;
    int loadingStartTime;
    int loadingCurrentTime;
    bool loadingTimerStarted;

    public PacificRobbery()
    {
        policeRelGroup = MissionWorld.RELATIONSHIP_MISSION_COP;
        robberRelGroup = MissionWorld.RELATIONSHIP_MISSION_AGGRESSIVE;
        hostagesRelGroup = MissionWorld.RELATIONSHIP_MISSION_PEDESTRIAN;

        bankMissions = new BankMissions();
        objectiveLocation = bankMissions.PACIFIC_LOCATION;
    }

    public override void MissionTick(object o, EventArgs e)
    {
        switch (currentObjective)
        {
            case Objectives.GoToLocation:
                {
                    if (!Game.Player.Character.IsInRange(objectiveLocation, 500))
                    {
                        return;
                    }
                    var enemyPeds = bankMissions.InitializePacificRobbers();
                    var policePeds = bankMissions.InitializePacificPolice();
                    var hostagePeds = bankMissions.InitializePacificHostages();
                    vehicles = bankMissions.InitializePacificVehicles();
                    while (!MissionWorld.IsPedListLoaded(enemyPeds))
                    {
                        Script.Wait(1);
                        if (!loadingTimerStarted)
                        {
                            loadingStartTime = Game.GameTime;
                            loadingTimerStarted = true;
                        } else
                        {
                            loadingCurrentTime = Game.GameTime;
                            if (loadingCurrentTime - loadingStartTime >= 3000)
                            {
                                foreach (Ped ped in enemyPeds)
                                {
                                    if (ped != null)
                                    {
                                        ped.Delete();
                                    }
                                }
                                enemyPeds = bankMissions.InitializePacificRobbers();
                                loadingTimerStarted = false;
                            }
                        }
                    }
                    loadingTimerStarted = false;
                    while (!MissionWorld.IsPedListLoaded(policePeds))
                    {
                        Script.Wait(1);
                        if (!loadingTimerStarted)
                        {
                            loadingStartTime = Game.GameTime;
                            loadingTimerStarted = true;
                        }
                        else
                        {
                            loadingCurrentTime = Game.GameTime;
                            if (loadingCurrentTime - loadingStartTime >= 3000)
                            {
                                foreach (Ped ped in policePeds)
                                {
                                    if (ped != null)
                                    {
                                        ped.Delete();
                                    }
                                }
                                policePeds = bankMissions.InitializePacificPolice();
                                loadingTimerStarted = false;
                            }
                        }
                    }
                    loadingTimerStarted = false;
                    while (!MissionWorld.IsPedListLoaded(hostagePeds))
                    {
                        Script.Wait(1);
                        if (!loadingTimerStarted)
                        {
                            loadingStartTime = Game.GameTime;
                            loadingTimerStarted = true;
                        }
                        else
                        {
                            loadingCurrentTime = Game.GameTime;
                            if (loadingCurrentTime - loadingStartTime >= 3000)
                            {
                                foreach (Ped ped in hostagePeds)
                                {
                                    if (ped != null)
                                    {
                                        ped.Delete();
                                    }
                                }
                                hostagePeds = bankMissions.InitializePacificHostages();
                                loadingTimerStarted = false;
                            }
                        }
                    }
                    loadingTimerStarted = false;
                    while (!MissionWorld.IsVehicleListLoaded(vehicles))
                    {
                        Script.Wait(1);
                        if (!loadingTimerStarted)
                        {
                            loadingStartTime = Game.GameTime;
                            loadingTimerStarted = true;
                        }
                        else
                        {
                            loadingCurrentTime = Game.GameTime;
                            if (loadingCurrentTime - loadingStartTime >= 3000)
                            {
                                foreach (Vehicle vehicle in vehicles)
                                {
                                    if (vehicle != null)
                                    {
                                        vehicle.Delete();
                                    }
                                }
                                vehicles = bankMissions.InitializePacificVehicles();
                                loadingTimerStarted = false;
                            }
                        }
                    }
                    for (var i = 0; i < enemyPeds.Count; i++)
                    {
                        enemies.Add(new MissionPed(enemyPeds[i], robberRelGroup));
                        enemies[i].ShowBlip();
                        enemies[i].GetTask().StartScenario("WORLD_HUMAN_STAND_IMPATIENT", 0);
                        Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, enemies[i].GetPed(), 1);
                    }
                    for (var i = 0; i < policePeds.Count; i++)
                    {
                        police.Add(new MissionPed(policePeds[i], policeRelGroup, false, true));
                        police[i].GetTask().StartScenario("WORLD_HUMAN_STAND_IMPATIENT", 0);
                        Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, police[i].GetPed(), 1);
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
                    objectiveLocationBlip.Delete();
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
        objectiveLocationBlip = World.CreateBlip(objectiveLocation);
        objectiveLocationBlip.Color = BlipColor.Yellow;
        objectiveLocationBlip.Name = "Pacific Standard robbery";
        objectiveLocationBlip.ShowRoute = true;

        GTA.UI.Screen.ShowSubtitle("Go to the ~y~Pacific Standard bank~w~ and stop the robbery.", 8000);
        currentObjective = Objectives.GoToLocation;
        MissionWorld.script.Tick += MissionTick;
        return true;
    }
}