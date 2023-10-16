#include "hw1.hpp"

void WObject::change_parent(WObject *new_parent)
{
     parent = new_parent;
}

void WLabel::set_label(std::string new_message) { message = std::move(new_message); }
WObject *WLabel::find_child(const std::string &s_name) { return s_name == name ? this : nullptr; }
std::ostream &WLabel::print(std::ostream &o, std::string prefix) const
{
     return o << prefix << "[Label:" << name << "] Displaying: "
              << "\"" << message << "\", parent: " << (parent ? parent->get_name() : "None") << "\n";
}

WObject *WButton::find_child(const std::string &s_name) { return s_name == name ? this : nullptr; }
std::ostream &WButton::print(std::ostream &o, std::string prefix) const
{
     return o << prefix << "[Button:" << name << "] |" << message << "| parent: " << (parent ? parent->get_name() : "None") << "\n";
}

WObject *WGridLayout::find_child(const std::string &s_name)
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

void WGridLayout::add_elem(double x, double y, WObject *new_object)
{
     auto pred = [&](auto a)
     {
          return a.first == std::tuple{x, y} || a.second->get_name() == new_object->get_name();
     };

     if (std::ranges::find_if(elems, pred) != elems.end())
          return;

     new_object->change_parent(this);
     elems.emplace(std::tuple{x, y}, new_object);
}

std::ostream &WGridLayout::print(std::ostream &o, std::string prefix) const
{
     o << prefix << "[GridLayout:" << name << "] parent: " << (parent ? parent->get_name() : "None") << "\n";
     for (auto [it, i] = std::tuple{elems.begin(), 0}; i + 1 < elems.size(); ++i, ++it)
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

WGridLayout::~WGridLayout()
{
     for (auto [a, b] : elems)
          delete b;
}

WObject *WTabLayout::find_child(const std::string &s_name)
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

std::ostream &WTabLayout::print(std::ostream &o, std::string prefix) const
{
     o << prefix << "[Tabs:" << name << "] parent: " << (parent ? parent->get_name() : "None") << " tabs: " << tabs.size() << "\n";
     for (auto [it, i] = std::tuple{tabs.begin(), 0}; i + 1 < tabs.size(); ++i, ++it)
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

void WTabLayout::add_tab(WObject *new_object)
{
     if (std::ranges::find(tabs, new_object->get_name(),
                           [](WObject *a)
                           { return a->get_name(); }) != tabs.end())
          return;
     tabs.push_back(new_object);
     new_object->change_parent(this);
}

WTabLayout::~WTabLayout()
{
     for (auto a : tabs)
          delete a;
}

void WWidget::set_child(WObject *new_child)
{
     delete child;
     child = new_child;
     child->change_parent(this);
}

WObject *WWidget::find_child(const std::string &s_name) { return s_name == name ? this : child ? child->find_child(std::move(s_name))
                                                                                          : nullptr; }

std::ostream &WWidget::print(std::ostream &o, std::string prefix) const
{
     o << prefix << "[Widget:" << name << "] "
        << "parent: " << (parent ? parent->get_name() : "None") << "|" << (child ? child->get_name() : "None") << "\n";

     return child ? (child->print(o, prefix + "   ")) : o;
}

WWidget::~WWidget() { delete child; }
