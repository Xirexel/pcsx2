package com.evgenypereguda.omegared.Util;

import java.util.BitSet;

public class Util {

    public static int bits2Int(BitSet bs) {
        int temp = 0;

        for (int j = 0; j < 32; j++)
            if (bs.get(0 * 32 + j))
                temp |= 1 << j;

        return temp;
    }
}
