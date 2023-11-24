#include "server.h"
#include <iostream>
#include <sstream>
#include <QTextStream>
#include "connectionhandler.h"
#include <QThreadPool>
#include <QHostAddress>
#include <QSocketNotifier>
#include <QCoreApplication>
#include <ranges>

Server::Server(): input{stdin}, output{stdout}
{
    if (!listen(QHostAddress::Any, 8888))
        qDebug() << "error\n";


    if (!input.atEnd()){
        notifier = new QSocketNotifier(fileno(stdin), QSocketNotifier::Read);
        connect(notifier, &QSocketNotifier::activated, this, &Server::handleInput);
        connect(this, &QObject::destroyed, notifier, &QObject::deleteLater);
    }
}

void Server::print_connection(QString &name){
    output << "recieved a connction from: " << name << '\n';
    output.flush();
}

void Server::incomingConnection(qintptr handle){
    ConnectionHandler *handler = new ConnectionHandler(this, handle);
    handler->setAutoDelete(true);
    QThreadPool::globalInstance()->start(handler);
}

bool parse_ico(std::stringstream &ss, QTextStream &out, int &ico){
    ss >> ico;
    char end;
    if (!ss){
        out << "error parsing ICO\n";
        out.flush();
        return false;
    }
    return true;
}

bool create_company(std::stringstream &ss, QTextStream &out, company &c){
    std::string name;
    std::string short_name;
    int ico;
    ss >> name >> short_name >> ico;
    if (!ss){
        out << "error parsing company\n";
        return false;
    }
    c = {name, short_name, ico};
    return true;
}

void Server::print_msg(QTextStream& out, const QString &str, const QString &add, bool print){
    out << str;
    out.flush();
    if (print){
        output << add << str;
        output.flush();
    }
}
/**************************************************/
/*                parse add-ers                   */
/**************************************************/
void Server::parse_add(std::stringstream &ss, QTextStream& out, const QString &client, bool print){
    std::string subject;
    ss >> subject;
    if (subject == "company"){
        std::string name, short_name;
        int ico;
        ss >> name >> short_name >> ico;
        if (!ss){
            out << "error parsing: add company <???>\n";
            return;
        }
        data.add_company(name, short_name, ico);
        print_msg(out, QString{"added company: "} + name.data() + ' ' +
                       short_name.data() + ' ' + QString::number(ico) + '\n', client + ": ", print);

    } else if (subject == "invoice"){
        int id, ico_exhibitor, ico_customer, price, vat;
        ss >> id >> ico_exhibitor >> ico_customer >> price >> vat;
        if (!ss){
            out << "error parsing: add invoice <???>\n";
            return;
        }
        data.add_invoice(id, ico_exhibitor, ico_customer, price, vat);
        print_msg(out, QString("added invoice: ") + QString::number(id) + ' ' + //it really wants to call QString(char) :/
                                                    QString::number(ico_exhibitor) + ' ' +
                                                    QString::number(ico_customer) + ' ' +
                                                    QString::number(price) + ' ' +
                                                    QString::number(vat) + '\n', client + ": ", print);

    } else if (subject == "vat-payment"){
        int id, price;
        std::string type;
        ss >> id >> type >> price;
        if (!ss){
            out << "error parsing: add vat-payment <???>!\n";
            return;
        }

        if (type == "added"){
            data.add_vat_payment(id, VAT_ADDED_T, price);
        } else if (type == "profit"){
            data.add_vat_payment(id, VAT_PROFIT_T, price);
        }
        print_msg(out, QString{"added VAT payment: "} + QString::number(id) + ' ' +
                       type.data() + ' ' + QString::number(price) + '\n', client + ": ", print);
    } else{
        out << "error parsing: add <???>\n";
    }
}

/**************************************************/
/*                parse getters                   */
/**************************************************/
void Server::parse_get(std::stringstream &ss, QTextStream& out, const QString &name, bool print){
    std::string subject;
    ss >> subject;
    if (subject == "total"){
        std::string a;
        ss >> a;

        if (a == "income"){
            int ico;
            if (!parse_ico(ss, out, ico))
                return;
            int num = data.total_income(ico);
            print_msg(out, QString{"total income: "} + QString::number(num) + '\n', name + ": requested ", print);

        } else if (a == "profit"){
            int ico;
            if (!parse_ico(ss, out, ico))
                return;
            int num = data.total_profit(ico);
            print_msg(out, QString{"total profit: "} + QString::number(num) + '\n', name + ": requested ", print);

        } else if (a == "movement"){
            int ico1, ico2;
            if (!parse_ico(ss, out, ico1) || !parse_ico(ss, out, ico2))
                return;
            auto [ num1, num2 ] = data.total_movement(ico1, ico2);
            print_msg(out, QString{"total movement: "} + QString::number(num1) + " - " +
                           QString::number(num2) + '\n', name + ": requested ", print);
        } else{
            out << "error parsing: total <???>!\n";
        }

    } else if (subject == "vat-by"){
        std::string by;
        ss >> by;

        if (by == "profit"){
            int ico;
            if (!parse_ico(ss, out, ico))
                return;
            print_msg(out, QString{"vat-by profit: "} + QString::number(data.get_profit_vat(ico)) + '\n',
                      name + ": requested ", print);

        } else if (by == "added"){
            int ico;
            if (!parse_ico(ss, out, ico))
                return;
            print_msg(out, QString{"vat-by added: "} + QString::number(data.get_added_vat(ico)) + '\n',
                      name + ": requested ", print);
        } else {
            out << "error parsing: total vat-by <???>\n";
        }

    } else if (subject == "unpaid"){
        std::string filler;
        ss >> filler;
        int ico;
        if (filler != "vat" || !parse_ico(ss, out, ico)){
            out << "error parsing: get unpaid <???>\n";
            return;
        }
        int num = data.get_unpaid_vat(ico);
        print_msg(out, QString{""} + QString::number(num) + '\n',
                  name + ": requested unpaid vat", print);
    } else {
        out << "error parsing: get <???>\n";
    }
}

