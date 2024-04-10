#include <cassert>
#include <iostream>

#include <optional>
#include <vector>
#include <memory>
#include <ranges>
#include <functional>
#include <tuple>

/* Cílem tohoto úkolu je naprogramovat běžná pravidla šachu.
 * Předepsané typy ‹position›, ‹piece_type›, ‹player› ani ‹result›
 * není dovoleno upravovat. */

struct position{
    int file; /* sloupec („písmeno“) – a = 1, b = 2, ... */
    int rank; /* řádek („číslo“) – 1, 2, …, 8 */

    position( int file, int rank ) : file( file ), rank( rank ) {}

    bool operator== ( const position &o ) const = default;
    auto operator<=>( const position &o ) const = default;
};

enum class piece_type{
    pawn, rook, knight, bishop, queen, king
};

enum class player { white, black };

enum class result{
    capture, ok, no_piece, bad_piece, bad_move, blocked, lapsed,
    in_check, would_check, has_moved, bad_promote
};

struct piece{
    player owner;
    piece_type type;
};

using occupant = std::optional< piece >;

#define LEFT_ROOK 1
#define RIGHT_ROOK 2
#define KING 0
#define WHITE 0
#define BLACK 3

class chess{
    using T = result(chess::*)(position, position);
    std::vector<std::unique_ptr<piece>> pieces;
    std::vector<piece*> vec_board;

    player cur_player = player::white;
    std::array<T, 6> funs = std::array{&chess::check_pawn, &chess::check_rook, &chess::check_knight,
                                       &chess::check_bishop, &chess::check_queen, &chess::check_king};
    
    std::array<bool, 6> moved = {{false, false, false, false, false, false}};
    std::array<position, 2> kings = {{{5, 1}, {5, 8}}};

    std::optional<position> castling;
    std::optional<position> en_passant_pos;
    piece* en_passantable = nullptr;

    piece*& get(int x, int y) { return vec_board[x + y * 8]; }
    piece*& get(position pos) { return vec_board[pos.file - 1 + (pos.rank - 1) * 8]; }
    piece* const & get(position pos) const { return vec_board.at(pos.file - 1 + (pos.rank - 1) * 8); }

    void add_pawns(player p, int row){
        for (int i = 0; i < 8; ++i){
            pieces.emplace_back(std::make_unique<piece>());
            pieces.back().get()->owner = p;
            pieces.back().get()->type = piece_type::pawn;

            get(i, row) = pieces.back().get();
        }
    }

    bool in_range(int i){
        return i >= 0 && i < 8;
    }

    bool in_range(position pos){
        return in_range(pos.file - 1) && in_range(pos.rank - 1);
    }
    
    bool piece_in_between(position from, int dx, int dy, int distance){
        auto [x, y] = std::tuple{ from.file, from.rank };
        x+= dx, y+= dy; --distance;
        // dont want to check the last
        for(;in_range({x, y}) && (distance > 0); x += dx, y += dy, distance--){
            if (get({x, y}) != nullptr)
                return true;
        }
        return false;
    }
    void switch_players(){
        cur_player = cur_player == player::white ? player::black : player::white;
    }
    
    result check_pawn(position from, position to){
        auto end_piece = get(to);
        auto [fx, fy] = std::tuple{from.file, from.rank};
        auto [tx, ty] = std::tuple{to.file, to.rank};
        int dx = tx - fx;
        int dy = ty - fy;

        int start_row = (cur_player == player::white) ? 2 : 7;
        int opponent_start_row = (cur_player != player::white) ? 2 : 7;
        int direction = (cur_player == player::white) ? 1 : -1;
        
        int max_dy = (fy == start_row) ? 2 : 1;

        //check for direction 
        if (dy * direction <= 0 || max_dy < dy * direction || std::abs(dx) > 1)
            return result::bad_move;

        //check dx
        if (dy * direction == 2 && std::abs(dx) != 0)
            return result::bad_move;

        //brani mimochodem
        if (auto candidate = get({tx, opponent_start_row + (-2) * direction}); std::abs(dx) > 0 && !end_piece){
            if (!candidate || candidate->owner == cur_player)
                return result::bad_move;

            if (!en_passantable || en_passantable != candidate)
                return result::lapsed;

            en_passant_pos = {tx, opponent_start_row + (-2) * direction};
            return result::ok;
        }
    
        if (dx != 0 && !end_piece)
                return result::bad_move;

        else if (dx != 0 && end_piece->owner == cur_player)
            return result::blocked;

        else if (dx == 0 && (end_piece || get({fx, fy + direction})))
            return result::blocked;

        if (std::abs(dy) == 2)
            en_passantable = get(from);
        
        return result::ok;
    }

