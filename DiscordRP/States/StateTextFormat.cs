using System.Text.RegularExpressions;
using System;

namespace DiscordRP.StateTextFormat
{
    public static class TextParser
    {
        public static string ParseVariables(string input, Part part = null, CelestialBody body = null)
        {
            return Regex.Replace(input, @"\[(.*?)\]", match =>
            {
                string key = match.Groups[1].Value;

                switch (key)
                {
                    case "cost":
                        return Utils.GetTotalCost(part).ToString();

                    case "craftName":
                        return Utils.GetCraftName();

                    case "partCount":
                        return Utils.GetTotalParts(part).ToString();

                    case "body":
                        return body.name;

                    case "velocity":
                        if (HighLogic.LoadedScene == GameScenes.FLIGHT)
                            return Math.Round(FlightGlobals.ActiveVessel.srfSpeed).ToString() + "m/s";
                        return "unkown";

                    case "altitude":
                        if (HighLogic.LoadedScene == GameScenes.FLIGHT)
                            return Utils.FormatDistance(FlightGlobals.ActiveVessel.altitude);
                        return "unkown";

                    case "AP":
                        if (HighLogic.LoadedScene == GameScenes.FLIGHT)
                            return Utils.FormatDistance(Utils.GetApoapsis(FlightGlobals.ActiveVessel.orbit));
                        return "unkown";

                    case "PE":
                        if (HighLogic.LoadedScene == GameScenes.FLIGHT)
                            return Utils.FormatDistance(Utils.GetPeriapsis(FlightGlobals.ActiveVessel.orbit));
                        return "unkown";

                    case "Ec":
                        if (HighLogic.LoadedScene == GameScenes.FLIGHT)
                            return Math.Round(FlightGlobals.ActiveVessel.orbit.eccentricity, 2).ToString();
                        return "unkown";

                    default:
                        return match.Value;
                }
            });
        }
    }
}