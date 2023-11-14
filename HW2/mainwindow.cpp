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
#include <QComboBox>
#include "parser.h"

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
    : QMainWindow(parent), ui(new Ui::MainWindow)
{
    ui->setupUi(this);
    v = new QRegularExpressionValidator(QRegularExpression("[M0123456789+\\-*\\/\\(\\)^.]*"));
    ui->lineEdit->setValidator(v);
    c = QGuiApplication::clipboard();
    std::vector<std::tuple<QPushButton *, const char *>> to_connect =
        {{ui->buttonNum9, "9"}, {ui->buttonNum8, "8"}, {ui->buttonNum7, "7"}, {ui->buttonNum6, "6"}, {ui->buttonNum5, "5"}, {ui->buttonNum4, "4"}, {ui->buttonNum3, "3"}, {ui->buttonNum2, "2"}, {ui->buttonNum1, "1"}, {ui->buttonNum0, "0"}, {ui->buttonRightP, ")"}, {ui->buttonLeftP, "("}, {ui->buttonMinus, "-"}, {ui->buttonPlus, "+"}, {ui->buttonTimes, "*"}, {ui->buttonDiv, "/"}, {ui->buttonPow, "^"}};

    for (auto [a, b] : to_connect)
    {
        connect(a, &QPushButton::pressed, this, [this, b]()
                { MainWindow::updateLabel(b); });
    }
    connect(ui->buttonEquals, &QPushButton::pressed, this, &MainWindow::compute_result);
    connect(this, &MainWindow::pressed, this, &MainWindow::updateLabel);
    connect(this, &MainWindow::pressedEq, this, &MainWindow::compute_result);
    connect(this, &MainWindow::pressedBs, ui->lineEdit, &QLineEdit::backspace);
    connect(ui->buttonCE, &QPushButton::pressed, ui->lineEdit, &QLineEdit::backspace);
    connect(ui->buttonClear, &QPushButton::pressed, ui->lineEdit, &QLineEdit::clear);
    connect(ui->buttonAddMem, &QPushButton::pressed, this, [this]()
            {
                                                                    int i = memory.size();
                                                                    ui->comboBox->insertItem(i, QString::number(i));
                                                                    memory.push_back({0,1});
                                                                    if (memory.size() == 1)
                                                                        ui->memorValue->setText(getNumStr(memory[0]));
    });
    connect(ui->buttonMem, &QPushButton::pressed, this, &MainWindow::addMem);
    connect(ui->buttonSetMem, &QPushButton::pressed, this, [this]()
            {
                                                                    compute_result();
                                                                    if (ui->comboBox->currentIndex() != -1){
                                                                        int i = ui->comboBox->currentIndex();
                                                                        memory[i] = result;
                                                                        ui->memorValue->setText(getNumStr(memory[i]));
                                                                    }else{
                                                                        QErrorMessage m;
                                                                        m.showMessage("MEMORY NOT ALLOCATED :/");
                                                                        m.exec();
                                                                    }
                                                                    showResult();
    });
    connect(ui->ButtonHint, &QPushButton::pressed, this, [this]()
            {
                                                                  QMessageBox m;
                                                                  m.setText(hintText);
                                                                  m.exec(); });
    connect(ui->comboBox, qOverload<int>(&QComboBox::currentIndexChanged), this, [this](int index){
        if (index >= 0 && memory.size() > 0)
            ui->memorValue->setText(getNumStr(memory[index]));
        else
            ui->memorValue->setText({"NONE"});
    });
}

void MainWindow::addMem()
{
    ui->lineEdit->insert(QString("M") + QString::number(ui->comboBox->currentIndex()));
}

void MainWindow::keyPressEvent(QKeyEvent *event)
{
    if (event->key() == Qt::Key_Equal || event->key() == Qt::Key_Return || event->key() == Qt::Key_Enter)
        emit(pressedEq());
    else if (event->key() == Qt::Key_Backspace)
        emit(pressedBs());
    else if (event->matches(QKeySequence::Copy))
    {
        QMessageBox m;
        m.setText(sussyShrek);
        m.exec();
        c->setText(ui->lineEdit->text());
    }
    else
        emit(pressed(event->text().toStdString().data()));
}

void MainWindow::updateLabel(const char *s)
{
    ui->lineEdit->insert(QString(s));
}

QString MainWindow::getNumStr(rational r){
    QString toShow;
    if (!ui->radioButton->isChecked())
        toShow = QString::number(r.value()).setNum(r.value(), 'g', ui->horizontalSlider->sliderPosition()); // QString::number(result.value());
    else
        toShow = QString::number(r.nom) + QString{"/"} + QString::number(r.denom);
    return toShow;
}

void MainWindow::showResult()
{
    ui->lineEdit->setText(getNumStr(result));
}

void MainWindow::compute_result()
{

    std::stringstream s{ui->lineEdit->text().toStdString()};
    int size = ui->lineEdit->text().size();
    try
    {
        result = parsePrio1(s, memory, false);
        showResult();
    }
    catch (int pos)
    {

        QErrorMessage m(this);
        QString text;
        if (pos < size && pos >= 0)
        {
            ui->lineEdit->setSelection(pos, 1);
            text = QString("A parsing error occured at index") + QString::number(pos) +
                   QString("\n the error has been selected :)");
        }
        else
        {
            text = "A parsing error has occured.\n Probably a missing \")\"";
        }
        m.showMessage(text);
        m.exec();
    }
}

MainWindow::~MainWindow()
{
    delete ui;
}
