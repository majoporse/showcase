#include "connectionhandler.h"
#include <QTcpSocket>
#include <sstream>
#include <iostream>
#include <QTextStream>

ConnectionHandler::ConnectionHandler(MyServer *server, qintptr sockNo): Server{server}, sockNo{sockNo}
{
}

void ConnectionHandler::run()
{

}
