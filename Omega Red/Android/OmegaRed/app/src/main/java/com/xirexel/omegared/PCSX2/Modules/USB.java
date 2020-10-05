package com.xirexel.omegared.PCSX2.Modules;

public final class USB  extends Module {

    static {
        System.loadLibrary("USB");
    }

    public USB(){ModuleType = ModuleManager.ModuleType.USB;}

    protected native long GetPCSX2LibAPI();

    protected native String Execute(String aCommand);
}