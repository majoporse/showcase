#ifndef CLIENT_H
#define CLIENT_H
#include <QNetworkAccessManager>
#include <QObject>
#include <QNetworkReply>
#include <QPushButton>
#include <QTextEdit>
#include <QLabel>
#include <vector>
#include <QDomDocument>
#include <QRadioButton>
#include <QListView>
#include <QStandardItemModel>
#include <QLineEdit>
#include <QWebEngineView>
#include <QTextBrowser>

class client: public QObject
{
    Q_OBJECT
    std::vector<QDomDocument> docs;
    QString serverUrl;

    QNetworkAccessManager *manager;
    QStandardItemModel *streams;
    std::vector<QStandardItemModel *>vecItems;

    //views
    QListView *streamsView;
    QListView *itemsView;

    //buttons
    QPushButton *baddLabel;
    QPushButton *baddStreamServer;
    QPushButton *bfetchStream;
    QPushButton *bremoveStream;
    QRadioButton *radioButton;

    QTextEdit *teAddLabel;
    QTextEdit *teAddStram;
    QLineEdit *teTime;
    QLabel *labelLabel;

    //details
    QLabel *imageLabel;
    QTextBrowser *feedDesc;
    QTextBrowser *feedDetails;

    QTextBrowser *itemDesc;

    void addStream(QNetworkReply* reply);
    void removeCurrentStream();
    void resetDetails();
    void clearDetails();
    void parseDom(const QDomElement &, QNetworkReply *reply);
    void changeDetails(int index, QNetworkReply *reply);

public:
    client(QWidget &, QString);

public slots:
    void processReply(QNetworkReply*);
    void requestPic(const QModelIndex &index);
};

#endif // CLIENT_H
