using GTA;
using GTA.Native;
using GTA.Math;
using System;
using System.Collections.Generic;

public class MissionWorld
{
    public static Script script;
    public static RelationshipGroup RELATIONSHIP_MISSION_LIKE;
    public static RelationshipGroup RELATIONSHIP_MISSION_NEUTRAL;
    public static RelationshipGroup RELATIONSHIP_MISSION_AGGRESSIVE;
    public static RelationshipGroup RELATIONSHIP_MISSION_PEDESTRIAN;
    public static RelationshipGroup RELATIONSHIP_MISSION_DISLIKE;
    public static RelationshipGroup RELATIONSHIP_MISSION_MASS_SHOOTER;
    public static RelationshipGroup RELATIONSHIP_MISSION_NEUTRAL_COP_FRIENDLY;
    public static bool isMissionActive;
    static Mission currentMission;
    bool blipCheckTimerStarted = false;
    int blipCheckStartTime;
    int blipCheckCurrentTime;
    
    public enum Missions
    {
        MostWanted1,
        MostWanted2,
        MostWanted3,
        MostWanted4,
        MostWanted5,
        MostWanted6,
        MostWanted7,
        MostWanted8,
        MostWanted9,
        MostWanted10,
        MostWanted111,
        Assault,
        GangActivity,
        StolenVehicle,
        SuspectOnTheRun,
        MassShooter,
        PacificStandard,
        FleecaBank,
        None
    }

    public MissionWorld(Script script)
    {
        MissionWorld.script = script;

        var RELATIONSHIP_PLAYER = Function.Call<int>(Hash.GET_HASH_KEY, "PLAYER");
        var RELATIONSHIP_COPS = Function.Call<int>(Hash.GET_HASH_KEY, "COP");
        var RELATIONSHIP_CIVMALE = Function.Call<int>(Hash.GET_HASH_KEY, "CIVMALE");
        var RELATIONSHIP_CIVFEMALE = Function.Call<int>(Hash.GET_HASH_KEY, "CIVFEMALE");

        RELATIONSHIP_MISSION_LIKE = World.AddRelationshipGroup("VIGILANTE_MOST_WANTED_MISSION_LIKE");
        RELATIONSHIP_MISSION_NEUTRAL = World.AddRelationshipGroup("VIGILANTE_MOST_WANTED_MISSION_NEUTRAL");
        RELATIONSHIP_MISSION_PEDESTRIAN = World.AddRelationshipGroup("VIGILANTE_MISSION_PEDESTRIAN");
        RELATIONSHIP_MISSION_AGGRESSIVE = World.AddRelationshipGroup("VIGILANTE_MISSION_AGGRESSIVE");
        RELATIONSHIP_MISSION_DISLIKE = World.AddRelationshipGroup("VIGILANTE_MISSION_DISLIKE");
        RELATIONSHIP_MISSION_MASS_SHOOTER = World.AddRelationshipGroup("VIGILANTE_MISSION_MASS_SHOOTER");
        RELATIONSHIP_MISSION_NEUTRAL_COP_FRIENDLY = World.AddRelationshipGroup("VIGILANTE_MISSION_NEUTRAL_COP_FRIENDLY");

        RELATIONSHIP_MISSION_LIKE.SetRelationshipBetweenGroups(RELATIONSHIP_PLAYER, Relationship.Respect);
        RELATIONSHIP_MISSION_NEUTRAL.SetRelationshipBetweenGroups(RELATIONSHIP_PLAYER, Relationship.Neutral, true);
        RELATIONSHIP_MISSION_NEUTRAL.SetRelationshipBetweenGroups(RELATIONSHIP_COPS, Relationship.Hate, true);
        RELATIONSHIP_MISSION_PEDESTRIAN.SetRelationshipBetweenGroups(RELATIONSHIP_PLAYER, Relationship.Pedestrians, true);
        RELATIONSHIP_MISSION_AGGRESSIVE.SetRelationshipBetweenGroups(RELATIONSHIP_PLAYER, Relationship.Hate, true);
        RELATIONSHIP_MISSION_AGGRESSIVE.SetRelationshipBetweenGroups(RELATIONSHIP_COPS, Relationship.Hate, true);
        RELATIONSHIP_MISSION_DISLIKE.SetRelationshipBetweenGroups(RELATIONSHIP_PLAYER, Relationship.Dislike, true);
        RELATIONSHIP_MISSION_DISLIKE.SetRelationshipBetweenGroups(RELATIONSHIP_COPS, Relationship.Hate, true);
        RELATIONSHIP_MISSION_MASS_SHOOTER.SetRelationshipBetweenGroups(RELATIONSHIP_CIVMALE, Relationship.Hate, true);
        RELATIONSHIP_MISSION_MASS_SHOOTER.SetRelationshipBetweenGroups(RELATIONSHIP_CIVFEMALE, Relationship.Hate, true);
        RELATIONSHIP_MISSION_MASS_SHOOTER.SetRelationshipBetweenGroups(RELATIONSHIP_PLAYER, Relationship.Hate);
        RELATIONSHIP_MISSION_MASS_SHOOTER.SetRelationshipBetweenGroups(RELATIONSHIP_COPS, Relationship.Hate, true);
        RELATIONSHIP_MISSION_NEUTRAL_COP_FRIENDLY.SetRelationshipBetweenGroups(RELATIONSHIP_PLAYER, Relationship.Neutral, true);

        isMissionActive = false;
        script.Tick += ObjectiveBlipCheck;
    }

