#include "client.h"
#include <QSplitter>
#include <QFileSystemModel>
#include <QTextBrowser>
#include <QListView>
#include <QTreeView>
#include <QStandardItemModel>
#include <QTableView>
#include <QPushButton>
#include <QGridLayout>
#include <QLineEdit>
#include <QComboBox>
#include <QTextEdit>
#include <QLabel>
#include <QStyledItemDelegate>
#include <QPainter>
#include <QSpacerItem>
#include <QErrorMessage>
#include <QDomDocument>
#include <QMessageBox>
#include <QRadioButton>
#include <QRegularExpressionValidator>
#include <QNetworkReply>

#define ADD 1
#define REMOVE 2
#define LAST_POLL 3
#define STATE 4
#define FETCH 5
#define DELETE 6
#define IMAGE 7

const char *finger = R"(
░░░░░░░░░░░░░░░░░░░░░░█████████░░░░░░░░░
░░███████░░░░░░░░░░███▒▒▒▒▒▒▒▒███░░░░░░░
░░█▒▒▒▒▒▒█░░░░░░░███▒▒▒▒▒▒▒▒▒▒▒▒▒███░░░░
░░░█▒▒▒▒▒▒█░░░░██▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██░░
░░░░█▒▒▒▒▒█░░░██▒▒▒▒▒██▒▒▒▒▒▒██▒▒▒▒▒███░
░░░░░█▒▒▒█░░░█▒▒▒▒▒▒████▒▒▒▒████▒▒▒▒▒▒██
░░░█████████████▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██
░░░█▒▒▒▒▒▒▒▒▒▒▒▒█▒▒▒▒▒▒▒▒▒█▒▒▒▒▒▒▒▒▒▒▒██
░██▒▒▒▒▒▒▒▒▒▒▒▒▒█▒▒▒██▒▒▒▒▒▒▒▒▒▒██▒▒▒▒██
██▒▒▒███████████▒▒▒▒▒██▒▒▒▒▒▒▒▒██▒▒▒▒▒██
█▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒█▒▒▒▒▒▒████████▒▒▒▒▒▒▒██
██▒▒▒▒▒▒▒▒▒▒▒▒▒▒█▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██░
░█▒▒▒███████████▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██░░░
░██▒▒▒▒▒▒▒▒▒▒████▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒█░░░░░
░░████████████░░░█████████████████░░░░░░
)";
const char *amogus = R"(
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⠞⠉⠉⠉⠉⠉⠉⠙⠓⢦⣄
⠀⠀⠀⠀⠀⠀⠀⠀⠀⣴⠏⢠⣤⡶⠶⠶⠶⠶⣤⣄⠀⠹⣧⡀
⠀⠀⠀⠀⠀⠀⠀⠀⣸⠟⠀⠸⣿⣦⣄⣀⣠⣤⣤⡿⠀⠀⠘⣧
⠀⠀⠀⠀⠀⠀⠀⢰⡟⠀⠀⠀⠀⠉⠉⠉⠉⠉⠁⠀⠀⠀⠀⢹⡄
⠀⠀⠀⠀⠀⠀⠀⣼⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⡇
⠀⠀⠀⠀⠀⠀⢀⡏⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠸⡇
⠀⠀⠀⠀⠀⠀⣸⠃⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣇
⠀⠀⠀⠀⠀⢀⡿⠀⠀⠀⢠⡶⠒⠒⠒⠒⠲⣦⡀⠀⠀⠀⠀⠀⣿
⠀⣀⣠⣄⣀⣼⡇⠀⠀⠀⢸⡇⠀⠀⠀⠀⠀⢸⡇⠀⠀⠀⠀⠀⡇
⠸⣏⡀⠀⠀⠈⠀⠀⠀⣀⣼⡇⠀⠀⣀⣀⣠⣼⠃⠀⠀⠀⠀⠀⡏
⠀⠈⠛⠛⠒⠒⠒⠛⠛⠉⠁⠀⠀⣾⠉⠀⠀⠀⠀⠀⠀⠀⢀⣰⠇
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠉⠛⠒⠒⠒⠚⠛⠉⠉
)";

