#include "connectionhandler.h"
#include <QTcpSocket>
#include <sstream>
#include <iostream>
ConnectionHandler::ConnectionHandler(Server *server, qintptr sockNo): myServer{server}, sockNo{sockNo}
{
}

void ConnectionHandler::run()
{
    QTcpSocket sock;
    sock.setSocketDescriptor(sockNo);
    QTextStream ts(&sock);
    sock.waitForReadyRead();

    QString name;
    ts >> name;
    myServer->print_connection(name);
    ts << "asda";
    ts.flush();
    sock.waitForReadyRead();
    while (sock.state() == QAbstractSocket::ConnectedState) {
        sock.waitForReadyRead();
        if (sock.state() != QAbstractSocket::ConnectedState) {
            return;
        }
        QString line = ts.readLine();
        std::string action;
        std::stringstream ss;
        ss << line.data();
        ss >> action;
        std::cout << action;

        if (action == "get"){
            myServer->parse_get(ss, ts, name, false);
        } else if (action == "add"){
            myServer->parse_add(ss, ts, name, false);
        } else if (action == "list-by"){
            myServer->parse_list(ss, ts);
        }  else {
            ts << "error parsing!";
        }
        ts << "asdfasd";
        ts.flush();
        sock.waitForBytesWritten();
    }
}
