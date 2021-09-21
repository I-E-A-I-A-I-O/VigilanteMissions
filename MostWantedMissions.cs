using GTA;
using GTA.Math;
using System;
using System.Collections.Generic;

class MostWantedMissions
{
    public readonly Vector3 MISSION_ONE_LOCATION = new Vector3(-356.5191f, -61.93171f, 54f);
    public readonly Vector3 MISSION_TWO_LOCATION = new Vector3(-2075.674f, -1020.837f, 4.88413f);
    public readonly Vector3 MISSION_THREE_LOCATION = new Vector3(1400.261f, 1147.438f, 118.3301f);
    public readonly Vector3 MISSION_FOUR_LOCATION = new Vector3(-1262.259f, -3361.105f, 12.9450493f);
    public readonly Vector3 MISSION_FOUR_FAIL_LOCATION = new Vector3(3557.105f, 3919.986f, 711.1465f);
    public readonly Vector3 MISSION_FIVE_LOCATION = new Vector3(-206.912659f, 1548.97009f, 320.199738f);
    public readonly Vector3 MISSION_SIX_LOCATION = new Vector3(3025.3f, 2071.449f, 3.107029f);
    public readonly Vector3 MISSION_SEVEN_START_LOCATION = new Vector3(-54.003788f, -778.6506f, 43.257515f);
    public readonly Vector3 MISSION_SEVEN_OFFICE_LOCATION = new Vector3(-75.8502655f, -824.573853f, 242.385849f);
    public readonly Vector3 MISSION_SEVEN_ROOF_LOCATION = new Vector3(-65.0182f, -822.9397f, 320.289f);
    public readonly Vector3 MISSION_SEVEN_EXPLOSION_LOCATION = new Vector3(-2057.91821f, -1749.02991f, -3.73062015f);
    public readonly Vector3 MISSION_EIGHT_LOCATON = new Vector3(958.260742f, -143.733139f, 73.4963f);

