import requests
import random

API_URL = "http://localhost:5093/api/quotes"  # api route
def load_quotes_from_file(filename):
    """Load quotes from a text file and add them to the API."""
    try:
        with open(filename, "r", encoding="utf-8") as file:
            for line in file:
                text = line.strip()
                if text:
                    response = requests.post(API_URL, json={"text": text})
                    if response.status_code == 201:
                        print(f"Added: {text}")
                    else:
                        print(f"Failed to add: {text}")
    except FileNotFoundError:
        print("File not found! Please provide a valid file.")

def add_quote():
    """Manually add a quote."""
    text = input("Enter the quote text: ").strip()
    author = input("Enter the author (or press enter if unknown): ").strip()
    
    if not text:
        print("Quote text cannot be empty!")
        return
    
    response = requests.post(API_URL, json={"text": text, "author": author})
    if response.status_code == 201:
        print("Quote added successfully!")
    else:
        print("Failed to add the quote.")

def get_random_quote():
    """Fetch and display a random quote."""
    response = requests.get(API_URL)
    if response.status_code == 200:
        quotes = response.json()
        if quotes:
            quote = random.choice(quotes)
            print(f'"{quote["text"]}" - {quote.get("author", "Unknown")}')
        else:
            print("No quotes available.")
    else:
        print("Failed to fetch quotes.")

def main():
    """Main menu for the client."""
    while True:
        print("\n1. Load quotes from file")
        print("2. Add a new quote")
        print("3. Get a random quote")
        print("4. Exit")

        choice = input("Choose an option: ").strip()
        
        if choice == "1":
            filename = input("Enter the filename(add path): ").strip()
            load_quotes_from_file(filename)
        elif choice == "2":
            add_quote()
        elif choice == "3":
            get_random_quote()
        elif choice == "4":
            print("Exiting...")
            break
        else:
            print("Invalid choice. Please select a valid option.")

if __name__ == "__main__":
    main()

