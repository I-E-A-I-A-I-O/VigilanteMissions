using GTA;
using GTA.Native;
using GTA.Math;
using System;
using System.Collections.Generic;

class MissionNine : Mission
{
    public override bool IsMostWanted => true;
    public override bool IsJokerMission => false;

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

    List<MissionPed> enemies = new List<MissionPed>();
    List<MissionPed> neutralPeds = new List<MissionPed>();
    List<Vehicle> vehicles = new List<Vehicle>();
    List<Prop> props = new List<Prop>();
    Vector3 objectiveLocation;
    Objectives currentObjective;
    public override Blip ObjectiveLocationBlip { get; set; }
    Blip bombOneBlip;
    Blip bombTwoBlip;
    Blip bombThreeBlip;
    RelationshipGroup neutralRelGroup;
    RelationshipGroup aggressiveRelGroup;
    RelationshipGroup pedRelGroup;
    bool bombOnePlanted = false;
    bool bombTwoPlanted = false;
    bool bombThreePlanted = false;
    bool reinforcementSpawned = false;

    public MissionNine()
    {
        neutralRelGroup = MissionWorld.RELATIONSHIP_MISSION_NEUTRAL;
        aggressiveRelGroup = MissionWorld.RELATIONSHIP_MISSION_AGGRESSIVE;
        pedRelGroup = MissionWorld.RELATIONSHIP_MISSION_LIKE;

        objectiveLocation = MostWantedMissions.MISSION_NINE_JESSE_LOCATION;
    }

    protected override void MissionTick(object o, EventArgs e)
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
                    ObjectiveLocationBlip.Delete();
                    objectiveLocation = MostWantedMissions.MISSION_NINE_JESSE_WALK_TO;
                    Loading.LoadModels(MostWantedMissions.MissionNineModels_1);
                    var peds = MostWantedMissions.InitializeMissionNineMotelRoomPeds();
                    Loading.UnloadModels(MostWantedMissions.MissionNineModels_1);
                    foreach (Ped ped in peds)
                    {
                        neutralPeds.Add(new MissionPed(ped, pedRelGroup,  true));
                    }
                    neutralPeds[(int)MotelRoomPeds.Jesse].GetTask().StartScenario("WORLD_HUMAN_DRUG_DEALER_HARD", 0);
                    neutralPeds[(int)MotelRoomPeds.JesseGF].GetTask().StartScenario("WORLD_HUMAN_YOGA", 0);

                    GTA.UI.Screen.ShowSubtitle("Enter the ~b~motel room~w~.", 8000);

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
                    neutralPeds[(int)MotelRoomPeds.Jesse].GetTask().PerformSequence(taskSequence);
                    neutralPeds[(int)MotelRoomPeds.JesseGF].GetTask().Cower(1800000);

                    GTA.UI.Screen.ShowSubtitle("~b~Interrogate ~r~Jesse Pinkman", 8000);

