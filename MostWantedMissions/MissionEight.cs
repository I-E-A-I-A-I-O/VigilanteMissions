using GTA;
using GTA.Math;
using System;
using System.Collections.Generic;

class MissionEight : Mission
{
    public override bool IsMostWanted => true;
    public override bool IsJokerMission => false;

    enum Enemies
    {
        MusicBox,
        Table01,
        Table02,
        Table03,
        Bartender,
        Bar,
        Hallway01,
        Hallway02,
        Couch01,
        Couch02,
        Hammering,
        Bathroom,
        Leaning,
        TalkingToLeaning,
        BikeAlone,
        BikeGroup01,
        BikeGroup02,
        BikeGroup03,
        Filming,
        Cheering,
        PushUp01,
        PushUp02,
        PushUp03,
        Smoking,
        Drinking,
        Door01,
        Door02
    }

    enum Props
    {
        Fireaxe,
        Firehose
    }

    enum Objectives
    {
        GoToLocation,
        KillEnemies,
        PickupAxe,
        KillTarget,
        Completed,
        None
    }

    Vector3 objectiveLocation;
    Objectives currentObjective;
    public override Blip ObjectiveLocationBlip { get; set; }
    RelationshipGroup enemiesRelGroup;
    RelationshipGroup targetRelGroup;
    List<MissionPed> enemies = new List<MissionPed>();
    List<Vehicle> vehicles = new List<Vehicle>();
    List<Prop> props = new List<Prop>();

    public MissionEight()
    {
        enemiesRelGroup = MissionWorld.RELATIONSHIP_MISSION_NEUTRAL;
        targetRelGroup = MissionWorld.RELATIONSHIP_MISSION_AGGRESSIVE;
        objectiveLocation = MostWantedMissions.MISSION_EIGHT_LOCATON;
    }

