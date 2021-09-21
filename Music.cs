using GTA;
using GTA.Native;
using System;

class Music
{
    public void StopMusic()
    {
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "AC_STOP");
    }
    public void IncreaseIntensity()
    {
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "FIN1_SHOOTOUT_4");
    }
    public void StartHeistMusic()
    {
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "MP_MC_START_HEIST_4");
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "fbi4_SHOOTOUT_MID_MA");
    }
    public void StartTedBundyMusic()
    {
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "FBI1_SHOOTOUT_HALFWAY_RT");
    }
    public void StartCityMusic()
    {
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "MP_MC_START");
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "MP_MC_START_CITY");
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "fbi4_SHOOTOUT_MID_MA");
    }
    public void StartGeneralMusic()
    {
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "MP_MC_START");
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "MP_MC_START_GENERAL_1");
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "fbi4_SHOOTOUT_MID_MA");
    }
    public void StartCountry()
    {
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "MP_MC_START");
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "MP_MC_START_COUNTRY");
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "fbi4_SHOOTOUT_MID_MA");
    }
    public void StartFunky()
    {
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "MP_MC_START");
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "MP_MC_START_FUNKY_JAM_3");
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "fbi4_SHOOTOUT_MID_MA");
    }
    public void StartFunkyTwo()
    {
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "MP_MC_START");
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "MP_MC_START_FUNKY_JAM_TWO_4");
        Function.Call(Hash.TRIGGER_MUSIC_EVENT, "fbi4_SHOOTOUT_MID_MA");
    }
}
