using GTA;
using GTA.Native;
using GTA.Math;
using System;
using System.Collections.Generic;

class MissionElevenPartOne : Mission
{
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
        Complete,
        None
    }

    Objectives currentObjective;
    Vector3 objectiveLocation;
    Blip objectiveLocationBlip;
    List<MissionPed> enemies = new List<MissionPed>();
    List<Vehicle> vehicles = new List<Vehicle>();
    Prop prop;
    int loadingStartTime;
    int loadingCurrentTime;
    bool loadingTimerStarted;
    bool messageOneShown = false;
    bool messageTwoShown = false;
    bool planeGotAway = false;
    bool heliOneSpawned = false;
    bool heliTwoSpawned = false;
    bool chaseStarted = false;
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

    public override void MissionTick(object o, EventArgs e)
    {
        if (currentObjective != Objectives.LoseCops)
        {
            if (Game.Player.WantedLevel >= 2)
            {
                Game.Player.WantedLevel = 1;
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
                    objectiveLocationBlip.Delete();
                    var ped = MostWantedMissions.InitializeMissionElevenMeetingPed();
                    while (!MissionWorld.IsEntityLoaded(ped))
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
                                ped = MostWantedMissions.InitializeMissionElevenMeetingPed();
                                loadingTimerStarted = false;
                            }
                        }
                    }
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
                    prop = World.CreateProp(new Model(-1685461222), enemies[0].GetPosition(), true, true);
                    while (!MissionWorld.IsEntityLoaded(prop))
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
                                prop = World.CreateProp(new Model(-1685461222), enemies[0].GetPosition(), true, true);
                                loadingTimerStarted = false;
                            }
                        }
                    }
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
                    if (Game.LastInputMethod == InputMethod.GamePad)
                    {
                        GTA.UI.Screen.ShowHelpTextThisFrame("Press DPad Right to grab the ~g~keycard");
                    }
                    else
                    {
                        GTA.UI.Screen.ShowHelpTextThisFrame($"Press {VigilanteMissions.interactKey} to grab the ~g~keycard");
                    }
                    if ((Game.LastInputMethod == InputMethod.MouseAndKeyboard && Game.IsKeyPressed(VigilanteMissions.interactMissionKey)) || (Game.LastInputMethod == InputMethod.GamePad && Game.IsControlJustReleased(GTA.Control.ScriptPadRight)))
                    {
                        prop.Delete();
                        objectiveLocation = MostWantedMissions.MISSION_ELEVEN_FACILITY_LOCATION;
                        objectiveLocationBlip = World.CreateBlip(objectiveLocation);
                        objectiveLocationBlip.Color = BlipColor.Yellow;
                        objectiveLocationBlip.Name = "IAA Server farm";
                        objectiveLocationBlip.ShowRoute = true;
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
                    var peds = MostWantedMissions.InitializeMissionElevenServerGuards();
                    while(!MissionWorld.IsPedListLoaded(peds))
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
                                foreach (Ped ped in peds)
                                {
                                    if (ped != null)
                                    {
                                        ped.Delete();
                                    }
                                    peds = MostWantedMissions.InitializeMissionElevenServerGuards();
                                }
                            }
                        }
                    }
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
                    objectiveLocationBlip.Delete();
                    foreach (MissionPed enemy in enemies)
                    {
                        enemy.ShowBlip();
                        enemy.GetBlip().Name = "IAA agent";
                        enemy.GetTask().GuardCurrentPosition();
                    }
                    objectiveLocationBlip = World.CreateBlip(objectiveLocation);
                    objectiveLocationBlip.Color = BlipColor.Yellow;
                    objectiveLocationBlip.Name = "Target server";
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
                    if (Game.LastInputMethod == InputMethod.GamePad)
                    {
                        GTA.UI.Screen.ShowHelpTextThisFrame("Press DPad Right to insert the device");
                    }
                    else
                    {
                        GTA.UI.Screen.ShowHelpTextThisFrame($"Press {VigilanteMissions.interactKey} to insert the device");
                    }
                    if ((Game.LastInputMethod == InputMethod.MouseAndKeyboard && Game.IsKeyPressed(VigilanteMissions.interactMissionKey)) || (Game.LastInputMethod == InputMethod.GamePad && Game.IsControlJustReleased(GTA.Control.ScriptPadRight)))
                    {
                        var sequence = new TaskSequence();
                        sequence.AddTask.AchieveHeading(Vector3.RelativeBack.ToHeading());
                        sequence.AddTask.PlayAnimation("weapons@first_person@aim_rng@generic@projectile@thermal_charge@", "plant_vertical", 8, -1, AnimationFlags.UpperBodyOnly);
                        Game.Player.Character.Task.PerformSequence(sequence);
                        Script.Wait(2000);
                        objectiveLocationBlip.Delete();
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
                        ped.Delete();
                    }
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
                    objectiveLocationBlip = World.CreateBlip(objectiveLocation);
                    objectiveLocationBlip.Name = "Joker's goon location";
                    objectiveLocationBlip.Color = BlipColor.Yellow;
                    objectiveLocationBlip.ShowRoute = true;
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
                    objectiveLocationBlip.Delete();
                    objectiveLocation = MostWantedMissions.MISSION_ELEVEN_CHASE_END_LOCATION;
                    Music.IncreaseIntensity();
                    var vehicle = MostWantedMissions.InitializeMissionElevenChaseVehicle();
                    while (!MissionWorld.IsEntityLoaded(vehicle))
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
                                vehicle = MostWantedMissions.InitializeMissionElevenChaseVehicle();
                                loadingTimerStarted = false;
                            }
                        }
                    }
                    vehicles.Add(vehicle);
                    var ped = vehicle.CreatePedOnSeat(VehicleSeat.Driver, new Model(PedHash.MPros01));
                    while (!MissionWorld.IsEntityLoaded(ped))
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
                                ped = vehicle.CreatePedOnSeat(VehicleSeat.Driver, new Model(PedHash.MPros01));
                                loadingTimerStarted = false;
                            }
                        }
                    }
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
                        GTA.UI.Screen.ShowSubtitle("~r~Mission failed, the goon got away!", 8000);
                        MissionWorld.script.Tick -= MissionTick;
                        return;
                    }
                    if (enemies[0].IsDead())
                    {
                        RemoveDeadEnemies();
                        MissionWorld.QuitMission();
                        GTA.UI.Screen.ShowSubtitle("~r~Mission failed, the goon is dead!", 8000);
                        MissionWorld.script.Tick -= MissionTick;
                        return;
                    }
                    if (!chaseStarted)
                    {
                        chaseStartTime = Game.GameTime;
                        chaseStarted = true;
                    } else
                    {
                        chaseCurrentTime = Game.GameTime;
                        if (chaseCurrentTime - chaseStartTime >= 45000 && !heliOneSpawned)
                        {
                            SpawnChaseHelicopter();
                            heliOneSpawned = true;
                        }
                        if (chaseCurrentTime - chaseStartTime >= 60000 && !heliTwoSpawned)
                        {
                            SpawnChaseHelicopter();
                            heliTwoSpawned = true;
                        }
                    }
                    if (!Game.Player.Character.IsInRange(objectiveLocation, 200))
                    {
                        return;
                    }
                    currentObjective = Objectives.Complete;
                    break;
                }
            case Objectives.Complete:
                {
                    RemoveVehiclesAndNeutrals();
                    GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Wanted Suspect", "Good job, your cut of the reward is already in your account.");
                    currentObjective = Objectives.None;
                    //Game.Player.Money += 15000;
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
        if (Game.Player.Character.IsInRange(objectiveLocation, 200))
        {
            GTA.UI.Notification.Show("Mission not available.");
            return false;
        }
        Music.StartFunky();
        GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Wanted suspect", "We are going to need access to the IAA servers if we want to find this fucker. I set up a fake ~y~meeting~w~ with a corrupt agent, go get his ~g~keycard~w~.");
        objectiveLocation = MostWantedMissions.MISSION_ELEVEN_CHASE_START_LOCATION;
        objectiveLocationBlip = World.CreateBlip(objectiveLocation);
        objectiveLocationBlip.Color = BlipColor.Yellow;
        objectiveLocationBlip.Name = "Meeting with IAA agent";
        objectiveLocationBlip.ShowRoute = true;
        GTA.UI.Screen.ShowSubtitle("Go to the ~y~meeting~w~.", 8000);
        currentObjective = Objectives.GoToChase;
        MissionWorld.script.Tick += MissionTick;
        return true;
    }

    void SpawnChaseHelicopter()
    {
        var position = Game.Player.Character.Position;
        var heli = World.CreateVehicle(new Model(VehicleHash.Buzzard), new Vector3(position.X, position.Y, position.Z + 30));
        while (!MissionWorld.IsEntityLoaded(heli))
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
                    heli = World.CreateVehicle(new Model(VehicleHash.Buzzard), new Vector3(position.X, position.Y, position.Z + 30));
                    loadingTimerStarted = false;
                }
            }
        }
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
        while (!MissionWorld.IsPedListLoaded(peds))
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
                    foreach (Ped ped in peds)
                    {
                        if (ped != null)
                        {
                            ped.Delete();
                        }
                    }
                    peds.Clear();
                    peds.Add(heli.CreatePedOnSeat(VehicleSeat.Driver, new Model(PedHash.MPros01)));
                    peds.Add(heli.CreatePedOnSeat(VehicleSeat.LeftRear, new Model(PedHash.MPros01)));
                    peds.Add(heli.CreatePedOnSeat(VehicleSeat.RightRear, new Model(PedHash.MPros01)));
                    loadingTimerStarted = false;
                }
            }
        }
        foreach (Ped ped in peds)
        {
            enemies.Add(new MissionPed(ped, enemiesRelGroup));
        }
        peds[0].Task.ChaseWithHelicopter(Game.Player.Character, new Vector3(4, 4, -10));
        peds[1].Task.VehicleShootAtPed(Game.Player.Character);
        peds[2].Task.VehicleShootAtPed(Game.Player.Character);
    }
}
