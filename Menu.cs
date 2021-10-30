using LemonUI;
using LemonUI.Menus;
using GTA;
using System;
using System.Collections.Generic;

class Menu
{
    public ObjectPool menuPool;
    public NativeMenu mainMenu;
    NativeMenu mostWantedMenu;
    NativeMenu currentCrimesMenu;
    NativeItem callBackupOption;
    NativeMenu statsMenu;
    NativeItem loseCopsOption;
    public bool jokerAdded = false;
    bool timerStarted = false;
    int startTime;
    int currentTime;
    bool firstTime = true;
    int lastTime;

    readonly Dictionary<int, int> wantedLevelPrices = new Dictionary<int, int>()
    {
        { 1, 0 },
        { 2, 1000 },
        { 3, 10000 },
        { 4, 25000 },
        { 5, 50000 }
    };

    readonly List<string> randomEvents = new List<string>()
    {
        "Stolen vehicle",
        "Assault",
        "Gang activity",
        "Suspect on the run",
        "Mass shooter",
        "Pacific Standard bank robbery",
        "Fleeca bank robbery"
    };

    public Menu()
    {
        menuPool = new ObjectPool();
        mainMenu = new NativeMenu("Police computer", "Los Santos county database");
        menuPool.Add(mainMenu);
        mostWantedMenu = new NativeMenu("Most Wanted", "Most wanted");
        menuPool.Add(mostWantedMenu);
        mainMenu.AddSubMenu(mostWantedMenu);
        var listOfItems = new List<NativeItem>()
        {
            new NativeItem("Wang Fang - Drug trafficking"),
            new NativeItem("Tony Mussolini - Racketeering"),
            new NativeItem("Martin Madrazo - Tax Evasion"),
            new NativeItem("Frank Abagnale - Check fraud"),
            new NativeItem("Ted Bundy - Murder"),
            new NativeItem("Song Jiang - Human Trafficking"),
            new NativeItem("Catherine Kerkow - Terrorism"),
            new NativeItem("Harry \"Taco\" Bowman - Racketeering"),
            new NativeItem("Heisenberg - Drug trafficking"),
            new NativeItem("Billy \"The Beaut\" Russo - Murder"),
        };
        for (var i = 0; i < listOfItems.Count; i++)
        {
            mostWantedMenu.Add(listOfItems[i]);
            var index = i;
            listOfItems[i].Activated += (o, e) =>
            {
                if (MissionWorld.isMissionActive)
                {
                    GTA.UI.Notification.Show("A mission is already in progress!");
                    return;
                }
                if (!Game.Player.CanStartMission)
                {
                    GTA.UI.Notification.Show("Mission not available.");
                    return;
                }
                MissionWorld.StartMission((MissionWorld.Missions)index);
            };
        }
        currentCrimesMenu = new NativeMenu("Current crimes", "Current crimes");
        menuPool.Add(currentCrimesMenu);
        mainMenu.AddSubMenu(currentCrimesMenu);
        callBackupOption = new NativeItem("Call backup", "Dispatch a police unit to your location");
        mainMenu.Add(callBackupOption);
        if (Progress.jokerKilled)
        {
            loseCopsOption = new NativeItem("Lose cops", "Pay Lester to remove your wanted level");
            mainMenu.Add(loseCopsOption);
            loseCopsOption.Activated += LoseCopsOption_Activated;
        }
        statsMenu = new NativeMenu("Stats", "Stats");
        menuPool.Add(statsMenu);
        mainMenu.AddSubMenu(statsMenu);
        statsMenu.Shown += StatsMenuShown;
        callBackupOption.Activated += BackUpCalled;
        currentCrimesMenu.Shown += CurrentCrimesMenu_Shown;
    }

    public void UpdateLoseCopsTitle()
    {
        if (loseCopsOption.Enabled && Game.Player.WantedLevel == 0)
        {
            loseCopsOption.Title = "Lose cops";
            loseCopsOption.Description = "Not available";
            loseCopsOption.Enabled = false;
        } else if (!loseCopsOption.Enabled && Game.Player.WantedLevel > 0)
        {
            if (firstTime)
            {
                goto Activate;
            } else if (Game.GameTime - lastTime >= 600000)
            {
                goto Activate;
            }

            Activate:
            {
                loseCopsOption.Enabled = true;
                loseCopsOption.Description = "Pay Lester to remove your wanted level";
            }
        } else if (loseCopsOption.Enabled && Game.Player.WantedLevel > 0)
        {
            loseCopsOption.Title = $"Lose cops - ${wantedLevelPrices[Game.Player.WantedLevel]}";
        }
    }

