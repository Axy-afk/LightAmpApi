using BardMusicPlayer.Coffer;
using BardMusicPlayer.Transmogrify.Song;
using BardMusicPlayer.Ui.Classic;
using BardMusicPlayer.Ui.Functions;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace BardMusicPlayer.Ui
{
    public class SongController : ApiController
    {
        [HttpGet]
        public ApiSong Get()
        {
            return ApiSong.Create(PlaybackFunctions.CurrentSong);
        }
        [HttpPatch]
        public void Patch(string id)
        {
            Classic_MainView.Instance.Dispatcher.BeginInvoke(new Action(() => Classic_MainView.Instance.PlaylistCtl.SelectSong(id)));
        }
        [HttpPut]
        public void Put(string id)
        {
            string decodedId = WebUtility.UrlDecode(id);
            Classic_MainView.Instance.Dispatcher.BeginInvoke(
                new Action(() => Classic_MainView.Instance.SongBrowser.OnAddSongFromBrowser?.Invoke(Classic_MainView.Instance.SongBrowser, decodedId))
            );
        }
        [HttpPut]
        [Route("song/load")]
        public IHttpActionResult LoadSong([FromUri] string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Filename is required.");

            var decodedFilename = WebUtility.UrlDecode(id);

            bool result = PlaybackFunctions.LoadSong(decodedFilename);

            if (!result)
                return InternalServerError(new Exception("Failed to load song."));

            return Ok($"Song '{decodedFilename}' loaded successfully.");
        }

        [HttpPut]
    
        [Route("song/insert/{playlist}/{idx}")]
        public async Task<IHttpActionResult> InsertSong(string playlist, int idx, [FromUri] string filename)
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
                return InternalServerError(new Exception($"Failed to open file '{decodedFilename}': {ex.Message}"));
            }

            var targetPlaylist = BmpCoffer.Instance.GetPlaylist(playlist);
            if (targetPlaylist == null)
                return NotFound();

            try
            {
                BmpCoffer.Instance.SaveSong(song);
                targetPlaylist.Add(idx, song);
                BmpCoffer.Instance.SavePlaylist(targetPlaylist);
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"Failed to insert song at index {idx}: {ex.Message}"));
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
