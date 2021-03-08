package com.evgenypereguda.omegared.PCSX2.Modules;

public final class VideoRenderer  extends Module {

    static {
        System.loadLibrary("VideoRenderer");
    }

    public VideoRenderer(){ModuleType = ModuleManager.ModuleType.VideoRenderer;}

    protected native long GetPCSX2LibAPI();

    protected native String Execute(String aCommand);
}