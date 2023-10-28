#include "mainwindow.h"
#include "./ui_mainwindow.h"
#include <iostream>
#include <QKeyEvent>
#include <QKeySequence>
#include <QDialog>
#include <QMessageBox>
#include <QClipboard>
#include <QErrorMessage>
#include <QTextBrowser>
const char *hintText = R"(
_____________________________

  CALCULATOR HINT:

  This calculator has an
  unlimited number of
  <memory bins>. To use
  memory, you have to
  <allocate> it first using
  the <add memory> button.

  To operate with memory,
  use the "mem" button or
  type "<M[index]>" where
  <index> is the memory
  bin number.

  The <.> operator is
  invalid and only used
  for displaying the
  number in floating point
  precision.

  If you allow display
  <display rational> you
  can save the result to
  the memory.

  The user cannot take a
  number to the power of a
  <rational number>. It
  works only on <whole>
  numbers.

  Happy calculating!
_____________________________)";

const char *sussyShrek = R"(
⠀⠀⠀⠀⠀⣀⡤⠤⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⡔⠋⠀⠀⠀⢠⡑⢄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⡜⠀⠀⠀⠀⠀⢸⡇⠈⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⢸⠀⠀⠀⠀⠀⠀⣨⡇⠀⠸⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣀⠤⠴⠒⠋⠉⠁⠈⠉⠓⠒⠤⢄⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⢸⡀⠀⠀⠀⠀⣼⣿⡟⠀⠀⢡⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⠔⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠑⠢⣄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠹⠿⢿⣿⣭⡉⠭⠀⠀⠀⠀⠱⡄⠀⠀⠀⠀⠀⢀⠎⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣀⠀⠀⠀⠀⠀⠀⠀⠙⠦⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠈⠉⠙⠓⢶⡀⠀⠀⠀⠘⢦⡀⠀⠀⡰⠃⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠤⣀⣠⢄⢍⠒⠤⠀⠀⠀⠀⠀⠀⠈⢦⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠓⢦⠄⠀⠀⠙⠢⠜⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⣤⣼⣿⠾⠿⢿⣿⣿⣶⡀⠀⠐⠀⡀⠀⠀⠀⠙⣄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢣⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠠⡾⠛⠋⠉⠁⠀⠀⠈⠙⢿⣿⣿⣷⣜⡀⠀⠀⠀⠀⠀⢨⠣⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠳⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠹⢿⣯⣝⡄⠀⠒⠀⠀⠀⠀⠙⢄⠀⠀⢀⢴⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠧⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⣠⣤⣀⡀⠀⠈⢻⣿⣷⡄⠀⠀⠀⠀⠀⠀⠀⠱⡴⠉⡼⢸⠀⠘⠆⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣸⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⣴⣾⡿⠿⢿⣿⢷⣄⠀⠀⠈⠋⠀⠀⠀⠀⠀⣠⣦⡴⣤⣜⣄⡇⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⠃⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠸⡏⢼⣿⣿⣿⣞⣿⠈⣿⠀⠀⠀⠀⠀⠀⠀⠀⠠⡟⠛⠻⣿⣿⣿⠷⠇⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⠆⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⠠⣑⣾⣿⣿⣯⡾⠏⣠⡟⠀⠀⠀⠀⠀⠀⢀⣴⢋⣁⡤⣭⣵⣌⣹⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⠎⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠻⠿⢿⣿⣷⡶⠾⠋⠀⠀⠀⠀⠀⠀⠀⠀⢷⣿⣿⣳⡟⢈⡿⠛⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⠎⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⠻⠯⣷⡿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⢀⣠⡞⢀⣶⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠱⠸⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⢀⣤⣾⣿⣿⠁⣘⠄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢡⢧⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⣶⣶⣶⣾⣿⣿⣿⡿⡿⠋⢀⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣀⣀⣀⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠠⢸⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⣿⣿⣿⣿⣿⡿⠋⡾⠁⠀⡞⠰⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⠿⠿⢿⣷⣤⡀⠀⠀⠀⠀⠀⠀⠀⢠⠎⢸⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⣿⣿⣿⣿⣿⡇⣠⣿⠀⠀⣇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠛⠷⣦⣤⣤⣤⣤⢴⠏⠀⢸⠀⠀⠀⠀⠀⠀⢀⣀⣠⠤⠤
⣿⣿⢿⣿⣿⠀⣾⣟⠀⢐⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣀⣀⠤⠔⠊⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠸⠀⢠⣮⠖⠒⠚⠉⠉⠉⠁⠀⠀⠀⠀
⣿⣿⣿⣿⣿⡋⣼⣯⠀⠈⣷⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⣿⡘⡆⠀⠁⠀⠀⠀⠀⠀⠀⠐⠀
⣾⣿⣿⣿⡏⠀⢀⣻⡆⠀⢹⡂⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣰⣿⣿⣇⣧⠀⢀⠀⠈⠀⠀⠐⠂⠀⠀
⣻⣿⣿⣿⣿⠀⡈⠸⣷⠀⠘⣷⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⠳⢶⣤⣤⣦⣤⣄⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⠋⢿⣿⣿⢸⠞⠽⠦⠠⢆⠀⢒⠀⠀⠀
⣿⣿⣿⣿⣿⡇⠐⠐⢿⣧⠀⠘⣧⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠉⠉⠛⠛⠿⠷⠶⣴⣄⣀⣀⣀⣠⡤⠤⠤⠶⠞⠁⣠⣼⣿⣿⣿⣁⢁⣀⡌⠀⣐⡐⠀⣀⠀
⣿⣿⣿⣿⣿⣷⢀⠀⠈⣼⣧⡀⠸⢷⣤⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣴⣿⣿⣿⣿⣿⣾⡙⣇⣆⣴⣤⣒⣆⠤⡠
⣿⣿⣿⣿⣿⣿⡐⠀⢄⠐⢿⣿⡀⠈⢻⡈⠂⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢰⢿⣿⣿⣿⣿⣿⣯⣿⣷⣆⣛⣟⣿⣠⠈⢈
⣿⣿⣿⣿⣿⣿⣷⠈⢆⠆⠠⢿⢱⣦⠀⠻⣄⠠⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⣏⣸⣿⣿⣿⣿⣿⣿⣷⣽⣿⣿⣿⣿⣓⡶⢶
⣿⣿⣿⡿⣿⣿⣿⣧⠈⢄⠀⠀⠀⡹⣿⣀⠈⠳⣼⣄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⢎⣁⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣃⡘
⣿⡏⠘⣆⣿⣿⣿⣿⡎⠀⠀⡀⢀⠀⠀⠛⣦⠀⠈⠛⣧⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⠋⡌⣸⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣯⣌
⣿⠀⠘⣿⣿⣞⣿⣿⣿⡄⠀⠁⠀⠀⡐⡐⡹⡄⠀⠀⠀⠙⢦⣄⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⠏⠘⣴⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣯
⣿⣿⣷⣿⣿⣿⣿⣿⢿⣿⣦⡀⠀⠀⠇⠀⠢⠛⢶⠷⣆⡀⠀⠈⠛⢦⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⢔⡍⢐⣶⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣻⣿⣿⣻
⣿⣿⣿⣿⣹⣿⣿⣷⣾⣿⢿⣷⡄⠈⠠⠀⠀⠀⠀⠀⠀⠉⠳⠿⠶⠶⡿⣷⣤⣀⣀⣀⣀⡀⢀⣀⣀⠀⠤⠔⠒⠉⠁⡠⠳⣥⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿
⣭⣿⣿⣿⣿⣿⡟⣿⣿⣿⣾⣿⣿⣦⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠀⠉⠀⠀⠀⠀⠀⠀⠤⠸⢀⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿
⣿⣿⣿⣿⢿⣿⣷⣿⣿⣿⣿⣽⡟⣿⣷⣬⣀⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠁⢠⣾⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿)";

