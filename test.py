# song_tracker.py

import requests

def check_current_song_index():
    playback_status = requests.get("http://localhost:9000/playback").json()

    # Only fetch current song if playback is active
    if playback_status.get("state") == "PLAYBACK_STATE_PLAYING":
        song_data = requests.get("http://localhost:9000/song").json()
        song_title = song_data.get("Title")

        if not song_title:
            print("No song title available.")
            return "Error"

        playlist = requests.get("http://localhost:9000/playlist/test").json()
        idx = next((i for i, entry in enumerate(playlist) if entry.get("Title") == song_title), None)

        if idx is not None:
            print(f"Found '{song_title}' at index {idx}.")
        else:
            print(f"'{song_title}' not found in playlist.")
    else:
        print("Playback is not active.")

# Example usage
if __name__ == "__main__":
    check_current_song_index()




