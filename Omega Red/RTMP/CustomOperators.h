
#include <vector>
#include <string>
#include "SimpleBinStream.h"

struct Product
{
	Product() : product_name(""), price(0.0f), qty(0) {}
	Product(const std::string& name, float _price, int _qty) : product_name(name), price(_price), qty(_qty) {}
	std::string product_name;
	float price;
	int qty;
};

template<typename same_endian_type>
typename simple::mem_istream<same_endian_type>& operator >> ( simple::mem_istream<same_endian_type>& istm, Product& val)
{
	return istm >> val.product_name >> val.price >> val.qty;
}

template<typename same_endian_type>
typename simple::ptr_istream<same_endian_type>& operator >> ( simple::ptr_istream<same_endian_type>& istm, Product& val)
{
	return istm >> val.product_name >> val.price >> val.qty;
}

template<typename same_endian_type>
typename simple::file_istream<same_endian_type>& operator >> ( simple::file_istream<same_endian_type>& istm, Product& val)
{
	return istm >> val.product_name >> val.price >> val.qty;
}

template<typename same_endian_type>
typename simple::memfile_istream<same_endian_type>& operator >> ( simple::memfile_istream<same_endian_type>& istm, Product& val)
{
	return istm >> val.product_name >> val.price >> val.qty;
}

template<typename same_endian_type>
typename simple::mem_ostream<same_endian_type>& operator << ( simple::mem_ostream<same_endian_type>& ostm, const Product& val)
{
	return ostm << val.product_name << val.price << val.qty;
}

template<typename same_endian_type>
typename simple::file_ostream<same_endian_type>& operator << ( simple::file_ostream<same_endian_type>& ostm, const Product& val)
{
	return ostm << val.product_name << val.price << val.qty;
}

template<typename same_endian_type>
typename simple::memfile_ostream<same_endian_type>& operator << ( simple::memfile_ostream<same_endian_type>& ostm, const Product& val)
{
	return ostm << val.product_name << val.price << val.qty;
}

template<typename same_endian_type>
typename simple::mem_istream<same_endian_type>& operator >> ( simple::mem_istream<same_endian_type>& istm, std::vector<Product>& vec)
{
	int size=0;
	istm >> size;

	if(size<=0)
		return istm;

	for(int i=0; i<size; ++i)
	{
		Product product;
		istm >> product;
		vec.push_back(product);
	}

	return istm;
}

template<typename same_endian_type>
typename simple::ptr_istream<same_endian_type>& operator >> ( simple::ptr_istream<same_endian_type>& istm, std::vector<Product>& vec)
{
	int size = 0;
	istm >> size;

	if (size <= 0)
		return istm;

	for (int i = 0; i<size; ++i)
	{
		Product product;
		istm >> product;
		vec.push_back(product);
	}

	return istm;
}

template<typename same_endian_type>
typename simple::file_istream<same_endian_type>& operator >> ( simple::file_istream<same_endian_type>& istm, std::vector<Product>& vec)
{
	int size=0;
	istm >> size;

	if(size<=0)
		return istm;

	for(int i=0; i<size; ++i)
	{
		Product product;
		istm >> product;
		vec.push_back(product);
	}

	return istm;
}

template<typename same_endian_type>
typename simple::memfile_istream<same_endian_type>& operator >> ( simple::memfile_istream<same_endian_type>& istm, std::vector<Product>& vec)
{
	int size = 0;
	istm >> size;

	if (size <= 0)
		return istm;

	for (int i = 0; i<size; ++i)
	{
		Product product;
		istm >> product;
		vec.push_back(product);
	}

	return istm;
}

template<typename same_endian_type>
typename simple::mem_ostream<same_endian_type>& operator << (simple::mem_ostream<same_endian_type>& ostm, const std::vector<Product>& vec)
{
	int size = vec.size();
	ostm << size;
	for(size_t i=0; i<vec.size(); ++i)
	{
		ostm << vec[i];
	}

	return ostm;
}

template<typename same_endian_type>
typename simple::file_ostream<same_endian_type>& operator << ( simple::file_ostream<same_endian_type>& ostm, const std::vector<Product>& vec)
{
	int size = vec.size();
	ostm << size;
	for(size_t i=0; i<vec.size(); ++i)
	{
		ostm << vec[i];
	}

	return ostm;
}

template<typename same_endian_type>
typename simple::memfile_ostream<same_endian_type>& operator << ( simple::memfile_ostream<same_endian_type>& ostm, const std::vector<Product>& vec)
{
	int size = vec.size();
	ostm << size;
	for (size_t i = 0; i<vec.size(); ++i)
	{
		ostm << vec[i];
	}

	return ostm;
}

void print_product(const Product& product)
{
	using namespace std;
	cout << "Product:" << product.product_name << ", Price:" << product.price << ", Qty:" << product.qty << endl;
}

void print_products(const std::vector<Product>& vec)
{
	for(size_t i=0; i<vec.size() ; ++i)
		print_product(vec[i]);
}
