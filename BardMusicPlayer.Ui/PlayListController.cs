using BardMusicPlayer.Coffer;
using BardMusicPlayer.Transmogrify.Song;
using BardMusicPlayer.Ui.Classic;
using BardMusicPlayer.Ui.Controls;
using BardMusicPlayer.Ui.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace BardMusicPlayer.Ui
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayListController : ControllerBase
    {
        [HttpGet]
        public IList<string> Get()
        {
            return BmpCoffer.Instance.GetPlaylistNames();
        }
        [HttpGet("{id}")]
        public ApiSong[] Get(string id)
        {
            return BmpCoffer.Instance.GetPlaylist(id).Select(x => ApiSong.Create(x)).ToArray();
        }
        [HttpPatch("{id}")]
        public IActionResult Patch(string id)
        {
            Classic_MainView.Instance.Dispatcher.BeginInvoke(new Action(() => Classic_MainView.Instance.PlaylistCtl.SelectPlayList("..")));
            Classic_MainView.Instance.Dispatcher.BeginInvoke(new Action(() => Classic_MainView.Instance.PlaylistCtl.SelectPlayList(id)));
            return Ok();
        }

        [HttpPut]
        [Route("shuffle/{id}")]
        public IActionResult Shuffle(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Playlist ID is required.");

            var playlist = BmpCoffer.Instance.GetPlaylist(id);
            if (playlist == null)
                return NotFound();

            var rng = new Random();
            var shuffledSongs = playlist.OrderBy(_ => rng.Next()).ToList(); // Convert to List for shuffling
         
            foreach (var song in shuffledSongs)
            {
                playlist.Remove(song); // Remove songs from the original playlist
            }
            foreach (var song in shuffledSongs)
            {
                playlist.Add(song); // Add shuffled songs back to the playlist
            }

            BmpCoffer.Instance.SavePlaylist(playlist); // Save the shuffled playlist
            if (id == Classic_MainView.Instance.PlaylistCtl.GetCurrentPlaylistName())
                Classic_MainView.Instance.Dispatcher.BeginInvoke(new Action(() => Classic_MainView.Instance.PlaylistCtl.SetCurrentPlayList(playlist)));
   
            return Ok($"Playlist '{id}' was shuffled and replaced.");
        }

        [HttpPost]
        [Route("copy/{src}/{dest}")]
        public IActionResult Copy(string src, string dest)
        {
            if (string.IsNullOrWhiteSpace(src) || string.IsNullOrWhiteSpace(dest))
                return BadRequest("Source and destination playlist IDs are required.");
            var sourcePlaylist = BmpCoffer.Instance.GetPlaylist(src);
            if (sourcePlaylist == null)
                return NotFound();
            var newPlaylist = BmpCoffer.Instance.CreatePlaylist(dest);
            foreach (var song in sourcePlaylist)
            {
                newPlaylist.Add(song);
            }
   
            BmpCoffer.Instance.SavePlaylist(newPlaylist);
            return Ok($"Playlist '{src}' was copied to '{dest}'.");
        }
    }
}