                    currentObjective = Objectives.InterrogateJesse;
                    break;
                }
            case Objectives.InterrogateJesse:
                {
                    if (neutralPeds[(int)MotelRoomPeds.Jesse].IsDead())
                    {
                        GTA.UI.Screen.ShowSubtitle("~r~Mission failed, Jesse Pinkman is dead.", 8000);
                        MissionWorld.QuitMission();
                        return;
                    }
                    if (Game.Player.Character.IsInRange(neutralPeds[(int)MotelRoomPeds.Jesse].GetPosition(), 2))
                    {
                        GTA.UI.Screen.ShowHelpTextThisFrame($"Press ~{VigilanteMissions.InteractionControl}~ to ~b~interrogate ~r~Jesse");
                        if (Function.Call<bool>(Hash.IS_CONTROL_JUST_RELEASED, 0, VigilanteMissions.InteractionControl))
                        {
                            neutralPeds[(int)MotelRoomPeds.Jesse].GetPed().PlayAmbientSpeech("GENERIC_FRIGHTENED_HIGH", SpeechModifier.ShoutedClear);
                            neutralPeds[(int)MotelRoomPeds.Jesse].GetTask().ClearAllImmediately();
                            neutralPeds[(int)MotelRoomPeds.Jesse].GetTask().PlayAnimation("busted", "idle_2_hands_up", 8f, -1, AnimationFlags.StayInEndFrame);
                            neutralPeds[(int)MotelRoomPeds.Jesse].GetPed().AddBlip();
                            neutralPeds[(int)MotelRoomPeds.Jesse].GetBlip().Scale = 0.8f;
                            neutralPeds[(int)MotelRoomPeds.Jesse].GetBlip().Color = BlipColor.Red;
                            neutralPeds[(int)MotelRoomPeds.Jesse].GetBlip().Name = "Jesse Pinkman";
                            neutralPeds[(int)MotelRoomPeds.Jesse].GetBlip().IsFlashing = true;

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
                        neutralPeds[(int)MotelRoomPeds.Jesse].GetBlip().Delete();
                        RemoveVehiclesAndNeutrals();
                        GTA.UI.Screen.ShowSubtitle("Leave the motel room.", 8000);
                        objectiveLocation = MostWantedMissions.MISSION_NINE_JESSE_LOCATION;
                        currentObjective = Objectives.ExitMotelRoom;
                    }
                    break;
                }
            case Objectives.ExitMotelRoom:
                {
                    if (Game.Player.Character.IsInRange(objectiveLocation, 20))
                    {
                        GTA.UI.Screen.ShowSubtitle("Go to the ~y~meth lab~w~.", 8000);
                        objectiveLocation = MostWantedMissions.MISSION_NINE_LAB_LOCATION;
                        ObjectiveLocationBlip = World.CreateBlip(objectiveLocation);
                        ObjectiveLocationBlip.Color = BlipColor.Yellow;
                        ObjectiveLocationBlip.Name = "Meth lab";
                        ObjectiveLocationBlip.ShowRoute = true;
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
                    Music.IncreaseIntensity();
                    ObjectiveLocationBlip.Delete();
                    Loading.LoadModel(MostWantedMissions.MissionNineLabEntranceModel);
                    var peds = MostWantedMissions.InitializeMissionNineLabEntranceGuards();
                    Loading.UnloadModel(MostWantedMissions.MissionNineLabEntranceModel);
                    for (var i = 0; i < peds.Count; i++)
                    {
                        enemies.Add(new MissionPed(peds[i], neutralRelGroup));
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
                        objectiveLocation = MostWantedMissions.MISSION_NINE_LAB_INSIDE_LOCATION;
                        Loading.LoadModels(MostWantedMissions.MissionNineModels_2);
                        var peds = MostWantedMissions.InitializeMissionNineLabPeds();
                        Loading.UnloadModels(MostWantedMissions.MissionNineModels_2);
                        foreach (Ped ped in peds)
                        {
                            enemies.Add(new MissionPed(ped, aggressiveRelGroup));
                        }
                        GTA.UI.Screen.ShowSubtitle("Enter the ~b~meth lab~w~.", 8000);
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
                        bombOneBlip = World.CreateBlip(MostWantedMissions.MISSION_NINE_LAB_BOMB_ONE_POSITION);
                        bombOneBlip.Color = BlipColor.Yellow;
                        bombOneBlip.Name = "Plant bomb";
                        bombTwoBlip = World.CreateBlip(MostWantedMissions.MISSION_NINE_LAB_BOMB_TWO_POSITION);
                        bombTwoBlip.Color = BlipColor.Yellow;
                        bombTwoBlip.Name = "Plant bomb";
                        bombThreeBlip = World.CreateBlip(MostWantedMissions.MISSION_NINE_LAB_BOMB_THREE_POSITION);
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
                        if (Game.Player.Character.IsInRange(MostWantedMissions.MISSION_NINE_LAB_BOMB_ONE_POSITION, 1.1f))
                        {
                            GTA.UI.Screen.ShowHelpTextThisFrame($"Press ~{VigilanteMissions.InteractionControl}~ to plant the ~y~bomb");
                            if (Function.Call<bool>(Hash.IS_CONTROL_JUST_RELEASED, 0, VigilanteMissions.InteractionControl))
                            {
                                PlayPlantBombAnim(Vector3.RelativeBack.ToHeading());
                                Loading.LoadModel(MostWantedMissions.MissionNineBombModel);
                                Script.Wait(2000);
                                props.Add(MostWantedMissions.InitializeMissionNineBombOne());
                                Loading.UnloadModel(MostWantedMissions.MissionNineBombModel);
                                bombOneBlip.Delete();
                                bombOnePlanted = true;
                            }
                        }
                    }
                    if (!bombTwoPlanted)
                    {
                        if (Game.Player.Character.IsInRange(MostWantedMissions.MISSION_NINE_LAB_BOMB_TWO_POSITION, 1.1f))
                        {
                            GTA.UI.Screen.ShowHelpTextThisFrame($"Press ~{VigilanteMissions.InteractionControl}~ to plant the ~y~bomb");
                            if (Function.Call<bool>(Hash.IS_CONTROL_JUST_RELEASED, 0, VigilanteMissions.InteractionControl))
                            {
                                PlayPlantBombAnim(VigilanteMissions.WOVCompatibility ? Vector3.RelativeFront.ToHeading() : Vector3.RelativeBack.ToHeading());
                                Loading.LoadModel(MostWantedMissions.MissionNineBombModel);
                                Script.Wait(2000);
                                props.Add(MostWantedMissions.InitializeMissionNineBombTwo());
                                Loading.UnloadModel(MostWantedMissions.MissionNineBombModel);
                                bombTwoBlip.Delete();
                                bombTwoPlanted = true;
                            }
                        }
                    }
                    if (!bombThreePlanted)
                    {
                        if (Game.Player.Character.IsInRange(MostWantedMissions.MISSION_NINE_LAB_BOMB_THREE_POSITION, 1.1f))
                        {
                            GTA.UI.Screen.ShowHelpTextThisFrame($"Press ~{VigilanteMissions.InteractionControl}~ to plant the ~y~bomb");
                            if (Function.Call<bool>(Hash.IS_CONTROL_JUST_RELEASED, 0, VigilanteMissions.InteractionControl))
                            {
                                PlayPlantBombAnim(Vector3.RelativeFront.ToHeading());
                                Loading.LoadModel(MostWantedMissions.MissionNineBombModel);
                                Script.Wait(2000);
                                props.Add(MostWantedMissions.InitializeMissionNineBombThree());
                                Loading.UnloadModel(MostWantedMissions.MissionNineBombModel);
                                bombThreeBlip.Delete();
                                bombThreePlanted = true;
                            }
                        }
                    }
                    if (bombOnePlanted && bombTwoPlanted && bombThreePlanted)
                    {
                        GTA.UI.Screen.ShowSubtitle("Leave the meth lab.", 8000);
                        objectiveLocation = MostWantedMissions.MISSION_NINE_LAB_LOCATION;
                        currentObjective = Objectives.LeaveLab;
                    }
                    break;
                }
            case Objectives.LeaveLab:
                {
                    if (Game.Player.Character.IsInRange(objectiveLocation, 20))
                    {
                        Loading.LoadModels(MostWantedMissions.MissionNineModels_3);
                        vehicles = MostWantedMissions.InitializeMissionNineVehicles();
                        objectiveLocation = MostWantedMissions.MISSION_NINE_REINFORCEMENT_LOCATION;
                        var peds = new List<Ped>
                        {
                            vehicles[(int)Vehicles.Car].CreatePedOnSeat(VehicleSeat.Driver, new Model(PedHash.Chef)),
                            vehicles[(int)Vehicles.Bike01].CreatePedOnSeat(VehicleSeat.Driver, new Model(PedHash.PoloGoon01GMY)),
                            vehicles[(int)Vehicles.Bike02].CreatePedOnSeat(VehicleSeat.Driver, new Model(PedHash.PoloGoon01GMY)),
                            vehicles[(int)Vehicles.Bike01].CreatePedOnSeat(VehicleSeat.Passenger, new Model(PedHash.PoloGoon01GMY)),
                            vehicles[(int)Vehicles.Bike02].CreatePedOnSeat(VehicleSeat.Passenger, new Model(PedHash.PoloGoon01GMY)),
                        };
                        Loading.UnloadModels(MostWantedMissions.MissionNineModels_3);
                        for (var i = 0; i < peds.Count; i++)
                        {
                            enemies.Add(new MissionPed(peds[i], neutralRelGroup, false, true));
                            enemies[i].ShowBlip();
                        }
                        enemies[(int)Vehicles.Car].GetPed().BlockPermanentEvents = true;
                        Function.Call(Hash.TASK_VEHICLE_MISSION_COORS_TARGET, enemies[(int)Vehicles.Car].GetPed().Handle, vehicles[(int)Vehicles.Car].Handle, objectiveLocation.X, objectiveLocation.Y, objectiveLocation.Z, 4, 300f, (int)DrivingStyle.Rushed, 5f, 5f, 1);
                        Function.Call(Hash.TASK_VEHICLE_MISSION_COORS_TARGET, enemies[(int)Vehicles.Bike01].GetPed().Handle, vehicles[(int)Vehicles.Bike01].Handle, objectiveLocation.X, objectiveLocation.Y, objectiveLocation.Z, 4, 225f, (int)DrivingStyle.Rushed, 5f, 5f, 1);
                        Function.Call(Hash.TASK_VEHICLE_MISSION_COORS_TARGET, enemies[(int)Vehicles.Bike02].GetPed().Handle, vehicles[(int)Vehicles.Bike02].Handle, objectiveLocation.X, objectiveLocation.Y, objectiveLocation.Z, 4, 225f, (int)DrivingStyle.Rushed, 5f, 5f, 1);
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
                            if (enemies[0].GetPed().IsInRange(objectiveLocation, 110))
                            {
                                Loading.LoadModel(MostWantedMissions.MissionNineReinforcementModel);
                                var peds = MostWantedMissions.InitializeMissionNineReinforcements();
                                Loading.UnloadModel(MostWantedMissions.MissionNineReinforcementModel);
                                for (var i = 0; i < peds.Count; i++)
                                {
                                    enemies.Add(new MissionPed(peds[i], aggressiveRelGroup));
                                    if (enemies[i].GetBlip() == null)
                                    {
                                        enemies[i].ShowBlip();
                                    }
                                }
                                reinforcementSpawned = true;
                                enemies[0].GetPed().BlockPermanentEvents = false;
                                var sequence = new TaskSequence();
                                sequence.AddTask.LeaveVehicle();
                                sequence.AddTask.FleeFrom(Game.Player.Character);
                                enemies[0].GetTask().PerformSequence(sequence);
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
                    GTA.UI.Screen.ShowHelpTextThisFrame($"Press ~{VigilanteMissions.InteractionControl}~ to blow up the ~y~meth lab");
                    if (Function.Call<bool>(Hash.IS_CONTROL_JUST_RELEASED, 0, VigilanteMissions.InteractionControl))
                    {
                        World.AddExplosion(MostWantedMissions.MISSION_NINE_LAB_LOCATION, ExplosionType.Tanker, 50, 15);
                        World.AddExplosion(MostWantedMissions.MISSION_NINE_LAB_LOCATION, ExplosionType.Tanker, 50, 15);
                        World.AddExplosion(MostWantedMissions.MISSION_NINE_LAB_LOCATION, ExplosionType.Tanker, 50, 15);
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
        if (ObjectiveLocationBlip.Exists())
        {
            ObjectiveLocationBlip.Delete();
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

        Music.StartCountry();

        GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Wanted suspect", "I couldn't find this ~r~Heisenberg~w~ guy, but i know where his associate ~r~Jesse Pinkman~w~ is. Head to ~y~The motor motel~w~.");

        ObjectiveLocationBlip = World.CreateBlip(objectiveLocation);
        ObjectiveLocationBlip.DisplayType = BlipDisplayType.BothMapSelectable;
        ObjectiveLocationBlip.Scale = 0.9f;
        ObjectiveLocationBlip.Color = BlipColor.Yellow;
        ObjectiveLocationBlip.Name = "Jesse Pinkman's location";
        ObjectiveLocationBlip.ShowRoute = true;

        GTA.UI.Screen.ShowSubtitle("Go to ~y~The Motor motel~w~.", 8000);

        currentObjective = Objectives.GoToMotelRoom;

        MissionWorld.script.Tick += MissionTick;

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
