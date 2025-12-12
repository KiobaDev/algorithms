def merge_dicts_max(initial_dictionary: dict[str, int], dictionary_to_merge: dict[str, int]) -> dict[str, int]:
    """
    Merges two dictionaries and takes the maximum value in case of key conflicts
    """
    
    result = initial_dictionary.copy()
    
    for key, value in dictionary_to_merge.items():
        if key in result:
            result[key] = max(result[key], value)
        else:
            result[key] = value
    
    return result


# Example usage
initial_dictionary = {"a": 10, "b": 20, "c": 30}
dictionary_to_merge = {"b": 25, "c": 15, "d": 40}

merged = merge_dicts_max(initial_dictionary, dictionary_to_merge)

print(f"Dictionary 1: {initial_dictionary}")
print(f"Dictionary 2: {dictionary_to_merge}")

print("----------------------------------------------------------------")

print(f"Merged (max values): {merged}")

