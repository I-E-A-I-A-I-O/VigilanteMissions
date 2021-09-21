using GTA;
using GTA.Native;
using System;
using System.Collections.Generic;

class MissionWorld
{
    Script script;
    RelationshipGroup RELATIONSHIP_MISSION_NEUTRAL;
    RelationshipGroup RELATIONSHIP_MISSION_AGGRESSIVE;
    RelationshipGroup RELATIONSHIP_MISSION_PEDESTRIAN;
    RelationshipGroup RELATIONSHIP_MISSION_DISLIKE;
    public bool isMissionActive;
    Mission currentMission;
    
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
        Assault,
        GangActivity,
        StolenVehicle,
        SuspectOnTheRun,
        None
    }

    public MissionWorld(Script script)
    {
        this.script = script;

        var RELATIONSHIP_PLAYER = Function.Call<int>(Hash.GET_HASH_KEY, "PLAYER");
        RELATIONSHIP_MISSION_NEUTRAL = World.AddRelationshipGroup("VIGILANTE_MOST_WANTED_MISSION_NEUTRAL");
        RELATIONSHIP_MISSION_PEDESTRIAN = World.AddRelationshipGroup("VIGILANTE_MISSION_PEDESTRIAN");
        RELATIONSHIP_MISSION_AGGRESSIVE = World.AddRelationshipGroup("VIGILANTE_MISSION_AGGRESSIVE");
        RELATIONSHIP_MISSION_DISLIKE = World.AddRelationshipGroup("VIGILANTE_MISSION_DISLIKE");

        RELATIONSHIP_MISSION_NEUTRAL.SetRelationshipBetweenGroups(RELATIONSHIP_PLAYER, Relationship.Neutral, true);
        RELATIONSHIP_MISSION_PEDESTRIAN.SetRelationshipBetweenGroups(RELATIONSHIP_PLAYER, Relationship.Pedestrians, true);
        RELATIONSHIP_MISSION_AGGRESSIVE.SetRelationshipBetweenGroups(RELATIONSHIP_PLAYER, Relationship.Hate, true);
        RELATIONSHIP_MISSION_DISLIKE.SetRelationshipBetweenGroups(RELATIONSHIP_PLAYER, Relationship.Dislike, true);

        isMissionActive = false;
    }

    public void StartMission(Missions mission)
    {
        switch (mission)
        {
            case Missions.MostWanted1:
                {
                    currentMission = new MissionOne(script, RELATIONSHIP_MISSION_NEUTRAL, this);
                    break;
                }
            case Missions.MostWanted2:
                {
                    currentMission = new MissionTwo(script, this, RELATIONSHIP_MISSION_NEUTRAL, RELATIONSHIP_MISSION_PEDESTRIAN);
                    break;
                }
            case Missions.MostWanted3:
                {
                    currentMission = new MissionThree(script, RELATIONSHIP_MISSION_NEUTRAL, RELATIONSHIP_MISSION_PEDESTRIAN, this);
                    break;
                }
            case Missions.MostWanted4:
                {
                    currentMission = new MissionFour(script, RELATIONSHIP_MISSION_AGGRESSIVE, this);
                    break;
                }
            case Missions.MostWanted5:
                {
                    currentMission = new MissionFive(script, this, RELATIONSHIP_MISSION_DISLIKE, RELATIONSHIP_MISSION_PEDESTRIAN);
                    break;
                }
            case Missions.MostWanted6:
                {
                    currentMission = new MissionSix(script, this, RELATIONSHIP_MISSION_AGGRESSIVE, RELATIONSHIP_MISSION_PEDESTRIAN);
                    break;
                }
            case Missions.MostWanted7:
                {
                    currentMission = new MissionSeven(script, this, RELATIONSHIP_MISSION_AGGRESSIVE, RELATIONSHIP_MISSION_NEUTRAL);
                    break;
                }
            case Missions.MostWanted8:
                {
                    currentMission = new MissionEight(script, this, RELATIONSHIP_MISSION_NEUTRAL, RELATIONSHIP_MISSION_AGGRESSIVE);
                    break;
                }
            case Missions.Assault:
                {
                    currentMission = new Assault(script, this, RELATIONSHIP_MISSION_AGGRESSIVE, RELATIONSHIP_MISSION_PEDESTRIAN);
                    break;
                }
            case Missions.StolenVehicle:
                {
                    currentMission = new StolenVehicle(script, this, RELATIONSHIP_MISSION_AGGRESSIVE);
                    break;
                }
            case Missions.GangActivity:
                {
                    currentMission = new GangActivity(script, this, RELATIONSHIP_MISSION_DISLIKE);
                    break;
                }
            case Missions.SuspectOnTheRun:
                {
                    currentMission = new SuspectOnTheRun(script, this, RELATIONSHIP_MISSION_AGGRESSIVE);
                    break;
                }
        }
        isMissionActive = currentMission.StartMission();
    }

    public void QuitMission()
    {
        GTA.UI.Screen.ShowSubtitle("~r~Vigilante mission canceled.");
        currentMission.QuitMission();
        isMissionActive = false;
        currentMission = null;
    }

    public void CompleteMission()
    {
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "MP_DM_COUNTDOWN_KILL");
        isMissionActive = false;
        currentMission = null;
    }
}

