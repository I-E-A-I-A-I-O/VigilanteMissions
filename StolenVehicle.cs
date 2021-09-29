using GTA;
using GTA.Native;
using GTA.Math;
using System;
using System.Collections.Generic;

class StolenVehicle : Mission
{
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
    List<Vehicle> vehicles = new List<Vehicle>();
    Objectives currentObjective;
    RandomMissions randomMissions;
    Blip objectiveLocationBlip;
    int loadingCurrentTime;
    int loadingStartTime;
    bool loadingTimerStarted = false;

    public StolenVehicle()
    {
        enemiesRelGroup = MissionWorld.RELATIONSHIP_MISSION_NEUTRAL;

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
                    var vehicle = randomMissions.CreateVehicle(objectiveLocation);
                    while (!MissionWorld.IsEntityLoaded(vehicle))
                    {
                        Script.Wait(1);
                        if (!loadingTimerStarted)
                        {
                            loadingTimerStarted = true;
                            loadingStartTime = Game.GameTime;
                        }
                        else
                        {
                            loadingCurrentTime = Game.GameTime;
                            if (loadingCurrentTime - loadingStartTime >= 3000)
                            {
                                vehicle = randomMissions.CreateVehicle(objectiveLocation);
                                loadingTimerStarted = false;
                            }
                        }
                    }
                    var ped = vehicle.CreatePedOnSeat(VehicleSeat.Driver, new Model(PedHash.MexGoon01GMY));
                    while (!MissionWorld.IsEntityLoaded(ped))
                    {
                        Script.Wait(1);
                        if (!loadingTimerStarted)
                        {
                            loadingTimerStarted = true;
                            loadingStartTime = Game.GameTime;
                        }
                        else
                        {
                            loadingCurrentTime = Game.GameTime;
                            if (loadingCurrentTime - loadingStartTime >= 3000)
                            {
                                ped = vehicle.CreatePedOnSeat(VehicleSeat.Driver, new Model(PedHash.MexGoon01GMY));
                                loadingTimerStarted = false;
                            }
                        }
                    }
                    enemies.Add(new MissionPed(ped, enemiesRelGroup, false, true));
                    vehicles.Add(vehicle);
                    GTA.UI.Screen.ShowSubtitle("Kill the ~r~target~w~.", 8000);
                    enemies[0].ShowBlip();
                    enemies[0].GetPed().Task.CruiseWithVehicle(vehicle, 250, DrivingStyle.Rushed);
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
        foreach (Vehicle vehicle in vehicles)
        {
            vehicle.MarkAsNoLongerNeeded();
        }
    }

    public override bool StartMission()
    {
        try
        {
            do
            {
                objectiveLocation = randomMissions.GetRandomLocation(RandomMissions.LocationType.Vehicle);
            } while (Game.Player.Character.IsInRange(objectiveLocation, 200f));

            currentObjective = Objectives.GoToLocation;
            objectiveLocationBlip = World.CreateBlip(objectiveLocation, 150f);
            objectiveLocationBlip.Color = BlipColor.Yellow;
            objectiveLocationBlip.ShowRoute = true;
            objectiveLocationBlip.Name = "Crime scene";
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