/**************************************************/
/*                parse listing                   */
/**************************************************/
void Server::parse_list(std::stringstream &ss, QTextStream& out){
    std::string listing;
    ss >> listing;
    std::vector<std::tuple<int, int>> sorted;
    if (listing == "asc"){
        sorted = data.get_companies(false);
    } else if (listing == "dsc"){
        sorted = data.get_companies(true);
    } else{
        out << "error parsing: list-by <???>\n";
        return;
    }
    std::ranges::for_each(sorted, [&](auto a){
    out << "company ICO: " << std::get<0>(a) << " profit: " << std::get<1>(a) << '\n';});

}
//using namespace string_literals;
auto angry_pepe = R"(
⠀⠀⠀⠀⠀⠀⠀⠀⠀⡴⠁⣠⣶⠞⠁⠀⠀⠀⠀⠀⠈⠑⠒⠤⡀⠀⠀⣀⡠⠔⠒⠉⠉⠉⠙⠛⠿⣶⡄⠀⠀⠀⠈⣷⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠘⠀⡴⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⡀⠀⠀⠀⠓⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠹⣆⠀⠀⠀⣿⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⢀⡞⠁⠀⠀⠀⠀⣀⠤⠒⠃⠉⠉⠉⠑⠓⠶⣦⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠸⣧⡀⢀⣯⣀⣀⣀⣀⠀⠀
⠀⠀⠀⠀⠀⢀⠀⣰⡞⠀⠀⠀⠀⠊⠉⠀⠀⠀⠀⠀⠀⠀⣀⣀⣀⠀⠉⢷⠀⠉⠈⠉⠉⠉⠉⠉⠉⠉⠉⠀⠛⠿⣿⣯⣹⣿⢯⡉⠳⣄
⠀⢀⣤⠄⠀⣩⣾⡟⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⡴⠾⠛⠉⠉⠉⠛⠻⢶⣤⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⣿⣽⡄⢻⡄⠉
⣰⠟⠀⢀⣴⠟⣻⠃⠀⠀⠀⠀⠀⠀⠀⣠⠴⢟⣉⠴⣲⣿⣿⣿⣯⣭⣖⣲⣮⣝⠶⡄⠀⠀⢀⡠⠔⠒⠈⢉⣉⠤⣤⣤⣾⣿⣿⣶⢷⣤
⠁⠀⢠⡾⠋⠀⡿⠀⠀⠀⠀⠀⠀⣀⣤⠴⢾⣫⠵⠟⠉⠁⠀⠀⠀⠀⠉⠙⠛⠿⢷⣞⡒⠉⡁⠠⠒⢒⣉⣵⠶⠟⠛⠉⠀⠈⠉⠙⡆⠁
⠀⣠⡟⠁⠀⠀⠁⠀⠀⠀⠀⠘⠿⠤⢴⣶⡿⠁⠀⠀⠀⠀⠀⠀⠀⣾⣿⡆⠀⠀⢀⣈⣿⣶⡶⠶⠚⠉⠉⠀⠀⠀⠀⢰⣿⣷⠀⢀⡇⠀
⢰⡟⠀⠀⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢉⡻⠷⣦⣄⣀⣀⣀⣀⣀⣈⣿⣵⠶⠟⠛⠿⣯⣥⣤⣤⣤⣤⣤⣤⣤⣤⣤⣼⣿⣶⣿⣿⠁⠀
⡿⠀⢀⣼⣁⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠛⠲⠦⣤⣭⣍⣉⣉⣉⣉⣀⣀⢠⡶⠂⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣰⡿⠟⠋⠁⠀⠀
⣟⣦⠞⠋⠉⠀⠀⠀⠀⠀⠀⠀⢀⣀⣠⣤⣤⣤⣀⡀⠀⠀⠀⠀⠀⠀⡤⠂⡉⠐⠂⠀⠀⠐⠠⠤⣤⡤⠤⠴⠒⠺⣿⣻⡀⠀⠀⠀⠀⠀
⡟⠁⠀⠀⠀⠀⠀⠀⠀⣀⣴⡾⠛⠋⠉⣉⣉⣉⡉⠛⠛⠒⠶⠦⠴⢯⣴⣧⣤⣤⣄⣀⣀⣀⣀⢀⣀⣀⣠⣤⣤⡶⣿⣿⡇⠀⠀⠀⠀⠀WRONG COMMAND!!
⠄⠀⠀⠀⠀⠀⠀⣠⣾⡿⠋⣠⣤⣶⣿⣿⣿⣿⣿⣿⣿⣶⣦⣤⣀⣀⠀⠀⠀⠉⠉⠛⠙⢛⢻⣿⣿⡿⢻⣏⣁⣤⣿⡿⠃⡀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⢰⣿⠋⢡⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣶⣶⣶⣶⡾⠟⠛⠛⠛⠛⠛⠛⠛⠋⢁⣀⣀⣀⡀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⣿⠏⢠⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⠋⠀⠀⠀⠀⠀⠀⠀⣀⣤⣶⣿⠿⠛⠻⠿⣦⡀⠀
⣤⣾⣻⡗⠀⢸⣿⠀⢸⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⠿⢛⣿⣭⣭⣭⣿⣟⣛⠿⢷⣶⣤⣤⣤⣤⣤⣴⣾⣿⣿⣿⣿⠿⠷⣤⡀⢈⡷⠀
⡏⠉⠉⠁⠀⢸⡟⠀⢸⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⠋⣴⣿⣿⣿⣿⣿⣿⣿⠿⠿⢷⣦⣬⣭⣩⣏⣽⣿⣿⡿⣿⡁⠀⠀⠀⠈⠉⠉⠀⠀
⣿⠀⠀⠀⠀⢸⣿⡀⠈⢿⣿⣿⣿⣿⣿⣿⣿⣿⠇⣼⣿⣿⣿⣿⣿⣿⣿⡟⠀⠀⠀⠀⠉⠉⠛⠛⠋⠉⠀⠰⣤⠀⠀⠀⠀⠀⠀⠀⠀⠀
⢻⣧⡀⠀⠀⡸⣿⣷⡀⠈⢻⣿⣿⣿⣿⣿⣿⣿⣤⣿⣿⣿⣿⣿⣿⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀⣀⣀⣀⣀⣀⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠾⠿⣷⣄⠀⠉⠹⣿⣧⣄⣀⣈⣻⣿⣿⣿⣭⣿⣿⣭⣭⣉⣉⣹⣿⣛⣛⣛⣻⣿⣿⣿⠿⠟⠛⠛⢛⣛⣿⣿⣿⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠈⠛⠿⢶⣦⣾⣟⣛⠋⠉⠉⠉⠉⠉⠉⣽⣿⠉⠉⢉⣿⠛⠛⠛⠛⠛⠛⠛⠛⠛⠛⠛⢛⣻⣿⠿⢻⡟⠁⠀⠀⠀⠀⢠⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠈⠉⠉⠛⠛⠿⠷⣶⣦⣴⡿⢿⣿⡤⠖⡟⠁⠀⠀⠀⠀⠀⠀⢀⣀⣠⣴⡾⠋⠉⠀⢸⣿⡇⠀⠀⠀⠀⠐⢑⠀⠀⠈⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⡴⠟⠁⠀⠀⠀⠀⠾⠭⢍⣉⡉⠉⠉⠉⠉⠉⠁⠙⠻⣷⣄⠀⠀⠘⣸⡇⠀⠄⠂⠀⠀⠀⠀⠐⠠⢀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠔⠊⠁⠀⠀⠀⠀⠀⠀⣀⣠⠤⠖⠋⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⢷⣜⠿⠛⠁⠀⠀⠀⠀⠀⠀⠀⠀⠄⠘
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣨⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠹⣇⠀⠀⠀⠀⠀⠀⠰⠀⠀⠀⠀⠀
)";

void Server::handleInput(QSocketDescriptor desc, QSocketNotifier::Type){
    if (!desc.isValid()) {
        output << "Error";
    }
    if (input.atEnd()) {
        QCoreApplication::quit();
        return;
    }
    QString message = input.readLine();
    std::stringstream ss;
    ss << message.toStdString();

    std::string action;
    ss >> action;

    if (action == "get"){
        parse_get(ss, output, "server", false);
    } else if (action == "add"){
        parse_add(ss, output, "server", false);
    } else if (action == "list-by"){
        parse_list(ss, output);
    } else if (message == "end") {
        output << "Ending...";
        QCoreApplication::quit();
        return;
    } else {
        std::cout << angry_pepe;
        std::cout.flush();
    }
    output.flush();
}
