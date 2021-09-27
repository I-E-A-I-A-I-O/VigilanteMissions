using GTA;
using GTA.Native;
using GTA.Math;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

class MissionSeven : Mission
{
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
    Script script;
    MissionWorld missionWorld;
    RelationshipGroup enemiesRelGroup;
    RelationshipGroup neutralsRelGroup;
    List<MissionPed> enemies = new List<MissionPed>();
    List<MissionPed> neutralPeds = new List<MissionPed>();
    List<Vehicle> vehicles = new List<Vehicle>();
    List<Prop> props = new List<Prop>();
    Vector3 objectiveLocation;
    Vector3 markerPosition;
    MostWantedMissions mostWantedMissions;
    Blip objectiveLocationBlip;
    Music music;
    bool timerStarted = false;
    bool countdownMusicStarted = false;
    int startTime;
    int currentTime;

    public MissionSeven(Script script, MissionWorld missionWorld, RelationshipGroup enemiesRelGroup, RelationshipGroup neutralsRelGroup)
    {
        this.script = script;
        this.missionWorld = missionWorld;
        this.enemiesRelGroup = enemiesRelGroup;

        mostWantedMissions = new MostWantedMissions();
        objectiveLocation = mostWantedMissions.MISSION_SEVEN_START_LOCATION;
        music = new Music();
        markerPosition = new Vector3(-77.04752f, -830.2404f, 242.3859f);

        var copRelGroupHash = Function.Call<int>(Hash.GET_HASH_KEY, "COP");
        neutralsRelGroup = new RelationshipGroup(copRelGroupHash);
    }

    public override void MissionTick(object o, EventArgs e)
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

                    objectiveLocationBlip.Delete();
                    music.IncreaseIntensity();

                    var peds = mostWantedMissions.InitializeMissionSevenStreetPeds();
                    vehicles = mostWantedMissions.InitializeMissionSevenStreetVehicles();
                    var neutrals = mostWantedMissions.InitializeMissionSevenPolice();
                    Script.Wait(1000);
                    foreach(Ped ped in peds)
                    {
                        enemies.Add(new MissionPed(ped, enemiesRelGroup, objectiveLocation, script));
                    }

