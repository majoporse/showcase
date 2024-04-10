# Chess Rules

The goal of this task is to implement common rules of chess. The predefined types `position`, `piece_type`, `player`, and `result` must not be modified.


# Method `play` Results

The `play` method can have one of the following outcomes. The options are listed in priority order, meaning the actual outcome is always the first applicable option.

| Outcome        | Description                                    |
|----------------|------------------------------------------------|
| `capture`      | Valid move and captured opponent's piece       |
| `ok`           | Valid move                                     |
| `no_piece`     | No piece at position `from`                    |
| `bad_piece`    | Piece at position `from` does not belong to player |
| `bad_move`     | Move is not valid for this piece               |
| `blocked`      | Move is blocked by another piece               |
| `lapsed`       | En passant capture is no longer possible       |
| `has_moved`    | Some castling piece has already moved          |
| `in_check`     | Player is in check and move wouldn't remove it |
| `would_check`  | Move would expose player to check              |
| `bad_promote`  | Attempted promotion to pawn or king            |

Attempting en passant capture in a situation where the pieces are in the wrong position is considered a `bad_move`. In addition to the `has_moved` result, attempting castling can result in these errors:

- `blocked`: A piece is in the way
- `in_check`: The king is in check
- `would_check`: The king would pass through or end up in check.
