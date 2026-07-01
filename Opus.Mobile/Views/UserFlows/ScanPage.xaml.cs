using ZXing.Net.Maui;

namespace Opus.Mobile.Views.UserFlows;

public partial class ScanPage : ContentPage
{
    private readonly TaskCompletionSource<string?> taskCompletionSource = new();
    private bool navigationHandled;

    public Task<string?> Dismissed => taskCompletionSource.Task;

    public ScanPage()
	{

		InitializeComponent();

        cameraView.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormats.All,
            AutoRotate = false,
            Multiple = false,
            TryHarder = true
        };
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        if (!navigationHandled)
            taskCompletionSource.TrySetResult(null);
    }

    private void BarcodeDetected(object sender, BarcodeDetectionEventArgs e)
    {
        var result = e.Results?.FirstOrDefault()?.Value;

        if (string.IsNullOrWhiteSpace(result))
            return;

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            try
            {
                if (navigationHandled)
                    return;

                cameraView.IsDetecting = false;
                navigationHandled = true;

                await Navigation.PopModalAsync();
                taskCompletionSource.TrySetResult(result);
            }
            catch (Exception ex)
            {
                taskCompletionSource.TrySetException(ex);
            }
        });
    }

    private async void CancelButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            navigationHandled = true;

            await Navigation.PopModalAsync();
            taskCompletionSource.TrySetResult(null);
        }
        catch (Exception ex)
        {
            taskCompletionSource.TrySetException(ex);
        }
    }
}