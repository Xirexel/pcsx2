/*  Framework - Framework stub for Omega Red PS2 Emulator for PCs
*
*  Framework is free software: you can redistribute it and/or modify it under the terms
*  of the GNU Lesser General Public License as published by the Free Software Found-
*  ation, either version 3 of the License, or (at your option) any later version.
*
*  Framework is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
*  without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
*  PURPOSE.  See the GNU General Public License for more details.
*
*  You should have received a copy of the GNU General Public License along with Framework.
*  If not, see <http://www.gnu.org/licenses/>.
*/

#include "longlong.h"

template <typename T, typename X>
inline T wx_truncate_cast_impl(X x)
{
#pragma warning(push)
	/* conversion from 'size_t' to 'type', possible loss of data */
#pragma warning(disable: 4267)
	/* conversion from 'type1' to 'type2', possible loss of data */
#pragma warning(disable: 4242)

	return x;

#pragma warning(pop)
}

#define wx_truncate_cast(t, x)  ((t)(x)) // wx_truncate_cast_impl<t>(x)


wxLongLongNative::wxLongLongNative() : m_ll(0) { }
	// from long long
wxLongLongNative::wxLongLongNative(wxLongLong_t ll) : m_ll(ll) { }
	// from 2 longs
wxLongLongNative::wxLongLongNative(wxInt32 hi, wxUint32 lo)
	{
		// cast to wxLongLong_t first to avoid precision loss!
		m_ll = ((wxLongLong_t)hi) << 32;
		m_ll |= (wxLongLong_t)lo;
	}


	// assignment wxLongLongNative::operators
	// from native 64 bit integer
#ifndef wxLongLongIsLong
	wxLongLongNative& wxLongLongNative::operator=(wxLongLong_t ll)
	{
		m_ll = ll; return *this;
	}
	wxLongLongNative& wxLongLongNative::operator=(wxULongLong_t ll)
	{
		m_ll = ll; return *this;
	}
#endif // !wxLongLongNative

	wxLongLongNative& wxLongLongNative::operator=(int l)
	{
		m_ll = l; return *this;
	}
	wxLongLongNative& wxLongLongNative::operator=(long l)
	{
		m_ll = l; return *this;
	}
	wxLongLongNative& wxLongLongNative::operator=(unsigned int l)
	{
		m_ll = l; return *this;
	}
	wxLongLongNative& wxLongLongNative::operator=(unsigned long l)
	{
		m_ll = l; return *this;
	}
#if wxUSE_LONGLONG_WX
	wxLongLongNative& wxLongLongNative::operator=(wxLongLongWx ll);
	wxLongLongNative& wxLongLongNative::operator=(const class wxULongLongWx &ll);
