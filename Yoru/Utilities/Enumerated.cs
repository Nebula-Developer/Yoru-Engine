namespace Yoru.Utilities;

public static class Enumerated {
    // Iterate an enum. Eg, if the val is A in { A, B, C }, iterate to B, or wrap to first [C->A].
    public static T? NextPotential<T>(T val) where T : Enum {
        var values = Enum.GetValues(typeof(T));
        var index = Array.IndexOf(values, val);
        return (T?)values.GetValue((index + 1) % values.Length);
    }
    
    public static T? PreviousPotential<T>(T val) where T : Enum {
        var values = Enum.GetValues(typeof(T));
        var index = Array.IndexOf(values, val);
        return (T?)values.GetValue((index - 1 + values.Length) % values.Length);
    }
    
    public static T Next<T>(T val) where T : Enum => NextPotential(val)!;
    public static T Previous<T>(T val) where T : Enum => PreviousPotential(val)!;
    
    
    public static T Index<T>(int index) where T : Enum => (T)Enum.GetValues(typeof(T)).GetValue(index % Enum.GetValues(typeof(T)).Length)!;
    public static int Count<T>() where T : Enum => Enum.GetValues(typeof(T)).Length;
}
