#include <memory>
#include <string_view>
#include <vector>
#include <map>
#include <algorithm>
#include <cassert>
#include <ranges>
#include <functional>
#include <memory>
#include <charconv>
#include <cctype>

struct json_value;
using json_ptr = std::unique_ptr< json_value >;
using json_ref = const json_value &;

enum class json_type { integer, array, object };

struct json_error{
    const char *what() const;
};

std::string_view BLANK = " \t\n";
std::string_view NONZERO = "123456789";
std::string_view ZERO = "0";

struct json_value{
    json_type t;
    std::map<std::string, json_ptr> obj_data;
    std::vector<std::unique_ptr< json_value >> vec_data;
    int int_val;
    json_type type() const {return t;}
    int int_value() const {if (t != json_type::integer) throw json_error(); return int_val;}

    json_ref item_at( int i) const {
        if (t == json_type::object){
            if (i > static_cast<int>(obj_data.size() - 1) || i < 0)
                throw std::out_of_range("");
    
            auto it = obj_data.begin();
            std::advance(it, i);
            return *(it->second);
        }
        if (t == json_type::array){
            return *(vec_data.at(i));
        }
        throw json_error();
    }

    json_ref item_at( std::string_view sv) const { 
        if (t != json_type::object)
            throw json_error();
        return *(obj_data.at(std::string{sv}));
    }

    std::string key_at( int i ) const{
        if (t != json_type::object)
            throw json_error();
        if (i > static_cast<int>(obj_data.size() - 1) || i < 0)
                throw std::out_of_range("");
        auto it = obj_data.begin();
        std::advance(it, i);
        return it->first;
    }

    int length() const { return (t == json_type::object ? obj_data.size() : vec_data.size()); }
    ~json_value() = default;
};


bool parse_value( std::string_view &sv, json_value *res );
bool parse_integer( std::string_view &sv, json_value *res );
bool parse_array( std::string_view &sv, json_value *res );
bool parse_object( std::string_view &sv, json_value *res );
bool parse_kvpair(std::string_view &sv, json_value *res);

bool maybe_throw(bool ptr){
    if (ptr) throw json_error();
    return false;
}

void skip_blanks(std::string_view &sv){
    auto pos = sv.find_first_not_of(BLANK);
    if (pos ==  std::string::npos)
        pos = sv.size();
    sv.remove_prefix(pos);
}

bool parse_value( std::string_view &sv, json_value *res ){
    skip_blanks(sv);
    if (sv.empty())
        return maybe_throw(res);

    bool ret = false;
    if ( NONZERO.find(sv[0]) != std::string::npos || 
        (sv.size() > 1 && sv[0] == '-' && NONZERO.find(sv[1]) != std::string::npos) || 
        (sv[0] == '0' && (sv.size() == 1 || !std::isdigit(sv[1]))) ){
        ret = parse_integer(sv, res);

    }else if (sv[0] == '['){
        ret = parse_array(sv, res);

    }else if  (sv[0] == '{'){
        ret = parse_object(sv, res);

    } else
        return maybe_throw(res);

    skip_blanks(sv);

    return ret;
}

bool parse_integer( std::string_view &sv, json_value *res ){
    if (sv.empty())
        return maybe_throw(res);

    int val = 0;
    auto [ptr, r] = std::from_chars(sv.data(), sv.data() + sv.size(), val);
    if (res){
        res->t = json_type::integer;
        res->int_val = val;
    }
    if (std::isdigit(*ptr))
        return maybe_throw(res);
    sv = sv.substr(ptr - sv.data());
    return true;
}

bool parse_array( std::string_view &sv, json_value *res ){
    if (sv.size() < 2 || sv[0] != '[')
        return maybe_throw(res);
    sv = sv.substr(1);

    if (res)
        res->t = json_type::array;

    for (; sv[0] != ']';){
        if (res)
            res->vec_data.emplace_back(std::make_unique<json_value>());
        
        if (!parse_value(sv, res ? res->vec_data.back().get() : nullptr))
            return false;

        if (sv[0] != ']' && sv[0] != ',')
            return maybe_throw(res);
        
        if (sv[0] == ','){
            sv.remove_prefix(1);
            if (sv[0] == ']') return maybe_throw(res);
        }
    }
    sv = sv.substr(1);
    return true;
}

