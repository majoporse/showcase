#include "database.h"
#include <QNetworkReply>
#include <QStandardPaths>
#include <QThread>
#include <QXmlStreamReader>
#include <QThread>
#include <QNetworkReply>
#include <shared_mutex>

database::database():conf{new config}{

    updaterThread = new QThread();

    updater = new databaseUpdater(conf.get());
    updater->moveToThread(updaterThread);

    connect(this, &database::newStream, updater, &databaseUpdater::setupTimer, Qt::QueuedConnection); // <-- sends signal to another thread
    connect(this, &database::stopTimer, updater, &databaseUpdater::stopTimer, Qt::QueuedConnection);

    connect(updaterThread, &QThread::started, updater, &databaseUpdater::start);
    connect(updaterThread, &QThread::finished, updater, &QObject::deleteLater);
    updaterThread->start();
}

bool database::addStream(const QString &url, int time, QList<QString> labels){
    if (auto u = QUrl(url); !u.isValid() || u.isRelative()){
        qDebug() << "not valid url" << u.toString();
        return false;
    }
    conf->addConfig(url, time, labels);
    //get xml file and save to disk,
    emit stopTimer(url);
    emit newStream(url, time);
    return true;
}

std::vector<QString> database::get_streams(const QString &label){
    std::shared_lock<std::shared_mutex> lock(conf->mut);

    std::vector<QString> to_read;
    for (auto it = conf->configJson.begin(); it != conf->configJson.end(); ++it) {
        QJsonValue value = it.value();

        if (label == "" || value.toObject()["labels"].toArray().contains(label)){
            to_read.emplace_back(std::move( it.key() ));
        }
    }

    return to_read;
}

QString database::get_last_poll(const QString &url){
    return conf->configJson[url].toObject()["last_poll"].toString();
}

int database::get_last_msg(const QString& url){

    for (auto a: conf->configJson.keys()){
        if (a == url){
            QJsonObject obj = conf->configJson[a].toObject();
            if (obj.contains("last_msg")){
                return obj["last_msg"].toInt();
            }
            break;

        }
    }
    return QNetworkReply::NoError;
}

bool database::contains(const QString &url){
    return conf->configJson.contains(url);
}

void database::remove(const QString &url){
    emit stopTimer(url);
    conf->removeConfig(url);
    QString path = conf->datadir.filePath(QString{url}.replace("/", ""));
    QFile toRemove(path);
    if (!toRemove.remove())
        qDebug() << "couldn't remove file";
    else
        qDebug() << "removed file " << path;
}

QByteArray database::getStreamContent(const QString &url){
    QFile f(conf->datadir.filePath(QString{url}.remove("/")));
    QByteArray buffer;
    if (f.open(QIODevice::ReadOnly))
        buffer = f.readAll();
    else
        qDebug() << "couldn't open a file" << f.fileName();
    return buffer;
}