#endif


	// from double: this one has an explicit name because otherwise we
	// would have ambiguity with "ll = int" and also because we don't want
	// to have implicit conversions between doubles and wxLongLongs
	wxLongLongNative& wxLongLongNative::Assign(double d)
	{
		m_ll = (wxLongLong_t)d; return *this;
	}

	// assignment wxLongLongNative::operators from wxLongLongNative is ok

	// accessors
	// get high part
	wxInt32 wxLongLongNative::GetHi() const
	{
		return wx_truncate_cast(wxInt32, m_ll >> 32);
	}
	// get low part
	wxUint32 wxLongLongNative::GetLo() const
	{
		return wx_truncate_cast(wxUint32, m_ll);
	}

	// get absolute value
	wxLongLongNative wxLongLongNative::Abs() const { return wxLongLongNative(*this).Abs(); }
	wxLongLongNative& wxLongLongNative::Abs() { if (m_ll < 0) m_ll = -m_ll; return *this; }

	// convert to native long long
	wxLongLong_t wxLongLongNative::GetValue() const { return m_ll; }

	// convert to long with range checking in debug mode (only!)
	long wxLongLongNative::ToLong() const
	{
		//wxASSERT_MSG((m_ll >= LONG_MIN) && (m_ll <= LONG_MAX),
		//	wxT("wxLongLong to long conversion loss of precision"));

		return wx_truncate_cast(long, m_ll);
	}

	// convert to double
	double wxLongLongNative::ToDouble() const { return wx_truncate_cast(double, m_ll); }

	// don't provide implicit conversion to wxLongLong_t or we will have an
	// ambiguity for all arithmetic operations
	//wxLongLongNative::operator wxLongLong_t() const { return m_ll; }

	// operations
	// addition
	wxLongLongNative wxLongLongNative::operator+(const wxLongLongNative& ll) const
	{
		return wxLongLongNative(m_ll + ll.m_ll);
	}
	wxLongLongNative& wxLongLongNative::operator+=(const wxLongLongNative& ll)
	{
		m_ll += ll.m_ll; return *this;
	}

	wxLongLongNative wxLongLongNative::operator+(const wxLongLong_t ll) const
	{
		return wxLongLongNative(m_ll + ll);
	}
	wxLongLongNative& wxLongLongNative::operator+=(const wxLongLong_t ll)
	{
		m_ll += ll; return *this;
	}

	// pre increment
	wxLongLongNative& wxLongLongNative::operator++()
	{
		m_ll++; return *this;
	}

	// post increment
	wxLongLongNative wxLongLongNative::operator++(int)
	{
		wxLongLongNative value(*this); m_ll++; return value;
	}

	// negation wxLongLongNative::operator
	wxLongLongNative wxLongLongNative::operator-() const
	{
		return wxLongLongNative(-m_ll);
	}
	wxLongLongNative& wxLongLongNative::Negate() { m_ll = -m_ll; return *this; }

	// subtraction
	wxLongLongNative wxLongLongNative::operator-(const wxLongLongNative& ll) const
	{
		return wxLongLongNative(m_ll - ll.m_ll);
	}
	wxLongLongNative& wxLongLongNative::operator-=(const wxLongLongNative& ll)
	{
		m_ll -= ll.m_ll; return *this;
	}

	wxLongLongNative wxLongLongNative::operator-(const wxLongLong_t ll) const
	{
		return wxLongLongNative(m_ll - ll);
	}
	wxLongLongNative& wxLongLongNative::operator-=(const wxLongLong_t ll)
	{
		m_ll -= ll; return *this;
	}

	// pre decrement
	wxLongLongNative& wxLongLongNative::operator--()
	{
		m_ll--; return *this;
	}

	// post decrement
	wxLongLongNative wxLongLongNative::operator--(int)
	{
		wxLongLongNative value(*this); m_ll--; return value;
	}

	// shifts
	// left shift
	wxLongLongNative wxLongLongNative::operator<<(int shift) const
	{
		return wxLongLongNative(m_ll << shift);
	}
	wxLongLongNative& wxLongLongNative::operator<<=(int shift)
	{
		m_ll <<= shift; return *this;
	}

	// right shift
	wxLongLongNative wxLongLongNative::operator>>(int shift) const
	{
		return wxLongLongNative(m_ll >> shift);
	}
	wxLongLongNative& wxLongLongNative::operator>>=(int shift)
	{
		m_ll >>= shift; return *this;
	}

	// bitwise wxLongLongNative::operators
	wxLongLongNative wxLongLongNative::operator&(const wxLongLongNative& ll) const
	{
		return wxLongLongNative(m_ll & ll.m_ll);
	}
	wxLongLongNative& wxLongLongNative::operator&=(const wxLongLongNative& ll)
	{
		m_ll &= ll.m_ll; return *this;
	}

	wxLongLongNative wxLongLongNative::operator|(const wxLongLongNative& ll) const
	{
		return wxLongLongNative(m_ll | ll.m_ll);
	}
	wxLongLongNative& wxLongLongNative::operator|=(const wxLongLongNative& ll)
	{
		m_ll |= ll.m_ll; return *this;
	}

	wxLongLongNative wxLongLongNative::operator^(const wxLongLongNative& ll) const
	{
		return wxLongLongNative(m_ll ^ ll.m_ll);
	}
	wxLongLongNative& wxLongLongNative::operator^=(const wxLongLongNative& ll)
	{
		m_ll ^= ll.m_ll; return *this;
	}

	// multiplication/division
	wxLongLongNative wxLongLongNative::operator*(const wxLongLongNative& ll) const
	{
		return wxLongLongNative(m_ll * ll.m_ll);
	}
	wxLongLongNative wxLongLongNative::operator*(long l) const
	{
		return wxLongLongNative(m_ll * l);
	}
	wxLongLongNative& wxLongLongNative::operator*=(const wxLongLongNative& ll)
	{
		m_ll *= ll.m_ll; return *this;
	}
	wxLongLongNative& wxLongLongNative::operator*=(long l)
	{
		m_ll *= l; return *this;
	}

	wxLongLongNative wxLongLongNative::operator/(const wxLongLongNative& ll) const
	{
		return wxLongLongNative(m_ll / ll.m_ll);
	}
	wxLongLongNative wxLongLongNative::operator/(long l) const
	{
		return wxLongLongNative(m_ll / l);
	}
	wxLongLongNative& wxLongLongNative::operator/=(const wxLongLongNative& ll)
	{
		m_ll /= ll.m_ll; return *this;
	}
	wxLongLongNative& wxLongLongNative::operator/=(long l)
	{
		m_ll /= l; return *this;
	}

	wxLongLongNative wxLongLongNative::operator%(const wxLongLongNative& ll) const
	{
		return wxLongLongNative(m_ll % ll.m_ll);
	}
	wxLongLongNative wxLongLongNative::operator%(long l) const
	{
		return wxLongLongNative(m_ll % l);
	}

	// comparison
	bool wxLongLongNative::operator==(const wxLongLongNative& ll) const
	{
		return m_ll == ll.m_ll;
	}
	bool wxLongLongNative::operator==(long l) const
	{
		return m_ll == l;
	}
	bool wxLongLongNative::operator!=(const wxLongLongNative& ll) const
	{
		return m_ll != ll.m_ll;
	}
	bool wxLongLongNative::operator!=(long l) const
	{
		return m_ll != l;
	}
	bool wxLongLongNative::operator<(const wxLongLongNative& ll) const
	{
		return m_ll < ll.m_ll;
	}
	bool wxLongLongNative::operator<(long l) const
	{
		return m_ll < l;
	}
	bool wxLongLongNative::operator>(const wxLongLongNative& ll) const
	{
		return m_ll > ll.m_ll;
	}
	bool wxLongLongNative::operator>(long l) const
	{
		return m_ll > l;
	}
	bool wxLongLongNative::operator<=(const wxLongLongNative& ll) const
	{
		return m_ll <= ll.m_ll;
	}
	bool wxLongLongNative::operator<=(long l) const
	{
		return m_ll <= l;
	}
	bool wxLongLongNative::operator>=(const wxLongLongNative& ll) const
	{
		return m_ll >= ll.m_ll;
	}
	bool wxLongLongNative::operator>=(long l) const
	{
		return m_ll >= l;
	}