bool parse_object( std::string_view &sv, json_value *res ){
    if (sv.size() < 2 || sv[0] != '{')
        return maybe_throw(res);
    sv = sv.substr(1);

    if (res)
        res->t = json_type::object;

    for (; sv[0] != '}';){
        
        if (!parse_kvpair(sv, res))
            return false;

        if (sv[0] != ',' && sv[0] != '}')
            return maybe_throw(res);

        if (sv[0] == ','){
            sv = sv.substr(1);
            if (sv[0] == '}') return maybe_throw(res);
        }
        
    }
    sv = sv.substr(1);
    return true;
}

bool parse_kvpair(std::string_view &sv, json_value *res){
    skip_blanks(sv);

    auto end_pos = sv.find_first_of(std::string{BLANK} + ":");

    std::string key {sv.substr(0, end_pos)};
    if (std::find_if_not(key.data(), key.data() + key.size(), [](auto c) {return std::isalpha(c);} ) != key.data() + key.size()
        || key.empty())
        return maybe_throw(res);
    sv.remove_prefix(end_pos);

    if (res && (key.empty() || (res && res->obj_data.contains(key))))
        throw json_error();

    skip_blanks(sv);
    if (sv.empty() || sv[0] != ':')
        return maybe_throw(res);
    sv.remove_prefix(1);

    if (res)
        res->obj_data[key] = std::make_unique<json_value>();
    return parse_value(sv, res ? res->obj_data[key].get() : nullptr);
}


json_ptr json_parse( std::string_view sv){
    auto ptr = std::make_unique<json_value>();
    parse_value(sv, ptr.get());
    if (!sv.empty())
        throw json_error();
    return ptr;
}

bool json_validate( std::string_view sv){
    return parse_value(sv, nullptr) && sv.empty();
}

