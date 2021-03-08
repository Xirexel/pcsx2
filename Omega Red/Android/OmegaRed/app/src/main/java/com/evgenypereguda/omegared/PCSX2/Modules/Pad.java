package com.evgenypereguda.omegared.PCSX2.Modules;

public final class Pad  extends Module {

    static {
        System.loadLibrary("Pad");
    }

    public Pad(){ModuleType = ModuleManager.ModuleType.Pad;}

    protected native long GetPCSX2LibAPI();

    protected native String Execute(String aCommand);
}
