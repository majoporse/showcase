#ifndef RATIONAL_H
#define RATIONAL_H
#include <compare>

struct rational
{
       int nom, denom;

    rational(int nom, int denom): nom{nom}, denom{denom}{}
    float value(){ return float(nom) / denom;}
    rational pow(const rational &);
    rational &operator+=(const rational &);
    rational &operator-=(const rational &);
    rational &operator*=(const rational &);
    rational &operator/=(const rational &);

    friend rational operator+( rational l, const rational &r){
        l += r;
        return l;
    }

    friend rational operator-( rational l, const rational &r){
        l -= r;
        return l;
    }

    friend rational operator*(rational l, const rational &r){
        l *= r;
        return l;
    }

    friend rational operator/(rational l, const rational &r){
        l /= r;
        return l;
    }


    friend bool operator==(const rational &r, const rational &l){
        return r.nom * l.denom == l.nom * r.denom;
    }

    friend bool operator!=(const rational &r, const rational &l){
        return r.nom * l.denom != l.nom * r.denom;
    }

    friend auto operator<=>(const rational& r, const rational &l) {
        return r.nom * l.denom <=> l.nom * r.denom ;
    }
};

#endif // RATIONAL_H
