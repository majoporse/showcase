#include "parser.h"

std::string_view NUMBERS{"0123456789"};
std::string_view OPERATORS_PRIO1{"+-"};
std::string_view OPERATORS_PRIO2{"*/"};
std::string_view OPERATORS_PRIO3{"^"};

bool is_numeric(char c){
    return c >= '0' && c <= '9';
}

rational parsePar(std::stringstream &ss, std::vector<rational> &memory){
    char par;
    ss >> par;
    rational cur = parsePrio1(ss, memory, true);
    if (ss.peek() != ')')
        throw (int) ss.tellg();
    ss >> par;
    return cur;
}


rational parsePrio1(std::stringstream &ss, std::vector<rational> &memory, bool is_nested){
    rational r = parsePrio2(ss, memory);
    while(OPERATORS_PRIO1.find_first_of(ss.peek()) != std::string::npos){
        char op;
        switch (ss.peek()) {
        case('+'):
            ss >> op;
            r += parsePrio2(ss, memory);
            break;
        case('-'):
            ss >> op;
            r -= parsePrio2(ss, memory);
            break;
        default:
            break;
        }
    }
    if (is_nested && ss.peek() != ')' || !ss.eof() && ss.peek() != ')')
        throw (int) ss.tellg();
    return r;
}

rational parsePrio2(std::stringstream &ss, std::vector<rational> &memory){
    rational res = parsePrio3(ss, memory);
    while (OPERATORS_PRIO2.find_first_of(ss.peek()) != std::string::npos){
        char op;
        switch (ss.peek()) {
            case('*'):
                ss >> op;
                res *= parsePrio3(ss, memory);
                break;
            case('/'):
                ss >> op;
                res /= parsePrio3(ss, memory);
                break;
            default:
                break;
        }
    }
    return res;
}

rational parsePrio3(std::stringstream &ss, std::vector<rational> &memory){
    int nom;
    rational r(0,0);
    if (ss.peek() == '('){
        char par;
        ss >> par;
        r = parsePrio1(ss, memory, true);
        if (ss.peek() != ')')
            throw (int) ss.tellg();
        ss >> par;
    } else if (is_numeric(ss.peek())){
        int nom;
        ss >> nom;
        r = {nom, 1};
    } else if (ss.peek() == 'M'){
        char M;
        ss >> M;
        if (!is_numeric(ss.peek()))
            throw (int) ss.tellg();
        int index;
        ss >> index;
        if (index >= memory.size())
            throw (int) ss.tellg();
        r = memory[index];
    } else
        throw (int) ss.tellg();

    while (OPERATORS_PRIO3.find_first_of(ss.peek()) != std::string::npos){
        char op;
        switch (ss.peek()) {
            case('^'):
                ss >> op;
                r = r.pow(parsePrio3(ss, memory));
                break;
        default:
            break;
        }
    }
//    std::cout << "asd";
    return r;
}