const char *buttonStyle = "QPushButton {"
                          "background-color: #7289da;"   // Discord's brand color
                          "color: #ffffff;"              // White text color
                          "border: none;"                // No border
                          "padding: 10px;"               // Padding around text
                          "border-radius: 5px;"         // More rounded corners
                          "}"
                          "QPushButton:hover {"
                          "background-color: #677bc4;"   // Slightly darker on hover
                          "}";

const char *radioStyle ="QRadioButton {"
                        "   color: #FFFFFF; /* Text color */"
                        "   background-color: #36393F; /* Background color */"
                        "}"
                        "QRadioButton::indicator {"
                        "   width: 12px; /* Size of the radio button */"
                        "   height: 12px;"
                        "   border-radius: 6px; /* Round the radio button */"
                        "   border: 2px solid #7289DA; /* Border color of the radio button */"
                        "   background-color: #7289DA; /* Fill color of the radio button */"
                        "}"
                        "QRadioButton::indicator:checked {"
                        "   background-color: #7289DA; /* Fill color when checked */"
                        "   border: 2px solid #7289DA; /* Border color when checked */"
                        "}"
                        "QRadioButton::indicator:unchecked {"
                        "   background-color: #36393F; /* Fill color when unchecked */"
                        "   border: 2px solid #7289DA; /* Border color when unchecked */"
                        "}";

const char *teStyle = "QTextEdit, QTextBrowser {"
                      "background-color: #36393f;"  // Darker background color
                      "color: #ffffff;"              // Light text color
                      "border: 1px solid #7289da;"   // Border color
                      "padding: 8px;"               // Padding
                      "border-radius: 5px;"         // More rounded corners
                      "}"
                      "QScrollBar:vertical {"
                      "border: 2px solid #2f3136;"   // Scrollbar border color
                      "background: #2c2f33;"        // Scrollbar background color
                      "width: 12px;"                // Scrollbar width
                      "}"
                      "QScrollBar::handle:vertical {"
                      "min-height: 20px;"           // Minimum height for the handle
                      "border: 0px solid black;"
                      "border-radius: 4px;"
                      "background-color: #7289da;"        // Scrollbar handle color
                      "}"
                      "QScrollBar::handle:vertical:hover {"
                      "background-color: #677bc4;"   // Slightly darker on hover
                      "}"
                      "QScrollBar::add-line:vertical, QScrollBar::sub-line:vertical {"
                      "background: none;"            // No background for arrow buttons
                      "}"
                      "QScrollBar::add-page:vertical, QScrollBar::sub-page:vertical {"
                      "background: none;"            // No background for page area"
                      "}";

const char *leStyle = "QLineEdit {"
                      "background-color: #36393f;"  // Darker background color
                      "color: #ffffff;"              // Light text color
                      "border: 1px solid #7289da;"   // Border color
                      "padding: 8px;"               // Padding
                      "border-radius: 5px;"         // More rounded corners
                      "}";

const char *desclabelStyle = "QLabel {"
                             "background-color: #36393f;"  // Darker background color
                             "color: #ffffff;"              // Light text color
                             "border: 1px solid #7289da;"   // Border color
                             "padding: 8px;"               // Padding
                             "border-radius: 5px;"         // More rounded corners
                             "}";

const char * listStyle = "QListView {"
                         "background-color: #36393f;"  // Dark background color
                         "color: #ffffff;"              // Light text color
                         "border: 3px solid #7289da;"   // Border color
                         "border-radius: 5px;"         // More rounded corners
                         "}"
                         "QListView::item {"
                         "background-color: #2f3136;"  // Item background color
                         "padding: 8px;"               // Padding around items
                         "border-bottom: 1px solid #444f77;"  // Separator color
                         "border-radius: 2px;"         // More rounded corners
                         "}"
                         "QListView::item:selected {"
                         "background-color: #202225;"  // Selected item background color
                         "color: #ffffff;"              // Selected item text color
                         "}"
                         "QScrollBar {"
                         "border: 2px solid #2f3136;"   // Scrollbar border color
                         "background: #2c2f33;"        // Scrollbar background color
                         "width: 12px;"                // Scrollbar width
                         "}"
                         "QScrollBar::handle {"
                         "min-height: 20px;"           // Minimum height for the handle
                         "border: 0px solid black;"
                         "border-radius: 4px;"
                         "background-color: #7289da;"        // Scrollbar handle color
                         "}"
                         "QScrollBar::handle:hover {"
                         "background-color: #677bc4;"   // Slightly darker on hover
                         "}"
                         "QScrollBar::add-line, QScrollBar::sub-line {"
                         "background: none;"            // No background for arrow buttons
                         "}"
                         "QScrollBar::add-page, QScrollBar::sub-page {"
                         "background: none;"            // No background for page area"
                         "}";

