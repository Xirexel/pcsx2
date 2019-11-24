STRIP_FPIC = sh $(top_srcdir)/strip_fPIC.sh

SUFFIXES = .asm

.asm.lo:
	$(LIBTOOL) --tag=CC --mode=compile \
	$(STRIP_FPIC) $(NASM) -f elf -d ELF -I${srcdir}/ $<

INCLUDES = -DPIXMAPDIR=\"${datadir}/pixmaps/\" \
	-DLOCALE_DIR=\"${datadir}/locale/\" \
	-DDATADIR=\"${datadir}/psemu/\" \
	$(GTK2_CFLAGS) $(GLADE2_CFLAGS) \
	-DVERSION=XVideo -DNUMBER=0.1.0

libdir = $(prefix)/lib/games/psemu/
lib_LTLIBRARIES = libDFXVideo.la

libDFXVideo_la_SOURCES = gpu.c gpu.h cfg.c cfg.h draw.c draw.h \
	fps.c fps.h key.c key.h menu.c menu.h prim.c prim.h \
	soft.c soft.h zn.c
if ARCH_X86
libDFXVideo_la_SOURCES += i386.asm
endif
libDFXVideo_la_LDFLAGS = -module -avoid-version
libDFXVideo_la_LDFLAGS += -lXv -lXext

bindir = $(prefix)/lib/games/psemu/
bin_PROGRAMS = cfgDFXVideo
cfgDFXVideo_SOURCES = gpucfg-0.1df/main.c
cfgDFXVideo_LDADD = $(GTK2_LIBS) $(GLADE2_LIBS)

glade_DATA = gpucfg-0.1df/dfxvideo.glade2 gpucfg-0.1df/gpucfg.gladep
gladedir = $(datadir)/psemu/
EXTRA_DIST = $(glade_DATA) \
	externals.h psemu.h swap.h \
	changelog.txt filemap.txt hq2x.h hq3x.h \
	support.c support.h interface.c interface.h \
	callbacks.c callbacks.h \
	interp.h license.txt macros.inc \
	res/gpu.bmp \
	gpucfg-0.1df/HACKING

