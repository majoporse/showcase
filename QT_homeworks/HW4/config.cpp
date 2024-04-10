#include "config.h"
#include <QJsonDocument>
#include <QIODevice>
#include <QStandardPaths>
#include <QJsonArray>
#include <QJsonObject>
#include <mutex>

config::config():
    datadir(QStandardPaths::writableLocation(QStandardPaths::AppLocalDataLocation)),
    configFile{datadir.filePath("refreshTimes")}
{
    loadConfigFile();
}

void config::loadConfigFile(){
    if (!configFile.exists())
        datadir.mkpath(datadir.absolutePath());

    QJsonDocument doc;
    if (configFile.open(QIODeviceBase::ReadWrite)){
        QJsonParseError e;
        doc = doc.fromJson(configFile.readAll(), &e);
        if (e.error != QJsonParseError::NoError){
            qDebug() << "failed to parse configfile\n";
        } else{
            qDebug() << "configfile opened successfully\n";
        }
        configFile.close();
    }else{
        qDebug() << "couldn't open configfile\n";
    }

    configJson = doc.object();

}

void config::addConfig(const QString &url, int time, QList<QString> &labels){
    std::unique_lock<std::shared_mutex> lock(mut);

    //prep the config file
    QJsonArray jsonLabels;
    if (!configJson.contains(url)){
        configJson.insert(url, QJsonObject{});
    }
    auto obj = configJson[url].toObject();
    for (auto &label: labels){
        jsonLabels.push_back(std::move(label));
    }

    obj.insert("time", QJsonValue(time));
    obj.insert("labels", jsonLabels);

    configJson.insert(url, obj);
}

void config::removeConfig(const QString &url){
    std::unique_lock<std::shared_mutex> lock(mut);
    configJson.remove(url);
}

void config::updateConfigTime(const QString &url, QTime time, int reply){
    std::unique_lock<std::shared_mutex> lock(mut);

    QJsonValue jsontime(time.toString());
    QJsonValue jsonreply(reply);

    //replaces value if necessary
    auto obj = configJson[url].toObject();
    obj.insert("last_poll", jsontime);
    obj.insert("last_msg", jsonreply);
    configJson.insert(url, obj);
}

config::~config(){
    QJsonDocument doc;
    doc.setObject(configJson);

    configFile.open(QIODeviceBase::WriteOnly);
    configFile.write(doc.toJson());
    configFile.close();
}
