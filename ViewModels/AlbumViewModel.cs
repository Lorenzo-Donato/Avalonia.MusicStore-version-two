using Avalonia.MusicStore.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Media.Imaging;
using System.Threading.Tasks;

namespace Avalonia.MusicStore.ViewModels;

public partial class AlbumViewModel : ViewModelBase
{
    [ObservableProperty] public partial Bitmap? Cover { get; private set; }

    private readonly Album _album;

    public AlbumViewModel(Album album)
    {
        _album = album;
    }

    public string Artist => _album.Artist;
    public string Title => _album.Title;


    public async Task LoadCover()
    {
        await using (var imageStream = await _album.LoadCoverBitmapAsync())
        {
            Cover = await Task.Run(() => Bitmap.DecodeToWidth(imageStream, 400));
        }
    }

}
