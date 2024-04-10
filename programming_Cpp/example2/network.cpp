#include <cassert>
#include <string>
#include <string_view>
#include <set>
#include <deque>
#include <vector>
#include <map>
#include <memory>
#include <list>
#include <iostream>
#include <charconv>
#include <functional>
#include <algorithm>
#include <ranges>

struct network;
struct router;
using connections = std::vector<std::tuple<std::string, std::string>>;
enum class node_type{ endpoint, bridge, router };
struct node;
struct comparator{

    bool operator()(const node*n1, const node*n2) const;
};

struct node{
    std::size_t max_nodes = 1;
    network *segment;
    std::set<node *, comparator> nodes{{}, comparator{}};
    std::string name;
    node_type type;

    node() = default;
    node(int i, network* n, std::string_view s, node_type t):
            max_nodes(i), segment{n}, name{std::move(s)}, type{t} {} 

    bool full()             const{ return max_nodes <= nodes.size();}
    bool connected(node *n) const{ return  nodes.contains(n);}

    virtual bool can_connect(node *n){
        return n != this && !full() && segment == n->segment && !connected(n);
    }

    virtual bool connect(node* n) {
        if ( !can_connect(n) || !(n->can_connect(this)))
            return false;
        nodes.emplace(n);
        n->nodes.emplace(this);
        return true;
    }

    virtual bool disconnect(node *n){
        if (!connected(n) || !n->connected(this))
            return false;
        nodes.erase(n);
        n->nodes.erase(this);
        return true;
    };

    bool reachable(const node *n) const{
        std::set<const node*> visited;
        std::deque<const node*> q;
        q.emplace_back(this);

        while (!q.empty()){

            const auto to = q.front();
            q.pop_front();

            if (to == n)
                return true;

            visited.insert(to);
            for (auto device: to->nodes){
                if (!visited.contains(device))
                    q.emplace_back(device);
            }
        }
        return false;
    }

    virtual ~node() = default;
};

struct endpoint : public node{
    endpoint(network* n, std::string s): node(1, n, s, node_type::endpoint) {}
};

struct bridge : public node{
    bridge(int i, network* n, std::string s): node(i, n, s, node_type::bridge) {}
};

struct router : public node{
    router(int i, network* n, std::string s): node(i, n, s, node_type::router) {}

    bool can_connect(node *n) override{
        if (full() || n->full())
            return false;
        for (node* node : nodes){
            if (node->segment == n->segment)
                return false;
        }
        return true;
    }
};

bool comparator::operator()(const node*n1, const node*n2) const {
    return n1->name < n2->name;
}

using namespace std::string_literals;

struct network{
    std::vector<std::unique_ptr<node>> devices;
    std::set<router*, comparator> vec_routers;
    std::set<bridge*, comparator> vec_bridges;
    std::set<endpoint*, comparator> vec_endpoints;
    std::map<std::string, node*> map_devices;
    using connections = std::vector<std::tuple<std::string, std::string>>;
    connections _connections;

    node* add_e(std::string name){
        devices.emplace_back(std::make_unique<endpoint>(this, std::move(name)));
        map_devices[name] = devices.back().get();
        vec_endpoints.insert(dynamic_cast<endpoint*>(devices.back().get()));
        return devices.back().get();
    }
    node* add_endpoint(){
        return add_e("e"s + std::to_string(devices.size()));
    }

    node* add_b(int p, std::string name){
        devices.emplace_back(std::make_unique<bridge>(p, this, std::move(name)));
        map_devices[name] = devices.back().get();
        vec_bridges.insert(dynamic_cast<bridge*>(devices.back().get()));
        return devices.back().get();
    }
    node* add_bridge( int p , std::string_view name){
        return add_b(p, "b"s + std::string{name});
    }

    node* add_r(int i, std::string name){
        devices.emplace_back(std::make_unique<router>(i, this, std::move(name)));
        map_devices[name] = devices.back().get();
        vec_routers.insert(dynamic_cast<router*>(devices.back().get()));
        return devices.back().get();
    }
    node* add_router( int i, std::string_view name){
        return add_r(i, "r"s + std::string{name});
    }
    