    protected override void MissionTick(object o, EventArgs e)
    {
        if (Game.Player.WantedLevel >= 2)
        {
            Game.Player.WantedLevel = 1;
        }
        
        switch(currentObjective)
        {
            case Objectives.GoToLocation:
                {
                    if (!Game.Player.Character.IsInRange(objectiveLocation, 200))
                    {
                        return;
                    }
                    Music.IncreaseIntensity();
                    ObjectiveLocationBlip.Delete();

                    var peds = MostWantedMissions.InitializeMissionEightPeds();
                    props = MostWantedMissions.InitializeMissionEightProps();
                    vehicles = MostWantedMissions.InitializeMissionEightVehicles();
                    peds = MissionWorld.PedListLoadLoop(peds, MostWantedMissions.InitializeMissionEightPeds);
                    props = MissionWorld.PropListLoadLoop(props, MostWantedMissions.InitializeMissionEightProps);
                    vehicles = MissionWorld.VehicleListLoadLoop(vehicles, MostWantedMissions.InitializeMissionEightVehicles);
                    
                    for (var i = 0; i < peds.Count; i++) 
                    {
                        enemies.Add(new MissionPed(peds[i], enemiesRelGroup));
                        enemies[i].ShowBlip();
                    }
                    GTA.UI.Screen.ShowSubtitle("Kill the ~r~bikers~w~.", 8000);
                    StartScenarios();
                    currentObjective = Objectives.KillEnemies;
                    break;
                }
            case Objectives.KillEnemies:
                {
                    if (enemies.Count > 0)
                    {
                        RemoveDeadEnemies();
                    } else
                    {
                        props[(int)Props.Fireaxe].AddBlip();
                        props[(int)Props.Fireaxe].AttachedBlip.Color = BlipColor.Green;
                        props[(int)Props.Fireaxe].AttachedBlip.Name = "Axe";
                        objectiveLocation = props[(int)Props.Fireaxe].Position;
                        GTA.UI.Screen.ShowSubtitle("Grab the ~g~axe~w~.", 8000);
                        currentObjective = Objectives.PickupAxe;
                    }
                    break;
                }
            case Objectives.PickupAxe:
                {
                    if (Game.Player.Character.IsInRange(objectiveLocation, 1))
                    {
                        if (Game.LastInputMethod == InputMethod.GamePad)
                        {
                            GTA.UI.Screen.ShowHelpTextThisFrame("Press DPad Right to grab the ~g~axe");
                        }
                        else
                        {
                            GTA.UI.Screen.ShowHelpTextThisFrame($"Press {VigilanteMissions.interactKey} to grab the ~g~axe");
                        }
                        if ((Game.LastInputMethod == InputMethod.MouseAndKeyboard && Game.IsKeyPressed(VigilanteMissions.interactMissionKey)) || (Game.LastInputMethod == InputMethod.GamePad && Game.IsControlJustReleased(GTA.Control.ScriptPadRight)))
                        {
                            props[(int)Props.Fireaxe].Delete();
                            props.RemoveAt((int)Props.Fireaxe);
                            Game.Player.Character.Weapons.Give(WeaponHash.BattleAxe, 1, true, true);
                            var ped = MostWantedMissions.InitializeMissionEightTarget();
                            ped = (Ped)MissionWorld.EntityLoadLoop(ped, MostWantedMissions.InitializeMissionEightTarget);
                            enemies.Add(new MissionPed(ped, targetRelGroup));
                            enemies[0].ShowBlip();
                            enemies[0].GetBlip().IsFlashing = true;
                            var sequence = new TaskSequence();
                            sequence.AddTask.Cower(8500);
                            sequence.AddTask.ReactAndFlee(Game.Player.Character);
                            enemies[0].GetTask().PerformSequence(sequence);
                            GTA.UI.Screen.ShowSubtitle("Cut down ~r~Harry Bowman~w~.", 8000);
                            currentObjective = Objectives.KillTarget;
                        }
                    }
                    break;
                }
            case Objectives.KillTarget:
                {
                    if (enemies.Count > 0)
                    {
                        RemoveDeadEnemies();
                    } else
                    {
                        MissionWorld.CompleteMission();
                        RemoveVehiclesAndNeutrals();
                        currentObjective = Objectives.None;
                        Game.Player.Money += 15000;
                        Game.Player.WantedLevel = 3;
                        GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Wanted Suspect", "Good job, your cut of the reward is in your account.");
                        MissionWorld.script.Tick -= MissionTick;
                    }
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

    protected override void RemoveDeadEnemies()
    {
        var aliveEnemies = enemies;
        for (var i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].IsDead())
            {
                enemies[i].Delete();
                aliveEnemies.RemoveAt(i);
                if (enemies[i].GetPed().Killer == Game.Player.Character)
                {
                    Progress.enemiesKilledCount += 1;
                }
            }
        }
        enemies = aliveEnemies;
    }

    protected override void RemoveVehiclesAndNeutrals()
    {
        foreach (Prop prop in props)
        {
            if (prop.AttachedBlip != null)
            {
                prop.AttachedBlip.Delete();
            }
            prop.MarkAsNoLongerNeeded();
        }
        foreach(Vehicle vehicle in vehicles)
        {
            vehicle.MarkAsNoLongerNeeded();
        }
    }

    public override bool StartMission()
    {
        if (Game.Player.Character.IsInRange(objectiveLocation, 200f))
        {
            GTA.UI.Notification.Show("Mission not available.");
            return false;
        }
        Music.StartCountry();
        currentObjective = Objectives.GoToLocation;
        ObjectiveLocationBlip = World.CreateBlip(objectiveLocation, 150f);
        ObjectiveLocationBlip.Color = BlipColor.Yellow;
        ObjectiveLocationBlip.ShowRoute = true;
        ObjectiveLocationBlip.Name = "Wanted suspect location";

        GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Wanated suspect", "Ok, i tracked them down, i'm sending you the location.");
        GTA.UI.Screen.ShowSubtitle("Go to the ~y~wanted suspect~w~.");

        MissionWorld.script.Tick += MissionTick;
        return true;
    }

    void StartScenarios()
    {
        enemies[(int)Enemies.Bar].GetTask().StartScenario("WORLD_HUMAN_DRINKING", 0);
        enemies[(int)Enemies.Bartender].GetTask().ChatTo(enemies[(int)Enemies.Bar].GetPed());
        enemies[(int)Enemies.Bathroom].GetTask().StartScenario("WORLD_HUMAN_STAND_IMPATIENT", 0);
        enemies[(int)Enemies.BikeAlone].GetTask().StartScenario("WORLD_HUMAN_SMOKING", 0);
        enemies[(int)Enemies.BikeGroup01].GetTask().ChatTo(enemies[(int)Enemies.BikeGroup02].GetPed());
        enemies[(int)Enemies.BikeGroup02].GetTask().ChatTo(enemies[(int)Enemies.BikeGroup01].GetPed());
        enemies[(int)Enemies.BikeGroup03].GetTask().StartScenario("WORLD_HUMAN_CLIPBOARD", 0);
        enemies[(int)Enemies.Cheering].GetTask().StartScenario("WORLD_HUMAN_CHEERING", 0);
        enemies[(int)Enemies.Couch01].GetTask().ChatTo(enemies[(int)Enemies.Couch02].GetPed());
        enemies[(int)Enemies.Couch02].GetTask().UseMobilePhone();
        enemies[(int)Enemies.Door01].GetTask().StartScenario("WORLD_HUMAN_SMOKING", 0);
        enemies[(int)Enemies.Door02].GetTask().ChatTo(enemies[(int)Enemies.Door01].GetPed());
        enemies[(int)Enemies.Drinking].GetTask().StartScenario("WORLD_HUMAN_DRINKING", 0);
        enemies[(int)Enemies.Filming].GetTask().StartScenario("WORLD_HUMAN_MOBILE_FILM_SHOCKING", 0);
        enemies[(int)Enemies.Hallway01].GetTask().ChatTo(enemies[(int)Enemies.Hallway02].GetPed());
        enemies[(int)Enemies.Hallway02].GetTask().ChatTo(enemies[(int)Enemies.Hallway01].GetPed());
        enemies[(int)Enemies.Hammering].GetTask().StartScenario("WORLD_HUMAN_HAMMERING", 0);
        enemies[(int)Enemies.Leaning].GetTask().StartScenario("WORLD_HUMAN_LEANING", 0);
        enemies[(int)Enemies.MusicBox].GetTask().StartScenario("WORLD_HUMAN_STAND_MOBILE_UPRIGHT", 0);
        enemies[(int)Enemies.PushUp01].GetTask().StartScenario("WORLD_HUMAN_PUSH_UPS", 0);
        enemies[(int)Enemies.PushUp02].GetTask().StartScenario("WORLD_HUMAN_PUSH_UPS", 0);
        enemies[(int)Enemies.PushUp03].GetTask().StartScenario("WORLD_HUMAN_PUSH_UPS", 0);
        enemies[(int)Enemies.Smoking].GetTask().StartScenario("WORLD_HUMAN_SMOKING", 0);
        enemies[(int)Enemies.Table01].GetTask().ChatTo(enemies[(int)Enemies.Table02].GetPed());
        enemies[(int)Enemies.Table02].GetTask().ChatTo(enemies[(int)Enemies.Table01].GetPed());
        enemies[(int)Enemies.Table03].GetTask().StartScenario("WORLD_HUMAN_HANG_OUT_STREET", 0);
        enemies[(int)Enemies.TalkingToLeaning].GetTask().ChatTo(enemies[(int)Enemies.Leaning].GetPed());
    }
}