    public List<Ped> IntializeMissionOnePeds()
    {
        return new List<Ped>
        {
            World.CreatePed(new Model(-1176698112), new Vector3(-384.9535f, -89.48703f, 54.42385f), -9.292741f),
            World.CreatePed(new Model(-9308122), new Vector3(-391.0056f, -89.59114f, 54.42527f), -24.26937f),
            World.CreatePed(new Model(-9308122), new Vector3(-392.5017f, -88.84636f, 54.42527f), -20.79102f),
            World.CreatePed(new Model(-9308122), new Vector3(-381.1948f, -86.44048f, 54.42526f), 24.15284f),
            World.CreatePed(new Model(-9308122), new Vector3(-392.8951f, -64.03995f, 55.79881f), 47.97223f),
            World.CreatePed(new Model(-9308122), new Vector3(-366.7247f, -57.3772f, 54.41915f), 176.8367f),
            World.CreatePed(new Model(-9308122), new Vector3(-362.2732f, -59.05697f, 54.4193f), -169.0289f),
            World.CreatePed(new Model(-9308122), new Vector3(-345.1667f, -56.89451f, 54.41919f), -83.99975f),
            World.CreatePed(new Model(-9308122), new Vector3(-344.9188f, -54.98076f, 54.41921f), -147.9862f),
            World.CreatePed(new Model(-9308122), new Vector3(-342.8146f, -57.18469f, 54.41929f), 44.56654f),
            World.CreatePed(new Model(-9308122), new Vector3(-350.1284f, -46.48768f, 54.42538f), -8.606504f),
            World.CreatePed(new Model(-9308122), new Vector3(-313.5885f, -78.25857f, 54.42533f), -85.72867f),
            World.CreatePed(new Model(-9308122), new Vector3(-315.5716f, -84.64844f, 54.42538f), -54.99984f)
    };
    }
    public List<Vehicle> InitializeMissionOneVehicles()
    {
        return new List<Vehicle>
        {
            World.CreateVehicle(new Model(65402552), new Vector3(-386.1818f, -92.67248f, 53.92717f), -171.5226f)
        };
    }
    public List<Ped> InitializeMissionTwoPeds()
    {
        return new List<Ped>
        {
            World.CreatePed(new Model(-412008429), new Vector3(-2108.266f, -1016.047f, 8.9718f), 0f),
            World.CreatePed(new Model(-412008429), new Vector3(-2105.627f, -1004.96f, 8.972195f), 152.007f),
            World.CreatePed(new Model(-39239064), new Vector3(-2110.015f, -1009.474f, 8.967435f), -113.8403f),
            World.CreatePed(new Model(-236444766), new Vector3(-2095.083f, -1020.348f, 8.972371f), 121.5841f),
            World.CreatePed(new Model(-39239064), new Vector3(-2093.168f, -1016.17f, 8.980458f), 12.00086f),
            World.CreatePed(new Model(1535236204), new Vector3(-2092.133f, -1020.985f, 8.969188f), -10.27278f),
            World.CreatePed(new Model(-412008429), new Vector3(-2085.525f, -1016.216f, 8.971098f), 110.6088f),
            World.CreatePed(new Model(-984709238), new Vector3(-2081.607f, -1020.792f, 8.971145f), -70.96234f),
            World.CreatePed(new Model(-984709238), new Vector3(-2080.355f, -1017.989f, 8.971145f), -120.9996f),
            World.CreatePed(new Model(-413447396), new Vector3(-2085.273f, -1017.958f, 12.7819f), 58.85679f),
            World.CreatePed(new Model(-39239064), new Vector3(-2071.25f, -1021.291f, 11.91093f), -116.1681f),
            World.CreatePed(new Model(-39239064), new Vector3(-2106.087f, -1010.83f, 5.885402f), 164.6348f),
            World.CreatePed(new Model(-236444766), new Vector3(-2053.446f, -1032.708f, 8.971491f), 44.60005f),
            World.CreatePed(new Model(-236444766), new Vector3(-2055.187f, -1028.713f, 8.971491f), -38.99986f),
            World.CreatePed(new Model(-236444766), new Vector3(-2054.666f, -1027.393f, 8.971491f), -139.9988f),
            World.CreatePed(new Model(-236444766), new Vector3(-2098.274f, -1014.407f, 5.884346f), -111.1938f),
            World.CreatePed(new Model(-412008429), new Vector3(-2096.059f, -1018.372f, 5.884164f), -179.2764f),
            World.CreatePed(new Model(-39239064), new Vector3(-2072.322f, -1019.197f, 5.88413f), 66.99957f),
            World.CreatePed(new Model(-984709238), new Vector3(-2036.194f, -1033.776f, 8.971484f), -104.2428f),
            World.CreatePed(new Model(-984709238), new Vector3(-2033.164f, -1042.431f, 5.882326f), 47.5585f),
            World.CreatePed(new Model(-984709238), new Vector3(-2034.393f, -1042.101f, 5.882571f), -47.96439f),
            World.CreatePed(new Model(-984709238), new Vector3(-2018.398f, -1039.799f, 2.446023f), -94.58332f),
            World.CreatePed(new Model(-984709238), new Vector3(-2081.947f, -1019.532f, 8.971145f), -103.9729f),
            World.CreatePed(new Model(-984709238), new Vector3(-2081.561f, -1018.354f, 8.971145f), -127.0124f)
        };
    }
    public List<Ped> InitializeMissionTwoCivilianPeds()
    {
        return new List<Ped>
        {
            World.CreatePed(new Model(42647445), new Vector3(-2106.914f, -1015.075f, 8.971136f), 117.6287f),
            World.CreatePed(new Model(42647445), new Vector3(-2108.665f, -1014.475f, 8.971003f), -147.2106f),
            World.CreatePed(new Model(42647445), new Vector3(-2104.866f, -1005.519f, 8.972008f), 101.7841f),
            World.CreatePed(new Model(695248020), new  Vector3(-2106.418f, -1011.327f, 10.00431f), 71.78985f),
            World.CreatePed(new Model(1381498905), new Vector3(-2106.218f, -1010.064f, 9.925285f), 104.7895f),
            World.CreatePed(new Model(2014052797), new Vector3(-2094.614f, -1014.906f, 8.980458f), -108.951f),
            World.CreatePed(new Model(1381498905), new Vector3(-2095.372f, -1015.403f, 5.884346f), 69.90386f),
            World.CreatePed(new Model(348382215), new  Vector3(-2073.048f, -1018.083f, 5.88413f), -131.9684f),
            World.CreatePed(new Model(348382215), new  Vector3(-2073.715f, -1019.312f, 5.88413f), -79.23396f),
            World.CreatePed(new Model(1544875514), new Vector3(-2078.302f, -1020.068f, 8.971491f), 73.27193f)
        };
    }
    public List<Vehicle> InitializeMissionTwoVehicles()
    {
        return new List<Vehicle>
        {
            World.CreateVehicle(new Model(-1660661558), new Vector3(-2042.405f, -1031.714f, 12.08313f), 76.36571f)
        };
    }
    public List<Ped> InitializeMissionThreePeds()
    {
        return new List<Ped>()
        {
            World.CreatePed(new Model(1129928304), new Vector3(1393.479f, 1147.862f, 114.3304f), -24.69588f),
            World.CreatePed(new Model(1329576454), new Vector3(1391.24f, 1140.197f, 114.4404f), 84.99852f),
            World.CreatePed(new Model(1329576454), new Vector3(1388.984f, 1131.304f, 114.3344f), 41.71875f),
            World.CreatePed(new Model(1329576454), new Vector3(1392.983f, 1156.143f, 114.4433f), 87.99777f),
            World.CreatePed(new Model(1329576454), new Vector3(1391.74f, 1157.945f, 114.4433f), 175.998f),
            World.CreatePed(new Model(1329576454), new Vector3(1403.695f, 1177.2f, 114.2767f), 91.99952f),
            World.CreatePed(new Model(1329576454), new Vector3(1402.6f, 1175.145f, 114.2508f), 43.01424f),
            World.CreatePed(new Model(1329576454), new Vector3(1401.618f, 1178.034f, 114.2659f), -158.9979f),
            World.CreatePed(new Model(-1561829034), new Vector3(1412.272f, 1165.354f, 114.674f), -150.0264f),
            World.CreatePed(new Model(-1561829034), new Vector3(1419.938f, 1154.062f, 114.674f), -117.9991f),
            World.CreatePed(new Model(1329576454), new Vector3(1420.062f, 1152.999f, 114.674f), -28.79289f),
            World.CreatePed(new Model(1329576454), new Vector3(1417.521f, 1136.334f, 114.3341f), -131.999f),
            World.CreatePed(new Model(1329576454), new Vector3(1406.172f, 1140.734f, 114.4431f), -86.13324f),
            World.CreatePed(new Model(1329576454), new Vector3(1405.867f, 1137.871f, 114.4431f), -71.43306f),
            World.CreatePed(new Model(1329576454), new Vector3(1407.629f, 1136.913f, 114.4431f), 0.0006305005f),
            World.CreatePed(new Model(1329576454), new Vector3(1413.055f, 1114.972f, 114.838f), 1.127281E-07f),
            World.CreatePed(new Model(1329576454), new Vector3(1413.933f, 1116.52f, 114.838f), 137.7389f),
            World.CreatePed(new Model(1329576454), new Vector3(1401.467f, 1123.096f, 114.838f), 109.8054f),
            World.CreatePed(new Model(1329576454), new Vector3(1385.149f, 1138.731f, 114.3344f), 45.99967f)
    };
    }
    public List<Ped> InitializeMissionThreeCivilianPeds()
    {
        return new List<Ped>()
        {
            World.CreatePed(new Model(-982642292), new Vector3(1395.002f, 1150.011f, 114.3336f), 8.999978f),
            World.CreatePed(new Model(261586155), new Vector3(1415.251f, 1165.195f, 114.3342f), 4.681286f)
        };
    }
    public List<Ped> InitializeMissionFourPeds()
    {
        return new List<Ped>()
        {
            World.CreatePed(new Model(PedHash.PoloGoon01GMY), new Vector3(-1279.253f, -3367.219f, 13.94115f), 10.22806f),
            World.CreatePed(new Model(PedHash.PoloGoon01GMY), new Vector3(-1280.593f, -3365.94f, 13.94114f), -54.99979f),
            World.CreatePed(new Model(PedHash.PoloGoon01GMY), new Vector3(-1278.305f, -3365.272f, 13.94115f), 90.99963f),
            World.CreatePed(new Model(PedHash.PoloGoon01GMY), new Vector3(-1279.451f, -3364.126f, 13.94115f), 173.998f),
            World.CreatePed(new Model(PedHash.PoloGoon01GMY), new Vector3(-1247.366f, -3373.423f, 13.94021f), -21.99963f),
            World.CreatePed(new Model(PedHash.PoloGoon01GMY), new Vector3(-1235.972f, -3387.076f, 24.32666f), 42.00459f),
            World.CreatePed(new Model(PedHash.PoloGoon01GMY), new Vector3(-1230.903f, -3377.111f, 13.94505f), -18.19806f),
            World.CreatePed(new Model(PedHash.PoloGoon01GMY), new Vector3(-1280.314f, -3340.25f, 13.94505f), -84.28448f),
        };
    }
    public List<Vehicle> InitializeMissionFourVehicles()
    {
        return new List<Vehicle>()
        {
            World.CreateVehicle(new Model(-1214293858), new Vector3(-1275.54f, -3388.459f, 14.54273f), -23.81056f),
            World.CreateVehicle(new Model(-394074634), new Vector3(-1290.425f, -3353.373f, 13.52801f), -95.03088f),
            World.CreateVehicle(new Model(-394074634), new Vector3(-1291.051f, -3359.197f, 13.52484f), -56.78358f),
            World.CreateVehicle(new Model(-394074634), new Vector3(-1287.795f, -3360.949f, 13.527f), -15.63847f)
        };
    }
    public List<Ped> InitializeMissionFivePeds()
    {
        return new List<Ped>()
        {
             World.CreatePed(new Model(216536661), new Vector3(-206.9127f, 1548.97f, 320.1997f), 67.99938f)
        };
    }
    public List<Ped> InitializeMissionFiveCivilianPeds()
    {
        return new List<Ped>()
        {
            World.CreatePed(new Model(1633872967), new Vector3(-211.8017f, 1551.514f, 320.8525f), 37.9999f)
        };
    }
    public List<Ped> InitializeMissionSixPeds()
    {
        return new List<Ped>()
        {
            World.CreatePed(new Model(-9308122), new Vector3(3023.153f, 2069.32f, 3.844249f), -56.99965f),
            World.CreatePed(new Model(-9308122), new Vector3(3023.333f, 2071.976f, 3.374274f), -55.99938f),
            World.CreatePed(new Model(-9308122), new Vector3(3036.506f, 2052.877f, 14.54304f), 40.19872f),
            World.CreatePed(new Model(-9308122), new Vector3(3029.119f, 2070.151f, 2.447087f), 54.99968f),
            World.CreatePed(new Model(-1176698112), new Vector3(3026.984f, 2072.936f, 2.496719f), 123.8273f),
            World.CreatePed(new Model(2119136831), new Vector3(3022.533f, 2081.284f, 2.795424f), -151.998f),
            World.CreatePed(new Model(2119136831), new Vector3(3022.148f, 2080.24f, 2.938469f), -68.99822f),
            World.CreatePed(new Model(-9308122), new Vector3(3013.816f, 2078.366f, 5.007706f), 136.9986f),
            World.CreatePed(new Model(-9308122), new Vector3(3028.909f, 2062.301f, 4.289201f), 28.07013f),
            World.CreatePed(new Model(-9308122), new Vector3(3019.032f, 2062.385f, 5.543861f), 77.9996f)
        };
    }
    public List<Ped> InitializeMissionSixCivilianPeds()
    {
        return new List<Ped>()
        {
            World.CreatePed(new Model(1064866854), new Vector3(3025.3f, 2071.449f, 3.107029f), -43.20979f),
            World.CreatePed(new Model(1064866854), new Vector3(3024.681f, 2072.474f, 3.091548f), -58.73301f),
            World.CreatePed(new Model(-1302522190), new Vector3(3025.913f, 2070.544f, 3.137019f), -36.99994f),
            World.CreatePed(new Model(-1302522190), new Vector3(3026.687f, 2069.837f, 3.101196f), -27.99905f),
            World.CreatePed(new Model(-1302522190), new Vector3(3024.213f, 2073.561f, 3.034813f), -71.07427f)
        };
    }
    public List<Vehicle> InitializeMissionSixVehicles()
    {
        return new List<Vehicle>()
        {
            World.CreateVehicle(new Model(231083307), new Vector3(3035.438f, 2079.784f, 0.1072293f), -77.93864f),
            World.CreateVehicle(new Model(231083307), new Vector3(3038.369f, 2073.003f, 0.2614459f), -49.30573f)
        };
    }