MainWindow::MainWindow(QWidget *parent)
    : QMainWindow(parent)
    , ui(new Ui::MainWindow)
{
    ui->setupUi(this);
    v = new QRegularExpressionValidator(QRegularExpression("[M0123456789+\\-*\\/\\(\\)^.]*"));
    ui->lineEdit->setValidator(v);
    c = QGuiApplication::clipboard();
    std::vector<std::tuple<QPushButton*, const char*>> to_connect =
            {{ui->buttonNum9, "9"}, {ui->buttonNum8, "8"},{ui->buttonNum7, "7"}, {ui->buttonNum6, "6"},
            {ui->buttonNum5, "5"}, {ui->buttonNum4, "4"},{ui->buttonNum3, "3"}, {ui->buttonNum2, "2"},
            {ui->buttonNum1, "1"}, {ui->buttonNum0, "0"},{ui->buttonRightP, ")"}, {ui->buttonLeftP, "("},
            {ui->buttonMinus, "-"}, {ui->buttonPlus, "+"},{ui->buttonTimes, "*"}, {ui->buttonDiv, "/"},{ui->buttonPow, "^"}};

    for (auto [a, b] : to_connect){
        connect(a, &QPushButton::pressed, this, [this, b](){ MainWindow::updateLabel(b); });
    }
    connect(ui->buttonEquals, &QPushButton::pressed, this, &MainWindow::compute_result);
    connect(this, &MainWindow::pressed, this, &MainWindow::updateLabel);
    connect(this, &MainWindow::pressedEq, this, &MainWindow::compute_result);
    connect(this, &MainWindow::pressedBs, ui->lineEdit, &QLineEdit::backspace);
    connect(ui->buttonCE, &QPushButton::pressed, ui->lineEdit, &QLineEdit::backspace);
    connect(ui->buttonClear, &QPushButton::pressed, ui->lineEdit, &QLineEdit::clear);
    connect(ui->buttonAddMem, &QPushButton::pressed, this, [this](){
                                                                    int i = memory.size();
                                                                    ui->comboBox->insertItem(i, QString::number(i));
                                                                    memory.push_back({0,1});
                                                                    });
    connect(ui->buttonMem, &QPushButton::pressed, this, &MainWindow::addMem);
    connect(ui->buttonSetMem, &QPushButton::pressed, this, [this](){
                                                                    compute_result();
                                                                    if (ui->comboBox->currentIndex() != -1)
                                                                        memory[ui->comboBox->currentIndex()] = result;
                                                                    else{
                                                                        QErrorMessage m;
                                                                        m.showMessage("MEMORY NOT ALLOCATED :/");
                                                                        m.exec();
                                                                    }
                                                                    showResult();
                                                                    });
    connect(ui->ButtonHint, &QPushButton::pressed, this, [this](){
                                                                  QMessageBox m;
                                                                  m.setText(hintText);
                                                                  m.exec();
                                                                  });
}

