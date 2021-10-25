using GTA;
using GTA.Math;
using System;
using System.Collections.Generic;

static class FleecaRobberiesInit
{
    static Vector3 LOCATION_ONE = new Vector3(-2969.897f, 483.3021f, 14.46835f);
    static Vector3 LOCATION_TWO = new Vector3(-349.3697f, -44.89134f, 48.04182f);
    static Vector3 LOCATION_THREE = new Vector3(314.4158f, -281.3477f, 54.16473f);
    static Vector3 LOCATION_FOUR = new Vector3(-1215.664f, -325.0962f, 36.68665f);
    static Vector3 LOCATION_FIVE = new Vector3(151.6956f, -1035.145f, 28.33931f);
    static Vector3 LOCATION_SIX = new Vector3(1175.124f, 2701.105f, 37.17781f);

    public static List<Model> Models => new List<Model>()
    {
        new Model(1822283721),
        new Model(-625565461)
    };

    public static void SelectLocation(int missionIndex, out Vector3 location)
    {
        switch(missionIndex)
        {
            case 0:
                {
                    location = LOCATION_ONE;
                    break;
                }
            case 1:
                {
                    location = LOCATION_TWO;
                    break;
                }
            case 2:
                {
                    location = LOCATION_THREE;
                    break;
                }
            case 3:
                {
                    location = LOCATION_FOUR;
                    break;
                }
            case 4:
                {
                    location = LOCATION_FIVE;
                    break;
                }
            case 5:
                {
                    location = LOCATION_SIX;
                    break;
                }
            default:
                {
                    location = Vector3.Zero;
                    break;
                }
        }
    }

    public static List<Ped> SelectLocationRobbers(int missionIndex)
    {
        List<Ped> enemyPeds;
        switch (missionIndex)
        {
            case 0:
                {
                    enemyPeds = InitializeLocationOneRobbers();
                    break;
                }
            case 1:
                {
                    enemyPeds = InitializeLocationTwoRobbers();
                    break;
                }
            case 2:
                {
                    enemyPeds = InitializeLocationThreeRobbers();
                    break;
                }
            case 3:
                {
                    enemyPeds = InitializeLocationFourRobbers();
                    break;
                }
            case 4:
                {
                    enemyPeds = InitializeLocationFiveRobbers();
                    break;
                }
            case 5:
                {
                    enemyPeds = InitializeLocationSixRobbers();
                    break;
                }
            default:
                {
                    enemyPeds = new List<Ped>();
                    break;
                }
        }
        return enemyPeds;
    }

    public static List<Ped> SelectLocationHostages(int missionIndex)
    {
        List<Ped> hostagePeds;
        switch (missionIndex)
        {
            case 0:
                {
                    hostagePeds = InitializeLocationOneHostages();
                    break;
                }
            case 1:
                {
                    hostagePeds = InitializeLocationTwoHostages();
                    break;
                }
            case 2:
                {
                    hostagePeds = InitializeLocationThreeHostages();
                    break;
                }
            case 3:
                {
                    hostagePeds = InitializeLocationFourHostages();
                    break;
                }
            case 4:
                {
                    hostagePeds = InitializeLocationFiveHostages();
                    break;
                }
            case 5:
                {
                    hostagePeds = InitializeLocationSixHostages();
                    break;
                }
            default:
                {
                    hostagePeds = new List<Ped>();
                    break;
                }
        }
        return hostagePeds;
    }

    static List<Ped> InitializeLocationOneRobbers()
    {
        return new List<Ped>()
        {
            World.CreatePed(new Model(1822283721), new Vector3(-2964.18f, 481.0777f, 15.69693f), 30.00155f),
            World.CreatePed(new Model(1822283721), new Vector3(-2963.857f, 484.278f, 15.697f), 125.999f),
            World.CreatePed(new Model(1822283721), new Vector3(-2958.51f, 477.1414f, 15.69691f), 26.05981f),
            World.CreatePed(new Model(1822283721), new Vector3(-2957.031f, 480.7106f, 15.70944f), 9.649774f),
            World.CreatePed(new Model(1822283721), new Vector3(-2964.271f, 477.429f, 15.69697f), -7.861593f)
        };
    }

    static List<Ped> InitializeLocationOneHostages()
    {
        return new List<Ped>()
        {
            World.CreatePed(new Model(-625565461), new Vector3(-2960.853f, 481.0599f, 15.69693f), 95.13031f),
            World.CreatePed(new Model(-625565461), new Vector3(-2960.613f, 483.9062f, 15.69703f), 94.10081f)
        };
    }

    static List<Ped> InitializeLocationTwoRobbers()
    {
        return new List<Ped>()
        {
            World.CreatePed(new Model(1822283721), new Vector3(-348.1603f, -49.06974f, 49.03651f), 48.98949f),
            World.CreatePed(new Model(1822283721), new Vector3(-352.2792f, -47.55939f, 49.03641f), -66.99968f),
            World.CreatePed(new Model(1822283721), new Vector3(-355.5144f, -47.00598f, 49.03641f), -103.9994f),
            World.CreatePed(new Model(1822283721), new Vector3(-357.8593f, -52.98608f, 49.0364f), -49.93279f),
            World.CreatePed(new Model(1822283721), new Vector3(-353.8931f, -54.31932f, 49.04078f), -91.17307f)
        };
    }

