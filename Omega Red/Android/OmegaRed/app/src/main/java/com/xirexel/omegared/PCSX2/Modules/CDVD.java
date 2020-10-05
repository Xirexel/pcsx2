package com.xirexel.omegared.PCSX2.Modules;

public final class CDVD extends Module {

    static {
        System.loadLibrary("CDVD");
    }

    public CDVD(){ModuleType = ModuleManager.ModuleType.CDVD;}

    protected native long GetPCSX2LibAPI();

    protected native String Execute(String aCommand);
}
