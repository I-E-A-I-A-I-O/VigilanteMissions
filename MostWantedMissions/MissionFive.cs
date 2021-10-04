using GTA;
using GTA.Math;
using System;
using System.Collections.Generic;

class MissionFive : Mission
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

    Vector3 objectiveLocation;
    List<MissionPed> enemies = new List<MissionPed>();
    List<MissionPed> neutralPeds = new List<MissionPed>();
    Objectives currentObjective;
    public override Blip ObjectiveLocationBlip { get; set; }
    RelationshipGroup enemiesRelGroup;
    RelationshipGroup neutralsRelGroup;

    public MissionFive()
    {
        enemiesRelGroup = MissionWorld.RELATIONSHIP_MISSION_DISLIKE;
        neutralsRelGroup = MissionWorld.RELATIONSHIP_MISSION_PEDESTRIAN;

        objectiveLocation = MostWantedMissions.MISSION_FIVE_LOCATION;
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
                    var peds = MostWantedMissions.InitializeMissionFivePeds();
                    var neutrals = MostWantedMissions.InitializeMissionFiveCivilianPeds();
                    peds = MissionWorld.PedListLoadLoop(peds, MostWantedMissions.InitializeMissionFivePeds);
                    neutrals = MissionWorld.PedListLoadLoop(neutrals, MostWantedMissions.InitializeMissionFiveCivilianPeds);
                    for (var i = 0; i < peds.Count; i++)
                    {
                        enemies.Add(new MissionPed(peds[i], enemiesRelGroup));
                        enemies[i].ShowBlip();
                    }
                    for (var i = 0; i < neutrals.Count; i++)
                    {
                        neutralPeds.Add(new MissionPed(neutrals[i], neutralsRelGroup, true));
                    }
                    GTA.UI.Screen.ShowSubtitle("Save the ~g~woman~w~, kill the ~r~target~w~ before it's too late!", 8000);
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

    protected override void RemoveDeadEnemies()
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

    protected override void RemoveVehiclesAndNeutrals()
    {
        foreach (MissionPed neutral in neutralPeds)
        {
            neutral.Delete();
        }
    }

    public override bool StartMission()
    {
        if (Game.Player.Character.IsInRange(objectiveLocation, 200f))
        {
            GTA.UI.Notification.Show("Mission not available.");
            return false;
        }
        Music.StartTedBundyMusic();
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
        enemies[0].GetPed().CanSufferCriticalHits = false;
        enemies[0].GetPed().Armor = 600;
        enemies[0].GetPed().MaxHealth = 600;
        enemies[0].GetPed().Health = 600;
        enemies[0].GetPed().CanRagdoll = true;
        enemies[0].GetPed().CanWrithe = false;

        var bundySequence = new TaskSequence();
        bundySequence.AddTask.FollowToOffsetFromEntity(neutralPeds[0].GetPed(), new Vector3(), 25, 15000);
        bundySequence.AddTask.ShootAt(neutralPeds[0].GetPed(), -1, FiringPattern.FullAuto);
        neutralPeds[0].GetTask().FleeFrom(enemies[0].GetPed());
        enemies[0].GetTask().PerformSequence(bundySequence);
        neutralPeds[0].GetPed().AddBlip();
        neutralPeds[0].GetBlip().Scale = 0.8f;
        neutralPeds[0].GetBlip().Color = BlipColor.Green;
        neutralPeds[0].GetBlip().Name = "Victim";
        neutralPeds[0].GetBlip().IsFlashing = true;

        MissionWorld.script.Tick += CheckWomanStatus;
    }

    private void CheckWomanStatus(object sender, EventArgs e)
    {
        if (!MissionWorld.isMissionActive || enemies.Count == 0)
        {
            MissionWorld.script.Tick -= CheckWomanStatus;
            return;
        }
        if (neutralPeds[0].IsDead())
        {
            neutralPeds[0].GetBlip().Delete();
            MissionWorld.QuitMission();
            GTA.UI.Screen.ShowSubtitle("~r~Mission failed, the woman was killed!", 8000);
            MissionWorld.script.Tick -= CheckWomanStatus;
            return;
        }
    }
}