const char *labelStyle = "QLabel {"
                         "color: #ffffff;"              // White text color
                         "}";

#include <QMouseEvent>
#include <QDesktopServices>
class ThreeLabelsItemDelegate : public QStyledItemDelegate
{
public:
    ThreeLabelsItemDelegate(QObject *parent = nullptr) : QStyledItemDelegate(parent) {}
    QSize sizeHint(const QStyleOptionViewItem &option, const QModelIndex &index) const override
    {
        return QSize(400, 120);
    }

    void paint(QPainter *painter, const QStyleOptionViewItem &option, const QModelIndex &index) const override
    {

        QStyledItemDelegate::paint(painter, option, index);  // <-- it took me forever to do this


        QStringList labels;
        labels << index.data(Qt::UserRole).toString();
        labels << index.data(Qt::UserRole + 1).toString();
        labels << index.data(Qt::UserRole + 3).toString();
        labels << index.data(Qt::UserRole + 4).toString();
        labels << index.data(Qt::UserRole + 5).toString();

        QRect leftRect = option.rect.adjusted(5, 5, -option.rect.width() - 5, -5);
        for (int i = 0; i < 5; ++i) {
            QRect labelRect = leftRect;
            labelRect.setHeight(15);  // Adjust the divisor for more spacing
            labelRect.moveTop(leftRect.top() + i * labelRect.height() + i * 5);  // Add vertical gap between items
            // Render other labels as regular text
            QTextDocument doc;
            doc.setHtml(QString("<span style='color:white;'>%1</span>").arg(labels.at(i)));
            doc.setTextWidth(labelRect.width());  // Set the width to the available space

            painter->save();
            painter->translate(labelRect.topLeft());
            doc.drawContents(painter);
            painter->restore();
        }
    }
};


