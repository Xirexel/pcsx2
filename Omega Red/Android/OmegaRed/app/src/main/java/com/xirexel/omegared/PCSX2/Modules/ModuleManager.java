package com.xirexel.omegared.PCSX2.Modules;

import java.util.HashMap;
import java.util.*;


public final class ModuleManager {

    public enum ModuleType
    {
        VideoRenderer,
        Pad,
        AudioRenderer,
        CDVD,
        USB,
        FW,
        DEV9,
        MemoryCard
    }

    private HashMap<ModuleType, Module> m_Modules = new HashMap<ModuleType, Module>();

    private static ModuleManager m_Instance = null;

    private ModuleManager(){
        addModule(new AudioRenderer());
        addModule(new CDVD());
        addModule(new DEV9());
        addModule(new FW());
        addModule(new MemoryCard());
        addModule(new Pad());
        addModule(new USB());
        addModule(new VideoRenderer());
    }

    public static ModuleManager getInstance()
    {
        if (m_Instance == null)
            m_Instance = new ModuleManager();

        return m_Instance;
    }

    private void addModule(Module a_Module)
    {
        m_Modules.put(a_Module.ModuleType, a_Module);
    }

    public Collection<Module> getModules(){
        return m_Modules.values();
    }


    public Module getModule(ModuleType a_ModuleType)
    {
        Module l_result = null;

        if(m_Modules.containsKey(a_ModuleType))
            l_result = m_Modules.get(a_ModuleType);

        return l_result;
    }
}
