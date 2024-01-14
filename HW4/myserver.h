#ifndef MYSERVER_H
#define MYSERVER_H

#include <QObject>
#include <QTcpServer>
#include <QHttpServer>
#include <QDir>
#include <QNetworkAccessManager>
#include <QNetworkReply>
#include <QString>
#include <vector>
#include <tuple>
#include <QJsonArray>
#include <QJsonDocument>

#include "database.h"

class MyServer: public QObject
{
    Q_OBJECT
    QTcpServer *tcpServ;
    QHttpServer *httpServ;

    database *data;

    QNetworkAccessManager *manager;

    QString address;
    qintptr port;

    void processResponse(QNetworkReply *);

public:
    MyServer(qintptr portNumber, QObject *parent = nullptr);
    ~MyServer();
    bool running() const;

public slots:
    void start_listening();
    void stop();



signals:
    void autoget(QString, int);
    void ending();
    void internalRequest();

};

#endif // MYSERVER_H
