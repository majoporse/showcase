#include "databaseupdater.h"
#include <QTime>
#include <QTimer>
#include <QNetworkRequest>
#include <QNetworkReply>
#include <QXmlStreamReader>

databaseUpdater::databaseUpdater(config *c):
    conf{c}
{
}

void databaseUpdater::setupTimer(const QString& url, int time){
    time = time * 1000;
    QTimer *timer = new QTimer(this);
    timer->setProperty("url", url);
    qDebug() << "setting up timer for: " << url << "poll time: " << time;
    auto get = [url, this](){
        qDebug() << "requesting...";
        auto *reply = manager->get(QNetworkRequest(QUrl(url)));
        reply->setProperty("destinationUrl", url);
    };
    timers.push_back(timer);

    connect(timer, &QTimer::timeout, this, get);
    connect(QThread::currentThread(), &QThread::finished, timer, &QObject::deleteLater);

    get(); //poll now
    timer->start(time); //poll periodically
}

void databaseUpdater::stopTimer(const QString &url){
    for (auto timer: timers){
        if (timer->property("url") == url){
            timer->stop();
            std::swap(timer, timers.back());
            timers.pop_back();
        }
    }
}
void databaseUpdater::start(){

    std::shared_lock<std::shared_mutex> lock(conf->mut);

    manager = new QNetworkAccessManager(this);
    connect(manager, &QNetworkAccessManager::finished, this, &databaseUpdater::store);

    for (QString url: conf->configJson.keys()){
        setupTimer(url, conf->configJson[url].toObject()["time"].toInt());
    }
}

//saves the file to DATADIR/"url"
void databaseUpdater::store(QNetworkReply *reply){

    if (reply->error()){
        qDebug() << reply->errorString();
        return;
    }

    QString url = reply->property("destinationUrl").toString();
    conf->updateConfigTime(url, QTime::currentTime(), reply->error());

    QByteArray body = reply->readAll();
    QString path = conf->datadir.filePath(url.replace("/", ""));
    QFile file(path);

    if (file.open(QIODevice::WriteOnly | QIODevice::Truncate | QIODevice::Text)){
        file.write(body);
        file.close();

        qDebug() << "File updated successfully:" << path << "\n";
    }
    else{
        qWarning() << "Failed to update the file:" << file.errorString() << "\n";
    }
    reply->deleteLater();
}
