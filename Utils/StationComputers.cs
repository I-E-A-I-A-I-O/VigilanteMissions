using GTA;
using GTA.Math;
using System;
using System.Collections.Generic;

static class StationComputers 
{
    static List<Vector3> computerLocations = new List<Vector3>()
    {
        new Vector3(441.4863f, -978.9369f, 29.68959f),
        new Vector3(447.5584f, -973.4796f, 29.68959f),
        new Vector3(1853.159f, 3690.25f, 33.69815f),
        new Vector3(-449.7587f, 6012.482f, 31.21436f)
    };

    public static bool IsNearPoliceComputer(Ped ped)
    {
        foreach (Vector3 computerLocation in computerLocations)
        {
            if (ped.IsInRange(computerLocation, 1.3f))
            {
                return true;
            }
        }
        return false;
    }
}