wxULongLongNative::wxULongLongNative() : m_ll(0) { }
	// from long long
wxULongLongNative::wxULongLongNative(wxULongLong_t ll) : m_ll(ll) { }
	// from 2 longs
wxULongLongNative::wxULongLongNative(wxUint32 hi, wxUint32 lo) : m_ll(0)
	{
		// cast to wxLongLong_t first to avoid precision loss!
		m_ll = ((wxULongLong_t)hi) << 32;
		m_ll |= (wxULongLong_t)lo;
	}

#if wxUSE_LONGLONG_WX
	wxULongLongNative(const class wxULongLongWx &ll);
#endif

	// default copy ctor is ok

	// no dtor

	// assignment wxLongLongNative::operators
	// from native 64 bit integer
#ifndef wxLongLongIsLong
	wxULongLongNative& wxULongLongNative::operator=(wxULongLong_t ll)
	{
		m_ll = ll; return *this;
	}
	wxULongLongNative& wxULongLongNative::operator=(wxLongLong_t ll)
	{
		m_ll = ll; return *this;
	}
#endif // !wxULongLongNative
	wxULongLongNative& wxULongLongNative::operator=(int l)
	{
		m_ll = l; return *this;
	}
	wxULongLongNative& wxULongLongNative::operator=(long l)
	{
		m_ll = l; return *this;
	}
	wxULongLongNative& wxULongLongNative::operator=(unsigned int l)
	{
		m_ll = l; return *this;
	}
	wxULongLongNative& wxULongLongNative::operator=(unsigned long l)
	{
		m_ll = l; return *this;
	}

