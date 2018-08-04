
#pragma once


#define wxLongLong_t __int64

#define wxULongLong_t unsigned wxLongLong_t

typedef int wxInt32;
typedef unsigned int wxUint32;



class wxLongLongNative
{
public:
	// ctors
	// default ctor initializes to 0
	wxLongLongNative();
	// from long long
	wxLongLongNative(wxLongLong_t ll);
	// from 2 longs
	wxLongLongNative(wxInt32 hi, wxUint32 lo);

	// default copy ctor is ok

	// no dtor

	// assignment operators
	// from native 64 bit integer
#ifndef wxLongLongIsLong
	wxLongLongNative& operator=(wxLongLong_t ll);
	wxLongLongNative& operator=(wxULongLong_t ll);
#endif // !wxLongLongNative
	//wxLongLongNative& operator=(const wxULongLongNative &ll)
	//{
	//	m_ll = ll.GetValue();
	//	return *this;
	//}
	wxLongLongNative& operator=(int l);
	wxLongLongNative& operator=(long l);
	wxLongLongNative& operator=(unsigned int l);
	wxLongLongNative& operator=(unsigned long l);
#if wxUSE_LONGLONG_WX
	wxLongLongNative& operator=(wxLongLongWx ll);
	wxLongLongNative& operator=(const class wxULongLongWx &ll);
#endif


	// from double: this one has an explicit name because otherwise we
	// would have ambiguity with "ll = int" and also because we don't want
	// to have implicit conversions between doubles and wxLongLongs
	wxLongLongNative& Assign(double d);

	// assignment operators from wxLongLongNative is ok

	// accessors
	// get high part
	wxInt32 GetHi() const;
	// get low part
	wxUint32 GetLo() const;

	// get absolute value
	wxLongLongNative Abs() const;
	wxLongLongNative& Abs();

	// convert to native long long
	wxLongLong_t GetValue() const;

	// convert to long with range checking in debug mode (only!)
	long ToLong() const;

	// convert to double
	double ToDouble() const;

	// don't provide implicit conversion to wxLongLong_t or we will have an
	// ambiguity for all arithmetic operations
	//operator wxLongLong_t() const { return m_ll; }

	// operations
	// addition
	wxLongLongNative operator+(const wxLongLongNative& ll) const;
	wxLongLongNative& operator+=(const wxLongLongNative& ll);

	wxLongLongNative operator+(const wxLongLong_t ll) const;
	wxLongLongNative& operator+=(const wxLongLong_t ll);

	// pre increment
	wxLongLongNative& operator++();

	// post increment
	wxLongLongNative operator++(int);

	// negation operator
	wxLongLongNative operator-() const;
	wxLongLongNative& Negate();

	// subtraction
	wxLongLongNative operator-(const wxLongLongNative& ll) const;
	wxLongLongNative& operator-=(const wxLongLongNative& ll);

	wxLongLongNative operator-(const wxLongLong_t ll) const;
	wxLongLongNative& operator-=(const wxLongLong_t ll);

	// pre decrement
	wxLongLongNative& operator--();

	// post decrement
	wxLongLongNative operator--(int);

	// shifts
	// left shift
	wxLongLongNative operator<<(int shift) const;
	wxLongLongNative& operator<<=(int shift);

	// right shift
	wxLongLongNative operator>>(int shift) const;
	wxLongLongNative& operator>>=(int shift);

	// bitwise operators
	wxLongLongNative operator&(const wxLongLongNative& ll) const;
	wxLongLongNative& operator&=(const wxLongLongNative& ll);

	wxLongLongNative operator|(const wxLongLongNative& ll) const;
	wxLongLongNative& operator|=(const wxLongLongNative& ll);

	wxLongLongNative operator^(const wxLongLongNative& ll) const;
	wxLongLongNative& operator^=(const wxLongLongNative& ll);

	// multiplication/division
	wxLongLongNative operator*(const wxLongLongNative& ll) const;
	wxLongLongNative operator*(long l) const;
	wxLongLongNative& operator*=(const wxLongLongNative& ll);
	wxLongLongNative& operator*=(long l);

	wxLongLongNative operator/(const wxLongLongNative& ll) const;
	wxLongLongNative operator/(long l) const;
	wxLongLongNative& operator/=(const wxLongLongNative& ll);
	wxLongLongNative& operator/=(long l);

	wxLongLongNative operator%(const wxLongLongNative& ll) const;
	wxLongLongNative operator%(long l) const;

	// comparison
	bool operator==(const wxLongLongNative& ll) const;
	bool operator==(long l) const;
	bool operator!=(const wxLongLongNative& ll) const;
	bool operator!=(long l) const;
	bool operator<(const wxLongLongNative& ll) const;
	bool operator<(long l) const;
	bool operator>(const wxLongLongNative& ll) const;
	bool operator>(long l) const;
	bool operator<=(const wxLongLongNative& ll) const;
	bool operator<=(long l) const;
	bool operator>=(const wxLongLongNative& ll) const;
	bool operator>=(long l) const;
	
private:
	wxLongLong_t  m_ll;
};

