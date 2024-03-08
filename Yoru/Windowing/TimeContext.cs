namespace Yoru.Windowing;

public class TimeContext(Window? window) : WindowContext(window) {
    public double DeltaTime { get; private set; }
    public double RawDeltaTime { get; private set; }
    public double Time { get; private set; }
    public double RawTime { get; private set; }
    public double TimeScale { get; set; } = 1;

    public void Update(double dt) {
        RawDeltaTime = dt;
        DeltaTime = dt * TimeScale;
        RawTime += RawDeltaTime;
        Time += DeltaTime;
    }
}