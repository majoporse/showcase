Sure, here's the README.md content with the comments extracted from the code:


# Simplified JSON Parser

This program implements a syntactic analyzer for a simplified version of JSON. In this version, we do not consider "quoted" strings; instead, scalars are only numbers, and keys are words without quotes (thus, they cannot contain spaces). The entire document consists of objects (key-value mappings in curly braces), arrays (lists of values in square brackets), and integers.

## EBNF Grammar:

(* toplevel elements *)
value   = blank, ( integer | array | object ), blank ;
integer = [ '-' ], digits  | '0' ;
array   = '[', valist, ']' | '[]' ;
object  = '{', kvlist, '}' | '{}' ;

(* compound data *)
valist  = value,  { ',', value } ;
kvlist  = kvpair, { ',', kvpair } ;
kvpair  = blank, key, blank, ':', value ;

(* lexemes *)
digits  = nonzero, { digit } ;
nonzero = '1' | '2' | '3' | '4' | '5' | '6' | '7' | '8' | '9' ;
digit   = '0' | nonzero ;
key     = keychar, { keychar } ;
keychar = ? ASCII upper- or lower-case alphabetical character ? ;
blank   = { ? ASCII space, tab, or newline character ? } ;


## Interface:

The `json_value` struct represents the parsed tree structure of the document. The following requirements are placed on it:

- The `item_at` method must throw a `std::out_of_range` exception if the requested element does not exist.
- For objects of type `json_type::object`, when given an index ⟦n⟧ to the `item_at` method, it returns the ⟦n⟧-th value in alphabetical order of keys. The corresponding key can be obtained using the `key_at` method.
- The `length` method does not fail (returns zero for integer nodes).

The `json_parse` function analyzes the document and creates the corresponding tree structure or throws a `json_error` exception in case of:

- Violation of the grammar rules.
- Presence of a duplicated key in any object.

The `json_validate` function determines whether the input document is correctly formed according to the specified grammar. This function must not throw exceptions except for `std::bad_alloc` in case of memory exhaustion.

## Usage:

The `main` function contains several assertions to demonstrate the usage of the parser and validator functions. You can use these as examples to understand how to parse and validate JSON documents using this program.