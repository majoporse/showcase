#ifndef PARSER_H
#define PARSER_H

#include <string>
#include <vector>
#include <utility>
#include "rational.h"
#include <sstream>

rational parsePrio1(std::stringstream &ss, std::vector<rational> &memory, bool is_nested);
rational parsePrio2(std::stringstream &ss, std::vector<rational> &memory) ;
rational parsePrio3(std::stringstream &ss, std::vector<rational> &memory);

#endif
