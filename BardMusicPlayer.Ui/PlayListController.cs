using BardMusicPlayer.Coffer;
using BardMusicPlayer.Transmogrify.Song;
using BardMusicPlayer.Ui.Classic;
using BardMusicPlayer.Ui.Controls;
using BardMusicPlayer.Ui.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace BardMusicPlayer.Ui
{
    public class PlayListController : ApiController
    {
        [HttpGet]
        public IList<string> Get()
        {
            return BmpCoffer.Instance.GetPlaylistNames();
        }
        [HttpGet]
        public ApiSong[] Get(string id)
        {
            return BmpCoffer.Instance.GetPlaylist(id).Select(x => ApiSong.Create(x)).ToArray();
        }
        [HttpPatch]
        public IHttpActionResult Patch(string id)
        {
            Classic_MainView.Instance.Dispatcher.BeginInvoke(new Action(() => Classic_MainView.Instance.PlaylistCtl.SelectPlayList(id)));
            return Ok();
        }

        [HttpPut]
        [Route("playlist/shuffle/{id}")]
        public IHttpActionResult Shuffle(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Playlist ID is required.");

            var playlist = BmpCoffer.Instance.GetPlaylist(id);
            if (playlist == null)
                return NotFound();

            var rng = new Random();
            var shuffledSongs = playlist.OrderBy(_ => rng.Next()).ToList(); // Convert to List for shuffling
            //var shuffledPlaylist = BmpCoffer.Instance.CreatePlaylist(id); // Create a new playlist with the same ID
            foreach (var song in shuffledSongs)
            {
                playlist.Remove(song); // Add shuffled songs to the new playlist
            }
            foreach (var song in shuffledSongs)
            {
                playlist.Add(song); // Add shuffled songs to the new playlist
            }

            BmpCoffer.Instance.SavePlaylist(playlist); // Save the new playlist

            return Ok($"Playlist '{id}' was shuffled and replaced.");
        }
    }
}
