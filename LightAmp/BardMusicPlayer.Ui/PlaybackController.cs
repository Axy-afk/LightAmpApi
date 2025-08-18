using BardMusicPlayer.Ui.Classic;
using BardMusicPlayer.Ui.Functions;
using System;
using System.Web.Http;

namespace BardMusicPlayer.Ui
{
    public class PlaybackController : ApiController
    {
        [HttpGet]
        public object Get()
        {
            return PlaybackFunctions.PlaybackState.ToString();
        }
        [HttpPatch]
        public IHttpActionResult Patch(int id)
        {
            Classic_MainView.Instance.Dispatcher.BeginInvoke(new Action(() => {
                if (id == 0)
                {
                    Classic_MainView.Instance.Pause();
                }
                else
                {
                    Classic_MainView.Instance.Play();
                }
            }));
            return Ok();
        }
    }
}
