#include "hw1.hpp"
#include <iostream>

WWidget *build_widget(const std::string &widget_name)
{
    WWidget *w = new WWidget(widget_name);
    WGridLayout *layout = new WGridLayout(2, 2, widget_name + "Layout");
    WLabel *left_label = new WLabel("Left", widget_name + "ul");
    WLabel *right_label = new WLabel("Right", widget_name + "ur");
    WButton *ok_button = new WButton("ok", widget_name + "dl");
    WButton *dec_button = new WButton("decline", widget_name + "dr");
    layout->add_elem(0, 0, left_label);
    layout->add_elem(0, 1, right_label);
    layout->add_elem(1, 0, ok_button);
    layout->add_elem(1, 1, dec_button);
    w->set_child(layout);
    return w;
}

int main()
{
    using std::cout;

    WGridLayout b(3, 3, "grid1");
    WLabel *label = new WLabel("Ahoj svete", "label1");
    WButton *button = new WButton("Click Me!", "button1");
    WGridLayout *b2 = new WGridLayout(1, 1, "grid2");
    WButton *button2 = new WButton("Don't click", "button2");
    WTabLayout *tab1 = new WTabLayout("tab1");

    tab1->add_tab(build_widget("widget_tab1"));
    tab1->add_tab(build_widget("widget_tab2"));
    b2->add_elem(0, 0, button2);
    b.add_elem(1, 0, b2);
    b.add_elem(1, 1, button);
    b.add_elem(0, 1, build_widget("widget1"));
    b.add_elem(2, 0, tab1);

    b.add_elem(0, 0, label);
    b.add_elem(2, 1, build_widget("widget2"));

    cout << "===== Print base output =====" << std::endl;
    cout << b << std::endl
         << std::endl;

    WObject *ptr = b.find_child("widget_tab1");

    cout << "===== Print subelement =====" << std::endl;
    cout << *ptr << std::endl
         << std::endl;
    WLabel *tab = dynamic_cast<WLabel *>(ptr->find_child("widget_tab1ul"));
    cout << tab << std::endl;
    tab->set_label("Empty");

    cout << "===== Show local update =====" << std::endl;
    cout << *tab << std::endl
         << std::endl;

    cout << "===== Show update on parent =====" << std::endl;
    cout << *ptr << std::endl
         << std::endl;

    cout << "===== Show update globally ======" << std::endl;
    cout << b;

    cout << "Ending simulation" << std::endl;

    return 0;
}