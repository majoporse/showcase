#include <ranges>
#include <tuple>
#include <map>
#include <vector>
#include <string>
#include <sstream>
#include <utility>
#include <iostream>

struct WObject
{
    WObject *parent;

    virtual WObject *find_child(std::string &&) = 0;
    std::string get_name() const { return name; }
    explicit WObject(std::string name, WObject *parent) : name{name}, parent{parent} {}
    virtual std::ostream &print(std::ostream &, std::string prefix) const = 0;
    friend std::ostream &operator<<(std::ostream &o, const WObject &w) { return w.print(o, ""); }
    virtual ~WObject() = default;

protected:
    std::string name;
};

struct WLabel : public WObject
{

    explicit WLabel(const std::string &message, const std::string &name, WObject *parent = nullptr) : WObject(name, parent), message{message} {}
    void set_label(std::string new_message) { message = std::move(new_message); }
    WObject *find_child(std::string &&s_name) override { return s_name == name ? this : nullptr; }
    std::ostream &print(std::ostream &o, std::string prefix) const override
    {
        return o << prefix << "[Label:" << name << "] Displaying: "
                 << "\"" << message << "\", parent: " << (parent ? parent->get_name() : "None") << "\n";
    }

private:
    std::string message;
};

struct WButton : public WObject
{
    explicit WButton(const std::string &message, const std::string &name, WObject *parent = nullptr) : WObject(name, parent), message{message} {}
    WObject *find_child(std::string &&s_name) override { return s_name == name ? this : nullptr; }
    std::ostream &print(std::ostream &o, std::string prefix) const override
    {
        return o << prefix << "[Button:" << name << "] |" << message << "| parent: " << (parent ? parent->get_name() : "None") << "\n";
    }

private:
    WObject *parent;
    std::string message;
};

struct WGridLayout : public WObject
{
    WGridLayout(int rows, int cols, std::string name, WObject *parent = nullptr) : WObject(name, parent), rows{rows}, cols{cols} {}

    WObject *find_child(std::string &&s_name) override
    {
        for (auto [a, ptr] : elems)
        {
            if (ptr->get_name() == s_name)
                return ptr;
            if (auto found = ptr->find_child(std::move(s_name)); found != nullptr)
                return found;
        }
        return nullptr;
    }

    void add_elem(double x, double y, WObject *new_object)
    {
        auto pred = [&](auto a)
        {
            return a.first == std::tuple{x, y} || a.second->get_name() == new_object->get_name();
        };

        if (std::ranges::find_if(elems, pred) != elems.end())
            return;

        elems.emplace(std::tuple{x, y}, new_object);
    }

    std::ostream &print(std::ostream &o, std::string prefix) const override
    {
        o << prefix << "[GridLayout:" << name << "] parent: " << (parent ? parent->get_name() : "None") << "\n";
        for (auto [it, i] = std::tuple{elems.begin(), 0}; i < elems.size() - 1; ++i, ++it)
        {
            o << prefix << "|-- [Row:" << std::get<0>(it->first) << "]";
            o << "[Col:" << std::get<1>(it->first) << "]\n";
            it->second->print(o, prefix + "|  ");
        }

        if (!elems.empty())
        {
            o << prefix << "\\-- [Row:" << std::get<0>(elems.rbegin()->first) << "]";
            o << "[Col:" << std::get<1>(elems.rbegin()->first) << "]\n";
            elems.rbegin()->second->print(o, prefix + "    ");
        }
        return o;
    }

    ~WGridLayout()
    {
        for (auto [a, b] : elems)
            delete b;
    }

private:
    std::map<std::tuple<double, double>, WObject *> elems;
    int rows;
    int cols;
};

struct WTabLayout : public WObject
{
    explicit WTabLayout(std::string name, WObject *parent = nullptr) : WObject(name, parent) {}

    WObject *find_child(std::string &&s_name) override
    {
        for (auto a : tabs)
        {
            if (a->get_name() == s_name)
                return a;
            if (auto found = a->find_child(std::move(s_name)); found != nullptr)
                return found;
        }
        return nullptr;
    }

    std::ostream &print(std::ostream &o, std::string prefix) const override
    {
        o << prefix << "[Tabs:" << name << "] parent: " << (parent ? parent->get_name() : "None") << " tabs: " << tabs.size() << "\n";
        for (auto [it, i] = std::tuple{tabs.begin(), 0}; i < tabs.size() - 1; ++i, ++it)
        {
            o << prefix << "|-- [Tab:" << i << "]\n";
            (*it)->print(o, prefix + "|   ");
        }

        if (!tabs.empty())
        {
            o << prefix << "\\-- [Tab:" << tabs.size() - 1 << "]\n";
            (*tabs.rbegin())->print(o, prefix + "    ");
        }

        return o;
    }

    void add_tab(WObject *new_object)
    {
        if (std::ranges::find(tabs, new_object->get_name(),
                              [](WObject *a)
                              { return a->get_name(); }) != tabs.end())
            return;
        tabs.push_back(new_object);
        new_object->parent = this;
    }

    ~WTabLayout()
    {
        for (auto a : tabs)
            delete a;
    }

private:
    std::vector<WObject *> tabs;
    WObject *parent;
};

struct WWidget : public WObject
{
    explicit WWidget(std::string name, WObject *child = nullptr, WObject *parent = nullptr) : WObject(name, parent), child{child}
    {
        if (child)
        {
            child->parent = this;
        }
    }

    void set_child(WObject *new_child)
    {
        delete child;
        child = new_child;
        child->parent = this;
    }

    WObject *find_child(std::string &&s_name) override { return s_name == name ? this : child ? child->find_child(std::move(s_name)) : nullptr; }

    std::ostream &print(std::ostream &o, std::string prefix) const override
    {
        o << prefix << "[Widget:" << name << "] "
          << " parent: " << (parent ? parent->get_name() : "None") << "|" << (child ? child->get_name() : "None") << "\n";

        return child ? (child->print(o, prefix + "   ")) : o;
    }

    ~WWidget() { delete child; }

private:
    WObject *child;
    WObject *parent;
};


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