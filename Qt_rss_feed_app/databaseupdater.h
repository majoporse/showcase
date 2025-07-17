#ifndef DATABASEUPDATER_H
#define DATABASEUPDATER_H

#ifndef CONFIG_H
#include "config.h"
#endif

#ifndef DATABASE_H
#include "database.h"
#endif
#include <QFile>
#include <QNetworkAccessManager>
#include <QTimer>
#include <vector>

class database;

class databaseUpdater: public QObject
{
    Q_OBJECT
    config *conf;
    std::vector<QTimer*> timers;

public:
    QNetworkAccessManager *manager;
    databaseUpdater(config*);
    void stopTimer(const QString &url);

public slots:
    void start();
    void store(QNetworkReply *);
    void setupTimer(const QString& url, int time);
};

#endif // DATABASEUPDATER_H
