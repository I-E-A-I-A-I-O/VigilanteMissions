using System;
using System.Collections.Generic;
using GTA;

internal static class Loading
{
    public static void LoadModels(List<Model> models)
    {
        foreach(Model model in models)
        {
            LoadModel(model);
        }
    }

    public static void UnloadModels(List<Model> models)
    {
        foreach (Model model in models)
        {
            model.MarkAsNoLongerNeeded();
        }
    }

    public static void LoadModel(Model model)
    {
        model.Request();
        while (!model.IsLoaded)
        {
            Script.Wait(10);
        }

        if (!model.IsLoaded)
        {
            LoadModel(model);
        }
    }

    public static void UnloadModel(Model model)
    {
        model.MarkAsNoLongerNeeded();
    }
}
