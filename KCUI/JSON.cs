using System;
using System.IO;
using MelonLoader.Assertions;
using UnityEngine;
using UnityEngine.Assertions;

namespace KCUI
{
    public class JSON
    {
        public static string json(Day day)
        {
            // The Day notes are formatted as a bullet list with '-' as bullets.
            // The first bullet specifies a general description, the second hints
            // at a fitting season.
            string[] notes = day.notes.Replace("\n", "").Replace("\r", "").Split('-');
            string notesGeneral = notes.Length >= 2 ? notes[1] : "";
            string notesSeason = notes.Length >= 3 ? notes[2] : "";
            return $"\"{day.name}\": {{\n\"type\": \"{day.wave.name}\",\n\"notes-general\": \"{notesGeneral}\",\n\"notes-season\": \"{notesSeason}\"\n}}";
        }

        public static string json(Day[] days)
        {
            string s = "";
            foreach (Day day in days)
                s += json(day) + ",\n";
            return s;
        }

        public static string json(Wave wave)
        {
            string s = $"\"{wave.name}\": {{\n";
            s += $"\"numTrolls\": {json(wave.numTrolls)},\n";
            s += $"\"numToughTrolls\": {json(wave.numToughTrolls)},\n";
            s += $"\"numBosses\": {json(wave.numBosses)},\n";
            s += $"\"numKillerBosses\": {json(wave.numKillerBosses)},\n";
            s += $"\"numOgres\": {json(wave.numOgres)},\n";
            s += $"\"numSquids\": {json(wave.numSquids)},\n";
            s += "}";
            return s;
        }

        public static string json(AnimationCurve curve)
        {
            string s = "{\n";
            float[] times = new float[curve.keys.Length];
            float[] values = new float[curve.keys.Length];
            for (int i = 0; i < curve.keys.Length; i++)
            {
                times[i] =  curve.keys[i].time;
                values[i] = curve.keys[i].value;
            }
            s += $"\"times\": {json(times)},\n";
            s += $"\"values\": {json(values)},\n";

            // Sample points to draw the curve more clearly.
            if (curve.keys.Length > 0)
            {
                int lo = (int)Math.Floor(curve.keys[0].time);
                int hi = (int)Math.Ceiling(curve.keys[curve.keys.Length - 1].time);
                float[] timesSampled = new float[hi-lo+1];
                float[] valuesSampled = new float[hi-lo+1];
                // Only consider the integer time values, because Kingdom samples
                // the curves for integer day numbers.
                for (int time = lo; time <= hi; time++)
                {
                    timesSampled[time-lo] = time;
                    valuesSampled[time-lo] = curve.Evaluate(time);
                }
                LemonAssert.IsEqual(lo, timesSampled[0]);
                LemonAssert.IsEqual(hi, timesSampled[timesSampled.Length - 1]);
                s += $"\"times-sampled\": {json(timesSampled)},\n";
                s += $"\"values-sampled\": {json(valuesSampled)},\n";
            }
            s += "}";
            return s;
        }

        public static string json(float[] values)
        {
            string s = "[";
            for (int i = 0; i < values.Length; i++)
            {
                s += values[i].ToString() + ( i < (values.Length - 1) ? "," : "" );
            }
            s += "]";
            return s;
        }
    }
}