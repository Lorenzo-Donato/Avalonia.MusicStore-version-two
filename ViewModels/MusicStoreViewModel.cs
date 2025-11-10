using System;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.MusicStore.Models;

namespace Avalonia.MusicStore.ViewModels;

public partial class MusicStoreViewModel : ViewModelBase
{




    [ObservableProperty] public partial string? SearchText { get; set; }

    [ObservableProperty] public partial bool IsBusy { get; private set; }
    [ObservableProperty] public partial AlbumViewModel? SelectedAlbum { get; set; }

    public ObservableCollection<AlbumViewModel> SearchResults { get; } = new();



    private async Task DoSearch(string? term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return;

        IsBusy = true;
        SearchResults.Clear();

        try
        {
            var albums = await Album.SearchAsync(term);

            foreach (var album in albums)
            {
                var vm = new AlbumViewModel(album);
                SearchResults.Add(vm);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Search failed: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnSearchTextChanged(string? value)
    {
        _ = DoSearch(SearchText);
    }

}
