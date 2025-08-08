using System;
using MelonLoader.Assertions;
using UnityEngine;

// A collection of structs that simplify Kingdom objects for dumping to JSON.
namespace KCUI
{
    [System.Serializable]
    public struct DumpDay
    {
        public string Name;
        public string Type;
        public string NotesGeneral;
        public string NotesSeason;

        public DumpDay(Day day)
        {
            string[] notes = day.notes.Replace("\n", "").Replace("\r", "").Split('-');
            this.Name=day.name;
            this.Type=day.wave.name;
            this.NotesGeneral = notes.Length >= 2 ? notes[1] : "";
            this.NotesSeason = notes.Length >= 3 ? notes[2] : "";
        }
    }

    [System.Serializable]
    public struct DumpCurve
    {
        public float[] Times;
        public float[] Values;
        public float[] TimesSampled;
        public float[] ValuesSampled;

        public DumpCurve(AnimationCurve curve)
        {
            this.Times = new float[curve.keys.Length];
            this.Values = new float[curve.keys.Length];
            for (int i = 0; i < curve.keys.Length; i++)
            {
                this.Times[i] =  curve.keys[i].time;
                this.Values[i] = curve.keys[i].value;
            }

            // Sample points to draw the curve less jaggedly.
            this.TimesSampled  = new float[0];
            this.ValuesSampled = new float[0];
            if (curve.keys.Length > 0)
            {
                int lo = (int)Math.Floor(curve.keys[0].time);
                int hi = (int)Math.Ceiling(curve.keys[curve.keys.Length - 1].time);
                this.TimesSampled = new float[hi-lo+1];
                this.ValuesSampled = new float[hi-lo+1];
                // Only consider the integer time values, because Kingdom samples
                // the curves for integer day numbers as time.
                for (int time = lo; time <= hi; time++)
                {
                    this.TimesSampled[time-lo] = time;
                    this.ValuesSampled[time-lo] = curve.Evaluate(time);
                }
                // Validate that we sample time values in the range [lo, hi].
                LemonAssert.IsEqual(lo, this.TimesSampled[0]);
                LemonAssert.IsEqual(hi, this.TimesSampled[this.TimesSampled.Length - 1]);
            }
        }
    }

    [System.Serializable]
    public struct DumpWave
    {
        public string Name;
        public DumpCurve NumTrolls;
        public DumpCurve NumToughTrolls;
        public DumpCurve NumBosses;
        public DumpCurve NumKillerBosses;
        public DumpCurve NumOgres;
        public DumpCurve NumSquids;

        public DumpWave(Wave wave)
        {
            this.Name = wave.name;
            this.NumTrolls = new DumpCurve(wave.numTrolls);
            this.NumToughTrolls = new DumpCurve(wave.numToughTrolls);
            this.NumBosses = new DumpCurve(wave.numBosses);
            this.NumKillerBosses = new DumpCurve(wave.numKillerBosses);
            this.NumOgres = new DumpCurve(wave.numOgres);
            this.NumSquids = new DumpCurve(wave.numSquids);
        }
    }
}