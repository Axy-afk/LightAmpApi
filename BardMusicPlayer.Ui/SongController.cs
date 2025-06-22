using BardMusicPlayer.Ui.Classic;
using BardMusicPlayer.Ui.Functions;
using System;
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
            Classic_MainView.Instance.Dispatcher.BeginInvoke(new Action(() => Classic_MainView.Instance.PlaylistCtl.AddSongToPlaylist(id)));
        }
        //[HttpPut]
        //public void Put(string id)
        //{
        //    Classic_MainView.Instance.Dispatcher.BeginInvoke(new Action(() => Classic_MainView.Instance.PlaylistCtl.Pl(id)));
        //}
    }
}
