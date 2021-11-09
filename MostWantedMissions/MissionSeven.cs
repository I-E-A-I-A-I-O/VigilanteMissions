using GTA;
using GTA.Native;
using GTA.Math;
using System;
using System.Collections.Generic;

class MissionSeven : Mission
{
    public override bool IsMostWanted => true;
    public override bool IsJokerMission => false;

    enum Objectives
    {
        GoToBuilding,
        KillEntranceTargets,
        EnterOffice,
        KillOfficeTargets,
        GrabBomb,
        GoToRoof,
        KillRoofTargets,
        DisposeBomb,
        Completed,
        None
    }

    enum StreetEnemies
    {
        Entrance01,
        Entrance02,
        Entrance03,
        Entrance04,
        Entrance05
    }

    enum OfficeEnemies
    {
        WithComputerUser,
        UsingComputerEntrance,
        LookingWindow,
        LookingMap01,
        LookingMap02,
        UsingComputerBomb,
        Drinking01,
        Drinking02,
        Bathroom,
        UsingLaptop,
        Talking01,
        Talking02
    }

    enum RoofEnemies
    {
        Pilot,
        Helipad01,
        Helipad02,
        Target
    }

    enum Neutrals
    {
        Fib01,
        Talking01,
        Fib02,
        Noose01,
        Noose02,
        Noose03,
        NooseClipboard,
        Fib03,
        Talking02,
        Talking03,
        Fbi04,
        Fbi05,
        Fbi06
    }

    enum Vehicles
    {
        Vehicle01,
        Vehicle02,
        Vehicle03,
        Vehicle04,
        Helicopter
    }

    Objectives currentObjective;
    RelationshipGroup enemiesRelGroup;
    List<MissionPed> enemies = new List<MissionPed>();
    List<MissionPed> neutralPeds = new List<MissionPed>();
    List<Vehicle> vehicles = new List<Vehicle>();
    List<Prop> props = new List<Prop>();
    Vector3 objectiveLocation;
    Vector3 markerPosition;
    public override Blip ObjectiveLocationBlip { get; set; }
    bool timerStarted = false;
    bool countdownMusicStarted = false;
    int startTime;
    int currentTime;

    public MissionSeven()
    {
        enemiesRelGroup = MissionWorld.RELATIONSHIP_MISSION_AGGRESSIVE;

        objectiveLocation = MostWantedMissions.MISSION_SEVEN_START_LOCATION;
        markerPosition = new Vector3(-77.04752f, -830.2404f, 242.3859f);
    }

    protected override void MissionTick(object o, EventArgs e)
    {
        if (Game.Player.WantedLevel >= 2)
        {
            Game.Player.WantedLevel = 1;
        }
        switch(currentObjective)
        {
            case Objectives.GoToBuilding:
                {
                    if (!Game.Player.Character.IsInRange(objectiveLocation, 200))
                    {
                        return;
                    }

                    ObjectiveLocationBlip.Delete();
                    Music.IncreaseIntensity();

                    var peds = MostWantedMissions.InitializeMissionSevenStreetPeds();
                    vehicles = MostWantedMissions.InitializeMissionSevenStreetVehicles();
                    var neutrals = MostWantedMissions.InitializeMissionSevenPolice();

                    for(var i = 0; i < neutrals.Count; i++)
                    {
                        neutralPeds.Add(new MissionPed(neutrals[i], "COP"));
                        neutralPeds[i].GetPed().Accuracy = 10;
                        Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, neutralPeds[i].GetPed().Handle, 1);
                        Function.Call(Hash.SET_PED_AS_COP, neutralPeds[i].GetPed().Handle, true);
                    }

                    for (var i = 0; i < peds.Count; i++)
                    {
                        enemies.Add(new MissionPed(peds[i], enemiesRelGroup));
                        enemies[i].ShowBlip();
                        enemies[i].GetPed().Accuracy = 80;
                        Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, enemies[i].GetPed().Handle, 1);
                    }

                    foreach(Vehicle vehicle in vehicles)
                    {
                        vehicle.IsSirenActive = true;
                    }

                    StartStreetScenarios();

                    GTA.UI.Screen.ShowSubtitle("Kill the ~r~terrorists~w~.", 8000);
                    currentObjective = Objectives.KillEntranceTargets;
                    break;
                }
            case Objectives.KillEntranceTargets:
                {
                    if (enemies.Count > 0)
                    {
                        RemoveDeadEnemies();
                    }
                    else
                    {
                        objectiveLocation = MostWantedMissions.MISSION_SEVEN_OFFICE_LOCATION;
                        ObjectiveLocationBlip = World.CreateBlip(objectiveLocation);
                        ObjectiveLocationBlip.Color = BlipColor.Yellow;
                        ObjectiveLocationBlip.Name = "Office";
                        GTA.UI.Screen.ShowSubtitle("Enter the ~y~office~w~.", 8000);
                        currentObjective = Objectives.EnterOffice;
                    }
                    break;
                }
            case Objectives.EnterOffice:
                {
                    if (!Game.Player.Character.IsInRange(objectiveLocation, 20))
                    {
                        return;
                    }
                    ObjectiveLocationBlip.Delete();

                    Loading.LoadModel(MostWantedMissions.MissionSevenOfficeModel);
                    var peds = MostWantedMissions.InitializeMissionSevenOfficePeds();
                    Loading.LoadModel(MostWantedMissions.MissionSevenOfficeModel);

                    foreach (Ped ped in peds)
                    {
                        enemies.Add(new MissionPed(ped, enemiesRelGroup));
                    }

                    foreach(MissionPed enemy in enemies)
                    {
                        enemy.ShowBlip();
                        enemy.GetPed().Accuracy = 80;
                        Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, enemy.GetPed().Handle, 3);
                    }

