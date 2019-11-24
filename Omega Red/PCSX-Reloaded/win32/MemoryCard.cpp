#undef inline

#define WINVER 0x0500
#define _WIN32_WINNT WINVER

#define INT8_MIN (-127i8 - 1)
#define INT16_MIN (-32767i16 - 1)
#define INT32_MIN (-2147483647i32 - 1)
#define INT64_MIN (-9223372036854775807i64 - 1)
#define INT8_MAX 127i8
#define INT16_MAX 32767i16
#define INT32_MAX 2147483647i32
#define INT64_MAX 9223372036854775807i64
#define UINT8_MAX 0xffui8
#define UINT16_MAX 0xffffui16
#define UINT32_MAX 0xffffffffui32
#define UINT64_MAX 0xffffffffffffffffui64

#define INTMAX_MIN INT64_MIN
#define INTMAX_MAX INT64_MAX
#define UINTMAX_MAX UINT64_MAX


#define FALSE	0
#define TRUE	1


#define MCDBLOCK_SIZE (1024)

#define MCD_SIZE (1024 * 8 * 16)



#include "plugins/Common/CommonCPP.h"
#include "MemoryCard.h"

extern "C" void LoadMcd(int mcd, char *str);

static __inline int wstat(wchar_t const *const _FileName, struct stat *const _Stat)
{
    _STATIC_ASSERT(sizeof(struct stat) == sizeof(struct _stat64i32));
    return _wstat64i32(_FileName, (struct _stat64i32 *)_Stat);
}


struct McdFile
{
    std::wstring m_file_path;
	
	uint8 m_isExist = FALSE;
};

std::map<int32_t, McdFile> m_McdFiles;


MemoryCard g_MemoryCard;


MemoryCard::MemoryCard() {}

MemoryCard::~MemoryCard() {}

void MemoryCard::open()
{ 
    for (auto &l_item : m_McdFiles) {
		
		struct stat buf;

		if (wstat(l_item.second.m_file_path.c_str(), &buf) != -1 && buf.st_size == MCD_SIZE)
		{
			l_item.second.m_isExist = TRUE;
		}
		else
		{
            createMcd(l_item.second.m_file_path.c_str());

			l_item.second.m_isExist = checkExistence(l_item.second.m_file_path.c_str());
		}
    }
}

uint8 MemoryCard::isMcdPresented(int a_port)
{
    uint8 l_result = FALSE;

    auto l_itr = m_McdFiles.find(a_port);

    if (l_itr != m_McdFiles.end()) {
        l_result = (*l_itr).second.m_isExist;
    }

	if (a_port == -1 && m_McdFiles.size() > 0)
	{
		for (auto &l_item : m_McdFiles)
		{
			if (l_item.second.m_isExist != FALSE) {
                l_result = l_item.second.m_isExist;

                break;
			}
		}
	}

    return l_result;
}

uint8 MemoryCard::formatMcd(int a_port)
{
    uint8 l_result = FALSE;

    auto l_itr = m_McdFiles.find(a_port);

    if (l_itr != m_McdFiles.end()) {

        std::fstream l_file_stream = std::fstream();

        l_file_stream.open((*l_itr).second.m_file_path, std::ios::in | std::ios::out | std::ios::binary | std::ios::trunc);

		l_file_stream.close();
		
        createMcd((*l_itr).second.m_file_path.c_str());

		l_result = TRUE;
    }
	
    return l_result;
}

void MemoryCard::setMcd(const wchar_t *a_filePath, int a_port)
{
    if (a_port < 0)
		m_McdFiles.clear();
    else {
        m_McdFiles[a_port].m_file_path = a_filePath;

        m_McdFiles[a_port].m_isExist = false;

        open();

        LoadMcd(a_port + 1, "");
	}
}