    public List<Ped> InitializeMissionSevenStreetPeds()
    {
        return new List<Ped>()
        {
            World.CreatePed(new Model(1822283721), new Vector3(-66.54572f, -796.516f, 44.22512f), -69.90891f),
            World.CreatePed(new Model(1822283721), new Vector3(-61.26125f, -799.1334f, 44.22731f), -80.00094f),
            World.CreatePed(new Model(1822283721), new Vector3(-72.70385f, -795.0867f, 44.22731f), -43.64849f),
            World.CreatePed(new Model(1822283721), new Vector3(-78.60847f, -794.9087f, 44.22731f), -48.21025f),
            World.CreatePed(new Model(1822283721), new Vector3(-57.20286f, -802.9669f, 44.22731f), -2.176433f)
        };
    }

    public List<Ped> InitializeMissionSevenPolice()
    {
        return new List<Ped>()
        {
            World.CreatePed(new Model(1558115333), new Vector3(-40.13184f, -782.5776f, 44.13447f), 152.9982f),
            World.CreatePed(new Model(1558115333), new Vector3(-37.51791f, -777.0939f, 44.22693f), 166.9976f),
            World.CreatePed(new Model(1558115333), new Vector3(-45.61734f, -780.9672f, 44.15178f), 148.2216f),
            World.CreatePed(new Model(-1920001264), new Vector3(-31.7812f, -789.6625f, 44.09612f), 41.81842f),
            World.CreatePed(new Model(-1920001264), new Vector3(-32.63076f, -787.6109f, 44.20688f), -142.9567f),
            World.CreatePed(new Model(-1920001264), new Vector3(-31.43986f, -788.192f, 44.16693f), 120.773f),
            World.CreatePed(new Model(-1920001264), new Vector3(-33.50474f, -789.2833f, 44.13837f), -52.22415f),
            World.CreatePed(new Model(1558115333), new Vector3(-42.73397f, -782.1128f, 44.14959f), 157.769f),
            World.CreatePed(new Model(1558115333), new Vector3(-38.17432f, -778.309f, 44.22774f), -45.50066f),
            World.CreatePed(new Model(1558115333), new Vector3(-36.93507f, -778.5262f, 44.22777f), 39.25488f),
            World.CreatePed(new Model(1558115333), new Vector3(-50.53336f, -778.1302f, 44.1537f), 130.999f),
            World.CreatePed(new Model(1558115333), new Vector3(-51.8546f, -777.5906f, 44.17066f), 162.4711f),
            World.CreatePed(new Model(1558115333), new Vector3(-53.76889f, -777.0287f, 44.2027f), 154.1908f)
        };
    }

