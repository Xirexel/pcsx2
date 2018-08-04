
#pragma once

extern void InitPlugins();

extern void ClosePlugins();

extern void ShutdownPlugins();

extern void OpenPlugins();

extern bool AreLoadedPlugins();

extern SysMainMemory& GetVmReserve();


extern std::unique_ptr<SysCpuProviderPack> m_CpuProviders;

extern void setInnerMcd(void* aMcd);
