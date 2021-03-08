//
// Created by Evgeny Pereguda on 8/29/2019.
//

#include "SIMDEmitter.h"

extern void bind_simd_soft();

void SIMDEmitter::init() {
	static SIMDEmitter l_Instance;
}

SIMDEmitter::SIMDEmitter() {

	bind_simd_soft();

}

SIMDEmitter::~SIMDEmitter() {

}
