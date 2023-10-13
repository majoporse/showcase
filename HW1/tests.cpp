#include "hw1.hpp"
#include <iostream>
#include <cassert>

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

int main2()
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

int main() {
    // Test WLabel class
    {
        WLabel label("Hello, World!", "label1");

        // Test the label's name
        assert(label.get_name() == "label1");

        // Test set_label method
        label.set_label("New Message");
        assert(label.get_name() == "label1");
    }

    // Test WButton class
    {
        WButton button("Click Me!", "button1");

        // Test the button's name
        assert(button.get_name() == "button1");
    }

    // Test WGridLayout class
    {
        WGridLayout grid(2, 2, "grid1");

        // Test the grid's name
        assert(grid.get_name() == "grid1");

        // Add some elements to the grid
        WButton *button1 = new WButton("Button 1", "button1");
        WButton *button2 = new WButton("Button 2", "button2");
        grid.add_elem(0, 0, button1);
        grid.add_elem(1, 1, button2);

        // Test find_child method
        assert(grid.find_child("button1") == button1);
        assert(grid.find_child("button2") == button2);
        assert(grid.find_child("button3") == nullptr);
    }

    // Test WTabLayout class
    {
        WTabLayout tabLayout("tabLayout");

        // Test the tabLayout's name
        assert(tabLayout.get_name() == "tabLayout");

        // Add some tabs to the tabLayout
        WButton *tab1 = new WButton("Tab 1", "tab1");
        WButton *tab2 = new WButton("Tab 2", "tab2");
        tabLayout.add_tab(tab1);
        tabLayout.add_tab(tab2);

        // Test find_child method
        assert(tabLayout.find_child("tab1") == tab1);
        assert(tabLayout.find_child("tab2") == tab2);
        assert(tabLayout.find_child("tab3") == nullptr);
    }

    // Test WWidget class
    {
        WWidget widget("parentWidget");
        WButton *childButton = new WButton("Child Button", "childButton");

        // Set the child widget
        widget.set_child(childButton);

        // Test find_child method
        assert(widget.find_child("childButton") == childButton);
        assert(widget.find_child("nonExistentChild") == nullptr);
    }

    std::cout << "All tests passed!" << std::endl;
//     main2();
    return 0;
}
