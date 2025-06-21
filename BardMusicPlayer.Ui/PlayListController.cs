using BardMusicPlayer.Coffer;
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
            return BmpCoffer.Instance.GetPlaylist(id).Select(x => ApiSong.Create(x)).ToArray();;
        }
        [HttpPatch]
        public IHttpActionResult Patch(string id)
        {
            Classic_MainView.Instance.Dispatcher.BeginInvoke(new Action(() => Classic_MainView.Instance.PlaylistCtl.SelectPlayList(id)));
            return Ok();
        }
    }
}
