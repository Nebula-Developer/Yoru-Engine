namespace Yoru.Graphics;

public class Animation {
    public double Delay;
    
    public AnimationDirection Direction = AnimationDirection.Forward;
    public double Duration = 1f;
    public Func<double, double>? Easing;
    public bool IsPlaying = true;
    public AnimationLoopMode LoopMode = AnimationLoopMode.None;
    
    public Action? OnComplete;
    public Action<double>? OnLoop;
    public Action<double>? OnUpdate;
    public double Progress;
    
    public void InvertDirection() {
        switch (LoopMode) {
            case AnimationLoopMode.Forward:
                Direction = AnimationDirection.Forward;
                Progress = 0;
                break;
            case AnimationLoopMode.Backward:
                Direction = AnimationDirection.Backward;
                Progress = Duration;
                break;
            case AnimationLoopMode.PingPong:
            case AnimationLoopMode.Mirror:
                Direction = Direction == AnimationDirection.Forward ? AnimationDirection.Backward : AnimationDirection.Forward;
                Progress = Direction == AnimationDirection.Forward ? 0 : Duration;
                break;
            case AnimationLoopMode.None:
                IsPlaying = false;
                Progress = Direction == AnimationDirection.Forward ? Duration : 0;
                OnComplete?.Invoke();
                return;
        }
        
        OnUpdate?.Invoke(Progress);
    }
    
    public void Update(double dt) {
        if (!IsPlaying) return;
        
        if (Delay > 0) {
            Delay -= dt;
            return;
        }
        
        if (Delay < 0) Delay = 0;
        
        Progress += Direction == AnimationDirection.Forward ? dt : -dt;
        
        if (Direction == AnimationDirection.Forward && Progress >= Duration
         || Direction == AnimationDirection.Backward && Progress <= 0) {
            var absDirection = Direction == AnimationDirection.Forward ? 1 : 0;
            OnLoop?.Invoke(absDirection);
            InvertDirection();
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