void MainWindow::addMem(){
    ui->lineEdit->insert(QString("M") + QString::number(ui->comboBox->currentIndex()));
}



void MainWindow::keyPressEvent(QKeyEvent* event){

    if (event->key() == Qt::Key_Equal)
        emit(pressedEq());
    else if (event->key() == Qt::Key_Backspace || event->key() == Qt::Key_Enter)
        emit(pressedBs());
    else if (event->matches(QKeySequence::Copy)){
        QMessageBox m;
        m.setText(sussyShrek);
        m.exec();
        c->setText(ui->lineEdit->text());
    }
    else
        emit(pressed(event->text().toStdString().data()));
}

void MainWindow::updateLabel(const char* s){
    ui->lineEdit->insert(QString(s));
}

void MainWindow::showResult(){
    QString toShow;
    if (!ui->radioButton->isChecked())
        toShow = QString::number(result.value());
    else
        toShow = QString::number(result.nom) + QString{"/"} + QString::number(result.denom);
    ui->lineEdit->setText(toShow);
}

void MainWindow::compute_result(){

    std::stringstream s{ui->lineEdit->text().toStdString()};
    int size = ui->lineEdit->text().size();
    try{
        result = parsePrio1(s);
        showResult();
    } catch (int pos){

        QErrorMessage m(this);
        QString text;
        if (pos < size && pos >= 0){
            ui->lineEdit->setSelection(pos, 1);
            text = QString("A parsing error occured at index") + QString::number(pos) +
                      QString("\n the error has been selected :)");
        } else {
            text = "A parsing error has occured.\n Probably a missing \")\"";
        }
        m.showMessage(text);
        m.exec();
    }

}

bool is_numeric(char c){
    return c >= '0' && c <= '9';
}

std::string_view NUMBERS{"0123456789"};
std::string_view OPERATORS_PRIO1{"+-"};
std::string_view OPERATORS_PRIO2{"*/"};
std::string_view OPERATORS_PRIO3{"^"};


rational MainWindow::parsePar(std::stringstream &ss){
    char par;
    ss >> par;
    rational cur = parsePrio1(ss);
    if (ss.peek() != ')')
        throw (int) ss.tellg();
    ss >> par;
    return cur;
}


rational MainWindow::parsePrio1(std::stringstream &ss){
    rational r = parsePrio2(ss);
    while(OPERATORS_PRIO1.find_first_of(ss.peek()) != std::string::npos){
        char op;
        switch (ss.peek()) {
        case('+'):
            ss >> op;
            r += parsePrio2(ss);
            break;
        case('-'):
            ss >> op;
            r -= parsePrio2(ss);
            break;
        default:
            break;
        }
    }
    if (!ss.eof() && ss.peek() != ')')
        throw (int) ss.tellg();
    return r;
}

rational MainWindow::parsePrio2(std::stringstream &ss){
    rational res = parsePrio3(ss);
    while (OPERATORS_PRIO2.find_first_of(ss.peek()) != std::string::npos){
        char op;
        switch (ss.peek()) {
            case('*'):
                ss >> op;
                res *= parsePrio3(ss);
                break;
            case('/'):
                ss >> op;
                res /= parsePrio3(ss);
                break;
            default:
                break;
        }
    }
    return res;
}

rational MainWindow::parsePrio3(std::stringstream &ss){
    int nom;
    rational r(0,0);
    if (ss.peek() == '('){
        char par;
        ss >> par;
        r = parsePrio1(ss);
        if (ss.peek() != ')')
            throw (int) ss.tellg();
        ss >> par;
    } else if (is_numeric(ss.peek())){
        int nom;
        ss >> nom;
        r = {nom, 1};
    } else if (ss.peek() == 'M'){
        char M;
        ss >> M;
        if (!is_numeric(ss.peek()))
            throw (int) ss.tellg();
        int index;
        ss >> index;
        if (index >= memory.size())
            throw (int) ss.tellg();
        r = memory[index];
    } else
        throw (int) ss.tellg();

    while (OPERATORS_PRIO3.find_first_of(ss.peek()) != std::string::npos){
        char op;
        switch (ss.peek()) {
            case('^'):
                ss >> op;
                r = r.pow(parsePrio3(ss));
                break;
        default:
            break;
        }
    }
//    std::cout << "asd";
    return r;
}


MainWindow::~MainWindow()
{
    delete ui;
}
