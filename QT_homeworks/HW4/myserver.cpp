#include "myserver.h"
#include <QJsonDocument>
#include <QJsonObject>
#include <QXmlStreamWriter>
#include <QSocketNotifier>
#include <QStandardPaths>
#include <QDir>
#include <QtConcurrent>
#include <QTimer>

MyServer::MyServer(qintptr portNumber, QObject *parent):
    QObject(parent), port(portNumber), httpServ{}
{
    manager = new QNetworkAccessManager(this);
    httpServ = new QHttpServer;
    tcpServ = new QTcpServer;
    data = new database;
    QTimer *timer = new QTimer(this);
    timer->setSingleShot(true);
    connect(timer, &QTimer::timeout, this, &QCoreApplication::quit);

    httpServ->route("/add;<arg>;<arg>", [&] (int time, QString labels, const QHttpServerRequest &request) {
        QString url = request.body();
        url.remove("\n");
        qDebug() << "arguments:" << url << time << labels;
        return QtConcurrent::run([data=data, url, labels, time](){
            if (!data->addStream(url, time, labels.split(" ")))
                return QHttpServerResponse(QHttpServerResponder::StatusCode::BadRequest);
            return QHttpServerResponse("ok");
        });
    });
    auto handler = [data=data](QString label){
        qDebug() << label;
        return QtConcurrent::run([label, data=data](){
            QByteArray buffer;
            for (auto item: data->get_streams(label)){
                buffer.push_back(item.toUtf8());
                buffer.push_back("\n");
            }
            return QHttpServerResponse(buffer);
        });
    };
    httpServ->route("/list/", handler);
    httpServ->route("/list", [data=data, handler](){return handler("");});

    httpServ->route("/hello", [](){return QHttpServerResponse("hello");});
    httpServ->route("/bye", [timer](){
        timer->start(1000);
        return QHttpServerResponse("bye bye");
    });

    httpServ->route("/feed", [&](const QHttpServerRequest &request){
        QString url = request.body();
        url.remove("\n");
        return QtConcurrent::run([url, data=data](){
            if (!data->contains(url)){
                qDebug() << "couldn't find url";
                return QHttpServerResponse(QHttpServerResponder::StatusCode::BadRequest);
            }
            return QHttpServerResponse(data->getStreamContent(url));
        });
    });

    httpServ->route("/last/message",[data=data](const QHttpServerRequest &request){
        QString url = request.body();
        url.remove("\n");
        return QtConcurrent::run([data=data, url](){
            if (!data->contains(url)){
                qDebug() << "couldn't find url";
                return QHttpServerResponse(QHttpServerResponder::StatusCode::BadRequest);
            }
            return QHttpServerResponse(QString::number(data->get_last_msg(url)));
        });
    });

    httpServ->route("/last/poll", [&](const QHttpServerRequest &request){
        QString url = request.body();
        url.remove("\n");
        return QtConcurrent::run([data=data, url](){
            if (!data->contains(url)){
                qDebug() << "couldn't find url";
                return QHttpServerResponse(QHttpServerResponder::StatusCode::BadRequest);
            }
            return QHttpServerResponse(data->get_last_poll(url));
        });
    });

    httpServ->route("/remove", [data=data](const QHttpServerRequest &request){
        QString url = request.body();
        url.remove("\n");
        qDebug() << "removing:" << url;
        if (!data->contains(url)){
            qDebug() << "couldn't find url";
            return QHttpServerResponse(QHttpServerResponder::StatusCode::BadRequest);
        }
        data->remove(url);
        return QHttpServerResponse("ok");
    });
}

MyServer::~MyServer()
{
    delete data;
}

void MyServer::start_listening()
{
    httpServ->listen(QHostAddress::Any, port);
}

void MyServer::stop()
{
    thread()->quit();
}