void MemoryCard::createMcd(const wchar_t *a_filePath)
{
    FILE *f;
    struct stat buf;
    int s = MCD_SIZE;
    int i = 0, j;

    f = _wfopen(a_filePath, L"wb");
    if (f == NULL)
        return;

    if (wstat(a_filePath, &buf) != -1) {
        if ((buf.st_size == MCD_SIZE + 3904) || wcsstr(a_filePath, L".gme")) {
            s = s + 3904;
            fputc('1', f);
            s--;
            fputc('2', f);
            s--;
            fputc('3', f);
            s--;
            fputc('-', f);
            s--;
            fputc('4', f);
            s--;
            fputc('5', f);
            s--;
            fputc('6', f);
            s--;
            fputc('-', f);
            s--;
            fputc('S', f);
            s--;
            fputc('T', f);
            s--;
            fputc('D', f);
            s--;
            for (i = 0; i < 7; i++) {
                fputc(0, f);
                s--;
            }
            fputc(1, f);
            s--;
            fputc(0, f);
            s--;
            fputc(1, f);
            s--;
            fputc('M', f);
            s--;
            fputc('Q', f);
            s--;
            for (i = 0; i < 14; i++) {
                fputc(0xa0, f);
                s--;
            }
            fputc(0, f);
            s--;
            fputc(0xff, f);
            while (s-- > (MCD_SIZE + 1))
                fputc(0, f);
        } else if ((buf.st_size == MCD_SIZE + 64) || wcsstr(a_filePath, L".mem") || wcsstr(a_filePath, L".vgs")) {
            s = s + 64;
            fputc('V', f);
            s--;
            fputc('g', f);
            s--;
            fputc('s', f);
            s--;
            fputc('M', f);
            s--;
            for (i = 0; i < 3; i++) {
                fputc(1, f);
                s--;
                fputc(0, f);
                s--;
                fputc(0, f);
                s--;
                fputc(0, f);
                s--;
            }
            fputc(0, f);
            s--;
            fputc(2, f);
            while (s-- > (MCD_SIZE + 1))
                fputc(0, f);
        }
    }
    fputc('M', f);
    s--;
    fputc('C', f);
    s--;
    while (s-- > (MCD_SIZE - 127))
        fputc(0, f);
    fputc(0xe, f);
    s--;

    for (i = 0; i < 15; i++) { // 15 blocks
        fputc(0xa0, f);
        s--;
        fputc(0x00, f);
        s--;
        fputc(0x00, f);
        s--;
        fputc(0x00, f);
        s--;
        fputc(0x00, f);
        s--;
        fputc(0x00, f);
        s--;
        fputc(0x00, f);
        s--;
        fputc(0x00, f);
        s--;
        fputc(0xff, f);
        s--;
        fputc(0xff, f);
        s--;
        for (j = 0; j < 117; j++) {
            fputc(0x00, f);
            s--;
        }
        fputc(0xa0, f);
        s--;
    }

    for (i = 0; i < 20; i++) {
        fputc(0xff, f);
        s--;
        fputc(0xff, f);
        s--;
        fputc(0xff, f);
        s--;
        fputc(0xff, f);
        s--;
        fputc(0x00, f);
        s--;
        fputc(0x00, f);
        s--;
        fputc(0x00, f);
        s--;
        fputc(0x00, f);
        s--;
        fputc(0xff, f);
        s--;
        fputc(0xff, f);
        s--;
        for (j = 0; j < 118; j++) {
            fputc(0x00, f);
            s--;
        }
    }

    while ((s--) >= 0)
        fputc(0, f);

    fclose(f);
}

uint8 MemoryCard::checkExistence(const wchar_t *a_filePath)
{

    uint8 l_result = FALSE;

    do {

        std::fstream l_file_stream = std::fstream();

        l_file_stream.open(a_filePath, std::ios::in | std::ios::out | std::ios::binary);

        if (!l_file_stream.is_open())
            break;

		l_result = TRUE;

    } while (false);

    return l_result;
}

uint8 MemoryCard::writeToMcd(int a_port, char *a_data, uint32_t adr, int size)
{
    uint8 l_result = FALSE;

    auto l_itr = m_McdFiles.find(a_port);

    if (l_itr != m_McdFiles.end()) {
		
        std::fstream l_file_stream = std::fstream();

        do {

            l_file_stream.open((*l_itr).second.m_file_path, std::ios::in | std::ios::out | std::ios::binary);

            if (!l_file_stream.is_open())
                break;

            int l_countReading = size / MCDBLOCK_SIZE;

            l_file_stream.seekg(adr, l_file_stream.beg);

            bool l_result_read = true;

            for (size_t i = 0; i < l_countReading; i++) {

                l_result_read = l_file_stream.write(a_data, MCDBLOCK_SIZE).good();

                if (!l_result_read)
                    break;

                a_data += MCDBLOCK_SIZE;

                size -= MCDBLOCK_SIZE;
            }

            if (!l_result_read)
                break;

            if (size > 0)
                l_result_read = l_file_stream.write(a_data, size).good();

            if (!l_result_read)
                break;

            l_result = TRUE;

        } while (false);

		if (l_file_stream.is_open())
            l_file_stream.close();
    }

    return l_result;
}

uint8 MemoryCard::readFromMcd(int a_port, char *a_data, uint32_t adr, int size)
{
    uint8 l_result = FALSE;

    auto l_itr = m_McdFiles.find(a_port);

    if (l_itr != m_McdFiles.end()) {

        std::fstream l_file_stream = std::fstream();

        do {

            l_file_stream.open((*l_itr).second.m_file_path, std::ios::in | std::ios::out | std::ios::binary);

            if (!l_file_stream.is_open())
                break;

            int l_countReading = size / MCDBLOCK_SIZE;

            l_file_stream.seekg(adr, l_file_stream.beg);
						
			bool l_result_read = true;

			for (size_t i = 0; i < l_countReading; i++) {

                l_result_read = l_file_stream.read(a_data, MCDBLOCK_SIZE).good();

				if (!l_result_read)
                    break;

				a_data += MCDBLOCK_SIZE;

				size -= MCDBLOCK_SIZE;
            }

            if (!l_result_read)
                break;

			if (size > 0)
                l_result_read = l_file_stream.read(a_data, size).good();

            if (!l_result_read)
                break;
			
            l_result = TRUE;

        } while (false);

        if (l_file_stream.is_open())
            l_file_stream.close();
    }

    return l_result;
}

extern "C" uint8 isMcdPresented(int a_port)
{
    return g_MemoryCard.isMcdPresented(a_port);
}

extern "C" uint8 writeToMcd(int a_port, char *a_data, uint32_t adr, int size)
{
    return g_MemoryCard.writeToMcd(a_port, a_data, adr, size);
}

extern "C" uint8 readFromMcd(int a_port, char *a_data, uint32_t adr, int size)
{
    return g_MemoryCard.readFromMcd(a_port, a_data, adr, size);
}

extern "C" uint8 formatMcd(int a_port)
{
    return g_MemoryCard.formatMcd(a_port);
}

extern "C" void __stdcall SetMcd(const wchar_t *a_filePath, int a_port)
{
    g_MemoryCard.setMcd(a_filePath, a_port);
}

