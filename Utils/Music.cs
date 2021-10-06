using GTA;
using GTA.Native;
using System;

static class Music
{
    public static void StopMusic()
    {
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "AC_STOP");
    }
    public static void IncreaseIntensity()
    {
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "FIN1_SHOOTOUT_4");
    }
    public static void LowerIntensinty()
    {
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "fbi4_SHOOTOUT_MID_MA");
    }
    public static void StartHeistMusic()
    {
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "MP_MC_START_HEIST_4");
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "fbi4_SHOOTOUT_MID_MA");
    }
    public static void StartTedBundyMusic()
    {
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "FBI1_SHOOTOUT_HALFWAY_RT");
    }
    public static void StartCityMusic()
    {
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "MP_MC_START");
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "MP_MC_START_CITY");
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "fbi4_SHOOTOUT_MID_MA");
    }
    public static void StartGeneralMusic()
    {
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "MP_MC_START");
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "MP_MC_START_GENERAL_1");
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "fbi4_SHOOTOUT_MID_MA");
    }
    public static void StartCountry()
    {
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "MP_MC_START");
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "MP_MC_START_COUNTRY");
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "fbi4_SHOOTOUT_MID_MA");
    }
    public static void StartFunky()
    {
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "MP_MC_START");
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "MP_MC_START_FUNKY_JAM_3");
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "fbi4_SHOOTOUT_MID_MA");
    }
    public static void StartFunkyTwo()
    {
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "MP_MC_START");
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "MP_MC_START_FUNKY_JAM_TWO_4");
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "fbi4_SHOOTOUT_MID_MA");
    }
    public static void PlayMissionCompleted()
    {
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "MP_DM_COUNTDOWN_KILL");
    }

    public static void Play30SecCountDown()
    {
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "MP_DM_COUNTDOWN_30_SEC");
    }
}