    std::vector<endpoint*> endpoints() const { 
        std::vector<endpoint*> vec;
        for (auto &n: devices){
            if (n->type == node_type::endpoint)
                vec.push_back(dynamic_cast<endpoint*>(n.get()));
        }
        return vec;
    }
    std::vector<bridge*> bridges() const     {
        std::vector<bridge*> vec;
        for (auto &n: devices){
            if (n->type == node_type::bridge)
                vec.push_back(dynamic_cast<bridge*>(n.get()));
        }
        return vec;
     }
    std::vector<router*> routers() const     {
        std::vector<router*> vec;
        for (auto &n: devices){
            if (n->type == node_type::router)
                vec.push_back(dynamic_cast<router*>(n.get()));
        }
        return vec;
     }


    bool dfs(node *from, node *parent, std::set<node*>&visited, bool ret, std::vector<std::tuple<node*, node*>> &to_remove) const{
        visited.insert(from);

        for (auto *child : from->nodes){
            if (child->segment != from->segment || child == parent)
                continue;
            if (visited.contains(child)){
                if (ret) return true;
                to_remove.emplace_back(from, child);
            }
            else if(dfs(child, from, visited, ret, to_remove))
                return true;
        }
        return false;
    }

    bool has_loops_with_fix(bool fix) const{
        std::set<node*> visited;
        std::vector<std::tuple<node*, node*>> to_remove;
        for (auto &n: devices){
            if (visited.contains(n.get())) 
                continue;
            if (dfs(n.get(), nullptr, visited, !fix, to_remove) && !fix)
                return true;
        }
        if (fix){
            for(auto [from, to]: to_remove)
                from->disconnect(to);
        }
        return false;
    }

    bool has_loops() const{
        return has_loops_with_fix(false);
    }

    void fix_loops() const{
        has_loops_with_fix(true);
    }
    std::tuple<std::string, std::string> get_names(std::string_view &sv, std::string sep){
        auto pos1 = sv.find_first_of(':');
        std::string str1{sv.substr(0, pos1)};
        sv.remove_prefix(pos1 + 1);

        auto pos2 = sv.find_first_of(sep);
        std::string str2{sv.substr(0, pos2)};

        sv.remove_prefix(pos2);
        return {str1, str2};
    }

    void parse_dev_type(std::string_view &sv, std::map<std::string, node*>& net_map, auto adder){
        sv.remove_prefix(1);

        while (sv[0] != ']'){
            if (sv[0] == ',')
                sv.remove_prefix(1);

            auto [ dev_name, max_nodes ] = get_names(sv, ",]");

            int n = stoi(max_nodes);
            auto ptr = adder(n, dev_name);

            net_map[dev_name] = ptr;

            
        }
        sv.remove_prefix(1);
    }

    void parse_devices(std::string_view &sv, std::map<std::string, node*>& net_map){
        parse_dev_type(sv, net_map, [this](int, std::string ){return add_endpoint();} );
        parse_dev_type(sv, net_map, [this](int i, std::string s){return add_b(i, s);} );
        parse_dev_type(sv, net_map, [this](int i, std::string s){return add_r(i, s);} );
    }

    void parse_connections(std::string_view &sv,  std::map<std::string, std::map<std::string, node*>> &net_map){
        sv.remove_prefix(1);

        while (sv[0] != '}'){
            if (sv[0] == '|')
                sv.remove_prefix(1);

            auto [ net, name ] = get_names(sv, ",");
            sv.remove_prefix(1);
            auto [ net_to, name_to ] = get_names(sv, "|}");
            net_map[net][name]->connect(net_map[net_to][name_to]);
        }
        sv.remove_prefix(1);
    }

    ~network(){
        for (auto &d: devices){
            std::vector<node*>to_remove;
            for (auto &dn: d->nodes)
                if (dn->segment != this){
                    to_remove.emplace_back(dn); //cannot iterate through set and change its content
                }
            for (auto *dn: to_remove){
                d->disconnect(dn);
            }
        }
        devices.clear();
    }
};

std::string convert_devices(auto &vec, int& i){
    std::string res;
    std::set<std::string> dev_names;
    for (auto &d: vec){
        std::string d_name = d->type == node_type::endpoint ?  "e"s + std::to_string(i++) : d->name;
        dev_names.emplace(d_name + ":" + std::to_string(d->max_nodes));
    }
    res += "[";
    for (auto &d: dev_names)
        res += std::move(d) + ',';
    if (!vec.empty())
        res.erase(res.end()-1);
    res += "]";
    return res;
}

