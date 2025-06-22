using BardMusicPlayer.Transmogrify.Song;
using System;


namespace BardMusicPlayer.Ui
{
    public class ApiSong
    {
        public string Title { get; set; }
        public string Id { get; set; }
        public TimeSpan Duration { get; set; } = default;
        public static ApiSong Create(BmpSong bmpSong)
        {
            if (bmpSong == null) return null;

            return new ApiSong
            {
                Title = bmpSong.Title,
                Id =  bmpSong.Id != null ? bmpSong.Id.ToString() : "",
                Duration = bmpSong.Duration,
            };
        }
    }
}
