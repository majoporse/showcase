#include <QCoreApplication>
#include <string>
#include <QTcpSocket>
#include <QTextStream>
#include <iostream>
#include <QThread>
#include <QHostAddress>

int main(int argc, char *argv[])
{
    if (argc != 3){
        std::cout << "wrong number of parameters!\nparameters: <name> <ipv4 address>";
        return -1;
    }
    QTcpSocket sock;
    sock.connectToHost(QHostAddress(argv[2]), 8888);

    sock.waitForConnected();
    if (sock.state() != QAbstractSocket::ConnectedState) {
        std::cerr << "Error: " << sock.errorString().toStdString() << std::endl;
        return -1;
    }

    QTextStream ts(&sock);
    ts << argv[1];
    ts.flush();
    sock.waitForBytesWritten();

    for (std::string cur_line;std::getline(std::cin, cur_line) && sock.state() == QAbstractSocket::ConnectedState;){
        ts << cur_line.data();
        ts.flush();
//        sock.waitForBytesWritten();

        sock.waitForReadyRead();
//            std::cout << "Timed out!\n";
//            return 0;
//        }
        QString s = ts.readAll();
        std::cout << s.toStdString();
    }


    if (sock.state() != QAbstractSocket::UnconnectedState) {
        sock.waitForDisconnected();
    }
    return 0;
}