std::string get_connections(auto &vec, std::string name, std::map<const network*, std::string> net_map, int& i){
    std::string res;
    std::set<std::string> cons;
    for (auto &d: vec){
        for (auto &n: d->nodes){
            if (net_map.contains(n->segment)){
                std::string d_name = d->type == node_type::endpoint ?  "e"s + std::to_string(i++) : d->name;
                std::string n_name = n->type == node_type::endpoint ?  "e"s + std::to_string(i++)  : n->name;
                cons.emplace( name + ":" += d_name + "," + net_map[n->segment] + ":" += n_name + "|");
            }
        }
    }
    for (auto &c: cons)
        res += std::move(c);
    return res;
}

std::string get_endpoints(auto &vec, std::string name, int& i){
    std::set<node *>used_ends;
    std::string res;
    for (auto &d: vec){
        for (auto &n: d->nodes){
            if (n->type == node_type::endpoint && !used_ends.contains(n)){
                used_ends.emplace(n);
                used_ends.emplace(d);
                std::string d_name = "e"s + std::to_string(i++);
                std::string n_name = "e"s + std::to_string(i++);
                res += name + ":" += d_name + "," += name + ":" += n_name + "|";
            }
        }
    }
    return res;
}

std::string serialize( const std::list< network > &l ){
    std::string res;
    std::map<const network*, std::string> net_map;
    for(auto [it, i] = std::tuple{l.begin(), 0}; it != l.end(); ++it, ++i){
        std::string name = "n"s + std::to_string(i);
        net_map[&(*it)] = name;
        res += name;
        int ends1 = 0;
        res += convert_devices(it->vec_endpoints, ends1);
        res += convert_devices(it->vec_bridges, ends1);
        res += convert_devices(it->vec_routers, ends1);
        res += "{";
        
        int ends = 0;
        res += get_endpoints(it->vec_endpoints, name, ends);
        res += get_connections(it->vec_bridges, name, net_map, ends);
        res += get_connections(it->vec_routers, name, net_map, ends);
        
        if (!it->devices.empty())
            res.erase(res.end()-1);
        res += "}\n";

    }
    return res;
}

std::list< network > deserialize( std::string_view sv){
    std::list< network > res;
    std::map<std::string, std::map<std::string, node*>> net_map;

    while (!sv.empty()){
        network &n = res.emplace_back();
        auto pos = sv.find_first_of('[');
        std::string net_name{sv.substr(0, pos)};
        sv.remove_prefix(pos);
        n.parse_devices(sv, net_map[net_name]);
        n.parse_connections(sv, net_map);
        sv.remove_prefix(1);
    }

    return res;
}