int main()
{
    assert( !json_validate( "x" ) );
    assert( json_validate( "{}" ) );
    assert( json_validate( "[ 1 ]" ) );

    auto t = json_parse( "{}" );
    assert( t->type() == json_type::object );
    {
    std::string_view json_string = "[ 1 , 2 , 3 ]";
    json_ptr result = json_parse(json_string);

    // Assert that the result has the correct type and length
    assert(result->type() == json_type::array);
    assert(result->length() == 3);

    // Assert that the individual elements are correct
    assert(result->item_at(0).int_value() == 1);
    assert(result->item_at(1).int_value() == 2);
    assert(result->item_at(2).int_value() == 3);
    }
    {
    std::string_view json_str = "{ name : 745375 , age : 30 , city : 5478 }";
    auto json = json_parse(json_str);

    assert(json->type() == json_type::object);
    assert(json->length() == 3);

    auto& name = json->item_at("name");
    assert(name.type() == json_type::integer);
    assert(name.int_value() == 745375);

    auto& age = json->item_at("age");
    assert(age.type() == json_type::integer);
    assert(age.int_value() == 30);

    auto& city = json->item_at("city");
    assert(city.type() == json_type::integer);
    assert(city.int_value() == 5478);
    }

    {
        std::string_view json_str = "[ 1 , 2 , 3 ]";
        auto json = json_parse(json_str);

        assert(json->type() == json_type::array);
        assert(json->length() == 3);

        auto& item1 = json->item_at(0);
        assert(item1.type() == json_type::integer);
        assert(item1.int_value() == 1);

        auto& item2 = json->item_at(1);
        assert(item2.type() == json_type::integer);
        assert(item2.int_value() == 2);

        auto& item3 = json->item_at(2);
        assert(item3.type() == json_type::integer);
        assert(item3.int_value() == 3);
    }

    {
        std::string_view valid_json_str = "{ name : 1234 , age : 30 , city : 5436 }";
        assert(json_validate(valid_json_str) == true);

        std::string_view invalid_json_str = "{ name : 0134 , age : 30 , city : 06235 }";
        assert(json_validate(invalid_json_str) == false);
    }
    {
    std::string_view json_str = "{ array : [ 1 , 2 , 3 ] , integer : 30 , object : { kokot : 1234 , jebem : 30 , pica : 5436 } }";
    auto json = json_parse(json_str);
    std::string_view json_str1 = "[ { array : [ 1 , 2 , 3 ] , integer : 30 , object : { kokot : 1234 , jebem : 30 , pica : 5436 } } ]";
    auto json1 = json_parse(json_str1);
    std::string_view json_str2 = "[ { array : [ 1 , 2 , 3 ] , integer : 30 } , { object : { kokot : 1234 , jebem : 30 , pica : 5436 } } ]";
    auto json2 = json_parse(json_str2);
    }
    assert(json_validate("0"));
    assert(!json_validate("[] []"));
    int caught = 0;
    try{
        json_parse("[] []");
    } catch(json_error e){
        caught++;
    };
    assert(caught == 1);
    try{
        auto a = json_parse("[]");
        a->item_at(0);
    } catch(std::out_of_range &e){
        caught++;
    };
    assert(caught == 2);
    assert(!json_validate("01"));
    assert(json_validate("[ 1 , 2 , 3 ]"));
    assert(json_validate("[ 1 , 1 , 1 ]"));
    assert(json_validate("{ a : 1 , b : 1 , a : 1 }"));
    assert(json_validate("[ 1 , { a : 1 , a : 1 , a : 1 } , 3 ]"));
    try{
        json_parse("[ 1 , { a : 1 , a : 1 , a : 1 } , 3 ]");
    } catch(json_error){
        caught++;
    }
    assert(caught == 3);
    try{
        auto a = json_parse("[ 1 , 3 ]");
        a->item_at(-5);
    } catch(std::out_of_range &){
        caught++;
    }
    assert(caught == 4);
    try{
        auto a = json_parse("{ a : 3 }");
        a->item_at(-5);
    } catch(std::out_of_range &){
        caught++;
    }
    assert(caught == 5);
    try{
        auto a = json_parse("{}");
        a->key_at(5);
    } catch(std::out_of_range &){
        caught++;
    }
    assert(caught == 6);
    assert(!json_validate("[ 1 , 3 , 6 , s ]"));
    assert(!json_validate(" [ 1 , 3 , 6 , s ] "));
    assert(json_validate(" [ 1 , 3 , 6 ] "));
    assert(!json_validate("[ 1 , 3 , 6 , s ] "));
    assert(!json_validate("[ 1 , 3 , 6 ,s ] "));
    assert(!json_validate("[ 1 , 3 , 6 "));
    assert(!json_validate("[ 1 , 3 , 6 }"));
    assert(!json_validate("[ 1 , s , 6 ]"));
    assert(!json_validate("[ 1 , 3 ,, 6 ]"));
    assert(json_validate("[ 1 , 3 ,  6 ]"));
    assert(!json_validate("["));
    assert(!json_validate("{"));
    assert(!json_validate("]"));
    assert(!json_validate("}"));
    assert(!json_validate("01"));
    assert(!json_validate(" 01 "));
    assert(!json_validate(" -01 "));
    assert(!json_validate(" --01 "));
    assert(!json_validate(" --1 "));
    assert(!json_validate(" --[] "));
    assert(!json_validate("{ s8* : 1 }"));
    assert(json_validate("[ 1,2  ]"));
    assert(json_validate("[ 1,2,[], [] ] "));
    json_parse("{empty: {}}");
    json_parse("{ empty:{} }");
    json_parse("{empty:{}}");
    assert(!json_validate( "[,0]" ));
    assert(!json_validate("[[]"));
    assert(!json_validate("["));
    assert(!json_validate("]"));
    assert(!json_validate("}"));
    assert(!json_validate("{"));
    assert(!json_validate("[    ]"));
    assert(!json_validate("[,]"));
    assert(!json_validate("[0,]"));
    assert(!json_validate("{    }"));
    assert(!json_validate("{a:a}"));
    assert(!json_validate("{a:}"));
    assert(!json_validate("{a}"));
    assert(!json_validate("{a,b}"));
    assert(!json_validate("{ }"));
    assert(!json_validate("{ a:{}"));
    assert(!json_validate("{ :a}"));
    assert(!json_validate("{ a:1,}"));
    assert(!json_validate("{ a:1 b : 2}"));
    assert(!json_validate("{ a a}"));
    assert(!json_validate("[ 1  2]"));

    return 0;
}