    result check_rook(position from, position to){
        auto [fx, fy] = std::tuple{from.file, from.rank};
        auto [tx, ty] = std::tuple{to.file, to.rank};
        int dx = tx - fx;
        int dy = ty - fy;
        int distance = std::abs(dx + dy);

        if ((dx != 0 && dy != 0))
            return result::bad_move;

        if (get(to) && get(to)->owner == cur_player)
            return result::blocked;

        int direction_x = dx == 0 ? 0 : dx / std::abs(dx);
        int direction_y = dy == 0 ? 0 : dy / std::abs(dy);
        return piece_in_between(from, direction_x, direction_y, distance) ?  result::blocked : result::ok ;
    }

    result check_knight(position from, position to){
        auto [fx, fy] = std::tuple{from.file, from.rank};
        auto [tx, ty] = std::tuple{to.file, to.rank};
        int dx = tx - fx;
        int dy = ty - fy;

        if(dx == 0 || dy == 0 || std::abs(dx) + std::abs(dy) != 3 )
            return result::bad_move;
    
        if (get(to) && get(to)->owner == cur_player)
            return result::blocked;

        return result::ok;
    }

    result check_bishop(position from, position to){
        auto [fx, fy] = std::tuple{from.file, from.rank};
        auto [tx, ty] = std::tuple{to.file, to.rank};
        int dx = tx - fx;
        int dy = ty - fy;

        if (std::abs(dx) != std::abs(dy))
            return result::bad_move;
    
        if (get(to) && get(to)->owner == cur_player)
            return result::blocked;

        return piece_in_between(from, dx > 0 ? 1 : -1, dy > 0 ? 1 : -1, std::abs(dy)) ? result::blocked : result::ok;
    }

    result check_queen(position from, position to){
        return check_bishop(from, to) == result::bad_move ? check_rook(from, to) : check_bishop(from, to);
    }

    bool in_check(position from, position to){
        switch_players();
        auto f = get(from);
        auto t = get(to);
        get(from) = nullptr;
        get(to) = f;

        for (int x = 1; x <= 8; ++x){
            for (int y = 1; y <= 8; ++y){
                if (get({x, y}) && playable({x, y}, to) == result::ok){
                    switch_players();
                    get(from) = f;
                    get(to) = t;
                    return true;
                }
            }
        }
        get(from) = f;
        get(to) = t;
        switch_players();
        return false;
    }

    result check_king(position from, position to){
        auto [fx, fy] = std::tuple{from.file, from.rank};
        auto [tx, ty] = std::tuple{to.file, to.rank};
        int dx = tx - fx;
        int dy = ty - fy;
        int start_row = cur_player == player::white ? 1 : 8;

        if (std::abs(dx) == 2 && dy == 0 && fy == start_row && fx == 5){
            // rosada
            int direction = dx > 0 ? 1 : -1;
            int closest_rook = dx < 0 ? LEFT_ROOK : RIGHT_ROOK;
            int color = static_cast<int>(cur_player) * 3;
            int rook_x = dx > 0 ? 8 : 1;

            if (!get({rook_x, start_row}) || get({rook_x, start_row})->type != piece_type::rook)
                return result::bad_move;

            if (piece_in_between(from, direction , 0, std::abs(fx - rook_x)))
                return result::blocked;

            if (moved[color + KING] || moved[color + closest_rook])
                return result::has_moved;

            if (in_check(from, {fx + direction, fy}) || in_check(from, to))
                return result::would_check;

            if (in_check(from, from))
                return result::in_check;
                
            castling = position(rook_x, start_row);
            return result::ok;

        } else if (std::abs(dx) > 1 || std::abs(dy) > 1)
            return result::bad_move;

        if (get(to) && get(to)->owner == cur_player)
            return result::blocked;
        return result::ok;
    }

    result move(position from, position to){
        auto a = result::ok;
        if (get(to) != nullptr)
            a = result::capture;
        get(to) = get(from);
        get(from) = nullptr;
        return a;
    }

