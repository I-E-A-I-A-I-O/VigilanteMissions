using GTA;
using GTA.Native;
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
    public static RelationshipGroup RELATIONSHIP_MISSION_COP;
    public static bool isMissionActive;
    static Mission currentMission;
    
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
        Assault,
        GangActivity,
        StolenVehicle,
        SuspectOnTheRun,
        MassShooter,
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
        RELATIONSHIP_MISSION_COP = new RelationshipGroup(RELATIONSHIP_COPS);

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

        isMissionActive = false;
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
                    currentMission = new MissionTen(script, this, RELATIONSHIP_MISSION_NEUTRAL, RELATIONSHIP_MISSION_PEDESTRIAN);
                    break;
                }
            case Missions.Assault:
                {
                    currentMission = new Assault(script, this, RELATIONSHIP_MISSION_AGGRESSIVE, RELATIONSHIP_MISSION_PEDESTRIAN);
                    break;
                }
            case Missions.StolenVehicle:
                {
                    currentMission = new StolenVehicle(script, this, RELATIONSHIP_MISSION_NEUTRAL);
                    break;
                }
            case Missions.GangActivity:
                {
                    currentMission = new GangActivity(script, this, RELATIONSHIP_MISSION_NEUTRAL);
                    break;
                }
            case Missions.SuspectOnTheRun:
                {
                    currentMission = new SuspectOnTheRun(script, this, RELATIONSHIP_MISSION_AGGRESSIVE);
                    break;
                }
            case Missions.MassShooter:
                {
                    currentMission = new MassShooter(script, this, RELATIONSHIP_MISSION_MASS_SHOOTER);
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
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "MP_DM_COUNTDOWN_KILL");
        isMissionActive = false;
        currentMission = null;
    }
}