    public List<Ped> InitializeMissionSevenOfficePeds()
    {
        return new List<Ped>()
        {
            World.CreatePed(new Model(1822283721), new Vector3(-73.2437f, -813.694f, 243.3858f), -149.5697f),
            World.CreatePed(new Model(1822283721), new Vector3(-72.1945f, -814.3579f, 243.3858f), 128.5591f),
            World.CreatePed(new Model(1822283721), new Vector3(-59.8345f, -806.2589f, 243.3858f), -30.99986f),
            World.CreatePed(new Model(1822283721), new Vector3(-61.89029f, -807.8169f, 243.3858f), -142.2706f),
            World.CreatePed(new Model(1822283721), new Vector3(-59.64361f, -809.4295f, 243.3858f), 47.71804f),
            World.CreatePed(new Model(1822283721), new Vector3(-80.56508f, -801.7592f, 243.4012f), -100.5803f),
            World.CreatePed(new Model(1822283721), new Vector3(-78.4592f, -805.9352f, 243.3858f), 112.4988f),
            World.CreatePed(new Model(1822283721), new Vector3(-79.54531f, -805.6289f, 243.3858f), -175.5277f),
            World.CreatePed(new Model(1822283721), new Vector3(-79.75951f, -810.9196f, 243.3858f), 68.01881f),
            World.CreatePed(new Model(1822283721), new Vector3(-65.01936f, -814.7318f, 243.3858f), 68.00429f),
            World.CreatePed(new Model(1822283721), new Vector3(-64.1201f, -820.8603f, 243.3858f), 94.84103f),
            World.CreatePed(new Model(1822283721), new Vector3(-65.00479f, -821.7914f, 243.3858f), -10.99999f)
        };
    }