    result move_piece(position from, position to){
        auto [fx, fy] = std::tuple{from.file, from.rank};
        auto [tx, ty] = std::tuple{to.file, to.rank};
        int color = cur_player == player::white ? WHITE : BLACK;
        int direction = fy > ty ? -1 : 1;

        if (castling != std::optional<position>{}){
            int closest_rook = castling.value().file;
            int direction = (tx - fx > 0) ? 1 : -1; 
            
            move(castling.value(), {tx - direction, fy});
            
            castling = {};
            moved[color + KING] = true;
            moved[color + closest_rook > 4 ? RIGHT_ROOK : LEFT_ROOK] = true;

        } else if (get(from)->type == piece_type::pawn && en_passantable
                        && get({tx, ty - direction}) == en_passantable ){
            en_passantable = nullptr;
            get(en_passant_pos.value()) = nullptr;
            en_passant_pos = {};
            move(from, to);
            return result::capture;
        }
        if (get(from) != en_passantable){
            en_passantable = nullptr;
            en_passant_pos = {};
        }

        result res = move(from, to);

        auto cur_piece = get(to);
        if (cur_piece->type == piece_type::king){
                if (!moved[color + KING])
                    moved[color + KING] = true;
                kings[static_cast<int>(cur_player)] = to;
        }
        if (cur_piece->type == piece_type::rook){
                int start_row = cur_player == player::white ? 1 : 8;
                if (from == position{1, start_row})
                    moved[color + LEFT_ROOK] = true;
                else if (from == position{8, start_row})
                    moved[color + RIGHT_ROOK] = true;
        }
        return res;
    }

public:

    /* Sestrojí hru ve výchozí startovní pozici. První volání metody
     * ‹play› po sestrojení hry indikuje tah bílého hráče. */

    chess(){
        vec_board.resize(8*8);
        std::array<piece_type, 8> piece_types = {{piece_type::rook, piece_type::knight,
                                             piece_type::bishop, piece_type::queen,
                                             piece_type::king, piece_type::bishop,
                                             piece_type::knight, piece_type::rook}};
        int row = 0;
        int d = 1;
        for (auto p: {player::white, player::black}){
            add_pawns(p, row+d);
            for (auto it = piece_types.begin(); it != piece_types.end(); ++it){
                pieces.emplace_back(std::make_unique<piece>());
                pieces.back().get()->owner = p;
                pieces.back().get()->type = *it;
                get(it - piece_types.begin(), row) = pieces.back().get();
            }
            row += 7;
            d *= -1;
        }
    }
    result playable(position from, position to){
        auto cur_piece = get(from);
        
        if (!cur_piece) return result::no_piece;
        if (cur_piece->owner != cur_player) return result::bad_piece;
        
        if (!in_range(to) || !in_range(from) || (to == from))
            return result::bad_move;

        auto fun = funs[static_cast<int>(cur_piece->type)];
        if (result res = (*this.*fun)(from, to); res != result::ok)
            return res;

        return result::ok;
    }

    result play( position from, position to,
                 piece_type promote = piece_type::pawn ){
        
        if (auto res = playable(from, to); res != result::ok)
            return res;


        position king_pos = kings[static_cast<int>(cur_player)];
        bool was_check = in_check(king_pos, king_pos);

        auto old_board = vec_board;
        auto old_state = std::tuple{castling, en_passantable, en_passant_pos, moved, kings};

        result captured = move_piece(from, to);
        auto new_king_pos = kings[static_cast<int>(cur_player)];

        if (in_check(new_king_pos, new_king_pos)){
            vec_board = std::move(old_board);
            std::tie(castling, en_passantable, en_passant_pos, moved, kings) = old_state;
            return was_check ? result::in_check : result::would_check;
        }
        
        int opponent_rank = cur_player == player::white ? 8 : 1;
        if (to.rank == opponent_rank && get(to)->type == piece_type::pawn){
            if (promote == piece_type::king || promote == piece_type::pawn){
                vec_board = std::move(old_board);
                std::tie(castling, en_passantable, en_passant_pos, moved, kings) = old_state;
                return result::bad_promote;
            }
            get(to)->type = promote;
        }

        //everything is ok now
        switch_players();
        return captured;
    }

    /* Metoda ‹at› vrátí stav zadaného pole. */

