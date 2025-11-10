using System;
using Avalonia.MusicStore.Models;
using Avalonia.MusicStore.Views;

namespace Avalonia.MusicStore.ViewModels;

public partial class AlbumViewModel : ViewModelBase
{

    private readonly Album _album;

    public AlbumViewModel(Album album)
    {
        _album = album;
    }

    public string Artist => _album.Artist;
    public string Title => _album.Title;
}