    public List<Ped> InitializeMissionSevenRoofPeds()
    {
        return new List<Ped>()
        {
            World.CreatePed(new Model(1822283721), new Vector3(-77.58086f, -816.5815f, 326.1753f), -45.00298f),
            World.CreatePed(new Model(1822283721), new Vector3(-71.22124f, -815.9979f, 326.1753f), -14.00009f),
            World.CreatePed(new Model(1822283721), new Vector3(-72.10974f, -815.1895f, 326.1753f), -33.93996f),
            World.CreatePed(new Model(-1514497514), new Vector3(-72.27792f, -817.8143f, 326.1753f), 2.845267f)
        };
    }

    public List<Vehicle> InitializeMissionSevenStreetVehicles()
    {
        return new List<Vehicle>()
        {
            World.CreateVehicle(new Model(-1205689942), new Vector3(-35.02313f, -790.634f, 44.46696f), -133.5401f),
            World.CreateVehicle(new Model(1127131465), new Vector3(-41.70132f, -783.6806f, 44.51595f), -106.9076f),
            World.CreateVehicle(new Model(1127131465), new Vector3(-47.0255f, -781.5803f, 44.51566f), -142.9964f),
            World.CreateVehicle(new Model(1127131465), new Vector3(-52.60672f, -778.8976f, 44.5451f), -110.1463f)
        };
    }