client::client(QWidget &mainWindow, QString serverUrl): serverUrl{serverUrl}
{
    manager = new QNetworkAccessManager();
    connect(manager, &QNetworkAccessManager::finished, this, &client::processReply);

    QHBoxLayout *splitter = new QHBoxLayout;

    streams = new QStandardItemModel;
    QStandardItem *parentItem = streams->invisibleRootItem();

    //default item xd
    QStandardItem *item = new QStandardItem(QString("item %0").arg(0));
    item->setData("miro jaros", Qt::DisplayRole);
    item->setData(QString("title: %0").arg(0), Qt::UserRole + 1);
    item->setData("link: ", Qt::UserRole + 2);
    item->setData("desc: ", Qt::UserRole + 3);
    item->setData("date: ", Qt::UserRole + 4);
    item->setData("author: ", Qt::UserRole + 5);
    item->setData("enclousure: ", Qt::UserRole + 6);
    item->setData(1, Qt::UserRole + 7); //index in docs
    parentItem->appendRow(item);


    /*******************STREAMS*********************/
    streamsView = new QListView;
    streamsView->setModel(streams);
    streamsView->setMaximumWidth(200);
    streamsView->setStyleSheet(listStyle);
    splitter->addWidget(streamsView);

    /*************BUTTONS AND LABELS****************/
    QGridLayout *mainLayout = new QGridLayout();

    baddStreamServer = new QPushButton("Add stream to proxy");
    bfetchStream = new QPushButton("Fetch stream from proxy");
    teAddStram = new QTextEdit();
    teAddStram->setMaximumHeight(40);
    teAddStram->setPlaceholderText("Url");

    baddLabel = new QPushButton("Add label");

    labelLabel = new QLabel();
    labelLabel->setText("Labels: ");

    teAddLabel= new QTextEdit();
    teAddLabel->setMaximumHeight(40);
    teAddLabel->setPlaceholderText("Add at least one label");

    QLabel *labelTime = new QLabel("Time");

    teTime = new QLineEdit;
    teTime->setMaximumHeight(40);
    teTime->setPlaceholderText("Enter poll time");
    QRegularExpression re("[0-9]*");
    teTime->setValidator(new QRegularExpressionValidator(re, this));

    QHBoxLayout *addStreamLayout = new QHBoxLayout();
    addStreamLayout->addWidget(bfetchStream);
    addStreamLayout->addWidget(baddStreamServer);
    addStreamLayout->addWidget(teAddStram);

    QHBoxLayout *addLabelLayout= new QHBoxLayout();
    addLabelLayout->addWidget(labelLabel);
    addLabelLayout->addWidget(baddLabel);
    addLabelLayout->addWidget(teAddLabel);
    addLabelLayout->addWidget(labelTime);
    addLabelLayout->addWidget(teTime);

    mainLayout->addLayout(addStreamLayout, 0, 0);
    mainLayout->addLayout(addLabelLayout, 1, 0);
    mainLayout->addLayout(splitter, 2, 0);

    //styling
    baddLabel->setStyleSheet(buttonStyle);
    baddStreamServer->setStyleSheet(buttonStyle);
    bfetchStream->setStyleSheet(buttonStyle);
    teAddLabel->setStyleSheet(teStyle);
    teAddStram->setStyleSheet(teStyle);
    labelLabel->setStyleSheet(labelStyle);
    teTime->setStyleSheet(leStyle);
    labelTime->setStyleSheet(labelStyle);

    /***********STREAM DETAILS**********/
    itemsView = new QListView;
    itemDesc = new QTextBrowser;
    auto itemLayout = new QHBoxLayout;
    itemLayout->addWidget(itemsView);
    itemLayout->addWidget(itemDesc);
    itemsView->setHorizontalScrollBarPolicy(Qt::ScrollBarAlwaysOn);


    vecItems.push_back(new QStandardItemModel);
    itemsView->setModel(vecItems[0]);
    itemsView->setStyleSheet(listStyle);
    itemDesc->setStyleSheet(teStyle);
    itemDesc->setReadOnly(true);
    itemDesc->setPlaceholderText("description goes here");
    itemDesc->setOpenExternalLinks(true);

    // Set the custom item delegate
    ThreeLabelsItemDelegate *delegate = new ThreeLabelsItemDelegate(this);
    itemsView->setItemDelegate(delegate);

    imageLabel = new QLabel;
    auto reply = manager->get( QNetworkRequest(QUrl("https://mrtns.sk/tovar/_xl/2228/xl2228657.jpg?v=17004114482")));
    reply->setProperty("type", STATE);

    imageLabel->setAlignment(Qt::AlignCenter);
    imageLabel->setStyleSheet(desclabelStyle);

    feedDetails = new QTextBrowser;
    feedDesc = new QTextBrowser;
    feedDesc->setReadOnly(true);
    feedDesc->setPlaceholderText(amogus);
    feedDetails->setHtml("mirojaorsmirojaros");
    feedDesc->resize(1024, 750);
    feedDetails->resize(1024, 150);
    feedDesc->setOpenExternalLinks(true);
    feedDetails->setOpenExternalLinks(true);

    bremoveStream = new QPushButton("Remove stream");
    radioButton = new QRadioButton("On server also");

    feedDetails->setStyleSheet(teStyle);
    feedDesc->setStyleSheet(teStyle);
    bremoveStream->setStyleSheet(buttonStyle);
    radioButton->setStyleSheet(radioStyle);

    feedDesc->setMaximumWidth(500);
    feedDetails->setMaximumWidth(500);
    feedDetails->setMaximumHeight(140);
    bremoveStream->setMaximumWidth(300);
//    feedDesc->setWordWrap(true);

    QHBoxLayout *streamDetails = new QHBoxLayout;
    QHBoxLayout *removeLayout = new QHBoxLayout;
    QVBoxLayout *wholeStream = new QVBoxLayout;
    QVBoxLayout *detailsLayout = new QVBoxLayout;

    detailsLayout->addWidget(feedDetails);
    detailsLayout->addWidget(feedDesc);

    removeLayout->addWidget(bremoveStream);
    removeLayout->addWidget(radioButton);

    detailsLayout->addLayout(removeLayout);

    streamDetails->addWidget(imageLabel, 0);
    streamDetails->addLayout(detailsLayout, 1);

    wholeStream->addLayout(streamDetails);
    wholeStream->addLayout(itemLayout);

    splitter->addLayout(wholeStream);

    mainWindow.setLayout(mainLayout);
    mainWindow.setWindowTitle("Model View in GridLayout");
    mainWindow.setStyleSheet("QWidget {"
                             "background-color: #36393f;"  // Dark background color
                             "}");

    /*************CONNECTING******************/


    connect(baddLabel, &QPushButton::pressed, this, [&](){
        QString l = teAddLabel->toPlainText();
        if (l.size() > 0)
            labelLabel->setText(labelLabel->text() + " " + l);
        teAddLabel->clear();
    });

    connect(baddStreamServer, &QPushButton::pressed, this, [&](){

        QString url = teAddStram->toPlainText();
        teAddStram->clear();

        QString time = teTime->text();
        teTime->clear();

        QString labels = labelLabel->text();
        labels.remove(0, 8);
        labelLabel->setText("Labels: ");

        auto request = QNetworkRequest(QUrl(QString(serverUrl) + "/add;" + time + ";" + labels));
        request.setHeader(QNetworkRequest::ContentTypeHeader, "text/plain");  // Set the content type

        qDebug() << QString(serverUrl) + "/add;" + time + ";" + labels;

        auto *reply = manager->post(request, url.toUtf8());
        reply->setProperty("type", ADD);
    });

    connect(bremoveStream, &QPushButton::pressed, this, [&](){
        QString url = streamsView->currentIndex().data(Qt::UserRole + 5).toString();
        if (streamsView->currentIndex().row() == -1){
            QErrorMessage msg;
            msg.showMessage("NO STREAM SELECTED");
            msg.exec();
            return;
        }
        if (radioButton->isChecked()){
            auto request = QNetworkRequest(QUrl(QString(serverUrl) + "/remove"));
            request.setHeader(QNetworkRequest::ContentTypeHeader, "text/plain");  // Set the content type
            auto *reply = manager->post(request, url.toUtf8());
            reply->setProperty("type", REMOVE);
        }
        removeCurrentStream();
    });

    connect(bfetchStream, &QPushButton::pressed, this, [&](){

        QString url = teAddStram->toPlainText();
        teAddStram->clear();

        auto request = QNetworkRequest(QUrl(QString(serverUrl) + "/feed"));
        request.setHeader(QNetworkRequest::ContentTypeHeader, "text/plain");  // Set the content type

        qDebug() << QString(serverUrl) + "/feed";

        auto *reply = manager->post(request, url.toUtf8());
        reply->setProperty("type", FETCH);
        reply->setProperty("url", url);
    });

    connect(streamsView, &QAbstractItemView::clicked, this, &client::requestPic);
    connect(itemsView, &QAbstractItemView::clicked, this, [&](const QModelIndex &index){ itemDesc->setHtml(index.data(Qt::UserRole + 2).toString()); });
}

