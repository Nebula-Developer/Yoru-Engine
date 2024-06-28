using ManagedBass;
using ManagedBass.Fx;
using ManagedBass.Mix;

namespace Yoru.Audio;

public class Channel {
    public int Handle { get; protected set; }
}

// extends channel but allows for changing the volume and frequency
public class AudioChannel : Channel {
    public float Volume {
        get {
            Bass.ChannelGetAttribute(Handle, ChannelAttribute.Volume, out var volume);
            return volume;
        }
        set => Bass.ChannelSetAttribute(Handle, ChannelAttribute.Volume, value);
    }

    public float Frequency {
        get {
            Bass.ChannelGetAttribute(Handle, ChannelAttribute.Frequency, out var frequency);
            return frequency;
        }
        set => Bass.ChannelSetAttribute(Handle, ChannelAttribute.Frequency, value);
    }

    public float Pan {
        get {
            Bass.ChannelGetAttribute(Handle, ChannelAttribute.Pan, out var pan);
            return pan;
        }
        set => Bass.ChannelSetAttribute(Handle, ChannelAttribute.Pan, value);
    }

    public double Position {
        get => Bass.ChannelBytes2Seconds(Handle, Bass.ChannelGetPosition(Handle));
        set => Bass.ChannelSetPosition(Handle, Bass.ChannelSeconds2Bytes(Handle, value));
    }

    public long PositionBytes {
        get => Bass.ChannelGetPosition(Handle);
        set => Bass.ChannelSetPosition(Handle, value);
    }

    public int GetLevelLeft() => AudioHelper.LowWord32(Bass.ChannelGetLevel(Handle));
    public int GetLevelRight() => AudioHelper.HighWord32(Bass.ChannelGetLevel(Handle));
    public int GetLevel() => (GetLevelLeft() + GetLevelRight()) / 2;

    public double GetNormalizedLevelLeft() => AudioHelper.GetNormalizedLevel(GetLevelLeft());
    public double GetNormalizedLevelRight() => AudioHelper.GetNormalizedLevel(GetLevelRight());
    public double GetNormalizedLevel() => AudioHelper.GetNormalizedLevel(GetLevel());

    public void Play() => Bass.ChannelPlay(Handle);
    public void Pause() => Bass.ChannelPause(Handle);
    public void Stop() => Bass.ChannelStop(Handle);
    public void Restart() {
        PositionBytes = 0;
        Play();
    }

    public const BassFlags DefaultFlags = BassFlags.Decode | BassFlags.Float | BassFlags.Prescan;
}

public class Stream : AudioChannel {
    public Stream(int handle) => Handle = handle;
    public Stream(string path, BassFlags flags = DefaultFlags) => Handle = Bass.CreateStream(path, 0, 0, flags);
    public Stream(byte[] data, BassFlags flags = DefaultFlags) => Handle = Bass.CreateStream(data, 0, data.Length, flags);
}

public class FXStream : Stream {
    public int AudioHandle { get; private set; }

    private void _createTempo() {
        BassFlags channelFlags = Bass.ChannelGetInfo(Handle).Flags;
        if (!channelFlags.HasFlag(BassFlags.Decode))
            throw new Exception("FXStream requires the Decode flag to be set on the channel");
        
        int newHandle = BassFx.TempoCreate(Handle, channelFlags);
        AudioHandle = Handle;
        Handle = newHandle;
    }

    public FXStream(int handle) : base(handle) => _createTempo();
    public FXStream(string path, BassFlags flags = DefaultFlags) : base(path, flags) => _createTempo();
    public FXStream(byte[] data, BassFlags flags = DefaultFlags) : base(data, flags) => _createTempo();

    private Dictionary<EffectType, int> _effects = new();
    public void SetEffect(EffectType type, IEffectParameter parameters, int priority = 0) {
        if (!_effects.ContainsKey(type))
            _effects[type] = Bass.ChannelSetFX(Handle, type, priority);
        else
            Bass.FXSetPriority(_effects[type], priority);
        
        Bass.FXSetParameters(_effects[type], parameters);
    }

    public void SetPitch(float pitch) => SetEffect(EffectType.PitchShift, new PitchShiftParameters { fSemitones = pitch, lFFTsize = 2048, lOsamp = 4 });
    public void SetReverb(float dry, float wet, float width, float damp, float roomSize) => SetEffect(EffectType.Freeverb, new ReverbParameters { fDryMix = dry, fWetMix = wet, fWidth = width, fDamp = damp, fRoomSize = roomSize });
}

public class Mixer : AudioChannel {
    public Mixer() {
        Handle = BassMix.CreateMixerStream(44100, 2, BassFlags.Default | BassFlags.MixerNonStop | BassFlags.MixerChanBuffer | BassFlags.MixerChanNoRampin);
        Bass.ChannelSetAttribute(Handle, ChannelAttribute.NoBuffer, 1);
        Bass.ChannelSetAttribute(Handle, ChannelAttribute.Buffer, 0);
    }

    public void JoinMixer(Channel channel, BassFlags flags) {
        if (channel.Handle == Handle)
            throw new Exception("Cannot join a mixer to itself");
        BassMix.MixerAddChannel(Handle, channel.Handle, flags);
    }

    public void JoinMixer(Channel channel) => JoinMixer(channel, BassFlags.Default);

    public void RemoveMixer(Channel channel) => BassMix.MixerRemoveChannel(Handle);
}
