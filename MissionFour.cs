using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;


class MissionFour : Mission
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
        ChattingGuard01,
        ChattingGuard02,
        ChattingGuard03,
        ChattingGuard04,
        EntranceGuard,
        StairsGuard,
        DoorGuard,
        TrashGuard,
        Pilot,
        Target,
        BodyGuard,
        ChaseGuard01,
        ChaseGuard02
    }

    enum Vehicles
    {
        Plane = 0,
        ChaseVehicle
    }

    Vector3 objectiveLocation;
    Vector3 planeDestination;
    RelationshipGroup enemiesRelGroup;
    Objectives currentObjective;
    List<MissionPed> enemies = new List<MissionPed>();
    List<Vehicle> vehicles = new List<Vehicle>();
    Blip objectiveLocationBlip;
    int loadingCurrentTime;
    int loadingStartTime;
    bool loadingTimerStarted = false;

    public MissionFour()
    {
        enemiesRelGroup = MissionWorld.RELATIONSHIP_MISSION_AGGRESSIVE;

        objectiveLocation = MostWantedMissions.MISSION_FOUR_LOCATION;
        planeDestination = MostWantedMissions.MISSION_FOUR_FAIL_LOCATION;
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
                    Music.IncreaseIntensity();
                    objectiveLocationBlip.Delete();
                    vehicles = MostWantedMissions.InitializeMissionFourVehicles();
                    var peds = MostWantedMissions.InitializeMissionFourPeds();
                    while (!MissionWorld.IsPedListLoaded(peds))
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
                                foreach (Ped ped in peds)
                                {
                                    if (ped != null)
                                    {
                                        ped.Delete();
                                    }
                                }
                                peds = MostWantedMissions.InitializeMissionFourPeds();
                                loadingTimerStarted = false;
                            }
                        }
                    }
                    while (!MissionWorld.IsVehicleListLoaded(vehicles))
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
                                foreach (Vehicle vehicle in vehicles)
                                {
                                    if (vehicle != null)
                                    {
                                        vehicle.Delete();
                                    }
                                }
                                vehicles = MostWantedMissions.InitializeMissionFourVehicles();
                                loadingTimerStarted = false;
                            }
                        }
                    }
                    for (var i = 0; i < peds.Count; i++)
                    {
                        enemies.Add(new MissionPed(peds[i], enemiesRelGroup));
                        enemies[i].ShowBlip();
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
        if (objectiveLocationBlip.Exists())
        {
            objectiveLocationBlip.Delete();
        }
        RemoveVehiclesAndNeutrals();
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
        objectiveLocationBlip = World.CreateBlip(objectiveLocation, 150f);
        objectiveLocationBlip.Color = BlipColor.Yellow;
        objectiveLocationBlip.ShowRoute = true;
        objectiveLocationBlip.Name = "Wanted suspect location";

        GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Wanated suspect", "Ok, i tracked them down, i'm sending you the location.");
        GTA.UI.Screen.ShowSubtitle("Go to the ~y~wanted suspect~w~.");

        MissionWorld.script.Tick += MissionTick;
        return true;
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

    void StartScenarios()
    {
        GTA.UI.Screen.ShowSubtitle("The target is trying to flee in that ~r~plane~w~!");

        vehicles[(int)Vehicles.Plane].AddBlip();
        vehicles[(int)Vehicles.Plane].AttachedBlip.Sprite = BlipSprite.Plane;
        vehicles[(int)Vehicles.Plane].AttachedBlip.Color = BlipColor.Red;
        vehicles[(int)Vehicles.Plane].AttachedBlip.Name = "Wanted target";

        vehicles[(int)Vehicles.ChaseVehicle].AddBlip();
        vehicles[(int)Vehicles.ChaseVehicle].AttachedBlip.Sprite = BlipSprite.PersonalVehicleCar;
        vehicles[(int)Vehicles.ChaseVehicle].AttachedBlip.Color = BlipColor.Red;
        vehicles[(int)Vehicles.ChaseVehicle].AttachedBlip.Name = "Wanted target";

        enemies.Add(new MissionPed(vehicles[(int)Vehicles.Plane].CreatePedOnSeat(VehicleSeat.Driver, PedHash.Pilot01SMM), enemies[0].GetPed().RelationshipGroup));
        enemies.Add(new MissionPed(vehicles[(int)Vehicles.Plane].CreatePedOnSeat(VehicleSeat.Passenger, PedHash.Bankman), enemies[0].GetPed().RelationshipGroup));
        enemies.Add(new MissionPed(vehicles[(int)Vehicles.Plane].CreatePedOnSeat(VehicleSeat.ExtraSeat1, PedHash.MerryWeatherCutscene), enemies[0].GetPed().RelationshipGroup));

        enemies.Add(new MissionPed(vehicles[(int)Vehicles.ChaseVehicle].CreatePedOnSeat(VehicleSeat.Driver, PedHash.PoloGoon01GMY), enemies[0].GetPed().RelationshipGroup));
        enemies.Add(new MissionPed(vehicles[(int)Vehicles.ChaseVehicle].CreatePedOnSeat(VehicleSeat.Passenger, PedHash.PoloGoon01GMY), enemies[0].GetPed().RelationshipGroup));

        Function.Call(Hash.TASK_PLANE_MISSION, enemies[(int)Enemies.Pilot].GetPed(), vehicles[(int)Vehicles.Plane], 0, 0, planeDestination.X, planeDestination.Y, planeDestination.Z, 4, 100f, 0f, 90f, 0, -5000f);
        this.enemies[(int)Enemies.ChaseGuard01].GetPed().Task.VehicleChase(Game.Player.Character);

        MissionWorld.script.Tick += CheckPlaneLocation;
    }

    void CheckPlaneLocation(Object o, EventArgs e)
    {
        if (!MissionWorld.isMissionActive || enemies.Count == 0)
        {
            MissionWorld.script.Tick -= CheckPlaneLocation;
            return;
        }
        if (vehicles[(int)Vehicles.Plane].IsDead)
        {
            vehicles[(int)Vehicles.Plane].AttachedBlip.Delete();
            MissionWorld.script.Tick -= CheckPlaneLocation;
            return;
        }
        if (vehicles[(int)Vehicles.Plane].IsInRange(planeDestination, 100))
        {
            MissionWorld.QuitMission();
            GTA.UI.Screen.ShowSubtitle("~r~Mission failed, the target escaped!");
            MissionWorld.script.Tick -= CheckPlaneLocation;
        }
    }
}
