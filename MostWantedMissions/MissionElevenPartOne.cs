using GTA;
using GTA.Native;
using GTA.Math;
using System;
using System.Collections.Generic;

class MissionElevenPartOne : Mission
{
    public override bool IsMostWanted => true;
    public override bool IsJokerMission => true;

    enum Objectives
    {
        GoToMeeting,
        KillDirtyCia,
        GrabKeycard,
        GoToServerFarm,
        EnterServer,
        PlaceBackDoor,
        LeaveServerFarm,
        LoseCops,
        GoToChase,
        ChaseGoon,
        AirportShootout,
        GoToTruck,
        StealTruck,
        LoseCopsTruck,
        GoToBase,
        StealJet,
        KillTarget,
        Complete,
        None
    }

    Objectives currentObjective;
    Vector3 objectiveLocation;
    public override Blip ObjectiveLocationBlip { get; set; }
    List<MissionPed> enemies = new List<MissionPed>();
    List<Vehicle> vehicles = new List<Vehicle>();
    Prop prop;
    bool messageOneShown = false;
    bool messageTwoShown = false;
    bool planeGotAway = false;
    bool heliSpawned = false;
    bool chaseStarted = false;
    bool flyingMusicPlayed = false;
    int chaseStartTime;
    int chaseCurrentTime;
    RelationshipGroup enemiesRelGroup;
    RelationshipGroup neutralGroup;

    public MissionElevenPartOne()
    {
        enemiesRelGroup = MissionWorld.RELATIONSHIP_MISSION_AGGRESSIVE;
        neutralGroup = MissionWorld.RELATIONSHIP_MISSION_NEUTRAL;
        objectiveLocation = MostWantedMissions.MISSION_ELEVEN_MEETING_LOCATION;
    }

