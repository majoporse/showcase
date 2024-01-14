#include <QCoreApplication>
#include "myserver.h"

int main(int argc, char *argv[])
{
    QCoreApplication a(argc, argv);

    MyServer srv(atoi(argv[1]));
    srv.start_listening();


    return a.exec();
}
