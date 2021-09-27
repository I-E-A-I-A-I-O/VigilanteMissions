using GTA;
using GTA.Native;
using GTA.Math;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

class MissionNine : Mission
{
    enum Objectives
    {
        GoToMotelRoom,
        EnterMotelRoom,
        InterrogateJesse,
        KillJesse,
        ExitMotelRoom,
        GoToLab,
        KillEntranceGuards,
        EnterLab,
        KillLabEnemies,
        PlantBombs,
        LeaveLab,
        KillTarget,
        DestroyLab,
        Complete,
        None
    }

    enum MotelRoomPeds
    {
        Jesse,
        JesseGF
    }

    enum Vehicles
    {
        Car,
        Bike01,
        Bike02
    }

    Script script;
    List<MissionPed> enemies = new List<MissionPed>();
    List<MissionPed> neutralPeds = new List<MissionPed>();
    List<Vehicle> vehicles = new List<Vehicle>();
    List<Prop> props = new List<Prop>();
    Vector3 objectiveLocation;
    Objectives currentObjective;
    Music music;
    Blip objectiveLocationBlip;
    Blip bombOneBlip;
    Blip bombTwoBlip;
    Blip bombThreeBlip;
    MissionWorld missionWorld;
    MostWantedMissions mostWantedMissions;
    RelationshipGroup neutralRelGroup;
    RelationshipGroup aggressiveRelGroup;
    RelationshipGroup pedRelGroup;
    bool bombOnePlanted = false;
    bool bombTwoPlanted = false;
    bool bombThreePlanted = false;
    bool reinforcementSpawned = false;

    public MissionNine(Script script, MissionWorld missionWorld, RelationshipGroup neutralRelGroup, RelationshipGroup aggressiveRelGroup, RelationshipGroup pedRelGroup)
    {
        this.script = script;
        this.missionWorld = missionWorld;
        this.neutralRelGroup = neutralRelGroup;
        this.aggressiveRelGroup = aggressiveRelGroup;
        this.pedRelGroup = pedRelGroup;

        music = new Music();
        mostWantedMissions = new MostWantedMissions();
        objectiveLocation = mostWantedMissions.MISSION_NINE_JESSE_LOCATION;
    }

