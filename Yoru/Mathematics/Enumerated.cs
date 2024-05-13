namespace Yoru.Mathematics;

public static class Enumerated {
    // Iterate an enum. Eg, if the val is A in { A, B, C }, iterate to B, or wrap to first [C->A].
    public static T? NextPotential<T>(T val) where T : Enum {
        var values = Enum.GetValues(typeof(T));
        var index = Array.IndexOf(values, val);
        return (T?)values.GetValue((index + 1) % values.Length);
    }
    
    public static T Next<T>(T val) where T : Enum => NextPotential(val)!;
}
