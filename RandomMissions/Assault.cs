using GTA;
using GTA.Math;
using System;
using System.Collections.Generic;

class Assault : Mission
{
    public override bool IsMostWanted => false;
    public override bool IsJokerMission => false;

    enum Objectives
    {
        GoToLocation,
        KillTargets,
        Completed,
        None
    }

    Vector3 objectiveLocation;
    RelationshipGroup enemiesRelGroup;
    RelationshipGroup neutralsRelGroup;
    List<MissionPed> enemies = new List<MissionPed>();
    List<MissionPed> neutralPeds = new List<MissionPed>();
    Objectives currentObjective;
    public override Blip ObjectiveLocationBlip { get; set; }
    int actionToTake;
    int shootRange;
    bool actionTaken = false;
    int startTime;
    int currentTime;
    bool timerStarted = false;
    bool actionStarted = false;

    public Assault()
    {
        enemiesRelGroup = MissionWorld.RELATIONSHIP_MISSION_AGGRESSIVE;
        neutralsRelGroup = MissionWorld.RELATIONSHIP_MISSION_PEDESTRIAN;
    }

    protected override void MissionTick(object o, EventArgs e)
    {
        switch (currentObjective)
        {
            case Objectives.GoToLocation:
                {
                    if (!Game.Player.Character.IsInRange(objectiveLocation, 200f))
                    {
                        return;
                    }
                    ObjectiveLocationBlip.Delete();
                    var enemy = RandomMissions.CreateCriminal(objectiveLocation);
                    var neutral = RandomMissions.CreateVictim(objectiveLocation);
                    enemy = (Ped)MissionWorld.EntityLoadLoop(enemy, RandomMissions.CreateCriminal, objectiveLocation);
                    neutral = (Ped)MissionWorld.EntityLoadLoop(neutral, RandomMissions.CreateVictim, objectiveLocation);
                    enemies.Add(new MissionPed(enemy, enemiesRelGroup));
                    neutralPeds.Add(new MissionPed(neutral, neutralsRelGroup, true));
                    enemies[0].ShowBlip();
                    neutralPeds[0].GetPed().AddBlip();
                    neutralPeds[0].GetBlip().Scale = 0.8f;
                    neutralPeds[0].GetBlip().Color = BlipColor.Green;
                    neutralPeds[0].GetBlip().IsFlashing = true;
                    neutralPeds[0].GetBlip().Name = "Victim";
                    enemies[0].GetTask().AimAt(neutralPeds[0].GetPed(), 1800000);
                    neutralPeds[0].GetTask().HandsUp(1800000);
                    GTA.UI.Screen.ShowSubtitle("Kill the ~r~target~w~, don't let the ~g~victim~w~ get killed!", 8000);
                    currentObjective = Objectives.KillTargets;
                    break;
                }
            case Objectives.KillTargets:
                {
                    if (neutralPeds[0].IsDead())
                    {
                        neutralPeds[0].GetBlip().Delete();
                        GTA.UI.Screen.ShowSubtitle("~r~Mission failed, the victim died.", 8000);
                        MissionWorld.QuitMission();
                        return;
                    }
                    if (enemies[0].IsDead())
                    {
                        RemoveDeadEnemies();
                        currentObjective = Objectives.Completed;
                    }
                    else
                    {
                        if (actionStarted)
                        {
                            return;
                        }
                        if (enemies[0].GetPed().IsInCombatAgainst(Game.Player.Character))
                        {
                            actionStarted = true;
                            return;
                        }
                        if (Game.Player.Character.IsInRange(enemies[0].GetPosition(), 55))
                        {
                            if (!actionTaken)
                            {
                                Random ran = new Random();
                                actionToTake = ran.Next(1, 11);
                                shootRange = ran.Next(15, 41);
                                actionTaken = true;
                            }
                            bool isPlayerInRange = Game.Player.Character.IsInRange(enemies[0].GetPosition(), shootRange);
                            if (isPlayerInRange && !timerStarted)
                            {
                                startTime = Game.GameTime;
                                timerStarted = true;
                                enemies[0].GetPed().PlayAmbientSpeech("GENERIC_INSULT_HIGH", SpeechModifier.ShoutedClear);
                            } else if (!isPlayerInRange && timerStarted)
                            {
                                timerStarted = false;
                            } else if (isPlayerInRange && timerStarted) {
                                currentTime = Game.GameTime;
                                if (currentTime - startTime >= 1500)
                                {
                                    enemies[0].GetTask().ClearAllImmediately();
                                    if (actionToTake <= 3)
                                    {
                                        enemies[0].GetTask().ShootAt(neutralPeds[0].GetPed(), -1, FiringPattern.FullAuto);
                                    } else
                                    {
                                        enemies[0].GetTask().FightAgainst(Game.Player.Character);
                                    }
                                    actionStarted = true;
                                }
                            }
                        }
                    }
                    break;
                }
            case Objectives.Completed:
                {
                    GTA.UI.Screen.ShowSubtitle("Crime scene cleared.", 8000);
                    Game.Player.Money += 1000;
                    currentObjective = Objectives.None;
                    RemoveVehiclesAndNeutrals();
                    MissionWorld.CompleteMission();
                    MissionWorld.script.Tick -= MissionTick;
                    break;
                }
        }
    }

    public override void QuitMission()
    {
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
        foreach(MissionPed neutral in neutralPeds)
        {
            neutral.Delete();
        }
    }

    public override bool StartMission()
    {
        try
        {
            do
            {
                objectiveLocation = RandomMissions.GetRandomLocation(RandomMissions.LocationType.Foot);
            } while (Game.Player.Character.IsInRange(objectiveLocation, 200f));

            currentObjective = Objectives.GoToLocation;
            ObjectiveLocationBlip = World.CreateBlip(objectiveLocation, 150f);
            ObjectiveLocationBlip.Color = BlipColor.Yellow;
            ObjectiveLocationBlip.ShowRoute = true;
            ObjectiveLocationBlip.Name = "Crime scene";
            GTA.UI.Screen.ShowSubtitle("Go to the ~y~crime scene~w~.", 8000);

            MissionWorld.script.Tick += MissionTick;
            return true;
        }
        catch (Exception)
        {
            GTA.UI.Notification.Show("Error starting the mission, try again...");
            return false;
        }
    }
}
