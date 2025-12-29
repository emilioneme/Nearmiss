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

        public static string ProcessFloat(float f)
        {
            float number = f;
            string unit = "";

            if (f >= 1_000_000_000f) { number = f / 1_000_000_000f; unit = "b"; }
            else if (f >= 1_000_000f) { number = f / 1_000_000f; unit = "m"; }
            else if (f >= 1_000f) { number = f / 1_000f; unit = "k"; }
            else { return Mathf.Round(number).ToString(); }

            number = Mathf.Round(number * 10f) / 10f;   // 1 decimal place
            return number.ToString("0.#") + unit;
        }

    }
}

