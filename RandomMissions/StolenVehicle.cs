using GTA;
using GTA.Native;
using GTA.Math;
using System;
using System.Collections.Generic;

class StolenVehicle : Mission
{
    public override bool IsMostWanted => false;

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
    public override Blip ObjectiveLocationBlip { get; set; }

    public StolenVehicle()
    {
        enemiesRelGroup = MissionWorld.RELATIONSHIP_MISSION_NEUTRAL;
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
                    ObjectiveLocationBlip.Delete();
                    var vehicle = RandomMissions.CreateVehicle(objectiveLocation);
                    vehicle = (Vehicle)MissionWorld.EntityLoadLoop(vehicle, RandomMissions.CreateVehicle, objectiveLocation);
                    var ped = vehicle.CreatePedOnSeat(VehicleSeat.Driver, new Model(PedHash.MexGoon01GMY));
                    ped = (Ped)MissionWorld.EntityLoadLoop(ped, vehicle, VehicleSeat.Driver, new Model(PedHash.MexGoon01GMY));
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
        if (ObjectiveLocationBlip.Exists())
        {
            ObjectiveLocationBlip.Delete();
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
                objectiveLocation = RandomMissions.GetRandomLocation(RandomMissions.LocationType.Vehicle);
            } while (Game.Player.Character.IsInRange(objectiveLocation, 200f));

            currentObjective = Objectives.GoToLocation;
            ObjectiveLocationBlip = World.CreateBlip(objectiveLocation, 150f);
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
