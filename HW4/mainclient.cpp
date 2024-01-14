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
    QApplication ap(argc, argv);
    QWidget mainWindow;

    client c(mainWindow);
    mainWindow.show();
    return ap.exec();
}
