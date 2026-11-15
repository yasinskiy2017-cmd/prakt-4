using MovieCatalogApp.ViewModels;

namespace MovieCatalogApp;

public partial class MainPage : ContentPage
{
    private readonly MainPageModel _viewModel;

    public MainPage(MainPageModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        _viewModel.UpdateOrientation(width, height);
    }
}
