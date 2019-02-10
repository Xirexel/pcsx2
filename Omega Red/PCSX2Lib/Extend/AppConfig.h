
#pragma once

#include "Utilities\Path.h"
#include "Config.h"
#include "wx\datetime.h"

enum MemoryCardType
{
	MemoryCard_None,
	MemoryCard_File,
	MemoryCard_Folder,
	MemoryCard_MaxCount
};

enum AspectRatioType
{
	AspectRatio_Stretch,
	AspectRatio_4_3,
	AspectRatio_16_9,
	AspectRatio_MaxCount
};

enum FoldersEnum_t
{
	// FIXME : Plugins, Settings, and Themes folders are no longer part of the user-local
	// ini storage.  They have been moved to an app-local ini storage class (which is currently
	// handled lazily via global variables)
	FolderId_Plugins = 0,
	FolderId_Settings,
	FolderId_Themes,

	FolderId_Bios,
	FolderId_Snapshots,
	FolderId_Savestates,
	FolderId_MemoryCards,
	FolderId_Logs,
	FolderId_Langs,

	FolderId_Documents,

	FolderId_Cheats,
	FolderId_CheatsWS,

	FolderId_COUNT
};

// --------------------------------------------------------------------------------------
//  PathDefs  (namespace)
// --------------------------------------------------------------------------------------
// Contains default values for various pcsx2 path names and locations.
//
// Note: The members of this namespace are intended for default value initialization only.
// Everything else should use the path folder assignments in g_Conf instead, since those
// are user-configurable.
//

namespace FilenameDefs
{
	extern wxFileName GetUiConfig();
	extern wxFileName GetUiKeysConfig();
	extern wxFileName GetVmConfig();
	extern wxFileName GetUsermodeConfig();
	extern const wxFileName& Memcard(uint port, uint slot);
};

class AppConfigInner
{
public:

	// ------------------------------------------------------------------------
	struct FolderOptions
	{
		BITFIELD32()
			bool
		UseDefaultBios : 1,
					 UseDefaultSnapshots : 1,
									   UseDefaultSavestates : 1,
														  UseDefaultMemoryCards : 1,
																			  UseDefaultLogs : 1,
																						   UseDefaultLangs : 1,
																										 UseDefaultCheats : 1,
																														UseDefaultCheatsWS : 1;
		BITFIELD_END

			wxDirName
			Bios,
			Snapshots,
			Savestates,
			MemoryCards,
			Langs,
			Logs,
			Cheats,
			CheatsWS;
	};

	// ------------------------------------------------------------------------
	struct FilenameOptions
	{
		wxFileName Plugins[PluginId_Count];

		void LoadSave(IniInterface& conf){}

		const wxFileName& operator[](PluginsEnum_t pluginidx) const{ return Plugins[pluginidx]; }
	};

	// ------------------------------------------------------------------------
	// Options struct for each memory card.
	//
	struct McdOptions
	{
		wxFileName	Filename;	// user-configured location of this memory card
		bool		Enabled;	// memory card enabled (if false, memcard will not show up in-game)
		MemoryCardType Type;	// the memory card implementation that should be used
	};

	// ------------------------------------------------------------------------
	// The GS window receives much love from the land of Options and Settings.
	//
	struct GSWindowOptions
	{
		// Closes the GS/Video port on escape (good for fullscreen activity)
		bool		CloseOnEsc;

		bool		DefaultToFullscreen;
		bool		AlwaysHideMouse;
		bool		DisableResizeBorders;
		bool		DisableScreenSaver;

		AspectRatioType AspectRatio;
		Fixed100	Zoom;
		Fixed100	StretchY;
		Fixed100	OffsetX;
		Fixed100	OffsetY;


		bool		IsMaximized;
		bool		IsFullscreen;
		bool		EnableVsyncWindowFlag;

		bool		IsToggleFullscreenOnDoubleClick;
		bool		IsToggleAspectRatioSwitch;

	};

	struct FramerateOptions
	{
		bool		SkipOnLimit;
		bool		SkipOnTurbo;

		Fixed100	NominalScalar;
		Fixed100	TurboScalar;
		Fixed100	SlomoScalar;
	};


public:

	// Master toggle for enabling or disabling all speedhacks in one fail-free swoop.
	// (the toggle is applied when a new EmuConfig is sent through AppCoreThread::ApplySettings)
	bool		EnableSpeedHacks;
	bool		EnableGameFixes;

//	// Presets try to prevent users from overwhelming when they want to change settings (usually to make a game run faster).
//	// The presets allow to modify the balance between emulation accuracy and emulation speed using a pseudo-linear control.
//	// It's pseudo since there's no way to arrange groups of all of pcsx2's settings such that each next group makes it slighty faster and slightly less compatiible for all games.
//	//However, By carefully selecting these preset config groups, it's hopefully possible to achieve this goal for a reasonable percentage (hopefully above 50%) of the games.
//	//when presets are enabled, the user has practically no control over the emulation settings, and can only choose the preset to use.

	wxString				CurrentIso;
	wxString				CurrentELF;
	wxString				CurrentIRX;

//	// Memorycard options - first 2 are default slots, last 6 are multitap 1 and 2
//	// slots (3 each)
//	McdOptions				Mcd[8];
	wxString				GzipIsoIndexTemplate; // for quick-access index with gzipped ISO

	FolderOptions			Folders;
	FilenameOptions			BaseFilenames;
	GSWindowOptions			GSWindow;
	FramerateOptions		Framerate;

	//// PCSX2-core emulation options, which are passed to the emu core prior to initiating
	//// an emulation session.  Note these are the options saved into the GUI ini file and
	//// which are shown as options in the gui preferences, but *not* necessarily the options
	//// used by emulation.  The gui allows temporary per-game and commandline level overrides.
	Pcsx2Config				EmuOptions;

	wxString CurrentGameArgs;

public:

	wxString FullpathToBios() const;
};

extern std::unique_ptr<AppConfigInner> g_Conf;


enum LimiterModeType
{
	Limit_Nominal,
	Limit_Turbo,
	Limit_Slomo,
};

extern LimiterModeType g_LimiterMode;