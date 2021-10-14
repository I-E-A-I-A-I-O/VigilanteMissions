using GTA;
using GTA.Native;
using System.Windows.Forms;
using System;
using System.IO;
using System.Collections.Generic;

public class VigilanteMissions: Script
{
    bool isPlayerInStoppedPoliceVehicle = false;
    bool isMenuOpenable = false;
    bool isInStationComputer = false;
    bool rewardEnabled = false;
    bool filterEnabled = false;
    static Menu menu;
    public static Keys accessComputerKey;
    public static Keys interactMissionKey;
    public static Keys cancelMissionKey;
    string accessKey;
    string cancelKey;
    public static string interactKey;
    ScriptSettings iniFile;
    int startTime;
    int currentTime;
    public static int jokerMissionCount = 15;
    string[] vehicleModels = new string[200];
    List<VehicleHash> vehicleHashes = new List<VehicleHash>();

    public VigilanteMissions()
    {
        iniFile = ScriptSettings.Load("scripts\\VigilanteMissionsConfig.ini");
        accessKey = iniFile.GetValue("Controls", "AccessComputer", "E");
        cancelKey = iniFile.GetValue("Controls", "CancelMission", "F1");
        interactKey = iniFile.GetValue("Controls", "Interact", "E");
        rewardEnabled = iniFile.GetValue("Gameplay", "RewardEnabled", false);
        filterEnabled = iniFile.GetValue("Gameplay", "FilterOn", false);
        vehicleModels = iniFile.GetAllValues<string>("Gameplay", "VehicleModels");
        vehicleModels = vehicleModels[0].Split(',');

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

        for (var i = 0; i < vehicleModels.Length; i++)
        {
            if (!Enum.TryParse<VehicleHash>(vehicleModels[i], out var parsedHash))
            {
                continue;
            } else
            {
                vehicleHashes.Add(parsedHash);
            }
        }

        new MissionWorld(this);
        menu = new Menu();
        ReadProgress();
        AddJoker();

        Tick += (o, e) =>
        {
            menu.menuPool.Process();
            IsPlayerDead();
            IsPlayerIsInPoliceCar();
            IsPlayerInStationComputer();
            SetIsMenuOpenable();
            ShowOpenMenuMessage();
        };

        Tick += GamepadControls;
        KeyUp += KeyboardControls;
        Aborted += ScriptAborted;
    }

    void ScriptAborted(object o, EventArgs e)
    {
        if (MissionWorld.isMissionActive)
        {
            MissionWorld.QuitMission();
        }
    }

    void KeyboardControls(object o, KeyEventArgs e)
    {
        if (e.KeyCode == accessComputerKey && !menu.menuPool.AreAnyVisible && isMenuOpenable)
        {
            menu.mainMenu.Visible = true;
        }
        if (e.KeyCode == cancelMissionKey && MissionWorld.isMissionActive)
        {
            GTA.UI.Screen.ShowSubtitle("~r~Vigilante mission canceled.");
            MissionWorld.QuitMission();
        }
    }

    void GamepadControls(object o, EventArgs e)
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

