#include "rational.h"
#include <numeric>


rational &rational::operator+=(const rational &r){
    int lcm = std::lcm(denom, r.denom);
    nom = (lcm / denom) * nom + (lcm / r.denom) * r.nom;
    denom = lcm;

    if (nom == denom){
        nom = 1;
        denom = 1;
    }

    return *this;
}

rational &rational::operator-=(const rational &r){

    int lcm = std::lcm(denom, r.denom);
    nom = (lcm / denom) * nom - (lcm / r.denom) * r.nom ;
    denom = lcm;

    int gcd = std::gcd(nom, denom);
    nom /= gcd;
    denom /= gcd;

    if (nom == denom){
        nom = 1;
        denom = 1;
    }
    return *this;
}

rational &rational::operator*=(const rational &r){
    int lgcd = std::gcd(denom, r.nom);
    int rgcd = std::gcd(r.denom, nom);

    nom = (nom / rgcd) * (r.nom / lgcd);
    denom = (denom / lgcd) * (r.denom / rgcd);

    if (nom == denom){
        nom = 1;
        denom = 1;
    }

    return *this;
}

rational &rational::operator/=(const rational &r){
    rational reversed = {r.denom, r.nom};
    *this *= reversed;
    return *this;
}

rational rational::pow(const rational &p){
    rational result = {1, 1};
    unsigned int power = p.nom;
    rational base = *this;

    while(power > 0) {

        if((power & 1) == 1) {
            result = result * base;
        }
        base = base * base;
        power = power / 2; // Can also use power >>= 1; to make code even faster
    }
    return result;
}

