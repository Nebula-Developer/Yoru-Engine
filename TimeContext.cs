
public class TimeContext(Window window) : WindowContext(window) {
    public float DeltaTime { get; private set; } = 0;
    public float RawDeltaTime { get; private set; } = 0;
    public float Time { get; private set; } = 0;
    public float RawTime { get; private set; } = 0;
    public float TimeScale { get; set; } = 1;

    public void Update(float dt) {
        RawDeltaTime = dt;
        DeltaTime = dt * TimeScale;
        RawTime += RawDeltaTime;
        Time += DeltaTime;
    }
}
