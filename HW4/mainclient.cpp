#include <QApplication>
#include <QWidget>
#include <QLabel>
#include <QPushButton>
#include <QVBoxLayout>
#include <QSplitter>
#include <QFileSystemModel>
#include <QListView>
#include <QTreeView>
#include <QStandardItemModel>
#include <QTableView>
#include <QPushButton>
#include <QGridLayout>
#include <QLineEdit>
#include <QComboBox>
#include <QLabel>
#include <QTextEdit>

#include <QHBoxLayout>
#include "client.h"


int main(int argc, char *argv[])
{

    // Check the number of command-line arguments
    if (argc != 2) {
        qDebug() << "Usage: " << argv[0] << " <URL>";
        return -1;
    }

    // Parse the URL from the command-line argument
    QUrl url(argv[1]);

    // Check if the URL is valid
    if (!url.isValid() || url.isRelative()) {
        qDebug() << "Invalid URL format. Please provide a valid URL.";
        return -1;
    }

    qDebug() << "Parsed URL: " << url.toString();
    QApplication ap(argc, argv);
    QWidget mainWindow;

    client c(mainWindow, argv[1]);
    mainWindow.show();
    return ap.exec();
}
