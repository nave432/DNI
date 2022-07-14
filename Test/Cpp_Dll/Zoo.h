#pragma once

#include <list>
#include <memory>
#include <string>
#include "Animal.h"

namespace ClassTest
{

	enum class AnimalTypes
	{
		Dog,
		Cat
	};
	
	class Zoo
	{
	public:
		int  GetAnimalCount();
		bool AddAnimal(Animal* pAnimals);
		bool RemoveAnimal(const std::string& name);
		
		// ideally it should be a factory, but for now will do it here
		Animal* CreateAnimal(const AnimalTypes type, const std::string& name);

	private:
		std::list< std::shared_ptr<Animal> > m_Animals;

	};

}