    occupant at( position pos) const{
        auto cur = get(pos);
        return cur ? *cur : std::optional<piece>{};
    }
};
int main()
{
    chess c;

    assert( c.play( { 1, 3 }, { 1, 4 } ) == result::no_piece );
    assert( c.play( { 1, 1 }, { 1, 2 } ) == result::blocked );
    assert( c.play( { 1, 7 }, { 1, 6 } ) == result::bad_piece );
    assert( c.play( { 1, 2 }, { 1, 4 } ) == result::ok );
    assert( c.play( { 1, 2 }, { 1, 4 } ) == result::no_piece );

    assert( c.at( { 1, 1 } ) );
    assert( c.at( { 1, 1 } )->type == piece_type::rook );
    assert( c.at( { 1, 1 } )->owner == player::white );
    assert( !c.at( { 1, 3 } ) );

    position a1 = position(1,1);
position a2 = position(1,2);
position a3 = position(1,3);
position a4 = position(1,4);
// position a5 = position(1,5);
position a6 = position(1,6);
position a7 = position(1,7);
// position a8 = position(1,8);
position b1 = position(2,1);
// position b2 = position(2,2);
// position b3 = position(2,3);
// position b4 = position(2,4);
position b5 = position(2,5);
// position b6 = position(2,6);
// position b7 = position(2,7);
position b8 = position(2,8);
position c1 = position(3,1);
// position c2 = position(3,2);
position c3 = position(3,3);
// position c4 = position(3,4);
position c5 = position(3,5);
position c6 = position(3,6);
// position c7 = position(3,7);
position c8 = position(3,8);
position d1 = position(4,1);
position d2 = position(4,2);
position d3 = position(4,3);
position d4 = position(4,4);
position d5 = position(4,5);
position d6 = position(4,6);
position d7 = position(4,7);
position d8 = position(4,8);
position e1 = position(5,1);
position e2 = position(5,2);
position e3 = position(5,3);
position e4 = position(5,4);
position e5 = position(5,5);
position e6 = position(5,6);
position e7 = position(5,7);
position e8 = position(5,8);
position f1 = position(6,1);
position f2 = position(6,2);
position f3 = position(6,3);
position f4 = position(6,4);
position f5 = position(6,5);
position f6 = position(6,6);
position f7 = position(6,7);
position f8 = position(6,8);
position g1 = position(7,1);
position g2 = position(7,2);
position g3 = position(7,3);
position g4 = position(7,4);
position g5 = position(7,5);
position g6 = position(7,6);
position g7 = position(7,7);
position g8 = position(7,8);
// position h1 = position(8,1);
// position h2 = position(8,2);
position h3 = position(8,3);
position h4 = position(8,4);
position h5 = position(8,5);
position h6 = position(8,6);
position h7 = position(8,7);
position h8 = position(8,8);
{
    chess c{};
    
    assert(c.play( a2, a4 ) == result::ok);
    assert(c.play( d7, d5 ) == result::ok);
    assert(c.play( a1, a3 ) == result::ok);
}
{
    chess c{};
    assert(c.play( g2, g3 ) == result::ok);
    assert(c.play( g7, g6 ) == result::ok);
    assert(c.play( f1, h3 ) == result::ok);
    assert(c.play( g8, f6 ) == result::ok);
    assert(c.play( g1, f3 ) == result::ok);
    assert(c.play( f8, g7 ) == result::ok);
    assert(c.play( e1, b1 ) == result::bad_move);
}
{
    chess c{};
    assert(c.play( g2, g3 ) == result::ok);
    assert(c.play( g7, g6 ) == result::ok);
    assert(c.play( f1, h3 ) == result::ok);
    assert(c.play( g8, f6 ) == result::ok);
    assert(c.play( g1, f3 ) == result::ok);
    assert(c.play( f8, g7 ) == result::ok);
    assert(c.play( e1, b1 ) == result::bad_move);
    assert(c.play( e1, c1 ) == result::blocked);
    assert(c.play( e1, g1 ) == result::ok);
    assert(c.play( e8, b8 ) == result::bad_move);
    assert(c.play( e8, c8 ) == result::blocked);
    assert(c.play( e8, c8 ) == result::blocked);
}
{
    chess c{};
    assert( c.play( g2, g4 ) == result::ok);
    assert( c.play( h7, h5 ) == result::ok);
    assert( c.play( g4, h5 ) == result::capture);
    assert( c.play( h8, h5 ) == result::capture);
    assert( c.play( f2, f4 ) == result::ok);
    assert( c.play( g8, f6 ) == result::ok);
    assert( c.play( e1, f2 ) == result::ok);
    assert( c.play( f6, g4 ) == result::ok);
    assert( c.play( e2, e3 ) == result::in_check);
    assert( c.play( f2, f3 ) == result::ok);
    assert( c.play( h5, h4 ) == result::ok);
    assert( c.play( f3, e4 ) == result::ok);
    assert( c.play( g7, g5 ) == result::ok);
    assert( c.play( f4, g5 ) == result::capture);
    assert( c.play( d7, d6 ) == result::ok);
    assert( c.play( g1, f3 ) == result::ok);
    assert( c.play( g4, f2 ) == result::ok);
    assert( c.play( f3, h4 ) == result::in_check);
    assert( c.play( e4, f4 ) == result::in_check);
    assert( c.play( e4, f3 ) == result::blocked);
    assert( c.play( e4, e5 ) == result::in_check);
    assert( c.play( e4, d5 ) == result::ok);
    assert( c.play( h4, h5 ) == result::ok);
    assert( c.play( g5, g6 ) == result::would_check);
    assert( c.play( d5, e4 ) == result::would_check);
    assert( c.play( e2, e4 ) == result::ok);
    assert( c.play( f7, f5 ) == result::ok);
    assert( c.play( g5, f6 ) == result::would_check);
    assert( c.play( g5, g6 ) == result::ok);
}
{
    chess c{};
  assert( c.play( g2, g4 ) == result::ok );
  assert( c.play( h7, h5 ) == result::ok );
  assert( c.play( g4, h5 ) == result::capture );
  assert( c.play( h8, h5 ) == result::capture );
  assert( c.play( f2, f4 ) == result::ok );
  assert( c.play( g8, f6 ) == result::ok );
  assert( c.play( e1, f2 ) == result::ok );
  assert( c.play( f6, g4 ) == result::ok );
  assert( c.play( e2, e3 ) == result::in_check );
  assert( c.play( f2, f3 ) == result::ok );
  assert( c.play( h5, h4 ) == result::ok );
  assert( c.play( f3, e4 ) == result::ok );
  assert( c.play( g7, g5 ) == result::ok );
  assert( c.play( f4, g5 ) == result::capture );
  assert( c.play( d7, d6 ) == result::ok );
  assert( c.play( g1, f3 ) == result::ok );
  assert( c.play( g4, f2 ) == result::ok );
  assert( c.play( f3, h4 ) == result::in_check );
  assert( c.play( e4, f4 ) == result::in_check );
  assert( c.play( e4, f3 ) == result::blocked );
  assert( c.play( e4, e5 ) == result::in_check );
  assert( c.play( e4, d5 ) == result::ok );
  assert( c.play( h4, h5 ) == result::ok );
  assert( c.play( g5, g6 ) == result::would_check );
  assert( c.play( d5, e4 ) == result::would_check );
  assert( c.play( e2, e4 ) == result::ok );
  assert( c.play( f7, f5 ) == result::ok );
  assert( c.play( g5, f6 ) == result::would_check );
  assert( c.play( g5, g6 ) == result::ok );
  assert( c.play( f8, h6 ) == result::ok );
  assert( c.play( f3, h4 ) == result::ok );
  assert( c.play( f5, e4 ) == result::capture );
  assert( c.play( d5, c5 ) == result::in_check );
  assert( c.play( d5, e4 ) == result::in_check );
  assert( c.play( d5, e5 ) == result::in_check );
  assert( c.play( d5, e6 ) == result::in_check );
  assert( c.play( d5, d6 ) == result::in_check );
  assert( c.play( d1, h5 ) == result::capture );
  assert( c.play( a7, a6 ) == result::ok );
  assert( c.play( d5, d4 ) == result::ok );
  assert( c.play( d6, d5 ) == result::ok );
  assert( c.play( f1, b5 ) == result::ok );
  assert( c.play( e7, e5 ) == result::in_check);
}
{
  chess c{};
  assert(c.play( d2, d3 ) == result::ok);
  assert(c.play( d7, d6 ) == result::ok);
  assert(c.play( c1, e3 ) == result::ok);
  assert(c.play( b8, c6 ) == result::ok);
  assert(c.play( b1, c3 ) == result::ok);
  assert(c.play( c8, e6 ) == result::ok);
  assert(c.play( d1, d2 ) == result::ok);
  assert(c.play( d8, d7 ) == result::ok);
  assert(c.play( a1, b1 ) == result::ok);
  assert(c.play( a7, a6 ) == result::ok);
  assert(c.play( e1, c1 ) == result::bad_move);
}
    return 0;
}