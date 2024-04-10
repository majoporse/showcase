#ifndef CONNECTIONHANDLER_H
#define CONNECTIONHANDLER_H

#include <QObject>
#include <QRunnable>
#include "server.h"

//class SocketHandler: public QObject, public QRunnable
class ConnectionHandler: public QRunnable, public QObject
{
    qintptr sockNo;
    Server *myServer;

public:


    ConnectionHandler(Server *server, qintptr sockNo);



public:
    void run();

};

#endif // CONNECTIONHANDLER_H
