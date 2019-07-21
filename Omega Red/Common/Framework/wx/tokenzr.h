
#pragma once


// wxStringTokenizer mode flags which determine its behaviour
enum wxStringTokenizerMode
{
	wxTOKEN_INVALID = -1,   // set by def ctor until SetString() is called
	wxTOKEN_DEFAULT,        // strtok() for whitespace delims, RET_EMPTY else
	wxTOKEN_RET_EMPTY,      // return empty token in the middle of the string
	wxTOKEN_RET_EMPTY_ALL,  // return trailing empty tokens too
	wxTOKEN_RET_DELIMS,     // return the delim with token (implies RET_EMPTY)
	wxTOKEN_STRTOK          // behave exactly like strtok(3)
};

class wxStringTokenizer
{
	wxArrayString m_wxArrayString;

	int m_cont = 0;

    void parsString(const wxString &str, const wxChar &delim)
    {

        std::wstring l_str(str);

        size_t l_pos = l_str.find(delim);

        while (l_pos != std::wstring::npos) {
            auto l_tempStr = l_str.substr(0, l_pos);

            m_wxArrayString.Add(l_tempStr.c_str());

            l_str = l_str.substr(l_pos + 1, l_str.size());

            l_pos = l_str.find(delim);
        }

        if (l_str.size() > 0)
            m_wxArrayString.Add(l_str.c_str());
    }

public:

	wxStringTokenizer(const wxString &str,
                                         const wxString &delims,
                                         wxStringTokenizerMode mode)
    {
        std::wstring l_delims(delims);

		for (auto &l_delim : l_delims) {
            parsString(str, l_delim);
		}
    }

	wxStringTokenizer(const wxString& str, const wxChar& delim){
        parsString(str, delim);
	}

	bool HasMoreTokens()
	{
		return !(m_cont >= m_wxArrayString.GetCount());
	}

	wxString GetNextToken(){
		
		if (m_cont >= m_wxArrayString.GetCount())
			return wxString();

		return m_wxArrayString[m_cont++];
	}
};