void client::clearDetails(){
    imageLabel->setPixmap({});
    feedDetails->clear();
    itemDesc->clear();
    feedDesc->clear();
}

void client::parseDom(const QDomElement &element, QNetworkReply *reply){
    QDomElement cur = element;
    cur = cur.firstChildElement();
    qDebug() << cur.tagName(); //should be channel
    auto item = new QStandardItem;
    item->setData(QString{"Title:\n"} + cur.elementsByTagName("title").at(0).toElement().text(), Qt::DisplayRole); //title
    item->setData(QString{"Link:\n"} + cur.elementsByTagName("link").at(0).toElement().text(), Qt::UserRole); //link
    item->setData(QString{"Description:\n"} + cur.elementsByTagName("description").at(0).toElement().text(), Qt::UserRole + 1); //description
    item->setData(QString{"PubDate:\n"} + cur.elementsByTagName("pubDate").at(0).toElement().text(), Qt::UserRole + 2); //pubDate
    item->setData(QString{"Last build date:\n"} + cur.elementsByTagName("lastBuildDate").at(0).toElement().text(), Qt::UserRole + 3); //lastbuilddate
    item->setData(QString{"Image:\n"} + cur.elementsByTagName("image").at(0).toElement().elementsByTagName("url").at(0).toElement().text(), Qt::UserRole + 4); //image
    item->setData(reply->property("url").toString(), Qt::UserRole + 5);
    streams->appendRow(item);

    auto model = new QStandardItemModel;
    auto items = cur.elementsByTagName("item");
    int i = 0;
    while (i++ < items.count() - 1){
        auto curItem = new QStandardItem;
        auto currElem = items.at(i).toElement();

        curItem->setData(QString{"Title:\n"} + currElem.elementsByTagName("title").at(0).toElement().text(), Qt::UserRole);
        curItem->setData(QString{"Link:\n"} + currElem.elementsByTagName("link").at(0).toElement().text(), Qt::UserRole + 1);
        curItem->setData(QString{"Description:\n"} + currElem.elementsByTagName("description").at(0).toElement().text(), Qt::UserRole + 2);
        curItem->setData(QString{"Author:\n"} + currElem.elementsByTagName("author").at(0).toElement().text(), Qt::UserRole + 3);
        curItem->setData(QString{"PubDate:\n"} + currElem.elementsByTagName("pubDate").at(0).toElement().text(), Qt::UserRole + 4);
        curItem->setData(QString{"Enclosure:\n"} + currElem.elementsByTagName("enclosure").at(0).toElement().attribute("url"), Qt::UserRole + 5);
        model->appendRow(curItem);
    }
    vecItems.push_back(model);
}

