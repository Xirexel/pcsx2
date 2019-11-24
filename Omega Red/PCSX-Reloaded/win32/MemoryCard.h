#pragma once

#include <string>
#include <vector>
#include <map>
#include <fstream>


class MemoryCard
{
public:

	MemoryCard();

	virtual ~MemoryCard();

	uint8 isMcdPresented(int a_port);
	
	uint8 writeToMcd(int a_port, char *a_data, uint32_t adr, int size);

	uint8 readFromMcd(int a_port, char *a_data, uint32_t adr, int size);

	uint8 formatMcd(int a_port);

	void setMcd(const wchar_t *a_filePath, int a_port);
	
private:
    void open();

	void createMcd(const wchar_t* a_filePath);

	uint8 checkExistence(const wchar_t *a_filePath);
};