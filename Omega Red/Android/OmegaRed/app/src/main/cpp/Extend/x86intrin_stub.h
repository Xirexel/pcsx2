//
// Created by Xirexel on 4/14/2019.
//

#ifndef OMEGARED_X86INTRIN_STUB_H
#define OMEGARED_X86INTRIN_STUB_H


extern unsigned int __builtin_ia32_readeflags_u32();
extern void __builtin_ia32_writeeflags_u32(unsigned int f);
extern unsigned long long __builtin_ia32_rdpmc(int __A);
extern unsigned long long __builtin_ia32_rdtscp(unsigned int *__A);
extern void __builtin_ia32_wbinvd();
extern void __builtin_ia32_emms();
//extern int __builtin_ia32_vec_init_v2si(int __i1, int __i2);





#endif //OMEGARED_X86INTRIN_STUB_H

