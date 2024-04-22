#nullable disable

using System.Diagnostics;

namespace Yoru;

public class TimeContext(Application app) : AppContext(app) {
    public Stopwatch Timer;
    public double TimeScale = 1.0;

    public double RawDeltaTime { get; private set; }
    public double RawTime { get; private set; }

    public double DeltaTime { get; private set; }
    public double Time { get; private set; }

    public void Update() {
        RawDeltaTime = Timer.Elapsed.TotalSeconds;
        RawTime += RawDeltaTime;

        DeltaTime = RawDeltaTime * TimeScale;
        Time += DeltaTime;

        Timer.Restart();
    }

    public override void Load() {
        Timer = new Stopwatch();
        Timer.Start();
    }
}
