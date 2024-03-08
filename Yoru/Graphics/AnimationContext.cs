using Yoru.Windowing;

namespace Yoru.Graphics;

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
            animation.Update(Window?.UpdateTime.DeltaTime ?? 0);
        }
    }
}