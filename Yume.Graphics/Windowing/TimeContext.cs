
namespace Yume.Graphics.Windowing;

public class TimeContext(Window? window) : WindowContext(window) {
    public double DeltaTime { get; private set; } = 0;
    public double RawDeltaTime { get; private set; } = 0;
    public double Time { get; private set; } = 0;
    public double RawTime { get; private set; } = 0;
    public double TimeScale { get; set; } = 1;

    public void Update(double dt) {
        RawDeltaTime = dt;
        DeltaTime = dt * TimeScale;
        RawTime += RawDeltaTime;
        Time += DeltaTime;
    }
}
