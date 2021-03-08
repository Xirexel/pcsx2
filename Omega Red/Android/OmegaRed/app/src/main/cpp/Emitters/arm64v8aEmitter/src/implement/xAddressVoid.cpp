//
// Created by evgen on 9/19/2020.
//


#include "../../include/PrecompiledHeader.h"
#include "../../include/types.h"


namespace x86Emitter
{
    // --------------------------------------------------------------------------------------
//  xAddressVoid  (method implementations)
// --------------------------------------------------------------------------------------

    xAddressVoid::xAddressVoid(const xAddressReg &base, const xAddressReg &index, int factor, sptr displacement, x86Emitter::DataType dataType)
    {
        Base = base;
        Index = index;
        Factor = factor;
        Displacement = displacement;
        DataType = dataType;
    }

    xAddressVoid::xAddressVoid(const xAddressReg &index, sptr displacement)
    {
        Base = xEmptyReg;
        Index = index;
        Factor = 0;
        Displacement = displacement;
    }


    xAddressVoid::xAddressVoid(const void *displacement)
    {
        throw L"Unimplemented!!!";
        Base = xEmptyReg;
        Index = xEmptyReg;
        Factor = 0;
        Displacement = (sptr)displacement;
    }





    xAddressVoid &xAddressVoid::Add(const xAddressReg &src)
    {
        if (src == Index) {
            Factor++;
        } else if (src == Base) {
            throw L"Unimplemented!!!";
            // Compound the existing register reference into the Index/Scale pair.
            Base = xEmptyReg;

            if (src == Index)
                Factor++;
            else {
                pxAssertDev(Index.IsEmpty(), "x86Emitter: Only one scaled index register is allowed in an address modifier.");
                Index = src;
                Factor = 2;
            }
        } else if (Base.IsEmpty())
            Base = src;
        else if (Index.IsEmpty())
        {
            throw L"Unimplemented!!!";
            Index = src;
        }
        else
        {
            throw L"Unimplemented!!!";
            pxAssumeDev(false, L"x86Emitter: address modifiers cannot have more than two index registers."); // oops, only 2 regs allowed per ModRm!
        }

        return *this;
    }

    xAddressVoid &xAddressVoid::Add(const xAddressVoid &src)
    {
        throw L"Unimplemented!!!";
        Add(src.Base);
        Add(src.Displacement);

        // If the factor is 1, we can just treat index like a base register also.
        if (src.Factor == 1) {
            Add(src.Index);
        } else if (Index.IsEmpty()) {
            Index = src.Index;
            Factor = src.Factor;
        } else if (Index == src.Index) {
            Factor += src.Factor;
        } else
            pxAssumeDev(false, L"x86Emitter: address modifiers cannot have more than two index registers."); // oops, only 2 regs allowed per ModRm!

        return *this;
    }
}