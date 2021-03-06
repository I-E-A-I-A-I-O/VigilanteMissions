using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;


class MissionFour : Mission
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
    public override Blip ObjectiveLocationBlip { get; set; }

    public MissionFour()
    {
        enemiesRelGroup = MissionWorld.RELATIONSHIP_MISSION_AGGRESSIVE;

        objectiveLocation = MostWantedMissions.MISSION_FOUR_LOCATION;
        planeDestination = MostWantedMissions.MISSION_FOUR_FAIL_LOCATION;
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
                    vehicles = MostWantedMissions.InitializeMissionFourVehicles();
                    var peds = MostWantedMissions.InitializeMissionFourPeds();
                    for (var i = 0; i < peds.Count; i++)
                    {
                        enemies.Add(new MissionPed(peds[i], enemiesRelGroup));
                        enemies[i].ShowBlip();
                        enemies[i].GetTask().FightAgainstHatedTargets(150);
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
        if (ObjectiveLocationBlip.Exists())
        {
            ObjectiveLocationBlip.Delete();
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
        ObjectiveLocationBlip = World.CreateBlip(objectiveLocation);
        ObjectiveLocationBlip.DisplayType = BlipDisplayType.BothMapSelectable;
        ObjectiveLocationBlip.Color = BlipColor.Yellow;
        ObjectiveLocationBlip.ShowRoute = true;
        ObjectiveLocationBlip.Name = "Frank Abagnale's location";

        GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Wanted suspect", "Found him, he's at the ~y~airport~w~ with a fake identity trying to board a private jet. I'm sending you the location.");
        GTA.UI.Screen.ShowSubtitle("Go to the ~y~Airport~w~.");

        MissionWorld.script.Tick += MissionTick;
        return true;
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

        var models = new List<Model>()
        {
            new Model(PedHash.Pilot01SMM),
            new Model(PedHash.Bankman),
            new Model(PedHash.PoloGoon01GMY)
        };

        Loading.LoadModels(models);

        enemies.Add(new MissionPed(vehicles[(int)Vehicles.Plane].CreatePedOnSeat(VehicleSeat.Driver, models[0]), enemies[0].GetPed().RelationshipGroup));
        enemies.Add(new MissionPed(vehicles[(int)Vehicles.Plane].CreatePedOnSeat(VehicleSeat.Passenger, models[1]), enemies[0].GetPed().RelationshipGroup));

        enemies.Add(new MissionPed(vehicles[(int)Vehicles.ChaseVehicle].CreatePedOnSeat(VehicleSeat.Driver, models[2]), enemies[0].GetPed().RelationshipGroup));
        enemies.Add(new MissionPed(vehicles[(int)Vehicles.ChaseVehicle].CreatePedOnSeat(VehicleSeat.Passenger, models[2]), enemies[0].GetPed().RelationshipGroup));

        Loading.UnloadModels(models);

        Script.Wait(0);

        Function.Call(Hash.TASK_PLANE_MISSION, enemies[(int)Enemies.Pilot].GetPed().Handle, vehicles[(int)Vehicles.Plane].Handle, 0, 0, planeDestination.X, planeDestination.Y, planeDestination.Z, 4, 100f, 0f, 90f, 0, -5000f);
        enemies[(int)Enemies.ChaseGuard01].GetPed().Task.VehicleChase(Game.Player.Character);

        MissionWorld.script.Tick += CheckPlaneLocation;
    }

    void CheckPlaneLocation(object o, EventArgs e)
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
        if (vehicles[(int)Vehicles.Plane].IsInRange(planeDestination, 250))
        {
            MissionWorld.QuitMission();
            Progress.missionsFailedCount += 1;
            VigilanteMissions.SaveProgress();
            GTA.UI.Screen.ShowSubtitle("~r~Mission failed, the target escaped!");
            MissionWorld.script.Tick -= CheckPlaneLocation;
        }
    }
}
