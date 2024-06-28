
namespace Yoru.Audio;

public static class AudioHelper {
    public static int LowWord32(int value) => value & 0xFFFF;
    public static int HighWord32(int value) => (value >> 16) & 0xFFFF;

    public const double MaxLevel = 32768.0;
    public static double GetNormalizedLevel(int level) => level / MaxLevel;
}
