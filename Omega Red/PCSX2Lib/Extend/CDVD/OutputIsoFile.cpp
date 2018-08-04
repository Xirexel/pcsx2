

#include "PrecompiledHeader.h"
#include "CDVD\IsoFileFormats.h"


OutputIsoFile::OutputIsoFile(){}

OutputIsoFile::~OutputIsoFile(){}

bool OutputIsoFile::IsOpened()const{ return false; }

void OutputIsoFile::Create(class wxString const &, int){}

void OutputIsoFile::Close(){}

void OutputIsoFile::WriteHeader(int, unsigned int, unsigned int){}

void OutputIsoFile::WriteSector(unsigned char const *, unsigned int){}

u32 OutputIsoFile::GetBlockSize()const{ return 0; }