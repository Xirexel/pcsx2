package com.xirexel.omegared.PCSX2.Modules;

public final class AudioRenderer extends Module {

    static {
        System.loadLibrary("AudioRenderer");
    }

    public AudioRenderer(){ModuleType = ModuleManager.ModuleType.AudioRenderer;}

    protected native long GetPCSX2LibAPI();

    protected native String Execute(String aCommand);
}