    public List<Vehicle> InitializeMissionSevenRoofVehicles()
    {
        return new List<Vehicle>()
        {
            World.CreateVehicle(new Model(788747387), new Vector3(-74.93462f, -818.5611f, 326.0729f), -18.93448f)
        };
    }

    public List<Prop> InitializeMissionSevenBomb()
    {
        return new List<Prop>()
        {
            World.CreateProp(new Model(929047740), new Vector3(-80.2579651f, -803.0665f, 243.05f), new Vector3(-90f, 15.9453955f, 0f), true, false)
        };
    }

    public List<Ped> InitializeMissionEightPeds()
    {
        return new List<Ped>()
        {
            World.CreatePed(new Model(-44746786), new Vector3(978.6215f, -91.82484f, 74.84506f), -36.24286f),
            World.CreatePed(new Model(-44746786), new Vector3(977.8893f, -95.02743f, 74.8694f), -38.76103f),
            World.CreatePed(new Model(-44746786), new Vector3(978.7422f, -94.20804f, 74.87115f), 137.2159f),
            World.CreatePed(new Model(1330042375), new Vector3(977.7718f, -94.13734f, 74.871f), -137.0009f),
            World.CreatePed(new Model(1330042375), new Vector3(987.7917f, -95.32742f, 74.84563f), -146.8757f),
            World.CreatePed(new Model(1032073858), new Vector3(988.3394f, -96.576f, 74.84539f), 16.67232f),
            World.CreatePed(new Model(1032073858), new Vector3(984.0063f, -96.79572f, 74.8456f), -47.99984f),
            World.CreatePed(new Model(1032073858), new Vector3(985.0663f, -96.5265f, 74.84689f), 88.9995f),
            World.CreatePed(new Model(1032073858), new Vector3(990.7222f, -96.74388f, 74.84507f), 130.6996f),
            World.CreatePed(new Model(1330042375), new Vector3(990.3204f, -97.94606f, 74.84506f), 6.954981f),
            World.CreatePed(new Model(1330042375), new Vector3(974.512f, -95.53335f, 74.8456f), 35.81996f),
            World.CreatePed(new Model(1032073858), new Vector3(980.6508f, -99.2776f, 74.84505f), 124.7209f),
            World.CreatePed(new Model(1330042375), new Vector3(986.3696f, -103.8527f, 74.85172f), -139.4447f),
            World.CreatePed(new Model(-44746786), new Vector3(986.8973f, -104.7069f, 74.85168f), 22.988f),
            World.CreatePed(new Model(1032073858), new Vector3(978.5614f, -127.2611f, 74.04119f), 118.4248f),
            World.CreatePed(new Model(850468060), new Vector3(976.6624f, -130.9574f, 73.99924f), 115.5153f),
            World.CreatePed(new Model(-44746786), new Vector3(976.0014f, -132.691f, 73.95243f), 24.57909f),
            World.CreatePed(new Model(1032073858), new Vector3(974.8788f, -131.0484f, 74.11576f), -102.9994f),
            World.CreatePed(new Model(-44746786), new Vector3(971.469f, -117.2388f, 74.3531f), 25.79008f),
            World.CreatePed(new Model(-44746786), new Vector3(973.1587f, -115.5818f, 74.3531f), 43.99918f),
            World.CreatePed(new Model(1032073858), new Vector3(968.9072f, -115.178f, 74.3531f), -136.71f),
            World.CreatePed(new Model(1330042375), new Vector3(970.0566f, -113.9088f, 74.3531f), -136.6793f),
            World.CreatePed(new Model(1330042375), new Vector3(971.4061f, -113.0788f, 74.3531f), -148.9985f),
            World.CreatePed(new Model(1330042375), new Vector3(978.986f, -110.7195f, 74.3531f), -42.56275f),
            World.CreatePed(new Model(1032073858), new Vector3(979.6688f, -109.1737f, 74.3531f), -138.9989f),
            World.CreatePed(new Model(1032073858), new Vector3(963.7977f, -119.8082f, 74.3531f), -97.53277f),
            World.CreatePed(new Model(1032073858), new Vector3(965.1519f, -120.0531f, 74.3531f), 75.04014f)
        };
    }

    public Ped InitializeMissionEightTarget()
    {
        return World.CreatePed(new Model(850468060), new Vector3(984.1689f, -91.15002f, 74.84882f), -132.2569f);
    }

    public List<Vehicle> InitializeMissionEightVehicles()
    {
        return new List<Vehicle>()
        {
            World.CreateVehicle(new Model(-570033273), new Vector3(979.0755f, -127.7626f, 73.42345f), 101.7186f),
            World.CreateVehicle(new Model(-570033273), new Vector3(976.4017f, -132.0998f, 73.42107f), 90.12802f),
            World.CreateVehicle(new Model(-1009268949), new Vector3(965.438f, -119.089f, 73.82143f), -103.8512f)
        };
    }

    public List<Prop> InitializeMissionEightProps()
    {
        return new List<Prop>()
        {
            World.CreateProp(new Model(2133533553), new Vector3(982.8341f, -93.3774643f, 74.9196548f), new Vector3(0f, -0f, 131.9984f), false, false),
            World.CreateProp(new Model(-956123246), new Vector3(983.3065f, -93.8901443f,  75.3182755f), new Vector3(0f, 0f, -49.999855f), false, false)
        };
    }
}

