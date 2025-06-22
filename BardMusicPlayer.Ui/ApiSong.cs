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
                Title = bmpSong.Title,
                Duration = bmpSong.Duration,
                Pid = bmpSong.Id != null ? bmpSong.Id.Pid : (short)-1 // or any other placeholder/default
            };
        }
    }
}
