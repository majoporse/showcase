#ifndef SERVER_H
#define SERVER_H
#include <QTcpServer>
#include <QSocketNotifier>
#include "database.h"
#include <memory>
#include <QObject>

class Server : public QTcpServer
{
    Q_OBJECT
    QTextStream input;
    QTextStream output;
    database data;
    QSocketNotifier *notifier;
    void print_msg(QTextStream&, const QString&, const QString&, bool);
    void handleInput(QSocketDescriptor, QSocketNotifier::Type);
public:
    Server();

    void parse_add(std::stringstream&, QTextStream&, const QString&, bool);
    void parse_get(std::stringstream&, QTextStream&, const QString&, bool);
    void parse_list(std::stringstream&, QTextStream&);
    void print_connection(QString&);

signals:



protected:
    void incomingConnection(qintptr handle);
};
#endif // SERVER_H
