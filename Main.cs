using GTA;
using GTA.Native;
using System.Windows.Forms;
using System;

public class VigilanteMissions: Script
{
    bool isPlayerInPoliceVehicle = false;
    Vehicle currentPoliceVehicle;
    Menu menu;
    public static Keys accessComputerKey;
    public static Keys interactMissionKey;
    public static Keys cancelMissionKey;
    string accessKey;
    string cancelKey;
    public static string interactKey;
    ScriptSettings iniFile;
    int startTime;
    int currentTime;

    public VigilanteMissions()
    {
        iniFile = ScriptSettings.Load("scripts\\VigilanteMissionsConfig.ini");
        accessKey = iniFile.GetValue("Controls", "AccessComputer", "E");
        cancelKey = iniFile.GetValue("Controls", "CancelMission", "F1");
        interactKey = iniFile.GetValue("Controls", "Interact", "E");

        if (!Enum.TryParse(accessKey, out accessComputerKey))
        {
            accessKey = "E";
            accessComputerKey = Keys.E;
            GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Vigilante missions", "I couldn't set up the key for accessing the police computer so the default key ~g~E~w~ is being used. Make sure you didn't fuck up something in the ~y~ini file~w~.");
        }
        if (!Enum.TryParse(cancelKey, out cancelMissionKey))
        {
            cancelKey = "F1";
            cancelMissionKey = Keys.F1;
            GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Vigilante missions", "I couldn't set up the key for cancelling the mission so the default key ~g~F1~w~ is being used. Make sure you didn't fuck up something in the ~y~ini file~w~.");
        }
        if (!Enum.TryParse(interactKey, out interactMissionKey))
        {
            interactKey = "E";
            interactMissionKey = Keys.E;
            GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Vigilante missions", "I couldn't set up the key for interacting so the default key ~g~E~w~ is being used. Make sure you didn't fuck up something in the ~y~ini file~w~.");
        }

        new MissionWorld(this);
        menu = new Menu();

        Tick += (o, e) =>
        {
            menu.menuPool.Process();
            CheckIfPlayerIsInPoliceCar();
            CheckIfPlayerDied();
        };

        Tick += (o, e) =>
        {
            if (Game.LastInputMethod == InputMethod.MouseAndKeyboard)
            {
                return;
            }

            if (Game.IsControlJustPressed(GTA.Control.ScriptPadLeft) && MissionWorld.isMissionActive)
            {
                startTime = Game.GameTime;
            }

            if (Game.IsControlPressed(GTA.Control.ScriptPadLeft) && MissionWorld.isMissionActive)
            {
                currentTime = Game.GameTime - startTime;
                if (currentTime >= 3000)
                {
                    GTA.UI.Screen.ShowSubtitle("~r~Vigilante mission canceled.");
                    MissionWorld.QuitMission();
                }
            }

            if (Game.IsControlJustReleased(GTA.Control.ScriptPadRight) && !menu.menuPool.AreAnyVisible && isPlayerInPoliceVehicle && currentPoliceVehicle.IsStopped)
            {
                menu.mainMenu.Visible = true;
            }
        };

        KeyUp += (o, e) =>
        {
            if (e.KeyCode == accessComputerKey && !menu.menuPool.AreAnyVisible && isPlayerInPoliceVehicle && currentPoliceVehicle.IsStopped)
            {
                menu.mainMenu.Visible = true;
            }
            if (e.KeyCode == cancelMissionKey && MissionWorld.isMissionActive)
            {
                GTA.UI.Screen.ShowSubtitle("~r~Vigilante mission canceled.");
                MissionWorld.QuitMission();
            }
        };
    }

    void CheckIfPlayerIsInPoliceCar()
    {
        isPlayerInPoliceVehicle = Function.Call<bool>(Hash.IS_PED_IN_ANY_POLICE_VEHICLE, Game.Player.Character);
        if (isPlayerInPoliceVehicle)
        {
            currentPoliceVehicle = Function.Call<Vehicle>(Hash.GET_VEHICLE_PED_IS_USING, Game.Player.Character);
            if (currentPoliceVehicle.IsStopped)
            {
                switch(Game.LastInputMethod)
                {
                    case InputMethod.MouseAndKeyboard:
                        {
                            GTA.UI.Screen.ShowHelpTextThisFrame($"Press {accessKey.ToUpper()} to access the police computer");
                            break;
                        }
                    case InputMethod.GamePad:
                        {
                            GTA.UI.Screen.ShowHelpTextThisFrame($"Press DPad Right to access the police computer");
                            break;
                        }
                }
            }
        } else
        {
            if (menu.menuPool.AreAnyVisible)
            {
                menu.menuPool.HideAll();
                menu.mainMenu.Visible = false;
            }
        }
    }

    void CheckIfPlayerDied()
    {
        if (Game.Player.IsDead && MissionWorld.isMissionActive)
        {
            MissionWorld.QuitMission();
        }
    }
}
