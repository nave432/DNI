#pragma once

#include <string>
namespace ClassTest
{
	class Animal
	{
	public:

		Animal(const std::string& name)
		{
			m_name = name;
		}

		const std::string& getName()
		{
			return m_name;
		}

	private:
		std::string m_name;
	};


	class Cat : public Animal
	{
	public:
		Cat(const std::string& name):Animal(name)
		{
		}
	};

	class Dog : public Animal
	{
	public:
		Dog(const std::string& name):Animal(name)
		{
		}
	};
}