    public override void MissionTick(object o, EventArgs e)
    {
        if (Game.Player.WantedLevel >= 2)
        {
            Game.Player.WantedLevel = 1;
        }
        switch (currentObjective)
        {
            case Objectives.GoToMotelRoom:
                {
                    if (!Game.Player.Character.IsInRange(objectiveLocation, 15))
                    {
                        return;
                    }
                    objectiveLocationBlip.Delete();
                    objectiveLocation = mostWantedMissions.MISSION_NINE_JESSE_WALK_TO;
                    var peds = mostWantedMissions.InitializeMissionNineMotelRoomPeds();
                    Script.Wait(500);
                    foreach (Ped ped in peds)
                    {
                        neutralPeds.Add(new MissionPed(ped, pedRelGroup, objectiveLocation, script, true));
                    }
                    neutralPeds[(int)MotelRoomPeds.Jesse].ped.Task.StartScenario("WORLD_HUMAN_DRUG_DEALER_HARD", 0);
                    neutralPeds[(int)MotelRoomPeds.JesseGF].ped.Task.StartScenario("WORLD_HUMAN_YOGA", 0);

                    GTA.UI.Screen.ShowSubtitle("Enter the motel room.", 8000);

                    currentObjective = Objectives.EnterMotelRoom;
                    break;
                }
            case Objectives.EnterMotelRoom:
                {
                    if (!Game.Player.Character.IsInRange(objectiveLocation, 20))
                    {
                        return;
                    }
                    var taskSequence = new TaskSequence();
                    taskSequence.AddTask.LookAt(Game.Player.Character);
                    taskSequence.AddTask.HandsUp(1800000);
                    neutralPeds[(int)MotelRoomPeds.Jesse].ped.Task.PerformSequence(taskSequence);
                    neutralPeds[(int)MotelRoomPeds.JesseGF].ped.Task.Cower(1800000);

                    GTA.UI.Screen.ShowSubtitle("~b~Interrogate ~r~Jesse Pinkman", 8000);

                    currentObjective = Objectives.InterrogateJesse;
                    break;
                }
            case Objectives.InterrogateJesse:
                {
                    if (neutralPeds[(int)MotelRoomPeds.Jesse].IsDead())
                    {
                        GTA.UI.Screen.ShowSubtitle("~r~Mission failed, Jesse Pinkman is dead.", 8000);
                        missionWorld.QuitMission();
                        return;
                    }
                    if (Game.Player.Character.IsInRange(neutralPeds[(int)MotelRoomPeds.Jesse].ped.Position, 2))
                    {
                        if (Game.LastInputMethod == InputMethod.GamePad)
                        {
                            GTA.UI.Screen.ShowHelpTextThisFrame("Press DPad Right to ~b~interrogate ~r~Jesse");
                        }
                        else
                        {
                            GTA.UI.Screen.ShowHelpTextThisFrame($"Press {VigilanteMissions.interactKey} to ~b~interrogate ~r~Jesse");
                        }
                        if ((Game.LastInputMethod == InputMethod.MouseAndKeyboard && Game.IsKeyPressed(VigilanteMissions.interactMissionKey)) || (Game.LastInputMethod == InputMethod.GamePad && Game.IsControlJustReleased(GTA.Control.ScriptPadRight)))
                        {
                            neutralPeds[(int)MotelRoomPeds.Jesse].ped.PlayAmbientSpeech("GENERIC_FRIGHTENED_HIGH", SpeechModifier.ShoutedClear);
                            neutralPeds[(int)MotelRoomPeds.Jesse].ped.Task.ClearAllImmediately();
                            neutralPeds[(int)MotelRoomPeds.Jesse].ped.Task.PlayAnimation("busted", "idle_2_hands_up", 8f, -1, AnimationFlags.StayInEndFrame);
                            neutralPeds[(int)MotelRoomPeds.Jesse].ped.AddBlip();
                            neutralPeds[(int)MotelRoomPeds.Jesse].ped.AttachedBlip.Scale = 0.8f;
                            neutralPeds[(int)MotelRoomPeds.Jesse].ped.AttachedBlip.Color = BlipColor.Red;
                            neutralPeds[(int)MotelRoomPeds.Jesse].ped.AttachedBlip.Name = "Jesse Pinkman";
                            neutralPeds[(int)MotelRoomPeds.Jesse].ped.AttachedBlip.IsFlashing = true;

                            GTA.UI.Screen.ShowSubtitle("Kill ~r~Jesse Pinkman~w~.", 8000);
                            currentObjective = Objectives.KillJesse;
                        }
                    }
                    break;
                }
            case Objectives.KillJesse:
                {
                    if (neutralPeds[(int)MotelRoomPeds.Jesse].IsDead())
                    {
                        neutralPeds[(int)MotelRoomPeds.Jesse].ped.AttachedBlip.Delete();
                        RemoveVehiclesAndNeutrals();
                        GTA.UI.Screen.ShowSubtitle("Leave the motel room.", 8000);
                        objectiveLocation = mostWantedMissions.MISSION_NINE_JESSE_LOCATION;
                        currentObjective = Objectives.ExitMotelRoom;
                    }
                    break;
                }
            case Objectives.ExitMotelRoom:
                {
                    if (Game.Player.Character.IsInRange(objectiveLocation, 20))
                    {
                        GTA.UI.Screen.ShowSubtitle("Go to the ~y~meth lab~w~.", 8000);
                        objectiveLocation = mostWantedMissions.MISSION_NINE_LAB_LOCATION;
                        objectiveLocationBlip = World.CreateBlip(objectiveLocation);
                        objectiveLocationBlip.Color = BlipColor.Yellow;
                        objectiveLocationBlip.Name = "Meth lab";
                        objectiveLocationBlip.ShowRoute = true;
                        currentObjective = Objectives.GoToLab;
                    }
                    break;
                }
            case Objectives.GoToLab:
                {
                    if (!Game.Player.Character.IsInRange(objectiveLocation, 50))
                    {
                        return;
                    }
                    music.IncreaseIntensity();
                    objectiveLocationBlip.Delete();
                    var peds = mostWantedMissions.InitializeMissionNineLabEntranceGuards();
                    Script.Wait(500);
                    for (var i = 0; i < peds.Count; i++)
                    {
                        enemies.Add(new MissionPed(peds[i], neutralRelGroup, objectiveLocation, script));
                        enemies[i].ShowBlip();
                        enemies[i].GiveRandomScenario();
                    }
                    GTA.UI.Screen.ShowSubtitle("Kill the ~r~guards~w~.", 8000);
                    currentObjective = Objectives.KillEntranceGuards;
                    break;
                }
            case Objectives.KillEntranceGuards:
                {
                    if (enemies.Count > 0)
                    {
                        RemoveDeadEnemies();
                    } else
                    {
                        objectiveLocation = mostWantedMissions.MISSION_NINE_LAB_INSIDE_LOCATION;
                        props = mostWantedMissions.InitializeMissionNineLabProps();
                        var peds = mostWantedMissions.InitializeMissionNineLabPeds();
                        Script.Wait(1000);
                        foreach(Ped ped in peds)
                        {
                            enemies.Add(new MissionPed(ped, aggressiveRelGroup, objectiveLocation, script));
                        }
                        GTA.UI.Screen.ShowSubtitle("Enter the meth lab.", 8000);
                        currentObjective = Objectives.EnterLab;
                    }
                    break;
                }
            case Objectives.EnterLab:
                {
                    if (Game.Player.Character.IsInRange(objectiveLocation, 20))
                    {
                        foreach(MissionPed enemy in enemies)
                        {
                            enemy.ShowBlip();
                        }
                        GTA.UI.Screen.ShowSubtitle("Kill the ~r~targets~w~.", 8000);
                        currentObjective = Objectives.KillLabEnemies;
                    }
                    break;
                }
            case Objectives.KillLabEnemies:
                {
                    if (enemies.Count > 0)
                    {
                        RemoveDeadEnemies();
                    } else
                    {
                        bombOneBlip = World.CreateBlip(mostWantedMissions.MISSION_NINE_LAB_BOMB_ONE_POSITION);
                        bombOneBlip.Color = BlipColor.Yellow;
                        bombOneBlip.Name = "Plant bomb";
                        bombTwoBlip = World.CreateBlip(mostWantedMissions.MISSION_NINE_LAB_BOMB_TWO_POSITION);
                        bombTwoBlip.Color = BlipColor.Yellow;
                        bombTwoBlip.Name = "Plant bomb";
                        bombThreeBlip = World.CreateBlip(mostWantedMissions.MISSION_NINE_LAB_BOMB_THREE_POSITION);
                        bombThreeBlip.Color = BlipColor.Yellow;
                        bombThreeBlip.Name = "Plant bomb";

                        GTA.UI.Screen.ShowSubtitle("Plant the ~y~bombs~w~.", 8000);

                        currentObjective = Objectives.PlantBombs;
                    }
                    break;
                }
            case Objectives.PlantBombs:
                {
                    if (!bombOnePlanted)
                    {
                        if (Game.Player.Character.IsInRange(mostWantedMissions.MISSION_NINE_LAB_BOMB_ONE_POSITION, 1.1f))
                        {
                            if (Game.LastInputMethod == InputMethod.GamePad)
                            {
                                GTA.UI.Screen.ShowHelpTextThisFrame("Press DPad Right to plant the ~y~bomb");
                            }
                            else
                            {
                                GTA.UI.Screen.ShowHelpTextThisFrame($"Press {VigilanteMissions.interactKey} to plant the ~y~bomb");
                            }
                            if ((Game.LastInputMethod == InputMethod.MouseAndKeyboard && Game.IsKeyPressed(VigilanteMissions.interactMissionKey)) || (Game.LastInputMethod == InputMethod.GamePad && Game.IsControlJustReleased(GTA.Control.ScriptPadRight)))
                            {
                                PlayPlantBombAnim(Vector3.RelativeBack.ToHeading());
                                Script.Wait(2000);
                                props.Add(mostWantedMissions.InitializeMissionNineBombOne());
                                bombOneBlip.Delete();
                                bombOnePlanted = true;
                                vehicles = mostWantedMissions.InitializeMissionNineVehicles();
                            }
                        }
                    }
                    if (!bombTwoPlanted)
                    {
                        if (Game.Player.Character.IsInRange(mostWantedMissions.MISSION_NINE_LAB_BOMB_TWO_POSITION, 1.1f))
                        {
                            if (Game.LastInputMethod == InputMethod.GamePad)
                            {
                                GTA.UI.Screen.ShowHelpTextThisFrame("Press DPad Right to plant the ~y~bomb");
                            }
                            else
                            {
                                GTA.UI.Screen.ShowHelpTextThisFrame($"Press {VigilanteMissions.interactKey} to plant the ~y~bomb");
                            }
                            if ((Game.LastInputMethod == InputMethod.MouseAndKeyboard && Game.IsKeyPressed(VigilanteMissions.interactMissionKey)) || (Game.LastInputMethod == InputMethod.GamePad && Game.IsControlJustReleased(GTA.Control.ScriptPadRight)))
                            {
                                PlayPlantBombAnim(Vector3.RelativeBack.ToHeading());
                                Script.Wait(2000);
                                props.Add(mostWantedMissions.InitializeMissionNineBombTwo());
                                bombTwoBlip.Delete();
                                bombTwoPlanted = true;
                            }
                        }
                    }
                    if (!bombThreePlanted)
                    {
                        if (Game.Player.Character.IsInRange(mostWantedMissions.MISSION_NINE_LAB_BOMB_THREE_POSITION, 1.1f))
                        {
                            if (Game.LastInputMethod == InputMethod.GamePad)
                            {
                                GTA.UI.Screen.ShowHelpTextThisFrame("Press DPad Right to plant the ~y~bomb");
                            }
                            else
                            {
                                GTA.UI.Screen.ShowHelpTextThisFrame($"Press {VigilanteMissions.interactKey} to plant the ~y~bomb");
                            }
                            if ((Game.LastInputMethod == InputMethod.MouseAndKeyboard && Game.IsKeyPressed(VigilanteMissions.interactMissionKey)) || (Game.LastInputMethod == InputMethod.GamePad && Game.IsControlJustReleased(GTA.Control.ScriptPadRight)))
                            {
                                PlayPlantBombAnim(Vector3.RelativeFront.ToHeading());
                                Script.Wait(2000);
                                props.Add(mostWantedMissions.InitializeMissionNineBombThree());
                                bombThreeBlip.Delete();
                                bombThreePlanted = true;
                            }
                        }
                    }
                    if (bombOnePlanted && bombTwoPlanted && bombThreePlanted)
                    {
                        GTA.UI.Screen.ShowSubtitle("Leave the meth lab.", 8000);
                        objectiveLocation = mostWantedMissions.MISSION_NINE_LAB_LOCATION;
                        currentObjective = Objectives.LeaveLab;
                    }
                    break;
                }
            case Objectives.LeaveLab:
                {
                    if (Game.Player.Character.IsInRange(objectiveLocation, 20))
                    {
                        objectiveLocation = mostWantedMissions.MISSION_NINE_REINFORCEMENT_LOCATION;
                        var peds = new List<Ped>
                        {
                            vehicles[(int)Vehicles.Car].CreatePedOnSeat(VehicleSeat.Driver, new Model(PedHash.Chef)),
                            vehicles[(int)Vehicles.Bike01].CreatePedOnSeat(VehicleSeat.Driver, new Model(PedHash.PoloGoon01GMY)),
                            vehicles[(int)Vehicles.Bike02].CreatePedOnSeat(VehicleSeat.Driver, new Model(PedHash.PoloGoon01GMY)),
                            vehicles[(int)Vehicles.Bike01].CreatePedOnSeat(VehicleSeat.Passenger, new Model(PedHash.PoloGoon01GMY)),
                            vehicles[(int)Vehicles.Bike02].CreatePedOnSeat(VehicleSeat.Passenger, new Model(PedHash.PoloGoon01GMY)),
                        };
                        Script.Wait(1000);
                        for (var i = 0; i < peds.Count; i++)
                        {
                            enemies.Add(new MissionPed(peds[i], neutralRelGroup, objectiveLocation, script, false, true));
                            enemies[i].ShowBlip();
                        }
                        enemies[(int)Vehicles.Car].ped.BlockPermanentEvents = true;
                        Function.Call(Hash.TASK_VEHICLE_MISSION_COORS_TARGET, enemies[(int)Vehicles.Car].ped.Handle, vehicles[(int)Vehicles.Car].Handle, objectiveLocation.X, objectiveLocation.Y, objectiveLocation.Z, 4, 300f, (int)DrivingStyle.Rushed, 5f, 5f, 1);
                        Function.Call(Hash.TASK_VEHICLE_MISSION_COORS_TARGET, enemies[(int)Vehicles.Bike01].ped.Handle, vehicles[(int)Vehicles.Bike01].Handle, objectiveLocation.X, objectiveLocation.Y, objectiveLocation.Z, 4, 225f, (int)DrivingStyle.Rushed, 5f, 5f, 1);
                        Function.Call(Hash.TASK_VEHICLE_MISSION_COORS_TARGET, enemies[(int)Vehicles.Bike02].ped.Handle, vehicles[(int)Vehicles.Bike02].Handle, objectiveLocation.X, objectiveLocation.Y, objectiveLocation.Z, 4, 225f, (int)DrivingStyle.Rushed, 5f, 5f, 1);
                        GTA.UI.Screen.ShowSubtitle("Kill ~r~Heisenberg~w~.", 8000);
                        currentObjective = Objectives.KillTarget;
                    }
                    break;
                }
            case Objectives.KillTarget:
                {
                    RemoveDeadEnemies();
                    if (enemies.Count > 0)
                    {
                        if (!reinforcementSpawned)
                        {
                            if (enemies[0].ped.IsInRange(objectiveLocation, 110))
                            {
                                var peds = mostWantedMissions.InitializeMissionNineReinforcements();
                                Script.Wait(1000);
                                for (var i = 0; i < peds.Count; i++)
                                {
                                    enemies.Add(new MissionPed(peds[i], aggressiveRelGroup, objectiveLocation, script));
                                    if (enemies[i].ped.AttachedBlip == null)
                                    {
                                        enemies[i].ShowBlip();
                                    }
                                }
                                reinforcementSpawned = true;
                                enemies[0].ped.BlockPermanentEvents = false;
                                var sequence = new TaskSequence();
                                sequence.AddTask.LeaveVehicle();
                                sequence.AddTask.FleeFrom(Game.Player.Character);
                                enemies[0].ped.Task.PerformSequence(sequence);
                                GTA.UI.Screen.ShowSubtitle("~r~Heisenberg~w~ arrived at his ~r~reinforcements~w~ arrived. Kill everyone.");
                            }
                        }
                    } else
                    {
                        GTA.UI.Screen.ShowSubtitle("Blow up the ~y~meth lab~w~.", 8000);
                        currentObjective = Objectives.DestroyLab;
                    }
                    break;
                }
            case Objectives.DestroyLab:
                {
                    if (Game.LastInputMethod == InputMethod.GamePad)
                    {
                        GTA.UI.Screen.ShowHelpTextThisFrame("Press DPad Right to blow up the ~y~meth lab");
                    }
                    else
                    {
                        GTA.UI.Screen.ShowHelpTextThisFrame($"Press {VigilanteMissions.interactKey} to blow up the ~y~meth lab");
                    }
                    if ((Game.LastInputMethod == InputMethod.MouseAndKeyboard && Game.IsKeyPressed(VigilanteMissions.interactMissionKey)) || (Game.LastInputMethod == InputMethod.GamePad && Game.IsControlJustReleased(GTA.Control.ScriptPadRight)))
                    {
                        World.AddExplosion(mostWantedMissions.MISSION_NINE_LAB_LOCATION, ExplosionType.Tanker, 50, 15);
                        World.AddExplosion(mostWantedMissions.MISSION_NINE_LAB_LOCATION, ExplosionType.Tanker, 50, 15);
                        World.AddExplosion(mostWantedMissions.MISSION_NINE_LAB_LOCATION, ExplosionType.Tanker, 50, 15);
                        currentObjective = Objectives.Complete;
                    }
                    break;
                }
            case Objectives.Complete:
                {
                    RemoveVehiclesAndNeutrals();
                    GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Wanted Suspect", "Good job, your cut of the reward is already in your account.");
                    currentObjective = Objectives.None;
                    Game.Player.Money += 15000;
                    missionWorld.CompleteMission();
                    script.Tick -= MissionTick;
                    break;
                }
        }
    }

