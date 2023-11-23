#include <QCoreApplication>
#include <QTcpServer>
#include "server.h"

int main(int argc, char *argv[])
{
    QCoreApplication a(argc, argv);
    Server serv;

    return a.exec();
}