    void ObjectiveBlipCheck(object o, EventArgs e)
    {
        if (!isMissionActive)
        {
            return;
        }
        if (currentMission.ObjectiveLocationBlip == null || !currentMission.ObjectiveLocationBlip.Exists())
        {
            return;
        }
        if (!blipCheckTimerStarted)
        {
            blipCheckStartTime = Game.GameTime;
            blipCheckTimerStarted = true;
        } else
        {
            blipCheckCurrentTime = Game.GameTime;
            if (blipCheckCurrentTime - blipCheckStartTime >= 8000)
            {
                currentMission.ObjectiveLocationBlip.ShowRoute = false;
                Script.Wait(1);
                currentMission.ObjectiveLocationBlip.ShowRoute = true;
                blipCheckTimerStarted = false;
            }
        }
    }

    public static void StartMission(Missions mission)
    {
        switch (mission)
        {
            case Missions.MostWanted1:
                {
                    currentMission = new MissionOne();
                    break;
                }
            case Missions.MostWanted2:
                {
                    currentMission = new MissionTwo();
                    break;
                }
            case Missions.MostWanted3:
                {
                    currentMission = new MissionThree();
                    break;
                }
            case Missions.MostWanted4:
                {
                    currentMission = new MissionFour();
                    break;
                }
            case Missions.MostWanted5:
                {
                    currentMission = new MissionFive();
                    break;
                }
            case Missions.MostWanted6:
                {
                    currentMission = new MissionSix();
                    break;
                }
            case Missions.MostWanted7:
                {
                    currentMission = new MissionSeven();
                    break;
                }
            case Missions.MostWanted8:
                {
                    currentMission = new MissionEight();
                    break;
                }
            case Missions.MostWanted9:
                {
                    currentMission = new MissionNine();
                    break;
                }
            case Missions.MostWanted10:
                {
                    currentMission = new MissionTen();
                    break;
                }
            case Missions.MostWanted111:
                {
                    currentMission = new MissionElevenPartOne();
                    break;
                }
            case Missions.Assault:
                {
                    currentMission = new Assault();
                    break;
                }
            case Missions.StolenVehicle:
                {
                    currentMission = new StolenVehicle();
                    break;
                }
            case Missions.GangActivity:
                {
                    currentMission = new GangActivity();
                    break;
                }
            case Missions.SuspectOnTheRun:
                {
                    currentMission = new SuspectOnTheRun();
                    break;
                }
            case Missions.MassShooter:
                {
                    currentMission = new MassShooter();
                    break;
                }
            case Missions.PacificStandard:
                {
                    currentMission = new PacificRobbery();
                    break;
                }
            case Missions.FleecaBank:
                {
                    currentMission = new FleecaRooberies();
                    break;
                }
        }
        isMissionActive = currentMission.StartMission();
    }

    public static void QuitMission()
    {
        currentMission.QuitMission();
        isMissionActive = false;
        currentMission = null;
    }

    public static void CompleteMission()
    {
        Music.PlayMissionCompleted();
        isMissionActive = false;
        if (currentMission.IsMostWanted)
        {
            Progress.completedMostWantedMissionsCount += 1;
            if (Progress.completedMostWantedMissionsCount >= VigilanteMissions.jokerMissionCount)
            {
                if (!Progress.jokerUnlockedMessageSent)
                {
                    GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Vigilante Missions", "There's a new bounty out for this sicko called 'The Joker'. Let me know when you want to go after him");
                    Progress.jokerUnlockedMessageSent = true;                
                }
                if (!Progress.jokerUnlocked)
                {
                    Progress.jokerUnlocked = true;
                }
                VigilanteMissions.AddJoker();
            }
            if (currentMission.IsJokerMission && !Progress.jokerKilled)
            {
                Progress.jokerKilled = true;
                GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Vigilante reward", "Now that you hacked the IAA servers to kill the Joker, you can now access the police computer using any vehicle. NOTE: you have to enable the reward in the ini file first!");
            }
        } else
        {
            Progress.completedCurrentCrimesMissionsCount += 1;
        }
        VigilanteMissions.SaveProgress();
        currentMission = null;
    }
}

