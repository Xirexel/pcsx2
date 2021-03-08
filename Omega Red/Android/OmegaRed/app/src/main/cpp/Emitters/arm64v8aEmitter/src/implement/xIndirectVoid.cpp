//
// Created by evgen on 9/15/2020.
//

#include "../../include/PrecompiledHeader.h"
#include "../../include/types.h"


namespace x86Emitter
{
    xIndirectVoid::xIndirectVoid(sptr disp, uint a_byteSize)
    {
        Base = xEmptyReg;
        Index = xEmptyReg;
        Scale = 0;
        Displacement = disp;
        DataType = setType(a_byteSize);
    }

    xIndirectVoid::xIndirectVoid(const xAddressVoid &src)
    {
        Base = src.Base;
        Index = src.Index;
        Scale = src.Factor;
        Displacement = src.Displacement;
        DataType = src.DataType;

        Reduce();
    }

    xIndirectVoid::xIndirectVoid(xAddressReg base, xAddressReg index, int scale, sptr displacement, x86Emitter::DataType dataType)
    {
        Base = base;
        Index = index;
        Scale = scale;
        Displacement = displacement;
        DataType = dataType;

        Reduce();
    }

    // Generates a 'reduced' ModSib form, which has valid Base, Index, and Scale values.
    // Necessary because by default ModSib compounds registers into Index when possible.
    //
    // If the ModSib is in illegal form ([Base + Index*5] for example) then an assertion
    // followed by an InvalidParameter Exception will be tossed around in haphazard
    // fashion.
    //
    // Optimization Note: Currently VC does a piss poor job of inlining this, even though
    // constant propagation *should* resove it to little or no code (VC's constprop fails
    // on C++ class initializers).  There is a work around [using array initializers instead]
    // but it's too much trouble for code that isn't performance critical anyway.
    // And, with luck, maybe VC10 will optimize it better and make it a non-issue. :D
    //
    void xIndirectVoid::Reduce()
    {
        if (Index.IsStackPointer()) {
            // esp cannot be encoded as the index, so move it to the Base, if possible.
            // note: intentionally leave index assigned to esp also (generates correct
            // encoding later, since ESP cannot be encoded 'alone')

            pxAssert(Scale == 0);     // esp can't have an index modifier!
            pxAssert(Base.IsEmpty()); // base must be empty or else!

            Base = Index;
            return;
        }

        // If no index reg, then load the base register into the index slot.
        if (Index.IsEmpty()) {
            Index = Base;
            Scale = 0;
            if (!Base.IsStackPointer()) // prevent ESP from being encoded 'alone'
                Base = xEmptyReg;
            return;
        }

        // The Scale has a series of valid forms, all shown here:

        switch (Scale) {
            case 0:
                break;
            case 1:
                Scale = 0;
                break;
            case 2:
                Scale = 1;
                break;

            case 3: // becomes [reg*2+reg]
                pxAssertDev(Base.IsEmpty(), "Cannot scale an Index register by 3 when Base is not empty!");
                Base = Index;
                Scale = 1;
                break;

            case 4:
                Scale = 2;
                break;

            case 5: // becomes [reg*4+reg]
                pxAssertDev(Base.IsEmpty(), "Cannot scale an Index register by 5 when Base is not empty!");
                Base = Index;
                Scale = 2;
                break;

            case 6: // invalid!
                pxAssumeDev(false, "x86 asm cannot scale a register by 6.");
                break;

            case 7: // so invalid!
                pxAssumeDev(false, "x86 asm cannot scale a register by 7.");
                break;

            case 8:
                Scale = 3;
                break;
            case 9: // becomes [reg*8+reg]
                pxAssertDev(Base.IsEmpty(), "Cannot scale an Index register by 9 when Base is not empty!");
                Base = Index;
                Scale = 3;
                break;

                jNO_DEFAULT
        }
    }

    uint xIndirectVoid::GetOperandSize() const
    {
        pxFailDev("Invalid operation on xIndirectVoid");
        return 0;
    }

    xIndirectVoid &xIndirectVoid::Add(sptr imm)
    {
        Displacement += imm;
        return *this;
    }
}