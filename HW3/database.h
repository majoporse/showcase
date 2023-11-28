#ifndef DATABASE_H
#define DATABASE_H

#include <vector>
#include <shared_mutex>
#include <string>

#define VAT_COSTS 0.21
#define VAT_PROFIT 0.19

#define VAT_PROFIT_T 2
#define VAT_ADDED_T 1

struct company{
    std::string name;
    std::string short_name;
    int ico = 0;
    company(std::string name, std::string short_name, int ico):
        name{name}, short_name{short_name}, ico{ico}{}
    company(company&&) = default;
    company& operator=(company&&) = default;

    company(const company&) = delete;
    company& operator=(const company&) = delete;
};

struct invoice{
    int id;
    int ico_exhibitor;
    int ico_customer;
    int price;
    int vat;
    invoice(int id, int ico_exhibitor, int ico_customer, int price, int vat):
        id{id}, ico_exhibitor{ico_exhibitor}, ico_customer{ico_customer},
        price{price}, vat{vat}{}
    invoice(const invoice&) = delete;
    invoice& operator=(const invoice&) = delete;

    invoice(invoice&&) = default;
    invoice& operator=(invoice&&) = default;
};

struct vat_payment{
    int ico;
    int payment_type;
    int price;
    vat_payment(int ico, int payment_type, int price):
        ico{ico}, payment_type{payment_type}, price{price}{}
    vat_payment(const vat_payment&) = delete;
    vat_payment& operator=(const vat_payment&) = delete;

    vat_payment(vat_payment&&) = default;
    vat_payment& operator=(vat_payment&&) = default;
};

class database
{
    mutable std::shared_mutex m;
    std::vector<company> companies;
    std::vector<invoice> invoices;
    std::vector<vat_payment> payments;

public:
    bool add_company(std::string name, std::string short_name, int ico);
    bool add_invoice(int ico, int ico_exhibitor, int ico_customer, int price, int vat);
    bool add_vat_payment(int ico, int type, int price);

    int get_added_vat(int, bool is_locked = false) const;
    int get_profit_vat(int, bool is_locked = false) const;
    int get_unpaid_vat(int, bool is_locked = false) const;
    std::tuple<int, int> total_movement(int, int) const;
    std::vector<std::tuple<int, int>> get_companies(bool) const;
    
    //helper functions
    int get_paid_vat_invoice(int, bool is_locked = false) const;
    int get_paid_vat_payment(int, bool is_locked = false) const;
    
    int total_income(int, bool is_locked = false) const;
    int total_costs(int, bool is_locked = false) const;
    int total_profit(int, bool is_locked = false) const;

    int movement_from_to(int, int) const;
};

#endif // DATABASE_H
