using GTA;
using GTA.Native;
using GTA.Math;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

class MissionEight : Mission
{
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
    Music music;
    Script script;
    MissionWorld missionWorld;
    MostWantedMissions mostWantedMissions;
    Blip objectiveLocationBlip;
    RelationshipGroup enemiesRelGroup;
    RelationshipGroup targetRelGroup;
    List<MissionPed> enemies = new List<MissionPed>();
    List<Vehicle> vehicles = new List<Vehicle>();
    List<Prop> props = new List<Prop>();

    public MissionEight(Script script, MissionWorld missionWorld, RelationshipGroup relationshipGroup, RelationshipGroup targetRelGroup)
    {
        this.script = script;
        this.missionWorld = missionWorld;
        enemiesRelGroup = relationshipGroup;
        this.targetRelGroup = targetRelGroup;

        music = new Music();
        mostWantedMissions = new MostWantedMissions();
        objectiveLocation = mostWantedMissions.MISSION_EIGHT_LOCATON;
    }

    public override void MissionTick(object o, EventArgs e)
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
                    music.IncreaseIntensity();
                    objectiveLocationBlip.Delete();
                    var peds = mostWantedMissions.InitializeMissionEightPeds();
                    props = mostWantedMissions.InitializeMissionEightProps();
                    vehicles = mostWantedMissions.InitializeMissionEightVehicles();
                    Script.Wait(1000);
                    for(var i = 0; i < peds.Count; i++) {
                        enemies.Add(new MissionPed(peds[i], enemiesRelGroup, objectiveLocation, script));
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
                            GTA.UI.Screen.ShowHelpTextThisFrame("Press RB to grab the ~g~axe");
                        }
                        else
                        {
                            GTA.UI.Screen.ShowHelpTextThisFrame("Press E to grab the ~g~axe");
                        }
                        if ((Game.LastInputMethod == InputMethod.MouseAndKeyboard && Game.IsKeyPressed(Keys.E)) || (Game.LastInputMethod == InputMethod.GamePad && Game.IsControlJustReleased(GTA.Control.ScriptRB)))
                        {
                            props[(int)Props.Fireaxe].Delete();
                            props.RemoveAt((int)Props.Fireaxe);
                            Game.Player.Character.Weapons.Give(WeaponHash.BattleAxe, 1, true, true);
                            var ped = mostWantedMissions.InitializeMissionEightTarget();
                            Script.Wait(1000);
                            enemies.Add(new MissionPed(ped, targetRelGroup, objectiveLocation, script));
                            enemies[0].ShowBlip();
                            enemies[0].ped.AttachedBlip.IsFlashing = true;
                            var sequence = new TaskSequence();
                            sequence.AddTask.Cower(8500);
                            sequence.AddTask.ReactAndFlee(Game.Player.Character);
                            enemies[0].ped.Task.PerformSequence(sequence);
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
                        missionWorld.CompleteMission();
                        RemoveVehiclesAndNeutrals();
                        currentObjective = Objectives.None;
                        Game.Player.Money += 15000;
                        Game.Player.WantedLevel = 3;
                        GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Wanted Suspect", "Good job, your cut of the reward is in your account.");
                        script.Tick -= MissionTick;
                    }
                    break;
                }
        }
    }

    public override void QuitMission()
    {
        music.StopMusic();
        currentObjective = Objectives.None;
        script.Tick -= MissionTick;
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
        music.StartCountry();
        currentObjective = Objectives.GoToLocation;
        objectiveLocationBlip = World.CreateBlip(objectiveLocation, 150f);
        objectiveLocationBlip.Color = BlipColor.Yellow;
        objectiveLocationBlip.ShowRoute = true;
        objectiveLocationBlip.Name = "Wanted suspect location";

        GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Wanated suspect", "Ok, i tracked them down, i'm sending you the location.");
        GTA.UI.Screen.ShowSubtitle("Go to the ~y~wanted suspect~w~.");

        script.Tick += MissionTick;
        return true;
    }

    void StartScenarios()
    {
        enemies[(int)Enemies.Bar].ped.Task.StartScenario("WORLD_HUMAN_DRINKING", 0);
        enemies[(int)Enemies.Bartender].ped.Task.ChatTo(enemies[(int)Enemies.Bar].ped);
        enemies[(int)Enemies.Bathroom].ped.Task.StartScenario("WORLD_HUMAN_STAND_IMPATIENT", 0);
        enemies[(int)Enemies.BikeAlone].ped.Task.StartScenario("WORLD_HUMAN_SMOKING", 0);
        enemies[(int)Enemies.BikeGroup01].ped.Task.ChatTo(enemies[(int)Enemies.BikeGroup02].ped);
        enemies[(int)Enemies.BikeGroup02].ped.Task.ChatTo(enemies[(int)Enemies.BikeGroup01].ped);
        enemies[(int)Enemies.BikeGroup03].ped.Task.StartScenario("WORLD_HUMAN_CLIPBOARD", 0);
        enemies[(int)Enemies.Cheering].ped.Task.StartScenario("WORLD_HUMAN_CHEERING", 0);
        enemies[(int)Enemies.Couch01].ped.Task.ChatTo(enemies[(int)Enemies.Couch02].ped);
        enemies[(int)Enemies.Couch02].ped.Task.UseMobilePhone();
        enemies[(int)Enemies.Door01].ped.Task.StartScenario("WORLD_HUMAN_SMOKING", 0);
        enemies[(int)Enemies.Door02].ped.Task.ChatTo(enemies[(int)Enemies.Door01].ped);
        enemies[(int)Enemies.Drinking].ped.Task.StartScenario("WORLD_HUMAN_DRINKING", 0);
        enemies[(int)Enemies.Filming].ped.Task.StartScenario("WORLD_HUMAN_MOBILE_FILM_SHOCKING", 0);
        enemies[(int)Enemies.Hallway01].ped.Task.ChatTo(enemies[(int)Enemies.Hallway02].ped);
        enemies[(int)Enemies.Hallway02].ped.Task.ChatTo(enemies[(int)Enemies.Hallway01].ped);
        enemies[(int)Enemies.Hammering].ped.Task.StartScenario("WORLD_HUMAN_HAMMERING", 0);
        enemies[(int)Enemies.Leaning].ped.Task.StartScenario("WORLD_HUMAN_LEANING", 0);
        enemies[(int)Enemies.MusicBox].ped.Task.StartScenario("WORLD_HUMAN_STAND_MOBILE_UPRIGHT", 0);
        enemies[(int)Enemies.PushUp01].ped.Task.StartScenario("WORLD_HUMAN_PUSH_UPS", 0);
        enemies[(int)Enemies.PushUp02].ped.Task.StartScenario("WORLD_HUMAN_PUSH_UPS", 0);
        enemies[(int)Enemies.PushUp03].ped.Task.StartScenario("WORLD_HUMAN_PUSH_UPS", 0);
        enemies[(int)Enemies.Smoking].ped.Task.StartScenario("WORLD_HUMAN_SMOKING", 0);
        enemies[(int)Enemies.Table01].ped.Task.ChatTo(enemies[(int)Enemies.Table02].ped);
        enemies[(int)Enemies.Table02].ped.Task.ChatTo(enemies[(int)Enemies.Table01].ped);
        enemies[(int)Enemies.Table03].ped.Task.StartScenario("WORLD_HUMAN_HANG_OUT_STREET", 0);
        enemies[(int)Enemies.TalkingToLeaning].ped.Task.ChatTo(enemies[(int)Enemies.Leaning].ped);
    }
}