                    StartOfficeScenarios();

                    GTA.UI.Screen.ShowSubtitle("Kill the ~r~terrorists~w~.", 8000);
                    currentObjective = Objectives.KillOfficeTargets;
                    break;
                }
            case Objectives.KillOfficeTargets:
                {
                    if (enemies.Count > 0)
                    {
                        RemoveDeadEnemies();
                    } else
                    {
                        Loading.LoadModel(MostWantedMissions.MissionSevenBombModel);
                        props = MostWantedMissions.InitializeMissionSevenBomb();
                        Loading.UnloadModel(MostWantedMissions.MissionSevenBombModel);

                        objectiveLocation = props[0].Position;
                        props[0].AddBlip();
                        props[0].AttachedBlip.Color = BlipColor.Green;
                        props[0].AttachedBlip.Name = "Bomb";

                        GTA.UI.Screen.ShowSubtitle("Grab the ~g~bomb~w~.", 8000);

                        currentObjective = Objectives.GrabBomb;
                    }
                    break;
                }
            case Objectives.GrabBomb:
                {
                    if (Game.Player.Character.IsInRange(objectiveLocation, 1))
                    {
                        GTA.UI.Screen.ShowHelpTextThisFrame($"Press ~{VigilanteMissions.InteractionControl}~ to grab the ~g~bomb");
                        if (Function.Call<bool>(Hash.IS_CONTROL_JUST_RELEASED, 0, VigilanteMissions.InteractionControl)) 
                        {
                            props[0].Delete();
                            props.Clear();
                            objectiveLocation = MostWantedMissions.MISSION_SEVEN_ROOF_LOCATION;
                            ObjectiveLocationBlip =  World.CreateBlip(objectiveLocation);
                            ObjectiveLocationBlip.Color = BlipColor.Yellow;
                            ObjectiveLocationBlip.Name = "Roof";
                            GTA.UI.Screen.ShowSubtitle("Go to the ~y~roof~w~.", 8000);
                            currentObjective = Objectives.GoToRoof;
                        }
                    }
                    break;
                }
            case Objectives.GoToRoof:
                {
                    if (!Game.Player.Character.IsInRange(objectiveLocation, 20))
                    {
                        Function.Call(Hash.DRAW_MARKER, 1, markerPosition.X, markerPosition.Y, markerPosition.Z, 0f, 0f, 0f, 0f, 0f, 0f, 0.75f, 0.75f, 0.75f, 255, 255, 0, 255, false, false, 2, false, false, false);
                        if (Game.Player.Character.IsInRange(markerPosition, 2))
                        {
                            GTA.UI.Screen.ShowHelpTextThisFrame($"Press ~{VigilanteMissions.InteractionControl}~ to go to the ~g~roof");
                            if (Function.Call<bool>(Hash.IS_CONTROL_JUST_RELEASED, 0, VigilanteMissions.InteractionControl))
                            {
                                Game.Player.Character.Position = objectiveLocation;
                            }
                        }
                        return;
                    }
                    ObjectiveLocationBlip.Delete();

                    Loading.LoadModels(MostWantedMissions.MissionSevenModels_2);
                    vehicles.Add(MostWantedMissions.InitializeMissionSevenRoofVehicles());
                    var peds = MostWantedMissions.InitializeMissionSevenRoofPeds();
                    Loading.UnloadModels(MostWantedMissions.MissionSevenModels_2);

                    foreach (Ped ped in peds)
                    {
                        enemies.Add(new MissionPed(ped, enemiesRelGroup));
                    }

                    foreach(MissionPed enemy in enemies)
                    {
                        enemy.ShowBlip();
                    }
                    GTA.UI.Screen.ShowSubtitle("Kill the ~r~terrorists~w~.", 8000);
                    StartRoofScenarios();
                    currentObjective = Objectives.KillRoofTargets;
                    break;
                }
            case Objectives.KillRoofTargets:
                {
                    if (enemies.Count > 0)
                    {
                        RemoveDeadEnemies();
                    } else
                    {
                        objectiveLocation = MostWantedMissions.MISSION_SEVEN_EXPLOSION_LOCATION;
                        ObjectiveLocationBlip = World.CreateBlip(objectiveLocation, 40);
                        ObjectiveLocationBlip.Color = BlipColor.Yellow;
                        ObjectiveLocationBlip.Name = "Safe bomb explosion location";
                        GTA.UI.Screen.ShowSubtitle("Get rid of the bomb, drop it in the ~y~sea~w~!", 8000);
                        currentObjective = Objectives.DisposeBomb;
                    }
                    break;
                }
            case Objectives.DisposeBomb:
                {
                    if (!Game.Player.Character.IsInRange(objectiveLocation, 40))
                    {
                        if (timerStarted && (65 - ((currentTime - startTime) / 1000)) <= 30 && !countdownMusicStarted)
                        {
                            Music.Play30SecCountDown();
                            countdownMusicStarted = true;
                        }
                        if (!timerStarted)
                        {
                            startTime = Game.GameTime;
                            currentTime = startTime;
                            timerStarted = true;
                        } else if (currentTime - startTime < 65000)
                        {
                            currentTime = Game.GameTime;
                            GTA.UI.Screen.ShowHelpTextThisFrame($"Remaining time: {65 - ((currentTime - startTime) / 1000)} seconds");
                        } else
                        {
                            ObjectiveLocationBlip.Delete();
                            World.AddExplosion(Game.Player.Character.Position, ExplosionType.StickyBomb, 100, 15);
                            if (Game.Player.IsAlive)
                            {
                                Game.Player.Character.Kill();
                            }
                            currentObjective = Objectives.None;
                        }
                        return;
                    }
                    ObjectiveLocationBlip.Delete();

                    World.AddExplosion(objectiveLocation, ExplosionType.StickyBomb, 10, 15);
                    World.AddExplosion(objectiveLocation, ExplosionType.StickyBomb, 10, 15);
                    World.AddExplosion(objectiveLocation, ExplosionType.StickyBomb, 10, 15);
                    World.AddExplosion(objectiveLocation, ExplosionType.StickyBomb, 10, 15);

                    currentObjective = Objectives.Completed;
                    break;
                }
            case Objectives.Completed:
                {
                    MissionWorld.CompleteMission();
                    RemoveVehiclesAndNeutrals();
                    currentObjective = Objectives.None;
                    Game.Player.Money += 20000;
                    GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Wanted Suspect", "Good job, you saved the day. As always, your cut of the reward is in your account.");
                    MissionWorld.script.Tick -= MissionTick;
                    break;
                }
        }
    }

    public override void QuitMission()
    {
        Music.StopMusic();
        currentObjective = Objectives.None;
        ObjectiveLocationBlip.Delete();
        foreach (MissionPed enemy in enemies)
        {
            enemy.Delete();
        }
        foreach (Prop prop in props)
        {
            prop.Delete();
        }
        RemoveVehiclesAndNeutrals();
        MissionWorld.script.Tick -= MissionTick;
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
        foreach (MissionPed ped in neutralPeds)
        {
            ped.Delete();
        }
    }

    public override bool StartMission()
    {
        if (Game.Player.Character.IsInRange(objectiveLocation, 200f))
        {
            GTA.UI.Notification.Show("Mission not available.");
            return false;
        }
        Music.StartCityMusic();
        GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Wanted Suspect", "They are at the Maze Bank Tower right now trying to blow up the place, hurry up!");
        GTA.UI.Screen.ShowSubtitle("Go to the ~y~Maze Bank Tower~w~.", 8000);
        currentObjective = Objectives.GoToBuilding;
        ObjectiveLocationBlip = World.CreateBlip(objectiveLocation);
        ObjectiveLocationBlip.DisplayType = BlipDisplayType.BothMapSelectable;
        ObjectiveLocationBlip.Color = BlipColor.Yellow;
        ObjectiveLocationBlip.Name = "Catherine Kerkow's location";
        ObjectiveLocationBlip.ShowRoute = true;

        MissionWorld.script.Tick += MissionTick;
        return true;
    }

    void StartStreetScenarios()
    {
        neutralPeds[(int)Neutrals.Fbi04].GetTask().StartScenario("WORLD_HUMAN_COP_IDLES", 0);
        neutralPeds[(int)Neutrals.Fbi05].GetTask().StartScenario("WORLD_HUMAN_COP_IDLES", 0);
        neutralPeds[(int)Neutrals.Fbi06].GetTask().StartScenario("WORLD_HUMAN_COP_IDLES", 0);
        neutralPeds[(int)Neutrals.Fib01].GetTask().StartScenario("WORLD_HUMAN_BINOCULARS", 0);
        neutralPeds[(int)Neutrals.Fib02].GetTask().StartScenario("WORLD_HUMAN_COP_IDLES", 0);
        neutralPeds[(int)Neutrals.Fib03].GetTask().StartScenario("WORLD_HUMAN_COP_IDLES", 0);
        neutralPeds[(int)Neutrals.NooseClipboard].GetTask().StartScenario("WORLD_HUMAN_CLIPBOARD", 0);
        neutralPeds[(int)Neutrals.Noose01].GetTask().ChatTo(neutralPeds[(int)Neutrals.NooseClipboard].GetPed());
        neutralPeds[(int)Neutrals.Noose02].GetTask().ChatTo(neutralPeds[(int)Neutrals.NooseClipboard].GetPed());
        neutralPeds[(int)Neutrals.Noose03].GetTask().ChatTo(neutralPeds[(int)Neutrals.NooseClipboard].GetPed());
        neutralPeds[(int)Neutrals.Talking01].GetTask().UseMobilePhone();
        neutralPeds[(int)Neutrals.Talking02].GetTask().ChatTo(neutralPeds[(int)Neutrals.Talking01].GetPed());
        neutralPeds[(int)Neutrals.Talking03].GetTask().ChatTo(neutralPeds[(int)Neutrals.Talking01].GetPed());

        enemies[(int)StreetEnemies.Entrance01].GetTask().StartScenario("WORLD_HUMAN_STAND_IMPATIENT", 0);
        enemies[(int)StreetEnemies.Entrance02].GetTask().StartScenario("WORLD_HUMAN_STAND_IMPATIENT_UPRIGHT", 0);
        enemies[(int)StreetEnemies.Entrance03].GetTask().StartScenario("WORLD_HUMAN_STAND_IMPATIENT", 0);
        enemies[(int)StreetEnemies.Entrance04].GetTask().StartScenario("WORLD_HUMAN_STAND_IMPATIENT", 0);
        enemies[(int)StreetEnemies.Entrance05].GetTask().StartScenario("WORLD_HUMAN_STAND_IMPATIENT_UPRIGHT", 0);
    }

    void StartOfficeScenarios()
    {
        enemies[(int)OfficeEnemies.UsingComputerEntrance].GetTask().FightAgainst(Game.Player.Character);
        enemies[(int)OfficeEnemies.WithComputerUser].GetTask().FightAgainst(Game.Player.Character);
        enemies[(int)OfficeEnemies.Bathroom].GetTask().FightAgainst(Game.Player.Character);
        enemies[(int)OfficeEnemies.Drinking01].GetTask().FightAgainst(Game.Player.Character);
        enemies[(int)OfficeEnemies.Drinking02].GetTask().FightAgainst(Game.Player.Character);
        enemies[(int)OfficeEnemies.LookingWindow].GetTask().FightAgainst(Game.Player.Character);
        enemies[(int)OfficeEnemies.LookingMap01].GetTask().FightAgainst(Game.Player.Character);
        enemies[(int)OfficeEnemies.LookingMap02].GetTask().FightAgainst(Game.Player.Character);
        enemies[(int)OfficeEnemies.UsingComputerBomb].GetTask().FightAgainst(Game.Player.Character);
        enemies[(int)OfficeEnemies.UsingLaptop].GetTask().FightAgainst(Game.Player.Character);
        enemies[(int)OfficeEnemies.Talking01].GetTask().FightAgainst(Game.Player.Character);
        enemies[(int)OfficeEnemies.Talking02].GetTask().FightAgainst(Game.Player.Character);
    }

    void StartRoofScenarios()
    {
        enemies[(int)RoofEnemies.Helipad01].GetTask().GuardCurrentPosition();
        enemies[(int)RoofEnemies.Helipad02].GetTask().StartScenario("WORLD_HUMAN_GUARD_STAND", 0);
        enemies[(int)RoofEnemies.Pilot].GetTask().EnterVehicle(vehicles[(int)Vehicles.Helicopter], VehicleSeat.Driver);
        vehicles[(int)Vehicles.Helicopter].IsEngineRunning = true;
        enemies[(int)RoofEnemies.Target].GetTask().StandStill(1800000);
    }
}