                    for(var i = 0; i < neutrals.Count; i++)
                    {
                        neutralPeds.Add(new MissionPed(neutrals[i], neutralsRelGroup, objectiveLocation, script));
                        neutralPeds[i].ped.Accuracy = 10;
                        Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, neutralPeds[i].ped, 1);
                    }

                    foreach(Vehicle vehicle in vehicles)
                    {
                        vehicle.IsSirenActive = true;
                    }

                    foreach(MissionPed enemy in enemies)
                    {
                        enemy.ShowBlip();
                        enemy.ped.Accuracy = 80;
                        Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, enemy.ped, 1);
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
                        objectiveLocation = mostWantedMissions.MISSION_SEVEN_OFFICE_LOCATION;
                        objectiveLocationBlip = World.CreateBlip(objectiveLocation);
                        objectiveLocationBlip.Color = BlipColor.Yellow;
                        objectiveLocationBlip.Name = "Office";
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
                    objectiveLocationBlip.Delete();

                    var peds = mostWantedMissions.InitializeMissionSevenOfficePeds();
                    Script.Wait(1000);
                    foreach (Ped ped in peds)
                    {
                        enemies.Add(new MissionPed(ped, enemiesRelGroup, objectiveLocation, script));
                    }

                    foreach(MissionPed enemy in enemies)
                    {
                        enemy.ShowBlip();
                        enemy.ped.Accuracy = 80;
                        Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, enemy.ped, 3);
                    }

                    StartOfficeScenarios();

                    GTA.UI.Screen.ShowSubtitle("Kill the ~r~terrorist~w~.", 8000);
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
                        props = mostWantedMissions.InitializeMissionSevenBomb();

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
                        if (Game.LastInputMethod == InputMethod.GamePad)
                        {
                            GTA.UI.Screen.ShowHelpTextThisFrame("Press DPad Right to grab the ~g~bomb");
                        } else
                        {
                            GTA.UI.Screen.ShowHelpTextThisFrame($"Press {VigilanteMissions.interactKey} to grab the ~g~bomb");
                        }
                        if ((Game.LastInputMethod == InputMethod.MouseAndKeyboard && Game.IsKeyPressed(VigilanteMissions.interactMissionKey)) || (Game.LastInputMethod == InputMethod.GamePad && Game.IsControlJustReleased(GTA.Control.ScriptPadRight))) 
                        {
                            props[0].Delete();
                            props.Clear();
                            objectiveLocation = mostWantedMissions.MISSION_SEVEN_ROOF_LOCATION;
                            objectiveLocationBlip =  World.CreateBlip(objectiveLocation);
                            objectiveLocationBlip.Color = BlipColor.Yellow;
                            objectiveLocationBlip.Name = "Roof";
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
                            if (Game.LastInputMethod == InputMethod.GamePad)
                            {
                                GTA.UI.Screen.ShowHelpTextThisFrame("Press RB to go to the ~y~roof");
                            }
                            else
                            {
                                GTA.UI.Screen.ShowHelpTextThisFrame("Press E to go to the ~g~roof");
                            }
                            if ((Game.LastInputMethod == InputMethod.MouseAndKeyboard && Game.IsKeyPressed(Keys.E)) || (Game.LastInputMethod == InputMethod.GamePad && Game.IsControlJustReleased(GTA.Control.ScriptRB)))
                            {
                                Game.Player.Character.Position = objectiveLocation;
                            }
                        }
                        return;
                    }
                    objectiveLocationBlip.Delete();

                    vehicles.Add(mostWantedMissions.InitializeMissionSevenRoofVehicles()[0]);
                    var peds = mostWantedMissions.InitializeMissionSevenRoofPeds();
                    Script.Wait(1000);
                    foreach(Ped ped in peds)
                    {
                        enemies.Add(new MissionPed(ped, enemiesRelGroup, objectiveLocation, script));
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
                        objectiveLocation = mostWantedMissions.MISSION_SEVEN_EXPLOSION_LOCATION;
                        objectiveLocationBlip = World.CreateBlip(objectiveLocation, 40);
                        objectiveLocationBlip.Color = BlipColor.Yellow;
                        objectiveLocationBlip.Name = "Safe bomb explosion location";
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
                            Function.Call(Hash.TRIGGER_MUSIC_EVENT, "MP_DM_COUNTDOWN_30_SEC");
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
                            objectiveLocationBlip.Delete();
                            World.AddExplosion(Game.Player.Character.Position, ExplosionType.StickyBomb, 100, 15);
                            if (Game.Player.IsAlive)
                            {
                                Game.Player.Character.Kill();
                            }
                            currentObjective = Objectives.None;
                        }
                        return;
                    }
                    objectiveLocationBlip.Delete();

                    World.AddExplosion(objectiveLocation, ExplosionType.StickyBomb, 10, 15);
                    World.AddExplosion(objectiveLocation, ExplosionType.StickyBomb, 10, 15);
                    World.AddExplosion(objectiveLocation, ExplosionType.StickyBomb, 10, 15);
                    World.AddExplosion(objectiveLocation, ExplosionType.StickyBomb, 10, 15);

                    currentObjective = Objectives.Completed;
                    break;
                }
            case Objectives.Completed:
                {
                    missionWorld.CompleteMission();
                    RemoveVehiclesAndNeutrals();
                    currentObjective = Objectives.None;
                    Game.Player.Money += 20000;
                    GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Wanted Suspect", "Good job, you saved the day. As always, your cut of the reward is in your account.");
                    script.Tick -= MissionTick;
                    break;
                }
        }
    }

    public override void QuitMission()
    {
        music.StopMusic();
        currentObjective = Objectives.None;
        objectiveLocationBlip.Delete();
        foreach (MissionPed enemy in enemies)
        {
            enemy.Delete();
        }
        foreach (Prop prop in props)
        {
            prop.Delete();
        }
        RemoveVehiclesAndNeutrals();
        script.Tick -= MissionTick;
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
        music.StartCityMusic();
        GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Wanted Suspect", "They are at the Maze Bank Tower right now trying to blow up the place, hurry up!");
        GTA.UI.Screen.ShowSubtitle("Go to the ~y~Maze Bank Tower~w~.", 8000);
        currentObjective = Objectives.GoToBuilding;
        objectiveLocationBlip = World.CreateBlip(objectiveLocation, 150f);
        objectiveLocationBlip.Color = BlipColor.Yellow;
        objectiveLocationBlip.Name = "Wanted suspect location";
        objectiveLocationBlip.ShowRoute = true;

        script.Tick += MissionTick;
        return true;
    }

    void StartStreetScenarios()
    {
        neutralPeds[(int)Neutrals.Fbi04].ped.Task.StartScenario("WORLD_HUMAN_COP_IDLES", 0);
        neutralPeds[(int)Neutrals.Fbi05].ped.Task.StartScenario("WORLD_HUMAN_COP_IDLES", 0);
        neutralPeds[(int)Neutrals.Fbi06].ped.Task.StartScenario("WORLD_HUMAN_COP_IDLES", 0);
        neutralPeds[(int)Neutrals.Fib01].ped.Task.StartScenario("WORLD_HUMAN_BINOCULARS", 0);
        neutralPeds[(int)Neutrals.Fib02].ped.Task.StartScenario("WORLD_HUMAN_COP_IDLES", 0);
        neutralPeds[(int)Neutrals.Fib03].ped.Task.StartScenario("WORLD_HUMAN_COP_IDLES", 0);
        neutralPeds[(int)Neutrals.NooseClipboard].ped.Task.StartScenario("WORLD_HUMAN_CLIPBOARD", 0);
        neutralPeds[(int)Neutrals.Noose01].ped.Task.ChatTo(neutralPeds[(int)Neutrals.NooseClipboard].ped);
        neutralPeds[(int)Neutrals.Noose02].ped.Task.ChatTo(neutralPeds[(int)Neutrals.NooseClipboard].ped);
        neutralPeds[(int)Neutrals.Noose03].ped.Task.ChatTo(neutralPeds[(int)Neutrals.NooseClipboard].ped);
        neutralPeds[(int)Neutrals.Talking01].ped.Task.UseMobilePhone();
        neutralPeds[(int)Neutrals.Talking02].ped.Task.ChatTo(neutralPeds[(int)Neutrals.Talking01].ped);
        neutralPeds[(int)Neutrals.Talking03].ped.Task.ChatTo(neutralPeds[(int)Neutrals.Talking01].ped);

        enemies[(int)StreetEnemies.Entrance01].ped.Task.StartScenario("WORLD_HUMAN_STAND_IMPATIENT", 0);
        enemies[(int)StreetEnemies.Entrance02].ped.Task.StartScenario("WORLD_HUMAN_STAND_IMPATIENT_UPRIGHT", 0);
        enemies[(int)StreetEnemies.Entrance03].ped.Task.StartScenario("WORLD_HUMAN_STAND_IMPATIENT", 0);
        enemies[(int)StreetEnemies.Entrance04].ped.Task.StartScenario("WORLD_HUMAN_STAND_IMPATIENT", 0);
        enemies[(int)StreetEnemies.Entrance05].ped.Task.StartScenario("WORLD_HUMAN_STAND_IMPATIENT_UPRIGHT", 0);
    }

    void StartOfficeScenarios()
    {
        enemies[(int)OfficeEnemies.UsingComputerEntrance].ped.Task.FightAgainst(Game.Player.Character);
        enemies[(int)OfficeEnemies.WithComputerUser].ped.Task.FightAgainst(Game.Player.Character);
        enemies[(int)OfficeEnemies.Bathroom].ped.Task.FightAgainst(Game.Player.Character);
        enemies[(int)OfficeEnemies.Drinking01].ped.Task.FightAgainst(Game.Player.Character);
        enemies[(int)OfficeEnemies.Drinking02].ped.Task.FightAgainst(Game.Player.Character);
        enemies[(int)OfficeEnemies.LookingWindow].ped.Task.FightAgainst(Game.Player.Character);
        enemies[(int)OfficeEnemies.LookingMap01].ped.Task.FightAgainst(Game.Player.Character);
        enemies[(int)OfficeEnemies.LookingMap02].ped.Task.FightAgainst(Game.Player.Character);
        enemies[(int)OfficeEnemies.UsingComputerBomb].ped.Task.FightAgainst(Game.Player.Character);
        enemies[(int)OfficeEnemies.UsingLaptop].ped.Task.FightAgainst(Game.Player.Character);
        enemies[(int)OfficeEnemies.Talking01].ped.Task.FightAgainst(Game.Player.Character);
        enemies[(int)OfficeEnemies.Talking02].ped.Task.FightAgainst(Game.Player.Character);
    }

    void StartRoofScenarios()
    {
        enemies[(int)RoofEnemies.Helipad01].ped.Task.GuardCurrentPosition();
        enemies[(int)RoofEnemies.Helipad02].ped.Task.StartScenario("WORLD_HUMAN_GUARD_STAND", 0);
        enemies[(int)RoofEnemies.Pilot].ped.Task.EnterVehicle(vehicles[(int)Vehicles.Helicopter], VehicleSeat.Driver);
        vehicles[(int)Vehicles.Helicopter].IsEngineRunning = true;
        enemies[(int)RoofEnemies.Target].ped.Task.StandStill(1800000);
    }
}