    public override void QuitMission()
    {
        music.StopMusic();
        currentObjective = Objectives.None;
        script.Tick -= MissionTick;
        if (bombOneBlip != null)
        {
            bombOneBlip.Delete();
        }
        if (bombTwoBlip != null)
        {
            bombTwoBlip.Delete();
        }
        if (bombThreeBlip != null)
        {
            bombThreeBlip.Delete();
        }
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
        foreach (MissionPed ped in neutralPeds)
        {
            ped.Delete();
        }
        foreach(Prop prop in props)
        {
            prop.MarkAsNoLongerNeeded();
        }
    }

    public override bool StartMission()
    {
        if (Game.Player.Character.IsInRange(objectiveLocation, 200))
        {
            GTA.UI.Notification.Show("Mission not available.");
            return false;
        }

        music.StartCountry();

        GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Wanted suspect", "I couldn't find this ~r~Heisenberg~w~ guy, but i know where his associate ~r~Jesse Pinkman~w~ is. Head to ~y~The motor motel~w~.");

        objectiveLocationBlip = World.CreateBlip(objectiveLocation);
        objectiveLocationBlip.Scale = 0.9f;
        objectiveLocationBlip.Color = BlipColor.Yellow;
        objectiveLocationBlip.Name = "Jesse Pinkman's location";
        objectiveLocationBlip.ShowRoute = true;

        GTA.UI.Screen.ShowSubtitle("Go to ~y~The Motor motel~w~.", 8000);

        currentObjective = Objectives.GoToMotelRoom;

        script.Tick += MissionTick;

        return true;
    }

    void PlayPlantBombAnim(float heading)
    {
        var sequence = new TaskSequence();
        sequence.AddTask.AchieveHeading(heading);
        sequence.AddTask.PlayAnimation("weapons@first_person@aim_rng@generic@projectile@thermal_charge@", "plant_vertical", 8, -1, AnimationFlags.UpperBodyOnly);
        Game.Player.Character.Task.PerformSequence(sequence);
    }
}
