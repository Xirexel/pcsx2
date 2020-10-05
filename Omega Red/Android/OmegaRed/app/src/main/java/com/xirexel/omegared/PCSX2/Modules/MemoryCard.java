package com.xirexel.omegared.PCSX2.Modules;

public final class MemoryCard extends Module {

    static {
        System.loadLibrary("MemoryCard");
    }

    public MemoryCard(){ModuleType = ModuleManager.ModuleType.MemoryCard;}

    protected native long GetPCSX2LibAPI();

    protected native String Execute(String aCommand);
}
