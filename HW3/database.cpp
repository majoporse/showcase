#include "database.h"
#include <mutex>
#include <ranges>
#include <algorithm>
#include <map>
#include <functional>

using std::ranges::any_of;

bool database::add_company(std::string name, std::string short_name, int ico){
    std::unique_lock<std::shared_mutex> lock(m);
    if (any_of(companies, [&](company &c){return c.name == name || c.ico == ico;}))
        return false;
    companies.emplace_back(name, short_name, ico);
    return true;
}

bool database::add_invoice(int id, int ico_exhibitor, int ico_customer, int price, int vat){
    std::unique_lock<std::shared_mutex> lock(m);
    if (any_of(invoices, [&](invoice &i){return i.id == id;}) || !any_of(companies, [&](company &c){return c.ico == ico_exhibitor;}))
        return false;
    invoices.emplace_back(id, ico_exhibitor, ico_customer, price, vat);
    return true;
}

bool database::add_vat_payment(int ico, int type, int price){
    std::unique_lock<std::shared_mutex> lock(m);
    if (!any_of(companies, [&](company&c){return c.ico == ico;}))
        return false;
    payments.emplace_back(ico, type, price);
    return true;
}

int database::get_unpaid_vat(int ico, bool is_locked) const{
    if (!is_locked)
        std::shared_lock<std::shared_mutex> lock(m);
    return get_added_vat(ico, true) + get_profit_vat(ico, true) - get_paid_vat_payment(ico, true);
}

int database::get_added_vat(int ico, bool is_locked)const{
    if (!is_locked)
        std::shared_lock<std::shared_mutex> lock(m);
    int total_in = total_income(ico, true);
    int total_vat_paid = get_paid_vat_invoice(ico, true);
    return total_in * VAT_COSTS - total_vat_paid;
}

int database::get_profit_vat(int ico, bool is_locked) const{
    if (!is_locked)
        std::shared_lock<std::shared_mutex> lock(m);
    return total_profit(ico, true) * VAT_PROFIT;
}

int database::get_paid_vat_payment(int ico, bool is_locked) const{
    std::shared_lock<std::shared_mutex> lock(m);
    int sum = 0;
    for (const auto &payment : payments){
        if (payment.ico == ico){
            sum += payment.price;
        }
    }
    return sum;
}

// i dont know if a recursive shared mutex exists in qt but it would be a nice thing to have
int database::get_paid_vat_invoice(int ico, bool is_locked) const{
    if (!is_locked)
        std::shared_lock<std::shared_mutex> lock(m);
    int paid = 0;
    for (const auto &in : invoices){
        if (in.ico_customer == ico)
            paid += in.vat;
    }
    return paid;
}

int database::total_profit(int ico, bool is_locked) const{
    if (!is_locked)
        std::shared_lock<std::shared_mutex> lock(m);
    int res = total_income(ico, true) - total_costs(ico, true);
    return res > 0 ? res : 0;
}

int database::total_income(int ico, bool is_locked) const{
    if (!is_locked)
        std::shared_lock<std::shared_mutex> lock(m);

    int income = 0;
    auto fun = [&]( const invoice& i) {
        if (ico  == i.ico_exhibitor)
            income += i.price;
    };
    std::ranges::for_each(invoices, fun);
    return income;
}

int database::total_costs(int ico, bool is_locked) const{
    if (!is_locked)
        std::shared_lock<std::shared_mutex> lock(m);

    int costs = 0;
    auto fun = [&]( const invoice& i) { 
        if (ico  == i.ico_customer)
            costs += i.price;
    };
    std::ranges::for_each(invoices, fun);
    return costs;
}

inline int database::movement_from_to(int ico, int ico2) const{
    int movement = 0;
    auto fun = [&]( const invoice& i) {
        if (ico  == i.ico_exhibitor && ico2 == i.ico_customer)
            movement += i.price + i.vat;
    };
    std::ranges::for_each(invoices, fun);
    return movement;
}

std::tuple<int, int> database::total_movement(int ico, int ico2) const{
    std::shared_lock<std::shared_mutex> lock(m);
    return { movement_from_to(ico, ico2), movement_from_to(ico2, ico) };
}

std::vector<std::tuple<int, int>> database::get_companies(bool is_desc) const{
    std::vector<std::tuple<int, int>> sorted;
    {
        std::shared_lock<std::shared_mutex> lock(m); //early free because sorting takes a lot of time
        for (const auto &c : companies)
            sorted.emplace_back(c.ico, total_income(c.ico, true) - total_costs(c.ico, true));
    }
    
    auto proj = [](auto &t) {return std::get<1>(t);};

    if (is_desc)
        std::ranges::sort(sorted, std::less{}, proj);
    else
        std::ranges::sort(sorted, std::greater{}, proj);

    return sorted;
}

