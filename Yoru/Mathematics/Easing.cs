
namespace Yoru.Mathematics;

public static class Easing {
    public static double ExpIn(double t) => Math.Pow(2f, 10f * (t - 1f));
    public static double ExpOut(double t) => 1f - Math.Pow(2f, -10f * t);
    public static double ExpInOut(double t) => t < 0.5f ? 0.5f * Math.Pow(2f, 20f * t - 10f) : -0.5f * Math.Pow(2f, 10f - 20f * t) + 1f;

    public static double CircIn(double t) => 1f - Math.Sqrt(1f - Math.Pow(t, 2f));
    public static double CircOut(double t) => Math.Sqrt(1f - Math.Pow(t - 1f, 2f));
    public static double CircInOut(double t) => t < 0.5f ? 0.5f * (1f - Math.Sqrt(1f - Math.Pow(2f * t, 2f))) : 0.5f * (Math.Sqrt(1f - Math.Pow(-2f * t + 2f, 2f)) + 1f);

    public static double QuadIn(double t) => Math.Pow(t, 2f);
    public static double QuadOut(double t) => 1f - Math.Pow(1f - t, 2f);
    public static double QuadInOut(double t) => t < 0.5f ? 2f * Math.Pow(t, 2f) : 1f - Math.Pow(-2f * t + 2f, 2f) / 2f;

    public static double BounceIn(double t) => 1f - BounceOut(1f - t);
    public static double BounceOut(double t) {
        if (t < 1f / 2.75f) {
            return 7.5625f * t * t;
        } else if (t < 2f / 2.75f) {
            return 7.5625f * (t -= 1.5f / 2.75f) * t + 0.75f;
        } else if (t < 2.5f / 2.75f) {
            return 7.5625f * (t -= 2.25f / 2.75f) * t + 0.9375f;
        } else {
            return 7.5625f * (t -= 2.625f / 2.75f) * t + 0.984375f;
        }
    }
    public static double BounceInOut(double t) => t < 0.5f ? BounceIn(t * 2f) / 2f : BounceOut(t * 2f - 1f) / 2f + 0.5f;

    public static double BackIn(double t) => t * t * (2.70158f * t - 1.70158f);
    public static double BackOut(double t) => 1f - BackIn(1f - t);
    public static double BackInOut(double t) => t < 0.5f ? BackIn(t * 2f) / 2f : BackOut(t * 2f - 1f) / 2f + 0.5f;

    public static double ElasticIn(double t) => Math.Sin(13f * Math.PI / 2f * t) * Math.Pow(2f, 10f * (t - 1f));
    public static double ElasticOut(double t) => 1f - ElasticIn(1f - t);
    public static double ElasticInOut(double t) => t < 0.5f ? ElasticIn(t * 2f) / 2f : ElasticOut(t * 2f - 1f) / 2f + 0.5f;

    public static double SineIn(double t) => 1f - Math.Cos(t * Math.PI / 2f);
    public static double SineOut(double t) => Math.Sin(t * Math.PI / 2f);
    public static double SineInOut(double t) => -0.5f * (Math.Cos(Math.PI * t) - 1f);

    public static double CubicIn(double t) => Math.Pow(t, 3f);
    public static double CubicOut(double t) => 1f - Math.Pow(1f - t, 3f);
    public static double CubicInOut(double t) => t < 0.5f ? 4f * Math.Pow(t, 3f) : 1f - Math.Pow(-2f * t + 2f, 3f) / 2f;

    public static double QuartIn(double t) => Math.Pow(t, 4f);
    public static double QuartOut(double t) => 1f - Math.Pow(1f - t, 4f);
    public static double QuartInOut(double t) => t < 0.5f ? 8f * Math.Pow(t, 4f) : 1f - Math.Pow(-2f * t + 2f, 4f) / 2f;

    public static double QuintIn(double t) => Math.Pow(t, 5f);
    public static double QuintOut(double t) => 1f - Math.Pow(1f - t, 5f);
    public static double QuintInOut(double t) => t < 0.5f ? 16f * Math.Pow(t, 5f) : 1f - Math.Pow(-2f * t + 2f, 5f) / 2f;

    public static double Linear(double t) => t;

    public static double SmoothStep(double t) => t * t * (3f - 2f * t);
    public static double SmootherStep(double t) => t * t * t * (t * (t * 6f - 15f) + 10f);
}