    static List<Ped> InitializeLocationTwoHostages()
    {
        return new List<Ped>()
        {
            World.CreatePed(new Model(-625565461), new Vector3(-350.4429f, -51.97402f, 49.03652f), -3.043174f),
            World.CreatePed(new Model(-625565461), new Vector3(-352.5433f, -51.1286f, 49.03647f), 1.822526f)
        };
    }

    static List<Ped> InitializeLocationThreeRobbers()
    {
        return new List<Ped>()
        {
            World.CreatePed(new Model(1822283721), new Vector3(311.6597f, -276.2448f, 54.1646f), -74.00441f),
            World.CreatePed(new Model(1822283721), new Vector3(316.7694f, -278.3633f, 54.16472f), 30.99782f),
            World.CreatePed(new Model(1822283721), new Vector3(307.5851f, -281.2281f, 54.16462f), -51.94263f),
            World.CreatePed(new Model(1822283721), new Vector3(309.4192f, -276.0536f, 54.16466f), -101.5581f),
            World.CreatePed(new Model(1822283721), new Vector3(310.9105f, -283.3813f, 54.17715f), -102.6255f)
        };
    }

    static List<Ped> InitializeLocationThreeHostages()
    {
        return new List<Ped>()
        {
            World.CreatePed(new Model(-625565461), new Vector3(314.4158f, -281.3477f, 54.16473f), 21.91808f),
            World.CreatePed(new Model(-625565461), new Vector3(312.0094f, -280.3156f, 54.16463f), -7.192459f)
        };
    }

    static List<Ped> InitializeLocationFourRobbers()
    {
        return new List<Ped>()
        {
            World.CreatePed(new Model(1822283721), new Vector3(-1215.184f, -330.0213f, 37.78087f), -15.79769f),
            World.CreatePed(new Model(1822283721), new Vector3(-1211.406f, -328.4123f, 37.78097f), 66.99969f),
            World.CreatePed(new Model(1822283721), new Vector3(-1217.893f, -331.9763f, 37.78086f), -48.88696f),
            World.CreatePed(new Model(1822283721), new Vector3(-1215.417f, -337.4013f, 37.78086f), -5.066119E-07f),
            World.CreatePed(new Model(1822283721), new Vector3(-1211.546f, -336.2029f, 37.79304f), -33.99992f)
        };
    }

    static List<Ped> InitializeLocationFourHostages()
    {
        return new List<Ped>()
        {
            World.CreatePed(new Model(-625565461), new Vector3(-1210.526f, -331.9603f, 37.78099f), 36.9999f),
            World.CreatePed(new Model(-625565461), new Vector3(-1212.919f, -333.1217f, 37.78091f), 17.99998f)
        };
    }

    static List<Ped> InitializeLocationFiveRobbers()
    {
        return new List<Ped>()
        {
            World.CreatePed(new Model(1822283721), new Vector3(152.3929f, -1039.924f, 29.36801f), 47.60934f),
            World.CreatePed(new Model(1822283721), new Vector3(147.5129f, -1038.305f, 29.36789f), -73.00114f),
            World.CreatePed(new Model(1822283721), new Vector3(143.2488f, -1043.078f, 29.3679f), -42.98598f),
            World.CreatePed(new Model(1822283721), new Vector3(145.0282f, -1037.781f, 29.36794f), -92.99951f),
            World.CreatePed(new Model(1822283721), new Vector3(146.5712f, -1045.173f, 29.38041f), -110.8552f)
        };
    }

    static List<Ped> InitializeLocationFiveHostages()
    {
        return new List<Ped>()
        {
            World.CreatePed(new Model(-625565461), new Vector3(147.9353f, -1042.076f, 29.36794f), 0.0002265055f),
            World.CreatePed(new Model(-625565461), new Vector3(149.421f, -1042.728f, 29.36801f), 1.072878f)
        };
    }

    static List<Ped> InitializeLocationSixRobbers()
    {
        return new List<Ped>()
        {
            World.CreatePed(new Model(1822283721), new Vector3(1173.477f, 2705.483f, 38.08796f), -119.4031f),
            World.CreatePed(new Model(1822283721), new Vector3(1177.076f, 2705.032f, 38.08787f), 120.9997f),
            World.CreatePed(new Model(1822283721), new Vector3(1180.469f, 2705.364f, 38.08789f), 79.99958f),
            World.CreatePed(new Model(1822283721), new Vector3(1180.707f, 2711.697f, 38.08786f), 104.9145f),
            World.CreatePed(new Model(1822283721), new Vector3(1176.749f, 2711.792f, 38.10051f), 83.0192f)
        };
    }

    static List<Ped> InitializeLocationSixHostages()
    {
        return new List<Ped>()
        {
            World.CreatePed(new Model(-625565461), new Vector3(1175.03f, 2708.574f, 38.08796f), 170.949f),
            World.CreatePed(new Model(-625565461), new Vector3(1176.591f, 2708.643f, 38.08788f), -166.713f)
        };
    }
}
