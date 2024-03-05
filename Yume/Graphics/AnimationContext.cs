namespace Yume.Windowing;

public enum AnimationDirection {
    Forward,
    Backward
}

public enum AnimationLoopMode {
    None,
    Forward,
    Backward,
    PingPong
}

public class Animation {
    public double Delay;
    public AnimationDirection Direction = AnimationDirection.Forward;
    public double Duration = 1f;
    public bool IsPlaying = true;
    public AnimationLoopMode LoopMode = AnimationLoopMode.None;
    public Action OnComplete;

    public Action<double> OnUpdate;
    public double Progress;

    public void Update(double dt) {
        if (Delay > 0) Delay -= dt;
        else Delay = 0;

        if (!IsPlaying) return;

        if (Direction == AnimationDirection.Forward) {
            Progress += dt / Duration;
            if (Progress >= 1) {
                Progress = 1;
                IsPlaying = false;
                OnComplete?.Invoke();
            }
        }
        else {
            Progress -= dt / Duration;
            if (Progress <= 0) {
                Progress = 0;
                IsPlaying = false;
                OnComplete?.Invoke();
            }
        }

        OnUpdate?.Invoke(Progress);
    }

    public virtual void Stop() { }
}

public class AnimationContext(Window? window) : WindowContext(window) {
    public Dictionary<string, Animation> Animations { get; } = new();


    public void Add(Animation animation, string? name = null) {
        name = name ?? new DateTime().Ticks.ToString();

        if (Animations.TryGetValue(name, out var animation1))
            animation1.Stop();

        Animations[name] = animation;
    }

    public void Update() {
        for (var i = 0; i < Animations.Count; i++) {
            var animation = Animations.ElementAt(i).Value;
            animation.Update(window?.UpdateTime.DeltaTime ?? 0);
        }
    }
}