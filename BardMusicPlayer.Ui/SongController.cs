using BardMusicPlayer.Coffer;
using BardMusicPlayer.Transmogrify.Song;
using BardMusicPlayer.Ui.Classic;
using BardMusicPlayer.Ui.Controls;
using BardMusicPlayer.Ui.Functions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace BardMusicPlayer.Ui
{
    public class SongController : ControllerBase
    {
        [HttpGet]
        public ApiSong Get()
        {
            return ApiSong.Create(PlaybackFunctions.CurrentSong);
        }
        [HttpPatch]
        public void Patch(string id)
        {
            Classic_MainView.Instance.Dispatcher.BeginInvoke(new Action(() => Classic_MainView.Instance.PlaylistCtl.SelectSongById(id)));
        }
        [HttpPut]
        public IActionResult Put([FromQuery] string id)
        {
          lock (Classic_MainView.Instance) {
            string decodedId = WebUtility.UrlDecode(id);
            bool found = false;
            Classic_MainView.Instance.Dispatcher.Invoke(
                new Action(() => {
                  var currentSong = PlaybackFunctions.CurrentSong;
                  Classic_MainView.Instance.PlaylistCtl.AddSongToPlaylistAndQueuee(id, currentSong, out LiteDB.ObjectId objectId);
                  found = objectId != null;
                  if (currentSong == null && objectId != null) {
                    Classic_MainView.Instance.PlaylistCtl.SelectSongById(objectId.ToString());
                  }
                })
            );
            return found ? Ok() : BadRequest();
          }
        }
        [HttpPut]
        [Route("song/load")]
        public IActionResult LoadSong([FromQuery] string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Filename is required.");

            var decodedFilename = WebUtility.UrlDecode(id);

            bool result = PlaybackFunctions.LoadSong(decodedFilename);

            if (!result)
                return StatusCode(500, new Exception("Failed to load song."));

            return Ok($"Song '{decodedFilename}' loaded successfully.");
        }

        [HttpPut]
    
        [Route("song/insert/{playlist}/{idx}")]
        public async Task<IActionResult> InsertSong(string playlist, int idx, [FromQuery] string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
                return BadRequest("Filename is required.");

            var decodedFilename = WebUtility.UrlDecode(filename);

            BmpSong song;
            try
            {
                song = await BmpSong.OpenFile(decodedFilename);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Exception($"Failed to open file '{decodedFilename}': {ex.Message}"));
            }

            var targetPlaylist = BmpCoffer.Instance.GetPlaylist(playlist);
            if (targetPlaylist == null)
                return NotFound();

            try
            {
                BmpCoffer.Instance.SaveSong(song);
                targetPlaylist.Add(idx, song);
                BmpCoffer.Instance.SavePlaylist(targetPlaylist);
                if(playlist == Classic_MainView.Instance.PlaylistCtl.GetCurrentPlaylistName())
                    Classic_MainView.Instance.Dispatcher.BeginInvoke(new Action(() => Classic_MainView.Instance.PlaylistCtl.SetCurrentPlayList(targetPlaylist)));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Exception($"Failed to insert song at index {idx}: {ex.Message}"));
            }

            return Ok($"Song inserted at index {idx} into playlist '{playlist}'.");
        }
        //[HttpPut]
        //public void Put(string id)
        //{
        //    Classic_MainView.Instance.Dispatcher.BeginInvoke(new Action(() => Classic_MainView.Instance.PlaylistCtl.Pl(id)));
        //}
    }
}
