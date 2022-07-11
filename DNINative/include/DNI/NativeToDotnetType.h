#pragma once

#include "DNI.h"
#include <string>
#include <map>

namespace DNI
{
	const std::string GetStringTypeName()
	{
		return "System.String,System.String";
	}

	template<typename T1, typename T2>
	const std::string GetTypeName(const std::map<T1,T2>*)
	{
		return "System.Collections.Generic.Dictionary`2";
	}

	const std::string GetTypeName(const std::string*)
	{
		return "System.String";
	}
}