void client::addStream(QNetworkReply *reply){
    QDomDocument doc;
    if (reply->error() != QNetworkReply::NoError){
        qDebug() << reply->errorString();
        return;
    }
    doc.setContent(reply->readAll());
//    qDebug() << doc.toString();
    parseDom(doc.documentElement(), reply);

    docs.push_back(std::move(doc));
}

void client::removeCurrentStream(){
    int i = streamsView->currentIndex().row();

    streamsView->selectionModel()->clearSelection();
    //sets index to invalid
    streamsView->setCurrentIndex({});

    clearDetails();

    //remove data
    docs.erase(docs.begin() + i);
    streams->removeRow(i);
    std::swap(vecItems[i], vecItems.back());
    delete vecItems.back();
    vecItems.pop_back();
}

void client::requestPic(const QModelIndex &index){
    qDebug() << "requesting picture: " << index.data(Qt::UserRole + 4).toString().remove(0, 7);
    auto reply = manager->get(QNetworkRequest(index.data(Qt::UserRole + 4).toString().remove(0, 7)));
    reply->setProperty("cur", index.row());
    reply->setProperty("type", IMAGE);
}

void client::changeDetails(int row, QNetworkReply *reply){

    auto current = streams->index(row, 0);
    feedDesc->setHtml(current.data(Qt::UserRole + 1).toString().toUtf8());
    QString text(current.data(Qt::DisplayRole).toString());
    text.append("\n");
    text.append(current.data(Qt::UserRole).toString());
    text.append("\n");
    text.append(current.data(Qt::UserRole + 2).toString());
    text.append("\n");
    text.append(current.data(Qt::UserRole + 3).toString());

    feedDetails->setText(text);

    itemsView->setModel(vecItems[current.row()]);
    QPixmap img;
    img.loadFromData(reply->readAll());
    imageLabel->setPixmap(img.scaledToWidth(200));
    itemsView->setModel(vecItems[row]);
}

void client::processReply(QNetworkReply *reply){

    QMessageBox box;
    QPixmap p;
    QErrorMessage msg;
    if (reply->error() != QNetworkReply::NoError){
        msg.showMessage(reply->errorString());
        msg.exec();
        return;
    }

    switch (reply->property("type").toInt()) {

    case ADD:
        box.setText(QString{"ADDED SUCCESSFULLY"} + finger);
        box.exec();
        break;

    case FETCH:
        addStream(reply);
        break;

    case REMOVE:
        box.setText(QString{"REMOVED SUCCESSFULLY"} +finger);
        box.exec();
        break;

    case LAST_POLL:
        break;

    case STATE:
        p.loadFromData(reply->readAll());
        imageLabel->setPixmap(p.scaledToWidth(200));
        break;
    case IMAGE:
        changeDetails(reply->property("cur").toInt(), reply);
        break;

    default:
        qDebug() << "wtf";
        break;
    }

    reply->deleteLater();
}
