#include "connectionhandler.h"
#include <QTcpSocket>
#include <sstream>
#include <iostream>
#include <QTextStream>

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

    while (sock.state() == QAbstractSocket::ConnectedState) {
        sock.waitForReadyRead();
        if (sock.state() != QAbstractSocket::ConnectedState) {
            return;
        }
        QString line = ts.readAll();
        // qDebug() << "line: " << line;
        std::string action;
        std::stringstream ss;
        ss << line.toStdString();
        ss >> action;

        if (action == "get"){
            myServer->parse_get(ss, ts, name, true);
        } else if (action == "add"){
            myServer->parse_add(ss, ts, name, true);
        } else if (action == "list-by"){
            myServer->parse_list(ss, ts);
        }  else {
            ts << "error parsing!\n";
        }
        ts.flush();
        sock.waitForBytesWritten();
    }
}
