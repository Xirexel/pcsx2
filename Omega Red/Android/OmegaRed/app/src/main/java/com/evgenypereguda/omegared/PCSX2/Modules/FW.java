package com.evgenypereguda.omegared.PCSX2.Modules;

public final class FW extends Module {

    static {
        System.loadLibrary("FW");
    }

    public FW(){ModuleType = ModuleManager.ModuleType.FW;}

    protected native long GetPCSX2LibAPI();

    protected native String Execute(String aCommand);
}