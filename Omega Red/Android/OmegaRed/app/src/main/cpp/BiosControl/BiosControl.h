//
// Created by Evgeny Pereguda on 7/14/2019.
//

#ifndef OMEGARED_BIOSCONTROL_H
#define OMEGARED_BIOSCONTROL_H

#include <string>


class BiosControl {

public:

	BiosControl();

	virtual ~BiosControl();


	static bool IsBIOS(
		const std::string& filename,
		std::string& zone,
		std::string& version,
		int& versionInt,
		std::string& data,
		std::string& build);

	static unsigned int getBIOSChecksum(
		const std::string& filename);

	static void loadBIOS(
		const std::string& filename,
		void* aPtrTarget,
		int aReadLength);

};


#endif //OMEGARED_BIOSCONTROL_H