    private void LoseCopsOption_Activated(object sender, EventArgs e)
    {
        if (Game.Player.WantedLevel == 0)
        {
            return;
        }

        if (firstTime)
        {
            lastTime = Game.GameTime;
            firstTime = false;
            goto Process;
        } else if (Game.GameTime - lastTime >= 600000)
        {
            lastTime = Game.GameTime;
            goto Process;
        } else
        {
            GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Wanted Level", "I can't do that again so soon! you are on your own for now");
            return;
        }

        Process:
        {
            var price = wantedLevelPrices[Game.Player.WantedLevel];
            if (Game.Player.Money < price)
            {
                GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Wanted Level", "Can't work for free, you know? Call me back when you got the money");
                return;
            }
            Game.Player.Money -= price;
            GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Wanted Level", "Alright, i'll take care of it");
            Game.Player.WantedLevel = 0;
            menuPool.HideAll();
        }
    }

    public void AddJoker()
    {
        var item = new NativeItem("The Joker");
        mostWantedMenu.Add(item);
        item.Activated += (o, e) =>
        {
            if (MissionWorld.isMissionActive)
            {
                GTA.UI.Notification.Show("A mission is already in progress!");
                return;
            }
            if (!Game.Player.CanStartMission)
            {
                GTA.UI.Notification.Show("Mission not available.");
                return;
            }
            MissionWorld.StartMission(MissionWorld.Missions.MostWanted111);
        };
        jokerAdded = true;
    }

    void BackUpCalled(object o, EventArgs e)
    {
        if (!timerStarted)
        {
            new PoliceBackup();
            timerStarted = true;
            startTime = Game.GameTime;
        } else
        {
            currentTime = Game.GameTime;
            if (currentTime - startTime >= 60000)
            {
                timerStarted = false;
                new PoliceBackup();
            } else
            {
                GTA.UI.Notification.Show($"Next police backup available in {60 - ((currentTime - startTime) / 1000)} seconds");
            }
        }
    }

    void StatsMenuShown(object o, EventArgs e)
    {
        statsMenu.Clear();
        statsMenu.Add(new NativeItem($"Most wanted targets killed: {Progress.completedMostWantedMissionsCount}"));
        statsMenu.Add(new NativeItem($"Crime scenes cleared: {Progress.completedCurrentCrimesMissionsCount}"));
        statsMenu.Add(new NativeItem($"Total missions completed: {Progress.completedCurrentCrimesMissionsCount + Progress.completedMostWantedMissionsCount}"));
        statsMenu.Add(new NativeItem($"Missions failed: {Progress.missionsFailedCount}"));
        statsMenu.Add(new NativeItem($"Enemies killed: {Progress.enemiesKilledCount}"));
    }

    void CurrentCrimesMenu_Shown(object sender, EventArgs e)
    {
        var random = new Random();
        currentCrimesMenu.Clear();
        for (var i = 0; i < 3; i++)
        {
            var text = randomEvents[random.Next(0, randomEvents.Count)];
            var item = new NativeItem(text);
            currentCrimesMenu.Add(item);
            item.Activated += (o, ev) =>
            {
                if (MissionWorld.isMissionActive)
                {
                    GTA.UI.Notification.Show("A mission is already in progress!");
                    return;
                }
                if (!Game.Player.CanStartMission)
                {
                    GTA.UI.Notification.Show("Mission not available.");
                    return;
                }
                switch (text)
                {
                    case "Suspect on the run":
                        {
                            MissionWorld.StartMission(MissionWorld.Missions.SuspectOnTheRun);
                            break;
                        }
                    case "Assault":
                        {
                            MissionWorld.StartMission(MissionWorld.Missions.Assault);
                            break;
                        }
                    case "Gang activity":
                        {
                            MissionWorld.StartMission(MissionWorld.Missions.GangActivity);
                            break;
                        }
                    case "Stolen vehicle":
                        {
                            MissionWorld.StartMission(MissionWorld.Missions.StolenVehicle);
                            break;
                        }
                    case "Mass shooter":
                        {
                            MissionWorld.StartMission(MissionWorld.Missions.MassShooter);
                            break;
                        }
                    case "Pacific Standard bank robbery":
                        {
                            MissionWorld.StartMission(MissionWorld.Missions.PacificStandard);
                            break;
                        }
                    case "Fleeca bank robbery":
                        {
                            MissionWorld.StartMission(MissionWorld.Missions.FleecaBank);
                            break;
                        }
                }
            };
        }
    }
}
