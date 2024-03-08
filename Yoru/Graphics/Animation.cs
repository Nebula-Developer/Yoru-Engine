namespace Yoru.Graphics;

public class Animation {
    public double Duration = 1f;
    public double Delay;
    public double Progress;
    public bool IsPlaying = true;
    
    public AnimationDirection Direction = AnimationDirection.Forward;
    public AnimationLoopMode LoopMode = AnimationLoopMode.None;
    public Func<double, double>? Easing;

    public Action? OnComplete;
    public Action<double>? OnUpdate;

    public void Update(double dt) {
        if (Delay > 0) {
            Delay -= dt;
            return;
        }

        if (Delay < 0) Delay = 0;

        if (!IsPlaying) return;

        Progress += Direction == AnimationDirection.Forward ? dt : -dt;


        if (Direction == AnimationDirection.Forward) {
            if (Progress >= Duration) {
                if (LoopMode == AnimationLoopMode.None) {
                    IsPlaying = false;
                    OnComplete?.Invoke();
                    Progress = Duration;
                } else {
                    switch (LoopMode) {
                        case AnimationLoopMode.Forward:
                            Progress = 0;
                            break;
                        case AnimationLoopMode.Backward:
                            Direction = AnimationDirection.Backward;
                            Progress = Duration;
                            break;
                        case AnimationLoopMode.PingPong:
                        case AnimationLoopMode.Mirror:
                            Direction = AnimationDirection.Backward;
                            Progress = Duration;
                            break;
                    }
                }
            }
        } else {
            if (Progress <= 0) {
                if (LoopMode == AnimationLoopMode.None) {
                    IsPlaying = false;
                    OnComplete?.Invoke();
                    Progress = 0;
                } else {
                    switch (LoopMode) {
                        case AnimationLoopMode.Forward:
                            Direction = AnimationDirection.Forward;
                            Progress = 0;
                            break;
                        case AnimationLoopMode.Backward:
                            Progress = Duration;
                            break;
                        case AnimationLoopMode.PingPong:
                        case AnimationLoopMode.Mirror:
                            Direction = AnimationDirection.Forward;
                            Progress = 0;
                            break;
                    }
                }
            }
        }

        var prog = Progress / Duration;

        if (Direction == AnimationDirection.Backward && LoopMode == AnimationLoopMode.PingPong)
            // Invert the easing method
            OnUpdate?.Invoke(1 - Easing?.Invoke(1 - prog) ?? prog);
        else
            OnUpdate?.Invoke(Easing?.Invoke(prog) ?? prog);
    }

    public virtual void Stop() { }
}