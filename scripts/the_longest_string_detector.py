def find_longest_string(words: list) -> str:
    """
    Finds the longest string in a list using the optimized max() function
    """
 
    return max(words, key=len, default="")


# Example usage
cities = [
    "Paris",
    "Tokyo",
    "Dubai",
    "Cairo",
    "New York",
    "Rio de Janeiro",
    "Wellington",
    "Kuala Lumpur"
]

longest = find_longest_string(cities)


print(f"Cities: {cities}")

print("----------------------------------------------------------------")

print(f"The longest string is: '{longest}'\n")