    protected override void MissionTick(object o, EventArgs e)
    {
        if (currentObjective != Objectives.LoseCops && currentObjective != Objectives.StealJet && currentObjective != Objectives.KillTarget)
        {
            if (Game.Player.WantedLevel >= 2)
            {
                Game.Player.WantedLevel = 1;
            }
        }
        if (currentObjective == Objectives.StealTruck || currentObjective == Objectives.GoToBase || currentObjective == Objectives.LoseCopsTruck)
        {
            if (vehicles[0].IsDead)
            {
                GTA.UI.Screen.ShowSubtitle("~r~Mission failed, the truck was destroyed.", 8000);
                MissionWorld.QuitMission();
                Progress.missionsFailedCount += 1;
                VigilanteMissions.SaveProgress();
            }
        }
        switch (currentObjective)
        {
            case Objectives.GoToMeeting:
                {
                    if (!Game.Player.Character.IsInRange(objectiveLocation, 200))
                    {
                        return;
                    }
                    ObjectiveLocationBlip.Delete();
                    Loading.LoadModel(MostWantedMissions.MissionElevenMeetingModel);
                    var ped = MostWantedMissions.InitializeMissionElevenMeetingPed();
                    Loading.UnloadModel(MostWantedMissions.MissionElevenMeetingModel);
                    enemies.Add(new MissionPed(ped, neutralGroup));
                    enemies[0].GetTask().StartScenario("WORLD_HUMAN_SMOKING", 0);
                    enemies[0].ShowBlip();
                    enemies[0].GetBlip().Name = "Corrupt IAA agent";
                    GTA.UI.Screen.ShowSubtitle("Kill the ~r~corrupt agent~w~.", 8000);
                    currentObjective = Objectives.KillDirtyCia;
                    break;
                }
            case Objectives.KillDirtyCia:
                {
                    if (!enemies[0].IsDead())
                    {
                        return;
                    }
                    var propModel = new Model(-1685461222);
                    Loading.LoadModel(propModel);
                    prop = World.CreateProp(propModel, enemies[0].GetPosition(), true, true);
                    Loading.UnloadModel(propModel);
                    prop.AddBlip();
                    prop.AttachedBlip.Scale = 0.7f;
                    prop.AttachedBlip.Color = BlipColor.Green;
                    prop.AttachedBlip.Name = "Keycard";
                    RemoveDeadEnemies();
                    GTA.UI.Screen.ShowSubtitle("Pick up the ~g~keycard~w~.", 8000);
                    currentObjective = Objectives.GrabKeycard;
                    break;
                }
            case Objectives.GrabKeycard:
                {
                    if (!Game.Player.Character.IsInRange(prop.Position, 1.5f))
                    {
                        return;
                    }

                    GTA.UI.Screen.ShowHelpTextThisFrame($"Press ~{VigilanteMissions.InteractionControl}~ to grab the ~g~keycard");
                    if (Function.Call<bool>(Hash.IS_CONTROL_JUST_RELEASED, 0, VigilanteMissions.InteractionControl))
                    {
                        prop.Delete();
                        objectiveLocation = MostWantedMissions.MISSION_ELEVEN_FACILITY_LOCATION;
                        ObjectiveLocationBlip = World.CreateBlip(objectiveLocation);
                        ObjectiveLocationBlip.Color = BlipColor.Yellow;
                        ObjectiveLocationBlip.Name = "IAA Server farm";
                        ObjectiveLocationBlip.ShowRoute = true;
                        GTA.UI.Screen.ShowSubtitle("Go to the ~y~IAA server farm~w~.", 8000);
                        currentObjective = Objectives.GoToServerFarm;
                    }
                    break;
                }
            case Objectives.GoToServerFarm:
                {
                    if (!Game.Player.Character.IsInRange(objectiveLocation, 200))
                    {
                        return;
                    }
                    Loading.LoadModel(MostWantedMissions.MissionElevenFarmModel);
                    var peds = MostWantedMissions.InitializeMissionElevenServerGuards();
                    Loading.UnloadModel(MostWantedMissions.MissionElevenFarmModel);
                    for (var i = 0; i < peds.Count; i++)
                    {
                        enemies.Add(new MissionPed(peds[i], enemiesRelGroup));
                        enemies[i].GetPed().Armor = 100;
                    }
                    objectiveLocation = MostWantedMissions.MISSION_ELEVEN_FACILITY_INSIDE_LOCATION;
                    currentObjective = Objectives.EnterServer;
                    break;
                }
            case Objectives.EnterServer:
                {
                    if (!Game.Player.Character.IsInRange(objectiveLocation, 20))
                    {
                        return;
                    }
                    Music.IncreaseIntensity();
                    objectiveLocation = MostWantedMissions.MISSION_ELEVEN_SERVER_LOCATION;
                    ObjectiveLocationBlip.Delete();
                    foreach (MissionPed enemy in enemies)
                    {
                        enemy.ShowBlip();
                        enemy.GetBlip().Name = "IAA agent";
                        enemy.GetTask().GuardCurrentPosition();
                        Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, enemy.GetPed().Handle, 3);
                    }
                    ObjectiveLocationBlip = World.CreateBlip(objectiveLocation);
                    ObjectiveLocationBlip.Color = BlipColor.Yellow;
                    ObjectiveLocationBlip.Name = "Target server";
                    currentObjective = Objectives.PlaceBackDoor;
                    break;
                }
            case Objectives.PlaceBackDoor:
                {
                    if (GTA.UI.Screen.IsFadedIn && !messageOneShown)
                    {
                        GTA.UI.Screen.ShowSubtitle("Insert the device in the ~y~server~w~.", 8000);
                        messageOneShown = true;
                    }
                    if (enemies.Count > 0)
                    {
                        RemoveDeadEnemies();
                    }
                    if (!Game.Player.Character.IsInRange(objectiveLocation, 1.3f))
                    {
                        return;
                    }

                    GTA.UI.Screen.ShowHelpTextThisFrame($"Press ~{VigilanteMissions.InteractionControl}~ to insert the device");
                    if (Function.Call<bool>(Hash.IS_CONTROL_JUST_RELEASED, 0, VigilanteMissions.InteractionControl))
                    {
                        var sequence = new TaskSequence();
                        sequence.AddTask.AchieveHeading(Vector3.RelativeBack.ToHeading());
                        sequence.AddTask.PlayAnimation("weapons@first_person@aim_rng@generic@projectile@thermal_charge@", "plant_vertical", 8, -1, AnimationFlags.UpperBodyOnly);
                        Game.Player.Character.Task.PerformSequence(sequence);
                        Script.Wait(2000);
                        ObjectiveLocationBlip.Delete();
                        GTA.UI.Screen.ShowSubtitle("Leave the server farm.", 8000);
                        objectiveLocation = MostWantedMissions.MISSION_ELEVEN_FACILITY_LOCATION;
                        currentObjective = Objectives.LeaveServerFarm;
                    }
                    break;
                }
            case Objectives.LeaveServerFarm:
                {
                    if (enemies.Count > 0)
                    {
                        RemoveDeadEnemies();
                    }
                    if (!Game.Player.Character.IsInRange(objectiveLocation, 40))
                    {
                        return;
                    }
                    foreach (MissionPed ped in enemies)
                    {
                        ped.Delete(true);
                    }
                    enemies.Clear();
                    Music.LowerIntensinty();
                    Game.Player.WantedLevel = 4;
                    currentObjective = Objectives.LoseCops;
                    break;
                }
            case Objectives.LoseCops:
                {
                    if (GTA.UI.Screen.IsFadedIn && !messageTwoShown)
                    {
                        GTA.UI.Screen.ShowSubtitle("Lose the cops.", 8000);
                        messageTwoShown = true;
                    }
                    if (Game.Player.WantedLevel > 0)
                    {
                        return;
                    }
                    objectiveLocation = MostWantedMissions.MISSION_ELEVEN_CHASE_START_LOCATION;
                    ObjectiveLocationBlip = World.CreateBlip(objectiveLocation);
                    ObjectiveLocationBlip.Name = "Joker's goon location";
                    ObjectiveLocationBlip.Color = BlipColor.Yellow;
                    ObjectiveLocationBlip.ShowRoute = true;
                    GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Vigilante Mission", "Ok, i tracked 'someone', but i'm not sure if that's him. You better get to that ~y~location~w~ fast.");
                    GTA.UI.Screen.ShowSubtitle("Go to the ~y~location~w~.", 8000);
                    currentObjective = Objectives.GoToChase;
                    Script.Wait(3000);
                    Music.StartHeistMusic();
                    break;
                }
            case Objectives.GoToChase:
                {
                    if (!Game.Player.Character.IsInRange(objectiveLocation, 200))
                    {
                        return;
                    }
                    ObjectiveLocationBlip.Delete();
                    objectiveLocation = MostWantedMissions.MISSION_ELEVEN_CHASE_END_LOCATION;
                    Music.IncreaseIntensity();
                    Loading.LoadModels(MostWantedMissions.MissionElevenChaseModels);
                    var vehicle = MostWantedMissions.InitializeMissionElevenChaseVehicle();
                    vehicles.Add(vehicle);
                    var ped = vehicle.CreatePedOnSeat(VehicleSeat.Driver, new Model(PedHash.MPros01));
                    Loading.UnloadModels(MostWantedMissions.MissionElevenChaseModels);
                    enemies.Add(new MissionPed(ped, enemiesRelGroup));
                    ped.Task.DriveTo(vehicle, objectiveLocation, 15, 350, DrivingStyle.AvoidTraffic);
                    ped.BlockPermanentEvents = true;
                    vehicle.AddBlip();
                    vehicle.AttachedBlip.Sprite = BlipSprite.PersonalVehicleCar;
                    vehicle.AttachedBlip.Color = BlipColor.Red;
                    vehicle.AttachedBlip.Name = "Joker's goon";
                    GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Vigilante Mission", "Fuck, that's not him. You better follow him, he might take us to the Joker.");
                    GTA.UI.Screen.ShowSubtitle("Follow the ~r~goon~w~.", 8000);
                    currentObjective = Objectives.ChaseGoon;
                    break;
                }
            case Objectives.ChaseGoon:
                {
                    if (!enemies[0].GetPed().IsInRange(Game.Player.Character.Position, 350))
                    {
                        MissionWorld.QuitMission();
                        Progress.missionsFailedCount += 1;
                        VigilanteMissions.SaveProgress();
                        GTA.UI.Screen.ShowSubtitle("~r~Mission failed, the goon got away!", 8000);
                        return;
                    }
                    if (enemies[0].IsDead())
                    {
                        RemoveDeadEnemies();
                        MissionWorld.QuitMission();
                        Progress.missionsFailedCount += 1;
                        VigilanteMissions.SaveProgress();
                        GTA.UI.Screen.ShowSubtitle("~r~Mission failed, the goon is dead!", 8000);
                        return;
                    }
                    if (!chaseStarted)
                    {
                        chaseStartTime = Game.GameTime;
                        chaseStarted = true;
                    } else
                    {
                        chaseCurrentTime = Game.GameTime;
                        if (chaseCurrentTime - chaseStartTime >= 60000 && !heliSpawned)
                        {
                            SpawnChaseHelicopter();
                            heliSpawned = true;
                        }
                    }
                    if (!Game.Player.Character.IsInRange(objectiveLocation, 200))
                    {
                        return;
                    }
                    enemies[0].GetPed().BlockPermanentEvents = false;
                    objectiveLocation = MostWantedMissions.MISSION_ELEVEN_PLANE_GETAWAY_LOCATION;
                    Loading.LoadModels(MostWantedMissions.MissionElevenAirportModels);
                    var airportVehicles = MostWantedMissions.InitializeMissionElevenAirportVehicles();
                    var airportPeds = MostWantedMissions.InitializeMissionElevenAirportPeds();
                    vehicles.AddRange(airportVehicles);
                    foreach (Ped ped in airportPeds)
                    {
                        enemies.Add(new MissionPed(ped, enemiesRelGroup));
                        ped.Task.StartScenario("WORLD_HUMAN_STAND_IMPATIENT", 0);
                        ped.AddBlip();
                        ped.AttachedBlip.Scale = 0.8f;
                        ped.AttachedBlip.Color = BlipColor.Red;
                        ped.AttachedBlip.Name = "Joker's goon";
                    }
                    var jokerPed = vehicles[vehicles.Count - 1].CreatePedOnSeat(VehicleSeat.Driver, new Model(PedHash.Clown01SMY));
                    Loading.UnloadModels(MostWantedMissions.MissionElevenAirportModels);
                    enemies.Add(new MissionPed(jokerPed, enemiesRelGroup));
                    jokerPed.BlockPermanentEvents = true;
                    Function.Call(Hash.TASK_PLANE_MISSION, jokerPed.Handle, vehicles[vehicles.Count - 1].Handle, 0, 0, objectiveLocation.X, objectiveLocation.Y, objectiveLocation.Z, 4, 100f, 0f, 90f, 0, -5000f);
                    vehicles[vehicles.Count - 1].AddBlip();
                    vehicles[vehicles.Count - 1].AttachedBlip.Sprite = BlipSprite.Plane;
                    vehicles[vehicles.Count - 1].AttachedBlip.Color = BlipColor.Red;
                    vehicles[vehicles.Count - 1].AttachedBlip.Name = "Joker's plane";
                    GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Vigilante Mission", "There he is, don't let him get away!!!");
                    GTA.UI.Screen.ShowSubtitle("Kill the ~r~Joker~w~ and his ~r~goons~w~.", 8000);
                    currentObjective = Objectives.AirportShootout;
                    break;
                }
            case Objectives.AirportShootout:
                {
                    if (vehicles[vehicles.Count -1].IsInRange(objectiveLocation, 100) && !planeGotAway)
                    {
                        enemies[enemies.Count - 1].Delete();
                        vehicles[vehicles.Count - 1].MarkAsNoLongerNeeded();
                        enemies.RemoveAt(enemies.Count - 1);
                        vehicles.RemoveAt(vehicles.Count - 1);
                        GTA.UI.Screen.ShowSubtitle("~r~The Joker got away", 8000);
                        GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Vigilante Mission", "Dammit, he managed to escape. Get rid of his goons, we can still catch him");
                        planeGotAway = true;
                        return;
                    }
                    if (enemies.Count > 0)
                    {
                        RemoveDeadEnemies();
                    } else
                    {
                        if (!planeGotAway)
                        {
                            currentObjective = Objectives.Complete;
                            return;
                        }
                        Music.LowerIntensinty();
                        objectiveLocation = MostWantedMissions.MISSION_ELEVEN_MARINE_TRUCK_LOCATION;
                        ObjectiveLocationBlip = World.CreateBlip(objectiveLocation);
                        ObjectiveLocationBlip.Color = BlipColor.Yellow;
                        ObjectiveLocationBlip.Name = "Military truck location";
                        ObjectiveLocationBlip.ShowRoute = true;
                        GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Vigilante Mission", "I tracked down a ~y~military truck~w~ use it to get into ~y~Fort Zancudo~w~ and steal a lazer.");
                        GTA.UI.Screen.ShowSubtitle("Find the ~y~truck~w~.", 8000);
                        RemoveVehiclesAndNeutrals();
                        vehicles.Clear();
                        currentObjective = Objectives.GoToTruck;
                    }
                    break;
                }
            case Objectives.GoToTruck:
                {
                    if (!Game.Player.Character.IsInRange(objectiveLocation, 200))
                    {
                        return;
                    }
                    ObjectiveLocationBlip.Delete();
                    Loading.LoadModels(MostWantedMissions.MissionElevenTruckModels);
                    var truck = MostWantedMissions.InitializeMissionElevenMarineTruck();
                    var truckDriver = truck.CreatePedOnSeat(VehicleSeat.Driver, new Model(PedHash.Marine01SMM));
                    var truckPassenger = truck.CreatePedOnSeat(VehicleSeat.Passenger, new Model(PedHash.Marine01SMY));
                    Loading.UnloadModels(MostWantedMissions.MissionElevenTruckModels);
                    enemies.Add(new MissionPed(truckDriver, MissionWorld.RELATIONSHIP_MISSION_NEUTRAL_COP_FRIENDLY, false, true));
                    enemies.Add(new MissionPed(truckPassenger, MissionWorld.RELATIONSHIP_MISSION_NEUTRAL_COP_FRIENDLY, false, true));
                    vehicles.Add(truck);
                    truck.AddBlip();
                    truck.AttachedBlip.Sprite = BlipSprite.PersonalVehicleCar;
                    truck.AttachedBlip.Color = BlipColor.Yellow;
                    truck.AttachedBlip.Name = "Truck";
                    truckDriver.Task.CruiseWithVehicle(truck, 100);
                    GTA.UI.Screen.ShowSubtitle("Steal the ~y~truck~w~.", 8000);
                    currentObjective = Objectives.StealTruck;
                    break;
                }
            case Objectives.StealTruck:
                {
                    if (Game.Player.WantedLevel > 0)
                    {
                        if (vehicles[0].AttachedBlip != null && vehicles[0].AttachedBlip.Exists())
                        {
                            vehicles[0].AttachedBlip.Delete();
                        }
                        GTA.UI.Screen.ShowSubtitle("Lose the cops.", 8000);
                        currentObjective = Objectives.LoseCopsTruck;
                        return;
                    }
                    if (enemies.Count > 0)
                    {
                        RemoveDeadEnemies();
                    }
                    if (vehicles[0].Driver != null && vehicles[0].Driver == Game.Player.Character)
                    {
                        vehicles[0].AttachedBlip.Delete();
                        objectiveLocation = MostWantedMissions.MISSION_ELEVEN_FORT_ZANCUDO_LOCATION;
                        ObjectiveLocationBlip = World.CreateBlip(objectiveLocation);
                        ObjectiveLocationBlip.Color = BlipColor.Yellow;
                        ObjectiveLocationBlip.Name = "Fort Zancudo";
                        ObjectiveLocationBlip.ShowRoute = true;
                        GTA.UI.Screen.ShowSubtitle("Go to ~y~Fort Zancudo~w~.", 8000);
                        currentObjective = Objectives.GoToBase;
                    }
                    break;
                }
            case Objectives.GoToBase:
                {
                    if (enemies.Count > 0)
                    {
                        RemoveDeadEnemies();
                    }
                    if (Game.Player.WantedLevel > 0)
                    {
                        if (ObjectiveLocationBlip!= null && ObjectiveLocationBlip.Exists())
                        {
                            ObjectiveLocationBlip.Delete();
                        }
                        GTA.UI.Screen.ShowSubtitle("Lose the cops.", 8000);
                        currentObjective = Objectives.LoseCopsTruck;
                        return;
                    }
                    if (vehicles[0].Driver == null)
                    {
                        ObjectiveLocationBlip.Delete();
                        vehicles[0].AddBlip();
                        vehicles[0].AttachedBlip.Sprite = BlipSprite.PersonalVehicleCar;
                        vehicles[0].AttachedBlip.Color = BlipColor.Yellow;
                        vehicles[0].AttachedBlip.Name = "Truck";
                        GTA.UI.Screen.ShowSubtitle("Get in the ~y~Truck~w~.", 8000);
                        currentObjective = Objectives.StealTruck;
                        return;
                    }
                    if (vehicles[0].IsInRange(objectiveLocation, 5))
                    {
                        ObjectiveLocationBlip.Delete();
                        GTA.UI.Screen.ShowSubtitle("Enter the base and steal a lazer.", 8000);
                        RemoveVehiclesAndNeutrals();
                        vehicles.Clear();
                        Loading.LoadModel(MostWantedMissions.MissionElevenLazerModel);
                        var lazer = MostWantedMissions.InitializeMissionElevenLazer();
                        Loading.UnloadModel(MostWantedMissions.MissionElevenLazerModel);
                        lazer.AddBlip();
                        lazer.AttachedBlip.Sprite = BlipSprite.Plane;
                        lazer.AttachedBlip.Color = BlipColor.Yellow;
                        lazer.AttachedBlip.Name = "Lazer";
                        lazer.AttachedBlip.IsShortRange = true;
                        vehicles.Add(lazer);
                        currentObjective = Objectives.StealJet;
                        return;
                    }
                    break;
                }
            case Objectives.LoseCopsTruck:
                {
                    if (Game.Player.WantedLevel == 0)
                    {
                        currentObjective = Objectives.StealTruck;
                        vehicles[0].AddBlip();
                        vehicles[0].AttachedBlip.Sprite = BlipSprite.PersonalVehicleCar;
                        vehicles[0].AttachedBlip.Color = BlipColor.Yellow;
                        vehicles[0].AttachedBlip.Name = "Truck";
                    }
                    break;
                }
            case Objectives.StealJet:
                {
                    Function.Call(Hash.TERMINATE_ALL_SCRIPTS_WITH_THIS_NAME, "am_armybase");
                    Function.Call(Hash.TERMINATE_ALL_SCRIPTS_WITH_THIS_NAME, "restrictedareas");
                    Function.Call(Hash.TERMINATE_ALL_SCRIPTS_WITH_THIS_NAME, "re_armybase");
                    if (Game.Player.WantedLevel >= 1)
                    {
                        Game.Player.WantedLevel = 0;
                    }
                    if (!Game.Player.Character.IsInRange(objectiveLocation, 3000))
                    {
                        GTA.UI.Screen.ShowSubtitle("~r~Mission abandoned.", 8000);
                        MissionWorld.QuitMission();
                        return;
                    }
                    if (Game.Player.LastVehicle != null && Game.Player.LastVehicle.Model == new Model(VehicleHash.Lazer))
                    {
                        if (enemies.Count > 0)
                        {
                            foreach (MissionPed enemy in enemies)
                            {
                                enemy.Delete();
                            }
                            enemies.Clear();
                        }
                        RemoveVehiclesAndNeutrals();
                        vehicles.Clear();
                        objectiveLocation = MostWantedMissions.MISSION_ELEVEN_PLANE_GETAWAY_LOCATION;
                        Loading.LoadModels(MostWantedMissions.MissionElevenAirChaseModels);
                        var plane = World.CreateVehicle(new Model(VehicleHash.Luxor), objectiveLocation);
                        plane.IsEngineRunning = true;
                        plane.AddBlip();
                        plane.AttachedBlip.Sprite = BlipSprite.Plane;
                        plane.AttachedBlip.Color = BlipColor.Red;
                        plane.AttachedBlip.Name = "The Joker";
                        objectiveLocation = MostWantedMissions.MISSION_ELEVEN_PLANE_CRASH_LOCATION;
                        var joker = plane.CreatePedOnSeat(VehicleSeat.Driver, new Model(PedHash.Clown01SMY));
                        Loading.UnloadModels(MostWantedMissions.MissionElevenAirChaseModels);
                        joker.BlockPermanentEvents = true;
                        Function.Call(Hash.TASK_PLANE_MISSION, joker.Handle, plane.Handle, 0, 0, objectiveLocation.X, objectiveLocation.Y, objectiveLocation.Z, 4, 100f, 0f, 90f, 2700f, 1000f);
                        enemies.Add(new MissionPed(joker, enemiesRelGroup));
                        vehicles.Add(plane);
                        GTA.UI.Screen.ShowSubtitle("Kill ~r~The Joker~w~.", 8000);
                        currentObjective = Objectives.KillTarget;
                    }
                    break;
                }
            case Objectives.KillTarget:
                {
                    if (Game.Player.Character.IsInRange(MostWantedMissions.MISSION_ELEVEN_FORT_ZANCUDO_LOCATION, 3000))
                    {
                        Function.Call(Hash.TERMINATE_ALL_SCRIPTS_WITH_THIS_NAME, "am_armybase");
                        Function.Call(Hash.TERMINATE_ALL_SCRIPTS_WITH_THIS_NAME, "restrictedareas");
                        Function.Call(Hash.TERMINATE_ALL_SCRIPTS_WITH_THIS_NAME, "re_armybase");
                        if (Game.Player.WantedLevel >= 1)
                        {
                            Game.Player.WantedLevel = 0;
                        }
                    }
                    if (Game.Player.LastVehicle != null && Game.Player.LastVehicle.IsInAir && !flyingMusicPlayed)
                    {
                        Script.Wait(2000);
                        Music.StartTedBundyMusic();
                        Music.IncreaseIntensity();
                        flyingMusicPlayed = true;

                    }
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
                    RemoveVehiclesAndNeutrals();
                    GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Wanted Suspect", "Good job, your cut of the reward is already in your account.");
                    currentObjective = Objectives.None;
                    Game.Player.Money += 50000;
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
        if (ObjectiveLocationBlip != null && ObjectiveLocationBlip.Exists())
        {
            ObjectiveLocationBlip.Delete();
        }
        if (prop != null)
        {
            if (prop.AttachedBlip != null)
            {
                prop.AttachedBlip.Delete();
            }
            prop.MarkAsNoLongerNeeded();
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
            if (vehicle != null)
            {
                vehicle.MarkAsNoLongerNeeded();
            }
        }
    }

    public override bool StartMission()
    {
        if (Game.Player.Character.IsInRange(objectiveLocation, 200))
        {
            GTA.UI.Notification.Show("Mission not available.");
            return false;
        }
        Music.StartFunky();
        GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Wanted suspect", "We are going to need access to the IAA servers if we want to find this fucker. I set up a fake ~y~meeting~w~ with a corrupt agent, go get his ~g~keycard~w~.");
        ObjectiveLocationBlip = World.CreateBlip(objectiveLocation);
        ObjectiveLocationBlip.DisplayType = BlipDisplayType.BothMapSelectable;
        ObjectiveLocationBlip.Color = BlipColor.Yellow;
        ObjectiveLocationBlip.Name = "Meeting with IAA agent";
        ObjectiveLocationBlip.ShowRoute = true;
        GTA.UI.Screen.ShowSubtitle("Go to the ~y~meeting~w~.", 8000);
        currentObjective = Objectives.GoToMeeting;
        MissionWorld.script.Tick += MissionTick;
        return true;
    }

    void SpawnChaseHelicopter()
    {
        var position = Game.Player.Character.Position;
        Loading.LoadModels(MostWantedMissions.MissionElevenHeliModels);
        var heli = World.CreateVehicle(new Model(VehicleHash.Buzzard), new Vector3(position.X, position.Y, position.Z + 50));
        heli.IsEngineRunning = true;
        heli.AddBlip();
        heli.AttachedBlip.Sprite = BlipSprite.HelicopterAnimated;
        heli.AttachedBlip.Color = BlipColor.Red;
        heli.AttachedBlip.Name = "Enemy";
        vehicles.Add(heli);
        var peds = new List<Ped>
        {
            heli.CreatePedOnSeat(VehicleSeat.Driver, new Model(PedHash.MPros01)),
            heli.CreatePedOnSeat(VehicleSeat.LeftRear, new Model(PedHash.MPros01)),
            heli.CreatePedOnSeat(VehicleSeat.RightRear, new Model(PedHash.MPros01))
        };
        Loading.UnloadModels(MostWantedMissions.MissionElevenHeliModels);
        foreach (Ped ped in peds)
        {
            enemies.Add(new MissionPed(ped, enemiesRelGroup));
        }
        peds[0].Task.ChaseWithHelicopter(Game.Player.Character, new Vector3(4, 4, -10));
        peds[1].Task.VehicleShootAtPed(Game.Player.Character);
        peds[2].Task.VehicleShootAtPed(Game.Player.Character);
    }
}