#if wxUSE_LONGLONG_WX
	wxULongLongNative& wxULongLongNative::operator=(wxLongLongWx ll);
	wxULongLongNative& wxULongLongNative::operator=(const class wxULongLongWx &ll);
#endif

	// assignment wxULongLongNative::operators from wxULongLongNative is ok

	// accessors
	// get high part
	wxUint32 wxULongLongNative::GetHi() const
	{
		return wx_truncate_cast(wxUint32, m_ll >> 32);
	}
	// get low part
	wxUint32 wxULongLongNative::GetLo() const
	{
		return wx_truncate_cast(wxUint32, m_ll);
	}

	// convert to native ulong long
	wxULongLong_t wxULongLongNative::GetValue() const { return m_ll; }

	// convert to ulong with range checking in debug mode (only!)
	unsigned long wxULongLongNative::ToULong() const
	{
		//wxASSERT_MSG(m_ll <= ULONG_MAX,
		//	wxT("wxULongLong to long conversion loss of precision"));

		return wx_truncate_cast(unsigned long, m_ll);
	}

	// convert to double
	//
	// For some completely obscure reasons compiling the cast below with
	// VC6 in DLL builds only (!) results in "error C2520: conversion from
	// unsigned __int64 to double not implemented, use signed __int64" so
	// we must use a different version for that compiler.

	double wxULongLongNative::ToDouble() const { return wx_truncate_cast(double, m_ll); }


	// operations
	// addition
	wxULongLongNative wxULongLongNative::operator+(const wxULongLongNative& ll) const
	{
		return wxULongLongNative(m_ll + ll.m_ll);
	}
	wxULongLongNative& wxULongLongNative::operator+=(const wxULongLongNative& ll)
	{
		m_ll += ll.m_ll; return *this;
	}

	wxULongLongNative wxULongLongNative::operator+(const wxULongLong_t ll) const
	{
		return wxULongLongNative(m_ll + ll);
	}
	wxULongLongNative& wxULongLongNative::operator+=(const wxULongLong_t ll)
	{
		m_ll += ll; return *this;
	}

	// pre increment
	wxULongLongNative& wxULongLongNative::operator++()
	{
		m_ll++; return *this;
	}

	// post increment
	wxULongLongNative wxULongLongNative::operator++(int)
	{
		wxULongLongNative value(*this); m_ll++; return value;
	}

	// subtraction
	wxULongLongNative wxULongLongNative::operator-(const wxULongLongNative& ll) const
	{
		return wxULongLongNative(m_ll - ll.m_ll);
	}
	wxULongLongNative& wxULongLongNative::operator-=(const wxULongLongNative& ll)
	{
		m_ll -= ll.m_ll; return *this;
	}

	wxULongLongNative wxULongLongNative::operator-(const wxULongLong_t ll) const
	{
		return wxULongLongNative(m_ll - ll);
	}
	wxULongLongNative& wxULongLongNative::operator-=(const wxULongLong_t ll)
	{
		m_ll -= ll; return *this;
	}

	// pre decrement
	wxULongLongNative& wxULongLongNative::operator--()
	{
		m_ll--; return *this;
	}

	// post decrement
	wxULongLongNative wxULongLongNative::operator--(int)
	{
		wxULongLongNative value(*this); m_ll--; return value;
	}

	// shifts
	// left shift
	wxULongLongNative wxULongLongNative::operator<<(int shift) const
	{
		return wxULongLongNative(m_ll << shift);
	}
	wxULongLongNative& wxULongLongNative::operator<<=(int shift)
	{
		m_ll <<= shift; return *this;
	}

	// right shift
	wxULongLongNative wxULongLongNative::operator>>(int shift) const
	{
		return wxULongLongNative(m_ll >> shift);
	}
	wxULongLongNative& wxULongLongNative::operator>>=(int shift)
	{
		m_ll >>= shift; return *this;
	}

	// bitwise wxULongLongNative::operators
	wxULongLongNative wxULongLongNative::operator&(const wxULongLongNative& ll) const
	{
		return wxULongLongNative(m_ll & ll.m_ll);
	}
	wxULongLongNative& wxULongLongNative::operator&=(const wxULongLongNative& ll)
	{
		m_ll &= ll.m_ll; return *this;
	}

	wxULongLongNative wxULongLongNative::operator|(const wxULongLongNative& ll) const
	{
		return wxULongLongNative(m_ll | ll.m_ll);
	}
	wxULongLongNative& wxULongLongNative::operator|=(const wxULongLongNative& ll)
	{
		m_ll |= ll.m_ll; return *this;
	}

	wxULongLongNative wxULongLongNative::operator^(const wxULongLongNative& ll) const
	{
		return wxULongLongNative(m_ll ^ ll.m_ll);
	}
	wxULongLongNative& wxULongLongNative::operator^=(const wxULongLongNative& ll)
	{
		m_ll ^= ll.m_ll; return *this;
	}

	// multiplication/division
	wxULongLongNative wxULongLongNative::operator*(const wxULongLongNative& ll) const
	{
		return wxULongLongNative(m_ll * ll.m_ll);
	}
	wxULongLongNative wxULongLongNative::operator*(unsigned long l) const
	{
		return wxULongLongNative(m_ll * l);
	}
	wxULongLongNative& wxULongLongNative::operator*=(const wxULongLongNative& ll)
	{
		m_ll *= ll.m_ll; return *this;
	}
	wxULongLongNative& wxULongLongNative::operator*=(unsigned long l)
	{
		m_ll *= l; return *this;
	}

	wxULongLongNative wxULongLongNative::operator/(const wxULongLongNative& ll) const
	{
		return wxULongLongNative(m_ll / ll.m_ll);
	}
	wxULongLongNative wxULongLongNative::operator/(unsigned long l) const
	{
		return wxULongLongNative(m_ll / l);
	}
	wxULongLongNative& wxULongLongNative::operator/=(const wxULongLongNative& ll)
	{
		m_ll /= ll.m_ll; return *this;
	}
	wxULongLongNative& wxULongLongNative::operator/=(unsigned long l)
	{
		m_ll /= l; return *this;
	}

	wxULongLongNative wxULongLongNative::operator%(const wxULongLongNative& ll) const
	{
		return wxULongLongNative(m_ll % ll.m_ll);
	}
	wxULongLongNative wxULongLongNative::operator%(unsigned long l) const
	{
		return wxULongLongNative(m_ll % l);
	}

	// comparison
	bool wxULongLongNative::operator==(const wxULongLongNative& ll) const
	{
		return m_ll == ll.m_ll;
	}
	bool wxULongLongNative::operator==(unsigned long l) const
	{
		return m_ll == l;
	}
	bool wxULongLongNative::operator!=(const wxULongLongNative& ll) const
	{
		return m_ll != ll.m_ll;
	}
	bool wxULongLongNative::operator!=(unsigned long l) const
	{
		return m_ll != l;
	}
	bool wxULongLongNative::operator<(const wxULongLongNative& ll) const
	{
		return m_ll < ll.m_ll;
	}
	bool wxULongLongNative::operator<(unsigned long l) const
	{
		return m_ll < l;
	}
	bool wxULongLongNative::operator>(const wxULongLongNative& ll) const
	{
		return m_ll > ll.m_ll;
	}
	bool wxULongLongNative::operator>(unsigned long l) const
	{
		return m_ll > l;
	}
	bool wxULongLongNative::operator<=(const wxULongLongNative& ll) const
	{
		return m_ll <= ll.m_ll;
	}
	bool wxULongLongNative::operator<=(unsigned long l) const
	{
		return m_ll <= l;
	}
	bool wxULongLongNative::operator>=(const wxULongLongNative& ll) const
	{
		return m_ll >= ll.m_ll;
	}
	bool wxULongLongNative::operator>=(unsigned long l) const
	{
		return m_ll >= l;
	}