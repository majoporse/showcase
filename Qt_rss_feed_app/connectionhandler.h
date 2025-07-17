#ifndef CONNECTIONHANDLER_H
#define CONNECTIONHANDLER_H

#include <QObject>
#include <QRunnable>
#include "myserver.h"

class ConnectionHandler: public QRunnable, public QObject
{
    qintptr sockNo;
    MyServer *Server;

public:

    ConnectionHandler(MyServer *server, qintptr sockNo);

public:
    void run();

};

#endif // CONNECTIONHANDLER_H
