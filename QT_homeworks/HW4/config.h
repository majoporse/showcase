#ifndef CONFIG_H
#define CONFIG_H
#include <shared_mutex>
#include <QDir>
#include <QJsonObject>
#include <QFile>
#include <QTime>

/*   CONFIG FILE SHOULD LOOK LIKE THIS
{
    "192.168...":
        {
            "time": 12435 (int)
            "last_msg": 1 (QNetrowkReply::NetworkError)
            "last_poll": (QTime)
            "labels":
                    [
                        12313, (string)
                        123, (string)
                    ]
        }
}
*/

class config
{

    void loadConfigFile();
public:
    mutable std::shared_mutex mut;
    QDir datadir;
    QJsonObject configJson;
    QFile configFile;
    config();
    void addConfig(const QString&, int, QList<QString>&);
    void removeConfig(const QString&);
    void updateConfigTime(const QString &url, QTime time, int reply);
    ~config();
};

#endif // CONFIG_H