int main(){
    std::list< network > sys_1;

    auto &n = sys_1.emplace_back();
    auto &m = sys_1.emplace_back();

    auto e1 = n.add_endpoint(),
         e2 = n.add_endpoint(),
         e3 = m.add_endpoint();
    auto b = n.add_bridge( 2, "ufo" );
    auto r1 = n.add_router( 2, "r1" );
    auto r2 = m.add_router( 2, "r2" );

    assert( n.bridges().size() == 1 );
    assert( n.routers().size() == 1 );
    assert( n.endpoints().size() == 2 );

    assert( b->connect( e1 ) );
    assert( b->connect( r1 ) );
    assert( r1->connect( r2 ) );
    assert( r2->connect( e3 ) );

    assert( e1->reachable( e3 ) );
    assert( !e1->reachable( e2 ) );

    std::string str = serialize( sys_1 );
    std::cout << str;
    std::list< network > sys_2 = deserialize( str );
    std::string str2 = serialize( sys_2 );
    std::cout << str2;
    assert( sys_2.size() == 2 );
    assert( serialize( sys_2 ) == str );

    const network &nn = sys_2.front(),
                  &mm = sys_2.back();

    auto nn_e = nn.endpoints();
    auto mm_e = mm.endpoints();

    assert( nn_e.size() == 2 );
    assert( mm_e.size() == 1 );

    assert( nn_e.front()->reachable( mm_e.front() ) ||
            nn_e.back()->reachable( mm_e.front() ) );
{
        std::list< network > sys_1;

        auto &n1 = sys_1.emplace_back();
        auto &m1 = sys_1.emplace_back();

        auto e11 = n1.add_endpoint();
        n1.add_endpoint();
        auto e31 = m1.add_endpoint();
        auto b1 = n1.add_bridge( 2, "ufo" );
        auto r11 = n1.add_router( 2, "r1" );
        auto r21 = m1.add_router( 2, "r2" );
        assert( b1->connect( e11 ) );
        assert( b1->connect( r11 ) );
        assert( r11->connect( r21 ) );
        assert( r21->connect( e31 ) );

        std::list< network > sys_2;

        auto &n2 = sys_2.emplace_back();
        auto &m2 = sys_2.emplace_back();

        n2.add_endpoint();
        auto r22 = m2.add_router( 2, "r2" );
        auto e12 = n2.add_endpoint();
        auto b2 = n2.add_bridge( 2, "ufo" );
        auto e32 = m2.add_endpoint();
        auto r12 = n2.add_router( 2, "r1" );
        assert( r12->connect( r22 ) );
        assert( b2->connect( r12 ) );
        assert( b2->connect( e12 ) );
        assert( r22->connect( e32 ) );

        std::list< network > sys_3;

        auto &n3 = sys_3.emplace_back();
        auto &m3 = sys_3.emplace_back();

        auto e13 = n3.add_endpoint();
        n3.add_endpoint();
        auto r13 = n3.add_router( 2, "r1" );
        auto r23 = m3.add_router( 2, "r2" );
        auto b3 = n3.add_bridge( 2, "ufo" );
        auto e33 = m3.add_endpoint();
        assert( r23->connect( e33 ) );
        assert( b3->connect( r13 ) );
        assert( r13->connect( r23 ) );
        assert( b3->connect( e13 ) );


        std::string str1 = serialize( sys_1 );
        std::string str2 = serialize( sys_2 );
        std::string str3 = serialize( sys_3 );
        std::cout << '\n' <<str1;
        std::cout << str2;
        std::cout << str3;
        assert(str1 == str2);
        assert(str1 == str3);
    }
    {
        std::list< network > sys_3;

        auto &n2 = sys_3.emplace_back();

        auto a = n2.add_endpoint();
        auto b = n2.add_endpoint();
        a->connect(b);
        std::cout << serialize(sys_3);
    }
    {
        std::list<network> l;
        auto &n = l.emplace_back();
        auto e1 = n.add_endpoint();
        auto e2 = n.add_endpoint();
        auto b = n.add_bridge(2, "sdf");
        assert(b->connect(e1));
        assert(b->connect(e2));
        assert(!b->connect(e2));
        assert(!b->connect(e1));
        
        std::cout << serialize(l);
    }
    {
        std::list<network> l;
        auto &n = l.emplace_back();
        auto e1 = n.add_endpoint();
        auto e2 = n.add_endpoint();
        auto b = n.add_bridge(2, "sdf");
        n.add_bridge(2, "1sdf");
        assert(b->connect(e1));
        assert(b->connect(e2));
        assert(b->disconnect(e2));
        assert(!b->disconnect(e2));
        assert(b->connect(e2));
        
        std::cout << serialize(l);
    }
    {
        std::list<network> l;
        auto &n = l.emplace_back();
        auto e1 = n.add_endpoint();
        auto e2 = n.add_endpoint();
        n.add_bridge(2, "1sdf");
        auto b = n.add_bridge(2, "sdf");
        assert(b->connect(e1));
        assert(b->connect(e2));
        assert(b->disconnect(e2));
        assert(!b->disconnect(e2));
        assert(b->connect(e2));
        
        std::cout << serialize(l);
    }
    {
        std::list<network> l;
        auto &n = l.emplace_back();
        auto e1 = n.add_endpoint();
        auto e2 = n.add_endpoint();
        auto b = n.add_bridge(2, "sdf");
        n.add_router(2, "1");
        n.add_router(2, "1sdf");
        assert(b->connect(e1));
        assert(b->connect(e2));
        assert(b->disconnect(e2));
        assert(!b->disconnect(e2));
        assert(b->connect(e2));
        
        std::cout << serialize(l);
    }
    {
        std::list<network> l;
        auto &n = l.emplace_back();
        auto e1 = n.add_endpoint();
        auto e2 = n.add_endpoint();
        n.add_router(2, "1sdf");
        n.add_router(2, "1");
        auto b = n.add_bridge(2, "sdf");
        assert(b->connect(e1));
        assert(b->connect(e2));
        assert(b->disconnect(e2));
        assert(!b->disconnect(e2));
        assert(b->connect(e2));
        
        std::cout << serialize(l);
    }
    return 0;
}