class wxULongLongNative
{
public:
	// ctors
	// default ctor initializes to 0
	wxULongLongNative();
	// from long long
	wxULongLongNative(wxULongLong_t ll);
	// from 2 longs
	wxULongLongNative(wxUint32 hi, wxUint32 lo);
	
	// default copy ctor is ok

	// no dtor

	// assignment operators
	// from native 64 bit integer
#ifndef wxLongLongIsLong
	wxULongLongNative& operator=(wxULongLong_t ll);
	wxULongLongNative& operator=(wxLongLong_t ll);
#endif // !wxLongLongNative
	wxULongLongNative& operator=(int l);
	wxULongLongNative& operator=(long l);
	wxULongLongNative& operator=(unsigned int l);
	wxULongLongNative& operator=(unsigned long l);
	wxULongLongNative& operator=(const wxLongLongNative &ll);

	// assignment operators from wxULongLongNative is ok

	// accessors
	// get high part
	wxUint32 GetHi() const;
	// get low part
	wxUint32 GetLo() const;

	// convert to native ulong long
	wxULongLong_t GetValue() const;

	// convert to ulong with range checking in debug mode (only!)
	unsigned long ToULong() const;

	// convert to double
	//
	// For some completely obscure reasons compiling the cast below with
	// VC6 in DLL builds only (!) results in "error C2520: conversion from
	// unsigned __int64 to double not implemented, use signed __int64" so
	// we must use a different version for that compiler.
#ifdef __VISUALC6__
	double ToDouble() const;
#else
	double ToDouble() const;
#endif

	// operations
	// addition
	wxULongLongNative operator+(const wxULongLongNative& ll) const;
	wxULongLongNative& operator+=(const wxULongLongNative& ll);

	wxULongLongNative operator+(const wxULongLong_t ll) const;
	wxULongLongNative& operator+=(const wxULongLong_t ll);

	// pre increment
	wxULongLongNative& operator++();

	// post increment
	wxULongLongNative operator++(int);

	// subtraction
	wxULongLongNative operator-(const wxULongLongNative& ll) const;
	wxULongLongNative& operator-=(const wxULongLongNative& ll);

	wxULongLongNative operator-(const wxULongLong_t ll) const;
	wxULongLongNative& operator-=(const wxULongLong_t ll);

	// pre decrement
	wxULongLongNative& operator--();

	// post decrement
	wxULongLongNative operator--(int);

	// shifts
	// left shift
	wxULongLongNative operator<<(int shift) const;
	wxULongLongNative& operator<<=(int shift);

	// right shift
	wxULongLongNative operator>>(int shift) const;
	wxULongLongNative& operator>>=(int shift);

	// bitwise operators
	wxULongLongNative operator&(const wxULongLongNative& ll) const;
	wxULongLongNative& operator&=(const wxULongLongNative& ll);

	wxULongLongNative operator|(const wxULongLongNative& ll) const;
	wxULongLongNative& operator|=(const wxULongLongNative& ll);

	wxULongLongNative operator^(const wxULongLongNative& ll) const;
	wxULongLongNative& operator^=(const wxULongLongNative& ll);

	// multiplication/division
	wxULongLongNative operator*(const wxULongLongNative& ll) const;
	wxULongLongNative operator*(unsigned long l) const;
	wxULongLongNative& operator*=(const wxULongLongNative& ll);
	wxULongLongNative& operator*=(unsigned long l);

	wxULongLongNative operator/(const wxULongLongNative& ll) const;
	wxULongLongNative operator/(unsigned long l) const;
	wxULongLongNative& operator/=(const wxULongLongNative& ll);
	wxULongLongNative& operator/=(unsigned long l);

	wxULongLongNative operator%(const wxULongLongNative& ll) const;
	wxULongLongNative operator%(unsigned long l) const;

	// comparison
	bool operator==(const wxULongLongNative& ll) const;
	bool operator==(unsigned long l) const;
	bool operator!=(const wxULongLongNative& ll) const;
	bool operator!=(unsigned long l) const;
	bool operator<(const wxULongLongNative& ll) const;
	bool operator<(unsigned long l) const;
	bool operator>(const wxULongLongNative& ll) const;
	bool operator>(unsigned long l) const;
	bool operator<=(const wxULongLongNative& ll) const;
	bool operator<=(unsigned long l) const;
	bool operator>=(const wxULongLongNative& ll) const;
	bool operator>=(unsigned long l) const;
	
private:
	wxULongLong_t  m_ll;
};

//inline
//wxLongLongNative& wxLongLongNative::operator=(const wxULongLongNative &ll)
//{
//	m_ll = ll.GetValue();
//	return *this;
//}

typedef wxULongLongNative wxULongLong;