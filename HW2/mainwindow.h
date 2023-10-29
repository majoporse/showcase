#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QMainWindow>
#include <QPushButton>
#include <string>
#include <QString>
#include <QValidator>
#include <QRegularExpression>
#include <vector>
#include <utility>
#include "rational.h"
#include <sstream>


QT_BEGIN_NAMESPACE
namespace Ui { class MainWindow; }
QT_END_NAMESPACE

class MainWindow : public QMainWindow
{
    Q_OBJECT
    QValidator * v;
    QClipboard *c;

public:
    MainWindow(QWidget *parent = nullptr);
    ~MainWindow();
    std::vector<rational> memory;
    rational result{0,1};
    void updateLabel(const char*    );
    void keyPressEvent(QKeyEvent*);

private:
    Ui::MainWindow *ui;
    void compute_result();
    rational parse_input(std::stringstream&);
    void showResult();
    void addMem();


signals:
    void pressed(const char*);
    void pressedEq();
    void pressedBs();
};
#endif // MAINWINDOW_H
