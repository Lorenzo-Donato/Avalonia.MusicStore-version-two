using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Avalonia.MusicStore.Models
{
    public class Album
    {
        private static readonly HttpClient _http = new();

        public string Artist { get; set; }
        public string Title { get; set; }
        public string CoverUrl { get; set; }
        private static HttpClient s_httpClient = new();
        private string CachePath => $"./Cache/{SanitizeFileName(Artist)} - {SanitizeFileName(Title)}";


        public Album(string artist, string title, string coverUrl)
        {
            Artist = artist;
            Title = title;
            CoverUrl = coverUrl;
        }

        public static async Task<IEnumerable<Album>> SearchAsync(string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Enumerable.Empty<Album>();

            var url = $"https://itunes.apple.com/search?term={Uri.EscapeDataString(searchTerm)}&entity=album&limit=10";

            var response = await _http.GetStringAsync(url);
            using var doc = JsonDocument.Parse(response);

            var results = doc.RootElement.GetProperty("results");

            var albums = results.EnumerateArray().Select(x => new Album(
                x.GetProperty("artistName").GetString() ?? "Unknown Artist",
                x.GetProperty("collectionName").GetString() ?? "Unknown Album",
                (x.TryGetProperty("artworkUrl100", out var art)
                    ? art.GetString()?.Replace("100x100bb", "600x600bb")
                    : null) ?? ""
            )).ToList();

            return albums;
        }


        public async Task<Stream> LoadCoverBitmapAsync()
        {
            if (File.Exists(CachePath + ".bmp"))
            {
                return File.OpenRead(CachePath + ".bmp");
            }
            else
            {
                var data = await s_httpClient.GetByteArrayAsync(CoverUrl);
                return new MemoryStream(data);
            }
        }

        private static string SanitizeFileName(string input)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                input = input.Replace(c, '_');
            }
            return input;
        }


    }
}
