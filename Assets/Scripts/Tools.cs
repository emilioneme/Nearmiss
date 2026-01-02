using UnityEngine;

namespace eneme 
{
    public static class Tools
    {
        public static float ElapsedTimeSince(float lastTime) 
        {
            return Time.time - lastTime;
        }

        public static float CooldownSince(float lastTime, float overTime)
        {
            return Mathf.Clamp(ElapsedTimeSince(lastTime) / overTime, 0, 1);
        }

        public static string ProcessFloat(float f, int decimalPlaces)
        {
            float number = f;
            string unit = "";

            if (f >= 1_000_000_000f) { number = f / 1_000_000_000f; unit = "b"; }
            else if (f >= 1_000_000f) { number = f / 1_000_000f; unit = "m"; }
            else if (f >= 1_000f) { number = f / 1_000f; unit = "k"; }
            else
            {
                return Mathf.Round(f).ToString();
            }

            // Round numerically (not string-based)
            float factor = Mathf.Pow(10f, decimalPlaces);
            number = Mathf.Round(number * factor) / factor;

            // Build format like "0.#", "0.##", "0.###", etc.
            string format = decimalPlaces > 0
                ? "0." + new string('#', decimalPlaces)
                : "0";

            return number.ToString(format) + unit;
        }


        public static string LimitNumberLength(float number, int length)
        {
            string s = number.ToString();
            if (s.Length <= length)
                return s;
            return s.Substring(0, length);
        }

    }
}

