using BardMusicPlayer.Transmogrify.Song;
using System;


namespace BardMusicPlayer.Ui
{
    public class ApiSong
    {
        public string Title { get; set; }
        public short Pid { get; set; }
        public TimeSpan Duration { get; set; } = default;
        public static ApiSong Create(BmpSong bmpSong)
        {
            if (bmpSong == null) return null;
            return new ApiSong
            {
                Pid = bmpSong.Id.Pid,
                Duration = bmpSong.Duration,
                Title = bmpSong.Title
            };
        }
    }
}