            if (Game.IsControlJustReleased(GTA.Control.ScriptPadRight) && !menu.menuPool.AreAnyVisible && isMenuOpenable)
            {
                menu.mainMenu.Visible = true;
            }
    }

    void ShowOpenMenuMessage()
    {
        if (isMenuOpenable)
        {
            if (menu.menuPool.AreAnyVisible)
            {
                return;
            }
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
        } else
        {
            if (menu.menuPool.AreAnyVisible)
            {
                menu.menuPool.HideAll();
            }
        }
    }

    void SetIsMenuOpenable()
    {
        isMenuOpenable = isPlayerInStoppedPoliceVehicle || isInStationComputer;
    }

    void IsPlayerInStationComputer()
    {
        isInStationComputer = StationComputers.IsNearPoliceComputer(Game.Player.Character);
    }

    void IsPlayerIsInPoliceCar()
    {
        bool isInVehicle = false;
        if (Progress.jokerKilled && rewardEnabled)
        {
            if (filterEnabled)
            {
                var currentVehicle = Game.Player.Character.CurrentVehicle;
                if (currentVehicle != null)
                {
                    foreach (VehicleHash model in vehicleHashes)
                    {
                        if (currentVehicle.Model == new Model(model))
                        {
                            isInVehicle = true;
                        }
                    }
                } else
                {
                    isInVehicle = false;
                }
            } else
            {
                isInVehicle = Game.Player.Character.IsInVehicle();
            }
        } else
        {
            isInVehicle = Game.Player.Character.IsInPoliceVehicle;
        }
        isPlayerInStoppedPoliceVehicle = isInVehicle && Game.Player.Character.CurrentVehicle.IsStopped;
    }

    void IsPlayerDead()
    {
        if (Game.Player.IsDead && MissionWorld.isMissionActive)
        {
            MissionWorld.QuitMission();
            Progress.missionsFailedCount += 1;
            SaveProgress();
        }
    }

    public static void AddJoker()
    {
        if (Progress.jokerUnlocked && !menu.jokerAdded)
        {
            menu.AddJoker();
        }
    }

    public static void SaveProgress()
    {
        GTA.UI.LoadingPrompt.Show("Saving vigilante missions progress");
        try
        {
            var currDir = Directory.GetCurrentDirectory();
            var fileDir = $"{currDir}\\scripts\\VigilanteMissions\\progress.data";
            if (!Directory.Exists($"{currDir}\\scripts\\VigilanteMissions"))
            {
                Directory.CreateDirectory($"{currDir}\\scripts\\VigilanteMissions");
            }
            using (BinaryWriter writer = new BinaryWriter(File.Open(fileDir, FileMode.OpenOrCreate, FileAccess.Write)))
            {
                writer.Write(Progress.jokerUnlocked);
                writer.Write(Progress.jokerUnlockedMessageSent);
                writer.Write(Progress.completedMostWantedMissionsCount);
                writer.Write(Progress.jokerKilled);
                writer.Write(Progress.completedCurrentCrimesMissionsCount);
                writer.Write(Progress.enemiesKilledCount);
                writer.Write(Progress.missionsFailedCount);
            }
        } catch (Exception)
        {
            GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Vigilante missions", "Fuck, there was an error saving your Vigilante Missions progress. Any unsaved progress will be lost after you close the game.");
        } finally
        {
            Wait(2500);
            GTA.UI.LoadingPrompt.Hide();
        }
    }

    public static void ReadProgress()
    {
        try
        {
            var currDir = Directory.GetCurrentDirectory();
            var fileDir = $"{currDir}\\scripts\\VigilanteMissions\\progress.data";
            if (!File.Exists(fileDir))
            {
                Progress.jokerUnlocked = false;
                Progress.jokerUnlockedMessageSent = false;
                Progress.completedMostWantedMissionsCount = 0;
                Progress.jokerKilled = false;
                Progress.completedCurrentCrimesMissionsCount = 0;
                Progress.enemiesKilledCount = 0;
                Progress.missionsFailedCount = 0;
                return;
            }
            using (BinaryReader reader = new BinaryReader(File.OpenRead(fileDir)))
            {
                Progress.jokerUnlocked = reader.ReadBoolean();
                Progress.jokerUnlockedMessageSent = reader.ReadBoolean();
                Progress.completedMostWantedMissionsCount = reader.ReadInt32();
                Progress.jokerKilled = reader.ReadBoolean();
                Progress.completedCurrentCrimesMissionsCount = reader.ReadInt32();
                Progress.enemiesKilledCount = reader.ReadInt32();
                Progress.missionsFailedCount = reader.ReadInt32();
            }
        } catch(Exception)
        {
            Progress.jokerUnlocked = false;
            Progress.jokerUnlockedMessageSent = false;
            Progress.completedMostWantedMissionsCount = 0;
            Progress.jokerKilled = false;
            Progress.completedCurrentCrimesMissionsCount = 0;
            Progress.enemiesKilledCount = 0;
            Progress.missionsFailedCount = 0;
            //GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Vigilante missions", "I couldn't read your vigilante missions progress file. Your progress for the last most wanted is lost!");
        }
    }
}
