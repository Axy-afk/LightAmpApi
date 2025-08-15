using BardMusicPlayer.Ui.Classic;
using BardMusicPlayer.Ui.Functions;
using System;
using Microsoft.AspNetCore.Mvc;

namespace BardMusicPlayer.Ui
{
    public class PlaybackController : ControllerBase
    {
        [HttpGet]
        public object Get()
        {
            return PlaybackFunctions.PlaybackState.ToString();
        }

        [HttpPatch]
        public IActionResult Patch(int id)
        {
            Classic_MainView.Instance.Dispatcher.BeginInvoke(new Action(() => {
                if (id == 0)
                {
                    Classic_MainView.Instance.PlaybackStopped(); // Updated to use PlaybackStopped
                }
                else
                {
                    Classic_MainView.Instance.PlaybackStarted(); // Updated to use PlaybackStarted
                }
            }));
            return Ok();
        }
    }
}
