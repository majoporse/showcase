#ifndef DATABASE_H
#define DATABASE_H

#include <QDir>
#include <QJsonArray>
#include <QJsonDocument>
#include <QFile>
#include <QJsonObject>
#include <QNetworkAccessManager>
#include <memory>
#include <QThread>

#ifndef CONFIG_H
#include "config.h"
#endif

#ifndef DATABASEUPDATER_H
#include "databaseupdater.h"
#endif

class databaseUpdater;

class database: public QObject
{
    Q_OBJECT

    databaseUpdater *updater;
    QThread *updaterThread;

    std::unique_ptr<config> conf;

public:
    database();
    bool addStream(const QString&, int, QList<QString>);
//    QByteArray getStreamContent(QString &url);
    std::vector<QString> get_streams(const QString &name);

    QString get_last_poll(const QString&name);
    int get_last_msg(const QString& name);
    bool contains(const QString &url);
    void remove(const QString &url);
    QByteArray getStreamContent(const QString &url);

signals:
    void newStream(const QString &url, int time);
    void stopTimer(const QString &url);
};

#endif // DATABASE_H
