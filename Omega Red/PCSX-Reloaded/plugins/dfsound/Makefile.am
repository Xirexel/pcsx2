INCLUDES = -DPIXMAPDIR=\"${datadir}/pixmaps/\" \
	-DLOCALE_DIR=\"${datadir}/locale/\" \
	-DDATADIR=\"${datadir}/psemu/\" \
	$(GTK2_CFLAGS) $(GLADE2_CFLAGS) \
	-DVERSION=0 -DREVISION=1 -DBUILD=0

libdir = $(prefix)/lib/games/psemu/
lib_LTLIBRARIES = libDFSound.la

libDFSound_la_SOURCES = spu.c spu.h cfg.c cfg.h dma.c dma.h \
	freeze.c psemu.c registers.c registers.h 
if HAVE_ALSA
libDFSound_la_SOURCES +=  alsa.c alsa.h
INCLUDES +=  -DUSEALSA
else
libDFSound_la_SOURCES +=  oss.c oss.h
endif
libDFSound_la_LDFLAGS = -module -avoid-version
if HAVE_ALSA
libDFSound_la_LDFLAGS += $(ALSA_LIBS)
endif

bindir = $(prefix)/lib/games/psemu/
bin_PROGRAMS = cfgDFSound
cfgDFSound_SOURCES = spucfg-0.1df/main.c
cfgDFSound_LDADD = $(GTK2_LIBS) $(GLADE2_LIBS)

glade_DATA = spucfg-0.1df/dfsound.glade2
gladedir = $(datadir)/psemu/
EXTRA_DIST = $(glade_DATA) \
	externals.h dsoundoss.h regs.h debug.c debug.h \
	record.c record.h resource.h xa.c xa.h gauss_i.h \
	adsr.c adsr.h reverb.c reverb.h stdafx.c stdafx.h \
	regs.h psemuxa.h \
	changelog.txt Filemap.txt License.txt spuPeopsSound.c \
	spuPeopsSound.def \
	spucfg-0.1df/HACKING 
